using Windows.Storage;

namespace LockIt.Worker
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var LocalSettings = ApplicationData.Current.LocalSettings;
            bool IsActive = (bool)LocalSettings.Values["IsActive"] == true;

            if (IsActive)
            {
                while (true)
                {
                    int Interval = (int)LocalSettings.Values["Interval"];

                    await Task.Delay(Interval);
                }
            }
        }

    }

}
