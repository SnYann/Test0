using SharpNav;
using SharpNav.Geometry;
using SharpNav.Pathfinding;
using Simulate;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace test.Scripts
{
    //一个调用示例
    static class Sample
    {
        /** 
         * 初始化地图
         * 初始化设置
         *
         */
         
        public static Simulate.Navigation songnav;
        public static List<Instance> _instance=new List<Instance>();
        //public List<ControlGate> controlGates;//用来控制道路的单向双向
        
        public static Stopwatch sw = new Stopwatch();//用来帮助计算程序耗时
        public static NavTile tile;
        public static string pathSetting;

        public static List<List<Area>> _areas;
        public static List<int> outAgentsSetted = new List<int>();

        public static int numsPeople;
        internal static bool outAgentsFile;
        internal static bool outpositionfileMode;
        internal static bool readMode;
        public static string mainDirectory;
        public static string projectName;
        public static Thread stepscontrolThread;
        public static int frameCount = 0;
        public static bool isSelectNearest = false;

        //用来记录 接收时间指令方式 的设置，并不记录到set文件中
        public static bool isSelectTimeRev_area = false;//选择了分区时间设置
        public static int girdTimeRevIntervel = 10;



        //用来存放出口选择
        public  static List<List<int>> _OutID = new List<List<int>>();

        public static void Clear()//读取文件时用,新的工程建立时用
        {
            _instance.Clear();
            outpositionfileMode = false;
            readMode = false;
            //_areas.Clear();
            //outAgentsSetted.Clear();
            numsPeople = 0;
            pathSetting = "";
            for(int i=0;i<outAgentsSetted.Count;i++)
            {
                outAgentsSetted[i] = 0;
            }
        }

        public static void OpenInit()
        {
            //1. 初始化地图
            sw.Start();
            InitMap();
            Console.WriteLine("读取生成耗时： " + sw.ElapsedMilliseconds / 1000);

            //2.得到疏散点给Instance
            Vector3[] _out = new Vector3[songnav.map._out.Count];
            //同一个输出目标的疏散人数统计,初始化0
            for (int i = 0; i < _out.Count(); i++) outAgentsSetted.Add(0);

            for (int i = 0; i < songnav.map._out.Count; i++)
            {
                _out[i] = new Vector3(songnav.map._out[i].x_, 0, songnav.map._out[i].y_);
            }
            Instance._out = _out;//先给疏散点赋值!!
        }

        //判断当前是否有仿真文件
        public static bool ExitsOutput()
        {
            //这里只判断一个线程的输出文件
            if (File.Exists(mainDirectory + "output0.txt"))
            {
                return true;
            }
            return false;
        }

        public static void SimulateInit()
        {
            Clear();
            outpositionfileMode = true;
            //_instance.Clear();

            //0.初始化目录
            pathSetting = mainDirectory + projectName;
            
            ////1. 初始化地图
            //sw.Start();
            //InitMap();
            //Console.WriteLine("读取生成耗时： " + sw.ElapsedMilliseconds / 1000);

#if outDebug
            ////输出agent信息文件2
            FileHelper.SetOutPath("mydebug.txt");
#endif

            

            //3.得到不同区域的数据，并设定不同区域人数
            GetAreasData(out _areas);

            //4.得到不同区域不同出口
            GetOut(pathSetting);

            //5.初始化Instance
            for (int i = 0; i < _areas.Count; i++)
            {
                List<Line> obs = new List<Line>();
                //得到分区域的obs
                obs = songnav.GetObstacleByArea(_areas[i]);
                _instance.Add(new Instance("output" + i.ToString(), _areas[i], obs,getMap(),getMap(),_OutID[i]));
            }

            //5.对每个instance寻径
            //foreach (Instance i in _instance)
            //{
            //    i.pathFind();
            //}

        }

        public static void GetOut(string path)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(path);
                XmlElement settings = xmlDocument.DocumentElement;
                XmlNodeList areaouts = settings.GetElementsByTagName("AreaOuts");

                _OutID.Clear();
                foreach (XmlElement element in areaouts)//这里应该只有一个AgentOuts
                {
                    XmlNodeList outs = element.GetElementsByTagName("Outs");
                    foreach (XmlElement e in outs)
                    {
                        List<int> outids = new List<int>();
                        foreach (XmlNode node in e)
                        {
                            outids.Add(int.Parse(((XmlElement)node).GetAttribute("outid"))-1);
                        }
                        _OutID.Add(new List<int>(outids));
                    }
                }
            }
            catch
            {

            }
        }


        public static void SimulatePathFind()
        {
            Thread t = new Thread(SimulatePathFindThread);
            t.Start();
        }
        public static void SimulatePathFindThread()
        {
            sw.Start();
            List<Thread> _T = new List<Thread>();
            foreach (Instance i in _instance)
            {
                Thread t = new Thread(i.PathFind);
                _T.Add(t);
                t.Start();
            }
            //等待4个线程全部完成
            for (int i = 0; i < _T.Count; i++)
            {
                while (_T[i].IsAlive) ;
            }
            Console.WriteLine("寻径耗时： " + sw.ElapsedMilliseconds / 1000);


            ////-输出各个agent导航点文件
            ////在每个每个区域地图的agent初始化完成后，输出每个agent的位置与导航点
            //var FHagents = new FileHelper(mainDirectory+"agents.txt");
            //for (int i = 0; i < _instance.Count; i++)
            //{
            //    foreach(var agent in _instance[i]._agents) //(int j = 0; j < _instance[i]._agents.Count; j++)
            //    {
            //        for (int j = 0; j < agent.navPoints.Count; j++)
            //        {
            //            if (agent.navPoints[j].x_ == 0 && agent.navPoints[j].y_ == 0)
            //            {
            //                agent.navPoints.RemoveAt(j--);
            //            }
            //        }
            //        //FileHelper.Write(_agents[i].positionNow.x_ + " " + _agents[i].positionNow.y_ + " " + _agents[i].navPoints.Count + " ");
            //        FHagents.Write(agent.outIndex + " " + _instance[i].RVOInstance.getAgentPosition(i).x() + " " + _instance[i].RVOInstance.getAgentPosition(i).y() + " " + agent.navPoints.Count + " ");
            //        for (int j = 0; j < agent.navPoints.Count; j++) FHagents.Write(agent.navPoints[j].x_.ToString("0.00") + " " + agent.navPoints[j].y_.ToString("0.00") + " ");
            //        FHagents.NewLine();
            //    }
            //}
            //FHagents.EndOut();

            //开启对不同部分地图的控制
            stepscontrolThread = new Thread(stepsControl);
            stepscontrolThread.Start();
        }


        public static bool stopped=false;
        public static void stepsControl()
        {
            //控制线程依次执行
            //思路是每当所有instance都为false，就都令其变为true
            while (true)
            {
                //如果所有controlSteps都是false, 就控制都为true
                bool newStep = true;
                for(int i=0;i<_instance.Count;i++)
                {
                    if (_instance[i].controlSteps == true)
                    {
                        newStep = false;//如果有任何Instance没有仿真完，就不进行下一步
                        break;
                    }
                }


                ////王吉-输出每个agent导航点位置
                if (_instance[2].stepCounts == 10) //当区域0的仿真次数大于30的时候输出agent的导航点信息
                {
                    OutAgentNavPoints();
                }

                //判断是否要停止,控制让其所有人数都少于百分之1时才停止
                bool stop = true;
                for (int i = 0; i < _instance.Count; i++)//如果有任何人数>百分之2，就令stop为false
                {
                    if (_instance[i]._agents.Count > (_instance[i].agentsOrigine / 100) || _instance[i]._agents.Count > 100)//剩余百分之XX的时候
                    {
                        stop = false;
                        break;
                    }
                }
                if (stop == true)
                    stopped = true;//这个参数用来给UI调用
                else stopped = false;

                if (newStep && !stop)
                {
                    foreach (var ins in _instance)
                    {
                        ins.controlSteps = true;
                    }
                }
            }
        }

        public static void OutAgentNavPoints()
        {
            
            ////在每个每个区域地图的agent初始化完成后，输出每个agent的位置与导航点
            var FHagents = new FileHelper(Sample.mainDirectory + "agents.txt");
            for (int i = 0; i < Sample._instance.Count; i++)
            {

                for (int j = 0; j < _instance[i]._agents.Count; j++)//( in Sample._instance[i]._agents) //
                {
                    var agent = _instance[i]._agents[j];
                    for (int k = 0; k < agent.navPoints.Count; k++)
                    {
                        if (agent.navPoints[k].x_ == 0 && agent.navPoints[k].y_ == 0)
                        {
                            agent.navPoints.RemoveAt(k--);
                        }
                    }
                    FHagents.Write(agent.outIndex + " " + agent.positionNow.x_ + " " + agent.positionNow.y_ + " " + agent.navPoints.Count + " ");
                    for (int k = 0; k < agent.navPoints.Count; k++) FHagents.Write(agent.navPoints[k].x_.ToString("0.00") + " " + agent.navPoints[k].y_.ToString("0.00") + " ");
                    FHagents.NewLine();
                }
            }
            FHagents.EndOut();
        }


        public static bool initRead2()
        {
            frameCount = 0;
            for (int i = 0; i < _instance.Count; i++)
            {
                _instance[i].ReadInit();
                //Thread r = new Thread(_instance[i].ReadLineThread);
                //r.Start();
                if (_instance[i].GetFrameCount() > frameCount)
                {
                    frameCount = _instance[i].GetFrameCount();
                }
            }
            return true;
        }

        public static bool initRead()
        {
            //0.设置模式
            outpositionfileMode = false;
            readMode = true;

            ////1. 初始化地图
            //sw.Start();
            //InitMap();
            //Console.WriteLine("读取生成耗时： " + sw.ElapsedMilliseconds / 1000);
            
            //初始化目录
            pathSetting = mainDirectory + projectName;

            //InitMap();
            Console.WriteLine("读取生成耗时： " + sw.ElapsedMilliseconds / 1000);
            Vector3[] _out = new Vector3[songnav.map._out.Count];
            //同一个输出目标的疏散人数统计,初始化0
            for (int i = 0; i < _out.Count(); i++) outAgentsSetted.Add(0);

            for (int i = 0; i < songnav.map._out.Count; i++)
            {
                _out[i] = new Vector3(songnav.map._out[i].x_, 0, songnav.map._out[i].y_);
            }
            Instance._out = _out;//先给疏散点赋值!!

            if(!GetAreasData(out _areas))return false;//得到不同区域的数据，并设定不同区域人数

            //4.得到不同区域不同出口
            GetOut(pathSetting);

            for (int i = 0; i < _areas.Count; i++)
            {
                List<Line> obs = new List<Line>();
                //得到分区域的obs
                obs = songnav.GetObstacleByArea(_areas[i]);

                _instance.Add(new Instance("output" + i.ToString(), _areas[i], obs, getMap(),getMap(),_OutID[i]));
            }
            return true;
            //每行一帧，读取位置并显示
        }

        public struct peopleAttribute
        {
            public string name;
            public int percentage;
            public float speedMin;
            public float speedMax;
            public int responseTimeMin;
            public int responseTimeMax;
        }

        public static int responseDistribution = 0;//用来标注人员反应时间的分布
        public static List<peopleAttribute> _peopleAttr = new List<peopleAttribute>();

        private static bool GetAreasData(out List<List<Area>> _areas)
        {
            _areas = new List<List<Area>>();
            if (!File.Exists(pathSetting))
            {
                return false;
            }
            // 从配置文件中获取数据，并进行初始化
            // 得到人数数据
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(pathSetting);
            XmlElement settings = xmlDocument.DocumentElement;
            XmlNodeList people = settings.GetElementsByTagName("People");
            XmlNodeList selectNearest = settings.GetElementsByTagName("SelectNearest");
            isSelectNearest=((XmlElement)selectNearest[0]).GetAttribute("bool")=="0"? false:true;
            XmlNodeList XmlAreas = settings.GetElementsByTagName("Areas");

            //得到人数
            List<peopleAttribute> peopleAttr=new List<peopleAttribute>();
            var e=(XmlElement)people[0];
            numsPeople = int.Parse(e.GetAttribute("num"));
            responseDistribution = int.Parse(e.GetAttribute("distribution"));

            _peopleAttr.Clear();
            int percentCount = 0;
            foreach (XmlNode node in e)
            {
                peopleAttribute p;
                p.name = node.Name;
                percentCount+= int.Parse(((XmlElement)node).GetAttribute("percentage"));
                p.percentage = percentCount;
                p.speedMin = float.Parse(((XmlElement)node).GetAttribute("speedMin"));
                p.speedMax = float.Parse(((XmlElement)node).GetAttribute("speedMax"));
                p.responseTimeMin = int.Parse(((XmlElement)node).GetAttribute("responseTimeMin"));
                p.responseTimeMax = int.Parse(((XmlElement)node).GetAttribute("responseTimeMax"));
                _peopleAttr.Add(p);
            }

            numsPeople = int.Parse(((XmlElement)people[0]).GetAttribute("num"));//第一个people设置的总人数
            //peopleDistribution = int.Parse(((XmlElement)people[0]).GetAttribute("ditribution"));//第一个people设置的总人数 ///////////////////////////////////////////////////////////

            tile = songnav.tiledNavMesh.GetTileAt(0, 0, 0);//得到网格

            float acreage_density = 0;//面积和密度想乘
            foreach (XmlElement element in XmlAreas)
            {
                foreach (XmlNode stepNode in element)
                {
                    string id = ((XmlElement)stepNode).GetAttribute("id");
                    string density = ((XmlElement)stepNode).GetAttribute("density");//总人数，由于这里的密度就是每个不同面积区域的总人数，因此density意思就是总人数了
                    float aceage = Instance.getAreaAcreage(tile, int.Parse(id));
                    acreage_density += aceage * (int.Parse(density));
                }
            }
            float countfactor = numsPeople / acreage_density;//系数=总人数/总(面积*密度)
            
            foreach (XmlElement element in XmlAreas)
            {
                List<Area> areas = new List<Area>();
                foreach (XmlNode stepNode in element)
                {
                    string id = ((XmlElement)stepNode).GetAttribute("id");
                    string direct = ((XmlElement)stepNode).GetAttribute("direct");
                    string name = ((XmlElement)stepNode).GetAttribute("name");
                    string pos1 = ((XmlElement)stepNode).GetAttribute("pos1");
                    string pos2 = ((XmlElement)stepNode).GetAttribute("pos2");
                    string density = ((XmlElement)stepNode).GetAttribute("density");//总人数，由于这里的密度就是每个不同面积区域的总人数，因此density意思就是总人数了
                    string receiveTime = ((XmlElement)stepNode).GetAttribute("receiveTime");//区域接收指令时间

                    float aceage = Instance.getAreaAcreage(tile, int.Parse(id));
                    acreage_density += aceage * (int.Parse(density));
                    //Console.WriteLine((int)(int.Parse(density) * countfactor));
                    int aeraAgentNums = (int)(int.Parse(density) * Instance.getAreaAcreage(tile, int.Parse(id)) * countfactor);
                    if (pos1 == "最近出口") pos1 = "20";
                    if (pos2 == "最近出口") pos2 = "20";

                    if (!isSelectTimeRev_area) receiveTime = "0";//如果当前选择的是从中心网格疏散，就让各个区域接收时间都变成0

                    areas.Add(new Area(byte.Parse(id), direct == "1" ? true : false, name, int.Parse(pos1), int.Parse(pos2), aeraAgentNums, int.Parse(receiveTime)));
                    //areas.Add(new Area(byte.Parse(id), direct == "1" ? true : false, name, int.Parse(pos1), int.Parse(pos2), (int)(int.Parse(density))));
                    //Console.WriteLine(byte.Parse(id));
                }
                _areas.Add(areas);
            }
            return true;
        }

        /// <summary>
        /// 初始化地图文件，生成
        /// </summary>
        /// 
        public static string snbName = "f8.snb";
        public static  void InitMap()
        {
            songnav = new Simulate.Navigation();
            //songnav.GenerateMeshFromFile("../../Meshes/mesh.obj");
            //songnav.LoadNavMeshFromFile("../../Meshes/f42.snb", "../../Meshes/mesh.obj");
            songnav.LoadNavMeshFromFile("Meshes/"+snbName, "Meshes/mesh.obj");

        }

        private static Navigation getMap(string name)
        {
            var sn = new Simulate.Navigation();
            //songnav.GenerateMeshFromFile("../../Meshes/mesh.obj");
            //sn.LoadNavMeshFromFile("../../Meshes/f42.snb", "../../Meshes/mesh.obj");
            sn.LoadNavMeshFromFile("Meshes/" + name, "Meshes/mesh.obj");
            //sn = songnav;
            
            return sn;
        }

        private static Navigation getMap()
        {
            var sn = new Simulate.Navigation();
            //songnav.GenerateMeshFromFile("../../Meshes/mesh.obj");
            //sn.LoadNavMeshFromFile("../../Meshes/f42.snb", "../../Meshes/mesh.obj");
            sn.LoadNavMeshFromFile("Meshes/" + snbName, "Meshes/mesh.obj");
            //sn = songnav;
          
            return sn;
        }

        /** 把density当成密度比例，而不是真正密度也不是人数
         *  统计面积，
         *  系数=总人数/总(面积*密度)
         *  一个区域人数=面积*密度*系数
         */
        private static void InitAgents(List<List<Area>> _areas)
        {
  

            

        }
        
    }
}
