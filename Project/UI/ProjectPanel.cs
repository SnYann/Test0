using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Window;
using test.Scripts;

namespace test.UI
{
    public partial class ProjectPanel : Form
    {

        private bool isSaveToAnotherProject = false;
        public ProjectPanel()
        {
            InitializeComponent();
            this.textBox2.Text = Directory.GetCurrentDirectory().ToString();
            this.textBox1.Text = "新建工程1";
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            OK();
        }
        private void OK()
        {
            if (CreateDirectory())
            {
                if (isSaveToAnotherProject)
                {
                    //复制文件
                    try
                    {
                        string srcPath = Sample.mainDirectory;
                        string destPath = textBox2.Text.Trim() + "\\" + textBox1.Text.Trim() + "\\";
                        DirectoryInfo dir = new DirectoryInfo(srcPath);
                        FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //获取目录下（不包含子目录）的文件和子目录
                        foreach (FileSystemInfo i in fileinfo)
                        {
                            if (i is DirectoryInfo)     //判断是否文件夹
                            {
                                //我们不拷贝文件夹 所以去掉
                                //if (!Directory.Exists(destPath + "\\" + i.Name))
                                //{
                                //    Directory.CreateDirectory(destPath + "\\" + i.Name);   //目标目录下不存在此文件夹即创建子文件夹
                                //}
                                //CopyDir(i.FullName, destPath + "\\" + i.Name);    //递归调用复制子文件夹
                            }
                            else
                            {
                                string aLastName = i.Name.Substring(i.Name.LastIndexOf(".") + 1, (i.Name.Length - i.Name.LastIndexOf(".") - 1));   //扩展名
                                if (aLastName == "sim")
                                {
                                    File.Copy(i.FullName, destPath + "\\" + textBox1.Text.Trim() + ".sim", true);      //不是文件夹即复制文件，true表示可以覆盖同名文件
                                }
                                else
                                {
                                    File.Copy(i.FullName, destPath + "\\" + i.Name, true);      //不是文件夹即复制文件，true表示可以覆盖同名文件
                                }

                            }
                        }
                    }
                    catch (Exception ee)
                    {
                        throw;
                    }
                    this.Close();
                    MessageBox.Show("工程另存为成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                else
                {
                    Sample.mainDirectory = textBox2.Text.Trim() + "\\" + textBox1.Text.Trim() + "\\";
                    Sample.projectName = textBox1.Text.Trim() + ".sim";

                    if (!File.Exists("set.xml"))
                    {
                        MessageBox.Show("缺失原始配置文件set.xml", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    (this.Owner as MainUI).ShowMessagePanel("set.xml", 0);//原来的XML文件
                    //if (MessageBox.Show("创建新工程成功,是否立即编辑XML文件", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                    //{
                    //    (this.Owner as newUI).ShowMessagePanel();
                    //}
                    this.Close();
                }
            }
        }

        public bool CreateDirectory()
        {
            if (!Directory.Exists(textBox2.Text.Trim()))
            {
                MessageBox.Show("目录不存在", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (Directory.Exists(textBox2.Text.Trim() + "\\" + textBox1.Text.Trim()))
            {
                MessageBox.Show("工程" + textBox1.Text.Trim() + "已经存在", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            Directory.CreateDirectory(textBox2.Text.Trim() + "\\" + textBox1.Text.Trim());
            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog f = new FolderBrowserDialog();
            f.SelectedPath = (Directory.GetCurrentDirectory().ToString());
            f.Tag = "选择工程目录";
            if(f.ShowDialog()==DialogResult.OK)
            {
                textBox2.Text = f.SelectedPath;
            }
        }

        public void ChangeTitle()
        {
            this.Text = "工程另存为";
            isSaveToAnotherProject = true;
        }


        private void ProjectPanel_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyValue=='\r')
            {
                OK();
            }
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
           
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == '\r')
            {
                OK();
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == '\r')
            {
                OK();
            }
        }
    }
}
