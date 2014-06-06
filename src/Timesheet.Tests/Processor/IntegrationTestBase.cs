using NHibernate;
using NUnit.Framework;
using Timesheet.Data;
using Timesheet.Domain.Entities;

namespace Timesheet.Tests.Processor
{
    public abstract class IntegrationTestBase
    {
        public ISessionFactory SessionFactory { get; set; }

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            BeforeTestFixtureSetup();

            InitNHibernateSessionFactory();

            AfterTestFixtureSetup();
        }

        private void InitNHibernateSessionFactory()
        {
            var configuration = new NHibernateConfigurationBuilder().Build();
            SessionFactory = configuration.BuildSessionFactory();
        }

        protected virtual void AfterTestFixtureSetup()
        {
        }

        protected virtual void BeforeTestFixtureSetup()
        {
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            BeforeTestFixtureTearDown();

            CleanupDatabase();
            if (SessionFactory != null) SessionFactory.Dispose();

            AfterTestFixtureTearDown();
        }

        protected void CleanupDatabase()
        {
            using (var session = SessionFactory.OpenSession())
            {
                session.CreateQuery("Delete from " + typeof (TimeEntry).Name).ExecuteUpdate();
                session.CreateQuery("Delete from " + typeof(Partition).Name).ExecuteUpdate();
            }
        }

        protected virtual void BeforeTestFixtureTearDown()
        {
        }

        protected virtual void AfterTestFixtureTearDown()
        {
        }

        [TearDown]
        public void TearDown()
        {
            CleanupDatabase();
        }
    }
}