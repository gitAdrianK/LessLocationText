namespace LessLocationText
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;
    using HarmonyLib;
    using JumpKing.Mods;
    using JumpKing.PauseMenu;

    [JumpKingMod(IDENTIFIER)]
    public static class ModEntry
    {
        private const string IDENTIFIER = "Zebra.LessLocationText";
        private const string HARMONY_IDENTIFIER = "Zebra.LessLocationText.Harmony";
        private const string SETTINGS_FILE = "Zebra.LessLocationText.Settings.xml";

        private static string AssemblyPath { get; set; }
        public static Preferences Preferences { get; private set; }

        [MainMenuItemSetting]
        [PauseMenuItemSetting]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for JK")]
        public static ToggleDiscoverLocationText ToggleDiscover(object factory, GuiFormat format) => new ToggleDiscoverLocationText();

        [MainMenuItemSetting]
        [PauseMenuItemSetting]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for JK")]
        public static ToggleEnterLocationText ToggleEnter(object factory, GuiFormat format) => new ToggleEnterLocationText();

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

            var harmony = new Harmony(HARMONY_IDENTIFIER);

            var locationComp = AccessTools.TypeByName("JumpKing.MiscSystems.LocationText.LocationComp");

            var pollCurrent = locationComp.GetMethod("PollCurrent");
            var pollCurrentPatch = new HarmonyMethod(typeof(ModEntry).GetMethod(nameof(PollCurrentPatch)));
            _ = harmony.Patch(
                pollCurrent,
                postfix: pollCurrentPatch);

            var pollNewScreen = locationComp.GetMethod("PollNewScreen");
            var pollNewScreenPatch = new HarmonyMethod(typeof(ModEntry).GetMethod(nameof(PollNewScreenPatch)));
            _ = harmony.Patch(
                pollNewScreen,
                postfix: pollNewScreenPatch);
        }

        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Harmony naming convention")]
        public static void PollCurrentPatch(ref bool __result)
        {
            if (Preferences.ShouldHideEnter)
            {
                __result = false;
            }
        }

        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Harmony naming convention")]
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
