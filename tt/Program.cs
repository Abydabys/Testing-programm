namespace tt
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            
            var serviceContainer = new ServiceContainer();
            
            Application.Run(new Form1());
        }
    }
}