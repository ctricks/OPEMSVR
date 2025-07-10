using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;

namespace OPEMSVRCS
{
    public partial class Service1 : ServiceBase
    {
        Timer Timer = new Timer();
        int Interval = 10000; // 10000 ms = 10 seconds  
        public Service1()
        {
            InitializeComponent();
            this.ServiceName = "OPEMServices";
        }

        protected override void OnStart(string[] args)
        {
            WriteLog("Service has been started");
            Timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            Timer.Interval = Interval;
            Timer.Enabled = true;
        }


        private void StartOPEMSService()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;

            string DTRPath = Path.Combine(path, "DTR");
            string DTRDone = Path.Combine(path, "DONE\\" + DateTime.Now.Month.ToString());

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);



            WriteLog("OPEMS Checking Directory:" + path);

            Timer.Enabled = false;

            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(DTRPath);
                foreach(FileInfo fi in directoryInfo.GetFiles("*.csv").ToList())
                {
                    string CSVFile = fi.FullName;
                }

                if (!Directory.Exists(DTRDone))
                    Directory.CreateDirectory(DTRDone);

                WriteLog("OPEMS: Done Processing...");

                WriteLog("OPEMS: Backup file to Directory : " + DTRDone);

            }
            catch (Exception ex)
            {
                WriteLog("Error Exception:" + ex.Message.ToString());
            }
               

            Timer.Enabled = true;
        }

        private void OnElapsedTime(object sender, ElapsedEventArgs e)
        {
            WriteLog("{0} ms elapsed.");
        }

        protected override void OnStop()
        {
            Timer.Stop();
            WriteLog("Service has been stopped.");
        }
        private void WriteLog(string logMessage, bool addTimeStamp = true)
        {
            string AppName = "OPEMSRVCS";

            var path = AppDomain.CurrentDomain.BaseDirectory;
            
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var filePath = String.Format("{0}\\{1}_{2}.txt",
                path,
                ServiceName,
                DateTime.Now.ToString("yyyyMMdd", CultureInfo.CurrentCulture)
                );

            if (addTimeStamp)
                logMessage = String.Format("[{0}] - {1}",
                    DateTime.Now.ToString("HH:mm:ss", CultureInfo.CurrentCulture),
                    logMessage);

            File.AppendAllText(filePath, logMessage);
        }
    }
}
