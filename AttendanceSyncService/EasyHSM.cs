using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace AttendanceSyncService
{
    public partial class EasyHMS : ServiceBase
    {
        private Timer timer1 = null;
        public EasyHMS()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            timer1 = new Timer();
            this.timer1.Interval = 600000; //every 10 mint

            //this.timer1.Elapsed += new System.Timers.ElapsedEventHandler(this.timer1_Tick);
            //timer1.Enabled = true;
            var zktService = new ZKT_Device.ZKT_Service.ZKTService();
            zktService.GetAttendanceFromMachine(); // This already has async inside 
            Library.WriteErrorLog("Test window service started");
        }
        //public static void StartService(string serviceName, int timeoutMilliseconds)
        //{
        //    ServiceController service = new ServiceController(serviceName);
        //    try
        //    {
        //        TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

        //        service.Start();
        //        service.WaitForStatus(ServiceControllerStatus.Running, timeout);
        //    }
        //    catch
        //    {
        //        // ...
        //    }
        //}
        private void timer1_Tick(object sender, ElapsedEventArgs e)
        {
            Library.InsertTestData();            
        }
        protected override void OnStop()
        {
            timer1.Enabled = false;
            Library.WriteErrorLog("Test window service stopped");
        }
        public void StartDebug()
        {
            OnStart(null); // Manually trigger your service's start logic
        }

    }
}
