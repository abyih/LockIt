using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LockIt.Controls
{
    public sealed partial class KeyCombinationBox : UserControl
    {
        public KeyCombinationBox()
        {
            InitializeComponent();
            System.Diagnostics.Debug.WriteLine("Hello");
            System.Diagnostics.Debug.WriteLine(Items);
        }

        public ObservableCollection<string> Items
        {
            get
            {
                return (ObservableCollection<string>)GetValue(ItemsProperty);
            }
            set
            {
                SetValue(ItemsProperty, value);
            }
        }

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(nameof(Items), typeof(ObservableCollection<string>), typeof(KeyCombinationBox), new PropertyMetadata(null));
    }
}
