using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ZM.Core.Extensions
{
    /// <summary>
    /// 数据操作
    /// </summary>
    public static class DataExtensions
    {
        /// <summary>
        /// byte[] 转字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="strFormat"></param>
        /// <returns></returns>
        public static string ToString(this byte[] bytes, string strFormat = "X2")
        {
            StringBuilder sb = new StringBuilder(40);
            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString(strFormat));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 字符串转成数据流
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static Stream ToStream(this string str, ref long length)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(str);
                    writer.Flush();
                    length = stream.Length;
                    return stream;
                }
            }
        }
        /// <summary>
        /// DataTable 转集合对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static IEnumerable<T> ToEnumerable<T>(this DataTable dt) where T : class, new()
        {
            PropertyInfo[] propertyInfos = typeof(T).GetProperties();
            T[] ts = new T[dt.Rows.Count];
            int i = 0;
            foreach (DataRow row in dt.Rows)
            {
                T t = new T();
                foreach (PropertyInfo p in propertyInfos)
                {
                    if (dt.Columns.IndexOf(p.Name) != -1 && row[p.Name] != DBNull.Value)
                        p.SetValue(t, row[p.Name], null);
                }
                ts[i] = t;
                i++;
            }
            return ts;
        }

        

    }
}
