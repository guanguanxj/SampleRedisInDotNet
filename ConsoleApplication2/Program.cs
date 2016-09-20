using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApplication2;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
//using ServiceStack.Redis;
using StackExchange.Redis;

namespace ConsoleApplication2
{
    public class Program
    {
        // static RedisClient redisClient = new RedisClient("127.0.0.1", 6379);//ServiceStack:redis服务IP和端口
        static void Main(string[] args)
        {
           // Console.WriteLine(redisClient.Get<string>("city"));

            //运行时 启动redis-service.exe服务
            SERedisHelper.StringSet("city","Beijing");
            var city = SERedisHelper.StringGet("city");
            
            Console.ReadKey();
        }
    }
}
