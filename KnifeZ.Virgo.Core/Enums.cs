namespace KnifeZ.Virgo.Core
{
    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DBTypeEnum { SqlServer, MySql, PgSql, Memory, SQLite, Oracle }

    /// <summary>
    /// 上传图片存储方式
    /// </summary>
    public enum SaveFileModeEnum
    {
        Local = 0,//本地
        Database = 1,
        TenCos = 2,
        AliOSS = 3,
    };

    /// <summary>
    /// 日期类型
    /// </summary>
    public enum DateTimeTypeEnum
    {
        /// <summary>
        /// 日期选择器
        /// 可选择：年、月、日
        /// </summary>
        Date,
        /// <summary>
        /// 日期时间选择器
        /// 可选择：年、月、日、时、分、秒
        /// </summary>
        DateTime,
        /// <summary>
        /// 年选择器
        /// 只提供年列表选择
        /// </summary>
        Year,
        /// <summary>
        /// 年月选择器
        /// 只提供年、月选择
        /// </summary>
        Month,
        /// <summary>
        /// 时间选择器
        /// 只提供时、分、秒选择
        /// </summary>
        Time
    };

    public enum FieldTypeEnum { String, Int, Bool, Date }
    /// <summary>
    /// 上传类型
    /// </summary>
    public enum UploadTypeEnum { AllFiles, ImageFile, ZipFile, ExcelFile, WordFile, PDFFile, TextFile }
    public enum ComponentRenderMode { Normal, Declare, Get, Reference }

    public enum SortDir { Asc, Desc }

    public enum BackgroudColorEnum
    {
        Grey,
        Yellow,
        Red
    };

}
