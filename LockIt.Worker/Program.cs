namespace LockIt.Worker
{
    internal class Program
    {
        static Mutex mutex = new Mutex(true, "LockItWorker");

        [STAThread]
        static void Main(string[] args)
        {
            if (!mutex.WaitOne(TimeSpan.Zero, true))
                return;

            using (Listener listener = new Listener())
            {
                Application.Run();
            }
        }

    }

}
