using System;
using KnifeZ.Virgo.Core;
using Microsoft.EntityFrameworkCore.Design;

namespace WebDemo.DataAccess
{
    public class DataContext: FrameworkContext
    {

        public DataContext (CS cs)
     : base(cs)
        {
        }


        public DataContext (string cs, DBTypeEnum dbtype)
             : base(cs, dbtype)
        {
        }
    }
    /// <summary>
    /// DesignTimeFactory for EF Migration, use your full connection string,
    /// EF will find this class and use the connection defined here to run Add-Migration and Update-Database
    /// </summary>
    public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext (string[] args)
        {
            return new DataContext("Server=172.16.0.123;Database=VirgoDb;User ID=sa;Password=hU7NDBFG;", DBTypeEnum.SqlServer);
        }
    }
}
