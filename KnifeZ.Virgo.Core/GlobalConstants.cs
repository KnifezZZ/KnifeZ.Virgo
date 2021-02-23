namespace KnifeZ.Virgo.Core
{
    public static class GlobalConstants
    {
        public static class CacheKey
        {
            public const string UserInfo = "UserInfo";
        }

        /// <summary>
        /// Ĭ���ϴ�·��
        /// </summary>
        public const string DEFAULT_UPLOAD_DIR = ".\\upload";

        /// <summary>
        /// Ĭ���б�����
        /// </summary>
        public const int DEFAULT_PAGESIZE = 20;

        /// <summary>
        /// Ĭ���ϴ��ļ�����
        /// </summary>
        public const int DEFAULT_UPLOAD_LIMIT = 20 * 1024 * 1024;
    }
}
