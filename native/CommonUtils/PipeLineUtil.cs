using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;

namespace LoggerEngine
{
    public class PipeData
    {
        public string LoggerPath { get; set; }
        public string Log { get; set; }
    }
    public static class PipeLineUtil
    {
        public static Queue<PipeData> Jobs = new Queue<PipeData>();
        public static void SendPipeLineMessage(string PipeLineName, string PipeLineMessage)
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
        public static void RecievePipeLineMessage(string PipeLineName)
        {
            while (true)
            {
                try
                {
                    using (var pipeClient = new NamedPipeClientStream(".", PipeLineName, PipeDirection.In))
                    {
                        Console.WriteLine("Attempting to connect to pipe...");
                        pipeClient.Connect();
                        Console.WriteLine("Connected to pipe.");

                        using (var reader = new StreamReader(pipeClient))
                        {
                            while (true)
                            {
                                string message = reader.ReadLine();
                                if (message == null)
                                {
                                    Console.WriteLine("Connection closed by server. Reconnecting...");
                                    break;
                                }
                                Console.WriteLine("Client received: " + message);
                                PipeData CurrentPipeData = JsonConvert.DeserializeObject<PipeData>(message);
                                Jobs.Enqueue(CurrentPipeData);
                            }
                        }
                    }
                }
                catch (IOException)
                {
                    Console.WriteLine("Server is not available. Retrying in 1 second...");
                    System.Threading.Thread.Sleep(1000);
                }

            }
        }
    }
}
