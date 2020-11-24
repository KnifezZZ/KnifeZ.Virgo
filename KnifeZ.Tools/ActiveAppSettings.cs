using System;
using System.IO;
using System.Text.Json;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using KnifeZ.Tools.Helpers;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace KnifeZ.Tools
{
    /// <summary>
    /// 动态写入读取json配置文件
    /// </summary>
    public class ActiveAppSettings
    {
        /// <summary>
        /// 默认路径
        /// </summary>
        private static readonly string _defaultPath = Directory.GetCurrentDirectory() + "\\runsettings.json";

        static ActiveAppSettings ()
        {

        }

        public static string ReadString (string key, string defaultPath = null)
        {
            var jsonElement = Read(key, defaultPath);
            string res = jsonElement.ValueKind switch
            {
                JsonValueKind.String => jsonElement.GetString(),
                JsonValueKind.Number => jsonElement.GetInt32().ToString(),
                _ => jsonElement.GetRawText(),
            };
            return res;
        }
        public static Guid ReadGuid (string key, string defaultPath = null)
        {
            return Read(key, defaultPath).GetGuid();
        }
        public static int ReadInt (string key, string defaultPath = null)
        {
            return Read(key, defaultPath).GetInt32();
        }
        /// <summary>
        /// 读取json 值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultPath"></param>
        /// <returns></returns>
        public static JsonElement Read (string key, string defaultPath = null)
        {
            if (defaultPath is null)
                defaultPath = _defaultPath;
            using var stream = File.OpenRead(defaultPath);
            using (JsonDocument document = JsonDocument.Parse(stream))
            {
                JsonElement json = document.RootElement.Clone();
                foreach (var item in key.Split(':'))
                {
                    json = json.GetProperty(item);
                }
                return json;
            }
        }
        /// <summary>
        /// 写入json键值对(暂时使用Newtonsoft.json)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="defaultPath"></param>
        public static void Write (string key, string value, string defaultPath = null)
        {
            if (defaultPath is null)
                defaultPath = _defaultPath;
            JObject jsonObject;
            using (StreamReader file = new StreamReader(defaultPath))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                jsonObject = (JObject)JToken.ReadFrom(reader);
                jsonObject[key] = value;
            }
            using (var writer = new StreamWriter(_defaultPath))
            using (JsonTextWriter jsonwriter = new JsonTextWriter(writer))
            {
                jsonwriter.Formatting = Formatting.Indented;
                jsonObject.WriteTo(jsonwriter);
            }
        }
    }
}
