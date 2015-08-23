/* ====================================================================
   Copyright (C) 2015 Procom Data Services Inc.

   This file is part of the TownSuite DataMigrator Tool.
	
   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at
       http://www.apache.org/licenses/LICENSE-2.0
   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.

   For additional information, visit
   the website https://github.com/TownSuite/DataMigrator.
*/
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
