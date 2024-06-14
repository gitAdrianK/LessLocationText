using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LessLocationText
{
    public class Preferences : INotifyPropertyChanged
    {
        private bool _shouldHideDiscover = false;
        private bool _shouldHideEnter = false;

        public bool ShouldHideDiscover
        {
            get => _shouldHideDiscover;
            set
            {
                _shouldHideDiscover = value;
                OnPropertyChanged();
            }
        }

        public bool ShouldHideEnter
        {
            get => _shouldHideEnter;
            set
            {
                _shouldHideEnter = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
