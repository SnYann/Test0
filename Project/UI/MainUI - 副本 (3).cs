using SharpNav;
using SharpNav.IO.Json;
using SharpNav.Pathfinding;
using Simulate;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using test.Scripts;
using test.UI;

namespace Window
{


    public partial class MainUI : Form
    {
        
        //绘图panel的gc
        Graphics gc;
        int stepCounts;//用于判断是否有新的步骤变化

        //线程控制
        bool heatmapThreadControl = false;

        private void newUI_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.CenterScreen; //窗体的位置由Location属性决定
            //this.Location = (Point)new Size(600, 15); //窗体的起始位置为0,0 
        }
        private void newUI_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }

        public MainUI()
        {
            InitializeComponent();
            menuStrip1.Renderer = new MyRenderer();
            HeatMap.newui = this;
            CheckForIllegalCrossThreadCalls = false;

            //采用双缓冲技术的控件必需的设置
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);

            Sample.OpenInit();
            tile = Sample.songnav.tiledNavMesh.GetTileAt(0, 0, 0);
        }


        #region 显示所需要的部分变量与复写的一些函数
        //位置偏移
        private PointF _gridLeftTop = new PointF(820, 436);

        //是否鼠标按下
        private bool _leftButtonPress = false;

        //鼠标位置
        private PointF _mousePosition = new PointF(0, 0);

        //缩放控制
        private float _zoomOld = 1.0f;
        private float _zoom = 0.18f;
        private float _zoomMin = 0.1f;
        private float _zoomMax = 1000f;

        //单元格大小
        private int _cellWidth_px = 30;
        private int _cellHeight_px = 30;

        //是否可初始化
        public bool isInitializeAvailable = false;
        //是否可运行
        public bool isRuningAvailable = false;
        //是否可查看结果
        public bool isPlayingAvailable = false;

        //编辑mesh所用的    
        int AreaIdSelected = 0xfe;//代表没有被选择
        float distance = 0;

        float mouseRealX;
        float mouseRealY;
        private void GetRealMouse()
        {
            mouseRealX = _mousePosition.X / _zoom - _gridLeftTop.X / _zoom;
            mouseRealY = _mousePosition.Y / _zoom - _gridLeftTop.Y / _zoom;
            mouseRealX /= 10;
            mouseRealY /= 10;
        }

        private void doubleBufferPanel1_MouseMove(object sender, MouseEventArgs e)
        {
            var offsetX = e.X - _mousePosition.X;
            var offsetY = e.Y - _mousePosition.Y;
            if (_leftButtonPress)
            {
                _gridLeftTop.X += offsetX;
                _gridLeftTop.Y += offsetY;

                _mousePosition.X = e.X;
                _mousePosition.Y = e.Y;

                this.Refresh();
            }
        }

        private void doubleBufferPanel1_MouseUp(object sender, MouseEventArgs e)
        {
            _leftButtonPress = false;
            this.Cursor = Cursors.Default;
        }

        private void doubleBufferPanel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _mousePosition.X = e.X;
                _mousePosition.Y = e.Y;

                _leftButtonPress = true;
                this.Cursor = Cursors.Hand;

                //设置poly被选中
                var tile = Sample._instance[0].songnav1.tiledNavMesh.GetTileAt(0, 0, 0); ;
                GetRealMouse();

                SearchAgent(mouseRealX, mouseRealY);
                Console.WriteLine(mouseRealX + "," + mouseRealY);

                var v1 = new Vector2(24, 13.8f);
                var v2 = new Vector2(mouseRealX, mouseRealY);
                distance = Simulate.MathHelper.abs(v1 - v2);
                NavPoint np = Sample.songnav.navMeshQuery.FindNearestPoly(new SharpNav.Geometry.Vector3(mouseRealX, 0, mouseRealY), new SharpNav.Geometry.Vector3(1, 1, 1));
                //当前选择的polyid
                if (np.Polygon.Id != 0)
                {
                    int id = Sample.songnav.navMeshQuery.nav.IdManager.DecodePolyIndex(ref np.Polygon);
                    //在选择的多边形不是空的情况下
                    //根据当前功能设置
                    Console.WriteLine(tile.Polys[id].Area.Id);
                    if (rB_AreaPartition.Checked)//分区更改
                    {
                        if (cB_AreasList.Text == "未选择区域")//未选择区域只能选择，不能取消
                        {
                            tile.Polys[id].Area = Area.None;
                            tB_Weight.Text = tile.Polys[id].Area.Id.ToString();
                            tile.Polys[id].Selected = true;
                        }
                        else
                        {
                            if (tile.Polys[id].Area == GetAreabyName(cB_AreasList.Text))
                            {
                                tile.Polys[id].Area = Area.None;
                                tile.Polys[id].Selected = false;
                            }
                            else
                            {
                                tile.Polys[id].Area = GetAreabyName(cB_AreasList.Text);
                                tB_Weight.Text = tile.Polys[id].Area.Id.ToString();
                                tile.Polys[id].Selected = true;
                                label_panelChangeParamenter.Text = tB_Weight.Text;
                            }
                        }

                    }
                    else if (rB_AreaSet.Checked)//区域参数设置
                    {
                        //tile.Polys[id].Selected = true;
                        AreaIdSelected = tile.Polys[id].Area.Id;
                        foreach (var areas in Sample._areas)
                        {
                            foreach (var area in areas)
                            {
                                if (AreaIdSelected == area.Id)
                                {
                                    //显示一些参数到panel
                                    label_panelChangeParamenter.Text = area.AreaName + "\r\n\r\n";
                                    label_panelChangeParamenter.Text += Instance.getAreaAcreage(tile, AreaIdSelected) + "\r\n\r\n";
                                    label_panelChangeParamenter.Text += "人员数量：" + area.headMaxcount + "\r\n\r\n";

                                    int c = 0;
                                    for (int i = 0; i < Sample._areas[2].Count; i++)
                                    {
                                        c += Sample._areas[2][i].headMaxcount;
                                    }
                                    label_panelChangeParamenter.Text += "人员数量：" + c + "\r\n\r\n";
                                    break;
                                }
                            }
                        }



                    }


                }
                else AreaIdSelected = 0xfe;//代表没选择

            }
        }
        private void SearchAgent(float x, float y)
        {
            Vector2 p = new Vector2(x, y);
            for (int i = 0; i < Sample._instance.Count; i++)
            {
                for (int j = 0; j < Sample._instance[i]._agents.Count; j++)
                {
                    if (Simulate.MathHelper.abs(Sample._instance[i]._agents[j].positionNow - p) < 1)
                    {
                        Sample._instance[i]._agents[j].color = Color.Red;
                        agentForNav = Sample._instance[i]._agents[j];
                        return;
                    }
                }
            }
        }
        private void doubleBufferPanel1_MouseWheel(object sender, MouseEventArgs e)
        {
            var delta = e.Delta;
            //if (Math.Abs(delta) < 10)
            //{
            //    return;
            //}
            var mousePosition = new PointF();
            mousePosition.X = e.X;
            mousePosition.Y = e.Y;
            _zoomOld = _zoom;

            if (delta < 0)
            {
                _zoom -= _zoom > 0.6f ? 0.1f : 0.02f; //FetchStep(delta);
            }
            else if (delta > 0)
            {
                _zoom += _zoom > 0.6f ? 0.1f : 0.02f; ;//FetchStep(delta);
            }
            if (_zoom < _zoomMin)
            {
                _zoom = _zoomMin;
            }
            else if (_zoom > _zoomMax)
            {
                _zoom = _zoomMax;
            }

            var zoomNew = _zoom;
            var zoomOld = _zoomOld;
            var deltaZoomNewToOld = zoomNew / zoomOld;

            var zero = _gridLeftTop;
            zero.X = mousePosition.X - (mousePosition.X - zero.X) * deltaZoomNewToOld;
            zero.Y = mousePosition.Y - (mousePosition.Y - zero.Y) * deltaZoomNewToOld;
            //zero.X = mousePosition.X * (1 - _zoom)-zero.X;
            //zero.Y = mousePosition.Y * (1 - _zoom)-zero.Y;
            _gridLeftTop = zero;

            doubleBufferPanel1.Refresh();
        }



        //鼠标点击时候用的，通过名字得到Area
        private Area GetAreabyName(string text)
        {
            for (int i = 0; i < Sample._areas.Count; i++)
            {
                for (int j = 0; j < Sample._areas[i].Count; j++)
                {
                    if (Sample._areas[i][j].AreaName == text)
                    {
                        return Sample._areas[i][j];
                    }
                }

            }
            return Area.None;
        }

        #endregion

        #region 仿真相关
        //仿真需要的
        public List<Instance> _instance;
        NavTile tile;
        Bitmap meshMap;


        internal void UISimulateInit()
        {
            char[] lineSplitChars = { '.' };
            this.Text = "东门步行街人群疏散系统 - " + Sample.projectName.Split(lineSplitChars, StringSplitOptions.RemoveEmptyEntries)[0];

            //清屏
            //if(Sample._instance!=null)
            //{
            //    for(int i=0;i<Sample._instance.Count;i++)
            //    {
            //        Sample._instance[i]._agents.Clear();
            //    }
            //    doubleBufferPanel1.Refresh();
            //}
            Sample.Clear();



            UpdateSimulateStatus(SimulateStates.SimulatInited);
        }


        public void DensityColorReadInit(string path)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(path);
                XmlElement settings = xmlDocument.DocumentElement;
                XmlNodeList colorDensity = settings.GetElementsByTagName("ColorDensity");
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
                            MessageBox.Show("颜色密度对应数据出错,有非浮点数", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                if (HeatMap.DensityFloat.Count < 5)
                {
                    MessageBox.Show("颜色密度对应数据出错", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch
            {
            }

        }

        #endregion

        #region 读取相关

        private int speedMulti = 1;

        internal bool ReadInit()
        {
           

            //可能配置文件出错,可能仿真结果没有
            if (Sample.initRead())
            {
                if(Sample.ExitsOutput())
                {
                    Sample.initRead2();
                    //如果读取文件存在但是位置记录都是空
                    if (Sample.frameCount <= 0)
                    {
                        MessageBox.Show("当前项目仿真结果文件内容为空, 需要先进行仿真", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    _instance = Sample._instance;
                     this.trackBar_replay.Maximum = Sample.frameCount;
                    this.label_FrameMax.Text = Sample.frameCount.ToString();
                    
                    HeatMap.HeatMapInit();//先初始化后计算
                    
                    timerRead.Enabled = true;
                    

                    //rB_Border.Checked = true;

                    panelAll.Enabled = true;
                    UpdateSimulateStatus(SimulateStates.Reading);
                    pictureBox_Play.Image = test.Properties.Resources.video_pause;//暂时放在这，最好后面增加状态play_pause
                    return true;
                }
                else
                {
                    menu_Start.Enabled = true;
                    panelAll.Enabled = true;
                    //rB_Border.Checked = true;
                    播放ToolStripMenuItem.Enabled = false;
                    MessageBox.Show("当前项目没有仿真结果文件, 需要先进行仿真", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                
            }
            else
            {
                MessageBox.Show("读取数据出现问题", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            
        }
        #endregion

        #region 绘制相关
        //panel的绘制
        private void doubleBufferPanel1_Paint(object sender, PaintEventArgs e)
        {
            gc = e.Graphics;
            gc.SmoothingMode = SmoothingMode.HighQuality;
            gc.ScaleTransform(_zoom, _zoom);
            gc.TranslateTransform(_gridLeftTop.X / _zoom, _gridLeftTop.Y / _zoom);

            if (rB_Mesh.Checked) DrawNavMesh();
            
            if (cB_ShowExits.Checked) DrawExits(gc);
            DrawBuildingName(gc);
            if (_instance == null)
                return;

            if (rB_Border.Checked)DrawBoudary();
            //else if (rB_Mesh.Checked) DrawNavMesh();
            else if (rB_AreaPartition.Checked) DrawMeshforPartition();
            else if (rB_AreaSet.Checked) DrawMeshforSet();
            //else if()
            if (cB_ShowGird.Checked) DrawGrid(gc);
            if (rB_corwds.Checked) DrawCrowds(_instance);
            else if (rB_heatmap.Checked) gc.DrawImage(heatmap, -HeatMap.xOffset * 10, -HeatMap.xOffset * 10);//gc.DrawImage(heatmap, -206, -1980); //-trackBar3.Value,-trackBar_repaly.Value);
            //if (cB_ShowExits.Checked) DrawExits(gc);
            //DrawBuildingName(gc);

            //显示颜色密度比例尺
            if (cB_DrawColorRule.Checked) DrawColorRule(gc);
            //for(int r=2;r<heatmap.Length-2;r++)
            //{
            //    for(int l=2;l<heatmap[r].Length-2;l++)
            //    {
            //        gc.FillRectangle(new SolidBrush(heatmap[r][l]), r*10,l*10,10,10);
            //    }
            //}

            drawPoints();
        }

        AgentClass agentForNav;
        private void drawPoints()
        {
            if (agentForNav == null) return;
            for (int n = 0; n < agentForNav.navPoints.Count; n++)
            {
                gc.FillEllipse(Brushes.Red, agentForNav.navPoints[n].x_*10, agentForNav.navPoints[n].y_*10, 20, 20);
            }
        }

        private void DrawBoudary()
        {
            Color color = Color.Red;
            float max = 0;

            for (int j = 0; j < _instance.Count; j++)
            {
                switch (j)
                {
                    case 0:
                        color = Color.Red;
                        break;
                    case 1:
                        color = Color.Green;
                        break;
                    case 2:
                        color = Color.Blue;
                        break;
                    case 3:
                        color = Color.Purple;
                        break;
                }

                Pen pen = new Pen(color, 0.1f);

                for (int i = 0; i < _instance[j].obsTest.Count - 1; i += 2)
                {
                    var v0 = _instance[j].obsTest[i];
                    var v1 = _instance[j].obsTest[i + 1];

                    if (v0.Z > max) max = v0.Z;
                    if (v1.Z > max) max = v1.Z;
                    if (max >= 209)
                    {
                        Console.WriteLine(max);
                    }

                    PointF[] points = new PointF[2];
                    points[0] = new PointF(v0.X * 10, v0.Z * 10);
                    points[1] = new PointF(v1.X * 10, v1.Z * 10);

                    gc.DrawPolygon(pen, points);
                }
            }

        }

        private void DrawNavMesh()
        {

            if (Sample.songnav.tiledNavMesh == null)
                return;
            var tile = Sample.songnav.tiledNavMesh.GetTileAt(0, 0, 0);
            
            Color color = Color.Purple;
            Pen pen = new Pen(Color.FromArgb(212, 212, 214));
            Brush brush = new SolidBrush(Color.FromArgb(212, 212, 214));
            for (int i = 0; i < tile.Polys.Length; i++)
            {
                if (!tile.Polys[i].Area.IsWalkable)
                    continue;
                    

                //Brush brush = new SolidBrush(Color.FromArgb(212, 212, 214));
                for (int j = 2; j < PathfindingCommon.VERTS_PER_POLYGON; j++)
                {
                    //if (color.R < 1) color.set += 0.1f;
                    //if (color.G < 1) color.G += 0.1f;
                    //if (color.R >= 1) color.R = 0.1f;
                    //if (color.G >= 1) color.G = 0.1f;
                    //GL.Color4(color);

                    //Brush brush = new SolidBrush(tile.Polys[i].Selected ? tile.Polys[i].ColorSelected : tile.Polys[i].ColorOriginal);
                    

                    //GL.Color4(tile.Polys[i].Selected ? tile.Polys[i].ColorSelected : tile.Polys[i].ColorOriginal);//
                    if (tile.Polys[i].Verts[j] == 0)
                        break;

                    int vertIndex0 = tile.Polys[i].Verts[0];
                    int vertIndex1 = tile.Polys[i].Verts[j - 1];
                    int vertIndex2 = tile.Polys[i].Verts[j];


                    var v0 = tile.Verts[vertIndex0];

                    var v1 = tile.Verts[vertIndex1];

                    var v2 = tile.Verts[vertIndex2];

                    PointF[] points = new PointF[3];
                    points[0] = new PointF(v0.X * 10, v0.Z * 10);
                    points[1] = new PointF(v1.X * 10, v1.Z * 10);
                    points[2] = new PointF(v2.X * 10, v2.Z * 10);
                    //gc.DrawPolygon(pen, points);
                    gc.FillPolygon(brush, points);
                    gc.DrawPolygon(pen, points);
                }

            }
        }
        private void DrawNavMeshWithColor()
        {
            //Graphics gc = panel1.CreateGraphics();


            if (Sample.songnav.tiledNavMesh == null)
                return;

            var tile = Sample.songnav.tiledNavMesh.GetTileAt(0, 0, 0);


            Color color = Color.Purple;

            //Pen pen = new Pen(color, 1);

            for (int i = 0; i < tile.Polys.Length; i++)
            {
                if (!tile.Polys[i].Area.IsWalkable)
                    continue;

                //PointF[] points = new PointF[tile.Polys[i].VertCount];
                //for(int v=0;v<tile.Polys[i].VertCount;v++)
                //{
                //    var vert = tile.Verts[v];
                //    points[v]=new PointF(vert.X,vert.Z);
                //}
                //gc.DrawPolygon(pen,points);

                for (int j = 2; j < PathfindingCommon.VERTS_PER_POLYGON; j++)
                {
                    //if (color.R < 1) color.set += 0.1f;
                    //if (color.G < 1) color.G += 0.1f;
                    //if (color.R >= 1) color.R = 0.1f;
                    //if (color.G >= 1) color.G = 0.1f;
                    //GL.Color4(color);

                    Brush brush = new SolidBrush(tile.Polys[i].Selected ? tile.Polys[i].ColorSelected : tile.Polys[i].ColorOriginal);

                    //GL.Color4(tile.Polys[i].Selected ? tile.Polys[i].ColorSelected : tile.Polys[i].ColorOriginal);//
                    if (tile.Polys[i].Verts[j] == 0)
                        break;

                    int vertIndex0 = tile.Polys[i].Verts[0];
                    int vertIndex1 = tile.Polys[i].Verts[j - 1];
                    int vertIndex2 = tile.Polys[i].Verts[j];


                    var v0 = tile.Verts[vertIndex0];

                    var v1 = tile.Verts[vertIndex1];

                    var v2 = tile.Verts[vertIndex2];

                    PointF[] points = new PointF[3];
                    points[0] = new PointF(v0.X * 10, v0.Z * 10);
                    points[1] = new PointF(v1.X * 10, v1.Z * 10);
                    points[2] = new PointF(v2.X * 10, v2.Z * 10);
                    //gc.DrawPolygon(pen, points);
                    gc.FillPolygon(brush, points);
                }

            }

            //neighbor edges
            //for (int i = 0; i < tile.Polys.Length; i++)
            //{
            //    for (int j = 0; j < PathfindingCommon.VERTS_PER_POLYGON; j++)
            //    {
            //        if (tile.Polys[i].Verts[j] == 0)
            //            break;
            //        if (PolyMesh.IsBoundaryEdge(tile.Polys[i].Neis[j]))
            //            continue;

            //        int nj = (j + 1 >= PathfindingCommon.VERTS_PER_POLYGON || tile.Polys[i].Verts[j + 1] == 0) ? 0 : j + 1;

            //        int vertIndex0 = tile.Polys[i].Verts[j];
            //        int vertIndex1 = tile.Polys[i].Verts[nj];

            //        var v = tile.Verts[vertIndex0];
            //        GL.Vertex3(v.X, v.Y, v.Z);

            //        v = tile.Verts[vertIndex1];
            //        GL.Vertex3(v.X, v.Y, v.Z);
            //    }
            //}
            //GL.End();

            ////boundary edges
            //GL.Color4(Color4.Yellow);
            //GL.LineWidth(4f);
            //GL.Begin(BeginMode.Lines);
            //for (int i = 0; i < tile.Polys.Length; i++)
            //{
            //    for (int j = 0; j < PathfindingCommon.VERTS_PER_POLYGON; j++)
            //    {
            //        if (tile.Polys[i].Verts[j] == 0)
            //            break;

            //        if (PolyMesh.IsInteriorEdge(tile.Polys[i].Neis[j]))
            //            continue;

            //        int nj = (j + 1 >= PathfindingCommon.VERTS_PER_POLYGON || tile.Polys[i].Verts[j + 1] == 0) ? 0 : j + 1;

            //        int vertIndex0 = tile.Polys[i].Verts[j];
            //        int vertIndex1 = tile.Polys[i].Verts[nj];

            //        var v = tile.Verts[vertIndex0];
            //        GL.Vertex3(v.X, v.Y, v.Z);

            //        v = tile.Verts[vertIndex1];
            //        GL.Vertex3(v.X, v.Y, v.Z);
            //    }
            //}

            //pen.Dispose();
        }

        private void DrawMeshforPartition()
        {
            //没有被选择的区域呈现红色
            //当前被选择的区域呈现
            if (Sample.songnav.tiledNavMesh == null)
                return;

            var tile = Sample.songnav.tiledNavMesh.GetTileAt(0, 0, 0);

            Color color = Color.Purple;

            Pen pen = new Pen(color, 1);

            for (int i = 0; i < tile.Polys.Length; i++)
            {
                if (!tile.Polys[i].Area.IsWalkable)
                    continue;

                for (int j = 2; j < PathfindingCommon.VERTS_PER_POLYGON; j++)
                {
                    //if (color.R < 1) color.set += 0.1f;
                    //if (color.G < 1) color.G += 0.1f;
                    //if (color.R >= 1) color.R = 0.1f;
                    //if (color.G >= 1) color.G = 0.1f;
                    //GL.Color4(color);


                    if (tile.Polys[i].Area.Id == Area.None)
                    {
                        color = Color.FromArgb(150, tile.Polys[i].ColorOriginal.R, tile.Polys[i].ColorOriginal.G, tile.Polys[i].ColorOriginal.B);
                    }
                    else
                    {
                        //不同区域不同颜色
                        switch (tile.Polys[i].Area.Id)
                        {
                            case 0: color = Color.Purple; break;
                            case 1: color = Color.Salmon; break;
                            case 2: color = Color.Chocolate; break;
                            case 3: color = Color.Red; break;
                            case 4: color = Color.MidnightBlue; break;
                            case 5: color = Color.SlateBlue; break;
                            case 6: color = Color.DodgerBlue; break;
                            case 7: color = Color.SkyBlue; break;
                            case 8: color = Color.Cyan; break;
                            case 9: color = Color.DarkGreen; break;
                            case 10: color = Color.MediumSeaGreen; break;
                            case 11: color = Color.PaleGreen; break;
                            case 12: color = Color.Gold; break;
                            case 13: color = Color.IndianRed; break;
                            case 14: color = Color.Salmon; break;
                            case 15: color = Color.Orange; break;
                            case 16: color = Color.HotPink; break;
                            case 17: color = Color.Maroon; break;
                            case 18: color = Color.DarkViolet; break;
                            case 19: color = Color.Purple; break;
                            case 20: color = Color.GreenYellow; break;
                            case 21: color = Color.RosyBrown; break;
                            case 22: color = Color.Wheat; break;
                            case 23: color = Color.Azure; break;
                            case 24: color = Color.OldLace; break;
                            case 0xfe: color = Color.Green; break;

                            default:
                                color = Color.Black;
                                break;
                                
                        }
                        color = Color.FromArgb(100,color.R,color.G,color.B);
                    }

                        Brush brush = new SolidBrush(tile.Polys[i].Selected ? tile.Polys[i].ColorSelected : color);
                    

                    //GL.Color4(tile.Polys[i].Selected ? tile.Polys[i].ColorSelected : tile.Polys[i].ColorOriginal);//
                    if (tile.Polys[i].Verts[j] == 0)
                        break;

                    int vertIndex0 = tile.Polys[i].Verts[0];
                    int vertIndex1 = tile.Polys[i].Verts[j - 1];
                    int vertIndex2 = tile.Polys[i].Verts[j];


                    var v0 = tile.Verts[vertIndex0];

                    var v1 = tile.Verts[vertIndex1];

                    var v2 = tile.Verts[vertIndex2];

                    PointF[] points = new PointF[3];
                    points[0] = new PointF(v0.X * 10, v0.Z * 10);
                    points[1] = new PointF(v1.X * 10, v1.Z * 10);
                    points[2] = new PointF(v2.X * 10, v2.Z * 10);
                    //gc.DrawPolygon(pen, points);
                    gc.FillPolygon(brush, points);
                }
            }
        }

        private void DrawMeshforSet()
        {
            //没有被选择的区域呈现红色
            //当前被选择的区域呈现
            if (Sample.songnav.tiledNavMesh == null)
                return;

            var tile = Sample.songnav.tiledNavMesh.GetTileAt(0, 0, 0);

            Color color = Color.Purple;

            //Pen pen = new Pen(color, 1);

            for (int i = 0; i < tile.Polys.Length; i++)
            {
                if (!tile.Polys[i].Area.IsWalkable)
                    continue;
                //Brush brush = new SolidBrush(tile.Polys[i].Selected ? tile.Polys[i].ColorSelected : tile.Polys[i].ColorOriginal);

                //不同区域不同颜色
                switch (tile.Polys[i].Area.Id)
                {
                    case 0: color = Color.Purple; break;
                    case 1: color = Color.Salmon; break;
                    case 2: color = Color.Chocolate; break;
                    case 3: color = Color.Red; break;
                    case 4: color = Color.MidnightBlue; break;
                    case 5: color = Color.SlateBlue; break;
                    case 6: color = Color.DodgerBlue; break;
                    case 7: color = Color.SkyBlue; break;
                    case 8: color = Color.Cyan; break;
                    case 9: color = Color.DarkGreen; break;
                    case 10: color = Color.MediumSeaGreen; break;
                    case 11: color = Color.PaleGreen; break;
                    case 12: color = Color.Gold; break;
                    case 13: color = Color.IndianRed; break;
                    case 14: color = Color.Salmon; break;
                    case 15: color = Color.Orange; break;
                    case 16: color = Color.HotPink; break;
                    case 17: color = Color.Maroon; break;
                    case 18: color = Color.DarkViolet; break;
                    case 19: color = Color.Purple; break;
                    case 20: color = Color.GreenYellow; break;
                    case 21: color = Color.RosyBrown; break;
                    case 22: color = Color.Wheat; break;
                    case 23: color = Color.Azure; break;
                    case 24: color = Color.OldLace; break;
                    case 0xfe: color = Color.Green; break;

                    default:
                        color = Color.Black;
                        break;
                }

                if (tile.Polys[i].Area.Id != AreaIdSelected) color = Color.FromArgb(60, color.R, color.G, color.B);
                Brush brush = new SolidBrush(color);

                for (int j = 2; j < PathfindingCommon.VERTS_PER_POLYGON; j++)
                {
                    if (tile.Polys[i].Verts[j] == 0)
                        break;
                    int vertIndex0 = tile.Polys[i].Verts[0];
                    int vertIndex1 = tile.Polys[i].Verts[j - 1];
                    int vertIndex2 = tile.Polys[i].Verts[j];

                    var v0 = tile.Verts[vertIndex0];
                    var v1 = tile.Verts[vertIndex1];
                    var v2 = tile.Verts[vertIndex2];

                    PointF[] points = new PointF[3];
                    points[0] = new PointF(v0.X * 10, v0.Z * 10);
                    points[1] = new PointF(v1.X * 10, v1.Z * 10);
                    points[2] = new PointF(v2.X * 10, v2.Z * 10);
                    gc.FillPolygon(brush, points);
                }
            }
        }

        public void DrawCrowds(List<Instance> _instance)
        {
            for (int i = 0; i < _instance.Count; i++)
            {
                DrawCrowd(_instance[i]);
            }
        }

        public void DrawCrowd(Instance instance)
        {
            Pen pen = new Pen(Color.Blue, 0.1f);
            for (int i = 0; i < instance._agents.Count; i++)
            {
                ////song 画导航点
                //GL.Begin(BeginMode.Points);
                //GL.Color4(Color4.Green);
                ////GL.Color4(new Color4((float)(Simulate.MathHelper.random.NextDouble()), (float)(Simulate.MathHelper.random.NextDouble()), (float)(Simulate.MathHelper.random.NextDouble()),0.5f));
                //for (int j = 0; j < _instance._agents[i].navPoints.Count - 1; j++)
                //{
                //    SharpNav.Geometry.Vector3 p = new SharpNav.Geometry.Vector3(_instance._agents[i].navPoints[j].x_, 1f, _instance._agents[i].navPoints[j].y_);
                //    GL.Vertex3(p.X, p.Y, p.Z);
                //}
                //GL.End();

                ////画导航线 song
                //try
                //{
                //    int c = instance._agents[i].navPoints.Count - 1;
                //    //if (c > 2000)
                //    {
                //        //GL.Color4(new Color4((float)(Simulate.MathHelper.random.NextDouble()), (float)(Simulate.MathHelper.random.NextDouble()), (float)(Simulate.MathHelper.random.NextDouble()), 0.5f));
                //        for (int j = 1; j < c; j++)
                //        {
                //            SharpNav.Geometry.Vector3 p1 = new SharpNav.Geometry.Vector3(instance._agents[i].navPoints[j].x_, 1f, instance._agents[i].navPoints[j].y_);
                            
                //            SharpNav.Geometry.Vector3 p2 = new SharpNav.Geometry.Vector3(instance._agents[i].navPoints[j - 1].x_, 1f, instance._agents[i].navPoints[j - 1].y_);
                            
                //            gc.DrawLine(pen, p1.X * 10, p1.Z * 10, p2.X * 10, p2.Z * 10);

                //        }
                //    }

                //}
                //catch
                //{
                //}

                if (cB_showGoalNow.Checked)
                {
                    //画当前目标线 song
                    try
                    {
                        if (instance._agents[i].navPoints.Count > 1)
                        {
                            //SharpNav.Geometry.Vector3 p1;
                            //SharpNav.Geometry.Vector3 p2;
                            //p1 = new SharpNav.Geometry.Vector3(instance._agents[i].positionNow.x_, 1.01f, instance._agents[i].positionNow.y_);

                            //p2 = new SharpNav.Geometry.Vector3(instance._agents[i].navPoints[0].x_, 1.01f, instance._agents[i].navPoints[0].y_);


                            gc.DrawLine(pen, instance._agents[i].positionNow.x_ * 10, instance._agents[i].positionNow.y_ * 10, instance._agents[i].navPoints[0].x_ * 10, instance._agents[i].navPoints[0].y_ * 10);
                            

                            //float pp = p1.X - p2.X + p1.Z - p2.Z;
                            //if (pp > 20)
                            //{
                            //    Console.WriteLine(p2);
                            //}
                        }
                    }
                    catch
                    {

                    }
                }


            }

            Brush brush = new SolidBrush(Color.Red);//填充的颜色

            //if (agentCylinder != null)
            {
                lock (instance._agents)
                {

                    for (int i = 0; i < instance._agents.Count; i++)
                    {
                        try
                        {
                            //if(Sample.readMode)brush = new SolidBrush(instance._agents[i].color);//暂时注释,不用更新区域颜色,都是绿色就可以

                            ///////////源于颜色设置
                            ///方法1，根据本来颜色，如果运行了heatmap就是根据密度设置的来操作
                            //brush = new SolidBrush(instance._agents[i].color);
                            ///方法2，
                            //if (instance._agents[i].state == AgentStates.Evacuating)
                            //    brush = new SolidBrush(Color.Red);
                            //else
                            //    brush = new SolidBrush(Color.Green);
                            ///方法3
                            if (instance._agents[i].state == AgentStates.Evacuating)
                                brush = new SolidBrush(instance._agents[i].color);
                            else
                                brush = new SolidBrush(Color.Black);

                            //if (instance._agents[i].haveReplaned) brush = new SolidBrush(Color.Black);

                            SharpNav.Geometry.Vector3 p = new SharpNav.Geometry.Vector3(instance._agents[i].positionNow.X(), 0, instance._agents[i].positionNow.Y());
                            //agentCylinder.Draw(new OpenTK.Vector3(p.X, p.Y, p.Z), instance._agents[i].color);
                            gc.FillEllipse(brush, p.X * 10, p.Z * 10, 6f, 6f);
                            //#if outfile    
                            //                        FileHelper.Write(Program._agents[i].positionNow.x_ + " " + Program._agents[i].positionNow.y_ + " ");
                            //#endif
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("多线程对_agents.count访问出错 - 绘制");
                            Console.WriteLine(e);
                        }

                    }
                }
            }
        }

        private void DrawGrid(Graphics g)
        {
            float cellWidth = _cellWidth_px;
            float cellHeight = _cellHeight_px;

            //单元格的宽和高最小为1像素
            cellWidth = cellWidth < 1 ? 1 : cellWidth;
            cellHeight = cellHeight < 1 ? 1 : cellHeight;


            int rowCount = 100;
            int columnCount = 100;
            var gridHeight = rowCount * cellHeight;
            var gridWidth = columnCount * cellWidth;

            Pen pen = new Pen(Color.Red, 1f);
            var p1 = new PointF();
            var p2 = new PointF();




            //绘制横线
            for (int r = 0; r <= rowCount; r++)
            {
                p1.X = -gridWidth / 2;
                p1.Y = r * cellHeight - gridWidth / 2;

                p2.X = p1.X + gridWidth;
                p2.Y = p1.Y;

                g.DrawLine(pen, p1, p2);
            }

            //绘制竖线
            for (int c = 0; c <= columnCount; c++)
            {
                p1.X = c * cellHeight - gridHeight / 2;
                p1.Y = -gridHeight / 2;

                p2.X = p1.X;
                p2.Y = p1.Y + gridHeight;

                g.DrawLine(pen, p1, p2);
            }

            g.DrawLine(pen, -2400,2000,-2400,-2000);
            g.DrawLine(pen, 2000, 2000, 2000, -2000);


            //绘制比例
            p1.X = 0;
            p1.Y = 0;

            g.DrawString($"{_zoom * 100}%", SystemFonts.DefaultFont, Brushes.Gray, p1);
            pen = new Pen(Color.Red, 20f);
            p1.X = gridWidth / 2;
            p1.Y = gridHeight / 2;

            g.DrawString(gridWidth + " x " + gridHeight, SystemFonts.DefaultFont, Brushes.Red, p1);
        }

        //地图上绘制数据信息
        private void DrawExits(Graphics g)
        {
            var p1 = new PointF();
            Font font = new Font(labelInfo.Font.FontFamily, 60, FontStyle.Bold);
            for (int i = 0; i < Instance._out.Length; i++)
            {
                p1.X = Instance._out[i].X * 10;
                p1.Y = Instance._out[i].Z * 10;
                //g.FillEllipse(Brushes.Green, p1.X, p1.Y, 10, 10);
                //p1.X += 10;

                ////数字位置细节调整
                if (i == 5 )
                {
                    p1.Y += 30;
                }
                else if(i == 6)
                {
                    p1.Y += 50;
                }
                else if (i == 1)
                {
                    p1.Y -= 80;
                }

                p1.X -= 30;
                p1.Y -= 30;

                if (i < 9)
                    g.FillEllipse(Brushes.DarkOrange, p1.X - 30, p1.Y - 20, 120, 120);
                else
                    g.FillEllipse(Brushes.DarkOrange, p1.X - 5, p1.Y - 20, 120, 120);
                g.DrawString((i + 1).ToString(), font, Brushes.White, p1);
                //g.DrawString(i + "人数: " + Instance._outAgentCount[i] + " " + Instance._outAgentCount[i] * 100 / (Sample.numsPeople - agentsOutCount) + "% " + Instance._outAgentCount[i] * 100 / Sample.numsPeople + "%", font, Brushes.Red, p1);
            }
        }

        private void DrawBuildingName(Graphics g)
        {
            //var p1 = new PointF();
            Font font = new Font(labelInfo.Font.FontFamily, 60, FontStyle.Bold);
            Brush brush = Brushes.LightGray;
            g.DrawString("太阳广场", font, brush, -1000, 310);
            g.DrawString("大世界商城", font, brush, 1050, 550);
            g.DrawString("天龙商业城", font, brush, 410, -1500);
            g.DrawString("宝华楼", font, brush, -700, -630);
            g.DrawString("东门鞋城", font, brush, 350, 420);
            g.DrawString("金世界", font, brush, -1950, 420);
            g.DrawString("新白马中心城", font, brush, 420, -380);

        }

        private Bitmap getNavMeshBmp()
        {
            //Graphics gc = panel1.CreateGraphics();

            Bitmap b = new Bitmap(1000, 1000);
            Graphics bmpG = Graphics.FromImage(b);//图像画布添加绘图

            if (Sample.songnav.tiledNavMesh == null)
                return null;

            var tile = Sample.songnav.tiledNavMesh.GetTileAt(0, 0, 0);


            Color color = Color.Purple;

            Pen pen = new Pen(color, 1);

            for (int i = 0; i < tile.Polys.Length; i++)
            {
                if (!tile.Polys[i].Area.IsWalkable)
                    continue;

                //PointF[] points = new PointF[tile.Polys[i].VertCount];
                //for(int v=0;v<tile.Polys[i].VertCount;v++)
                //{
                //    var vert = tile.Verts[v];
                //    points[v]=new PointF(vert.X,vert.Z);
                //}
                //gc.DrawPolygon(pen,points);

                for (int j = 2; j < PathfindingCommon.VERTS_PER_POLYGON; j++)
                {
                    //if (color.R < 1) color.set += 0.1f;
                    //if (color.G < 1) color.G += 0.1f;
                    //if (color.R >= 1) color.R = 0.1f;
                    //if (color.G >= 1) color.G = 0.1f;
                    //GL.Color4(color);


                    //GL.Color4(tile.Polys[i].Selected ? tile.Polys[i].ColorSelected : tile.Polys[i].ColorOriginal);//
                    if (tile.Polys[i].Verts[j] == 0)
                        break;

                    int vertIndex0 = tile.Polys[i].Verts[0];
                    int vertIndex1 = tile.Polys[i].Verts[j - 1];
                    int vertIndex2 = tile.Polys[i].Verts[j];


                    var v0 = tile.Verts[vertIndex0];

                    var v1 = tile.Verts[vertIndex1];

                    var v2 = tile.Verts[vertIndex2];

                    PointF[] points = new PointF[3];
                    points[0] = new PointF(v0.X * 10, v0.Z * 10);
                    points[1] = new PointF(v1.X * 10, v1.Z * 10);
                    points[2] = new PointF(v2.X * 10, v2.Z * 10);
                    bmpG.DrawPolygon(pen, points);
                }

            }
            //for (int i = 0; i < tile.Polys.Length; i++)
            //{
            //    for (int j = 0; j < PathfindingCommon.VERTS_PER_POLYGON; j++)
            //    {
            //        if (tile.Polys[i].Verts[j] == 0)
            //            break;
            //        if (PolyMesh.IsBoundaryEdge(tile.Polys[i].Neis[j]))
            //            continue;

            //        int nj = (j + 1 >= PathfindingCommon.VERTS_PER_POLYGON || tile.Polys[i].Verts[j + 1] == 0) ? 0 : j + 1;

            //        int vertIndex0 = tile.Polys[i].Verts[j];
            //        int vertIndex1 = tile.Polys[i].Verts[nj];

            //        var v = tile.Verts[vertIndex0];
            //        GL.Vertex3(v.X, v.Y, v.Z);

            //        v = tile.Verts[vertIndex1];
            //        GL.Vertex3(v.X, v.Y, v.Z);
            //    }
            //}
            //GL.End();

            ////boundary edges
            //GL.Color4(Color4.Yellow);
            //GL.LineWidth(4f);
            //GL.Begin(BeginMode.Lines);
            //for (int i = 0; i < tile.Polys.Length; i++)
            //{
            //    for (int j = 0; j < PathfindingCommon.VERTS_PER_POLYGON; j++)
            //    {
            //        if (tile.Polys[i].Verts[j] == 0)
            //            break;

            //        if (PolyMesh.IsInteriorEdge(tile.Polys[i].Neis[j]))
            //            continue;

            //        int nj = (j + 1 >= PathfindingCommon.VERTS_PER_POLYGON || tile.Polys[i].Verts[j + 1] == 0) ? 0 : j + 1;

            //        int vertIndex0 = tile.Polys[i].Verts[j];
            //        int vertIndex1 = tile.Polys[i].Verts[nj];

            //        var v = tile.Verts[vertIndex0];
            //        GL.Vertex3(v.X, v.Y, v.Z);

            //        v = tile.Verts[vertIndex1];
            //        GL.Vertex3(v.X, v.Y, v.Z);
            //    }
            //}

            pen.Dispose();
            bmpG.Dispose();

            return b;
        }

        private void DrawColorRule(Graphics g)
        {

            int x = 2000;
            int y = -2000;

            //DrawRect(g,0.44f, ref x, ref y);
            //DrawRect(g, 1.32f, ref x, ref y);
            //DrawRect(g, 2.2f, ref x, ref y);
            //DrawRect(g, 2.64f, ref x, ref y);
            //DrawRect(g, 3.52f, ref x, ref y);
            //DrawRect(g, 4f, ref x, ref y);

            DrawRect(g, HeatMap.DensityFloat[0], ref x, ref y);
            DrawRect(g, HeatMap.DensityFloat[1], ref x, ref y);
            DrawRect(g, HeatMap.DensityFloat[2], ref x, ref y);
            DrawRect(g, HeatMap.DensityFloat[3], ref x, ref y);
            DrawRect(g, HeatMap.DensityFloat[4], ref x, ref y);
            //最后一个单独弄
            {
                Font font = new Font(labelInfo.Font.FontFamily, 50, FontStyle.Bold);
                Color c = HeatMap.GetColor(HeatMap.DensityFloat[4] + 0.02f, 1);
                Brush b = new SolidBrush(c);
                g.FillRectangle(b, x, y, 100, 80);
                g.DrawString(">" + HeatMap.DensityFloat[4].ToString("0.00"), font, Brushes.White, x + 100, y);
                y += 90;

                //最后一行加个单位
                g.DrawString("人/平米", font, Brushes.White, x - 10, y + 50);
            }
            


            //for (int i = 0; i < HeatMap.maxNum; i += 2)
            //{
            //    Color color = HeatMap.GetColor(i, 1, HeatMap.maxNum);
            //    Brush b = new SolidBrush(color);
            //    g.FillRectangle(b, x, y, 60, 60);
            //    //if(i%2==0)gc.DrawString(i.ToString(), font, brush, 160, y);
            //    if (i == 44) g.DrawString(">" + ((float)i / 54).ToString("0.00"), font, brush, x + 60, y);
            //    else
            //    {
            //        if (i % 4 == 0) g.DrawString(((float)i / 54).ToString("0.00"), font, brush, x + 60, y);
            //    }
            //    y += 50;
            //}
            //g.DrawString("人/平米", font, brush, x - 10, y + 50);
        }
        private void DrawRect(Graphics g, float value, ref int x, ref int y)
        {
            Font font = new Font(labelInfo.Font.FontFamily, 50, FontStyle.Bold);
            Color c =HeatMap.GetColor(value, 1);
            Brush b = new SolidBrush(c);
            g.FillRectangle(b, x, y, 100, 80);
            g.DrawString("<" + value.ToString("0.00"), font, Brushes.White, x + 100, y );
            y+= 90;
        }
        #endregion
        
        #region 热力图相关 暂时没有用
        Bitmap heatmap = new Bitmap(1, 1);
        public int mapCount = 0;
        private void CalculateMap()
        {
            //while (true)
            //{
            //    if (heatmapThreadControl)
            //    {
            //        List<List<AgentClass>> _agents = new List<List<AgentClass>>();
            //        for (int i = 0; i < _instance.Count; i++)
            //        {
            //            _agents.Add(_instance[i]._agents);
            //        }
            //        heatmap = HeatMap.CalculateMap(ref _agents);
            //        mapCount++;
            //    }
            //}
        }
        #endregion

        

           
        #region 顶部菜单相关
        public class MyRenderer : ToolStripProfessionalRenderer
        {
            public MyRenderer() : base(new MyColors()) { }
        }


        private void 新建工程ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Sample.mainDirectory == null)
            {
                ProjectPanel projectPanel = new ProjectPanel();
                projectPanel.Show(this);
            }
            else
            {
                if (Sample.stepscontrolThread != null)
                {
                    //先将所有的仿真暂停
                    if (Sample.stepscontrolThread.IsAlive) Sample.stepscontrolThread.Suspend();
                    if (MessageBox.Show("确定要关闭当前工程吗?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                    {
                        if (Sample.stepscontrolThread.IsAlive) Sample.stepscontrolThread.Resume();//防止出错
                        StopSimulate();
                        ProjectPanel projectPanel = new ProjectPanel();
                        projectPanel.Show(this);
                    }
                    else
                    {
                        if (Sample.stepscontrolThread.IsAlive) Sample.stepscontrolThread.Resume();
                    }
                }
                else //上次正在读取文件
                {
                    timerRead.Enabled = false;
                    Sample.Clear();
                    ProjectPanel projectPanel = new ProjectPanel();
                    projectPanel.Show(this);
                }

            }


        }
    
        
        public void ShowMessagePanel(string filename, int command)
        {
            SettingPanel messagePanel = new SettingPanel(command, filename);

            messagePanel.Show(this);

        }
        
        private void 编辑工程设置ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (Sample.mainDirectory == null)
            {
                MessageBox.Show("没有创建工程，不存在工程文件，请先创建工程", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!File.Exists(Sample.mainDirectory + Sample.projectName))
            {
                MessageBox.Show("当前工程还没有工程文件，请先新建工程文件", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            ShowMessagePanel(Sample.mainDirectory + Sample.projectName, 2);
        }


        private void timer_Progress_Tick(object sender, EventArgs e)
        {
            int count = 0;
            for (int i = 0; i < Sample._instance.Count; i++)
            {
                count += Sample._instance[i].pathFindedNums;
            }
            progressBar1.Value = count;

            if (Sample.stepscontrolThread != null && Sample.stepscontrolThread.IsAlive)
            {
                //已经结束初始化

                timer_Progress.Enabled = false;
                //tile = Sample.songnav.tiledNavMesh.GetTileAt(0, 0, 0);
                progressBar1.Visible = false;
                labelInfo.Text = "初始化成功……";
                labelPanelInfo.Text = "初始化成功……";

                HeatMap.HeatMapInit();//先初始化后计算, 仿真的时候是否计算颜色
                timerSimulate.Enabled = true;
                timerforHeatmap.Enabled = true;
                panelAll.Enabled = true;
                //rB_Border.Checked = true;
                menu_Start.Enabled = true;
                //
                UpdateSimulateStatus(SimulateStates.Simulating);

                foreach (var i in Sample._instance)
                {
                    i.Intervals();
                }
            }
        }

        private void 打开工程ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog();
            //默认所有工程都在project文件夹中
            f.InitialDirectory = Directory.GetCurrentDirectory();
            f.Multiselect = false;
            f.Title = "请选择工程文件";
            f.Filter = "工程文件(*.sim)|*.sim";
            if (f.ShowDialog() == DialogResult.OK)
            {
                Sample.Clear();
                Sample.mainDirectory = System.IO.Path.GetDirectoryName(f.FileName) + "\\";
                Sample.projectName = System.IO.Path.GetFileName(f.FileName);
                this.Text = "东门步行街人群疏散系统 - " + System.IO.Path.GetFileNameWithoutExtension(f.FileName); ;
                //控制区读取
                DensityColorReadInit(Sample.mainDirectory + Sample.projectName);
                ReadInit();
            }


            //FolderBrowserDialog f = new FolderBrowserDialog();
            //f.SelectedPath = Directory.GetCurrentDirectory() + "\\project";
            //if (f.ShowDialog() == DialogResult.OK)
            //{
            //    if (File.Exists(f.SelectedPath + "" +))
            //    {
            //        Sample.Clear();
            //        Sample.mainDirectory = f.SelectedPath;
            //        //控制区读取
            //        ReadInit();
            //    }
            //    else
            //    {
            //        MessageBox.Show("错误，这不是一个工程", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}
        }

        private void 编辑ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (Sample.mainDirectory == null)
            {
                MessageBox.Show("没有创建工程，请先创建工程", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (state == SimulateStates.Reading)
            {

            }
            else if (state == SimulateStates.Simulating)
            {
                if (Sample.stepscontrolThread != null)
                {
                    MessageBox.Show("另存为文件需要先结束仿真", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            ProjectPanel p = new ProjectPanel();
            p.ChangeTitle();
            p.Show(this);
        }

        private void 停止ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //先将所有的仿真暂停
            if (Sample.stepscontrolThread.IsAlive) Sample.stepscontrolThread.Suspend();
            if (MessageBox.Show("确定要停止此次仿真吗?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
            {
                Sample.stepscontrolThread.Resume();//防止出错
                menu_Start.Text = "仿真";
                StopSimulate();
            }
            else
            {
                Sample.stepscontrolThread.Resume();
            }
        }

        private void menu_Start_Click(object sender, EventArgs e)
        {
            //如果
            if (state == SimulateStates.SimulatInited || state == SimulateStates.Reading || state == SimulateStates.ReadingEnd || state == SimulateStates.SimulateEnd) //显示
            {
                //先判断有没有相应的仿真结果文件,如果有就提示, 如果没有就仿真
                if (Sample.ExitsOutput())
                {
                    if (MessageBox.Show("有仿真文件存在, 确定要替换掉他们吗?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.Cancel)
                    {
                        return;
                    }
                }
                labelInfo.Text = "开始初始化……";
                labelPanelInfo.Text = "开始初始化,请稍等……";



                //如果reader存在，就把reader关掉
                for (int i = 0; i < Sample._instance.Count; i++)
                {
                    if (Sample._instance[i].reader != null) Sample._instance[i].reader.Close();
                }

                Sample.SimulateInit();//初始化人,并且寻径

                timer_Progress.Enabled = true;
                progressBar1.Visible = true;
                progressBar1.Maximum = Sample.numsPeople;

                Sample.SimulatePathFind();

                _instance = Sample._instance;

                //更新这个为simulatInited状态，防止 播放一半-> 重新仿真 -》出错，出现两个仿真结束
                UpdateSimulateStatus(SimulateStates.SimulatInited);
                //先将开始按钮变灰，等初始化完毕再enable
                menu_Start.Enabled = false;
                停止ToolStripMenuItem.Enabled = false;
                播放ToolStripMenuItem.Enabled = false;
                //UpdateSimulateStatus(SimulateStates.Simulating);
            }
            else if (state == SimulateStates.Simulating)
            {
                if (Sample.stepscontrolThread.IsAlive) Sample.stepscontrolThread.Suspend();
                UpdateSimulateStatus(SimulateStates.SimulatPaused);
            }
            else if (state == SimulateStates.SimulatPaused)
            {
                if (Sample.stepscontrolThread.IsAlive) Sample.stepscontrolThread.Resume();
                UpdateSimulateStatus(SimulateStates.Simulating);
            }
        }

        private void 播放ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (state == SimulateStates.SimulateEnd)
            {
                Sample.Clear();
                //Sample.mainDirectory = f.SelectedPath;
                //控制区读取
                if(ReadInit())UpdateSimulateStatus(SimulateStates.Reading);
            }
        }

        public class MyColors : ProfessionalColorTable
        {
            public override Color MenuItemSelected
            {
                get { return Color.FromArgb(90, 194, 231); }
            }
            public override Color MenuItemSelectedGradientBegin
            {
                get { return Color.FromArgb(90, 194, 231); }
            }
            public override Color MenuItemSelectedGradientEnd
            {
                get { return Color.FromArgb(90, 194, 231); }
            }

            public override Color MenuItemPressedGradientMiddle
            {
                get { return Color.FromArgb(90, 194, 231); }
            }

            public override Color MenuItemPressedGradientBegin
            {
                get { return Color.FromArgb(90, 194, 231); }
            }

            public override Color MenuItemPressedGradientEnd
            {
                get { return Color.FromArgb(90, 194, 231); }
            }
        }

        #endregion

        #region 右侧按钮，主要是测试用

        private void rB_AreaPartition_CheckedChanged(object sender, EventArgs e)
        {
            if (rB_AreaPartition.Checked)
            {
                groupBox_ChangeArea.Visible = true;
                cB_AreasList.Items.Clear();
                cB_AreasList.Items.Add("未选择区域");
                if (Sample._areas != null)
                {
                    for (int i = 0; i < Sample._areas.Count; i++)
                    {
                        for (int j = 0; j < Sample._areas[i].Count; j++)
                        {
                            cB_AreasList.Items.Add(Sample._areas[i][j].AreaName);
                        }

                    }
                    cB_AreasList.SelectedIndex = 0;
                }
            }
            else groupBox_ChangeArea.Visible = false;
        }

        private void cB_AreasList_SelectedIndexChanged(object sender, EventArgs e)
        {

            int areaId = GetAreabyName(cB_AreasList.Text).Id;//是0代表空！是0xff代表未选择
            if (areaId > 0)
            {
                for (int i = 0; i < tile.Polys.Length; i++)
                {
                    if (tile.Polys[i].Area.Id == areaId)
                    {
                        tile.Polys[i].Selected = true;
                    }
                    else
                    {
                        tile.Polys[i].Selected = false;
                    }
                }
            }
            else
            {
                for (int i = 0; i < tile.Polys.Length; i++)
                {
                    tile.Polys[i].Selected = false;
                }
            }
        }
        private void rB_AreaSet_CheckedChanged(object sender, EventArgs e)
        {
            groupBox_paramenter.Visible = rB_AreaSet.Checked;
        }

        private void SaveNavMeshToFile(string path)
        {
            try
            {
                new NavMeshJsonSerializer().Serialize(path, Sample.songnav.tiledNavMesh);
            }
            catch (Exception e)
            {
                Console.WriteLine("Navmesh saving failed with exception:" + Environment.NewLine + e.ToString());
                return;
            }
            Console.WriteLine("Saved to file!");
        }

        private void btn_SaveMesh_Click(object sender, EventArgs e)
        {
            //Gwen.Platform.Neutral.FileSave("Save NavMesh to file", ".", "All SharpNav Files(.snb, .snx, .snj)|*.snb;*.snx;*.snj|SharpNav Binary(.snb)|*.snb|SharpNav XML(.snx)|*.snx|SharpNav JSON(.snj)|*.snj", SaveNavMeshToFile);
            SaveNavMeshToFile("../../Meshes/" + Sample.snbName);
        }


        private void cB_MultiSpeed_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cB_MultiSpeed.SelectedIndex)
            {
                case 0:
                    speedMulti = 1;
                    break;
                case 1:
                    speedMulti = 2;
                    break;
                case 2:
                    speedMulti = 5;
                    break;
                case 3:
                    speedMulti = 10;
                    break;
            }
        }

        
        private void chBoxChart_CheckedChanged(object sender, EventArgs e)
        {
            if (chBoxChart.CheckState == CheckState.Checked)
            {
                panelInfo.Visible = true;
            }
            else
            {
                panelInfo.Visible = false;
            }
        }


        //用来控制，是对人赋予颜色，还是生成位图热力图
        private void rB_heatmap_CheckedChanged(object sender, EventArgs e)
        {
            if (rB_heatmap.Checked) heatmapThreadControl = true;
            else heatmapThreadControl = false;
        }

        #endregion
        
        #region 左侧界面
        public System.Threading.Timer timerSlide = null;
        private void labelDrowBack_Click(object sender, EventArgs e)
        {
            if (SidePanel.Left > -10)
            {
                timerSlide = new System.Threading.Timer(SlideToLeft, this, 0, 30);
                label4.Text = ">>";
            }
            else
            {
                timerSlide = new System.Threading.Timer(SlideToRight, this, 0, 30);
                label4.Text = "<<";
            }
        }


        public void SlideToRight(Object obj)
        {
            SidePanel.Left += 30;
            if (SidePanel.Left >= -10)
            {
                SidePanel.Left = -8;

                timerSlide.Dispose();
            }
        }

        public void SlideToLeft(Object obj)
        {
            SidePanel.Left -= 30;
            if (SidePanel.Left <= -360)
            {
                SidePanel.Left = -382;
                timerSlide.Dispose();
            }
        }
        #endregion

        #region 播放控件相关
        
        //背景
        private void panelReadPlay_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, panelReadPlay.ClientRectangle, Color.FromArgb(64, 128, 185), ButtonBorderStyle.Solid);
        }

        //控制条
        private void trackBar_replay_MouseDown(object sender, MouseEventArgs e)
        {
            timerRead.Enabled = false;
        }
        private void trackBar_replay_ValueChanged(object sender, EventArgs e)
        {
            this.label_frameMin.Text = trackBar_replay.Value.ToString();
        }

        private void trackBar_replay_MouseUp(object sender, MouseEventArgs e)
        {
            Console.WriteLine(trackBar_replay.Value);
            int linesIndex = trackBar_replay.Value;
            pictureBox_Play.Image = test.Properties.Resources.video_pause;//用鼠标移动后，图片显示为暂停
            timerRead.Enabled = false;

            for (int i = 0; i < _instance.Count; i++)
            {
                _instance[i].ReadLineNoThread(linesIndex, speedMulti);
            }
            timerRead.Enabled = true;
        }

        //两个按钮,播放停止
        private void pictureBox_Play_Click(object sender, EventArgs e)
        {
            if (timerRead.Enabled == true)
            {
                timerRead.Enabled = false;
                pictureBox_Play.Image = test.Properties.Resources.video_play;
            }
            else
            {
                if (trackBar_replay.Value == trackBar_replay.Maximum)
                {
                    for (int i = 0; i < _instance.Count; i++)
                    {
                        _instance[i].ReadLineNoThread(1, speedMulti);
                    }
                    trackBar_replay.Value = 1;
                }
                timerRead.Enabled = true;
                pictureBox_Play.Image = test.Properties.Resources.video_pause;
            }
        }
        private void pictureBox_Stop_Click(object sender, EventArgs e)
        {
            timerRead.Enabled = false;
            pictureBox_Play.Image = test.Properties.Resources.video_play;// Image.FromFile("easyicon_201810020300314909/1184885.png");
            trackBar_replay.Value = 1;

            stepCounts = _instance[0].stepCounts;
            for (int i = 0; i < _instance.Count; i++)
            {
                _instance[i]._agents.Clear();
                _instance[i].linesCount = 1;
                _instance[i].ReadInit();
            }

            labelPanelInfo.Text = GetInfoforDisplay();
            doubleBufferPanel1.Refresh();

            chartOuts.Series[0].Points.Clear();
            //chartOuts.Series[1].Points.Clear();

            //for (int i = 0; i < Instance._out.Length; i++)
            //{
            //    chartOuts.Series[0].Points.AddXY((i + 1), Sample.outAgentsSetted[i]);
            //    //chartOuts.Series[1].Points.AddXY((i + 1), Sample.outAgentsSetted[i]);
            //}
            UpdateSimulateStatus(SimulateStates.ReadingEnd);
        }
        #endregion

        public enum SimulateStates
        {
            SimulatInited,
            Simulating,
            SimulatPaused,
            SimulateEnd,
            Reading,
            ReadingEnd
        }

        SimulateStates state;

        public void UpdateSimulateStatus(SimulateStates s)
        {
            //根据当前状态, 设置UI界面的显示消失与失效
            //仿真/暂停 停止 播放 panelReadPlay
            state = s;
            switch (state)
            {
                case SimulateStates.SimulatInited:
                    menu_Start.Text = "仿真";
                    menu_Start.Enabled = true;
                    停止ToolStripMenuItem.Enabled = false;
                    播放ToolStripMenuItem.Enabled = false;
                    panelReadPlay.Visible = false;

                    //rB_Border.Checked = true;
                    menu_Start.Image = test.Properties.Resources.simulate_play;//Image.FromFile("img_play.png");

                    break;
                case SimulateStates.Simulating:
                    menu_Start.Text = "暂停";
                    menu_Start.Enabled = true;
                    停止ToolStripMenuItem.Enabled = true;
                    播放ToolStripMenuItem.Enabled = false;
                    panelReadPlay.Visible = false;
                    panelAll.Enabled = true;
                    menu_Start.Image = test.Properties.Resources.simulate_pause;// Image.FromFile("img_pause.png");

                    timerforHeatmap.Enabled = true;

                    break;
                case SimulateStates.SimulatPaused:
                    menu_Start.Text = "继续";
                    menu_Start.Enabled = true;
                    停止ToolStripMenuItem.Enabled = true;
                    播放ToolStripMenuItem.Enabled = false;
                    panelReadPlay.Visible = false;

                    timerforHeatmap.Enabled = false;

                    menu_Start.Image = test.Properties.Resources.simulate_play;
                    break;
                case SimulateStates.SimulateEnd:
                    menu_Start.Text = "仿真";
                    menu_Start.Enabled = true;
                    停止ToolStripMenuItem.Enabled = false;
                    播放ToolStripMenuItem.Enabled = true;
                    panelReadPlay.Visible = false;
                    menu_Start.Image = test.Properties.Resources.simulate_play;
                    break;
                case SimulateStates.Reading:
                    menu_Start.Text = "仿真";
                    menu_Start.Enabled = true;
                    停止ToolStripMenuItem.Enabled = false;
                    播放ToolStripMenuItem.Enabled = false;
                    menu_Start.Image = test.Properties.Resources.simulate_play;
                    panelReadPlay.Visible = true;
                    trackBar_replay.Value = 1;
                    break;
                case SimulateStates.ReadingEnd:
                    menu_Start.Text = "仿真";
                    menu_Start.Enabled = true;
                    停止ToolStripMenuItem.Enabled = false;
                    播放ToolStripMenuItem.Enabled = true;
                    menu_Start.Image = test.Properties.Resources.simulate_play;
                    panelReadPlay.Visible = true;
                    break;
            }
        }

        #region 定时运行
        
        //private void StopSimulate()
        //{
        //    timer1.Enabled = false;

        //    for (int i = 0; i < _instance.Count; i++)
        //    {
        //        _instance[i].controlSteps = true;
        //        _instance[i].sw.Stop();
        //        if(_instance[i].stepsThread!=null && _instance[i].stepsThread.IsAlive) _instance[i].stepsThread.Abort();
        //        _instance[i].OutpositionFileEnd();
        //        _instance[i]._agents.Clear();
        //    }
        //    Sample.stepscontrolThread.Abort();
        //    doubleBufferPanel1.Refresh();

        //    UpdateSimulateStatus(SimulateStates.SimulateEnd);
        //}

        private void StopSimulate()
        {
            timerSimulate.Enabled = false;
            timerforHeatmap.Enabled = false;

            int agentsCount = 0;
            for (int i = 0; i < _instance.Count; i++)
            {
                _instance[i].controlSteps = true;
                _instance[i].sw.Stop();
                if (_instance[i].stepsThread != null && _instance[i].stepsThread.IsAlive) _instance[i].stepsThread.Abort();
                _instance[i].OutpositionFileEnd();
                _instance[i]._agents.Clear();

                agentsCount += _instance[i].agentsPositionsforExodus.Count;
            }


            //输出vrs
            if (checkBox_outputVRS.Checked)
            {
                FileHelper FHforExodus;
                FHforExodus = new FileHelper(Sample.mainDirectory + "_Exodus" + ".vrs");
                FHforExodus.Write("3 0");//先输出疏散人数
                FHforExodus.NewLine();//换行
                FHforExodus.Write(agentsCount.ToString() + " 0 " + getMaxStep());//先输出疏散人数
                for (int i = 0; i < _instance.Count; i++)
                {
                    _instance[i].outForExodus(FHforExodus);
                    _instance[i].agentsPositionsforExodus.Clear();
                }
                FHforExodus.EndOut();
            }


            Sample.stepscontrolThread.Abort();
            doubleBufferPanel1.Refresh();

            UpdateSimulateStatus(SimulateStates.SimulateEnd);
        }
        int getMaxStep()//为Exodus ，得到最大不长数
        {
            int max = 0;
            for (int i = 0; i < _instance.Count; i++)
            {
                if (max < _instance[i].FH.writeflame)//没输出一帧数据，也同时更新步长数，这里帧数writeflame可以看成步长数
                    max = _instance[i].FH.writeflame;
            }
            return max;
        }
        //设置图表
        private void UpdataInfo()
        {
            if (Instance._out != null)
            {
                string s1 = "出口ID\r\n\r\n";
                string s2 = "待疏散人数\r\n\r\n";
                string s3 = "所占比例\r\n\r\n";

                chartOuts.ChartAreas[0].AxisY.Maximum = GetMaxOutAgentSetted();

                chartOuts.Series[0].Points.Clear();
                for (int i = 0; i < Instance._out.Length; i++)
                {
                    int outagents = Instance._outAgentCount[i];
                    chartOuts.Series[0].Points.AddXY((i + 1), outagents > 0 ? outagents : 0);
                    s1 += " " + (i + 1).ToString() + "\r\n";
                    s2 += "   " + (Sample.outAgentsSetted[i] - Instance._outAgentCount[i]) + "\r\n";
                    s3 += "   " + (Instance._outAgentCount[i] * 100 / (Sample.numsPeople - agentsOutCount)).ToString() + "%\r\n";
                }
                labelOutdoor.Text = s1;
                labelOutAgents.Text = s2;
                labelOutBili.Text = s3;
            }
        }

        private int GetMaxOutAgentSetted()
        {
            int max = 0;
            for (int i = 0; i < Sample.outAgentsSetted.Count; i++)
            {
                max = max > Sample.outAgentsSetted[i] ? max : Sample.outAgentsSetted[i];
            }
            return max;
        }

        //参数显示
        public int agentsOutCount;
        public string GetInfoforDebug()
        {
            //总人数，
            string s = "";
            agentsOutCount = 0;
            for (int i = 0; i < _instance.Count; i++)
            {
                agentsOutCount += _instance[i]._agents.Count;
            }
            s += "总人数： " + agentsOutCount + " " + (agentsOutCount * 100 / Sample.numsPeople) + "% \r\n";
            //s += "疏散口1： " + Instance._outAgentCount[0] + " " + (Instance._outAgentCount[0] * 100 / Sample.numsPeople) + "% \r\n";

            s += "0总步数： " + _instance[0].stepCounts + " 仿真时间：" + (_instance[0].stepCounts * Settings.deltaTDefault).ToString("0.0") + "秒 \r\n";
            s += "计算耗时： " + (((float)(_instance[0].sw.ElapsedMilliseconds)) / 1000).ToString("0.0") + "秒 \r\n";
            s += AreaIdSelected + "  " + Instance.getAreaAcreage(Sample.tile, AreaIdSelected) + "  \r\n";
            //s += Instance.getPolyAcreageTest();
            s += "最大速度：" + _instance[0].speedMaxTest + "  \r\n";
            //s += "0的行走距离：" + _instance[0].agentRoadLength + "  \r\n";
            s += "鼠标距离： " + distance + "  \r\n";
            s += "鼠标坐标：" + mouseRealX + " , " + mouseRealY + "  \r\n";
            s += "mapcount：" + mapCount + "  \r\n";
            s += "_zoom：" + _zoom + "  \r\n";
            s += " _gridLeftTop：" + _gridLeftTop + "  \r\n";

            for (int i = 0; i < Sample._instance.Count; i++)
            {
                s += " linescount：" + Sample._instance[i].linesCount + "  \r\n";
            }


            return s;
        }
        public string GetInfoforDisplay()
        {
            //总人数，
            string s = "";

            s += "总人数： " + Sample.numsPeople + " \r\n\r\n";

            #region 计算疏散时间
            //int minStepCount = 900000;
            //for (int i = 0; i < _instance.Count; i++)
            //{
            //    if (minStepCount > _instance[i].stepCounts) minStepCount = _instance[i].stepCounts;
            //}
            //现在线程同步,所以用最大的,以前线程不同步,用最小的
            int maxStepCount = 0;
            for (int i = 0; i < _instance.Count; i++)
            {
                if (maxStepCount < _instance[i].stepCounts) maxStepCount = _instance[i].stepCounts;
            }
            int time = (int)(maxStepCount * Settings.deltaTDefault);

            //为了方便,暂时这里控制最快读取速率
            if (Sample.readMode)//如果是读取模式
            {
                int chushu = Sample.numsPeople >= 50000 ? 1 : 5;//一秒一帧和0.2秒一帧
                time = (int)(GetMaxReadLine() * Settings.deltaTDefault * 10 / chushu);

                if (Sample.numsPeople >= 50000) timerRead.Interval = 100;//5万以下, 一秒10帧
                else timerRead.Interval = 100;//5万以下, 一秒1帧
            }

            //time = (int)(1.35f * time);
            s += "疏散时间： " + time / 60 + "分 " + time % 60 + "秒 \r\n\r\n";
            //s += "计算耗时： " + (((float)(_instance[0].sw.ElapsedMilliseconds)) / 1000).ToString("0.0") + "秒 \r\n";
            #endregion


            agentsOutCount = 0;
            for (int i = 0; i < _instance.Count; i++)
            {
                agentsOutCount += _instance[i]._agents.Count;
            }
            int agentOuted = Sample.numsPeople - agentsOutCount;
            s += "已疏散人数：" + agentOuted + "  百分比：" + (agentOuted * 100 / Sample.numsPeople) + "% \r\n";

            return s;
        }
        public int GetMaxReadLine()//从多个Instance中找最多的那个,当前读取的帧数
        {
            int maxLine = 0;
            for (int i = 0; i < Sample._instance.Count; i++)
            {
                if (maxLine < Sample._instance[i].linesCount) maxLine = Sample._instance[i].linesCount;
            }
            return maxLine;
        }

        //Chart crowdChart = new Chart();
        //private void chBoxChart_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (chBoxChart.CheckState == CheckState.Checked)
        //    {
        //        crowdChart.Show(this);
        //    }
        //    else
        //    {
        //        crowdChart.Hide();
        //        //crowdChart.Dispose();
        //    }
        //}
        #endregion

        //定时函数,默认0.1秒触发一次
        int timerCount = 0;//定时计数，通过定时技术用来控制

        private void timerSimulate_Tick(object sender, EventArgs e)
        {
            if (_instance != null)//&& stepCounts != _instance[0].stepCounts
            {
                timerCount++;

                //显示信息
                labelInfo.Text = GetInfoforDebug();
                labelPanelInfo.Text = GetInfoforDisplay();

                //刷新显示
                doubleBufferPanel1.Refresh();
                stepCounts = _instance[0].stepCounts;
                
                if (!Sample.readMode)//如果当前不是读取模式,也就是仿真模式
                {
                    //每刷新十次，刷新一次圆柱颜色
                    if (timerCount >= 5)
                    {
                        ////判断是否要停止,控制让其所有人数都少于百分之1时才停止
                        //bool stop = true;
                        //for (int i = 0; i < _instance.Count; i++)//如果有任何人数>百分之2，就令stop为false
                        //{
                        //    if (_instance[i]._agents.Count > (_instance[i].agentsOrigine / 100) || _instance[i]._agents.Count > 100)//剩余百分之XX的时候
                        //    {
                        //        stop = false;
                        //        break;
                        //    }
                        //}
                        //if (stop == true)
                        //{
                        //    StopSimulate();
                        //    MessageBox.Show("仿真结束", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //    menu_Start.Text = "仿真";
                        //}

                        if (Sample.stopped == true && state != SimulateStates.SimulatInited)
                        {
                            StopSimulate();
                            MessageBox.Show("仿真结束", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            menu_Start.Text = "仿真";
                        }
                        UpdataInfo();//更新左上角的pannel
                        timerCount = 0;
                    }
                }
            }
        }

        int infectCounts = 0;//用来记录上次感染第几次
        private void timerforHeatmap_Tick(object sender, EventArgs e)
        {
            if (_instance != null)//&& stepCounts != _instance[0].stepCounts
            {
                if (!heatmapThreadControl)
                {
                    if (infectCounts != stepCounts / Sample.girdTimeRevIntervel)//判断如果这次间隔数跟上次间隔数不同
                    {
                        Console.WriteLine("infectCounts "+ infectCounts);
                        Console.WriteLine("stepCounts  " + stepCounts);
                        List<List<AgentClass>> _agents = new List<List<AgentClass>>();
                        for (int i = 0; i < _instance.Count; i++)
                        {
                            _agents.Add(_instance[i]._agents);
                        }
                        HeatMap.CalculateColor(ref _agents, stepCounts, true);//stepCounts
                        infectCounts = stepCounts / Sample.girdTimeRevIntervel;
                    }
                    else
                    {
                        List<List<AgentClass>> _agents = new List<List<AgentClass>>();
                        for (int i = 0; i < _instance.Count; i++)
                        {
                            _agents.Add(_instance[i]._agents);
                        }
                        HeatMap.CalculateColor(ref _agents, stepCounts, false);//stepCounts
                    }
                }
            }
        }

        private void timerRead_Tick(object sender, EventArgs e)
        {
            
            for (int i = 0; i < _instance.Count; i++)
            {
                _instance[i].ReadLineNoThread(0, speedMulti);//0是默认时的另外情况判断, 这里不用跳,所以0
            }
            if (trackBar_replay.Value < trackBar_replay.Maximum)
            {
                int lineNow = GetMaxReadLine();
                if(lineNow>0)trackBar_replay.Value = lineNow > trackBar_replay.Maximum ? trackBar_replay.Maximum : lineNow;
            }
            UpdataInfo();

            //显示信息
            labelInfo.Text = GetInfoforDebug();
            labelPanelInfo.Text = GetInfoforDisplay();

            //刷新显示
            doubleBufferPanel1.Refresh();
            stepCounts = _instance[0].stepCounts;

        }

        private void cB_ShowExits_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cB_DrawColorRule_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox_outputVRS_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
