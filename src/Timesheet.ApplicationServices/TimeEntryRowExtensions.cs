using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Timesheet.ApplicationServices.Excel;

namespace Timesheet.ApplicationServices
{
    public static class TimeEntryRowExtensions
    {
        public static string GenerateChecksum(this IEnumerable<TimeEntryRow> timeEntryRows)
        {
            if (!timeEntryRows.Any()) return null;

            var jsonString = JsonConvert.SerializeObject(timeEntryRows);

            using (var provider = new MD5CryptoServiceProvider())
            {
                byte[] hashBytes = provider.ComputeHash(Encoding.UTF8.GetBytes(jsonString));

                string base64String = Convert.ToBase64String(hashBytes, 0, hashBytes.Length);

                return base64String;
            }
        }
    }
}