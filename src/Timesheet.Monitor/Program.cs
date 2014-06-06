using System.Configuration;
using Castle.Facilities.Logging;
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
                    config.Service<TimesheetProcessingService>(
                        s =>
                        {
                            s.ConstructUsing(f => container.Resolve<TimesheetProcessingService>());

                            s.WhenStarted(f => f.Start());
                            s.WhenStopped(f => f.Stop());

                            s.WhenShutdown((service, host) => container.Release(service));
                        });


                    config.RunAsLocalSystem();
                    config.SetServiceName(ConfigurationManager.AppSettings["TopShelf.Service.Name"]);
                    config.SetDisplayName(ConfigurationManager.AppSettings["TopShelf.Service.DisplayName"]);
                    config.SetDescription(ConfigurationManager.AppSettings["TopShelf.Service.Description"]);
                });

            return (int) exitCode;
        }

        private static IWindsorContainer CreateContainer()
        {
            var container = new WindsorContainer();

            container.AddFacility<LoggingFacility>(f => f.UseNLog().WithAppConfig());

            container.Install(new DataInstaller(), new ApplicationServicesInstaller());

            return container;
        }
    }
}