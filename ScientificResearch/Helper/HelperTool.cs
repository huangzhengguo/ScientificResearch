using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ScientificResearch.Helper
{
    public static class HelperTool
    {
        // private static readonly byte POLYNOMIAL = 0x131;
        /// <summary>
        /// 获取本地保存的控制器信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T[] ReadControllersFromFile<T>()
        {
            using (FileStream stream = new FileStream("controllers.dat", FileMode.OpenOrCreate))
            {
                if (stream == null)
                {
                    return null;
                }

                BinaryFormatter binaryFormatter = new BinaryFormatter();
                return binaryFormatter.Deserialize(stream) as T[];
            }
        }

        /// <summary>
        /// 保存控制器信息到文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        public static void WriteControllersToFile<T>(T[] items)
        {
            using (FileStream stream = new FileStream("controllers.dat", FileMode.Open))
            {
                if (stream == null)
                {
                    return;
                }

                BinaryFormatter binaryFormatter = new BinaryFormatter();

                binaryFormatter.Serialize(stream, items.ToArray());
            }
        }

        /// <summary>
        /// 计算校验码
        /// </summary>
        /// <param name="dataBytes"></param>
        /// <returns></returns>
        public static byte GetChecksum(Crc8 crc8, byte[] dataBytes)
        {
            byte crc = crc8.Init;
            byte poly = 0;
            if (crc8.Refin)
            {
                poly = Reflect(crc8.Poly);
                foreach(byte item in dataBytes)
                {
                    crc ^= item;
                    for(int i=0;i<8;i++)
                    {
                        if ((crc & 0x01) != 0)
                        {
                            crc = (byte)((crc >> 1) ^ poly);
                        }
                        else
                        {
                            crc >>= 1;
                        }
                    }
                }
            }
            else
            {
                poly = crc8.Poly;
                foreach (byte item in dataBytes)
                {
                    crc ^= item;
                    for (int i = 0; i < 8; i++)
                    {
                        if ((crc & 0x80) != 0)
                        {
                            crc = (byte)((crc << 1) ^ poly);
                        }
                        else
                        {
                            crc <<= 1;
                        }
                    }
                }
            }

            if (crc8.Refin ^ crc8.Refout)
            {
                crc = Reflect(crc);
            }
            else
            {
                crc ^= crc8.Xorout;
            }

            return crc;
        }

        public static byte Reflect(byte data)
        {
            byte result = 0;

            for(int i=0;i<8;i++)
            {
                result <<= 1;
                if ((data & (1 << i)) != 0)
                {
                    result |= 0x01;
                }
            }

            return result;
        }

        /// <summary>
        /// 转换日期为分钟数
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static int ConverDateTime2Minutes(DateTime dateTime)
        {
            return dateTime.Hour * 60 + dateTime.Minute;
        }

        /// <summary>
        /// 分钟转换为时间字符串
        /// </summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        public static string ConverMinutes2DateTime(int minutes)
        {
            return string.Format("{0:00}:{1:00}", minutes / 60, minutes % 60);
        }

        /// <summary>
        /// 解析控制器id
        /// </summary>
        /// <param name="addressString"></param>
        /// <param name="groupId"></param>
        /// <param name="addressId"></param>
        public static void GetControllerAddressId(string addressString, out int groupId, out int addressId)
        {
            if (string.IsNullOrEmpty(addressString) || string.IsNullOrWhiteSpace(addressString))
            {
                groupId = 0;
                addressId = 0;
                return;
            }

            string[] addressArray = addressString.Split(',');
            if (addressArray.Length == 2)
            {
                groupId = int.Parse(addressArray[0]);
                addressId = int.Parse(addressArray[1]);
            }
            else
            {
                groupId = 0;
                addressId = 0;
            }

            return;
        }

        /// <summary>
        /// 获取通道数量
        /// </summary>
        /// <returns>通道数量</returns>
        public static int GetChannelNum()
        {
            return 6;
        }

        /// <summary>
        /// 根据通道数量获取所有通道默认颜色值
        /// </summary>
        /// <param name="channelNum"></param>
        /// <returns></returns>
        public static List<int>GetDefaultColorValues(int channelNum)
        {
            List<int> colorValues = new List<int>(channelNum);

            for(int i=0;i<channelNum;i++)
            {
                colorValues.Add(0);
            }

            return colorValues;
        }

        /// <summary>
        /// 获取当前时间分钟数
        /// </summary>
        /// <returns></returns>
        public static int GetDefaultMinutes()
        {
            return GetMinutes(DateTime.Now);
        }

        /// <summary>
        /// 获取时间分钟数
        /// </summary>
        /// <returns></returns>
        public static int GetMinutes(DateTime dateTime)
        {
            return dateTime.Hour * 60 + dateTime.Minute;
        }
    }

    public class Crc8
    {
        public byte Poly { get; set; }
        public byte Init { get; set; }
        public byte Xorout { get; set; }
        public bool Refin { get; set; }
        public bool Refout { get; set; }
    }
}
