using tt.Client;

namespace tt
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            
            var serviceContainer = new NetworkServiceContainer();
            
            Application.Run(new Form1());
        }
    }
}