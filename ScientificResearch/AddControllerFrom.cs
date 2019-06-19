using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ScientificResearch.Models;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ScientificResearch.Helper;

namespace ScientificResearch
{
    public partial class AddControllerFrom : Form
    {
        public Func<int, int, string, string, bool> PassGroupAddressId;
        public AddControllerFrom()
        {
            InitializeComponent();
        }

        private void confirmButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(groupComboBox.Text) || string.IsNullOrEmpty(addressComboBox.Text))
            {
                MessageBox.Show("不能为空！");
                return;
            }

            if (PassGroupAddressId?.Invoke(int.Parse(groupComboBox.Text), int.Parse(addressComboBox.Text), equipNameTextBox.Text, zoneTextBox.Text) == true)
            {
                this.Close();
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
