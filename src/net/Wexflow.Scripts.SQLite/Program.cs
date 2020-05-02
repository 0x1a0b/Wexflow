﻿using System;
using System.Configuration;
using Wexflow.Core.Db.SQLite;

namespace Wexflow.Scripts.SQLite
{
    class Program
    {
        static void Main()
        {
            try
            {
                Db.Create(ConfigurationManager.AppSettings["connectionString"]);
                var db = Db.Instance;
                Core.Helper.InsertWorkflowsAndUser(db);
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occured: {0}", e);
            }

            Console.Write("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
