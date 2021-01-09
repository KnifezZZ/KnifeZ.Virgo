using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KnifeZ.Virgo.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace KnifeZ.Virgo.Mvc
{
    public class VirgoMiddleware
    {
        private readonly RequestDelegate _next;

        public VirgoMiddleware (RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync (HttpContext context, IOptions<Configs> configs)
        {
            context.Request.EnableBuffering();
            context.Request.Body.Position = 0;
            StreamReader tr = new StreamReader(context.Request.Body);
            string body = tr.ReadToEndAsync().Result;
            context.Request.Body.Position = 0;
            if (context.Items.ContainsKey("DONOTUSE_REQUESTBODY") == false)
            {
                context.Items.Add("DONOTUSE_REQUESTBODY", body);
            }
            else
            {
                context.Items["DONOTUSE_REQUESTBODY"] = body;
            }
            await _next(context);
            if (context.Response.StatusCode == 404)
            {
                await context.Response.WriteAsync(string.Empty);
            }
        }
    }

    public static class VirgoMiddlewareExtensions
    {
        public static IApplicationBuilder UseVirgo (
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<VirgoMiddleware>();
        }
    }
}
