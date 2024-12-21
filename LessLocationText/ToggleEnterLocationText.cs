namespace LessLocationText
{
    using JumpKing.PauseMenu.BT.Actions;

    public class ToggleEnterLocationText : ITextToggle
    {
        public ToggleEnterLocationText() : base(ModEntry.Preferences.ShouldHideEnter)
        {
        }

        protected override string GetName() => "Disable enter text";

        protected override void OnToggle()
            => ModEntry.Preferences.ShouldHideEnter = !ModEntry.Preferences.ShouldHideEnter;
    }
}
