namespace LessLocationText
{
    using JumpKing.PauseMenu.BT.Actions;

    public class ToggleDiscoverLocationText : ITextToggle
    {
        public ToggleDiscoverLocationText() : base(ModEntry.Preferences.ShouldHideDiscover)
        {
        }

        protected override string GetName() => "Disable discover text";

        protected override void OnToggle()
            => ModEntry.Preferences.ShouldHideDiscover = !ModEntry.Preferences.ShouldHideDiscover;
    }
}
