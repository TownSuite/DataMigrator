using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMigrator.Tables
{
    public class Filters
    {
        public void SetTableOrder(List<string> keepList)
        {
            var tableOrder = Program.TableOrder.Reverse();

            foreach (var table in tableOrder)
            {
                var foundTable = keepList.Where(p => p == table).Select(p => p).FirstOrDefault();
                if (string.IsNullOrWhiteSpace(foundTable)) continue;

                keepList.Remove(foundTable);
                keepList.Insert(0, foundTable);
            }
        }

        public IEnumerable<string> KeepList(IEnumerable<Poco.TableName> original, IEnumerable<string> ignoreList,
            IEnumerable<string> extraFilters)
        {
            var keep = new List<string>();
            var filteredTables = new List<Poco.TableName>();

            FilterExplicetIgnoreList(original, ignoreList, keep, filteredTables);
            FilterWildcards(extraFilters, keep, filteredTables);

            return keep;
        }

        private void FilterWildcards(IEnumerable<string> extraFilters, List<string> keep,
            List<Poco.TableName> filteredTables)
        {
            foreach (var table in filteredTables.OrderBy(p => p.LevelSimple).ThenBy(p => p.Name))
            {
                var shouldIgnore = extraFilters.Where(p => table.Name.StartsWith(p)).Any();

                if (!shouldIgnore && !keep.Contains(table.Name))
                {
                    keep.Add(table.Name);
                }
            }
        }

        private void FilterExplicetIgnoreList(IEnumerable<Poco.TableName> original,
            IEnumerable<string> ignoreList, List<string> keep, List<Poco.TableName> filteredTables)
        {
            foreach (var table in original)
            {
                var shouldIgnore = ignoreList.Where(p => p == table.Name).Any();

                if (!shouldIgnore && !keep.Contains(table.Name))
                {
                    filteredTables.Add(table);
                }
            }
        }
    }
}
