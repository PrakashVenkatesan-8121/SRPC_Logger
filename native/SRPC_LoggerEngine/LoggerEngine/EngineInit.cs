using System;
using System.Collections.Generic;
using System.Threading;

namespace LoggerEngine
{
 
    class EngineInit
    {

        static void InitListener()
        {
            PipeLineUtil.RecievePipeLineMessage("Zoho_Assist");
        }
        static void Main(string[] args)
        {
            Thread ListenerThread = new Thread(InitListener);
            Thread JobProcessor = new Thread(ProcessListenerJob);
            ListenerThread.Start();
            JobProcessor.Start();
            ListenerThread.Join();
            JobProcessor.Join();

            string name = Console.ReadLine();
        }

        static void ProcessListenerJob()
        {
            while(true)
            {
                while(PipeLineUtil.Jobs.Count > 0)
                {
                    PipeData job = PipeLineUtil.Jobs.Dequeue();
                    FileOperator.WriteLog(job.LoggerPath,job.Log);
                }
            }
        }
    }
}
