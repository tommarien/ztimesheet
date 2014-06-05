using Castle.Windsor;
using Timesheet.ApplicationServices;
using Timesheet.Data;
using Topshelf;

namespace Timesheet.Monitor
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            IWindsorContainer container = CreateContainer();

            TopshelfExitCode exitCode = HostFactory.Run(
                config =>
                {
                    config.Service<FileMonitorService>(
                        s =>
                        {
                            s.ConstructUsing(f => container.Resolve<FileMonitorService>());

                            s.WhenStarted(f => f.Start());
                            s.WhenStopped(f => f.Stop());

                            s.WhenShutdown((service, host) => container.Release(service));
                        });


                    config.RunAsLocalSystem();
                    config.SetServiceName("TimeSheetMonitor");
                    config.SetDisplayName("Timesheet monitor");
                });

            return (int) exitCode;
        }

        private static IWindsorContainer CreateContainer()
        {
            var container = new WindsorContainer();

            container.Install(new DataInstaller(), new ApplicationServicesInstaller());

            return container;
        }
    }
}