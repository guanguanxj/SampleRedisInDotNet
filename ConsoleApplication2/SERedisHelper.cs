using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StackExchange.Redis;
using System.Configuration;
using Newtonsoft.Json;

namespace ConsoleApplication2
{
    /// <summary>
    /// Redis操作类
    /// </summary>
    public class SERedisHelper
    {
        private static string _conn = ConfigurationManager.AppSettings["SERedis"] ?? "127.0.0.1:6379";
        private static ConnectionMultiplexer _redis;
        private static Object _locker = new Object();
        private static C testC;
        public static ConnectionMultiplexer Manager
        {
            get
            {
                if (_redis == null)
                {
                    lock (_locker)
                    {
                        if (_redis != null) return _redis;

                        _redis = GetManager();
                        return _redis;
                    }
                }

                return _redis;
            }
        }

        private static ConnectionMultiplexer GetManager(string connectionString = null)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = _conn;
            }

            return ConnectionMultiplexer.Connect(connectionString);
        }

        #region string类型
        /// <summary>
        /// 根据Key获取值
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns>System.String.</returns>
        public static string StringGet(string key)
        {
            try
            {
                //using(Manager) //disponse阶段会将Manager.IsConnect置为false，因为static变量，Manager仍旧存在
                //using (testC = new C())
                //{
                //    var s = testC._c;
                //    testC.UseLimitedResource();
                //}
                return Manager.GetDatabase().StringGet(key);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 批量获取值
        /// </summary>
        public static string[] StringGetMany(string[] keyStrs)
        {
            var count = keyStrs.Length;
            var keys = new RedisKey[count];
            var addrs = new string[count];

            for (var i = 0; i < count; i++)
            {
                keys[i] = keyStrs[i];
            }
            try
            {
                var values = Manager.GetDatabase().StringGet(keys);
                for (var i = 0; i < values.Length; i++)
                {
                    addrs[i] = values[i];
                }
                return addrs;
            }
            catch (Exception)
            {
                return null;
            }
        }


        /// <summary>
        /// 单条存值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool StringSet(string key, string value)
        {
            return Manager.GetDatabase().StringSet(key, value);
        }


        /// <summary>
        /// 批量存值
        /// </summary>
        /// <param name="keysStr">key</param>
        /// <param name="valuesStr">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool StringSetMany(string[] keysStr, string[] valuesStr)
        {
            var count = keysStr.Length;
            var keyValuePair = new KeyValuePair<RedisKey, RedisValue>[count];
            for (int i = 0; i < count; i++)
            {
                keyValuePair[i] = new KeyValuePair<RedisKey, RedisValue>(keysStr[i], valuesStr[i]);
            }
            return Manager.GetDatabase().StringSet(keyValuePair);
        }

        #endregion

        #region 泛型
        /// <summary>
        /// 存值并设置过期时间
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">key</param>
        /// <param name="t">实体</param>
        /// <param name="ts">过期时间间隔</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool Set<T>(string key, T t, TimeSpan ts)
        {
            var str = JsonConvert.SerializeObject(t);
            return Manager.GetDatabase().StringSet(key, str, ts);
        }

        /// <summary>
        /// 
        /// 根据Key获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns>T.</returns>
        public static T Get<T>(string key) where T : class
        {
            var strValue = Manager.GetDatabase().StringGet(key);
            return string.IsNullOrEmpty(strValue) ? null : JsonConvert.DeserializeObject<T>(strValue);
        }
        #endregion
    }
    class C : IDisposable
    {
        public string _c { get; set; }
        public C()
        {
            _c = "123";
        }
        public void UseLimitedResource()
        {
            Console.WriteLine("Using limited resource...");
        }

        void IDisposable.Dispose()
        {
            Console.WriteLine("Disposing limited resource.");
        }
    }
}
