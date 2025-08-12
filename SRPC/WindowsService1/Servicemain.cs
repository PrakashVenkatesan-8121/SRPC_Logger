using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace CentralLogStreamHandler
{
    /// <summary>
    /// CentralLogStreamHandler: Windows Service for centralized, concurrent log stream handling via IPC.
    /// </summary>
    public partial class Servicemain : ServiceBase
    {
        private Thread ipcThread;
        private Thread logThread;
        private bool running;
        private const string PipeName = "CentralLogStreamHandler";
        private const string DefaultLogFilePath = "C://CentralLogStreamHandler.log";
        private const int MaxConcurrentConnections = 10; // You can adjust this
        private ConcurrentQueue<(string logPath, string message, string logType)> logQueue = new ConcurrentQueue<(string, string, string)>();

        public Servicemain()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            running = true;
            ipcThread = new Thread(StartIpcListenerPool);
            ipcThread.IsBackground = true;
            ipcThread.Start();
            logThread = new Thread(ProcessLogQueue);
            logThread.IsBackground = true;
            logThread.Start();
        }

        protected override void OnStop()
        {
            running = false;
            ipcThread?.Join(2000);
            logThread?.Join(2000);
        }

        private void StartIpcListenerPool()
        {
            // Start multiple listeners for concurrent connections
            var threads = new List<Thread>();
            for (int i = 0; i < MaxConcurrentConnections; i++)
            {
                var t = new Thread(StartIpcListener);
                t.IsBackground = true;
                t.Start();
                threads.Add(t);
            }
            // Wait for all threads to finish when stopping
            foreach (var t in threads)
            {
                t.Join();
            }
        }

        private void StartIpcListener()
        {
            while (running)
            {
                using (var server = new NamedPipeServerStream(PipeName, PipeDirection.In, MaxConcurrentConnections, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
                {
                    try
                    {
                        server.WaitForConnection();
                        using (var reader = new StreamReader(server))
                        {
                            string message = reader.ReadToEnd();
                            EnqueueJsonLog(message);
                        }
                    }
                    catch (Exception ex)
                    {
                        EnqueueLog(DefaultLogFilePath, $"Exception: {ex.Message}", "Critical");
                    }
                }
            }
        }

        private void EnqueueJsonLog(string json)
        {
            try
            {
                var serializer = new JavaScriptSerializer();
                var obj = serializer.Deserialize<Dictionary<string, string>>(json);
                string logMessage = obj.ContainsKey("log") ? obj["log"] : "";
                string logPath = obj.ContainsKey("log_path") ? obj["log_path"] : DefaultLogFilePath;
                string logType = obj.ContainsKey("LogType") ? obj["LogType"] : "Normal";
                EnqueueLog(logPath, logMessage, logType);
            }
            catch (Exception ex)
            {
                EnqueueLog(DefaultLogFilePath, $"JSON Parse Exception: {ex.Message}\nRaw: {json}", "Critical");
            }
        }

        private void EnqueueLog(string logPath, string message, string logType)
        {
            logQueue.Enqueue((logPath, message, logType));
        }

        private void ProcessLogQueue()
        {
            const int batchSize = 20; // Number of log entries per batch
            const int idleDelayMs = 10; // Delay when queue is empty
            while (running)
            {
                var batch = new List<(string logPath, string message, string logType)>();
                while (batch.Count < batchSize && logQueue.TryDequeue(out var logItem))
                {
                    batch.Add(logItem);
                }
                if (batch.Count > 0)
                {
                    // Group log entries by file path
                    var logsByPath = batch.GroupBy(x => x.logPath);
                    var tasks = new List<Task>();
                    foreach (var group in logsByPath)
                    {
                        var logPath = group.Key;
                        var sb = new StringBuilder();
                        foreach (var item in group)
                        {
                            sb.AppendLine($"{DateTime.Now} [{item.logType}]: {item.message}");
                        }
                        tasks.Add(WriteLogAsync(logPath, sb.ToString()));
                    }
                    try
                    {
                        Task.WaitAll(tasks.ToArray());
                    }
                    catch (Exception ex)
                    {
                        // Log the reason for log failure in the default log file
                        try
                        {
                            File.AppendAllText(DefaultLogFilePath, $"{DateTime.Now} [Critical]: Log batch failure. Reason: {ex.Message}\r\n");
                        }
                        catch { /* Ignore logging errors */ }
                    }
                }
                else
                {
                    Thread.Sleep(idleDelayMs); // Only sleep if queue is empty
                }
            }
        }

        private Task WriteLogAsync(string logPath, string content)
        {
            return Task.Run(() =>
            {
                try
                {
                    using (var fs = new FileStream(logPath, FileMode.Append, FileAccess.Write, FileShare.Read))
                    using (var sw = new StreamWriter(fs))
                    {
                        sw.Write(content);
                    }
                }
                catch (Exception ex)
                {
                    // Log the reason for log failure in the default log file
                    try
                    {
                        File.AppendAllText(DefaultLogFilePath, $"{DateTime.Now} [Critical]: Log failure for path '{logPath}'. Reason: {ex.Message}\r\n");
                    }
                    catch { /* Ignore logging errors */ }
                }
            });
        }
    }
}
