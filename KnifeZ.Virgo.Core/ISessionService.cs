namespace KnifeZ.Virgo.Core
{
    /// <summary>
    /// Session接口
    /// </summary>
    public interface ISessionService
    {
        T Get<T>(string key);
        void Set<T>(string key, T val);
        string SessionId { get; }
    }
}
