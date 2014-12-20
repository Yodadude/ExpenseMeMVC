using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExpenseMeMVC.Infrastructure
{
    public interface IDatabaseFactory
    {
        IDatabase Create();
    }

    public class DatabaseFactory : IDatabaseFactory
    {
        public IDatabase Create()
        {
            var connectionString = @"Server=(local)\sqlexpress;Database=pm98_mobile;User Id=promasterweb; Password=changeoninstall;";

            var db = new Database(connectionString, DatabaseType.SqlServer2008);

            return db;
        }
    }
}