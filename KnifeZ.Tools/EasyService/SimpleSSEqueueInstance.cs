using System;
using System.Collections;
using System.Collections.Generic;

namespace KnifeZ.Tools.EasyService
{
    /// <summary>
    /// 长链接消息推送容器实例
    /// </summary>
    public class SimpleSSEqueueInstance
    {
        private static readonly Hashtable singleInstance = new Hashtable();
        private static Queue<dynamic> instance = new Queue<dynamic>();

        private static readonly object LockHelper = new object();
        /// <summary>
        /// 推送给所有在线用户实例
        /// </summary>
        public static Queue<dynamic> Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (LockHelper)
                    {
                        if (instance == null)
                        {
                            instance = new Queue<dynamic>();
                        }
                    }
                }

                return instance;
            }
        }
        /// <summary>
        /// 有限实例，针对单个用户
        /// </summary>
        /// <param name="subID"></param>
        /// <returns></returns>
        public static Queue<dynamic> SingleInstance(long subID = 0)
        {
            if (singleInstance[subID] == null)
            {
                try
                {
                    singleInstance.Add(subID, new Queue<dynamic>());
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return (Queue<dynamic>)singleInstance[subID];
        }
    }
}
