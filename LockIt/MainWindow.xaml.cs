using LockIt.Controls;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Windows.Storage;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LockIt
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SetWindowSizeAndPosition(900, 1200);
            SetUpSavedKeys();
        }

        private void SetUpSavedKeys()
        {
            var LocalSettings = ApplicationData.Current.LocalSettings;

            if (LocalSettings.Values.ContainsKey("SavedKeys") && LocalSettings.Values["SavedKeys"] != null)
            {
                Items = new ObservableCollection<string>(((string)LocalSettings.Values["SavedKeys"]).Split(",").ToList());
            }
        }

        private ObservableCollection<string> _items = new ObservableCollection<string>();

        public ObservableCollection<string> Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
            }
        }


        private void SetWindowSizeAndPosition(int Width, int Height)
        {
            var WindowHandle = WindowNative.GetWindowHandle(this);
            var WindowID = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(WindowHandle);
            var ApplicationWindow = AppWindow.GetFromWindowId(WindowID);

            AppWindow.Resize(new Windows.Graphics.SizeInt32(Width, Height));

            var DisplayAreaSize = DisplayArea.GetFromWindowId(WindowID, DisplayAreaFallback.Primary);
            var CenterX = (DisplayAreaSize.WorkArea.Width - Width) / 2;
            var CenterY = (DisplayAreaSize.WorkArea.Height - Height) / 2;
        }

        private void ShowSuccessToast(string Message = "")
        {
            if (!string.IsNullOrEmpty(Message))
            {
                CombinationSaveSuccessToast.Message = Message;
            }
            CombinationSaveSuccessToast.IsOpen = true;
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            timer.Tick += (s, e) =>
            {
                CombinationSaveSuccessToast.IsOpen = false;
                timer.Stop();
            };
            timer.Start();
        }

        private async void btnChangeCombination_Click(object sender, RoutedEventArgs e)
        {
            KeyCombinationConfigBox dialog = new KeyCombinationConfigBox(Items);
            dialog.XamlRoot = this.Content.XamlRoot;
            ContentDialogResult result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var LocalSettings = ApplicationData.Current.LocalSettings;

                if (dialog.Items.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine(dialog.Items);
                    Items = dialog.Items;
                    this.customKeyBox.Items = Items;

                    LocalSettings.Values["SavedKeys"] = string.Join(",", Items);
                    ShowSuccessToast();
                }
            }
        }

        private void StartBackgroundWorker()
        {
            if (!Process.GetProcessesByName("LockIt.Worker").Any())
            {
                string WorkerPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LockIt.Worker", "LockIt.Worker.exe");
                if (Path.Exists(WorkerPath))
                    Process.Start(WorkerPath);
            }
        }

        private void StopBackgroundWorker()
        {
            if (!Process.GetProcessesByName("LockIt.Worker").Any())
            {
            }
        }

        private async void btnSaveConfiguration_Click(object sender, RoutedEventArgs e)
        {
            var LocalSettings = ApplicationData.Current.LocalSettings;

            bool IsActive = checkActive.IsChecked == true;
            bool StartWithWindows = checkStartWithWindows.IsChecked == true;
            int Interval = (int)numInputInterval.Value;

            if (StartWithWindows)
            {
                //LocalSettings.Values["StartWithWindows"] = true;
            }

            if (IsActive)
            {
                StartBackgroundWorker();
            }
            else
            {
                StopBackgroundWorker();
            }
        }
    }
}
