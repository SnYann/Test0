using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using test.Scripts;
using Window;

namespace test.UI
{
    public partial class SettingPanel : Form
    {
        //是否是浏览模式
        public bool isScan = false;
        //是否新建XML文件
        public bool isAddNewXml = false;

        ////用来存放出口选择
        //List<List<int>> _OutID = new List<List<int>>();
        
        public SettingPanel(int command,string filename)
        {
            
            InitializeComponent();
            

            this.cb_outMode.Text = cb_outMode.Items[0].ToString();
            switch(command)
            {
                case 0://新建, 只有确定
                    btn_fromFile.Visible = true;
                    btn_OK.Visible=true;
                    btn_Save.Visible = false;
                    break;
                case 1:// 打开当前设置, 没有确定也没有保存
                    btn_fromFile.Visible = false;
                    btn_OK.Visible = false;
                    btn_Save.Visible = false;
                    break;
                case 2://编辑 保存XML配置文件, 只有保存按键
                    btn_fromFile.Visible = true;
                    btn_OK.Visible =false;
                    btn_Save.Visible = true;
                    break;
            }

            InitDataView(filename);

        }

        //切换为浏览模式
        public void EditModeToScanMode()
        {
            this.button4.Visible = true;
            this.button3.Visible = false;
            this.btn_OK.Visible = false;
            this.btn_fromFile.Visible = false;
            this.textBox1.Enabled = false;
            ((DataGridViewComboBoxColumn)dataGridView_Areas.Columns[3]).DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            isScan = true;
            ChangeEditState();
        }

        //改变datagridview可编辑状态
        public void ChangeEditState()
        {
            if(isScan)
            {
                dataGridView_Areas.Columns[1].ReadOnly = true;
                dataGridView_Areas.Columns[2].ReadOnly = true;
                dataGridView_Areas.Columns[3].ReadOnly = true;
                dataGridView2.Columns[1].ReadOnly = true;
                dataGridView2.Columns[2].ReadOnly = true;
            }
            else
            {
                dataGridView_Areas.Columns[1].ReadOnly = false;
                dataGridView_Areas.Columns[2].ReadOnly = false;
                dataGridView_Areas.Columns[3].ReadOnly = false;
                dataGridView2.Columns[1].ReadOnly = false;
                dataGridView2.Columns[2].ReadOnly = false;
            }
        }

        public void InitDataView(string path)
        {
            dataGridView_Areas.Rows.Clear();
            dataGridView2.Rows.Clear();

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(path);
                XmlElement settings = xmlDocument.DocumentElement;
                XmlNodeList people = settings.GetElementsByTagName("People");
                XmlNodeList selectNearest = settings.GetElementsByTagName("SelectNearest");
                bool isSelectNearest = ((XmlElement)selectNearest[0]).GetAttribute("bool") == "0" ? false : true;
                XmlNodeList areas = settings.GetElementsByTagName("Areas");
                XmlNodeList areaouts = settings.GetElementsByTagName("AreaOuts");
                XmlNodeList colorDensity = settings.GetElementsByTagName("ColorDensity");
                foreach (XmlElement element in people)
                {
                    textBox1.Text = element.GetAttribute("num");
                    cb_ditribution.SelectedIndex = int.Parse(element.GetAttribute("distribution"));
                    foreach (XmlNode node in element)
                    {
                        int index = dataGridView2.Rows.Add();
                        dataGridView2.Rows[index].Cells[0].Value = node.Name;
                        dataGridView2.Rows[index].Cells[1].Value = ((XmlElement)node).GetAttribute("percentage");
                        dataGridView2.Rows[index].Cells[2].Value = ((XmlElement)node).GetAttribute("speedMin");
                        dataGridView2.Rows[index].Cells[3].Value = ((XmlElement)node).GetAttribute("speedMax");
                        dataGridView2.Rows[index].Cells[4].Value = ((XmlElement)node).GetAttribute("responseTimeMin");
                        dataGridView2.Rows[index].Cells[5].Value = ((XmlElement)node).GetAttribute("responseTimeMax");
                    }
                }

                if (isSelectNearest) cb_outMode.SelectedIndex = 1;

                Sample._OutID.Clear();
                foreach (XmlElement element in areaouts)//这里应该只有一个AgentOuts
                {
                    XmlNodeList outs = element.GetElementsByTagName("Outs");
                    foreach (XmlElement e in outs)
                    {
                        List<int> outids = new List<int>();
                        foreach (XmlNode node in e)
                        {
                            outids.Add(int.Parse(((XmlElement)node).GetAttribute("outid")));
                        }
                        Sample._OutID.Add(new List<int>(outids));
                    }
                }

                HeatMap.DensityFloat.Clear();
                foreach (XmlElement element in colorDensity)//这里应该只有一个AgentOuts
                {
                    foreach (XmlNode node in element)
                    {
                        try
                        {
                            HeatMap.DensityFloat.Add(float.Parse(((XmlElement)node).GetAttribute("dens")));
                        }
                        catch
                        {

                        }
                    }
                }
                if (HeatMap.DensityFloat.Count < 5)
                {
                    MessageBox.Show("颜色密度对应数据出错", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                density1.Text = HeatMap.DensityFloat[0].ToString();
                density2.Text = HeatMap.DensityFloat[1].ToString();
                density3.Text = HeatMap.DensityFloat[2].ToString();
                density4.Text = HeatMap.DensityFloat[3].ToString();
                density5.Text = HeatMap.DensityFloat[4].ToString();

                for (int i = 0; i < areas.Count; i++)
                {
                    XmlNodeList nodes = ((XmlElement)areas[i]).ChildNodes;
                    for (int y = 0; y < nodes.Count; y++)
                    {
                        int index = dataGridView_Areas.Rows.Add();
                        dataGridView_Areas.Rows[index].Cells[0].Value = ((XmlElement)nodes[y]).GetAttribute("id");
                        dataGridView_Areas.Rows[index].Cells[1].Value = ((XmlElement)nodes[y]).GetAttribute("name");
                        dataGridView_Areas.Rows[index].Cells[2].Value = ((XmlElement)nodes[y]).GetAttribute("density");
                        dataGridView_Areas.Rows[index].Cells[4].Value = ((XmlElement)nodes[y]).GetAttribute("receiveTime");
                        //dataGridView1.Rows[index].Cells[3].Value = ((DataGridViewComboBoxColumn)dataGridView1.Columns[3]).Items[entranceId].ToString();
                        //dataGridView1.Rows[index].Cells[3].Value = tempColumn.Items[0].ToString();
                        //dataGridView1.Rows[index].Cells[3].Value = (int.Parse(((XmlElement)node).GetAttribute("pos1")) + 1).ToString();
                        //(dataGridView1.Rows[index].Cells[3]).Value = (int.Parse(((XmlElement)nodes[y]).GetAttribute("pos1")) + 1).ToString();
                        var cb = (DataGridViewComboBoxCell)dataGridView_Areas.Rows[index].Cells[3];
                        for (int n = 0; n < Sample._OutID[i].Count; n++)
                        {
                            cb.Items.Add(Sample._OutID[i][n].ToString());
                        }
                        cb.Items.Add("最近出口");
                        //判断当前出口在出口选项中排第几,然后让其选择

                        if(((XmlElement)nodes[y]).GetAttribute("pos1")=="最近出口")
                        {
                            dataGridView_Areas.Rows[index].Cells[3].Value = "最近出口";
                        }
                        else
                        {
                            int pos1 = (int.Parse(((XmlElement)nodes[y]).GetAttribute("pos1")) + 1);
                            bool outerror = true;
                            for (int n = 0; n < Sample._OutID[i].Count; n++)
                            {
                                if (pos1 == Sample._OutID[i][n])
                                {
                                    dataGridView_Areas.Rows[index].Cells[3].Value = pos1.ToString();
                                    outerror = false;
                                    break;
                                }
                            }
                            if (outerror) MessageBox.Show("原始出口数据校验错误,需手动设置部分出口目标", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        
                       
                    }
                }
            }
            catch
            {
               
            }
            
        }


     

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView_Areas.BeginEdit(true); ;
           /* dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;
            if(e.ColumnIndex>1&&e.ColumnIndex>=0)
            {
                DataGridViewComboBoxColumn combo = dataGridView1.Columns[e.ColumnIndex] as DataGridViewComboBoxColumn;
                if(combo!=null)
                {
                    DataGridViewComboBoxEditingControl comboEdit = dataGridView1.EditingControl as DataGridViewComboBoxEditingControl;
                    if(comboEdit!=null)
                    {
                        comboEdit.DroppedDown = true;
                    }
                }
                DataGridViewTextBoxColumn textBoxColumn = dataGridView1.Columns[e.ColumnIndex] as DataGridViewTextBoxColumn;
                if(textBoxColumn!=null)
                {
                    dataGridView1.BeginEdit(true);
                }
            }*/
        }

        private void dataGridView1_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex==-1|| e.ColumnIndex == 3||isScan)
            {
                dataGridView_Areas.Cursor = Cursors.Default;
                return;
            }
            if(e.ColumnIndex==2)
            {
                dataGridView_Areas.Cursor = Cursors.IBeam;
            }
            else if(e.ColumnIndex<2)
            {
                dataGridView_Areas.Cursor = Cursors.No;
            }
        }

        private void dataGridView2_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1||isScan)
            {
                dataGridView2.Cursor = Cursors.Default;
                return;
            }
            if (e.ColumnIndex>0)
            {
                dataGridView2.Cursor = Cursors.IBeam;
            }
            else
            {
                dataGridView2.Cursor = Cursors.No;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            isScan = false;
            btn_fromFile.Visible = true;
            btn_OK.Visible = true;
            button3.Visible = true;
            button4.Visible = false;
            textBox1.Enabled = true;
            ((DataGridViewComboBoxColumn)dataGridView_Areas.Columns[3]).DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if(dataGridView_Areas.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()!="最近出口")
                {
                    int test = int.Parse(dataGridView_Areas.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                }
                
            }
            catch
            {
                //dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Red;
                if(MessageBox.Show("所设置的数据非法","提示",MessageBoxButtons.OK,MessageBoxIcon.Error)==DialogResult.None)
                {
                    dataGridView_Areas.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;
                    dataGridView_Areas.BeginEdit(true);
                }
            }
        }

        public bool IsDataLegal()
        {
            bool isDataLegal = true;
            try
            {
                for (int i = 0; i < 6; i++)
                {
                    ////Console.WriteLine("data2:" + i);
                    ////string[] responseTime = Regex.Split(dataGridView2.Rows[i].Cells[2].Value.ToString(), " ", RegexOptions.IgnoreCase);
                    //int temp = int.Parse(dataGridView2.Rows[i].Cells[2].Value.ToString());


                }
                for (int i=0;i<dataGridView_Areas.RowCount;i++)
                {
                    //Console.WriteLine("data1:" + i+"/"+dataGridView1.RowCount + "::"+ dataGridView1.Rows[i].Cells[2].Value.ToString());
                    int temp = int.Parse(dataGridView_Areas.Rows[i].Cells[2].Value.ToString());
                }
            }
            catch
            {
                isDataLegal = false;
                return isDataLegal;
            }
            return isDataLegal;
        }

        public void SaveXmlFile(string path)
        {
            if(File.Exists(path))
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(path);
                XmlElement settings = xmlDocument.DocumentElement;
                XmlNodeList people = settings.GetElementsByTagName("People");
                XmlNodeList selectNearest = settings.GetElementsByTagName("SelectNearest");
                XmlNodeList areas = settings.GetElementsByTagName("Areas");
                XmlNodeList colorDensity = settings.GetElementsByTagName("ColorDensity");

                //1.总人数
                int rowCount = 0;
                foreach (XmlElement element in people)
                {
                    element.SetAttribute("num", textBox1.Text.ToString());
                    element.SetAttribute("distribution", cb_ditribution.SelectedIndex.ToString());
                    foreach (XmlNode node in element)
                    {
                        ((XmlElement)node).SetAttribute("percentage", dataGridView2.Rows[rowCount].Cells[1].Value.ToString());
                        ((XmlElement)node).SetAttribute("speedMin", dataGridView2.Rows[rowCount].Cells[2].Value.ToString());
                        ((XmlElement)node).SetAttribute("speedMax", dataGridView2.Rows[rowCount].Cells[3].Value.ToString());
                        ((XmlElement)node).SetAttribute("responseTimeMin", dataGridView2.Rows[rowCount].Cells[4].Value.ToString());
                        ((XmlElement)node).SetAttribute("responseTimeMax", dataGridView2.Rows[rowCount].Cells[5].Value.ToString());
                        rowCount++;
                    }
                }
                //2. 是否选择最近出口
                ((XmlElement)(selectNearest[0])).SetAttribute("bool", cb_outMode.SelectedIndex == 1 ? "1":"0");

                //3.密度和目标
                rowCount = 0;
                foreach (XmlElement element in areas)
                {
                    foreach (XmlNode node in element)
                    {
                        ((XmlElement)node).SetAttribute("density", dataGridView_Areas.Rows[rowCount].Cells[2].Value.ToString());
                        if(dataGridView_Areas.Rows[rowCount].Cells[3].Value.ToString()=="最近出口")
                        {
                            ((XmlElement)node).SetAttribute("pos1", ("最近出口").ToString());
                        }
                        else
                        {
                            ((XmlElement)node).SetAttribute("pos1", (int.Parse(dataGridView_Areas.Rows[rowCount].Cells[3].Value.ToString()) - 1).ToString());
                        }
                        rowCount++;
                    }
                }

                //4密度颜色设置
                int colorCount=0;
                foreach (XmlElement element in colorDensity)
                {
                    foreach (XmlNode node in element)
                    {
                        ((XmlElement)node).SetAttribute("dens", HeatMap.DensityFloat[colorCount].ToString());
                        colorCount++;
                    }
                }
                xmlDocument.Save(path);
            }
            //没有XML则拷贝
            else
            {
                File.Copy("set.xml", path);
                SaveXmlFile(path);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

   

        private void MessagePanel_Load(object sender, EventArgs e)
        {
            textBox_girdTimeRevIntervel.Text = Sample.girdTimeRevIntervel.ToString();
            if(Sample.isSelectTimeRev_area)
            {
                timeRev_area.Checked = true;
            }
            else
            {
                timeRev_area.Checked = false;
                timeRev_girdCenter.Checked = true;
            }
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            saveAndOK(true);
        }
        private void btn_Save_Click(object sender, EventArgs e)
        {
            saveAndOK(false);
        }

        void saveAndOK(bool isOK)
        {
            //不保存到set中的设置
            if(timeRev_area.Checked)
            {
                Sample.isSelectTimeRev_area = true;
            }
            else
            {
                Sample.isSelectTimeRev_area = false;
            }
            try
            {
                int temp = int.Parse(textBox_girdTimeRevIntervel.Text);
                if(temp==0)
                {
                    Console.WriteLine("时间间隔设置有问题，至少为1秒，请重新设置");
                    return;
                }
                Sample.girdTimeRevIntervel = temp;
            }
            catch
            {
                Console.WriteLine("时间间隔设置有问题，请重新设置");
                return;
            }


            //保存到set中的
            int tempNum = int.Parse(textBox1.Text.ToString());
            if (tempNum > 110000)
            {
                MessageBox.Show("最大人数不能超过11万", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Text = "100000";
                return;
            }

            int percentage = 0;
            try
            {

                for (int i = 0; i < 6; i++)
                {
                    //string[] ratio = Regex.Split(dataGridView2.Rows[i].Cells[1].Value.ToString(), " ", RegexOptions.IgnoreCase);
                    percentage += int.Parse(dataGridView2.Rows[i].Cells[1].Value.ToString());

                    if (int.Parse(dataGridView2.Rows[i].Cells[4].Value.ToString()) > int.Parse(dataGridView2.Rows[i].Cells[5].Value.ToString()))
                    {
                        MessageBox.Show("人员设置有问题,最短响应时间不能大于最长响应时间,请重新修改", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }
            catch
            {
                MessageBox.Show("人员设置输入数据有问题,请重新修改", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (percentage != 100)
            {
                MessageBox.Show("人员设置比例错误", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (IsDataLegal())
            {
                if (Sample.mainDirectory == null)
                {
                    MessageBox.Show("当前未创建工程，请先创建工程", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (densityCheck())
                {

                    SaveXmlFile(Sample.mainDirectory + Sample.projectName);
                    if(isOK)(this.Owner as MainUI).UISimulateInit();
                    this.Close();
                }

            }
            else
            {
                MessageBox.Show("填写数据不合法,请重新修改", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        bool densityCheck()
        {
            float i1, i2, i3, i4, i5;
            try
            {
                i1 = float.Parse(density1.Text);
                i2 = float.Parse(density2.Text);
                i3 = float.Parse(density3.Text);
                i4 = float.Parse(density4.Text);
                i5 = float.Parse(density5.Text);
            }
            catch
            {
                MessageBox.Show("颜色密度设置不是合法数字", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if(i5 >= i4 && i4 >= i3 && i3 >= i2 && i2 >= i1)
            {
                HeatMap.DensityFloat[0] = i1;
                HeatMap.DensityFloat[1] = i2;
                HeatMap.DensityFloat[2] = i3;
                HeatMap.DensityFloat[3] = i4;
                HeatMap.DensityFloat[4] = i5;
                return true;
            }
            else
            {
                MessageBox.Show("颜色密度设置数字不是递增", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            
        }

        

        private void cb_outMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            //就近选择方式，应更新panel显示
            if (cb_outMode.SelectedIndex == 1)
            {
                dataGridView_Areas.Columns[3].Visible = false;
            }
            else
            {
                dataGridView_Areas.Columns[3].Visible = true;
                if (Sample.mainDirectory == null)
                {
                    InitDataView(Sample.mainDirectory + Sample.projectName);
                }
                
            }
        }

        private void btn_fromFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog();
            //默认所有工程都在project文件夹中
            f.InitialDirectory = Directory.GetCurrentDirectory();
            f.Multiselect = false;
            f.Title = "请选择需要导入的配置文件";
            f.Filter = "配置文件(*.sim)|*.sim";
            if (f.ShowDialog() == DialogResult.OK)
            {
                Console.WriteLine(f.FileName);
                if(File.Exists(Sample.mainDirectory + Sample.projectName))
                {
                    File.Delete(Sample.mainDirectory + Sample.projectName);
                }
                File.Copy(f.FileName, Sample.mainDirectory + Sample.projectName);
              
                InitDataView(Sample.mainDirectory + Sample.projectName);
                
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // 如果输入的不是数字键，也不是回车键、Backspace键，则取消该输入
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)13 && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (MessageBox.Show("XXX不知道什么错误", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.None)
            {
               
            }
        }

        //控制只能输入数字
        public DataGridViewTextBoxEditingControl CellEdit = null;
        private void dataGridView2_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            //if ((this.dataGridView1.CurrentCellAddress.X == 1) || (this.dataGridView1.CurrentCellAddress.X == 2))
            {
                CellEdit = (DataGridViewTextBoxEditingControl)e.Control;
                CellEdit.SelectAll();
                CellEdit.KeyPress += Cells_KeyPress; //绑定事件
            }
        }
        private void Cells_KeyPress(object sender, KeyPressEventArgs e) //自定义事件
        {
                if (!(e.KeyChar >= '0' && e.KeyChar <= '9')) e.Handled = true;
                if (e.KeyChar == '\b') e.Handled = false;
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            //{
            //    CellEdit = (DataGridViewTextBoxEditingControl)e.Control;
            //    CellEdit.SelectAll();
            //    CellEdit.KeyPress += Cells_KeyPress; //绑定事件
            //}
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

   
        private void density_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8 && e.KeyChar != '.')//&& density1.Text.Length>4
            {
                e.Handled = true;
            }
        }
        

        private void colorDensity_Enter(object sender, EventArgs e)
        {

        }

        private void density1_TextChanged(object sender, EventArgs e)
        {
            label_density2.Text = density1.Text;
        }

        private void density2_TextChanged(object sender, EventArgs e)
        {
            label_density3.Text = density2.Text;
        }

        private void density3_TextChanged(object sender, EventArgs e)
        {
            label_density4.Text = density3.Text;
        }

        private void density4_TextChanged(object sender, EventArgs e)
        {
            label_density5.Text = density4.Text;
        }

        private void density5_TextChanged(object sender, EventArgs e)
        {
            label_density6.Text = density5.Text;
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void timeRev_girdCenter_CheckedChanged(object sender, EventArgs e)
        {
            if(timeRev_girdCenter.Checked)
            {
                groupBox_timeset.Enabled = true;
                dataGridView_Areas.Columns[4].Visible = false;
            }
            else
            {
                groupBox_timeset.Enabled = false;
                dataGridView_Areas.Columns[4].Visible = true;
            }
        }
    }
}
