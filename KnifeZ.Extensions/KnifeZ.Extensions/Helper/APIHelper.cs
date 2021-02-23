using KnifeZ.Extensions.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace KnifeZ.Extensions.Helper
{
    public enum HttpMethodEnum { GET, POST, PUT, DELETE }

    /// <summary>
    /// 有关HTTP请求的辅助类
    /// </summary>
    public class APIHelper
    {
        private static readonly string DefaultUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.67 Safari/537.36 Edg/87.0.664.52";

        private static readonly object LockObj = new object();

        private static HttpClient client = null;
        public APIHelper()
        {
            Instance();
        }
        /// <summary>
        /// HttpClient单例模式
        /// </summary>
        /// <returns></returns>
        public static HttpClient Instance()
        {
            if (client == null)
            {
                lock (LockObj)
                {
                    if (client == null)
                    {
                        client = new HttpClient()
                        {
                            //设置超时时间10分钟
                            Timeout = new TimeSpan(0, 30, 0)
                        };
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Add("UserAgent", DefaultUserAgent);
                        //帮HttpClient热身
                        client.SendAsync(new HttpRequestMessage
                        {
                            Method = new HttpMethod("HEAD"),
                            RequestUri = new Uri("http://127.0.0.1/")
                        }).Result.EnsureSuccessStatusCode();
                    }
                }
            }
            return client;
        }

        public async Task<HttpContent> PostAsync(string url, string json)
        {
            HttpContent content = new StringContent(json);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            return response.Content;
        }
        public async Task<HttpContent> GetAsync(string url)
        {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return response.Content;
        }
        /// <summary>
        /// 调用远程地址
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="method">请求方式</param>
        /// <param name="postdata">Post数据</param>
        /// <param name="timeout">超时秒数</param>
        /// <param name="proxy">代理</param>
        /// <returns>远程方法返回的内容</returns>
        public static async Task<string> CallAPI(string url, HttpMethodEnum method = HttpMethodEnum.GET, IDictionary<string, string> postdata = null, int? timeout = null, string proxy = null)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    return string.Empty;
                }
                //新建http请求
                var request = WebRequest.Create(url) as HttpWebRequest;
                //如果配置了代理，则使用代理
                if (string.IsNullOrEmpty(proxy) == false)
                {
                    request.Proxy = new WebProxy(proxy);
                }

                request.Method = method.ToString();
                //如果是Post请求，则设置表单
                if (method == HttpMethodEnum.POST || method == HttpMethodEnum.PUT)
                {
                    if (postdata.ContainsKey("content-type"))
                    {
                        request.ContentType = postdata["content-type"];
                    }
                    else
                    {
                        request.ContentType = "application/x-www-form-urlencoded";
                    }
                    if (postdata == null || postdata.Count == 0)
                    {
                        request.ContentLength = 0;
                    }
                }
                request.UserAgent = DefaultUserAgent;
                //设置超时
                if (timeout.HasValue)
                {
                    request.ContinueTimeout = timeout.Value;
                }
                request.CookieContainer = new CookieContainer();
                //填充表单数据
                if (!(postdata == null || postdata.Count == 0))
                {
                    var buffer = new StringBuilder();
                    var i = 0;
                    foreach (string key in postdata.Keys)
                    {
                        if (i > 0)
                        {
                            buffer.AppendFormat("&{0}={1}", key, postdata[key]);
                        }
                        else
                        {
                            buffer.AppendFormat("{0}={1}", key, postdata[key]);
                        }
                        i++;
                    }
                    var data = Encoding.UTF8.GetBytes(buffer.ToString());
                    using (var stream = await request.GetRequestStreamAsync())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }


                var res = await request.GetResponseAsync();
                var wsp = (HttpWebResponse)res;
                Stream st = null;
                //如果远程服务器采用了gzip，增进行相应的解压缩
                if (wsp.Headers[HttpResponseHeader.ContentEncoding]?.ToLower().Contains("gzip") == true)
                {
                    st = new GZipStream(st, CompressionMode.Decompress);
                }
                else
                {
                    st = wsp.GetResponseStream();
                }
                //设置编码
                var encode = Encoding.UTF8;
                if (!string.IsNullOrEmpty(wsp.Headers[HttpResponseHeader.ContentEncoding]))
                {
                    encode = Encoding.GetEncoding(wsp.Headers[HttpResponseHeader.ContentEncoding]);
                }
                //读取内容
                var sr = new StreamReader(st, encode);
                var ss = sr.ReadToEnd();
                sr.Dispose();
                wsp.Dispose();
                return ss;
            }
            catch (Exception ex)
            {
                //返回失败信息
                ListItemModel rv = new ListItemModel()
                {
                    Text = ex.Message,
                    Value = ex.StackTrace
                };
                return HttpUtility.UrlDecode(JsonSerializer.Serialize(rv));
            }
        }

        /// <summary>
        /// 调用远程方法，返回强类型
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="url">地址</param>
        /// <param name="method">请求方式</param>
        /// <param name="postdata">Post数据</param>
        /// <param name="timeout">超时秒数</param>
        /// <param name="proxy">代理</param>
        /// <returns>强类型</returns>
        public static async Task<T> CallAPI<T>(string url, HttpMethodEnum method = HttpMethodEnum.GET, IDictionary<string, string> postdata = null, int? timeout = null, string proxy = null)
        {
            var s = await CallAPI(url, method, postdata, timeout, proxy);
            return JsonSerializer.Deserialize<T>(s);
        }
    }
}
