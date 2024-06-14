using HarmonyLib;
using JumpKing.Mods;
using JumpKing.PauseMenu;
using LanguageJK;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace LessLocationText
{
    [JumpKingMod(IDENTIFIER)]
    public static class ModEntry
    {
        const string IDENTIFIER = "Zebra.LessLocationText";
        const string HARMONY_IDENTIFIER = "Zebra.LessLocationText.Harmony";
        const string SETTINGS_FILE = "Zebra.LessLocationText.Settings.xml";

        private static string AssemblyPath { get; set; }
        public static Preferences Preferences { get; private set; }

        private static ConstructorInfo CenteredLocationText { get; set; }
        private static ConstructorInfo LocationNotification { get; set; }

        [MainMenuItemSetting]
        [PauseMenuItemSetting]
        public static ToggleDiscoverLocationText ToggleDiscover(object factory, GuiFormat format)
        {
            return new ToggleDiscoverLocationText();
        }

        [MainMenuItemSetting]
        [PauseMenuItemSetting]
        public static ToggleEnterLocationText ToggleEnter(object factory, GuiFormat format)
        {
            return new ToggleEnterLocationText();
        }

        /// <summary>
        /// Called by Jump King before the level loads
        /// </summary>
        [BeforeLevelLoad]
        public static void BeforeLevelLoad()
        {
            //Debugger.Launch();
            AssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            try
            {
                Preferences = XmlSerializerHelper.Deserialize<Preferences>($@"{AssemblyPath}\{SETTINGS_FILE}");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"[ERROR] [{IDENTIFIER}] {e.Message}");
                Preferences = new Preferences();
            }
            Preferences.PropertyChanged += SaveSettingsOnFile;

            Harmony harmony = new Harmony(HARMONY_IDENTIFIER);

            Type locationTextManager = AccessTools.TypeByName("JumpKing.MiscSystems.LocationText.LocationTextManager");
            CenteredLocationText = AccessTools.TypeByName("JumpKing.MiscSystems.LocationText.CenteredLocationText").GetConstructor(new[] { typeof(string) });
            LocationNotification = AccessTools.TypeByName("JumpKing.MiscSystems.LocationText.LocationNotification").GetConstructor(new[] { typeof(string) });

            MethodInfo locationTextManagerUpdate = locationTextManager.GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Instance);
            HarmonyMethod customUpdate = new HarmonyMethod(AccessTools.Method(typeof(ModEntry), nameof(CustomUpdate)));
            harmony.Patch(
                locationTextManagerUpdate,
                prefix: customUpdate
            );
        }

        public static bool CustomUpdate(object __instance)
        {
            if (!Preferences.ShouldHideDiscover && !Preferences.ShouldHideEnter)
            {
                return true;
            }

            var locationComp = Traverse.Create(__instance).Field("m_location_comp").GetValue();
            Type typeLocationComp = locationComp.GetType();
            var pollNewScreen = AccessTools.Method(typeLocationComp, "PollNewScreen");
            var pollCurrent = AccessTools.Method(typeLocationComp, "PollCurrent");

            object[] parameters = new object[] { null };
            if ((bool)pollNewScreen.Invoke(locationComp, parameters))
            {
                if (!Preferences.ShouldHideDiscover)
                {
                    string @string = language.ResourceManager.GetString((string)parameters[0]);
                    if (@string != null)
                    {
                        CenteredLocationText.Invoke(new object[] { @string });

                    }
                    else
                    {
                        CenteredLocationText.Invoke(new object[] { (string)parameters[0] });
                    }
                }
            }

            if ((bool)pollCurrent.Invoke(locationComp, parameters))
            {
                if (!Preferences.ShouldHideEnter)
                {
                    string string2 = language.ResourceManager.GetString((string)parameters[0]);
                    if (string2 != null)
                    {
                        LocationNotification.Invoke(new object[] { string2 });
                    }
                    else
                    {
                        LocationNotification.Invoke(new object[] { (string)parameters[0] });
                    }
                }
            }
            return false;
        }

        private static void SaveSettingsOnFile(object sender, System.ComponentModel.PropertyChangedEventArgs args)
        {
            try
            {
                XmlSerializerHelper.Serialize($@"{AssemblyPath}\{SETTINGS_FILE}", Preferences);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"[ERROR] [{IDENTIFIER}] {e.Message}");
            }
        }
    }
}
