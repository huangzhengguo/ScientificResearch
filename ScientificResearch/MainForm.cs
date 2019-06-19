using System;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;
using ScientificResearch.Models;
using ScientificResearch.Helper;
using System.Runtime.Serialization.Formatters.Binary;

namespace ScientificResearch
{
    public partial class MainForm : Form
    {
        // 串口
        private SerialPort serialPort = null;
        private readonly Crc8 crc8 = new Crc8()
        {
            Poly = 0x31,
            Init = 0x00,
            Xorout = 0x00,
            Refin = true,
            Refout = true
        };

        // 所有控制器
        private List<ControllerItem> controllerItems = new List<ControllerItem>();

        // 当前控制器
        private ControllerItem currentControllerItem;

        private readonly int[] colsLeft = { 18, 82, 157, 268, 398 };
        private readonly int rowHeight = 20;
        private readonly int rowInterval = 15;

        // 只显示10个控制器，当前第几页
        private int currentPageIndex = 0;
        private readonly int pageControllerNum = 10;

        // 当前读取的控制器
        private ControllerItem controllerToRead;
        private int controllerToReadIndex = 0;

        private Action reLoadControllerItems;

        public MainForm()
        {
            InitializeComponent();

            reLoadControllerItems = ReLoadControllerItems;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            RefreshPortList();

            ReLoadControllerItems();
        }

        /// <summary>
        /// 刷新端口列表
        /// </summary>
        private void RefreshPortList()
        {
            comComboBox.Items.Clear();
            comComboBox.Items.AddRange(SerialPort.GetPortNames());
        }

        private void ReLoadControllerItems()
        {
            controllerItems.Clear();
            // 读取控制器列表
            controllerItems.AddRange(HelperTool.ReadControllersFromFile<ControllerItem>().OrderBy(m => m.GroupId));

            // 创建控制器列表
            LoadControllerItems(controllerItems.ToArray(), currentPageIndex * pageControllerNum);
        }

        /// <summary>
        /// 加载并创建控制器列表
        /// </summary>
        /// <param name="startIndex">开始索引</param>
        private void LoadControllerItems(ControllerItem[] items, int startIndex)
        {
            if (items == null)
            {
                return;
            }

            controllerListPanel.Controls.Clear();

            // 添加上一页按钮
            Button prePageBtn = new Button();

            prePageBtn.Text = "Pre Page";
            prePageBtn.Location = new Point(7, pageControllerNum * (rowHeight + rowInterval));
            prePageBtn.Size = new Size(100, rowHeight);
            prePageBtn.Click += PrePageBtn_Click;

            controllerListPanel.Controls.Add(prePageBtn);

            // 添加下一页按钮
            Button nextPageBtn = new Button();

            nextPageBtn.Text = "Next Page";
            nextPageBtn.Location = new Point(107, pageControllerNum * (rowHeight + rowInterval));
            nextPageBtn.Size = new Size(100, rowHeight);
            nextPageBtn.Click += NextPageBtn_Click;

            controllerListPanel.Controls.Add(nextPageBtn);

            for (int i=startIndex; i<items.Count();i++)
            {
                if (i-startIndex>=pageControllerNum)
                {
                    if (i<items.Count())
                    {
                        nextPageBtn.Enabled = true;
                    }
                    else
                    {
                        nextPageBtn.Enabled = false;
                    }

                    if (i>(currentPageIndex+1) * pageControllerNum)
                    {
                        prePageBtn.Enabled = true;
                    }
                    else
                    {
                        prePageBtn.Enabled = false;
                    }

                    break;
                }

                var item = items[i];

                var topX = rowInterval + (i % pageControllerNum) * (rowHeight + rowInterval);
                // 动态创建控制器列表
                // 名称列
                Label nameLabel = new Label();

                nameLabel.Text = item.Name;
                nameLabel.Location = new Point(colsLeft[0], topX);
                nameLabel.Size = new Size(colsLeft[1] - colsLeft[0], rowHeight);
                controllerListPanel.Controls.Add(nameLabel);

                // 区域列
                Label zoneLabel = new Label();

                zoneLabel.Text = item.Zone;
                zoneLabel.Location = new Point(colsLeft[1], topX);
                zoneLabel.Size = new Size(colsLeft[2] - colsLeft[1], rowHeight);
                controllerListPanel.Controls.Add(zoneLabel);

                // 地址列
                Label addressLabel = new Label();

                addressLabel.Text = item.GroupId + "," + item.AddressId;
                addressLabel.Location = new Point(colsLeft[2], topX);
                addressLabel.Size = new Size(colsLeft[3] - colsLeft[2], rowHeight);
                controllerListPanel.Controls.Add(addressLabel);

                // Program Status列
                Label programStatusLabel = new Label();

                programStatusLabel.Text = "N/A";
                programStatusLabel.Location = new Point(colsLeft[3], topX);
                programStatusLabel.Size = new Size(colsLeft[4] - colsLeft[3], rowHeight);
                controllerListPanel.Controls.Add(programStatusLabel);

                // Channel列
                Label channelLabel = new Label();

                channelLabel.Text = "N/A";
                channelLabel.Location = new Point(colsLeft[4], topX);
                channelLabel.Size = new Size(100, rowHeight);
                controllerListPanel.Controls.Add(channelLabel);

                // 设置按钮
                Button settingButton = new Button();

                settingButton.FlatStyle = FlatStyle.Flat;
                settingButton.FlatAppearance.BorderSize = 0;
                settingButton.BackgroundImageLayout = ImageLayout.Stretch;
                settingButton.BackgroundImage = Properties.Resources.set1;
                settingButton.Tag = item.GroupId + "," + item.AddressId;
                settingButton.Location = new Point(colsLeft[4] + 100, topX);
                settingButton.Size = new Size(rowHeight, rowHeight);
                settingButton.Click += SettingButton_Click;

                controllerListPanel.Controls.Add(settingButton);

                // 开关按钮
                Button powerButton = new Button();

                powerButton.FlatStyle = FlatStyle.Flat;
                powerButton.FlatAppearance.BorderSize = 0;
                powerButton.BackgroundImageLayout = ImageLayout.Stretch;
                powerButton.Tag = item.GroupId + "," + item.AddressId;
                powerButton.Location = new Point(colsLeft[4] + 150 + 5, topX);
                powerButton.Size = new Size(rowHeight, rowHeight);
                powerButton.Click += PowerButton_Click;
                if (item.PowerState)
                {
                    powerButton.BackgroundImage = Properties.Resources.powerOn;
                }
                else
                {
                    powerButton.BackgroundImage = Properties.Resources.powerOff;
                }

                controllerListPanel.Controls.Add(powerButton);

                // 开关按钮
                Button deleteButton = new Button();

                deleteButton.FlatStyle = FlatStyle.Flat;
                deleteButton.FlatAppearance.BorderSize = 0;
                deleteButton.BackgroundImageLayout = ImageLayout.Stretch;
                deleteButton.BackgroundImage = Properties.Resources.delete;
                deleteButton.Tag = item.GroupId + "," + item.AddressId;
                deleteButton.Location = new Point(colsLeft[4] + 150 + 55, topX);
                deleteButton.Size = new Size(rowHeight, rowHeight);
                deleteButton.Click += DeleteButton_Click;

                controllerListPanel.Controls.Add(deleteButton);
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Confirm delete?", "Delete", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }

            // 删除控制器
            Button powerBtn = (Button)sender;
            int groupId = 0;
            int addressId = 0;
            HelperTool.GetControllerAddressId(powerBtn.Tag.ToString(), out groupId, out addressId);
            controllerItems.RemoveAll(c => c.GroupId == groupId && c.AddressId == addressId);

            HelperTool.WriteControllersToFile<ControllerItem>(controllerItems.ToArray());

            ReLoadControllerItems();
        }

        private void PrePageBtn_Click(object sender, EventArgs e)
        {
            currentPageIndex--;
            if (currentPageIndex < 0)
            {
                return;
            }

            LoadControllerItems(controllerItems.ToArray(), currentPageIndex * pageControllerNum);
        }

        private void NextPageBtn_Click(object sender, EventArgs e)
        {
            currentPageIndex++;

            LoadControllerItems(controllerItems.ToArray(), currentPageIndex * pageControllerNum);
        }

        /// <summary>
        /// 开关
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PowerButton_Click(object sender, EventArgs e)
        {
            Button powerBtn = (Button)sender;
            int groupId = 0;
            int addressId = 0;
            HelperTool.GetControllerAddressId(powerBtn.Tag.ToString(), out groupId, out addressId);

            currentControllerItem = GetCurrentController(groupId, addressId);
            if (currentControllerItem == null)
            {
                return;
            }

            byte powerByte = 0x01;
            if (currentControllerItem.PowerState)
            {
                powerByte = 0x00;
                powerBtn.BackgroundImage = Properties.Resources.powerOff;
            }
            else
            {
                powerBtn.BackgroundImage = Properties.Resources.powerOn;
            }

            currentControllerItem.PowerState = !currentControllerItem.PowerState;

            byte[] txData = { (byte)currentControllerItem.GroupId, (byte)currentControllerItem.AddressId, 0x10, 0x10, 0x01, powerByte };

            SerialPortHelper.SendData(serialPort, txData);
        }

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingButton_Click(object sender, EventArgs e)
        {
            Button settingBtn = (Button)sender;
            int groupId = 0;
            int addressId = 0;
            HelperTool.GetControllerAddressId(settingBtn.Tag.ToString(), out groupId, out addressId);

            currentControllerItem = GetCurrentController(groupId, addressId);
            if (currentControllerItem == null)
            {
                return;
            }

            SettingControllerForm settingControllerForm = new SettingControllerForm(serialPort, currentControllerItem, controllerItems.ToArray());

            settingControllerForm.saveSetting = () =>
            {
                // 刷新控制器列表
                Invoke(reLoadControllerItems);
            };

            settingControllerForm.ShowDialog();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            AddControllerFrom addControllerFrom = new AddControllerFrom();

            addControllerFrom.PassGroupAddressId = (int groupId, int addressId, string name, string zone) =>
            {
                ControllerItem controllerItem = new ControllerItem()
                {
                    Name = name,
                    Zone = zone,
                    GroupId = groupId,
                    AddressId = addressId
                };

                var items = HelperTool.ReadControllersFromFile<ControllerItem>().ToList();

                foreach (var item in items)
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

                currentPageIndex = 0;
                controllerItems.Add(controllerItem);
                LoadControllerItems(controllerItems.ToArray(), 0);

                return true;
            };

            addControllerFrom.StartPosition = FormStartPosition.CenterParent;
            addControllerFrom.ShowDialog();
        }

        private void ComComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var comboBox = (ComboBox)sender;

            // 需要关闭端口后设置
            serialPort = SerialPortHelper.GetSerialPort(comboBox.Text);

            // 数据接收回调
            SerialPortHelper.ParseReData = (byte[] data) =>
            {
                Console.WriteLine("解析数据！");
                controllerToRead.ParseDataToControllerItem(data);

                // 解析完成后，读取下一个控制器数据
                // ReadControllerData(serialPort, ++controllerToReadIndex);
            };

            // 同步时间
            byte[] txData = { 0x00, 0x00, 0x10, 0x01, 0x07, (byte)(DateTime.Now.Year-2000),
                             (byte)DateTime.Now.Month, (byte)DateTime.Now.Day, (byte)DateTime.Now.DayOfWeek, (byte)DateTime.Now.Hour, (byte)DateTime.Now.Minute,
                             (byte)DateTime.Now.Second };

            SerialPortHelper.SendData(serialPort, txData);

            Thread.Sleep(100);

            // 读取第一个控制器信息
            ReadControllerData(serialPort, 0);
        }

        private void ReadControllerData(SerialPort serialPort, int controllerIndex)
        {
            if (controllerIndex < controllerItems.Count())
            {
                controllerToRead = controllerItems[controllerToReadIndex];

                SerialPortHelper.ReadAllData(serialPort, controllerToRead.GroupId, controllerToRead.AddressId);
            }
        }

        private ControllerItem GetCurrentController(int groupId, int addressId)
        {
            // 获取当前控制器
            foreach (var item in controllerItems)
            {
                if (item.GroupId == groupId && item.AddressId == addressId)
                { 
                    return item;
                }
            }

            return null;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            RefreshPortList();

            ReLoadControllerItems();
        }
    }
}
