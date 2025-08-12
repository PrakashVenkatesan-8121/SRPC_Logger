using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace CentralLogStreamHandler
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for CentralLogStreamHandler Windows Service.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Servicemain()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
