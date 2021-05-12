using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ZM.Core.Utilities
{
    public static class DataHelper
    {
        /// <summary>
        /// 生成单个随机数字
        /// </summary>
        private static int createNum()
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            int num = random.Next(10);
            return num;
        }

        /// <summary>
        /// 生成单个大写随机字母
        /// </summary>
        private static string createBigAbc()
        {
            //A-Z的 ASCII值为65-90
            Random random = new Random(Guid.NewGuid().GetHashCode());
            int num = random.Next(65, 91);
            string abc = Convert.ToChar(num).ToString();
            return abc;
        }

        /// <summary>
        /// 生成单个小写随机字母
        /// </summary>
        private static string createSmallAbc()
        {
            //a-z的 ASCII值为97-122
            Random random = new Random(Guid.NewGuid().GetHashCode());
            int num = random.Next(97, 123);
            string abc = Convert.ToChar(num).ToString();
            return abc;
        }

        /// <summary>
        /// 生成随机字符串
        /// </summary>
        /// <param name="length">字符串的长度</param>
        /// <returns></returns>
        public static string CreateRandomStr(int length)
        {
            // 创建一个StringBuilder对象存储密码
            StringBuilder sb = new StringBuilder();
            //使用for循环把单个字符填充进StringBuilder对象里面变成14位密码字符串
            for (int i = 0; i < length; i++)
            {
                Random random = new Random(Guid.NewGuid().GetHashCode());
                //随机选择里面其中的一种字符生成
                switch (random.Next(3))
                {
                    case 0:
                        //调用生成生成随机数字的方法
                        sb.Append(createNum());
                        break;
                    case 1:
                        //调用生成生成随机小写字母的方法
                        sb.Append(createSmallAbc());
                        break;
                    case 2:
                        //调用生成生成随机大写字母的方法
                        sb.Append(createBigAbc());
                        break;
                }
            }
            return sb.ToString();
        }


        /// <summary>
        /// byte[] 转字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="strFormat"></param>
        /// <returns></returns>
        public static string BytesToString(byte[] bytes, string strFormat = "X2")
        {
            StringBuilder sb = new StringBuilder(40);
            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString(strFormat));
            }
            return sb.ToString();
        }

        public static byte[] StringToBytes(string str, Encoding encoding)
        {
            if (encoding == null) { encoding = Encoding.UTF8; }
            return encoding.GetBytes(str);
        }
        public static byte[] StringToBytes(string str)
        {
            return StringToBytes(str,null);
        }

        /// <summary>
        /// 字符串转成数据流
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static Stream ToStream(string str)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(str);
                    writer.Flush();
                    return stream;
                }
            }
        }
        /// <summary>
        /// DataTable 转集合对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static IEnumerable<T> ToEnumerable<T>(DataTable dt) where T : class, new()
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
