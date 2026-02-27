using System.Runtime.InteropServices;

public class HotKeyWindow : NativeWindow
{
    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    // Modifier constants
    private const uint MOD_ALT = 0x0001;
    private const uint MOD_CONTROL = 0x0002;
    private const uint MOD_SHIFT = 0x0004;
    private const uint MOD_WIN = 0x0008;

    private const int HOTKEY_ID = 9000; // Unique ID for this hotkey

    public HotKeyWindow()
    {
        // Create a hidden window handle to receive messages
        this.CreateHandle(new CreateParams());

        // Register Ctrl + Shift + L (L key code is 0x4C)
        // You can find Virtual Key codes online (e.g., 0x4C for 'L')
        RegisterKeyCombination(["Ctrl", "Shift", "L"]);
    }

    private uint GetKeyCode(string Key)
    {
        return Key switch
        {
            "Ctrl" => MOD_CONTROL,
            "Alt" => MOD_ALT,
            "Shift" => MOD_SHIFT,
            "Win" => MOD_WIN,
            _ => 0,
        };
    }

    private uint GetModifires(string[] Keys)
    {
        uint modifire = 0;
        foreach (string Modifire in Keys)
        {
            if (Modifire == "Ctrl" || Modifire == "Shift" || Modifire == "Alt" || Modifire == "Win")
            {
                modifire = modifire | GetKeyCode(Modifire);
            }
        }
        return modifire;
    }

    private void RegisterKeyCombination(string[] Keys)
    {
        uint Modifires = GetModifires(Keys);
        uint Key = (uint)Keys.Last()[0];
        RegisterHotKey(this.Handle, HOTKEY_ID, Modifires, Key);
    }

    protected override void WndProc(ref Message m)
    {
        const int WM_HOTKEY = 0x0312;
        if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == HOTKEY_ID)
        {
            OnHotKeyPressed();
        }
        base.WndProc(ref m);
    }

    private void OnHotKeyPressed()
    {
        // 1. Update status so UI knows it was triggered
        var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
        settings.Values["LastActivation"] = DateTime.Now.ToString();

        // 2. Launch the UI App
        // Using the Protocol link is the cleanest way to bring a WinUI app to front
        var uri = new Uri("lockit-app://activate");
        _ = Windows.System.Launcher.LaunchUriAsync(uri);
    }

    public void Dispose()
    {
        UnregisterHotKey(this.Handle, HOTKEY_ID);
        this.DestroyHandle();
    }
}