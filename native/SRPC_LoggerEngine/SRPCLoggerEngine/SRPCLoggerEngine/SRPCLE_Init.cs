using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace SRPCLoggerEngine
{
    static class SRPCLE_Init
    {
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new LoggerEngineService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
