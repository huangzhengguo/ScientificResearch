namespace ScientificResearch
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.portComboBox = new System.Windows.Forms.ComboBox();
            this.addButton = new System.Windows.Forms.Button();
            this.controllersDataGridView = new System.Windows.Forms.DataGridView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.onTimerDataGridView = new System.Windows.Forms.DataGridView();
            this.deleteOnTimerButton = new System.Windows.Forms.Button();
            this.addOnTimerButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.offTimerDataGridView = new System.Windows.Forms.DataGridView();
            this.deleteOffTimerButton = new System.Windows.Forms.Button();
            this.addOffTimerButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.trackBar2 = new System.Windows.Forms.TrackBar();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.label7 = new System.Windows.Forms.Label();
            this.trackBar3 = new System.Windows.Forms.TrackBar();
            this.label6 = new System.Windows.Forms.Label();
            this.trackBar4 = new System.Windows.Forms.TrackBar();
            this.label5 = new System.Windows.Forms.Label();
            this.trackBar5 = new System.Windows.Forms.TrackBar();
            this.label4 = new System.Windows.Forms.Label();
            this.trackBar6 = new System.Windows.Forms.TrackBar();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.powerButton = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.deleteControllerButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.controllersDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.onTimerDataGridView)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.offTimerDataGridView)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar6)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "端口:";
            // 
            // portComboBox
            // 
            this.portComboBox.FormattingEnabled = true;
            this.portComboBox.Location = new System.Drawing.Point(69, 19);
            this.portComboBox.Name = "portComboBox";
            this.portComboBox.Size = new System.Drawing.Size(121, 20);
            this.portComboBox.TabIndex = 2;
            this.portComboBox.SelectedIndexChanged += new System.EventHandler(this.portComboBox_SelectedIndexChanged);
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(210, 19);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 23);
            this.addButton.TabIndex = 3;
            this.addButton.Text = "添加";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // controllersDataGridView
            // 
            this.controllersDataGridView.AllowUserToAddRows = false;
            this.controllersDataGridView.AllowUserToDeleteRows = false;
            this.controllersDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.controllersDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controllersDataGridView.Location = new System.Drawing.Point(0, 0);
            this.controllersDataGridView.Name = "controllersDataGridView";
            this.controllersDataGridView.ReadOnly = true;
            this.controllersDataGridView.RowTemplate.Height = 23;
            this.controllersDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.controllersDataGridView.Size = new System.Drawing.Size(224, 736);
            this.controllersDataGridView.TabIndex = 4;
            this.controllersDataGridView.CurrentCellChanged += new System.EventHandler(this.controllersDataGridView_CurrentCellChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(-1, 48);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.controllersDataGridView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox3);
            this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel2.Controls.Add(this.powerButton);
            this.splitContainer1.Size = new System.Drawing.Size(1242, 736);
            this.splitContainer1.SplitterDistance = 224;
            this.splitContainer1.TabIndex = 6;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.onTimerDataGridView);
            this.groupBox3.Controls.Add(this.deleteOnTimerButton);
            this.groupBox3.Controls.Add(this.addOnTimerButton);
            this.groupBox3.Location = new System.Drawing.Point(222, 359);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(789, 374);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "打开定时器";
            // 
            // onTimerDataGridView
            // 
            this.onTimerDataGridView.AllowUserToAddRows = false;
            this.onTimerDataGridView.AllowUserToDeleteRows = false;
            this.onTimerDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.onTimerDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.onTimerDataGridView.Location = new System.Drawing.Point(0, 50);
            this.onTimerDataGridView.Name = "onTimerDataGridView";
            this.onTimerDataGridView.ReadOnly = true;
            this.onTimerDataGridView.RowTemplate.Height = 23;
            this.onTimerDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.onTimerDataGridView.Size = new System.Drawing.Size(789, 324);
            this.onTimerDataGridView.TabIndex = 1;
            this.onTimerDataGridView.CurrentCellChanged += new System.EventHandler(this.onTimerDataGridView_CurrentCellChanged);
            // 
            // deleteOnTimerButton
            // 
            this.deleteOnTimerButton.Location = new System.Drawing.Point(87, 21);
            this.deleteOnTimerButton.Name = "deleteOnTimerButton";
            this.deleteOnTimerButton.Size = new System.Drawing.Size(75, 23);
            this.deleteOnTimerButton.TabIndex = 0;
            this.deleteOnTimerButton.Text = "删除";
            this.deleteOnTimerButton.UseVisualStyleBackColor = true;
            this.deleteOnTimerButton.Click += new System.EventHandler(this.deleteOnTimerButton_Click);
            // 
            // addOnTimerButton
            // 
            this.addOnTimerButton.Location = new System.Drawing.Point(6, 20);
            this.addOnTimerButton.Name = "addOnTimerButton";
            this.addOnTimerButton.Size = new System.Drawing.Size(75, 23);
            this.addOnTimerButton.TabIndex = 0;
            this.addOnTimerButton.Text = "添加";
            this.addOnTimerButton.UseVisualStyleBackColor = true;
            this.addOnTimerButton.Click += new System.EventHandler(this.addOnTimerButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.Controls.Add(this.offTimerDataGridView);
            this.groupBox2.Controls.Add(this.deleteOffTimerButton);
            this.groupBox2.Controls.Add(this.addOffTimerButton);
            this.groupBox2.Location = new System.Drawing.Point(3, 358);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(213, 375);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "关断定时器";
            // 
            // offTimerDataGridView
            // 
            this.offTimerDataGridView.AllowUserToAddRows = false;
            this.offTimerDataGridView.AllowUserToDeleteRows = false;
            this.offTimerDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.offTimerDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.offTimerDataGridView.Location = new System.Drawing.Point(0, 51);
            this.offTimerDataGridView.Name = "offTimerDataGridView";
            this.offTimerDataGridView.ReadOnly = true;
            this.offTimerDataGridView.RowTemplate.Height = 23;
            this.offTimerDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.offTimerDataGridView.Size = new System.Drawing.Size(213, 324);
            this.offTimerDataGridView.TabIndex = 1;
            this.offTimerDataGridView.CurrentCellChanged += new System.EventHandler(this.offTimerDataGridView_CurrentCellChanged);
            // 
            // deleteOffTimerButton
            // 
            this.deleteOffTimerButton.Location = new System.Drawing.Point(88, 21);
            this.deleteOffTimerButton.Name = "deleteOffTimerButton";
            this.deleteOffTimerButton.Size = new System.Drawing.Size(75, 23);
            this.deleteOffTimerButton.TabIndex = 0;
            this.deleteOffTimerButton.Text = "删除";
            this.deleteOffTimerButton.UseVisualStyleBackColor = true;
            this.deleteOffTimerButton.Click += new System.EventHandler(this.deleteOffTimerButton_Click);
            // 
            // addOffTimerButton
            // 
            this.addOffTimerButton.Location = new System.Drawing.Point(7, 21);
            this.addOffTimerButton.Name = "addOffTimerButton";
            this.addOffTimerButton.Size = new System.Drawing.Size(75, 23);
            this.addOffTimerButton.TabIndex = 0;
            this.addOffTimerButton.Text = "添加";
            this.addOffTimerButton.UseVisualStyleBackColor = true;
            this.addOffTimerButton.Click += new System.EventHandler(this.addOffTimerButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.trackBar2);
            this.groupBox1.Controls.Add(this.trackBar1);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.trackBar3);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.trackBar4);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.trackBar5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.trackBar6);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(3, 49);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1008, 303);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "设置";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(882, 254);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(17, 12);
            this.label13.TabIndex = 7;
            this.label13.Text = "0%";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(882, 209);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(17, 12);
            this.label12.TabIndex = 7;
            this.label12.Text = "0%";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(882, 164);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(17, 12);
            this.label11.TabIndex = 7;
            this.label11.Text = "0%";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(882, 119);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(17, 12);
            this.label10.TabIndex = 7;
            this.label10.Text = "0%";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(882, 74);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(17, 12);
            this.label9.TabIndex = 7;
            this.label9.Text = "0%";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(882, 29);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(17, 12);
            this.label8.TabIndex = 7;
            this.label8.Text = "0%";
            // 
            // trackBar2
            // 
            this.trackBar2.Location = new System.Drawing.Point(111, 66);
            this.trackBar2.Maximum = 1000;
            this.trackBar2.Name = "trackBar2";
            this.trackBar2.Size = new System.Drawing.Size(754, 45);
            this.trackBar2.TabIndex = 0;
            this.trackBar2.Tag = "10002";
            this.trackBar2.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar2.ValueChanged += new System.EventHandler(this.trackBar1_ValueChanged);
            this.trackBar2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trackBar1_MouseUp);
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(111, 21);
            this.trackBar1.Maximum = 1000;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(754, 45);
            this.trackBar1.TabIndex = 0;
            this.trackBar1.Tag = "10001";
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar1.ValueChanged += new System.EventHandler(this.trackBar1_ValueChanged);
            this.trackBar1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trackBar1_MouseUp);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(17, 245);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 12);
            this.label7.TabIndex = 6;
            this.label7.Text = "Channel 6";
            // 
            // trackBar3
            // 
            this.trackBar3.Location = new System.Drawing.Point(111, 111);
            this.trackBar3.Maximum = 1000;
            this.trackBar3.Name = "trackBar3";
            this.trackBar3.Size = new System.Drawing.Size(754, 45);
            this.trackBar3.TabIndex = 0;
            this.trackBar3.Tag = "10003";
            this.trackBar3.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar3.ValueChanged += new System.EventHandler(this.trackBar1_ValueChanged);
            this.trackBar3.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trackBar1_MouseUp);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 202);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 12);
            this.label6.TabIndex = 5;
            this.label6.Text = "Channel 5";
            // 
            // trackBar4
            // 
            this.trackBar4.Location = new System.Drawing.Point(111, 156);
            this.trackBar4.Maximum = 1000;
            this.trackBar4.Name = "trackBar4";
            this.trackBar4.Size = new System.Drawing.Size(754, 45);
            this.trackBar4.TabIndex = 0;
            this.trackBar4.Tag = "10004";
            this.trackBar4.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar4.ValueChanged += new System.EventHandler(this.trackBar1_ValueChanged);
            this.trackBar4.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trackBar1_MouseUp);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 159);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 12);
            this.label5.TabIndex = 4;
            this.label5.Text = "Channel 4";
            // 
            // trackBar5
            // 
            this.trackBar5.Location = new System.Drawing.Point(111, 201);
            this.trackBar5.Maximum = 1000;
            this.trackBar5.Name = "trackBar5";
            this.trackBar5.Size = new System.Drawing.Size(754, 45);
            this.trackBar5.TabIndex = 0;
            this.trackBar5.Tag = "10005";
            this.trackBar5.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar5.ValueChanged += new System.EventHandler(this.trackBar1_ValueChanged);
            this.trackBar5.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trackBar1_MouseUp);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 116);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "Channel 3";
            // 
            // trackBar6
            // 
            this.trackBar6.Location = new System.Drawing.Point(111, 246);
            this.trackBar6.Maximum = 1000;
            this.trackBar6.Name = "trackBar6";
            this.trackBar6.Size = new System.Drawing.Size(754, 45);
            this.trackBar6.TabIndex = 0;
            this.trackBar6.Tag = "10006";
            this.trackBar6.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar6.ValueChanged += new System.EventHandler(this.trackBar1_ValueChanged);
            this.trackBar6.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trackBar1_MouseUp);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "Channel 2";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "Channel 1";
            // 
            // powerButton
            // 
            this.powerButton.Location = new System.Drawing.Point(10, 20);
            this.powerButton.Name = "powerButton";
            this.powerButton.Size = new System.Drawing.Size(75, 23);
            this.powerButton.TabIndex = 7;
            this.powerButton.Text = "打开";
            this.powerButton.UseVisualStyleBackColor = true;
            this.powerButton.Click += new System.EventHandler(this.powerButton_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(368, 19);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "刷新";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // deleteControllerButton
            // 
            this.deleteControllerButton.Location = new System.Drawing.Point(289, 19);
            this.deleteControllerButton.Name = "deleteControllerButton";
            this.deleteControllerButton.Size = new System.Drawing.Size(75, 23);
            this.deleteControllerButton.TabIndex = 8;
            this.deleteControllerButton.Text = "删除";
            this.deleteControllerButton.UseVisualStyleBackColor = true;
            this.deleteControllerButton.Click += new System.EventHandler(this.deleteControllerButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1241, 784);
            this.Controls.Add(this.deleteControllerButton);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.portComboBox);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.controllersDataGridView)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.onTimerDataGridView)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.offTimerDataGridView)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar6)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox portComboBox;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.DataGridView controllersDataGridView;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar trackBar6;
        private System.Windows.Forms.TrackBar trackBar5;
        private System.Windows.Forms.TrackBar trackBar4;
        private System.Windows.Forms.TrackBar trackBar3;
        private System.Windows.Forms.TrackBar trackBar2;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Button powerButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button addOffTimerButton;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button addOnTimerButton;
        private System.Windows.Forms.DataGridView offTimerDataGridView;
        private System.Windows.Forms.DataGridView onTimerDataGridView;
        private System.Windows.Forms.Button deleteOffTimerButton;
        private System.Windows.Forms.Button deleteOnTimerButton;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button deleteControllerButton;
    }
}

