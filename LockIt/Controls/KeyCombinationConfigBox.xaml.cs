using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Collections.ObjectModel;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LockIt.Controls;

public sealed partial class KeyCombinationConfigBox : ContentDialog
{
    public ObservableCollection<string> Items
    {
        get; set;
    } = new ObservableCollection<string>();

    public KeyCombinationConfigBox(ObservableCollection<string>? Items = null)
    {
        InitializeComponent();
        if (Items != null)
        {
            this.Items = new ObservableCollection<string>(Items);
        }
    }

    private string GetKeyString(VirtualKey Key)
    {
        return Key switch
        {
            VirtualKey.Menu => "Alt",
            VirtualKey.Control => "Ctrl",
            VirtualKey.LeftWindows => "Win",
            VirtualKey.RightWindows => "Win",
            VirtualKey.CapitalLock => "Caps Lock",
            _ => Key.ToString()
        };
    }

    public void window_KeyPress(object sender, KeyRoutedEventArgs e)
    {
        string Key = GetKeyString(e.Key);
        if (Items.Count < 4 && !Items.Contains(Key))
        {
            Items.Add(Key);
        }
    }

    public void btnRecord_Click(object sender, RoutedEventArgs e)
    {
        txtInfo.Text = "Listening...";
        btnRecord.IsEnabled = false;
        this.panelDialog.PreviewKeyDown += window_KeyPress;
        Items.Clear();
    }
    public void btnReset_Click(object sender, RoutedEventArgs e)
    {
        txtInfo.Text = "Click the Record button to start recording the key combination";
        btnRecord.IsEnabled = true;
        this.panelDialog.PreviewKeyDown -= window_KeyPress;
        Items.Clear();
    }
}
