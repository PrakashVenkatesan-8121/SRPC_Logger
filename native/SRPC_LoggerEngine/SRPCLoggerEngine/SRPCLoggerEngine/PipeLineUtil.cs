using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace LoggerEngine
{
    public class PipeData
    {
        public string LoggerPath { get; set; }
        public string Log { get; set; }
    }

    public class PipeLineUtil
    {
        public Queue<PipeData> Jobs = new Queue<PipeData>();
        public string PipeName = "";
        public bool SetFree = false;

        public PipeLineUtil(string PipeName)
        {
            this.PipeName = PipeName;
        }
        public void SendPipeLineMessage(string PipeLineName, string PipeLineMessage)
        {
            using (NamedPipeServerStream pipeServer = new NamedPipeServerStream(PipeLineName, PipeDirection.Out))
            {
                Console.WriteLine("Pipe Server: Waiting for connection...");
                pipeServer.WaitForConnection();
                Console.WriteLine("Pipe Server: Connected.");

                using (StreamWriter sw = new StreamWriter(pipeServer))
                {
                    sw.AutoFlush = true;
                    sw.WriteLine(PipeLineMessage);
                }
            }
        }
        public void RecievePipeLineMessage()
        {
            while (!SetFree)
            {

                try
                {
                    using (var pipeClient = new NamedPipeClientStream(".", PipeName, PipeDirection.In))
                    {
                        WriteToFile("Attempting to connect to pipe...");
                        pipeClient.Connect();

                        using (var reader = new StreamReader(pipeClient))
                        {
                            while (true)
                            {
                                string message = reader.ReadLine();
                                if (message == null)
                                {
                                    WriteToFile("Connection closed by server. Reconnecting...");
                                    break;
                                }
                                WriteToFile("Client received: " + message);
                                PipeData CurrentPipeData = JsonConvert.DeserializeObject<PipeData>(message);
                                Jobs.Enqueue(CurrentPipeData);
                            }
                        }
                    }
                }
                catch (IOException)
                {
                    WriteToFile("Server is not available. Retrying in 1 second...");
                    System.Threading.Thread.Sleep(1000);
                }

            }
        }

        public static void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\PipiLineData" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.
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
