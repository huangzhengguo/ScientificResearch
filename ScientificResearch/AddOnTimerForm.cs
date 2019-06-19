using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ScientificResearch.Helper;

namespace ScientificResearch
{
    public partial class AddOnTimerForm : Form
    {
        public Action<int, int, int, int, int, int, int> PassOnTimer;
        public AddOnTimerForm()
        {
            InitializeComponent();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            // 传递时间点和对应颜色百分比
            PassOnTimer?.Invoke(HelperTool.ConverDateTime2Minutes(onDateTimePicker.Value), (int)numericUpDown1.Value, (int)numericUpDown2.Value,
                                                                       (int)numericUpDown3.Value, (int)numericUpDown4.Value,
                                                                       (int)numericUpDown5.Value, (int)numericUpDown6.Value);
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
