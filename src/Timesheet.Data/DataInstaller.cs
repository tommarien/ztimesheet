using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using NHibernate;

namespace Timesheet.Data
{
    public class DataInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var sessionFactory = new NHibernateConfigurationBuilder()
                .Build()
                .BuildSessionFactory();

            container.Register(Component.For<ISessionFactory>()
                .Instance(sessionFactory)
                .LifestyleSingleton());
        }
    }
}