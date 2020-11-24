using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace KnifeZ.Tools.EasyService
{
    public class SimpleEmail
    {
        /// <summary>
        /// 发件人邮箱
        /// </summary>
        private static string UserEmailAddress = "KnifeZRed@qq.com";
        /// <summary>
        /// 发件人姓名
        /// </summary>
        private static readonly string UseDisplayName = "KnifeZ";
        /// <summary>
        /// 邮箱密码，不一定是登录密码
        /// </summary>
        private static string password = "";

        public SimpleEmail (string _userEmail, string _password)
        {
            UserEmailAddress = _userEmail;
            password = _password;
        }
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="sendTo"></param>
        /// <param name="sendCopy"></param>
        public static void SendMailByQQ(string subject, string body, string sendTo, string sendCopy = "")
        {

            SmtpClient client = new SmtpClient
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,//指定电子邮件发送方式    
                Host = "smtp.qq.com",//邮件服务器
                EnableSsl = true,
                Port = 587,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(UserEmailAddress, password)//用户名、密码
            };

            using (MailMessage msg = new MailMessage
            {
                From = new MailAddress(UserEmailAddress, UseDisplayName)
            })
            {
                msg.To.Add(sendTo);
                if (!string.IsNullOrWhiteSpace(sendCopy))
                {
                    msg.CC.Add(sendCopy);
                }

                msg.Subject = subject;//邮件标题   
                msg.Body = body;//邮件内容   
                msg.BodyEncoding = Encoding.UTF8;//邮件内容编码   
                msg.IsBodyHtml = false;//是否是HTML邮件   
                msg.Priority = MailPriority.High;//邮件优先级   
                try
                {
                    client.Send(msg);
                }
                finally
                {
                    client.Dispose();
                }
            }
        }
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="sendTo"></param>
        /// <param name="sendCopy"></param>
        /// <param name="sendMail"></param>
        /// <param name="sendPwd"></param>
        /// <param name="priority"></param>
        public static void SendMail(string subject,
            string body,
            string sendTo,
            string sendCopy,
            string sendMail,
            string sendPwd,
            MailPriority priority = MailPriority.Normal)
        {
            string host = sendMail.Split('@')[1];
            SmtpClient client = new SmtpClient
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,//指定电子邮件发送方式    
                Host = "smtp." + host,//邮件服务器
                EnableSsl = true,
                Port = 587,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(sendMail, sendPwd)//用户名、密码
            };
            using (MailMessage msg = new MailMessage
            {
                From = new MailAddress(sendMail, UseDisplayName)
            })
            {
                msg.To.Add(sendTo);
                if (!string.IsNullOrWhiteSpace(sendCopy))
                {
                    msg.CC.Add(sendCopy);
                }

                msg.Subject = subject;//邮件标题   
                msg.Body = body;//邮件内容   
                msg.BodyEncoding = Encoding.UTF8;//邮件内容编码   
                msg.IsBodyHtml = false;//是否是HTML邮件   
                msg.Priority = priority;//邮件优先级   

                try
                {
                    client.Send(msg);
                }
                finally
                {
                    client.Dispose();
                }
            }
        }
    }
}
