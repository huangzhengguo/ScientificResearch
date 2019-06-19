using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace ScientificResearch.Helper
{
    public static class SerialPortHelper
    {
        // 所有需要读取数据长度
        private static readonly int headLength = 5;
        private static readonly int dataAllToRead = 96;
        private static readonly int checkCodeLength = 1;
        private static int receDataLength = -1;
        private static List<byte> recData = null;
        // 标记数据是否读取完毕
        private static bool isReadAll = false;
        // 解析数据代理
        public static Action<byte[]> ParseReData;
        private static SerialPort serialPort = new SerialPort();

        /// <summary>
        /// crc8
        /// </summary>
        private static readonly Crc8 crc8 = new Crc8()
        {
            Poly = 0x31,
            Init = 0x00,
            Xorout = 0x00,
            Refin = true,
            Refout = true
        };

        /// <summary>
        /// 获取串口对象
        /// </summary>
        /// <param name="protName">串口名称</param>
        /// <returns></returns>
        public static SerialPort GetSerialPort(string portName)
        {
            if (string.IsNullOrEmpty(portName) || string.IsNullOrWhiteSpace(portName))
            {
                return null;
            }

            // 需要关闭端口后设置
            serialPort.Close();

            serialPort.PortName = portName;
            serialPort.BaudRate = 9600;
            serialPort.Parity = Parity.None;
            serialPort.DataBits = 8;
            serialPort.StopBits = StopBits.One;

            // 接收数据回调
            serialPort.DataReceived += SerialPort_DataReceived;

            return serialPort;
        }

        private static void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort serialPort = (SerialPort)sender;
            // 接收到数据之前，需要设置接收的数据长度
            int bytesToRead = serialPort.BytesToRead;
            byte[] ReDatas = new byte[bytesToRead];
            serialPort.Read(ReDatas, 0, bytesToRead);
            if (bytesToRead == 0)
            {
                return;
            }

            if (isReadAll == false)
            {
                return;
            }

            receDataLength += bytesToRead;
            Console.WriteLine("接收数据长度:" + receDataLength);
            if (recData == null)
            {
                recData = new List<byte>();
            }

            recData.AddRange(ReDatas);
            if ((receDataLength + 1) == (headLength + dataAllToRead + checkCodeLength) && HelperTool.GetChecksum(crc8, recData.ToArray()) == 0)
            {
                // 解析数据
                ParseReData(recData.ToArray());

                receDataLength = -1;
                recData.Clear();
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="serialPort">串口</param>
        /// <param name="txData">要发送的数据</param>
        /// <param name="isWriteData">是否是写数据</param>
        public static void SendData(SerialPort serialPort, byte[] txData, bool isWriteData = true)
        {
            if (serialPort == null)
            {
                return;
            }

            List<byte> sendData = new List<byte>();

            sendData.AddRange(txData);
            sendData.Add(HelperTool.GetChecksum(crc8, txData));

            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }

            if (!serialPort.IsOpen)
            {
                serialPort.Open();
            }

            receDataLength = -1;
            recData?.Clear();

            serialPort.DiscardOutBuffer();
            if (isWriteData == false)
            {
                isReadAll = true;
                serialPort.DiscardInBuffer();
            }
            else
            {
                isReadAll = false;
            }

            serialPort.Write(sendData.ToArray(), 0, sendData.Count());

            Console.WriteLine("发送数据！");
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="serialPort"></param>
        /// <param name="groupId"></param>
        /// <param name="addressId"></param>
        /// <param name="dataToRead"></param>
        public static void ReadData(SerialPort serialPort, int groupId, int addressId, int dataToRead)
        {
            byte[] txData = { (byte)groupId, (byte)addressId, 0x03, 0x10, (byte)dataToRead };

            SendData(serialPort, txData, false);
        }

        /// <summary>
        /// 读取所有数据
        /// </summary>
        /// <param name="serialPort"></param>
        /// <param name="groupId"></param>
        /// <param name="addressId"></param>
        /// <param name="dataToRead"></param>
        public static void ReadAllData(SerialPort serialPort, int groupId, int addressId)
        {
            ReadData(serialPort, groupId, addressId, dataAllToRead);
        }
    }
}
