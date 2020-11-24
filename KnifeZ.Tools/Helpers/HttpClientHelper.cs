using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KnifeZ.Tools.Helpers
{
    public class HttpClientHelper
    {
        private static readonly object LockObj = new object();
        private static HttpClient client = null;
        public HttpClientHelper()
        {
            GetInstance();
        }
        /// <summary>
        /// 单例模式
        /// </summary>
        /// <returns></returns>
        public static HttpClient GetInstance()
        {

            if (client == null)
            {
                lock (LockObj)
                {
                    if (client == null)
                    {
                        client = new HttpClient();
                    }
                }
            }
            return client;
        }
        /// <summary>
        /// post异步请求方法 Quickly using
        /// </summary>
        /// <param name="url"></param>
        /// <param name="strJson"></param>
        /// <returns></returns>
        public static async Task<string> PostAsync(string url, string strJson)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpContent content = new StringContent(strJson);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                //由HttpClient发出异步Post请求
                HttpResponseMessage res = await httpClient.PostAsync(url, content);
                if (res.StatusCode == HttpStatusCode.OK)
                {
                    string str = res.Content.ReadAsStringAsync().Result;
                    return str;
                }
                else
                    return null;
            }
        }
        /// <summary>
        /// post同步请求方法 Quickly using
        /// </summary>
        /// <param name="url"></param>
        /// <param name="strJson"></param>
        /// <returns></returns>
        public static string Post(string url, string strJson)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpContent content = new StringContent(strJson);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                //client.DefaultRequestHeaders.Connection.Add("keep-alive");
                //由HttpClient发出Post请求
                Task<HttpResponseMessage> res = httpClient.PostAsync(url, content);
                if (res.Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string str = res.Result.Content.ReadAsStringAsync().Result;
                    return str;
                }
                else
                    return null;

            }
        }
        /// <summary>
        /// get同步请求 Quickly using
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string Get(string url)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var responseString = httpClient.GetStringAsync(url);
                return responseString.Result;
            }
        }
        /// <summary>
        /// 获取url返回数据 Quickly using
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string DownloadDataByWebClient(string url)
        {
            using (WebClient webClient = new WebClient())
            {
                byte[] responseData = webClient.DownloadData(url);
                string srcString = System.Text.Encoding.UTF8.GetString(responseData);//解码  
                return srcString;
            }
        }

    }

}
