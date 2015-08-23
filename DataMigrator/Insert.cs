using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace DataMigrator
{
    public class Insert
    {
        private Insert() { }

        readonly string table;
        public Insert(string table)
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
                    syntax.Append("            Insert.IntoTable(\"" + table + "\").Row(new {");

                    int colCount = 0;

                    bool specialIgnoreFound = false;
                    foreach (var col in columns)
                    {
                        if (col.IsIdentity) continue;
                        if (SpecialIgnoreFound(row, col))
                        {
                            specialIgnoreFound = true;
                            break;
                        };

                        if (col.DataType.Contains("char"))
                        {
                            CharInsert(row, syntax, col);
                        }
                        else if (col.DataType.Contains("date"))
                        {
                            DateTimeInsert(row, syntax, col);
                        }
                        else if (col.DataType.Contains("bit"))
                        {
                            BitInsert(row, syntax, col);
                        }
                        else if (col.DataType.Contains("uniqueidentifier"))
                        {
                            GuidInsert(row, syntax, col);
                        }
                        else if (col.DataType.Contains("varbinary"))
                        {
                            BinaryInsert(row, syntax, col);
                        }
                        else
                        {
                            FallbackInsert(row, syntax, col);
                        }

                        colCount++;
                        if (colCount > 1)
                        {
                            syntax.AppendLine();
                            syntax.Append("               ");
                        }
                    }


                    syntax.Append(" });");
                    syntax.AppendLine();

                    if (specialIgnoreFound)
                    {
                        syntax.Clear();
                    }

                    Program.Write(syntax.ToString());

                }


            }
            Program.Write("            // end data for table " + table);
            Program.Write(System.Environment.NewLine);
        }

        private bool SpecialIgnoreFound(dynamic row, Poco.Columns col)
        {
            var value = GetProperty(row, col.Name);
            foreach (var item in Program.SpecialIgnores)
            {
                if (table == item.Item1 && item.Item2 == col.Name && item.Item3 == value)
                {
                    return true;
                }
            }

            return false;
        }

        private void DateTimeInsert(dynamic row, StringBuilder syntax, Poco.Columns col)
        {
            var value = GetProperty(row, col.Name);
            if (value != null)
            {
                syntax.AppendFormat(" {0} = DateTime.Parse(\"{1}\"),", col.Name, value);
            }
        }

        private void FallbackInsert(dynamic row, StringBuilder syntax, Poco.Columns col)
        {
            var value = GetProperty(row, col.Name);

            if (value != null)
            {
                syntax.AppendFormat(" {0} = {1},", col.Name, value);
            }
        }

        private void BinaryInsert(dynamic row, StringBuilder syntax, Poco.Columns col)
        {
            var value = GetProperty(row, col.Name);

            if (value != null)
            {
                syntax.AppendFormat(" {0} = Convert.FromBase64String(\"{1}\"),", col.Name, Convert.ToBase64String(value));
            }
        }

        private void GuidInsert(dynamic row, StringBuilder syntax, Poco.Columns col)
        {
            var value = GetProperty(row, col.Name);

            if (value != null)
            {
                syntax.AppendFormat(" {0} = Guid.Parse(\"{1}\"),", col.Name, value.ToString().ToLower());
            }
        }

        private void BitInsert(dynamic row, StringBuilder syntax, Poco.Columns col)
        {
            var value = GetProperty(row, col.Name);

            if (value != null)
            {
                syntax.AppendFormat(" {0} = {1},", col.Name, value.ToString().ToLower());

            }
        }

        private void CharInsert(dynamic row, StringBuilder syntax, Poco.Columns col)
        {
            var value = GetProperty(row, col.Name);
            if (value != null)
            {
                syntax.AppendFormat(" {0} = \"{1}\",", col.Name, value.Replace("\"", "\\\""));
            }
        }

        private object GetProperty(object target, string name)
        {
            var value = Dynamitey.Dynamic.InvokeGet(target, name);
            return value;
        }


    }
}
