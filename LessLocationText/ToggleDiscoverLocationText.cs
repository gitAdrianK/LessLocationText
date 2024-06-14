using JumpKing.PauseMenu.BT.Actions;

namespace LessLocationText
{
    public class ToggleDiscoverLocationText : ITextToggle
    {
        public ToggleDiscoverLocationText() : base(ModEntry.Preferences.ShouldHideDiscover)
        {
        }

        protected override string GetName() => "Hide discover text";

        protected override void OnToggle()
        {
            ModEntry.Preferences.ShouldHideDiscover = !ModEntry.Preferences.ShouldHideDiscover;
        }
    }
}