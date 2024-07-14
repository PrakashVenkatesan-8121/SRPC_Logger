using LoggerEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace SRPCLoggerEngine
{
    public class InitEngine
    {
        public static PipeLineUtil PipeLineHandle ;
        public Thread ListenerThread = new Thread(InitListener);
        public Thread JobProcessor = new Thread(ProcessListenerJob);
        public string PipeName = "IDon'tKnow";
        static void InitListener()
        {
            PipeLineHandle.RecievePipeLineMessage();
        }

        public InitEngine(string PipeName){ this.PipeName = PipeName; PipeLineHandle  = new PipeLineUtil(this.PipeName); }

        public static void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\InitEngine" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
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

        public void startEngine()
        {
            WriteToFile("Start ENgine");
            ListenerThread.Start();
            JobProcessor.Start();
        }

        public void stopEngine()
        {
            WriteToFile("Stop ENgine");
            PipeLineHandle.SetFree = true;
            ListenerThread.Abort();
            ListenerThread.Join();
            Thread.Sleep(1000);
            JobProcessor.Join();
        }

        static void ProcessListenerJob()
        {
            try
            {
                while (!PipeLineHandle.SetFree)
                {
                    WriteToFile("ProcessLister running");
                    Thread.Sleep(100);
                    while (PipeLineHandle.Jobs.Count > 0)
                    {
                        WriteToFile("ProcessLister PipeLineHandle.Jobs.Count > 0");
                        PipeData job = PipeLineHandle.Jobs.Dequeue();
                        FileOperator.WriteLog(job.LoggerPath, job.Log);
                        //FileOperator.WriteLog(@"D:\pk\PK_DEVS\SRPC_Logger\native\SRPC_LoggerEngine\SRPCLoggerEngine\SRPCLoggerEngine\bin\Debug\Logs\pk.txt", job.Log);
                    }
                }
            }catch(Exception ex)
            {
                WriteToFile($"An unknown exception occurred: {ex.Message}");
            }
        }
    }
}
