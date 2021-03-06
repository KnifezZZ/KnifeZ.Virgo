using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using MySqlConnector;
using System.Threading;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace KnifeZ.Extensions.DatabaseAccessor
{
    public partial class EmptyContext : DbContext, IDataContext
    {
        private ILoggerFactory _loggerFactory;

        /// <summary>
        /// Commited
        /// </summary>
        public bool Commited { get; set; }

        /// <summary>
        /// IsFake
        /// </summary>
        public bool IsFake { get; set; }

        public bool IsDebug { get; set; }
        /// <summary>
        /// CSName
        /// </summary>
        public string CSName { get; set; }

        public DBTypeEnum DBType { get; set; }

        public string Version { get; set; }
        public ConnectionStrings ConnectionString { get; set; }
        /// <summary>
        /// FrameworkContext
        /// </summary>
        public EmptyContext()
        {
            CSName = "default";
            DBType = DBTypeEnum.SqlServer;
        }

        /// <summary>
        /// FrameworkContext
        /// </summary>
        /// <param name="cs"></param>
        public EmptyContext(string cs)
        {
            CSName = cs;
            DBType = DBTypeEnum.SqlServer;
        }

        public EmptyContext(string cs, DBTypeEnum dbtype, string version = null)
        {
            CSName = cs;
            DBType = dbtype;
            Version = version;
        }

        public EmptyContext(ConnectionStrings cs)
        {
            CSName = cs.Value;
            DBType = cs.DbType ?? DBTypeEnum.SqlServer;
            Version = cs.Version;
            ConnectionString = cs;
        }

        public EmptyContext(DbContextOptions options) : base(options) { }

        public IDataContext CreateNew()
        {
            if (ConnectionString != null)
            {
                return (IDataContext)this.GetType().GetConstructor(new Type[] { typeof(ConnectionStrings) }).Invoke(new object[] { ConnectionString }); ;
            }
            else
            {
                return (IDataContext)this.GetType().GetConstructor(new Type[] { typeof(string), typeof(DBTypeEnum), typeof(string) }).Invoke(new object[] { CSName, DBType, Version });
            }
        }

        public IDataContext ReCreate()
        {
            if (ConnectionString != null)
            {
                return (IDataContext)this.GetType().GetConstructor(new Type[] { typeof(ConnectionStrings) }).Invoke(new object[] { ConnectionString }); ;
            }
            else
            {
                return (IDataContext)this.GetType().GetConstructor(new Type[] { typeof(string), typeof(DBTypeEnum), typeof(string) }).Invoke(new object[] { CSName, DBType, Version });
            }
        }
        /// <summary>
        /// 将一个实体设为填加状态
        /// </summary>
        /// <param name="entity">实体</param>
        public void AddEntity<T>(T entity) where T : TopBasePoco
        {
            this.Entry(entity).State = EntityState.Added;
        }

        /// <summary>
        /// 将一个实体设为修改状态
        /// </summary>
        /// <param name="entity">实体</param>
        public void UpdateEntity<T>(T entity) where T : TopBasePoco
        {
            this.Entry(entity).State = EntityState.Modified;
        }

        /// <summary>
        /// 将一个实体的某个字段设为修改状态，用于只更新个别字段的情况
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="fieldExp">要设定为修改状态的字段</param>
        public void UpdateProperty<T>(T entity, Expression<Func<T, object>> fieldExp)
            where T : TopBasePoco
        {
            var set = this.Set<T>();
            if (set.Local.AsQueryable().CheckID(entity.GetID()).FirstOrDefault() == null)
            {
                set.Attach(entity);
            }
            this.Entry(entity).Property(fieldExp).IsModified = true;
        }

        /// <summary>
        /// UpdateProperty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="fieldName"></param>
        public void UpdateProperty<T>(T entity, string fieldName)
            where T : TopBasePoco
        {
            var set = this.Set<T>();
            if (set.Local.AsQueryable().CheckID(entity.GetID()).FirstOrDefault() == null)
            {
                set.Attach(entity);
            }
            this.Entry(entity).Property(fieldName).IsModified = true;
        }

        /// <summary>
        /// 将一个实体设定为删除状态
        /// </summary>
        /// <param name="entity">实体</param>
        public void DeleteEntity<T>(T entity) where T : TopBasePoco
        {
            var set = this.Set<T>();
            var exist = set.Local.AsQueryable().CheckID(entity.GetID()).FirstOrDefault();
            if (exist == null)
            {
                set.Attach(entity);
                set.Remove(entity);
            }
            else
            {
                set.Remove(exist);

            }
        }

        /// <summary>
        /// CascadeDelete
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public void CascadeDelete<T>(T entity) where T : TreePoco
        {
            if (entity != null && entity.ID != Guid.Empty)
            {
                var set = this.Set<T>();
                var entities = set.Where(x => x.ParentId == entity.ID).ToList();
                if (entities.Count > 0)
                {
                    foreach (var item in entities)
                    {
                        CascadeDelete(item);
                    }
                }
                DeleteEntity(entity);
            }
        }

        /// <summary>
        /// GetCoreType
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Type GetCoreType(Type t)
        {
            if (t != null && t.IsNullable())
            {
                if (!t.GetTypeInfo().IsValueType)
                {
                    return t;
                }
                else
                {
                    if ("DateTime".Equals(t.GenericTypeArguments[0].Name))
                    {
                        return typeof(string);
                    }
                    return Nullable.GetUnderlyingType(t);
                }
            }
            else
            {
                if ("DateTime".Equals(t.Name))
                {
                    return typeof(string);
                }
                return t;
            }
        }

        /// <summary>
        /// OnModelCreating
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (DBType == DBTypeEnum.Oracle)
            {
                modelBuilder.Model.SetMaxIdentifierLength(30);
            }
        }

        /// <summary>
        /// OnConfiguring
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            switch (DBType)
            {
                case DBTypeEnum.SqlServer:
                    optionsBuilder.UseSqlServer(CSName);
                    break;
                case DBTypeEnum.MySql:
                    ServerVersion sv = null;
                    if (string.IsNullOrEmpty(Version) == false)
                    {
                        ServerVersion.TryFromString(Version, out sv);
                    }
                    if (sv == null)
                    {
                        sv = ServerVersion.AutoDetect(CSName);
                    }
                    optionsBuilder.UseMySql(CSName, sv);
                    break;
                case DBTypeEnum.PgSql:
                    optionsBuilder.UseNpgsql(CSName);
                    break;
                case DBTypeEnum.Memory:
                    optionsBuilder.UseInMemoryDatabase(CSName);
                    break;
                case DBTypeEnum.SQLite:
                    optionsBuilder.UseSqlite(CSName);
                    break;
                case DBTypeEnum.Oracle:

                    optionsBuilder.UseOracle(CSName, option => {
                        if (string.IsNullOrEmpty(Version) == false)
                        {
                            option.UseOracleSQLCompatibility(Version);
                        }
                        else
                        {
                            option.UseOracleSQLCompatibility("11");
                        }
                    });
                    break;
                default:
                    break;
            }
            if (IsDebug == true)
            {
                optionsBuilder.EnableDetailedErrors();
                optionsBuilder.EnableSensitiveDataLogging();
                if (_loggerFactory != null)
                {
                    optionsBuilder.UseLoggerFactory(_loggerFactory);
                }
            }
            base.OnConfiguring(optionsBuilder);
        }

        public void SetLoggerFactory(ILoggerFactory factory)
        {
            this._loggerFactory = factory;
        }


        /// <summary>
        /// 数据初始化
        /// </summary>
        /// <param name="allModules"></param>
        /// <param name="IsSpa"></param>
        /// <returns>返回true表示需要进行初始化数据操作，返回false即数据库已经存在或不需要初始化数据</returns>
        public async virtual Task<bool> DataInit(object allModules, bool IsSpa)
        {
            bool rv = await Database.EnsureCreatedAsync();
            return rv;
        }

        #region 执行存储过程返回datatable
        /// <summary>
        /// 执行存储过程，返回datatable结果集
        /// </summary>
        /// <param name="command">存储过程名称</param>
        /// <param name="paras">存储过程参数</param>
        /// <returns></returns>
        public DataTable RunSP(string command, params object[] paras)
        {
            return Run(command, CommandType.StoredProcedure, paras);
        }
        #endregion

        public IEnumerable<TElement> RunSP<TElement>(string command, params object[] paras)
        {
            return Run<TElement>(command, CommandType.StoredProcedure, paras);
        }

        #region 执行Sql语句，返回datatable
        public DataTable RunSQL(string sql, params object[] paras)
        {
            return Run(sql, CommandType.Text, paras);
        }
        #endregion

        public IEnumerable<TElement> RunSQL<TElement>(string sql, params object[] paras)
        {
            return Run<TElement>(sql, CommandType.Text, paras);
        }


        #region 执行存储过程或Sql语句返回DataTable
        /// <summary>
        /// 执行存储过程或Sql语句返回DataTable
        /// </summary>
        /// <param name="sql">存储过程名称或Sql语句</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="paras">参数</param>
        /// <returns></returns>
        public DataTable Run(string sql, CommandType commandType, params object[] paras)
        {
            DataTable table = new DataTable();
            switch (this.DBType)
            {
                case DBTypeEnum.SqlServer:
                    SqlConnection con = this.Database.GetDbConnection() as SqlConnection;
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        adapter.SelectCommand = cmd;
                        cmd.CommandTimeout = 2400;
                        cmd.CommandType = commandType;
                        if (paras != null)
                        {
                            foreach (var param in paras)
                                cmd.Parameters.Add(param);
                        }
                        adapter.Fill(table);
                        adapter.SelectCommand.Parameters.Clear();
                    }
                    break;
                case DBTypeEnum.MySql:
                    MySqlConnection mySqlCon = this.Database.GetDbConnection() as MySqlConnection;
                    using (MySqlCommand cmd = new MySqlCommand(sql, mySqlCon))
                    {
                        if (mySqlCon.State == ConnectionState.Closed)
                        {
                            mySqlCon.Open();
                        }
                        cmd.CommandTimeout = 2400;
                        cmd.CommandType = commandType;
                        if (paras != null)
                        {
                            foreach (var param in paras)
                                cmd.Parameters.Add(param);
                        }
                        MySqlDataReader dr = cmd.ExecuteReader();
                        table.Load(dr);
                        dr.Close();
                        mySqlCon.Close();
                    }
                    break;
                case DBTypeEnum.PgSql:
                    Npgsql.NpgsqlConnection npgCon = this.Database.GetDbConnection() as Npgsql.NpgsqlConnection;
                    using (Npgsql.NpgsqlCommand cmd = new Npgsql.NpgsqlCommand(sql, npgCon))
                    {
                        if (npgCon.State == ConnectionState.Closed)
                        {
                            npgCon.Open();
                        }
                        cmd.CommandTimeout = 2400;
                        cmd.CommandType = commandType;
                        if (paras != null)
                        {
                            foreach (var param in paras)
                                cmd.Parameters.Add(param);
                        }
                        Npgsql.NpgsqlDataReader dr = cmd.ExecuteReader();
                        table.Load(dr);
                        dr.Close();
                        npgCon.Close();
                    }
                    break;
                case DBTypeEnum.SQLite:
                case DBTypeEnum.Oracle:
                    var connection = this.Database.GetDbConnection();
                    var isClosed = connection.State == ConnectionState.Closed;
                    if (isClosed)
                    {
                        connection.Open();
                    }
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = sql;
                        command.CommandTimeout = 2400;
                        command.CommandType = commandType;
                        if (paras != null)
                        {
                            foreach (var param in paras)
                                command.Parameters.Add(param);
                        }
                        using (var reader = command.ExecuteReader())
                        {
                            table.Load(reader);
                        }
                    }
                    if (isClosed)
                    {
                        connection.Close();
                    }
                    break;
            }
            return table;
        }
        #endregion


        public IEnumerable<TElement> Run<TElement>(string sql, CommandType commandType, params object[] paras)
        {
            IEnumerable<TElement> entityList = new List<TElement>();
            DataTable dt = Run(sql, commandType, paras);
            entityList = EntityHelper.GetEntityList<TElement>(dt);
            return entityList;
        }


        public object CreateCommandParameter(string name, object value, ParameterDirection dir)
        {
            object rv = null;
            switch (this.DBType)
            {
                case DBTypeEnum.SqlServer:
                    rv = new SqlParameter(name, value) { Direction = dir };
                    break;
                case DBTypeEnum.MySql:
                    rv = new MySqlParameter(name, value) { Direction = dir };
                    break;
                case DBTypeEnum.PgSql:
                    rv = new NpgsqlParameter(name, value) { Direction = dir };
                    break;
                case DBTypeEnum.SQLite:
                    rv = new SqliteParameter(name, value) { Direction = dir };
                    break;
                case DBTypeEnum.Oracle:
                    //rv = new OracleParameter(name, value) { Direction = dir };
                    break;
            }
            return rv;
        }
    }

    public class NullContext : IDataContext
    {


        public bool IsFake { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IModel Model => throw new NotImplementedException();

        public DatabaseFacade Database => throw new NotImplementedException();

        public string CSName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DBTypeEnum DBType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsDebug { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void AddEntity<T>(T entity) where T : TopBasePoco
        {
            throw new NotImplementedException();
        }

        public void CascadeDelete<T>(T entity) where T : TreePoco
        {
            throw new NotImplementedException();
        }

        public object CreateCommandParameter(string name, object value, ParameterDirection dir)
        {
            throw new NotImplementedException();
        }

        public IDataContext CreateNew()
        {
            throw new NotImplementedException();
        }

        public Task<bool> DataInit(object AllModel, bool IsSpa)
        {
            throw new NotImplementedException();
        }

        public void DeleteEntity<T>(T entity) where T : TopBasePoco
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {

        }

        public IDataContext ReCreate()
        {
            throw new NotImplementedException();
        }

        public DataTable Run(string sql, CommandType commandType, params object[] paras)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TElement> Run<TElement>(string sql, CommandType commandType, params object[] paras)
        {
            throw new NotImplementedException();
        }

        public DataTable RunSP(string command, params object[] paras)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TElement> RunSP<TElement>(string command, params object[] paras)
        {
            throw new NotImplementedException();
        }

        public DataTable RunSQL(string command, params object[] paras)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TElement> RunSQL<TElement>(string sql, params object[] paras)
        {
            throw new NotImplementedException();
        }

        public int SaveChanges()
        {
            throw new NotImplementedException();
        }

        public int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public DbSet<T> Set<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public void SetLoggerFactory(ILoggerFactory factory)
        {
            throw new NotImplementedException();
        }

        public void UpdateEntity<T>(T entity) where T : TopBasePoco
        {
            throw new NotImplementedException();
        }

        public void UpdateProperty<T>(T entity, Expression<Func<T, object>> fieldExp) where T : TopBasePoco
        {
            throw new NotImplementedException();
        }

        public void UpdateProperty<T>(T entity, string fieldName) where T : TopBasePoco
        {
            throw new NotImplementedException();
        }
    }
}