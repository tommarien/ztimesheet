using System;
using NHibernate.Cfg;
using NUnit.Framework;
using Timesheet.Data;

namespace Timesheet.Tests
{
    [TestFixture]
    public class MappingTests
    {
        private Configuration _configuration;

        [SetUp]
        public void Setup()
        {
            _configuration = new NHibernateConfigurationBuilder().Build();
        }

        [Test]
        public void it_can_query_all_mapped_classes()
        {
            var sessionFactory = _configuration.BuildSessionFactory();

            using (var session = sessionFactory.OpenSession())
            {
                foreach (var persistentClass in _configuration.ClassMappings)
                {
                    try
                    {
                        var query = session
                            .CreateQuery(string.Format("from {0}", persistentClass.MappedClass.Name))
                            .SetMaxResults(1);

                        query.List();
                    }
                    catch (Exception e)
                    {
                        Assert.Fail("Mapping issue encountered with {0}\r\n{1}", persistentClass.MappedClass.Name, e);
                    }
                }
            }
        }
    }
}