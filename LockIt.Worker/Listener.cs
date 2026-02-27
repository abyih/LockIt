using System.Runtime.InteropServices;
using Timer = System.Threading.Timer;
public class Listener : NativeWindow, IDisposable
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

    private Timer _intervalTimer;
    private TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

    public Listener()
    {
        this.CreateHandle(new CreateParams());
        RegisterKeyCombination(["Ctrl", "Shift", "L"]);
        _intervalTimer = new Timer(OnTimerTick, null, TimeSpan.Zero, _checkInterval);
    }
    private void OnTimerTick(object? state)
    {
        //Lock
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
        _intervalTimer.Change(TimeSpan.Zero, _checkInterval);
    }

    public void Dispose()
    {
        UnregisterHotKey(this.Handle, HOTKEY_ID);
        this.DestroyHandle();
        _intervalTimer?.Dispose();
    }
}