using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMigrator
{
    class Program
    {
        private static string connectionStr;
        public static System.Data.IDbConnection ConnectionFactory
        {
            get
            {
                return new System.Data.SqlClient.SqlConnection(connectionStr);
            }
      
        }

        public static IEnumerable<string> IgnoreTables
        {
            get;
            private set;
        }

        public static IEnumerable<string> UpdateTables
        {
            get;
            private set;
        }

        public static IEnumerable<string> TableOrder
        {
            get;
            private set;
        }


        public static IEnumerable<Tuple<string, string, string>> SpecialIgnores
        {
            get;
            private set;
        }


        public static string SaveFile
        {
            get;
            private set;
        }

        static void Main(string[] args)
        {
            connectionStr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;
            
            SaveFile = System.Configuration.ConfigurationManager.AppSettings["SaveFile"].ToString();

            SetIgnoreTable();
            SetUpdateTable();
            SetTableOrder();
            SetSpecialIgnores();


            var gen = new Generate();
            gen.Execute();
        }

        private static void SetIgnoreTable()
        {
            var ignore = System.Configuration.ConfigurationManager.AppSettings["IgnoreTables"].ToString();
            if (!string.IsNullOrWhiteSpace(ignore))
            {
                IgnoreTables = ignore.Split(',').Select(p => p.Trim()).ToList();
            }
        }

        private static void SetUpdateTable()
        {
            var update = System.Configuration.ConfigurationManager.AppSettings["UpdateTables"].ToString();
            if (!string.IsNullOrWhiteSpace(update))
            {
                UpdateTables = update.Split(',').Select(p => p.Trim()).ToList();
            }
        }

        private static void SetTableOrder()
        {
            var order = System.Configuration.ConfigurationManager.AppSettings["TableOrder"].ToString();
            if (!string.IsNullOrWhiteSpace(order))
            {
                TableOrder = order.Split(',').Select(p => p.Trim()).ToList();
            }
        }

        private static void SetSpecialIgnores()
        {
            var order = System.Configuration.ConfigurationManager.AppSettings["SpecialIgnores"].ToString();
            if (!string.IsNullOrWhiteSpace(order))
            {
                var split = order.Split(',').Select(p => p.Trim()).ToList();

                var specialIgnore = new List<Tuple<string, string, string>>();

                foreach(var item in split)
                {
                    var tableSplit = item.Split(';');
                    var tuple = new Tuple<string, string, string>(tableSplit[0], tableSplit[1], tableSplit[2]);
                    specialIgnore.Add(tuple);
                }

                SpecialIgnores = specialIgnore;
            }
        }


        public static void Write(string msg)
        {
            System.IO.File.AppendAllText(Program.SaveFile, msg);
        }
    }
}
