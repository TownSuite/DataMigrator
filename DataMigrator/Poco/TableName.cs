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
