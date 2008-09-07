using System.ServiceProcess;

namespace AffinityChanger
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] { new AffinityChangerService() };
            ServiceBase.Run(ServicesToRun);
        }
    }
}