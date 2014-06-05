﻿using System;
using System.Configuration;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Timesheet.ApplicationServices.Excel;
using Timesheet.ApplicationServices.Monitor;

namespace Timesheet.ApplicationServices
{
    public class ApplicationServicesInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<FileMonitorService>()
                .LifestyleTransient());

            container.Register(Component.For<ConsolidatingFileSystemMonitor>()
                .DependsOn(Dependency.OnAppSettingsValue("path", "Monitor.Path"))
                .DependsOn(Dependency.OnAppSettingsValue("filter", "Monitor.Filter"))
                .DependsOn(Property.ForKey("GracePeriod").Eq(Convert.ToInt32(ConfigurationManager.AppSettings["Monitor.GracePeriod"])))
                .LifestyleTransient());

            container.Register(Component.For<TimeEntryReader>().LifestyleSingleton());
        }
    }
}