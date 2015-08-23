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
using Dapper;

namespace DataMigrator.Poco
{
    public class TableName
    {
        public string Name { get; set; }

        public string Level { get; set; }

        public int LevelSimple { get; set; }

        public string DepPath { get; set; }

        public static IEnumerable<TableName> Retrieve()
        {
            using (var cn = Program.ConnectionFactory)
            {
                cn.Open();
                var sql = Properties.Resources.TableHierarchy;
                var tables = cn.Query<Poco.TableName>(sql);
                return tables;
            }

        }

    }
}
