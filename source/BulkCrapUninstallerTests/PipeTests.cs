using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class PipeTests
    {
        [TestMethod]
        public void PipeTest()
        {
            using (var server = new NamedPipeServerStream("UninstallAutomatizerDaemon", PipeDirection.In))
            using (var reader = new StreamReader(server))
            {

                using (var client = new NamedPipeClientStream(".", "UninstallAutomatizerDaemon", PipeDirection.Out))
                using (var writer = new StreamWriter(client))
                {
                    new Thread(() => {
                        
                                         server.WaitForConnection();
                                         while (true)
                                         {
                                             var l = reader.ReadLine();
                                             Debugger.Break();
                                         }
                    }).Start();
                    client.Connect();
                    
                    writer.WriteLine("test");
                    writer.WriteLine("231");
                    writer.WriteLine("3");
                    writer.Flush();

                    Thread.Sleep(100000);
                }
            }
        }

    }
}