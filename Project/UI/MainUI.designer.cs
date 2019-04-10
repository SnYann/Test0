using System;
using System.Windows.Forms;

namespace Window
{
    partial class MainUI
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainUI));
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.timerSimulate = new System.Windows.Forms.Timer(this.components);
            this.groupBox_ChangeArea = new System.Windows.Forms.GroupBox();
            this.cB_AreasList = new System.Windows.Forms.ComboBox();
            this.btn_SaveMesh = new System.Windows.Forms.Button();
            this.labelInfo = new System.Windows.Forms.Label();
            this.groupBox_paramenter = new System.Windows.Forms.GroupBox();
            this.label_panelChangeParamenter = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.panelAll = new System.Windows.Forms.Panel();
            this.groupBox_Weight = new System.Windows.Forms.GroupBox();
            this.tB_Weight = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.menuStrip3 = new System.Windows.Forms.MenuStrip();
            this.播放ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.工程ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.新建工程ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开工程ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.编辑ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.编辑ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.编辑工程设置ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip2 = new System.Windows.Forms.MenuStrip();
            this.menu_Start = new System.Windows.Forms.ToolStripMenuItem();
            this.停止ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timer_Progress = new System.Windows.Forms.Timer(this.components);
            this.BottomToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.TopToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.RightToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.LeftToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.ContentPanel = new System.Windows.Forms.ToolStripContentPanel();
            this.panelInfo = new System.Windows.Forms.Panel();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.panel3 = new System.Windows.Forms.Panel();
            this.labelOutBili = new System.Windows.Forms.Label();
            this.labelOutAgents = new System.Windows.Forms.Label();
            this.labelOutdoor = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cB_showGoalNow = new System.Windows.Forms.CheckBox();
            this.chBoxChart = new System.Windows.Forms.CheckBox();
            this.cB_ShowGird = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rB_Border = new System.Windows.Forms.RadioButton();
            this.rB_AreaSet = new System.Windows.Forms.RadioButton();
            this.rB_AreaPartition = new System.Windows.Forms.RadioButton();
            this.rB_Mesh = new System.Windows.Forms.RadioButton();
            this.trackBar2 = new System.Windows.Forms.TrackBar();
            this.timerforHeatmap = new System.Windows.Forms.Timer(this.components);
            this.rB_heatmap = new System.Windows.Forms.RadioButton();
            this.rB_corwds = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.timerRead = new System.Windows.Forms.Timer(this.components);
            this.doubleBufferPanel1 = new Windows.DoubleBufferPanel();
            this.panelReadPlay = new System.Windows.Forms.TableLayoutPanel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox_Stop = new System.Windows.Forms.PictureBox();
            this.cB_MultiSpeed = new System.Windows.Forms.ComboBox();
            this.pictureBox_Play = new System.Windows.Forms.PictureBox();
            this.label_FrameMax = new System.Windows.Forms.Label();
            this.trackBar_replay = new System.Windows.Forms.TrackBar();
            this.label_frameMin = new System.Windows.Forms.Label();
            this.SidePanel = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.checkBox_outputVRS = new System.Windows.Forms.CheckBox();
            this.cB_ShowExits = new System.Windows.Forms.CheckBox();
            this.cB_DrawColorRule = new System.Windows.Forms.CheckBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.labelPanelInfo = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.BackGroundPanel = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.chartOuts = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.groupBox_ChangeArea.SuspendLayout();
            this.groupBox_paramenter.SuspendLayout();
            this.panelAll.SuspendLayout();
            this.groupBox_Weight.SuspendLayout();
            this.panel2.SuspendLayout();
            this.menuStrip3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.menuStrip2.SuspendLayout();
            this.panelInfo.SuspendLayout();
            this.panel3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).BeginInit();
            this.panel1.SuspendLayout();
            this.doubleBufferPanel1.SuspendLayout();
            this.panelReadPlay.SuspendLayout();
            this.panel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Stop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Play)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_replay)).BeginInit();
            this.SidePanel.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel5.SuspendLayout();
            this.BackGroundPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartOuts)).BeginInit();
            this.SuspendLayout();
            // 
            // timerSimulate
            // 
            this.timerSimulate.Interval = 40;
            this.timerSimulate.Tick += new System.EventHandler(this.timerSimulate_Tick);
            // 
            // groupBox_ChangeArea
            // 
            this.groupBox_ChangeArea.Controls.Add(this.cB_AreasList);
            this.groupBox_ChangeArea.Controls.Add(this.btn_SaveMesh);
            this.groupBox_ChangeArea.Location = new System.Drawing.Point(1566, 366);
            this.groupBox_ChangeArea.Name = "groupBox_ChangeArea";
            this.groupBox_ChangeArea.Size = new System.Drawing.Size(157, 80);
            this.groupBox_ChangeArea.TabIndex = 7;
            this.groupBox_ChangeArea.TabStop = false;
            this.groupBox_ChangeArea.Text = "更改区域";
            this.groupBox_ChangeArea.Visible = false;
            // 
            // cB_AreasList
            // 
            this.cB_AreasList.FormattingEnabled = true;
            this.cB_AreasList.Location = new System.Drawing.Point(17, 20);
            this.cB_AreasList.Name = "cB_AreasList";
            this.cB_AreasList.Size = new System.Drawing.Size(121, 20);
            this.cB_AreasList.TabIndex = 2;
            this.cB_AreasList.SelectedIndexChanged += new System.EventHandler(this.cB_AreasList_SelectedIndexChanged);
            // 
            // btn_SaveMesh
            // 
            this.btn_SaveMesh.Location = new System.Drawing.Point(17, 46);
            this.btn_SaveMesh.Name = "btn_SaveMesh";
            this.btn_SaveMesh.Size = new System.Drawing.Size(121, 23);
            this.btn_SaveMesh.TabIndex = 0;
            this.btn_SaveMesh.Text = "保存";
            this.btn_SaveMesh.UseVisualStyleBackColor = true;
            this.btn_SaveMesh.Click += new System.EventHandler(this.btn_SaveMesh_Click);
            // 
            // labelInfo
            // 
            this.labelInfo.AutoSize = true;
            this.labelInfo.Location = new System.Drawing.Point(4, 17);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(29, 12);
            this.labelInfo.TabIndex = 3;
            this.labelInfo.Text = "信息";
            // 
            // groupBox_paramenter
            // 
            this.groupBox_paramenter.Controls.Add(this.label_panelChangeParamenter);
            this.groupBox_paramenter.Controls.Add(this.button3);
            this.groupBox_paramenter.Location = new System.Drawing.Point(1566, 72);
            this.groupBox_paramenter.Name = "groupBox_paramenter";
            this.groupBox_paramenter.Size = new System.Drawing.Size(197, 149);
            this.groupBox_paramenter.TabIndex = 9;
            this.groupBox_paramenter.TabStop = false;
            this.groupBox_paramenter.Text = "更改参数";
            this.groupBox_paramenter.Visible = false;
            // 
            // label_panelChangeParamenter
            // 
            this.label_panelChangeParamenter.AutoSize = true;
            this.label_panelChangeParamenter.Location = new System.Drawing.Point(15, 24);
            this.label_panelChangeParamenter.Name = "label_panelChangeParamenter";
            this.label_panelChangeParamenter.Size = new System.Drawing.Size(41, 12);
            this.label_panelChangeParamenter.TabIndex = 1;
            this.label_panelChangeParamenter.Text = "label4";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(17, 108);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(121, 23);
            this.button3.TabIndex = 0;
            this.button3.Text = "保存";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // panelAll
            // 
            this.panelAll.Controls.Add(this.doubleBufferPanel1);
            this.panelAll.Controls.Add(this.labelInfo);
            this.panelAll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelAll.Enabled = false;
            this.panelAll.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.panelAll.Location = new System.Drawing.Point(0, 0);
            this.panelAll.Name = "panelAll";
            this.panelAll.Size = new System.Drawing.Size(1350, 729);
            this.panelAll.TabIndex = 22;
            this.panelAll.Text = "0";
            // 
            // groupBox_Weight
            // 
            this.groupBox_Weight.Controls.Add(this.tB_Weight);
            this.groupBox_Weight.Controls.Add(this.button2);
            this.groupBox_Weight.Controls.Add(this.label3);
            this.groupBox_Weight.Location = new System.Drawing.Point(1566, 227);
            this.groupBox_Weight.Name = "groupBox_Weight";
            this.groupBox_Weight.Size = new System.Drawing.Size(157, 80);
            this.groupBox_Weight.TabIndex = 8;
            this.groupBox_Weight.TabStop = false;
            this.groupBox_Weight.Text = "更改权重";
            this.groupBox_Weight.Visible = false;
            // 
            // tB_Weight
            // 
            this.tB_Weight.Location = new System.Drawing.Point(57, 20);
            this.tB_Weight.Name = "tB_Weight";
            this.tB_Weight.Size = new System.Drawing.Size(81, 21);
            this.tB_Weight.TabIndex = 4;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(17, 47);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(121, 23);
            this.button2.TabIndex = 0;
            this.button2.Text = "保存";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "权重：";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(95)))), ((int)(((byte)(111)))));
            this.panel2.Controls.Add(this.menuStrip3);
            this.panel2.Controls.Add(this.progressBar1);
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Controls.Add(this.menuStrip1);
            this.panel2.Controls.Add(this.menuStrip2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1350, 56);
            this.panel2.TabIndex = 45;
            // 
            // menuStrip3
            // 
            this.menuStrip3.AllowDrop = true;
            this.menuStrip3.AutoSize = false;
            this.menuStrip3.BackColor = System.Drawing.Color.Transparent;
            this.menuStrip3.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.播放ToolStripMenuItem});
            this.menuStrip3.Location = new System.Drawing.Point(908, 5);
            this.menuStrip3.Name = "menuStrip3";
            this.menuStrip3.Size = new System.Drawing.Size(104, 47);
            this.menuStrip3.TabIndex = 47;
            this.menuStrip3.Text = "menuStrip3";
            // 
            // 播放ToolStripMenuItem
            // 
            this.播放ToolStripMenuItem.Enabled = false;
            this.播放ToolStripMenuItem.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.播放ToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.播放ToolStripMenuItem.Image = global::test.Properties.Resources.play;
            this.播放ToolStripMenuItem.Name = "播放ToolStripMenuItem";
            this.播放ToolStripMenuItem.Size = new System.Drawing.Size(96, 43);
            this.播放ToolStripMenuItem.Text = " 播放  ";
            this.播放ToolStripMenuItem.Click += new System.EventHandler(this.播放ToolStripMenuItem_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.BackColor = System.Drawing.Color.Red;
            this.progressBar1.ForeColor = System.Drawing.Color.Black;
            this.progressBar1.Location = new System.Drawing.Point(1066, 16);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(272, 23);
            this.progressBar1.TabIndex = 46;
            this.progressBar1.Visible = false;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.Transparent;
            this.panel4.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel4.BackgroundImage")));
            this.panel4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel4.Controls.Add(this.label6);
            this.panel4.Location = new System.Drawing.Point(230, -8);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(253, 51);
            this.panel4.TabIndex = 44;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label6.Location = new System.Drawing.Point(59, 17);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(132, 27);
            this.label6.TabIndex = 0;
            this.label6.Text = "人群疏散系统";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // menuStrip1
            // 
            this.menuStrip1.AutoSize = false;
            this.menuStrip1.BackColor = System.Drawing.Color.Transparent;
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.工程ToolStripMenuItem,
            this.编辑ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(11, 5);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(221, 47);
            this.menuStrip1.TabIndex = 42;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 工程ToolStripMenuItem
            // 
            this.工程ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.新建工程ToolStripMenuItem,
            this.打开工程ToolStripMenuItem,
            this.编辑ToolStripMenuItem1});
            this.工程ToolStripMenuItem.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.工程ToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.工程ToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("工程ToolStripMenuItem.Image")));
            this.工程ToolStripMenuItem.Name = "工程ToolStripMenuItem";
            this.工程ToolStripMenuItem.Size = new System.Drawing.Size(80, 43);
            this.工程ToolStripMenuItem.Text = "工程  ";
            // 
            // 新建工程ToolStripMenuItem
            // 
            this.新建工程ToolStripMenuItem.Image = global::test.Properties.Resources.newProject;
            this.新建工程ToolStripMenuItem.Name = "新建工程ToolStripMenuItem";
            this.新建工程ToolStripMenuItem.Size = new System.Drawing.Size(144, 26);
            this.新建工程ToolStripMenuItem.Text = "新建工程";
            this.新建工程ToolStripMenuItem.Click += new System.EventHandler(this.新建工程ToolStripMenuItem_Click);
            // 
            // 打开工程ToolStripMenuItem
            // 
            this.打开工程ToolStripMenuItem.Name = "打开工程ToolStripMenuItem";
            this.打开工程ToolStripMenuItem.Size = new System.Drawing.Size(144, 26);
            this.打开工程ToolStripMenuItem.Text = "打开工程";
            this.打开工程ToolStripMenuItem.Click += new System.EventHandler(this.打开工程ToolStripMenuItem_Click);
            // 
            // 编辑ToolStripMenuItem1
            // 
            this.编辑ToolStripMenuItem1.Name = "编辑ToolStripMenuItem1";
            this.编辑ToolStripMenuItem1.Size = new System.Drawing.Size(144, 26);
            this.编辑ToolStripMenuItem1.Text = "另存为";
            this.编辑ToolStripMenuItem1.Click += new System.EventHandler(this.编辑ToolStripMenuItem1_Click);
            // 
            // 编辑ToolStripMenuItem
            // 
            this.编辑ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.编辑工程设置ToolStripMenuItem1});
            this.编辑ToolStripMenuItem.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.编辑ToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.编辑ToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("编辑ToolStripMenuItem.Image")));
            this.编辑ToolStripMenuItem.Name = "编辑ToolStripMenuItem";
            this.编辑ToolStripMenuItem.Size = new System.Drawing.Size(80, 43);
            this.编辑ToolStripMenuItem.Text = "编辑  ";
            // 
            // 编辑工程设置ToolStripMenuItem1
            // 
            this.编辑工程设置ToolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("编辑工程设置ToolStripMenuItem1.Image")));
            this.编辑工程设置ToolStripMenuItem1.Name = "编辑工程设置ToolStripMenuItem1";
            this.编辑工程设置ToolStripMenuItem1.Size = new System.Drawing.Size(144, 26);
            this.编辑工程设置ToolStripMenuItem1.Text = "工程设置";
            this.编辑工程设置ToolStripMenuItem1.Click += new System.EventHandler(this.编辑工程设置ToolStripMenuItem1_Click);
            // 
            // menuStrip2
            // 
            this.menuStrip2.AllowDrop = true;
            this.menuStrip2.AutoSize = false;
            this.menuStrip2.BackColor = System.Drawing.Color.Transparent;
            this.menuStrip2.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menu_Start,
            this.停止ToolStripMenuItem});
            this.menuStrip2.Location = new System.Drawing.Point(678, 5);
            this.menuStrip2.Name = "menuStrip2";
            this.menuStrip2.Size = new System.Drawing.Size(344, 47);
            this.menuStrip2.TabIndex = 43;
            this.menuStrip2.Text = "menuStrip2";
            // 
            // menu_Start
            // 
            this.menu_Start.Enabled = false;
            this.menu_Start.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.menu_Start.ForeColor = System.Drawing.Color.White;
            this.menu_Start.Image = global::test.Properties.Resources.simulate_play;
            this.menu_Start.Name = "menu_Start";
            this.menu_Start.Size = new System.Drawing.Size(96, 43);
            this.menu_Start.Text = " 仿真  ";
            this.menu_Start.Click += new System.EventHandler(this.menu_Start_Click);
            // 
            // 停止ToolStripMenuItem
            // 
            this.停止ToolStripMenuItem.Enabled = false;
            this.停止ToolStripMenuItem.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.停止ToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.停止ToolStripMenuItem.Image = global::test.Properties.Resources.simulate_stop;
            this.停止ToolStripMenuItem.Name = "停止ToolStripMenuItem";
            this.停止ToolStripMenuItem.Size = new System.Drawing.Size(90, 43);
            this.停止ToolStripMenuItem.Text = " 停止 ";
            this.停止ToolStripMenuItem.Click += new System.EventHandler(this.停止ToolStripMenuItem_Click);
            // 
            // timer_Progress
            // 
            this.timer_Progress.Tick += new System.EventHandler(this.timer_Progress_Tick);
            // 
            // BottomToolStripPanel
            // 
            this.BottomToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.BottomToolStripPanel.Name = "BottomToolStripPanel";
            this.BottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.BottomToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.BottomToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // TopToolStripPanel
            // 
            this.TopToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.TopToolStripPanel.Name = "TopToolStripPanel";
            this.TopToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.TopToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.TopToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // RightToolStripPanel
            // 
            this.RightToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.RightToolStripPanel.Name = "RightToolStripPanel";
            this.RightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.RightToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.RightToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // LeftToolStripPanel
            // 
            this.LeftToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.LeftToolStripPanel.Name = "LeftToolStripPanel";
            this.LeftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.LeftToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.LeftToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // ContentPanel
            // 
            this.ContentPanel.Size = new System.Drawing.Size(527, 18);
            // 
            // panelInfo
            // 
            this.panelInfo.BackColor = System.Drawing.Color.White;
            this.panelInfo.Controls.Add(this.radioButton1);
            this.panelInfo.Controls.Add(this.panel3);
            this.panelInfo.Controls.Add(this.radioButton2);
            this.panelInfo.Location = new System.Drawing.Point(1583, 906);
            this.panelInfo.Name = "panelInfo";
            this.panelInfo.Size = new System.Drawing.Size(403, 103);
            this.panelInfo.TabIndex = 26;
            this.panelInfo.Visible = false;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(307, 43);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(71, 16);
            this.radioButton1.TabIndex = 4;
            this.radioButton1.Text = "详细信息";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.Visible = false;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.panel3.Controls.Add(this.labelOutBili);
            this.panel3.Controls.Add(this.labelOutAgents);
            this.panel3.Controls.Add(this.labelOutdoor);
            this.panel3.Controls.Add(this.label5);
            this.panel3.Location = new System.Drawing.Point(17, 21);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(274, 71);
            this.panel3.TabIndex = 5;
            // 
            // labelOutBili
            // 
            this.labelOutBili.AutoSize = true;
            this.labelOutBili.Location = new System.Drawing.Point(211, 23);
            this.labelOutBili.Name = "labelOutBili";
            this.labelOutBili.Size = new System.Drawing.Size(77, 12);
            this.labelOutBili.TabIndex = 2;
            this.labelOutBili.Text = "labelOutBili";
            // 
            // labelOutAgents
            // 
            this.labelOutAgents.AutoSize = true;
            this.labelOutAgents.Location = new System.Drawing.Point(105, 23);
            this.labelOutAgents.Name = "labelOutAgents";
            this.labelOutAgents.Size = new System.Drawing.Size(89, 12);
            this.labelOutAgents.TabIndex = 1;
            this.labelOutAgents.Text = "labelOutAgents";
            // 
            // labelOutdoor
            // 
            this.labelOutdoor.AutoSize = true;
            this.labelOutdoor.Location = new System.Drawing.Point(10, 23);
            this.labelOutdoor.Name = "labelOutdoor";
            this.labelOutdoor.Size = new System.Drawing.Size(77, 12);
            this.labelOutdoor.TabIndex = 0;
            this.labelOutdoor.Text = "labelOutdoor";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.ForeColor = System.Drawing.SystemColors.Highlight;
            this.label5.Location = new System.Drawing.Point(105, 5);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 12);
            this.label5.TabIndex = 3;
            this.label5.Text = "实时疏散数据";
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Checked = true;
            this.radioButton2.Location = new System.Drawing.Point(307, 21);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(59, 16);
            this.radioButton2.TabIndex = 2;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "柱状图";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.Visible = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cB_showGoalNow);
            this.groupBox2.Controls.Add(this.chBoxChart);
            this.groupBox2.Controls.Add(this.cB_ShowGird);
            this.groupBox2.Location = new System.Drawing.Point(1572, 473);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(151, 137);
            this.groupBox2.TabIndex = 43;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "显示选项";
            this.groupBox2.Visible = false;
            // 
            // cB_showGoalNow
            // 
            this.cB_showGoalNow.AutoSize = true;
            this.cB_showGoalNow.Location = new System.Drawing.Point(16, 26);
            this.cB_showGoalNow.Name = "cB_showGoalNow";
            this.cB_showGoalNow.Size = new System.Drawing.Size(96, 16);
            this.cB_showGoalNow.TabIndex = 9;
            this.cB_showGoalNow.Text = "显示当前目标";
            this.cB_showGoalNow.UseVisualStyleBackColor = true;
            // 
            // chBoxChart
            // 
            this.chBoxChart.AutoSize = true;
            this.chBoxChart.Checked = true;
            this.chBoxChart.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chBoxChart.Location = new System.Drawing.Point(16, 70);
            this.chBoxChart.Name = "chBoxChart";
            this.chBoxChart.Size = new System.Drawing.Size(96, 16);
            this.chBoxChart.TabIndex = 13;
            this.chBoxChart.Text = "显示实时信息";
            this.chBoxChart.UseVisualStyleBackColor = true;
            this.chBoxChart.CheckedChanged += new System.EventHandler(this.chBoxChart_CheckedChanged);
            // 
            // cB_ShowGird
            // 
            this.cB_ShowGird.AutoSize = true;
            this.cB_ShowGird.Location = new System.Drawing.Point(16, 48);
            this.cB_ShowGird.Name = "cB_ShowGird";
            this.cB_ShowGird.Size = new System.Drawing.Size(72, 16);
            this.cB_ShowGird.TabIndex = 11;
            this.cB_ShowGird.Text = "显示网格";
            this.cB_ShowGird.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rB_Border);
            this.groupBox1.Controls.Add(this.rB_AreaSet);
            this.groupBox1.Controls.Add(this.rB_AreaPartition);
            this.groupBox1.Controls.Add(this.rB_Mesh);
            this.groupBox1.Location = new System.Drawing.Point(1751, 473);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(120, 137);
            this.groupBox1.TabIndex = 42;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "显示模式";
            this.groupBox1.Visible = false;
            // 
            // rB_Border
            // 
            this.rB_Border.AutoSize = true;
            this.rB_Border.Location = new System.Drawing.Point(11, 25);
            this.rB_Border.Name = "rB_Border";
            this.rB_Border.Size = new System.Drawing.Size(71, 16);
            this.rB_Border.TabIndex = 4;
            this.rB_Border.Text = "道路边界";
            this.rB_Border.UseVisualStyleBackColor = true;
            // 
            // rB_AreaSet
            // 
            this.rB_AreaSet.AutoSize = true;
            this.rB_AreaSet.Location = new System.Drawing.Point(13, 91);
            this.rB_AreaSet.Name = "rB_AreaSet";
            this.rB_AreaSet.Size = new System.Drawing.Size(95, 16);
            this.rB_AreaSet.TabIndex = 10;
            this.rB_AreaSet.Text = "分区参数设置";
            this.rB_AreaSet.UseVisualStyleBackColor = true;
            this.rB_AreaSet.CheckedChanged += new System.EventHandler(this.rB_AreaSet_CheckedChanged);
            // 
            // rB_AreaPartition
            // 
            this.rB_AreaPartition.AutoSize = true;
            this.rB_AreaPartition.Location = new System.Drawing.Point(11, 69);
            this.rB_AreaPartition.Name = "rB_AreaPartition";
            this.rB_AreaPartition.Size = new System.Drawing.Size(71, 16);
            this.rB_AreaPartition.TabIndex = 6;
            this.rB_AreaPartition.Text = "分区更改";
            this.rB_AreaPartition.UseVisualStyleBackColor = true;
            this.rB_AreaPartition.CheckedChanged += new System.EventHandler(this.rB_AreaPartition_CheckedChanged);
            // 
            // rB_Mesh
            // 
            this.rB_Mesh.AutoSize = true;
            this.rB_Mesh.Checked = true;
            this.rB_Mesh.Location = new System.Drawing.Point(11, 47);
            this.rB_Mesh.Name = "rB_Mesh";
            this.rB_Mesh.Size = new System.Drawing.Size(71, 16);
            this.rB_Mesh.TabIndex = 5;
            this.rB_Mesh.TabStop = true;
            this.rB_Mesh.Text = "三角网格";
            this.rB_Mesh.UseVisualStyleBackColor = true;
            // 
            // trackBar2
            // 
            this.trackBar2.Location = new System.Drawing.Point(1560, 649);
            this.trackBar2.Name = "trackBar2";
            this.trackBar2.Size = new System.Drawing.Size(186, 45);
            this.trackBar2.TabIndex = 46;
            this.trackBar2.Visible = false;
            // 
            // timerforHeatmap
            // 
            this.timerforHeatmap.Tick += new System.EventHandler(this.timerforHeatmap_Tick);
            // 
            // rB_heatmap
            // 
            this.rB_heatmap.AutoSize = true;
            this.rB_heatmap.Location = new System.Drawing.Point(18, 56);
            this.rB_heatmap.Name = "rB_heatmap";
            this.rB_heatmap.Size = new System.Drawing.Size(59, 16);
            this.rB_heatmap.TabIndex = 21;
            this.rB_heatmap.Text = "热力图";
            this.rB_heatmap.UseVisualStyleBackColor = true;
            this.rB_heatmap.CheckedChanged += new System.EventHandler(this.rB_heatmap_CheckedChanged);
            // 
            // rB_corwds
            // 
            this.rB_corwds.AutoSize = true;
            this.rB_corwds.Checked = true;
            this.rB_corwds.Location = new System.Drawing.Point(18, 22);
            this.rB_corwds.Name = "rB_corwds";
            this.rB_corwds.Size = new System.Drawing.Size(35, 16);
            this.rB_corwds.TabIndex = 20;
            this.rB_corwds.TabStop = true;
            this.rB_corwds.Text = "人";
            this.rB_corwds.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rB_corwds);
            this.panel1.Controls.Add(this.rB_heatmap);
            this.panel1.Location = new System.Drawing.Point(1778, 72);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(117, 100);
            this.panel1.TabIndex = 22;
            this.panel1.Tag = "是对人赋予颜色，还是用只一个热力图";
            this.panel1.Visible = false;
            // 
            // timerRead
            // 
            this.timerRead.Tick += new System.EventHandler(this.timerRead_Tick);
            // 
            // doubleBufferPanel1
            // 
            this.doubleBufferPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(88)))), ((int)(((byte)(88)))));
            this.doubleBufferPanel1.Controls.Add(this.panelReadPlay);
            this.doubleBufferPanel1.Controls.Add(this.SidePanel);
            this.doubleBufferPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.doubleBufferPanel1.Location = new System.Drawing.Point(0, 0);
            this.doubleBufferPanel1.Name = "doubleBufferPanel1";
            this.doubleBufferPanel1.Size = new System.Drawing.Size(1350, 729);
            this.doubleBufferPanel1.TabIndex = 2;
            this.doubleBufferPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.doubleBufferPanel1_Paint);
            this.doubleBufferPanel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.doubleBufferPanel1_MouseDown);
            this.doubleBufferPanel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.doubleBufferPanel1_MouseMove);
            this.doubleBufferPanel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.doubleBufferPanel1_MouseUp);
            this.doubleBufferPanel1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.doubleBufferPanel1_MouseWheel);
            // 
            // panelReadPlay
            // 
            this.panelReadPlay.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(93)))), ((int)(((byte)(103)))));
            this.panelReadPlay.ColumnCount = 3;
            this.panelReadPlay.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.panelReadPlay.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.panelReadPlay.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.panelReadPlay.Controls.Add(this.panel7, 1, 0);
            this.panelReadPlay.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelReadPlay.Location = new System.Drawing.Point(0, 678);
            this.panelReadPlay.Name = "panelReadPlay";
            this.panelReadPlay.RowCount = 1;
            this.panelReadPlay.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.panelReadPlay.Size = new System.Drawing.Size(1350, 51);
            this.panelReadPlay.TabIndex = 46;
            this.panelReadPlay.Visible = false;
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.label1);
            this.panel7.Controls.Add(this.pictureBox_Stop);
            this.panel7.Controls.Add(this.cB_MultiSpeed);
            this.panel7.Controls.Add(this.pictureBox_Play);
            this.panel7.Controls.Add(this.label_FrameMax);
            this.panel7.Controls.Add(this.trackBar_replay);
            this.panel7.Controls.Add(this.label_frameMin);
            this.panel7.Location = new System.Drawing.Point(128, 3);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(1094, 45);
            this.panel7.TabIndex = 46;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(3, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 44;
            this.label1.Text = "播放速度:";
            // 
            // pictureBox_Stop
            // 
            this.pictureBox_Stop.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox_Stop.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox_Stop.Image")));
            this.pictureBox_Stop.Location = new System.Drawing.Point(1042, 0);
            this.pictureBox_Stop.Name = "pictureBox_Stop";
            this.pictureBox_Stop.Size = new System.Drawing.Size(48, 50);
            this.pictureBox_Stop.TabIndex = 47;
            this.pictureBox_Stop.TabStop = false;
            this.pictureBox_Stop.Click += new System.EventHandler(this.pictureBox_Stop_Click);
            // 
            // cB_MultiSpeed
            // 
            this.cB_MultiSpeed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cB_MultiSpeed.FormattingEnabled = true;
            this.cB_MultiSpeed.Items.AddRange(new object[] {
            "X1",
            "X2",
            "X5",
            "X10"});
            this.cB_MultiSpeed.Location = new System.Drawing.Point(68, 11);
            this.cB_MultiSpeed.Name = "cB_MultiSpeed";
            this.cB_MultiSpeed.Size = new System.Drawing.Size(46, 20);
            this.cB_MultiSpeed.TabIndex = 43;
            this.cB_MultiSpeed.Text = "X1";
            this.cB_MultiSpeed.SelectedIndexChanged += new System.EventHandler(this.cB_MultiSpeed_SelectedIndexChanged);
            // 
            // pictureBox_Play
            // 
            this.pictureBox_Play.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox_Play.Image = global::test.Properties.Resources.video_play;
            this.pictureBox_Play.Location = new System.Drawing.Point(978, 0);
            this.pictureBox_Play.Name = "pictureBox_Play";
            this.pictureBox_Play.Size = new System.Drawing.Size(48, 50);
            this.pictureBox_Play.TabIndex = 46;
            this.pictureBox_Play.TabStop = false;
            this.pictureBox_Play.Click += new System.EventHandler(this.pictureBox_Play_Click);
            // 
            // label_FrameMax
            // 
            this.label_FrameMax.Location = new System.Drawing.Point(952, 14);
            this.label_FrameMax.Name = "label_FrameMax";
            this.label_FrameMax.Size = new System.Drawing.Size(13, 10);
            this.label_FrameMax.TabIndex = 42;
            this.label_FrameMax.Text = "0";
            this.label_FrameMax.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.label_FrameMax.Visible = false;
            // 
            // trackBar_replay
            // 
            this.trackBar_replay.AutoSize = false;
            this.trackBar_replay.BackColor = System.Drawing.SystemColors.Control;
            this.trackBar_replay.Location = new System.Drawing.Point(149, 6);
            this.trackBar_replay.Maximum = 2200;
            this.trackBar_replay.Minimum = 1;
            this.trackBar_replay.Name = "trackBar_replay";
            this.trackBar_replay.Size = new System.Drawing.Size(796, 36);
            this.trackBar_replay.TabIndex = 38;
            this.trackBar_replay.Value = 1;
            this.trackBar_replay.ValueChanged += new System.EventHandler(this.trackBar_replay_ValueChanged);
            this.trackBar_replay.MouseDown += new System.Windows.Forms.MouseEventHandler(this.trackBar_replay_MouseDown);
            this.trackBar_replay.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trackBar_replay_MouseUp);
            // 
            // label_frameMin
            // 
            this.label_frameMin.ForeColor = System.Drawing.Color.White;
            this.label_frameMin.Location = new System.Drawing.Point(105, 14);
            this.label_frameMin.Name = "label_frameMin";
            this.label_frameMin.Size = new System.Drawing.Size(43, 11);
            this.label_frameMin.TabIndex = 39;
            this.label_frameMin.Text = "0";
            this.label_frameMin.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label_frameMin.Visible = false;
            // 
            // SidePanel
            // 
            this.SidePanel.BackColor = System.Drawing.Color.Transparent;
            this.SidePanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("SidePanel.BackgroundImage")));
            this.SidePanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.SidePanel.Controls.Add(this.panel6);
            this.SidePanel.Controls.Add(this.panel5);
            this.SidePanel.Controls.Add(this.label4);
            this.SidePanel.Controls.Add(this.BackGroundPanel);
            this.SidePanel.Location = new System.Drawing.Point(-7, 62);
            this.SidePanel.Name = "SidePanel";
            this.SidePanel.Size = new System.Drawing.Size(420, 497);
            this.SidePanel.TabIndex = 45;
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(90)))), ((int)(((byte)(96)))));
            this.panel6.Controls.Add(this.checkBox_outputVRS);
            this.panel6.Controls.Add(this.cB_ShowExits);
            this.panel6.Controls.Add(this.cB_DrawColorRule);
            this.panel6.Location = new System.Drawing.Point(37, 419);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(335, 60);
            this.panel6.TabIndex = 46;
            // 
            // checkBox_outputVRS
            // 
            this.checkBox_outputVRS.AutoSize = true;
            this.checkBox_outputVRS.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(90)))), ((int)(((byte)(96)))));
            this.checkBox_outputVRS.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBox_outputVRS.ForeColor = System.Drawing.Color.White;
            this.checkBox_outputVRS.ImageKey = "(无)";
            this.checkBox_outputVRS.Location = new System.Drawing.Point(15, 36);
            this.checkBox_outputVRS.Name = "checkBox_outputVRS";
            this.checkBox_outputVRS.Size = new System.Drawing.Size(124, 21);
            this.checkBox_outputVRS.TabIndex = 30;
            this.checkBox_outputVRS.Text = "输出vrs文件(测试)";
            this.checkBox_outputVRS.UseVisualStyleBackColor = false;
            // 
            // cB_ShowExits
            // 
            this.cB_ShowExits.AutoSize = true;
            this.cB_ShowExits.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(90)))), ((int)(((byte)(96)))));
            this.cB_ShowExits.Checked = true;
            this.cB_ShowExits.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cB_ShowExits.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cB_ShowExits.ForeColor = System.Drawing.Color.White;
            this.cB_ShowExits.ImageKey = "(无)";
            this.cB_ShowExits.Location = new System.Drawing.Point(15, 6);
            this.cB_ShowExits.Name = "cB_ShowExits";
            this.cB_ShowExits.Size = new System.Drawing.Size(99, 21);
            this.cB_ShowExits.TabIndex = 27;
            this.cB_ShowExits.Text = "显示出口标识";
            this.cB_ShowExits.UseVisualStyleBackColor = false;
            // 
            // cB_DrawColorRule
            // 
            this.cB_DrawColorRule.AutoSize = true;
            this.cB_DrawColorRule.Checked = true;
            this.cB_DrawColorRule.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cB_DrawColorRule.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cB_DrawColorRule.ForeColor = System.Drawing.Color.White;
            this.cB_DrawColorRule.Location = new System.Drawing.Point(194, 6);
            this.cB_DrawColorRule.Name = "cB_DrawColorRule";
            this.cB_DrawColorRule.Size = new System.Drawing.Size(123, 21);
            this.cB_DrawColorRule.TabIndex = 29;
            this.cB_DrawColorRule.Text = "显示密度颜色参考";
            this.cB_DrawColorRule.UseVisualStyleBackColor = true;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(90)))), ((int)(((byte)(96)))));
            this.panel5.Controls.Add(this.labelPanelInfo);
            this.panel5.Location = new System.Drawing.Point(37, 19);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(335, 131);
            this.panel5.TabIndex = 45;
            // 
            // labelPanelInfo
            // 
            this.labelPanelInfo.AutoSize = true;
            this.labelPanelInfo.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.labelPanelInfo.ForeColor = System.Drawing.Color.White;
            this.labelPanelInfo.Location = new System.Drawing.Point(7, 7);
            this.labelPanelInfo.Name = "labelPanelInfo";
            this.labelPanelInfo.Size = new System.Drawing.Size(17, 20);
            this.labelPanelInfo.TabIndex = 27;
            this.labelPanelInfo.Text = "  ";
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("宋体", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(376, 231);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(20, 32);
            this.label4.TabIndex = 44;
            this.label4.Text = "<<";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label4.Click += new System.EventHandler(this.labelDrowBack_Click);
            // 
            // BackGroundPanel
            // 
            this.BackGroundPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(90)))), ((int)(((byte)(96)))));
            this.BackGroundPanel.Controls.Add(this.label7);
            this.BackGroundPanel.Controls.Add(this.chartOuts);
            this.BackGroundPanel.Location = new System.Drawing.Point(37, 165);
            this.BackGroundPanel.Name = "BackGroundPanel";
            this.BackGroundPanel.Size = new System.Drawing.Size(335, 240);
            this.BackGroundPanel.TabIndex = 44;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(12, 4);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(37, 20);
            this.label7.TabIndex = 46;
            this.label7.Text = "人数";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chartOuts
            // 
            this.chartOuts.BackColor = System.Drawing.Color.Transparent;
            chartArea1.AxisX.IntervalOffset = 1D;
            chartArea1.AxisX.IsLabelAutoFit = false;
            chartArea1.AxisX.LabelAutoFitStyle = ((System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles)((((System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.IncreaseFont | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.DecreaseFont) 
            | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.StaggeredLabels) 
            | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.WordWrap)));
            chartArea1.AxisX.LabelStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea1.AxisX.LabelStyle.ForeColor = System.Drawing.Color.White;
            chartArea1.AxisX.LineColor = System.Drawing.Color.White;
            chartArea1.AxisX.MajorGrid.Enabled = false;
            chartArea1.AxisX.MajorTickMark.Enabled = false;
            chartArea1.AxisX.Maximum = 11.5D;
            chartArea1.AxisX.Minimum = 0D;
            chartArea1.AxisX.ScaleBreakStyle.Enabled = true;
            chartArea1.AxisX.Title = "各出口已疏散人数";
            chartArea1.AxisX.TitleForeColor = System.Drawing.Color.White;
            chartArea1.AxisY.IsLabelAutoFit = false;
            chartArea1.AxisY.LabelStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea1.AxisY.LabelStyle.ForeColor = System.Drawing.Color.White;
            chartArea1.AxisY.LineColor = System.Drawing.Color.White;
            chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.White;
            chartArea1.AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea1.AxisY.MajorTickMark.Enabled = false;
            chartArea1.BackColor = System.Drawing.Color.Transparent;
            chartArea1.Name = "ChartArea1";
            this.chartOuts.ChartAreas.Add(chartArea1);
            this.chartOuts.Location = new System.Drawing.Point(0, 25);
            this.chartOuts.Margin = new System.Windows.Forms.Padding(0);
            this.chartOuts.Name = "chartOuts";
            this.chartOuts.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.EarthTones;
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            series1.YValuesPerPoint = 6;
            this.chartOuts.Series.Add(series1);
            this.chartOuts.Size = new System.Drawing.Size(335, 212);
            this.chartOuts.TabIndex = 1;
            // 
            // MainUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(88)))), ((int)(((byte)(88)))));
            this.ClientSize = new System.Drawing.Size(1350, 729);
            this.Controls.Add(this.trackBar2);
            this.Controls.Add(this.panelInfo);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.groupBox_paramenter);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox_ChangeArea);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panelAll);
            this.Controls.Add(this.groupBox_Weight);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip2;
            this.Name = "MainUI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "东门步行街人群疏散系统-空项目";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.newUI_FormClosed);
            this.Load += new System.EventHandler(this.newUI_Load);
            this.groupBox_ChangeArea.ResumeLayout(false);
            this.groupBox_paramenter.ResumeLayout(false);
            this.groupBox_paramenter.PerformLayout();
            this.panelAll.ResumeLayout(false);
            this.panelAll.PerformLayout();
            this.groupBox_Weight.ResumeLayout(false);
            this.groupBox_Weight.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.menuStrip3.ResumeLayout(false);
            this.menuStrip3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.menuStrip2.ResumeLayout(false);
            this.menuStrip2.PerformLayout();
            this.panelInfo.ResumeLayout(false);
            this.panelInfo.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.doubleBufferPanel1.ResumeLayout(false);
            this.panelReadPlay.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Stop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Play)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_replay)).EndInit();
            this.SidePanel.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.BackGroundPanel.ResumeLayout(false);
            this.BackGroundPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartOuts)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        

        #endregion
        private Timer timerSimulate;
        private RadioButton rB_Mesh;
        private RadioButton rB_AreaPartition;
        private RadioButton rB_Border;
        private GroupBox groupBox_ChangeArea;
        private Button btn_SaveMesh;
        private CheckBox cB_showGoalNow;
        private RadioButton rB_AreaSet;
        private CheckBox cB_ShowGird;
        public CheckBox chBoxChart;
        public Windows.DoubleBufferPanel doubleBufferPanel1;
        private RadioButton radioButton1;
        private RadioButton radioButton2;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartOuts;
        private Panel panel3;
        private Label label5;
        private Label labelInfo;
        public Panel panelInfo;
        public Label labelOutBili;
        public Label labelOutAgents;
        public Label labelOutdoor;
        private Label labelPanelInfo;
        private ComboBox cB_AreasList;
        public CheckBox cB_ShowExits;
        private GroupBox groupBox_paramenter;
        private Button button3;
        private Label label_panelChangeParamenter;
        public CheckBox cB_DrawColorRule;
        private Panel panelAll;
        private GroupBox groupBox_Weight;
        private TextBox tB_Weight;
        private Button button2;
        private Label label3;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private ProgressBar progressBar1;
        private Timer timer_Progress;
        private ToolStripMenuItem 播放ToolStripMenuItem;
        private MenuStrip menuStrip3;
        private ToolStripMenuItem menu_Start;
        private ToolStripMenuItem 停止ToolStripMenuItem;
        private MenuStrip menuStrip2;
        private ToolStripMenuItem 工程ToolStripMenuItem;
        private ToolStripMenuItem 新建工程ToolStripMenuItem;
        private ToolStripMenuItem 打开工程ToolStripMenuItem;
        private ToolStripMenuItem 编辑ToolStripMenuItem1;
        private ToolStripMenuItem 编辑ToolStripMenuItem;
        private ToolStripMenuItem 编辑工程设置ToolStripMenuItem1;
        private MenuStrip menuStrip1;
        private Panel panel2;
        private Panel panel4;
        private Label label6;
        private ToolStripPanel BottomToolStripPanel;
        private ToolStripPanel TopToolStripPanel;
        private ToolStripPanel RightToolStripPanel;
        private ToolStripPanel LeftToolStripPanel;
        private ToolStripContentPanel ContentPanel;
        private Panel SidePanel;
        private Label label4;
        private Panel BackGroundPanel;
        private Panel panel5;
        private Label label7;
        private Panel panel6;
        private TrackBar trackBar2;
        public CheckBox checkBox_outputVRS;
        private Timer timerforHeatmap;
        private RadioButton rB_heatmap;
        private RadioButton rB_corwds;
        private Panel panel1;
        private Timer timerRead;
        private PictureBox pictureBox_Stop;
        private PictureBox pictureBox_Play;
        private Label label1;
        private ComboBox cB_MultiSpeed;
        private Label label_FrameMax;
        private Label label_frameMin;
        public TrackBar trackBar_replay;
        private Panel panel7;
        private TableLayoutPanel panelReadPlay;
    }
}