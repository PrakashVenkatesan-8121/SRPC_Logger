
namespace LoggerEngine
{
    class FileOperator
    {
        public static readonly object fileLock = new object();

        private static void WriteLogLine(string text,string logFileName ,bool append = true)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }
            lock (fileLock)
            {
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(logFileName, append, System.Text.Encoding.UTF8))
                {
                    writer.WriteLine(text);
                }
            }
        }

        public static bool WriteLog(string logFileName, string log)
        {
            WriteLogLine(log, logFileName,true);
            return false;
        }

    }
}
