using HarmonyLib;
using JumpKing.Mods;
using JumpKing.PauseMenu;
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

            Type locationComp = AccessTools.TypeByName("JumpKing.MiscSystems.LocationText.LocationComp");

            MethodInfo pollCurrent = locationComp.GetMethod("PollCurrent");
            HarmonyMethod pollCurrentPatch = new HarmonyMethod(AccessTools.Method(typeof(ModEntry), nameof(PollCurrentPatch)));
            harmony.Patch(
                pollCurrent,
                postfix: pollCurrentPatch
            );

            MethodInfo pollNewScreen = locationComp.GetMethod("PollNewScreen");
            HarmonyMethod pollNewScreenPatch = new HarmonyMethod(AccessTools.Method(typeof(ModEntry), nameof(PollNewScreenPatch)));
            harmony.Patch(
                pollNewScreen,
                postfix: pollNewScreenPatch
            );
        }

        public static void PollCurrentPatch(ref bool __result)
        {
            if (Preferences.ShouldHideEnter)
            {
                __result = false;
            }
        }

        public static void PollNewScreenPatch(ref bool __result)
        {
            if (Preferences.ShouldHideDiscover)
            {
                __result = false;
            }
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
