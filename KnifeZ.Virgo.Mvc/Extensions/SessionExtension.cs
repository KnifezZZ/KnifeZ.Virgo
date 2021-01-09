using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace KnifeZ.Virgo.Mvc
{
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key,JsonSerializer.Serialize(value));
            session.CommitAsync().Wait();
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) :
                                  JsonSerializer.Deserialize<T>(value);
        }
    }
}
