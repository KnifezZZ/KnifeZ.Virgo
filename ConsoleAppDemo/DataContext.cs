﻿using KnifeZ.Extensions.DatabaseAccessor;
using KnifeZ.Virgo.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppDemo
{

    public class DataContext : FrameworkContext
    {

        public DataContext(ConnectionStrings cs) : base(cs)
        {
        }


        public DataContext(string cs, DBTypeEnum dbtype)
             : base(cs, dbtype)
        {
        }

        public override async Task<bool> DataInit(object allModules, bool IsSpa)
        {
            var state = await base.DataInit(allModules, IsSpa);
            //if (!state)
            //{
            //    state = !Set<FrameworkUser>().Any();
            //}
            //if (state)
            //{
            //    var user = new FrameworkUser
            //    {
            //        ITCode = "admin",
            //        Password = Utils.GetMD5String("000000"),
            //        IsValid = true,
            //        Name = "Admin"
            //    };

            //    var userrole = new FrameworkUserRole
            //    {
            //        User = user,
            //        Role = Set<FrameworkRole>().Where(x => x.RoleCode == "001").FirstOrDefault()
            //    };
            //    Set<FrameworkUser>().Add(user);
            //    Set<FrameworkUserRole>().Add(userrole);
            //    await SaveChangesAsync();
            //}
            return state;
        }
    }
    /// <summary>
    /// DesignTimeFactory for EF Migration, use your full connection string,
    /// EF will find this class and use the connection defined here to run Add-Migration and Update-Database
    /// </summary>
    public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            return new DataContext("Data Source=demoDb.db", DBTypeEnum.SQLite);
        }
    }
}
