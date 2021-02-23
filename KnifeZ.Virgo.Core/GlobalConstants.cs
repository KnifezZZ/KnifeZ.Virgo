namespace KnifeZ.Virgo.Core
{
    public static class GlobalConstants
    {
        public static class CacheKey
        {
            public const string UserInfo = "UserInfo";
        }

        /// <summary>
        /// 默认上传路径
        /// </summary>
        public const string DEFAULT_UPLOAD_DIR = ".\\upload";

        /// <summary>
        /// 默认列表行数
        /// </summary>
        public const int DEFAULT_PAGESIZE = 20;

        /// <summary>
        /// 默认上传文件限制
        /// </summary>
        public const int DEFAULT_UPLOAD_LIMIT = 20 * 1024 * 1024;
    }
}
