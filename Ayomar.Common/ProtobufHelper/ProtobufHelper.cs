using System;
using System.IO;

namespace Ayomar.Common.ProtobufHelper
{
    public class ProtobufHelper
    {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="value">object</param>
        /// <returns></returns>
        public static byte[] Serialize(object value)
        {
            using (var ms = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(ms, value);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static byte[] Serialize<T>(T t)
        {
            using (var ms = new MemoryStream())
            {               
                ProtoBuf.Serializer.Serialize<T>(ms, t);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object DeSerialize(byte[] value)
        {
            using (var ms = new MemoryStream(value))
            {
                return ProtoBuf.Serializer.Deserialize<object>(ms);
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T DeSerialize<T>(byte[] value)
        {
            using (var ms = new MemoryStream(value))
            {
                return ProtoBuf.Serializer.Deserialize<T>(ms);
            }
        }

        /// <summary>
        /// Object对象转动态类
        /// </summary>
        /// <param name="obj">Object对象</param>
        /// <returns></returns>
        public static dynamic Object2Dynamic(object obj)
        {
            return DeSerialize<dynamic>(Serialize(obj));
        }
    }
}
