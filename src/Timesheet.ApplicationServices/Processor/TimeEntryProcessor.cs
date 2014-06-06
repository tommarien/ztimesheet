using System;
using System.Data;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using Timesheet.ApplicationServices.Excel;
using Timesheet.Domain;
using Timesheet.Domain.Entities;

namespace Timesheet.ApplicationServices.Processor
{
    public class TimeEntryProcessor
    {
        private const int DefaultBatchSize = 100;
        private readonly ISessionFactory _sessionFactory;

        public TimeEntryProcessor(ISessionFactory sessionFactory)
        {
            if (sessionFactory == null) throw new ArgumentNullException("sessionFactory");
            _sessionFactory = sessionFactory;
        }

        public void Process(TimeEntryRow[] rows)
        {
            if (rows == null) throw new ArgumentNullException("rows");

            var impactedPartitions =
                   (from row in rows
                    group row by new PartitionKey(row.Date, row.User)
                    into partition
                    select new
                    {
                        PartitionKey = partition.Key,
                        Rows = partition.ToList(),
                        Checksum = partition.GenerateChecksum()
                    })
                    .ToList();

            var impactedPartitionKeys = impactedPartitions.Select(p => p.PartitionKey).ToList();
            using (var session = _sessionFactory.OpenStatelessSession().SetBatchSize(DefaultBatchSize))
            using (var tx = session.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    var storedPartitions = session.Query<Partition>()
                                .Where(p => impactedPartitionKeys.Contains(p.Key))
                                .ToList();

                    var modifiedPartitions = impactedPartitions.Where(p =>
                        !storedPartitions.Any(x => Equals(x.Key, p.PartitionKey)) ||
                        storedPartitions.Single(x => Equals(x.Key, p.PartitionKey)).Checksum != p.Checksum);

                    foreach (var partition in modifiedPartitions)
                    {
                        session.CreateQuery("delete from TimeEntry where PartitionKey = ?")
                            .SetParameter(0, partition.PartitionKey)
                            .ExecuteUpdate();

                        var entries = partition.Rows.Select(r => r.CreateTimeEntry());
                        foreach (var entry in entries)
                        {
                            session.Insert(entry);
                        }

                        if (!storedPartitions.Any(x => Equals(x.Key, partition.PartitionKey)))
                        {
                            session.Insert(new Partition { Key = partition.PartitionKey, Checksum = partition.Checksum });
                        }
                        else
                        {
                            var storePartition = storedPartitions.Single(x => Equals(x.Key, partition.PartitionKey));
                            storePartition.Checksum = partition.Checksum;
                            session.Update(storePartition);
                        }
                    }

                    tx.Commit();
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
        }
    }
}