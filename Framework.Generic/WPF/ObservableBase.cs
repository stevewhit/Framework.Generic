using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Framework.Generic.WPF
{
    public abstract class ObservableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void Set<TValue>(ref TValue field, TValue newValue, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<TValue>.Default.Equals(field, default) || !field.Equals(newValue))
            {
                field = newValue;
                RaisePropertyChanged(propertyName);
            }
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
