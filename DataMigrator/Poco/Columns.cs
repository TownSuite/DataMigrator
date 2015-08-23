using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace DataMigrator.Poco
{
    public class Columns
    {

        public string Name { get; set; }

        public string IS_NULLABLE { get; set; }

        public string DataType { get; set; }

        public bool IsIdentity { get; set; }

        public bool Nullable
        {
            get
            {
                return IS_NULLABLE.ToLower() == "yes" ? true : false;
            }
        }

        public static IEnumerable<Columns> Retrieve(string table)
        {
            using (var cn = Program.ConnectionFactory)
            {
                cn.Open();
                var columns = cn.Query<Poco.Columns>("select Column_Name as 'Name', IS_NULLABLE, DATA_TYPE as 'DataType', COLUMNPROPERTY(object_id(TABLE_NAME), COLUMN_NAME, 'IsIdentity') as 'IsIdentity' from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME=@Table",
                    new { Table = table });

                return columns;

            }

        }
    }
}
