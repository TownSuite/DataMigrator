using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace DataMigrator.Tables
{
    public class Filtered
    {

        private Filtered()
        {
        }

        private IEnumerable<string> ignoreTables;
        public Filtered(IEnumerable<string> ignoreTables)
        {
            this.ignoreTables = ignoreTables;
        }

        public IEnumerable<string> Values
        {
            get
            {
                var filter = new Filters();
                var data = Poco.TableName.Retrieve();
                var extraFilters = ignoreTables.Where(p => p.EndsWith("*")).Select(p => p.Replace("*", ""));
                var keepList = filter.KeepList(data, ignoreTables, extraFilters).ToList();
                filter.SetTableOrder(keepList);

                return keepList;
            }
        }

    }
}
