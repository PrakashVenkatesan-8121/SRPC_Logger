using System;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace SRPCLoggerEngine
{
    public partial class LoggerEngineService : ServiceBase
    {
        private CancellationTokenSource _cancellationTokenSource;
        private Task _backgroundTask;
        public InitEngine EngineKey = new InitEngine("ZohoAssist");
        public LoggerEngineService()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _backgroundTask = Task.Run(() => MainLoop(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
        }

        private void MainLoop(CancellationToken cancellationToken)
        {
            EngineKey.startEngine();
            while (!cancellationToken.IsCancellationRequested)
            {
                WriteToFile("Service is heartbeat running at " + DateTime.Now);
                Thread.Sleep(2000);
            }

            Console.WriteLine("Service stopped.");
        }

        protected override void OnStop()
        {
            EngineKey.stopEngine();

            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
            }

            if (_backgroundTask != null)
            {
                _backgroundTask.Wait();
            }
            
            WriteToFile("Service is stopped by pk at " + DateTime.Now);
        }
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            WriteToFile("Service is heartbeat at " + DateTime.Now);
        }
        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }
    }
}
