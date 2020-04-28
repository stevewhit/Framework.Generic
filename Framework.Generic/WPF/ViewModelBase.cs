using Framework.Generic.WPF;
using System.ComponentModel;
using System.Windows;

namespace Framework.Generic.WPF
{
    public abstract class ViewModelBase : ObservableBase
    {
        public bool IsInDesignMode()
        {
            return Application.Current.MainWindow == null
                ? true
                : DesignerProperties.GetIsInDesignMode(Application.Current.MainWindow);
        }
    }
}
