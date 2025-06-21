using System;
using System.ServiceProcess;
using System.Diagnostics;

namespace AttendanceSyncService
{
    static class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            // Run in debug mode as console
            Console.WriteLine("Debug mode: Starting service logic without installation...");

            var service = new EasyHMS();
            service.StartDebug(); // We'll define this method in the service

            Console.WriteLine("Service is running in debug mode. Press Enter to exit.");
            Console.ReadLine();

            service.Stop(); // Call Stop logic if needed
#else
            // Standard service startup
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new EasyHMS()
            };
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
