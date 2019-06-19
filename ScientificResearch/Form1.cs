using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using ScientificResearch.Helper;
using ScientificResearch.Models;
using System.Runtime.Serialization.Formatters.Binary;

namespace ScientificResearch
{
    public partial class Form1 : Form
    {
        public delegate void InitMainUI();
        Action<List<int>> setTrackBar;
        Action<int, List<int>> setOffTimerDataGridView;
        Action<int, List<int>, List<List<int>>> setOnTimerDataGridView;

        // 串口
        private SerialPort serialPort = new SerialPort();
        private readonly Crc8 crc8 = new Crc8()
        {
            Poly = 0x31,
            Init = 0x00,
            Xorout = 0x00,
            Refin = true,
            Refout = true
        };
        private readonly string tipMessage = "请选择串口！";
        private readonly string powerOnStr = "打开";
        private readonly string powerOffStr = "关闭";
        private readonly float maxColorValue = 1000.0f;
        private readonly int maxOffTimerCount = 8;
        private readonly int maxOnTimerCount = 8;
        private readonly int headLength = 5;
        private readonly int dataToRead = 96;
        private readonly int checkCodeLength = 1;
        private int receDataLength = -1;
        private List<byte> recData = null;
        private bool isReadAll = true;

        // 当前控制器
        private ControllerItem currentControllerItem;
        // 当前定时器索引
        private int currentOffTimerIndex = 0;
        private int currentOnTimerIndex = 0;
        public Form1()
        {
            InitializeComponent();

            setTrackBar = SetColorTrackBar;
            setOffTimerDataGridView = SetOffTimerDataGridView;
            setOnTimerDataGridView = SetOnTimerDataGridView;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 读取已经添加过的控制器
            LoadControllerItems();
            portComboBox.Items.AddRange(SerialPort.GetPortNames());
            portComboBox.Text = serialPort.PortName;
        }

        private void LoadControllerItems()
        {
            var items = HelperTool.ReadControllersFromFile<ControllerItem>();

            if (items != null)
            {
                controllersDataGridView.DataSource = items;
            }

            controllersDataGridView.Columns["GroupName"].Visible = false;

            if (items.Count() > 0)
            {
                currentControllerItem = items[0];
            }
        }

        /// <summary>
        /// 添加控制器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addButton_Click(object sender, EventArgs e)
        {
            AddControllerFrom addControllerFrom = new AddControllerFrom();

            addControllerFrom.PassGroupAddressId = (int groupId, int addressId, string name, string zone) =>
            {
                ControllerItem controllerItem = new ControllerItem()
                {
                    GroupId = groupId,
                    AddressId = addressId
                };

                var items = HelperTool.ReadControllersFromFile<ControllerItem>().ToList();

                foreach(var item in items)
                {
                    if (item.GroupId == groupId && item.AddressId == addressId)
                    {
                        MessageBox.Show("已经添加，不能重复添加！");
                        return false;
                    }
                }

                items.Add(controllerItem);

                Stream stream = File.Open("controllers.dat", FileMode.Open);
                BinaryFormatter binaryFormatter = new BinaryFormatter();

                binaryFormatter.Serialize(stream, items.ToArray());

                stream.Close();

                LoadControllerItems();

                return true;
            };

            addControllerFrom.StartPosition = FormStartPosition.CenterParent;
            addControllerFrom.ShowDialog();
        }

        /// <summary>
        /// 删除控制器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteControllerButton_Click(object sender, EventArgs e)
        {
            var controllerItems = HelperTool.ReadControllersFromFile<ControllerItem>().ToList();

            if (currentControllerItem == null)
            {
                return;
            }

            if (controllerItems == null || controllerItems.Count <= 0)
            {
                MessageBox.Show("没有可以删除的项！");
                return;
            }

            if (MessageBox.Show("确认删除?","确认删除项？", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }

            List<ControllerItem> items = new List<ControllerItem>();
            foreach(var item in controllerItems)
            {
                if (item.GroupId != currentControllerItem.GroupId || item.AddressId != currentControllerItem.AddressId)
                {
                    items.Add(item);
                }
            }

            HelperTool.WriteControllersToFile<ControllerItem>(items.ToArray());
            LoadControllerItems();
        }

        /// <summary>
        /// 串口选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void portComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Console.WriteLine("选择串口!");
            ComboBox comboBox = (ComboBox)sender;
            if (string.IsNullOrEmpty(comboBox.Text) || string.IsNullOrWhiteSpace(comboBox.Text))
            {
                MessageBox.Show(tipMessage);
                return;
            }

            // 需要关闭端口后设置
            serialPort.Close();

            serialPort.PortName = comboBox.Text;
            serialPort.BaudRate = 9600;
            serialPort.Parity = Parity.None;
            serialPort.DataBits = 8;
            serialPort.StopBits = StopBits.One;

            // 数据接收回调
            serialPort.DataReceived += SerialPort_DataReceived;

            // 同步时间
            byte[] txData = { 0x00, 0x00, 0x10, 0x01, 0x07, (byte)(DateTime.Now.Year-2000),
                             (byte)DateTime.Now.Month, (byte)DateTime.Now.Day, (byte)DateTime.Now.DayOfWeek, (byte)DateTime.Now.Hour, (byte)DateTime.Now.Minute,
                             (byte)DateTime.Now.Second };

            SendData(txData);
        }

        /// <summary>
        /// 选择控制器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void controllersDataGridView_CurrentCellChanged(object sender, EventArgs e)
        {
            DataGridView dataGridView = (DataGridView)sender;
            var items = HelperTool.ReadControllersFromFile<ControllerItem>();
            if (items == null || items.Count() == 0)
            {
                return;
            }

            if (dataGridView.CurrentRow == null)
            {
                return;
            }

            Console.WriteLine("选择了第 {0} 行！", dataGridView.CurrentRow.Index);
            currentControllerItem = items[dataGridView.CurrentRow.Index];

            // 读取当前设备的设置
            ReadCurrentControllerAllData(currentControllerItem);
        }

        private void ReadCurrentControllerAllData(ControllerItem controllerItem)
        {
            byte[] txData = { (byte)controllerItem.GroupId, (byte)controllerItem.AddressId, 0x03, 0x10, (byte)dataToRead };

            SendData(txData, false);
        }

        /// <summary>
        /// 收到数据回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Console.WriteLine("收到数据: " + serialPort.BytesToRead);
            // 接收到数据之前，需要设置接收的数据长度
            byte[] ReDatas = new byte[serialPort.BytesToRead];
            int bytesToRead = serialPort.BytesToRead;
            serialPort.Read(ReDatas, 0, serialPort.BytesToRead);
            
            // 解析数据
            ParseReData(ref currentControllerItem, bytesToRead, ReDatas);
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="controllerItem"></param>
        /// <param name="rcDataLength"></param>
        /// <param name="reDatas"></param>
        private void ParseReData(ref ControllerItem controllerItem, int rcDataLength, byte[] reDatas)
        {
            if (rcDataLength == 0)
            {
                return;
            }

            if (isReadAll == false)
            {
                return;
            }

            receDataLength = receDataLength + rcDataLength;
            Console.WriteLine("接收数据长度:" + receDataLength);
            if (recData == null)
            {
                recData = new List<byte>();
            }

            recData.AddRange(reDatas);
            if ((receDataLength + 1) == (headLength + dataToRead + checkCodeLength))
            {
                receDataLength = -1;
                // 接收完毕，开始解析
                int startIndex = 5;

                // 解析开关状态
                controllerItem.PowerState = recData[startIndex] == 0 ? false : true;

                // 解析各路数据
                controllerItem.LightsColorValues = new List<int>();
                for (int i = 0; i < 6; i++)
                {
                    startIndex = startIndex + 2;
                    int prev = (recData[startIndex] << 8);
                    int next = recData[startIndex - 1];
                    controllerItem.LightsColorValues.Add(prev | next);
                }

                startIndex = startIndex + 4;
                // 解析关闭定时器
                controllerItem.OffTimerCount = 0;
                controllerItem.OffTimersMinutes = new List<int>();
                for (int i = 0; i < 8; i++)
                {
                    if (recData[startIndex + 1] == 255 || recData[startIndex] == 255)
                    {
                        startIndex = startIndex + 2;
                        continue;
                    }

                    int prev = (recData[startIndex + 1] << 8);
                    int next = recData[startIndex];
                    controllerItem.OffTimersMinutes.Add(prev | next);

                    startIndex = startIndex + 2;

                    controllerItem.OffTimerCount++;
                }

                // 解析打开定时器
                controllerItem.OnTimerCount = 0;
                controllerItem.OnTimersMinutes = new List<int>();
                controllerItem.OnTimersColorValues = new List<List<int>>();
                for (int i = 0; i < 8; i++)
                {
                    if (recData[startIndex + 1] == 255 || recData[startIndex] == 255)
                    {
                        startIndex = startIndex + 8;
                        continue;
                    }

                    int prev = (recData[startIndex + 1] << 8);
                    int next = recData[startIndex];
                    controllerItem.OnTimersMinutes.Add(prev | next);

                    List<int> colorValues = new List<int>();
                    for (int j = 0; j < 6; j++)
                    {
                        colorValues.Add(recData[startIndex + 2]);
                        startIndex = startIndex + 1;
                    }

                    controllerItem.OnTimersColorValues.Add(colorValues);
                    controllerItem.OnTimerCount++;
                    startIndex = startIndex + 2;
                }

                Console.WriteLine("解析完毕");

                recData = null;

                //// 颜色条设置
                Invoke(setTrackBar, currentControllerItem.LightsColorValues);

                //// 设置关闭定时器信息
                Invoke(setOffTimerDataGridView, currentControllerItem.OffTimerCount, currentControllerItem.OffTimersMinutes);

                //// 设置打开定时器信息
                Invoke(setOnTimerDataGridView, currentControllerItem.OnTimerCount, currentControllerItem.OnTimersMinutes, currentControllerItem.OnTimersColorValues);
            }
        }

        /// <summary>
        /// 开关
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void powerButton_Click(object sender, EventArgs e)
        {
            Button powerBtn = (Button)sender;
            byte powerByte = 0x00;
            if (string.Compare(powerBtn.Text, powerOnStr, false) == 0)
            {
                powerByte = 0x01;
                powerBtn.Text = powerOffStr;
            }
            else
            {
                powerBtn.Text = powerOnStr;
            }

            byte[] txData = { (byte)currentControllerItem.GroupId, (byte)currentControllerItem.AddressId, 0x10, 0x10, 0x01, powerByte };

            SendData(txData);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="txData">要发送的数据</param>
        private void SendData(byte[] txData, bool isWriteData = true)
        {
            List<byte> sendData = new List<byte>();

            sendData.AddRange(txData);
            sendData.Add(HelperTool.GetChecksum(crc8, txData));

            if (!serialPort.IsOpen)
            {
                serialPort.Open();
            }

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
        /// 设置颜色滑动条
        /// </summary>
        /// <param name="colorValues"></param>
        private void SetColorTrackBar(List<int> colorValues)
        {
            powerButton.Text = currentControllerItem.PowerState == true ? powerOffStr : powerOnStr;

            TrackBar[] trackBars = { trackBar1, trackBar2, trackBar3, trackBar4, trackBar5, trackBar6 };
            if (colorValues == null || colorValues.Count != trackBars.Length)
            {
                return;
            }

            for(int i=0;i<trackBars.Length;i++)
            {
                trackBars[i].Value = colorValues[i];
            }
        }

        /// <summary>
        /// 设置关断时间点列表
        /// </summary>
        /// <param name="offTimerCount"></param>
        /// <param name="offTimersMinutes"></param>
        private void SetOffTimerDataGridView(int offTimerCount, List<int> offTimersMinutes)
        {
            offTimerDataGridView.ColumnCount = 1;
            offTimerDataGridView.Columns[0].Name = "时间点";
            offTimerDataGridView.Rows.Clear();

            for (int i = 0; i < currentControllerItem.OffTimerCount; i++)
            {
                offTimerDataGridView.Rows.Add(HelperTool.ConverMinutes2DateTime(currentControllerItem.OffTimersMinutes[i]));
            }
        }

        /// <summary>
        /// 添加关闭定时器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addOffTimerButton_Click(object sender, EventArgs e)
        {
            if (currentControllerItem.OffTimerCount >= 8)
            {
                MessageBox.Show("最多8个时间点！");
                return;
            }

            AddOffTimerForm addOffTimerForm = new AddOffTimerForm();

            addOffTimerForm.PassMinutes = (int minutes) =>
            {
                // 添加时间点到关闭时间点中
                byte[] minutesBytes = BitConverter.GetBytes(minutes);
                int startAddress = 0x20 + currentControllerItem.OffTimerCount * 2;
                byte[] txData = { (byte)currentControllerItem.GroupId, (byte)currentControllerItem.AddressId, 0x10, (byte)startAddress, 0x02, minutesBytes[0], minutesBytes[1] };

                SendData(txData);

                currentControllerItem.OffTimerCount += 1;
                currentControllerItem.OffTimersMinutes.Add(minutes);

                Invoke(setOffTimerDataGridView, currentControllerItem.OffTimerCount, currentControllerItem.OffTimersMinutes);
             };

            addOffTimerForm.ShowDialog();
        }

        /// <summary>
        /// 删除关闭时间点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteOffTimerButton_Click(object sender, EventArgs e)
        {
            if (currentControllerItem.OffTimerCount == 0)
            {
                MessageBox.Show("没有时间点可以删除");
                return;
            }

            if (MessageBox.Show("确认删除？","删除时间点", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }

            currentControllerItem.OffTimersMinutes.RemoveAt(currentOffTimerIndex);

            int startAddress = 0x20;
            byte[] minutesBytes;
            List<byte> txData = new List<byte>() { (byte)currentControllerItem.GroupId, (byte)currentControllerItem.AddressId, 0x10,
                                                   (byte)startAddress, (byte)(maxOffTimerCount * 2) };
            // 发送命令
            for (int i=0;i<maxOffTimerCount;i++)
            {
                if (i >= currentControllerItem.OffTimersMinutes.Count)
                {
                    txData.Add(0xff);
                    txData.Add(0xff);
                }
                else
                {
                    minutesBytes = BitConverter.GetBytes(currentControllerItem.OffTimersMinutes[i]);

                    txData.Add(minutesBytes[0]);
                    txData.Add(minutesBytes[1]);
                }
            }

            SendData(txData.ToArray());
            currentControllerItem.OffTimerCount--;
            SetOffTimerDataGridView(currentControllerItem.OffTimersMinutes.Count, currentControllerItem.OffTimersMinutes);
        }

        /// <summary>
        /// 设置打开时间点列表
        /// </summary>
        /// <param name="onTimerCount"></param>
        /// <param name="onTimersMinutes"></param>
        private void SetOnTimerDataGridView(int onTimerCount, List<int> onTimersMinutes, List<List<int>> onTimersColorValues)
        {
            onTimerDataGridView.Rows.Clear();

            onTimerDataGridView.ColumnCount = 7;
            onTimerDataGridView.Columns[0].Name = "时间";
            onTimerDataGridView.Columns[1].Name = "Channel 1";
            onTimerDataGridView.Columns[2].Name = "Channel 2";
            onTimerDataGridView.Columns[3].Name = "Channel 3";
            onTimerDataGridView.Columns[4].Name = "Channel 4";
            onTimerDataGridView.Columns[5].Name = "Channel 5";
            onTimerDataGridView.Columns[6].Name = "Channel 6";

            for (int i=0;i<onTimerCount;i++)
            {
                List<string> row = new List<string>();

                row.Add(HelperTool.ConverMinutes2DateTime(onTimersMinutes[i]));
                foreach (var item in onTimersColorValues[i])
                {
                    row.Add(item.ToString());
                }

                onTimerDataGridView.Rows.Add(row.ToArray());
            }
        }

        /// <summary>
        /// 添加打开定时器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addOnTimerButton_Click(object sender, EventArgs e)
        {
            if (currentControllerItem.OnTimerCount >= 8)
            {
                MessageBox.Show("最多8个时间点!");
                return;
            }

            AddOnTimerForm addOnTimerForm = new AddOnTimerForm();

            // 添加打开定时器回调
            addOnTimerForm.PassOnTimer = (int minutes, int ch1, int ch2, int ch3, int ch4, int ch5, int ch6) =>
            {
                byte[] minutesBytes = BitConverter.GetBytes(minutes);
                int startAddress = 0x30 + 8 * currentControllerItem.OnTimerCount;
                byte[] txData = { (byte)currentControllerItem.GroupId, (byte)currentControllerItem.AddressId, 0x10, (byte)startAddress, 0x08,
                                  minutesBytes[0], minutesBytes[1],
                                  (byte)ch1, (byte)ch2, (byte)ch3, (byte)ch4, (byte)ch5, (byte)ch6 };

                SendData(txData);

                currentControllerItem.OnTimerCount++;
                currentControllerItem.OnTimersMinutes.Add(minutes);
                currentControllerItem.OnTimersColorValues.Add(new List<int> { ch1, ch2, ch3, ch4, ch5, ch6 });
                SetOnTimerDataGridView(currentControllerItem.OnTimerCount, currentControllerItem.OnTimersMinutes, currentControllerItem.OnTimersColorValues);
            };

            addOnTimerForm.ShowDialog();
        }

        /// <summary>
        /// 删除打开时间点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteOnTimerButton_Click(object sender, EventArgs e)
        {
            if (currentControllerItem.OnTimerCount == 0)
            {
                MessageBox.Show("没有时间点可以删除");
                return;
            }

            if (MessageBox.Show("确认删除？", "删除时间点", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }

            currentControllerItem.OnTimerCount--;
            currentControllerItem.OnTimersMinutes.RemoveAt(currentOnTimerIndex);
            currentControllerItem.OnTimersColorValues.RemoveAt(currentOnTimerIndex);

            int startAddress = 0x30;
            byte[] minutesBytes;
            List<byte> txData = new List<byte>() { (byte)currentControllerItem.GroupId, (byte)currentControllerItem.AddressId, 0x10,
                                                   (byte)startAddress, (byte)(maxOnTimerCount * 8) };
            // 构造发送命令
            for (int i = 0; i < maxOnTimerCount; i++)
            {
                if (i >= currentControllerItem.OffTimersMinutes.Count)
                {
                    txData.Add(0xff);
                    txData.Add(0xff);
                    txData.AddRange(new byte[] { 0,0,0,0,0,0 });
                }
                else
                {
                    if (currentControllerItem.OnTimersColorValues.Count <= 0)
                    {
                        txData.Add(0xff);
                        txData.Add(0xff);
                        txData.AddRange(new byte[] { 0, 0, 0, 0, 0, 0 });
                    }
                    else
                    {
                        minutesBytes = BitConverter.GetBytes(currentControllerItem.OffTimersMinutes[i]);

                        txData.Add(minutesBytes[0]);
                        txData.Add(minutesBytes[1]);
                        foreach (var item in currentControllerItem.OnTimersColorValues[i])
                        {
                            txData.Add((byte)item);
                        }
                    }
                }
            }

            SendData(txData.ToArray());
            SetOnTimerDataGridView(currentControllerItem.OnTimerCount, currentControllerItem.OnTimersMinutes, currentControllerItem.OnTimersColorValues);
        }

        /// <summary>
        /// 滑动结束时发送命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackBar1_MouseUp(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrEmpty(serialPort.PortName) || string.IsNullOrWhiteSpace(serialPort.PortName))
            {
                MessageBox.Show(tipMessage);
                return;
            }

            TrackBar trackBar = (TrackBar)sender;

            // 滑动条索引
            int trackBarIndex = (int.Parse(trackBar.Tag.ToString()) - 10000);
            // 寄存器地址
            int registerAddr = 0x10 + trackBarIndex * 2 - 1;

            if (!serialPort.IsOpen)
            {
                serialPort.Open();
            }

            Int16 colorValue = (Int16)trackBar.Value;

            byte[] colorValueBytes = BitConverter.GetBytes(colorValue);
            // 发送数据
            byte[] txData = { (byte)currentControllerItem.GroupId, (byte)currentControllerItem.AddressId, 0x10, (byte)registerAddr, 0x02, colorValueBytes[0], colorValueBytes[1] };

            SendData(txData);
        }

        /// <summary>
        /// 滑动条值变化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            TrackBar trackBar = (TrackBar)sender;
            // 更新百分比
            UpdatePercentLabel(trackBar.Value, (int.Parse(trackBar.Tag.ToString()) - 10000));
        }

        /// <summary>
        /// 更新百分比显示
        /// </summary>
        /// <param name="value"></param>
        /// <param name="index"></param>
        private void UpdatePercentLabel(int value, int index)
        {
            // 百分比
            Label[] percentLabels = { label8, label9, label10, label11, label12, label13 };
            percentLabels[index - 1].Text = string.Format("{0:0.00%}", (float)value / maxColorValue);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            portComboBox.Items.Clear();
            portComboBox.Items.AddRange(SerialPort.GetPortNames());
        }

        private void offTimerDataGridView_CurrentCellChanged(object sender, EventArgs e)
        {
            DataGridView dataGridView = (DataGridView)sender;

            if (dataGridView.CurrentRow == null)
            {
                return;
            }

            currentOffTimerIndex = dataGridView.CurrentRow.Index;
        }

        private void onTimerDataGridView_CurrentCellChanged(object sender, EventArgs e)
        {
            DataGridView dataGridView = (DataGridView)sender;
            if (dataGridView.CurrentRow == null)
            {
                return;
            }
            currentOnTimerIndex = dataGridView.CurrentRow.Index;
        }

        /// <summary>
        /// 获取示例数据
        /// </summary>
        private void GetExampleData()
        {
            currentControllerItem.GroupId = 1;
            currentControllerItem.AddressId = 1;
            currentControllerItem.GroupName = "分组名称";
            currentControllerItem.PowerState = true;
            currentControllerItem.LightsColorValues = new List<int> { 100, 10, 200, 20, 300, 800 };
            currentControllerItem.OffTimerCount = 3;
            currentControllerItem.OffTimersMinutes = new List<int> { 360, 720, 1080 };
            currentControllerItem.OnTimerCount = 3;
            currentControllerItem.OnTimersMinutes = new List<int> { 653, 592, 1080 };
            currentControllerItem.OnTimersColorValues = new List<List<int>>
            {
                new List<int> { 100, 100, 100, 100, 100, 100 },
                new List<int> { 100,50, 100, 30, 100, 100 },
                new List<int> { 100, 10, 100, 20, 10, 100 }
            };
        }
    }
}
