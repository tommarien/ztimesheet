using System.Configuration;
using Timesheet.ApplicationServices;
using Topshelf;

namespace Timesheet.Monitor
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            TopshelfExitCode exitCode = HostFactory.Run(
                config =>
                {
                    config.Service<FileMonitorService>(
                        s =>
                        {
                            s.ConstructUsing(f => new FileMonitorService(
                                ConfigurationManager.AppSettings["Monitor.Path"]
                                , ConfigurationManager.AppSettings["Monitor.Filter"]));
                            s.WhenStarted(f => f.Start());
                            s.WhenStopped(f => f.Stop());
                        });


                    config.RunAsLocalSystem();
                    config.SetServiceName("TimeSheetMonitor");
                    config.SetDisplayName("Timesheet monitor");
                });

            return (int) exitCode;
        }
    }
}