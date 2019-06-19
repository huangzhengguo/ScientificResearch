using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ScientificResearch.Models;
using ScientificResearch.Helper;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms.DataVisualization.Charting;
using System.Runtime.Serialization.Formatters.Binary;

namespace ScientificResearch
{
    public partial class SettingControllerForm : Form
    {
        private readonly SerialPort serialPort;
        private readonly ControllerItem currentControllerItem;
        private readonly ControllerItem[] controllerItems;
        private readonly int channelNum = 4;
        private readonly int maxTimePointCount = 8;

        // 用来标记修改的是打开时间点还是关断时间点
        private bool isEditTurnOnTimer = true;
        private int currentTimerIndex = 0;

        // 刷新界面代理
        private Action refreshSettingDisplay;

        // 控件
        TrackBar[] trackBars;
        Label[] labels;

        // 时间列表
        private readonly int rowHeight = 30;

        // 时间点列表tag起始值
        private readonly int addTurnOnTimePointBtnTag = 30000;
        private readonly int addTurnOffTimePointBtnTag = 40000;

        // 时间点列表Label数组
        private List<Label> turnOnTimerLabels = new List<Label>();
        private List<Label> turnOffTimerLabels = new List<Label>();

        // 保存回调
        public Action saveSetting;

        public SettingControllerForm(SerialPort serialPort, ControllerItem controllerItem, ControllerItem[] controllerItems)
        {
            InitializeComponent();

            refreshSettingDisplay = RefreshSettingDisplay;

            this.serialPort = serialPort;
            this.currentControllerItem = controllerItem;
            this.controllerItems = controllerItems;

            this.trackBars = new TrackBar[] { trackBar1, trackBar2, trackBar3, trackBar4, trackBar5 };
            this.labels = new Label[] { label8, label9, label10, label11, label12 };

            // 控制器列表
            foreach (var item in HelperTool.ReadControllersFromFile<ControllerItem>().OrderBy(m => m.GroupId))
            {
                comboBox1.Items.Add(item.GroupId + "," + item.AddressId);
            }

            comboBox1.Text = currentControllerItem.GroupId + "," + currentControllerItem.AddressId;
        }

        private void SettingControllerForm_Load(object sender, EventArgs e)
        {
            // 同步时间
            byte[] txData = { 0x00, 0x00, 0x10, 0x01, 0x07, (byte)(DateTime.Now.Year-2000),
                             (byte)DateTime.Now.Month, (byte)DateTime.Now.Day, (byte)DateTime.Now.DayOfWeek, (byte)DateTime.Now.Hour, (byte)DateTime.Now.Minute,
                             (byte)DateTime.Now.Second };

            SerialPortHelper.SendData(serialPort, txData);

            // RefreshSettingDisplay();

            ReadSelectedComData(currentControllerItem.GroupId, currentControllerItem.AddressId);
        }

        private void ReadSelectedComData(int groupId, int addressId)
        {
            // 设置解析代理
            SerialPortHelper.ParseReData = (recData) =>
            {
                currentControllerItem.ParseDataToControllerItem(recData);

                Invoke(refreshSettingDisplay);
            };

            // 读取控制器所有设置数据
            SerialPortHelper.ReadAllData(serialPort, groupId, addressId);

            if (currentControllerItem.PowerState)
            {
                powerButton.BackgroundImage = Properties.Resources.powerOn;
            }
            else
            {
                powerButton.BackgroundImage = Properties.Resources.powerOff;
            }
        }

        private void RefreshSettingDisplay()
        {
            panel1.Visible = true;

            timePointChart.ChartAreas[0].AxisX.Minimum = 0;
            timePointChart.ChartAreas[0].AxisX.Maximum = 1440;
            timePointChart.ChartAreas[0].AxisX.Interval = 360;
            timePointChart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            for (int i = 0; i < 1439; i++)
            {
                if (i % 360 == 0)
                {
                    CustomLabel customLabel = new CustomLabel(i, i + 50, HelperTool.ConverMinutes2DateTime(i), 0, LabelMarkStyle.None);

                    timePointChart.ChartAreas[0].AxisX.CustomLabels.Add(customLabel);
                }
            }

            timePointChart.ChartAreas[0].AxisY.Maximum = 100;
            timePointChart.ChartAreas[0].AxisY.MajorGrid.Enabled = false;

            CreateTimePointList(currentControllerItem);
            DrawChart(currentControllerItem);
            if (currentControllerItem.OnTimerCount == 0 && currentControllerItem.OffTimerCount == 0)
            {
                enableCheckBox.Checked = false;
            }
            else
            {
                enableCheckBox.Checked = true;
            }
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 读取控制器信息
            ComboBox comboBox = (ComboBox)sender;

            int groupId = 0;
            int addressId = 0;
            HelperTool.GetControllerAddressId(comboBox.Text, out groupId, out addressId);

            if (groupId == 0 || addressId == 0)
            {
                return;
            }

            ControllerItem selectedControllerItem = GetCurrentController(groupId, addressId);

            nameTextBox.Text = selectedControllerItem.Name;
            zoneTextBox.Text = selectedControllerItem.Zone;

            panel1.Visible = false;

            // 读取数据
            ReadSelectedComData(groupId, addressId);
        }

        private void DrawChart(ControllerItem item)
        {
            List<LineModel> lineModels = new List<LineModel>();
            for (int i=0;i<item.OnTimerCount;i++)
            {
                var model = new LineModel()
                {
                    XAxis = item.OnTimersMinutes[i],
                    IsTurnOn = true,
                    ColorPercent = item.OnTimersColorValues[i]
                };

                lineModels.Add(model);
            }

            for (int i=0;i<item.OffTimerCount;i++)
            {
                var model = new LineModel()
                {
                    XAxis = item.OffTimersMinutes[i],
                    IsTurnOn = false,
                    ColorPercent = new List<int>(channelNum) { 0,0,0,0 }
                };

                lineModels.Add(model);
            }

            lineModels.Sort((x, y) => x.XAxis.CompareTo(y.XAxis));

            if (lineModels == null || lineModels.Count <= 0)
            {
                return;
            }

            timePointChart.Series.Clear();
            for (int i=0;i<channelNum;i++)
            {
                Series series = timePointChart.Series.Add("Channel" + i.ToString());

                series.ChartType = SeriesChartType.Line;
                series.MarkerStyle = MarkerStyle.Circle;
                series.XAxisType = AxisType.Primary;

                series.Points.Add(new DataPoint(0, lineModels[lineModels.Count - 1].ColorPercent[i]));
                series.Points.Add(new DataPoint(lineModels[0].XAxis, lineModels[lineModels.Count - 1].ColorPercent[i]));
                for (int j=0;j<lineModels.Count; j++)
                {
                    var model = lineModels[j];
                    series.Points.Add(new DataPoint(model.XAxis, model.ColorPercent[i]));

                    if (j + 1 < lineModels.Count)
                    {
                        series.Points.Add(new DataPoint(lineModels[j + 1].XAxis, model.ColorPercent[i]));
                    }
                }

                series.Points.Add(new DataPoint(1439, lineModels[lineModels.Count - 1].ColorPercent[i]));
            }
        }

        private void CreateTimePointList(ControllerItem item)
        {
            //if (item.OnTimersMinutes == null)
            //{
            //    CreateAddTimePointRow(timePanel, new Point(7, 20 + 0), rowHeight, "Add Time Point", 0, addTurnOnTimePointBtnTag);
            //    return;
            //}

            //if (item.OffTimersMinutes == null)
            //{
            //    CreateAddTimePointRow(timePanel, new Point(299, 20 + 0), rowHeight, "Add Turn Off Time", 299, addTurnOffTimePointBtnTag);
            //    return;
            //}

            timePanel.Controls.Clear();
            turnOffTimerLabels.Clear();
            turnOnTimerLabels.Clear();
            // 创建打开定时器
            for (int i=0;i<item.OnTimersMinutes.Count;i++)
            {
                var onTimer = item.OnTimersMinutes[i];

                CreateTimePointRow(timePanel, 10000, i, rowHeight, HelperTool.ConverMinutes2DateTime(onTimer), 0);
            }

            // 创建增加打开定时器按钮
            CreateAddTimePointRow(timePanel, new Point(7, 20 + item.OnTimersMinutes.Count * rowHeight), rowHeight, "Add Time Point", 0, addTurnOnTimePointBtnTag);

            // 创建关闭定时器
            for (int i = 0; i < item.OffTimersMinutes.Count; i++)
            {
                var offTimer = item.OffTimersMinutes[i];

                CreateTimePointRow(timePanel, 20000, i, rowHeight, HelperTool.ConverMinutes2DateTime(offTimer), 299);
            }

            // 创建增加关闭定时器按钮
            CreateAddTimePointRow(timePanel, new Point(299, 20 + item.OffTimersMinutes.Count * rowHeight), rowHeight, "Add Turn Off Time", 299, addTurnOffTimePointBtnTag);
        }

        public void CreateAddTimePointRow(Panel parentPanel, Point position, int rowHeight, string title, int leftMargin, int panelTag)
        {
            Panel panel = new Panel();

            panel.Tag = panelTag;
            panel.Location = position;

            Button addBtn = new Button();

            addBtn.Location = new Point(0, 0);
            addBtn.BackgroundImage = Properties.Resources.addTimePoint;
            addBtn.BackgroundImageLayout = ImageLayout.Stretch;
            addBtn.FlatAppearance.BorderSize = 0;
            addBtn.FlatStyle = FlatStyle.Flat;
            addBtn.Size = new Size(20, 20);
            addBtn.Click += AddBtn_Click;

            panel.Controls.Add(addBtn);

            Label timeLabel = new Label();
            
            timeLabel.Location = new Point(20 + 7, addBtn.Location.Y + 5);
            timeLabel.AutoSize = true;
            timeLabel.Text = title;

            panel.Controls.Add(timeLabel);

            Label underLineLabel = new Label();

            underLineLabel.Location = new Point(7, addBtn.Location.Y + rowHeight + 1);
            underLineLabel.AutoSize = false;
            underLineLabel.Size = new Size(192, 1);
            underLineLabel.BackColor = Color.DarkGray;

            panel.Controls.Add(underLineLabel);

            parentPanel.Controls.Add(panel);
        }


        public void CreateTimePointRow(Panel parentPanel, int startTag, int index, int rowHeight, string timeStr, int leftMargin)
        {
            Label timerLabel = new Label();

            timerLabel.Location = new Point(20 + leftMargin, 16 + index * rowHeight);
            timerLabel.AutoSize = true;
            timerLabel.Text = timeStr;

            parentPanel.Controls.Add(timerLabel);

            if (startTag == 10000)
            {
                turnOnTimerLabels.Add(timerLabel);
            }
            else
            {
                turnOffTimerLabels.Add(timerLabel);
            }

            Button editBtn = new Button();

            editBtn.Tag = startTag + index;
            editBtn.Location = new Point(140 + leftMargin, 14 + index * rowHeight);
            editBtn.BackgroundImage = Properties.Resources.edit;
            editBtn.BackgroundImageLayout = ImageLayout.Stretch;
            editBtn.FlatAppearance.BorderSize = 0;
            editBtn.FlatStyle = FlatStyle.Flat;
            editBtn.Size = new Size(20, 20);
            editBtn.Click += EditBtn_Click;

            parentPanel.Controls.Add(editBtn);

            Button deleteBtn = new Button();

            deleteBtn.Tag = startTag + 8 + index;
            deleteBtn.Location = new Point(169 + leftMargin, 14 + index * rowHeight);
            deleteBtn.BackgroundImage = Properties.Resources.delete;
            deleteBtn.BackgroundImageLayout = ImageLayout.Stretch;
            deleteBtn.FlatAppearance.BorderSize = 0;
            deleteBtn.FlatStyle = FlatStyle.Flat;
            deleteBtn.Size = new Size(20, 20);
            deleteBtn.Click += DeleteBtn_Click;

            parentPanel.Controls.Add(deleteBtn);

            Label underLineLabel = new Label();

            underLineLabel.Location = new Point(7 + leftMargin, 37 + index * rowHeight);
            underLineLabel.AutoSize = false;
            underLineLabel.Size = new Size(192, 1);
            underLineLabel.BackColor = Color.DarkGray;
            underLineLabel.Text = "";

            parentPanel.Controls.Add(underLineLabel);
        }

        /// <summary>
        /// 增加时间点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddBtn_Click(object sender, EventArgs e)
        {
            saveButton.Visible = true;

            Button btn = (Button)sender;
            Panel panel = (Panel)btn.Parent;
            int minutes = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
            if ((int)panel.Tag == addTurnOnTimePointBtnTag)
            {
                if (currentControllerItem.OnTimerCount >= 8)
                {
                    MessageBox.Show("Eight time points at most!");
                    return;
                }

                // 增加一个时间点
                CreateTimePointRow(timePanel, 10000, currentControllerItem.OnTimerCount, rowHeight, HelperTool.ConverMinutes2DateTime(minutes), 0);

                // 往下移动添加按钮
                btn.Parent.Location = new Point(btn.Parent.Location.X, btn.Parent.Location.Y + rowHeight);

                currentControllerItem.OnTimersMinutes.Add(HelperTool.GetDefaultMinutes());

                var defaultColorValues = HelperTool.GetDefaultColorValues(HelperTool.GetChannelNum());

                currentControllerItem.OnTimersColorValues.Add(defaultColorValues);

                // SetTrackBarColorValue(this.trackBars, this.labels, currentControllerItem.OnTimersColorValues[currentControllerItem.OnTimerCount].ToArray());

                currentControllerItem.OnTimerCount++;
            }
            else if ((int)panel.Tag == addTurnOffTimePointBtnTag)
            {
                if (currentControllerItem.OffTimerCount >= 8)
                {
                    MessageBox.Show("Eight time points at most!");
                    return;
                }

                // 增加一个时间点
                CreateTimePointRow(timePanel, 20000, currentControllerItem.OffTimerCount, rowHeight, HelperTool.ConverMinutes2DateTime(minutes), 299);

                currentControllerItem.OffTimersMinutes.Add(HelperTool.GetDefaultMinutes());

                // 往下移动添加按钮
                btn.Parent.Location = new Point(btn.Parent.Location.X, btn.Parent.Location.Y + rowHeight);

                currentControllerItem.OffTimerCount++;
            }

            DrawChart(currentControllerItem);
        }

        private void DeleteBtn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Confirm to delete?", "Delete", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }

            saveButton.Visible = true;

            Button editBtn = (Button)sender;
            int btnTag = int.Parse(editBtn.Tag.ToString());
            if (btnTag < 20000)
            {
                // 删除打开时间点
                currentTimerIndex = btnTag - 10000 - 8;

                currentControllerItem.OnTimersMinutes.RemoveAt(currentTimerIndex);
                currentControllerItem.OnTimersColorValues.RemoveAt(currentTimerIndex);
                currentControllerItem.OnTimerCount--;
            }
            else
            {
                currentTimerIndex = btnTag - 20000 - 8;

                currentControllerItem.OffTimersMinutes.RemoveAt(currentTimerIndex);
                currentControllerItem.OffTimerCount--;
            }

            DrawChart(currentControllerItem);
            CreateTimePointList(currentControllerItem);
        }

        private void EditBtn_Click(object sender, EventArgs e)
        {
            saveButton.Visible = true;

            Button editBtn = (Button)sender;
            int btnTag = int.Parse(editBtn.Tag.ToString());
            if (btnTag < 20000)
            {
                isEditTurnOnTimer = true;

                currentTimerIndex = btnTag - 10000;
                channelGroupBox.Visible = true;

                // 打开定时器
                int minutes = currentControllerItem.OnTimersMinutes[currentTimerIndex];

                // 设置修改时间
                timePointLabel.Visible = true;
                timPointDateTimePicker.Visible = true;
                timPointDateTimePicker.Value = new DateTime(2000, 1, 1, minutes / 60, minutes % 60, 0);

                // 设置滑动条
                SetTrackBarColorValue(this.trackBars, this.labels, currentControllerItem.OnTimersColorValues[currentTimerIndex].ToArray());
            }
            else
            {
                isEditTurnOnTimer = false;

                currentTimerIndex = btnTag - 20000;
                channelGroupBox.Visible = false;

                // 关断定时器
                int minutes = currentControllerItem.OffTimersMinutes[currentTimerIndex];

                // 设置修改时间
                timePointLabel.Visible = true;
                timPointDateTimePicker.Visible = true;
                timPointDateTimePicker.Value = new DateTime(2000, 1, 1, minutes / 60, minutes % 60, 0);
            }
        }

        /// <summary>
        /// 设置滑动条
        /// </summary>
        /// <param name="trackBars">滑动条</param>
        /// <param name="labels">百分比</param>
        /// <param name="timePointColorValues">颜色百分比值</param>
        private void SetTrackBarColorValue(TrackBar[] trackBars, Label[] labels, int[] timePointColorValues)
        {
            // 设置滑动条
            for (int i = 0; i < trackBars.Length; i++)
            {
                if (i < 5)
                {
                    trackBars[i].Value = timePointColorValues[i] * 10;
                    labels[i].Text = timePointColorValues[i] + "%";
                }
            }
        }

        /// <summary>
        /// 获取示例数据
        /// </summary>
        private void GetSampleData()
        {
            currentControllerItem.GroupId = 1;
            currentControllerItem.AddressId = 1;
            currentControllerItem.GroupName = "分组名称";
            currentControllerItem.PowerState = true;
            currentControllerItem.LightsColorValues = new List<int> { 100, 10, 200, 20, 300, 800 };
            currentControllerItem.OffTimerCount = 2;
            currentControllerItem.OffTimersMinutes = new List<int> { 360, 1080 };
            currentControllerItem.OnTimerCount = 3;
            currentControllerItem.OnTimersMinutes = new List<int> { 653, 592, 1080 };
            currentControllerItem.OnTimersColorValues = new List<List<int>>
            {
                new List<int> { 100, 100, 100, 100, 100, 100 },
                new List<int> { 100, 50, 100, 30, 100, 100 },
                new List<int> { 100, 10, 100, 20, 10, 100 }
            };
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            Button saveBtn = (Button)sender;

            saveBtn.Visible = false;

            channelGroupBox.Visible = false;

            // 设置修改时间
            timePointLabel.Visible = false;
            timPointDateTimePicker.Visible = false;

            var items = HelperTool.ReadControllersFromFile<ControllerItem>().ToList();

            foreach (var item in items)
            {
                if (item.GroupId == currentControllerItem.GroupId)
                {
                    item.Zone = zoneTextBox.Text;
                    if (item.AddressId == currentControllerItem.AddressId)
                    {
                        item.Name = nameTextBox.Text;
                    }
                }
            }

            Stream stream = File.Open("controllers.dat", FileMode.Open);
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            binaryFormatter.Serialize(stream, items.ToArray());

            stream.Close();

            // 发送命令
            if (enableCheckBox.Checked)
            {
                SendSettingToDevice(currentControllerItem);
            }

            saveSetting?.Invoke();
        }

        private void EnableCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;

            if (!checkBox.Checked)
            {
                timePointLabel.Visible = false;
                timPointDateTimePicker.Visible = false;
                channelGroupBox.Visible = true;
                programGroupBox.Visible = false;
                powerButton.Visible = true;
                saveButton.Visible = false;

                List<int> colorValues = new List<int>();
                foreach(var colorValue in currentControllerItem.LightsColorValues)
                {
                    colorValues.Add((int)((float)colorValue / 1000.0 * 100));
                }
                // 对滑动条赋值
                SetTrackBarColorValue(trackBars, labels, colorValues.ToArray());
            }
            else
            {
                timePointLabel.Visible = false;
                timPointDateTimePicker.Visible = false;
                channelGroupBox.Visible = false;
                programGroupBox.Visible = true;
                powerButton.Visible = false;
            }

            checkBox.Visible = true;
        }

        /// <summary>
        /// 修改时间点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimPointDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            DateTimePicker dateTimePicker = (DateTimePicker)sender;
            int minutes = HelperTool.ConverDateTime2Minutes(dateTimePicker.Value);
            if (isEditTurnOnTimer)
            {
                currentControllerItem.OnTimersMinutes[currentTimerIndex] = minutes;

                // 更新列表中label显示
                turnOnTimerLabels[currentTimerIndex].Text = HelperTool.ConverMinutes2DateTime(minutes);
            }
            else
            {
                currentControllerItem.OffTimersMinutes[currentTimerIndex] = minutes;

                // 更新列表中label显示
                turnOffTimerLabels[currentTimerIndex].Text = HelperTool.ConverMinutes2DateTime(minutes);
            }

            // 重绘曲线图
            DrawChart(currentControllerItem);
        }

        private void TrackBar1_MouseUp(object sender, MouseEventArgs e)
        {
            // 如果设置定时器，直接返回
            if (enableCheckBox.Checked)
            {
                return;
            }

            // 直接发送命令
            TrackBar trackBar = (TrackBar)sender;

            // 滑动条索引
            int trackBarIndex = (int.Parse(trackBar.Tag.ToString()) - 10000);
            // 寄存器地址
            int registerAddr = 0x10 + trackBarIndex * 2 - 1;

            Int16 colorValue = (Int16)trackBar.Value;

            byte[] colorValueBytes = BitConverter.GetBytes(colorValue);
            // 发送数据
            byte[] txData = { (byte)currentControllerItem.GroupId, (byte)currentControllerItem.AddressId, 0x10, (byte)registerAddr, 0x02, colorValueBytes[0], colorValueBytes[1] };

            SerialPortHelper.SendData(serialPort, txData);
        }

        private void TrackBar1_ValueChanged(object sender, EventArgs e)
        {
            TrackBar trackBar = (TrackBar)sender;
            int colorIndex = int.Parse(trackBar.Tag.ToString()) - 10001;

            int colorPercent = (int)((float)trackBar.Value / 1000.0 * 100);
            labels[colorIndex].Text = colorPercent + "%";

            // 设置定时器，如果不是则直接返回
            if (!enableCheckBox.Checked)
            {
                return;
            }

            currentControllerItem.OnTimersColorValues[currentTimerIndex][colorIndex] = colorPercent;

            DrawChart(currentControllerItem);
        }

        private void PowerButton_Click(object sender, EventArgs e)
        {
            byte powerByte = 0x01;
            if (currentControllerItem.PowerState)
            {
                powerByte = 0x0;
                powerButton.BackgroundImage = Properties.Resources.powerOff;
            }
            else
            {
                powerButton.BackgroundImage = Properties.Resources.powerOn;
            }

            currentControllerItem.PowerState = !currentControllerItem.PowerState;
            byte[] txData = { (byte)currentControllerItem.GroupId, (byte)currentControllerItem.AddressId, 0x10, 0x10, 0x01, powerByte };

            SerialPortHelper.SendData(serialPort, txData);
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

        /// <summary>
        /// 导出当前配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "config(*.dat)|*.dat";
            saveFileDialog.FileName = "config.dat";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Stream stream = File.Open(saveFileDialog.FileName, FileMode.Create);
                BinaryFormatter binaryFormatter = new BinaryFormatter();

                binaryFormatter.Serialize(stream, currentControllerItem);

                stream.Close();
            }
        }

        /// <summary>
        /// 导入配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImportButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "*.dat|*.dat";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Stream stream = File.Open(openFileDialog.FileName, FileMode.Open);
                BinaryFormatter binaryFormatter = new BinaryFormatter();

                ControllerItem controllerItem = binaryFormatter.Deserialize(stream) as ControllerItem;

                stream.Close();

                // 发送到灯具
                SendSettingToDevice(controllerItem);

                ReadSelectedComData(controllerItem.GroupId, controllerItem.AddressId);
            }
        }

        /// <summary>
        /// 发送命令到设备
        /// </summary>
        /// <param name="controllerItem"></param>
        private void SendSettingToDevice(ControllerItem controllerItem)
        {
            controllerItem.SortTimers();

            // 起始地址
            int startAddress = 0x20;
            List<byte> txData = new List<byte>() { (byte)controllerItem.GroupId, (byte)controllerItem.AddressId, 0x10, (byte)startAddress, 0x50 };
            // 添加关断定时器数据
            for (int i = 0; i < maxTimePointCount; i++)
            {
                if (i >= controllerItem.OffTimerCount)
                {
                    txData.AddRange(new byte[] { 0xff, 0xff });

                    continue;
                }

                byte[] minutesBytes = BitConverter.GetBytes(controllerItem.OffTimersMinutes[i]);

                txData.AddRange(new byte[] { minutesBytes[0], minutesBytes[1] });
            }

            // 添加打开定时器
            for (int i = 0; i < maxTimePointCount; i++)
            {
                if (i >= controllerItem.OnTimerCount)
                {
                    txData.AddRange(new byte[] { 0xff, 0xff, 0, 0, 0, 0, 0, 0 });

                    continue;
                }

                byte[] minutesBytes = BitConverter.GetBytes(controllerItem.OnTimersMinutes[i]);

                txData.AddRange(new byte[] { minutesBytes[0], minutesBytes[1] });

                foreach (var item in controllerItem.OnTimersColorValues[i])
                {
                    txData.Add((byte)item);
                }
            }

            SerialPortHelper.SendData(serialPort, txData.ToArray());
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            // 同步时间
            byte[] txData = { 0x00, 0x00, 0x10, 0x01, 0x07, (byte)(DateTime.Now.Year-2000),
                             (byte)DateTime.Now.Month, (byte)DateTime.Now.Day, (byte)DateTime.Now.DayOfWeek, (byte)DateTime.Now.Hour, (byte)DateTime.Now.Minute,
                             (byte)DateTime.Now.Second };

            SerialPortHelper.SendData(serialPort, txData);

            // RefreshSettingDisplay();

            ReadSelectedComData(currentControllerItem.GroupId, currentControllerItem.AddressId);
        }
    }
}
