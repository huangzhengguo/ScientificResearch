﻿using System;
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
    public partial class AddOffTimerForm : Form
    {
        public Action<int> PassMinutes;
        public AddOffTimerForm()
        {
            InitializeComponent();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            PassMinutes?.Invoke(HelperTool.ConverDateTime2Minutes(offTimerDateTimePicker.Value));

            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
