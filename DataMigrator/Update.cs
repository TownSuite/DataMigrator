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

namespace DataMigrator
{
    public class Update
    {

        private Update() { }

        string table;
        public Update(string table)
        {
            this.table = table;
        }

        public void Execute()
        {
            Program.Write("            // Begin data for table " + table + System.Environment.NewLine);

            using (var cn = Program.ConnectionFactory)
            {
                cn.Open();
                var columns = Poco.Columns.Retrieve(table);


                var tableData = cn.Query("SELECT * FROM " + table);

                foreach (var row in tableData)
                {

                    var syntax = new StringBuilder();
                    syntax.Append("            Update.Table(\"" + table + "\").Set(new {");

                    int colCount = 0;
                    foreach (var col in columns)
                    {
                        if (col.IsIdentity) continue;

                        SetInsert(row, syntax, col);


                        colCount++;
                        if (colCount > 1)
                        {
                            syntax.AppendLine();
                            syntax.Append("               ");
                        }
                    }

                    syntax.Append(" }).Where(new { ");
                    SetWhere(row, syntax, columns);
                    syntax.Append(" });");
                    syntax.AppendLine();

                    Program.Write(syntax.ToString());

                }


            }
            Program.Write("            // end data for table " + table);
            Program.Write(System.Environment.NewLine);



        }

        private void SetInsert(dynamic row, StringBuilder syntax, Poco.Columns col)
        {
            var value = GetProperty(row, col.Name);
            SyntaxAppend(syntax, value, col);
        }

        private void SetWhere(dynamic row, StringBuilder syntax, IEnumerable<Poco.Columns> cols)
        {
            using (var cn = Program.ConnectionFactory)
            {
                cn.Open();

                var primaryKeys = cn.Query<string>("SELECT column_name FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE OBJECTPROPERTY(OBJECT_ID(constraint_name), 'IsPrimaryKey') = 1 AND table_name = @Table",
                    new { Table = this.table });

                foreach (var key in primaryKeys)
                {
                    var col = cols.Where(p => p.Name == key).Select(p => p).FirstOrDefault();
                    if (col == null) continue;

                    var value = GetProperty(row, col.Name);

                    SyntaxAppend(syntax, value, col);

                }


            }
        }

        private void SyntaxAppend(StringBuilder syntax, dynamic value, Poco.Columns col)
        {
            if (value != null)
            {
                if (col.DataType.Contains("char"))
                {
                    syntax.AppendFormat(" {0} = \"{1}\",", col.Name, value);
                }
                else if (col.DataType.Contains("bit"))
                {
                    syntax.AppendFormat(" {0} = \"{1}\",", col.Name, value.ToString().ToLower());
                }
                else if (col.DataType.Contains("date"))
                {
                    syntax.AppendFormat(" {0} = DateTime.Parse(\"{1}\"),", col.Name, value);
                }
                else
                {
                    syntax.AppendFormat(" {0} = {1},", col.Name, value);
                }
            }
        }

        private object GetProperty(object target, string name)
        {
            var value = Dynamitey.Dynamic.InvokeGet(target, name);
            return value;
        }
    }


}
