using System;
using System.Collections.Generic;
using System.Threading;


using Simulate;
using RVO;
using SharpNav;
using SharpNav.Geometry;
using SharpNav.Pathfinding;
using System.Diagnostics;
using System.IO;
using System.Xml;
using test.Scripts;

namespace Simulate
{

    public class Instance
    {

        public List<AgentClass> _agents = new List<AgentClass>();
        public Navigation songnav1;
        public Navigation songnav2;

        public string instanceID;
        //public List<ControlGate> controlGates;//用来控制道路的单向双向

        public Stopwatch sw = new Stopwatch();//用来帮助计算程序耗时
        public int stepCounts = 0;//统计步数

        //读取相关
        public string pathReading;
        public int linesCount;//用于记录读取文件的行数
        int frame = 0;
        public bool readControl = false;
        public StreamReader reader;


        //输出位置文件相关
        public FileHelper FH;

        public List<Area> areas;
        public Simulator RVOInstance;
        public int agentsOrigine;
        public static Vector3[] _out;
        public static int[] _outAgentCount;


        //限制的出口
        public List<int> OutIDs = new List<int>();

        public Instance(string id, List<Area> a, List<Simulate.Line> obs, Navigation sn1,Navigation sn2, List<int> outs)
        {
            instanceID = id;
            //_agents = _a;//初始化人
            songnav1 = sn1;//总地图
            songnav2 = sn2;

            areas = a;
            ResetAreas();


            OutIDs = outs;

            //感觉位置不对,暂时这样 
            if (Sample.outpositionfileMode)
            {
                FH = new FileHelper(Sample.mainDirectory + instanceID + ".txt");
            }

            _outAgentCount = new int[_out.Length];
            for (int i = 0; i < _outAgentCount.Length; i++) _outAgentCount[i] = 0;


            //RVO的设置
            RVOInstance = new Simulator();
            InitRVO(obs);//初始化RVO




            for (int i = 0; i < areas.Count; i++)
            {
                AddAreaAgents(areas[i],_out);
            }
            agentsOrigine = _agents.Count;//方便界面调用统计人数

            //_agents[10].state = AgentStates.Evacuating;//感染新agent方法初始化时
            

        }

        //重新设置area
        private void ResetAreas()
        {
            for (int i = 0; i < areas.Count; i++)
            {
                songnav1.roadfilter.SetAreaCost(areas[i], 1);
                songnav2.roadfilter.SetAreaCost(areas[i], 1);
            }
        }
        //private bool ContainsID(int id)
        //{
        //    for(int i=0;i<areas.Count;i++)
        //    {
        //        if(areas[i].Id==id)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}



        public void InstanceClear()
        {
            //sw.Stop();
            //if(stepsThread.IsAlive)stepsThread.Abort();
            //OutpositionFileEnd();
        }

        #region 初始化人相关
        //根据不同poly的不同面积大小进行随机人数分配的方法
        public void AddAreaAgents(Area area,Vector3[] _out)
        {
            int agentCount = area.headMaxcount;
            int areaID = area.Id;
            int recevieTime = area.receiveTime;

            if (agentCount == 0) return;
            Simulate.Vector2 posStart;
            Simulate.Vector2 posTarget;
            int positionIndex = 0;
            var tile = songnav1.tiledNavMesh.GetTileAt(0, 0, 0);//得到网格

            //同一个区域同一个疏散目标点，因此只运行一次就可以了
            //根据已知区域，得到疏散目标

            int colorIndex = 0;


            if (areaID != 0xfe)//如果是已经选中设定好的区域，就找到他们的预制好的目标
            {
                //得到区域的目标在out数组中的位置

                //从areas中找到areaID相同的area，得到其目标
                int i;
                for (i = 0; i < areas.Count; i++) if (areas[i].Id == areaID) break;
                positionIndex = areas[i].getGoal();//这样写，别忘了正向反向的方向无法被文件记住,positionIndex即为区域的疏散目标在out数组中的位置
                colorIndex = positionIndex;//颜色指标，跟疏散点对应，相同疏散点，相同颜色
            }
            else///如果是还未设定的区域
            {
                //从areas中找到areaID相同的area，得到其目标
                int i;
                for (i = 0; i < areas.Count; i++) if (areas[i].Id == areaID) break;
                positionIndex = areas[i].getGoal();//这样写，别忘了正向反向的方向无法被文件记住,positionIndex即为区域的疏散目标在out数组中的位置
                colorIndex = positionIndex;//颜色指标，跟疏散点对应，相同疏散点，相同颜色
            }
            Console.WriteLine(instanceID + "目标out" + positionIndex);
            //else//如果是还未设定的区域，就重置一次最终目标 根据最近的出口是哪个(加上判断，如果当前目标的目标点是0,0,0，则重新寻找最近疏散目标点)
            //{
            //    int min = 0;
            //    float minVector2 = Simulate.MathHelper.abs(posStart - nav.level._out[0]);
            //    float delta = Simulate.MathHelper.abs(posStart - nav.level._out[0]);
            //    for (int n = 0; n < nav.level._out.Count; n++)
            //    {
            //        if (delta > Simulate.MathHelper.abs(nav.level._out[n] - posStart))
            //        {
            //            min = n;
            //            delta = Simulate.MathHelper.abs(nav.level._out[n] - posStart);
            //        }
            //    }
            //    //min = new Random().Next(nav.level._out.Count);
            //    colorIndex = min;
            //    //NavPoint navP = nav.navMeshQuery.FindRandomPointAroundCircle(nav.navMeshQuery.FindNearestPoly(new Vector3(e.x_, 0, e.y_), new Vector3(10, 10, 10)), 5);
            //    Vector3 navP = nav.navMeshQuery.FindRandomPointOnPoly(new NavPolyId(nav.navMeshQuery.FindNearestPoly(new Vector3(nav.level._out[min].x_, 0, nav.level._out[min].y_), new Vector3(20, 20, 20)).Polygon.Id));
            //    posTarget = new Simulate.Vector2(navP.X, navP.Z);
            //}


            //为每个agent配置随机起始与目标点
            //随机选择位置
            Vector3 pointStart;
            int areaIndex;

            /**在mesh中 找到相同ID的poly，求出各自面积
                * 根据总人数*单独面积/总面积得到当前poly的人数，然后找随机点
                */
            List<NavPoly> polyLists = new List<NavPoly>();
            float acreageAll = 0;
            for (int j = 0; j < tile.Polys.Length; j++)
            {
                if (tile.Polys[j].Area.Id == areaID)
                {
                    polyLists.Add(tile.Polys[j]);
                    acreageAll += getPolyAcreage(tile, tile.Polys[j]);
                }
            }

            //if (1 == 1)
            //{
            //    Vector3 navP = songnav.navMeshQuery.FindRandomPointOnPoly(new NavPolyId(songnav.navMeshQuery.FindNearestPoly(_out[9], new Vector3(20, 20, 20)).Polygon.Id));
            //    posTarget = new Simulate.Vector2(navP.X, navP.Z);
            //    //Console.WriteLine("总面积" + acreageAll);
            //    posStart = new Simulate.Vector2(24, 13.8f);
            //    int nov = RVOInstance.addAgent(new RVO.Vector2(posStart.x_, posStart.y_), Settings.RVODefault.neighborDist, Settings.RVODefault.maxNeighbors, Settings.RVODefault.timeHorizon, Settings.RVODefault.timeHorizonObst, Settings.RVODefault.radius, 2, new RVO.Vector2(1, 1));
            //    if(nov==0)
            //    _agents.Add(new AgentClass(nov, posStart, posTarget, AgentStates.Enter, 3));//初始化agent编号、位置、目标、状态，根据目标设定颜色

            //}


            int polyAgents = 0;
            for (int j = 0; j < tile.Polys.Length; j++)
            {
                if (tile.Polys[j].Area.Id == areaID)
                {
                    float acreage = getPolyAcreage(tile, tile.Polys[j]);
                    polyAgents = (int)(agentCount * acreage / acreageAll);
                    //Console.WriteLine("面积" + acreage + " ;人数" + polyAgents);
                    for (int n = 0; n < polyAgents; n++)
                    {
                        pointStart = songnav1.navMeshQuery.FindRandomPointOnPoly(songnav1.navMeshQuery.nav.IdManager.Encode(1, 0, j));//根据polyIndex对其编码输出该poly的ID
                        posStart = new Simulate.Vector2(pointStart.X, pointStart.Z);
                        //Console.WriteLine(positionIndex);
                        int outIndex;
                        if (positionIndex == 20 || Sample.isSelectNearest)//如果出口数字是20，代表还未设定的区域或者需要自己计算哪个出口位置最近就去哪个的区域，就重置一次最终目标 根据最近的出口是哪个(加上判断，如果当前目标的目标点是0,0,0，则重新寻找最近疏散目标点)
                        {
                            int min = OutIDs[0];
                            //float minVector2 = Simulate.MathHelper.abs(posStart - songnav1.level._out[0]);
                            float delta = Simulate.MathHelper.abs(posStart - songnav1.map._out[OutIDs[0]]);
                            for (int m = 0; m < OutIDs.Count; m++)
                            {
                                if (delta > Simulate.MathHelper.abs(songnav1.map._out[OutIDs[m]] - posStart))
                                {
                                    min =OutIDs[m];
                                    delta = Simulate.MathHelper.abs(songnav1.map._out[OutIDs[m]] - posStart);
                                }
                            }
                            //min = new Random().Next(nav.level._out.Count);
                            colorIndex = min;
                            Console.WriteLine("min "+min);
                            //NavPoint navP = nav.navMeshQuery.FindRandomPointAroundCircle(nav.navMeshQuery.FindNearestPoly(new Vector3(e.x_, 0, e.y_), new Vector3(10, 10, 10)), 5);
                            Vector3 navP1 = songnav1.navMeshQuery.FindRandomPointOnPoly(new NavPolyId(songnav1.navMeshQuery.FindNearestPoly(_out[min], new Vector3(20, 20, 20)).Polygon.Id));
                            Sample.outAgentsSetted[min]++;
                            posTarget = new Simulate.Vector2(navP1.X, navP1.Z);
                            outIndex = min;
                        }
                        else
                        {
                            Vector3 navP = songnav1.navMeshQuery.FindRandomPointOnPoly(new NavPolyId(songnav1.navMeshQuery.FindNearestPoly(_out[positionIndex], new Vector3(20, 20, 20)).Polygon.Id));
                            Sample.outAgentsSetted[positionIndex]++;
                            posTarget = new Simulate.Vector2(navP.X, navP.Z);
                            outIndex = positionIndex;
                        }
                        //var pointEnd = nav.navMeshQuery.FindRandomPoint();//随机选择位置，后面重新赋值
                        //posTarget = new Simulate.Vector2(pointEnd.Position.X, pointEnd.Position.Z);
                        //float randomSpeed = Simulate.MathHelper.RandomNormal(10, 1.2f, 0.8f, 1.6f);//这里超过1，只是计算距离有意义，速度不会超过1，除非改其他参数
                       
                        
                        _agents.Add(getNewAgent(posStart, posTarget, AgentStates.Enter, colorIndex, outIndex,recevieTime));//初始化agent编号、位置、目标、状态，根据目标设定颜色
                        //_agents.Add(new AgentClass(nov, posStart, posTarget, AgentStates.Enter));//初始化agent编号、位置、目标、状态
                    }
                }
            }
        }

        private AgentClass getNewAgent(Vector2 posStart, Vector2 posTarget, AgentStates enter, int colorIndex, int outIndex,int recevieTime)
        {
            //从人员配置中随机选择人物类型和其对应的速度与反应时间
            int r = MathHelper.random.Next(100);
            for(int i=0;i<Sample._peopleAttr.Count;i++)
            {
                if(r<Sample._peopleAttr[i].percentage)//这里已经假设人物属性里的占比最大累计数是递增
                {
                    float maxSpeed = MathHelper.RandomUniform(Sample._peopleAttr[i].speedMin, Sample._peopleAttr[i].speedMax);
                    int nov = RVOInstance.addAgent(new RVO.Vector2(posStart.x_, posStart.y_), Settings.RVODefault.neighborDist, Settings.RVODefault.maxNeighbors, Settings.RVODefault.timeHorizon, Settings.RVODefault.timeHorizonObst, Settings.RVODefault.radius, maxSpeed, new RVO.Vector2(1, 1));
                    int responseTime = 0;
                    if (Sample.responseDistribution == 0) responseTime = (int)(MathHelper.RandomUniform(Sample._peopleAttr[i].responseTimeMin, Sample._peopleAttr[i].responseTimeMax));//0代表平均分布
                    else responseTime = (int)(MathHelper.RandomNormal(Sample._peopleAttr[i].responseTimeMin, Sample._peopleAttr[i].responseTimeMax));//均匀分布

                    //根据当前位置更改反应时间
                    //responseTime+=(int)MathHelper.abs(new Vector2(posStart.x_ + 12, posStart.y_ - 12));
                    //Console.WriteLine("responseTime " + responseTime);
                    AgentClass newAgent = new AgentClass(nov, posStart, posTarget, enter, colorIndex, outIndex, maxSpeed,responseTime+recevieTime);
                    return newAgent;
                }
            }
            Console.WriteLine("添加人出错，属性没超过100");
            return new AgentClass(0, posStart, posTarget, enter, colorIndex, outIndex, 0,0);
        }

        #region 面积相关函数
        //song 求一个区域的面积,通过areaID
        public static float getAreaAcreage(NavTile tile, int areaID)
        {
            float acreage = 0;
            for (int j = 0; j < tile.Polys.Length; j++)
            {
                if (tile.Polys[j].Area.Id == areaID)
                {
                    acreage += getPolyAcreage(tile, tile.Polys[j]);
                }
            }
            return acreage;
        }
        //song 求一个poly的面积
        public static float getPolyAcreage(NavTile tile, NavPoly poly)
        {
            float acreage = 0;
            //开始遍历三角形，求出面积
            for (int j = 2; j < PathfindingCommon.VERTS_PER_POLYGON; j++)
            {
                if (poly.Verts[j] == 0)
                    break;

                int vertIndex0 = poly.Verts[0];
                int vertIndex1 = poly.Verts[j - 1];
                int vertIndex2 = poly.Verts[j];

                var v1 = tile.Verts[vertIndex0];
                var v2 = tile.Verts[vertIndex1];
                var v3 = tile.Verts[vertIndex2];

                float a = (v1 - v2).Length();
                float b = (v1 - v3).Length();
                float c = (v2 - v3).Length();

                float m = (a + b + c) / 2;
                acreage += (float)Math.Sqrt(m * (m - a) * (m - b) * (m - c));
            }
            return acreage;
        }

        public static float getPolyAcreageTest()
        {
            float acreage = 0;
            //开始遍历三角形，求出面积

            float a = 3;
            float b = 4;
            float c = 5;

            float m = (a + b + c) / 2;
            acreage += (float)Math.Sqrt(m * (m - a) * (m - b) * (m - c));

            return acreage;
        }
        #endregion
        #endregion

        #region 初始化RVO相关


        /// <summary>
        /// RVO初始化函数
        /// 设置步长等等参数，设置障碍物
        /// </summary>
        
        public List<Vector3> obsTest;
        private void InitRVO(List<Simulate.Line> obs)
        {
            ///* Specify the global time step of the simulation. */
            RVOInstance.setTimeStep(Settings.RVODefault.deltaT);
            RVOInstance.setAgentDefaults(Settings.RVODefault.neighborDist, Settings.RVODefault.maxNeighbors, Settings.RVODefault.timeHorizon, Settings.RVODefault.timeHorizonObst, Settings.RVODefault.radius, Settings.RVODefault.maxSpeed, new RVO.Vector2(1f, 1f));
            obsTest = new List<Vector3>();
#if outObsfile //王吉-输出障碍物边界，4个
            var FHObs=new FileHelper(Sample.mainDirectory + " " + instanceID +"obs.txt");
#endif
            //obs.Clear();
            //obs = songnav.GetObstacle();

            for (int i = 0; i < obs.Count; i++)
            {
                if (obs[i].valid == true)
                {
                    IList<RVO.Vector2> ObsVector = new List<RVO.Vector2>();
                    ObsVector.Add(new RVO.Vector2(obs[i].point1.X, obs[i].point1.Z));
                    ObsVector.Add(new RVO.Vector2(obs[i].point2.X, obs[i].point2.Z));
                    RVOInstance.addObstacle(ObsVector);

                    obsTest.Add(obs[i].point1);
                    obsTest.Add(obs[i].point2);

#if outObsfile
                    FHObs.Write(obs[i].point1.X + " " + obs[i].point1.Z + " " + obs[i].point2.X + " " + obs[i].point2.Z, true);
#endif
                }
            }
#if outObsfile
            //在out时可以先在底下设置断点
            FHObs.EndOut();
#endif
            RVOInstance.processObstacles();
        }

        /// <summary>
        /// 初始化障碍物
        /// </summary>
        private void InitObstacle()
        {
            IList<RVO.Vector2> ObsVector = new List<RVO.Vector2>();
            float x = 10;
            float z = 0;
            float sx = 10;
            float sz = 10;

            //Debug.Log(Obs.name);
            //Debug.Log((x + (sx * 0.5f))+ "  " +  (z + (sz * 0.5f)));
            //Debug.Log((x - (sx * 0.5f)) + "  " + (z + (sz * 0.5f)));
            //Debug.Log((x + (sx * 0.5f)) + "  " + (z - (sz * 0.5f)));
            //Debug.Log((x - (sx * 0.5f)) + "  " + (z - (sz * 0.5f)));

            ObsVector.Add(new RVO.Vector2(x + (sx * 0.5f), z + (sz * 0.5f)));
            ObsVector.Add(new RVO.Vector2(x - (sx * 0.5f), z + (sz * 0.5f)));
            ObsVector.Add(new RVO.Vector2(x - (sx * 0.5f), z - (sz * 0.5f)));
            ObsVector.Add(new RVO.Vector2(x + (sx * 0.5f), z - (sz * 0.5f)));

            RVOInstance.addObstacle(ObsVector);

            RVOInstance.processObstacles();
        }

        #endregion

        //方向控制
        //public class ControlGate
        //{
        //    public int poly1;
        //    public int poly2;
        //    public NavPolyId npi1;
        //    public NavPolyId npi2;
        //    public Vector3 pos1;
        //    public Vector3 pos2;
        //    public ControlGate(int p1, int p2)
        //    {
        //        poly1 = p1;
        //        poly2 = p2;
        //        npi1 = songnav.navMeshQuery.nav.IdManager.Encode(1, 0, poly1);
        //        npi2 = songnav.navMeshQuery.nav.IdManager.Encode(1, 0, poly2);
        //        pos1 = new Vector3();
        //        pos2 = new Vector3();
        //        songnav.navMeshQuery.CenterPointOnPoly(npi1, ref pos1);
        //        songnav.navMeshQuery.CenterPointOnPoly(npi2, ref pos2);
        //    }
        //    public void SwitchGate()
        //    {
        //        int poly = poly1;
        //        NavPolyId npi = npi1;
        //        Vector3 pos = pos1;

        //        poly1 = poly2;
        //        npi1 = npi2;
        //        pos1 = pos2;

        //        poly2 = poly;
        //        npi2 = npi;
        //        pos2 = pos;
        //    }
        //}



        //public void PathFindandCheck()
        //{
        //    //别忘重新安置这段代码
        //    //nav.roadfilter.SetAreaCost(2, 1000);
        //    //var tile = songnav.tiledNavMesh.GetTileAt(0, 0, 0);

        //    for (int i = 0; i < _agents.Count; i++)
        //    {
        //        SharpNav.Pathfinding.Path path = songnav.SmothPathfinding_paths(_agents[i].positionNow, _agents[i].positionTarget);
        //        #region 方向
        //        //控制方向的
        //        //for (int p = 0; p < path.polys.Count - 1; p++)
        //        //{
        //        //    for (int cg = 0; cg < controlGates.Count; cg++)
        //        //    {
        //        //        if(p>=path.polys.Count || cg>=controlGates.Count)
        //        //        {
        //        //            Console.WriteLine(" P太大 ");
        //        //        }
        //        //        else if (path.polys[p].Id == controlGates[cg].npi1.Id)
        //        //        {
        //        //            if (path.polys[p + 1].Id == controlGates[cg].npi2.Id)
        //        //            {
        //        //                tile.Polys[controlGates[cg].poly1].Area = new Area(2);
        //        //                path.Clear();
        //        //                path = nav.SmothPathfinding_paths(_agents[i].positionNow, _agents[i].positionTarget);

        //        //                //Console.WriteLine(i + " 逆向");
        //        //            }
        //        //        }
        //        //    }
        //        //    for (int cg = 0; cg < controlGates.Count; cg++)
        //        //    {
        //        //        tile.Polys[controlGates[cg].poly1].Area = new Area(1);
        //        //    }
        //        //}
        //        #endregion
        //        for (int pi = 0; pi < path.Count; pi++) _agents[i].polyIds.Add(path[pi].Id);
        //        //Console.WriteLine(instanceID+" poly点 "+i);
        //        _agents[i].navPoints = songnav.SmothPathfinding_Points();
        //        _agents[i].navPoints.Add(_agents[i].positionTarget);

        //        if (_agents[i].navPoints[0].x_ == 0 && _agents[i].navPoints[0].y_ == 0)
        //        {
        //            Console.WriteLine(instanceID + "编号 " + i + " 路径计算出错");
        //        }
        //        if (_agents[i].navPoints.Count > 2000)
        //        {
        //            Console.WriteLine(instanceID + "编号 " + i + " 路径过多 " + _agents[i].navPoints.Count);
        //        }
        //    }

        //}

        //public void RePathFindandCheck()
        //{
        //    //别忘重新安置这段代码
        //    //nav.roadfilter.SetAreaCost(2, 1000);
        //    //var tile = nav.tiledNavMesh.GetTileAt(0, 0, 0);
        //    //SharpNav.Pathfinding.Path path;
        //    try
        //    {
        //        for (int i = 0; i < _agents.Count; i++)
        //        {
        //            for (int p = 0; p < _agents[i].polyIds.Count - 1; p++)
        //            {
        //                for (int cg = 0; cg < controlGates.Count; cg++)
        //                {
        //                    if (_agents[i].polyIds[p] == controlGates[cg].npi1.Id)
        //                    {
        //                        if (_agents[i].polyIds[p + 1] == controlGates[cg].npi2.Id)
        //                        {
        //                            //tile.Polys[controlGates[cg].poly1].Area = new Area(2);
        //                            //path.Clear();
        //                            songnav.SmothPathfinding_paths(_agents[i].positionNow, _agents[i].positionTarget);
        //                            _agents[i].navPoints = songnav.SmothPathfinding_Points();
        //                            _agents[i].navPoints.Add(_agents[i].positionTarget);
        //                        }
        //                    }
        //                }
        //                //for (int cg = 0; cg < controlGates.Count; cg++)
        //                //{
        //                //    tile.Polys[controlGates[cg].poly1].Area = new Area(1);
        //                //}
        //            }
        //        }
        //    }
        //    catch
        //    {

        //    }
        //}

        #region 读取相关的函数
        private static readonly char[] lineSplitChars = { ' ' };
        //public void Read()
        //{
        //    string path = instanceID + "output.txt";
        //    //string path = "output.txt";
        //    sw.Restart();
        //    using (StreamReader reader = new StreamReader(path))
        //    {
        //        List<string> list = new List<string>();
        //        string l;
        //        while ((l = reader.ReadLine()) != null)
        //        {
        //            list.Add(l);
        //        }
        //        reader.Close();

        //        try
        //        {
        //            foreach (string frame in list)
        //            {
        //                sw.Restart();
        //                //trim any extras
        //                string tl = frame;
        //                string[] line = tl.Split(lineSplitChars, StringSplitOptions.RemoveEmptyEntries);
        //                if (line == null || line.Length == 0)
        //                    continue;

        //                lock (_agents)
        //                {
        //                    _agents.Clear();
        //                    for (int i = 0; i < line.Length - 2; i += 3)
        //                    {
        //                        try
        //                        {
        //                            _agents.Add(new AgentClass(_agents.Count, new Simulate.Vector2(float.Parse(line[i + 1]) / 100, float.Parse(line[i + 2]) / 100), new Simulate.Vector2(0, 0), AgentStates.Enter, int.Parse(line[i]), 0));//出口位置无所谓，写成0
        //                        }
        //                        catch
        //                        {
        //                            Console.WriteLine("超过：" + i);
        //                        }

        //                    }
        //                }

        //                Console.WriteLine((float)sw.ElapsedMilliseconds / 1000 + " 秒");
        //                //Thread.Sleep(200);
        //                //mainWindow.DrawCrowd();
        //                //mainWindow.SwapBuffers();
        //            }
        //        }
        //        catch
        //        {
        //        }
        //    }
        //}

        public int GetFrameCount()
        {
            int lines = 0;
            using (var readers = new StreamReader(pathReading))
            {
                while (readers.ReadLine() != null)
                {
                    lines++;
                }
                readers.Close();
            }
            return lines;
        }

        public void ReadInit()
        {
            //初始化读取路径
            pathReading= Sample.mainDirectory + instanceID + ".txt";
            sw.Restart();
            reader = new StreamReader(pathReading);
            //using (StreamReader reader = new StreamReader(path))
            //{
            //    //List<string> list = new List<string>();
            //    string l;
            //    while ((l = reader.ReadLine()) != null)
            //    {
            //        list.Add(l);
            //    }
            //    reader.Close();
            //}
        }
        //public void ReadLineNoThread()
        //{
        //    Console.WriteLine((float)sw.ElapsedMilliseconds / 1000 + " 秒2");
        //    sw.Restart();
        //    string l;
        //    Console.WriteLine(instanceID + "new");
        //    if ((l = reader.ReadLine()) != null)
        //    {
        //        try
        //        {

        //            //trim any extras
        //            //string tl = list[frame++];
        //            string[] line = l.Split(lineSplitChars, StringSplitOptions.RemoveEmptyEntries);
        //            if (line == null || line.Length == 0)
        //                return;

        //            //lock (_agents)
        //            {
        //                _agents.Clear();

        //                //先加上当前每个出口的输出人数
        //                for (int i = 0; i < _outAgentCount.Length; i++)
        //                {
        //                    Sample.outAgentsSetted[i] = int.Parse(line[i]);
        //                }
        //                for (int j = _outAgentCount.Length; j < _outAgentCount.Length * 2; j++)
        //                {
        //                    _outAgentCount[j - _outAgentCount.Length] = int.Parse(line[j]);
        //                }

        //                for (int i = _outAgentCount.Length * 2; i < line.Length - 2; i += 3)
        //                {
        //                    try
        //                    {
        //                        _agents.Add(new AgentClass(_agents.Count, new Simulate.Vector2(float.Parse(line[i + 1]) / 100, float.Parse(line[i + 2]) / 100), new Simulate.Vector2(0, 0), AgentStates.Enter, int.Parse(line[i]), 0));//出口位置无所谓，写成0
        //                    }
        //                    catch
        //                    {
        //                        Console.WriteLine("超过：" + i);
        //                    }
        //                }
        //            }
        //            Console.WriteLine((float)sw.ElapsedMilliseconds / 1000 + " 秒1");
        //            sw.Restart();

        //            //Thread.Sleep(200);
        //            List<List<AgentClass>> _agentss = new List<List<AgentClass>>();
        //            _agentss.Add(_agents);
        //            Window.HeatMap.CalculateColor(ref _agentss);
        //        }
        //        catch
        //        {
        //        }
        //    }
        //}

        public void ReadLineNoThread(int linesIndex = 0, int multiple=1)
        {
            //Console.WriteLine((float)sw.ElapsedMilliseconds / 1000 + " 秒2");
            sw.Restart();
            string l;
            //Console.WriteLine(instanceID + "new");

            bool flag = true;
            
            //偏移文件指针位置
            if (linesIndex != 0)
            {
                /*Console.WriteLine("-----------------" + linesIndex+"count:"+linesCount);
                int offset = linesIndex > linesCount ? 1 : -1;
                Console.WriteLine(offset);
                while(linesCount!=linesIndex)
                {
                    reader.BaseStream.Seek(offset, SeekOrigin.Current);
                    if((char)reader.Peek()=='\n')
                    {
                        linesIndex += offset;
                    }
                }
                reader.ReadLine();*/
                if (linesIndex > linesCount)
                {
                    while (linesIndex != linesCount)
                    {
                        if ((reader.ReadLine()) == null) return;
                        else linesCount++;
                    }
                    flag = false;
                }
                else
                {
                    //reader.Close();
                    //reader.Dispose();
                    //string path = Sample.mainDirectory + "\\" + instanceID + "output.txt";
                    //reader = new StreamReader(pathReading);
                    reader.ReadToEnd();
                    reader.BaseStream.Seek(0, SeekOrigin.Begin);
                    linesCount = 1;
                    if(linesIndex!=1)ReadLineNoThread(linesIndex, multiple);
                    else if(linesIndex==1) ReadLineNoThread(0, multiple);
                }
            }


            _agents.Clear();
            if (reader!=null && (l = reader.ReadLine()) != null)
            {
                linesCount++;
                while (--multiple > 0)
                {
                    l = reader.ReadLine();
                    if (l != null) linesCount++;
                    else return;
                }
                //if (flag) linesCount++; 不明白
                try
                {

                    //trim any extras
                    //string tl = list[frame++];
                    string[] line = l.Split(lineSplitChars, StringSplitOptions.RemoveEmptyEntries);
                    if (line == null || line.Length == 0)
                        return;

                    //lock (_agents)
                    {
                        //_agents.Clear();

                        //先加上当前每个出口的输出人数
                        for (int i = 0; i < _outAgentCount.Length; i++)
                        {
                            Sample.outAgentsSetted[i] = int.Parse(line[i]);
                        }
                        for (int j = _outAgentCount.Length; j < _outAgentCount.Length * 2; j++)
                        {
                            _outAgentCount[j - _outAgentCount.Length] = int.Parse(line[j]);
                        }

                        for (int i = _outAgentCount.Length * 2; i < line.Length - 2; i += 3)
                        {
                            try
                            {
                                _agents.Add(new AgentClass(_agents.Count, new Simulate.Vector2(float.Parse(line[i + 1]) / 100, float.Parse(line[i + 2]) / 100), new Simulate.Vector2(0, 0), AgentStates.Evacuating, int.Parse(line[i]), 0,0,0));//出口位置无所谓，写成0
                            }
                            catch
                            {
                                Console.WriteLine("超过：" + i);
                            }
                        }
                    }
                    //Console.WriteLine((float)sw.ElapsedMilliseconds / 1000 + " 秒1");
                    sw.Restart();

                    //Thread.Sleep(200);
                    List<List<AgentClass>> _agentss = new List<List<AgentClass>>();
                    _agentss.Add(_agents);
                    Window.HeatMap.CalculateColor(ref _agentss,0,false);
                }
                catch
                {
                }
            }
        }
        #endregion

        #region 速度计算与位置更新相关
        /// <summary>
        /// 用来计算agent下一步位置并在场景中更新显示
        /// </summary>
        static int maxNavSteps = 0;//仿真多少步后重新计算导航点
        long steptimebefore = 0;
        public float speedMaxTest = 0f;//速度
        //public float agentRoadLength = 0;
        public bool controlSteps = true;
        public bool continueSteps = true;

        public void Intervals()
        {
            //Repeat(UpdatePosition, 0);//每步仿真

            //Repeat(CheckDelete, 1000);//每步仿真
            //Decisions.DecideFromAgent(_agents);
            sw.Restart();
            stepsThread = new Thread(UpdatePosition);
            stepsThread.Start();
            //while (true)
            //{
            //    UpdatePosition();
            //}

            //Console.WriteLine("make decisions");
        }

        public void outputAgents()
        {
            if (Sample.outpositionfileMode)
            {
                int chushu = Sample.numsPeople >= 50000 ? 10 : 2;//一秒一帧和0.2秒一帧
                if (stepCounts % chushu == 0 && _agents.Count > 0)
                {
                    //写入一帧帧头  song
                    FH.NewFrame(_agents.Count.ToString());
                    long timebefore = sw.ElapsedMilliseconds;

                    //写入每个口的当前疏散个数
                    for (int i = 0; i < Instance._outAgentCount.Length; i++)
                    {
                        FH.Write(Sample.outAgentsSetted[i] + " ");
                    }
                    for (int i = 0; i < Instance._outAgentCount.Length; i++)
                    {
                        FH.Write(_outAgentCount[i] + " ");
                    }

                    //写入一帧所有数据
                    for (int i = 0; i < _agents.Count; i++)
                    {
                        if (_agents[i].state != AgentStates.Hide)
                        {
                            RVO.Vector2 pos = RVOInstance.getAgentPosition(i);//
                            _agents[i].positionNow = new Simulate.Vector2(pos.x(), pos.y());
                            if (Double.IsNaN(pos.x())) Console.WriteLine("不是数！" + "nov:" + _agents[i].nov + "; agent[i]:" + i);
                            else
                            {
                                //位置输出到文件
                                //FileHelper.Write("/" + _agents[i].nov + " " + _agents[i].positionNow.x_ + " " + _agents[i].positionNow.y_ + " 0");//最后的0是预留的
                                //FileHelper.Write("/" + i + " " + (RVOInstance.getAgentPosition(i).x_ * 100).ToString("f1") + " " + (RVOInstance.getAgentPosition(i).y_ * 100).ToString("f1") + " 0");//最后的0是预留的

                                //FileHelper.Write("/" + i + " " + (RVOInstance.getAgentPosition(i).x_).ToString("f1") + "," + (RVOInstance.getAgentPosition(i).y_).ToString("f1") + " 0");//最后的0是预留的
                                FH.Write(_agents[i].colorIndex + " " + (RVOInstance.getAgentPosition(i).x() * 100).ToString("f0") + " " + (RVOInstance.getAgentPosition(i).y() * 100).ToString("f0") + " ");//最后的0是预留的
                                _agents[i].positions.Add(new Simulate.Vector2((int)(pos.x()*100), (int)(pos.y()*100)));                                                                                                                                                                            //Console.WriteLine("/" + _agents[i].nov + " " + RVOInstance.getAgentPosition(i).x_.ToString("f1") + " " + RVOInstance.getAgentPosition(i).y_.ToString("f1") + " 0");
                            }
                        }
                    }
                    //Console.WriteLine("储存一帧总时间： " + (sw.ElapsedMilliseconds - timebefore).ToString());
                    FH.NewLine();//换行
                }
                if (FH.writeflame > 50000) OutpositionFileEnd();//3万步，就是30000/10/60=50分钟
            }


            //if (Sample.outAgentsFile) 以前的方法，每个区域都要输出一次，所以弃用了
            //{
            //    if (maxNavSteps++ >= 80)
            //    {
            //        //输出agent信息文件1
            //        FileHelper.SetOutPath("agents.txt");
            //        for (int i = 0; i < _agents.Count; i++)
            //        {
            //            if (_agents[i].navPoints.Count > 100)
            //            {
            //                for (int j = 0; j < _agents[i].navPoints.Count; j++)
            //                {
            //                    if (_agents[i].navPoints[j].x_ == 0 && _agents[i].navPoints[j].y_ == 0)
            //                    {
            //                        _agents[i].navPoints.RemoveAt(j--);
            //                    }
            //                }
            //            }
            //            //FileHelper.Write(_agents[i].positionNow.x_ + " " + _agents[i].positionNow.y_ + " " + _agents[i].navPoints.Count + " ");
            //            FileHelper.Write(_agents[i].colorIndex + " " + RVOInstance.getAgentPosition(i).x() + " " + RVOInstance.getAgentPosition(i).y() + " " + _agents[i].navPoints.Count + " ");
            //            for (int j = 0; j < _agents[i].navPoints.Count; j++) FileHelper.Write(_agents[i].navPoints[j].x_.ToString("0.00") + " " + _agents[i].navPoints[j].y_.ToString("0.00") + " ");
            //            FileHelper.NewLine();
            //        }
            //        FileHelper.EndOut();
            //    }

        }

        public void nextTarget()
        {
            //如果到达导航点，就到下一个目标
            for (int i = 0; i < _agents.Count; i++)
            {

                if (_agents[i].navPoints.Count > 0 && _agents[i].navPoints[0].x_ == 0 && _agents[i].navPoints[0].y_ == 0)
                {
                    ////方法1, 删除
                    ////删掉目标为0,0的agent
                    ////for (int n = 0; n < songnav1.level._out.Count; n++)
                    //{
                    //    //if (i < _agents.Count && MathHelper.abs(songnav1.level._out[n] - _agents[i].positionNow) < 15)
                    //    {
                    //        RVOInstance.delAgentAt(i);
                    //        _agents.RemoveAt(i);
                    //        //_outAgentCount[n]++;
                    //        Console.WriteLine(i+"目标为0, 移除");
                    //    }
                    //}
                    //continue;

                    //方法2, 重新规划
                    //_agents[i].navPoints.RemoveAt(0);
                    ////判断是否后面都为0 如果是就都移除
                    //for (int iii = 0; iii < _agents[i].navPoints.Count; iii++)
                    //{
                    //    if (_agents[i].navPoints[0].x_ == 0 && _agents[i].navPoints[0].y_ == 0)
                    //    {
                    //        _agents[i].navPoints.RemoveAt(0);
                    //    }
                    //}

                    Console.WriteLine(instanceID + "编号 " + i + " 出错，重新规划1");
                    ChangeTarget(_agents[i]);//根据当前在的poly位置重新寻径
                    var path = songnav1.SmothPathfinding_paths(_agents[i].positionNow, _agents[i].positionTarget);
                    _agents[i].navPoints.Clear();
                    _agents[i].navPoints = songnav1.SmothPathfinding_Points(path);

                    //_agents[i].navPoints = nav.SmothPathfinding(_agents[i].positionNow, _agents[i].positionTarget);
                    //把最后位置的目标也加进去
                    var outPoint = _out[_agents[i].outIndex];
                    _agents[i].navPoints.Add(new Vector2(outPoint.X, outPoint.Z));

                }

                if (_agents[i].navPoints.Count > 1)
                {
                    #region 原来那种判断距离移除目标点的方式
                    if (Simulate.MathHelper.abs(_agents[i].positionNow - _agents[i].navPoints[0]) < 6f)//不知道为什么会有很多点导向原点，因此添加这个删除他
                    {

                        if (_agents[i].navPoints.Count > 1) _agents[i].navPoints.RemoveAt(0);
                        //for (int iii = 0; iii < _agents[i].navPoints.Count - 1; iii++)
                        //{
                        //    if (_agents[i].navPoints[iii].x_ == 0 && _agents[i].navPoints[iii].y_ == 0)
                        //    {
                        //        _agents[i].navPoints.RemoveAt(iii);
                        //    }
                        //    if (iii > 2 && iii < 20 && MathHelper.abs(_agents[i].navPoints[iii] - _agents[i].navPoints[iii - 1]) < 5)
                        //    {
                        //        _agents[i].navPoints.RemoveAt(iii);
                        //        iii--;
                        //    }
                        //}

                    }
                    else if (Simulate.MathHelper.abs(_agents[i].positionNow - _agents[i].navPoints[0]) > 15f)//超过8米时重新规划
                    {

                        //多个线程时重新寻径会出错，不知道为什么！！
                        ChangeTarget(_agents[i]);//根据当前在的poly位置重新寻径
                                                 //ChangeTarget();
                        var path = songnav1.SmothPathfinding_paths(_agents[i].positionNow, _agents[i].positionTarget);
                        _agents[i].navPoints.Clear();
                        _agents[i].navPoints = songnav1.SmothPathfinding_Points(path);

                        //for (int iii = 0; iii < _agents[i].navPoints.Count - 1; iii++)
                        //{
                        //    if (_agents[i].navPoints[iii].x_ == 0 && _agents[i].navPoints[iii].y_ == 0)
                        //    {
                        //        _agents[i].navPoints.RemoveAt(iii);
                        //    }
                        //    if (iii > 2 && MathHelper.abs(_agents[i].navPoints[iii] - _agents[i].navPoints[iii - 1]) < 2)
                        //    {
                        //        _agents[i].navPoints.RemoveAt(iii);
                        //        iii--;
                        //    }
                        //}

                        //_agents[i].navPoints = nav1.SmothPathfinding(_agents[i].positionNow, _agents[i].positionTarget);
                        //_agents[i].navPoints.Add(_agents[i].positionTarget);

                        ////把最后位置的目标也加进去
                        var outPoint = _out[_agents[i].outIndex];
                        _agents[i].navPoints.Add(new Vector2(outPoint.X, outPoint.Z));



                        Console.WriteLine(i + " 重新规划2 " + _agents[i].haveReplaned);

                        _agents[i].haveReplaned = true;

                    }
                    #endregion

                    ////新的方法移除当前目标点
                    //if(MathHelper.absSq(_agents[i].positionNow - _agents[i].navPoints[0])+ MathHelper.absSq(_agents[i].navPoints[0] - _agents[i].navPoints[1])> MathHelper.absSq(_agents[i].positionNow - _agents[i].navPoints[1]))
                    //{
                    //    _agents[i].navPoints.RemoveAt(0);
                    //}
                }

            }
        }

        public void UpdatePosition()
        {
            while (continueSteps)
            {
                //Thread.Sleep(20);//防止过快,价格延时
                while (controlSteps)
                {

                    //Thread.Sleep(100);
                    //Console.WriteLine("XXXXXXXXXXXXXXXXXXXXXXXXXXXXinstanceID: " + this.instanceID);
                    SetPreferredVelocities();
                    RVOInstance.doStep();
                    //Thread.Sleep(10);

                    outputAgents();

                    nextTarget();
                    
                    //////暂时不进行重新规划
                    ////if (maxNavSteps++ > 30)
                    ////{
                    ////    maxNavSteps = 0;
                    ////    if (_agents.Count > 0)
                    ////    {
                    ////        Thread thread = new Thread(ReplanNav);
                    ////        thread.Start();
                    ////    }
                    ////}
                    
                    CheckDelete();
                    
                    //统计
                    if (_agents.Count > 0) stepCounts++;
                    if (stepCounts % 10 == 0)
                    {
                        //Console.WriteLine("总人数： " + _agents.Count);
                        //Console.WriteLine("10步耗时： " + ((float)(sw.ElapsedMilliseconds-steptimebefore)) / 1000);
                        //steptimebefore = sw.ElapsedMilliseconds;

                        //infectAgents();
                    }
                    //if (_agents.Count < (agentsOrigine / 50))//剩余百分之XX的时候
                    //{
                    //    ////Console.WriteLine("总步数： " + stepCounts);
                    //    ////Console.WriteLine("总耗时： " + sw.ElapsedMilliseconds / 1000);
                    //    //sw.Stop();
                    //    //steps.Abort();

                    //}
                    if (stepCounts % 10 == 0 && instanceID == "output2")  //if (stepCounts % 60 == 0 && _agents.Count > 1200)
                    {
                        //暂时只输出instance2的
                        
                        Console.Write("\t" + instanceID + "总步数： " + stepCounts / 10);
                        Console.Write("\t" + instanceID + "总人数： " + _agents.Count);
                        Console.Write("\t" + instanceID + "总耗时： " + ((float)(sw.ElapsedMilliseconds)) / 1000);
                        Console.WriteLine("\t" + instanceID + "10步耗时： " + ((float)(sw.ElapsedMilliseconds - steptimebefore)) / 1000);


#if outDebug
                    FileHelper.Write("\t" + instanceID + "总步数： " + stepCounts / 60);
                    FileHelper.Write("\t" + instanceID + "总人数： " + _agents.Count);
                    FileHelper.Write("\t" + instanceID + "总耗时： " + ((float)(sw.ElapsedMilliseconds)) / 1000);
                    FileHelper.Write("\t" + instanceID + "100步耗时： " + ((float)(sw.ElapsedMilliseconds - steptimebefore)) / 1000,true);
#endif

                        steptimebefore = sw.ElapsedMilliseconds;
                    }
                    controlSteps = false;
                }
            }
                

        }

        public void infectAgents()
        {
            //for(int i=0;i<_agents.Count;i++)
            //{

            //}

            //更改状态  舍弃掉通过自带KD树得到邻居的方法
            //弄一个表，如果当前agent是新改变的状态，就让其不搜索
            //对所有
            for (int agenti = 0; agenti < _agents.Count; agenti++)
            {
                _agents[agenti].newStateChanged = false;
            }
            int count = 0;
            for (int i = 0; i < _agents.Count; i++)
            {
                if (_agents[i].newStateChanged == false && _agents[i].state == AgentStates.Evacuating)//如果没有发生新的颜状态变化
                {
                    for (int j = 0; j < 3; j++)
                    {
                        int id = RVOInstance.getAgentAgentNeighbor(_agents[i].nov, j);
                        if (id < _agents.Count)
                        {
                            _agents[id].state = AgentStates.Evacuating;
                            _agents[id].newStateChanged = true;
                            count++;
                        }
                    }
                }
            }
            Console.WriteLine("count: "+count);
        }

        private void ChangeTarget(AgentClass agent)
        {
            //找到当前位置的poly
            //找到当前poly的area
            //根据area得到POS ，根据pos1得到目标

            var navpoint = songnav1.navMeshQuery.FindNearestPoly(new Vector3(agent.positionNow.x_, 0, agent.positionNow.y_), new Vector3(10, 10, 10));
            var navpolyid = new NavPolyId(navpoint.Polygon.Id);

            NavTile tile;
            NavPoly poly;
            songnav1.navMeshQuery.nav.TryGetTileAndPolyByRef(navpolyid, out tile, out poly);
            foreach (var areas in Sample._areas)
            {
                foreach (var area in areas)
                {
                    if (poly.Area.Id == area.Id)
                    {
                        if (area.pos1 == 20 || Sample.isSelectNearest)//如果出口数字是20，代表还未设定的区域或者需要自己计算哪个出口位置最近就去哪个的区域，就重置一次最终目标 根据最近的出口是哪个(加上判断，如果当前目标的目标点是0,0,0，则重新寻找最近疏散目标点)
                        {
                            var posStart = agent.positionNow;
                            int min = OutIDs[0];
                            //float minVector2 = Simulate.MathHelper.abs(posStart - songnav1.level._out[0]);

                            //float delta = Simulate.MathHelper.abs(posStart - songnav1.level._out[0]);
                            //for (int m = 0; m < songnav1.level._out.Count; m++)
                            //{
                            //    if (delta > Simulate.MathHelper.abs(songnav1.level._out[m] - posStart))
                            //    {
                            //        min = m;
                            //        delta = Simulate.MathHelper.abs(songnav1.level._out[m] - posStart);
                            //    }
                            //}

                            float delta = Simulate.MathHelper.abs(posStart - songnav1.map._out[OutIDs[0]]);
                            for (int m = 0; m < OutIDs.Count; m++)
                            {
                                if (delta > Simulate.MathHelper.abs(songnav1.map._out[OutIDs[m]] - posStart))
                                {
                                    min = OutIDs[m] ;
                                    delta = Simulate.MathHelper.abs(songnav1.map._out[OutIDs[m]] - posStart);
                                }
                            }

                            var p = _out[min];
                            Vector3 navP = songnav1.navMeshQuery.FindRandomPointOnPoly(new NavPolyId(songnav1.navMeshQuery.FindNearestPoly(p, new Vector3(20, 20, 20)).Polygon.Id));
                            agent.outIndex = min;
                            agent.positionTarget = new Simulate.Vector2(navP.X, navP.Z);
                            //agent.positionTarget = new Simulate.Vector2(p.X, p.Z);
                            return;
                        }
                        else
                        {
                            var p = _out[area.pos1];
                            agent.outIndex = area.pos1;
                            Vector3 navP = songnav1.navMeshQuery.FindRandomPointOnPoly(new NavPolyId(songnav1.navMeshQuery.FindNearestPoly(p, new Vector3(20, 20, 20)).Polygon.Id));
                            agent.positionTarget = new Simulate.Vector2(navP.X, navP.Z);
                            //agent.positionTarget = new Simulate.Vector2(p.X, p.Z);
                            return;
                        }

                    }
                }
            }
        }

        /// <summary>
        /// 根据agent当前位置与目的地计算设置最佳速度矢量，每次更新位置需要调用
        /// </summary>
        void SetPreferredVelocities()
        {
            /*
                * Set the preferred velocity to be a vector of unit magnitude
                * (speed) in the direction of the goal.
                */
            //for (int i = 0; i < RVOInstance.getNumAgents(); ++i)
            for (int i = 0; i < _agents.Count; ++i)
            {


                if (_agents[i].state == AgentStates.Evacuating && _agents[i].timeResponse - Settings.deltaTDefault * stepCounts < 0)
                {
                    RVO.Vector2 goalVector;

                    //给agent的属性赋值，方便计算path
                    RVO.Vector2 pos = RVOInstance.getAgentPosition(i);
                    _agents[i].positionNow = new Simulate.Vector2(pos.x(), pos.y());

                    if (_agents[i].navPoints.Count > 0)
                    {
                         goalVector = new RVO.Vector2(_agents[i].navPoints[0].x_, _agents[i].navPoints[0].y_) - RVOInstance.getAgentPosition(i);
                        
                    }
                    else goalVector = new RVO.Vector2(0, 0);
                    if (RVOMath.absSq(goalVector) > 1.0f)
                    {
                        goalVector = RVOMath.normalize(goalVector);
                    }

                    ////乘以一个系数让其接近人行走速度
                    //goalVector *= 1f;//人快走的速度

                    //goalVector.x_ += _agents[i].ControlSpeedx;
                    //goalVector.y_ += _agents[i].ControlSpeedy;
                    //goalVector *= _agents[i].ControlSpeed;

                    //float offsetx=ClaculateX(goalVector.x_,goalVector.y_,_agents[i].ControlSpeedx,_agents[i].ControlSpeedy);
                    //float offsety = ClaculateY(goalVector.x_, goalVector.y_, _agents[i].ControlSpeedx, _agents[i].ControlSpeedy,offsetx);
                    //goalVector.x_ += offsetx;
                    //goalVector.y_ += offsety;

                    RVOInstance.setAgentMaxSpeed(i, _agents[i].ControlSpeed);
                    RVOInstance.setAgentPrefVelocity(i, goalVector);
                    
                    

                    /* Perturb a little to avoid deadlocks due to perfect symmetry. */
                    float angle = (float)Simulate.MathHelper.random.NextDouble() * 2.0f * (float)Math.PI;
                    float dist = (float)Simulate.MathHelper.random.NextDouble() * 0.001f;

                    RVOInstance.setAgentPrefVelocity(i, RVOInstance.getAgentPrefVelocity(i) +
                        dist * new RVO.Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)));
                }
            }

        }

        public float ClaculateX(float x1, float y1, float x2, float y2)
        {
            return (x2 * y1 * y1 - x1 * y1 * y2) / (y1 * y1 + x1 * x1);
        }
        public float ClaculateY(float x1, float y1, float x2, float y2, float x3)
        {
            return y2-y1*(x2-x3)/x1;
        }

        public List<List<Vector2>> agentsPositionsforExodus = new List<List<Vector2>>();
        public void CheckDelete()
        {
            for (int i = 0; i < _agents.Count; i++)
            {
                for (int n = 0; n < songnav1.map._out.Count; n++)
                {
                    //try
                    {
                        if (i<_agents.Count &&MathHelper.abs(songnav1.map._out[n] - _agents[i].positionNow) < 15)
                        {
                            agentsPositionsforExodus.Add(_agents[i].positions);
                            RVOInstance.delAgentAt(i);
                            _agents.RemoveAt(i);
                            _outAgentCount[n]++;
                        }
                    }
                    //catch
                    {

                    }
                }
                //if (_agents[i].navPoints.Count < 7)
                //{
                //    RVOInstance.delAgentAt(i);
                //    _agents.RemoveAt(i);
                //    //Console.WriteLine("del " + i);
                //}
            }
        }

        #endregion

        #region 寻径相关的函数

        //public void PathFind()
        //{
        //    sw.Start();
        //    //计算路径 第一次赋初始位置和目标位置时计算一次
        //    Console.WriteLine("开始计算路径");

        //    //Thread T1 = new Thread(PathFindandCheck);
        //    //T1.Start(_agents);
        //    //while (T1.IsAlive) ;
        //    PathFindandCheck();
        //    Console.WriteLine("计算路径结束");
        //    Console.WriteLine("初始化人物耗时： " + sw.ElapsedMilliseconds / 1000);
        //    sw.Stop();
        //}
        public int pathFindedNums;
        //public void PathFindandCheck()//(Object obj)
        //{
        //    //List<AgentClass> agents = (List<AgentClass>)obj;
        //    List<AgentClass> agents = _agents;
        //    //别忘重新安置这段代码
        //    //nav.roadfilter.SetAreaCost(2, 1000);
        //    //var tile = songnav.tiledNavMesh.GetTileAt(0, 0, 0);

        //    pathFindedNums = 0;
        //    for (int i = 0; i < agents.Count; i++)
        //    {
        //        pathFindedNums = i;
        //        SharpNav.Pathfinding.Path path = songnav.SmothPathfinding_paths(agents[i].positionNow, agents[i].positionTarget);
        //        //SharpNav.Pathfinding.Path path1 = songnav.SmothPathfinding_paths(agents[i-1].positionNow, agents[i-1].positionTarget);
        //        #region 方向
        //        //控制方向的
        //        //for (int p = 0; p < path.polys.Count - 1; p++)
        //        //{
        //        //    for (int cg = 0; cg < controlGates.Count; cg++)
        //        //    {
        //        //        if(p>=path.polys.Count || cg>=controlGates.Count)
        //        //        {
        //        //            Console.WriteLine(" P太大 ");
        //        //        }
        //        //        else if (path.polys[p].Id == controlGates[cg].npi1.Id)
        //        //        {
        //        //            if (path.polys[p + 1].Id == controlGates[cg].npi2.Id)
        //        //            {
        //        //                tile.Polys[controlGates[cg].poly1].Area = new Area(2);
        //        //                path.Clear();
        //        //                path = nav.SmothPathfinding_paths(_agents[i].positionNow, _agents[i].positionTarget);

        //        //                //Console.WriteLine(i + " 逆向");
        //        //            }
        //        //        }
        //        //    }
        //        //    for (int cg = 0; cg < controlGates.Count; cg++)
        //        //    {
        //        //        tile.Polys[controlGates[cg].poly1].Area = new Area(1);
        //        //    }
        //        //}
        //        #endregion

        //        for (int pi = 0; pi < path.Count; pi++) agents[i].polyIds.Add(path[pi].Id);
        //        //Console.WriteLine(instanceID+" poly点 "+i);
        //        agents[i].navPoints = songnav.SmothPathfinding_Points(path);

        //        //for (int iii = 0; iii < _agents[i].navPoints.Count - 1; iii++)
        //        //{
        //        //    if (_agents[i].navPoints[iii].x_ == 0 && _agents[i].navPoints[iii].y_ == 0)
        //        //    {
        //        //        _agents[i].navPoints.RemoveAt(iii);
        //        //    }
        //        //    if (iii > 2 && MathHelper.abs(_agents[i].navPoints[iii]-_agents[i].navPoints[iii-1])<5)
        //        //    {
        //        //        _agents[i].navPoints.RemoveAt(iii);
        //        //        iii--;
        //        //    }
        //        //}

        //        //agents[i].navPoints.Add(agents[i].positionTarget);
        //        //把最后位置的目标也加进去
        //        var outPoint = _out[agents[i].outIndex];
        //        agents[i].navPoints.Add(new Vector2(outPoint.X, outPoint.Z));
        //        ////如果是在出口10，则把最后的目标点也加上
        //        //Vector2 out11 = new Vector2(215.21f,11.3f);
        //        //if( MathHelper.abs(agents[i].positionTarget-out11)<50)
        //        //{
        //        //    agents[i].navPoints.Add(out11);
        //        //}

        //        if (agents[i].navPoints[0].x_ == 0 && agents[i].navPoints[0].y_ == 0)
        //        {
        //            Console.WriteLine(instanceID + "编号 " + i + " 路径计算出错");
        //        }
        //        if (agents[i].navPoints.Count > 2000)
        //        {
        //            Console.WriteLine(instanceID + "编号 " + i + " 路径过多 " + agents[i].navPoints.Count);
        //        }
        //    }
        //}


        public void PathFind()
        {
            sw.Start();
            //计算路径 第一次赋初始位置和目标位置时计算一次
            Console.WriteLine("开始计算路径");

            List<AgentClass> agents1=new List<AgentClass>();
            for(int i=0;i<_agents.Count/2;i++)
            {
                agents1.Add(_agents[i]);
            }
            List<AgentClass> agents2 = new List<AgentClass>();
            for (int i = _agents.Count / 2; i < _agents.Count; i++)
            {
                agents2.Add(_agents[i]);
            }
            pathFindedNums = 0;
            Thread T1 = new Thread(PathFindandCheck1);
            T1.Start(agents1);
            Thread T2 = new Thread(PathFindandCheck2);
            T2.Start(agents2);
            while (T1.IsAlive);
            while (T2.IsAlive) ;
            Console.WriteLine("计算路径结束");
            Console.WriteLine("初始化人物耗时： " + sw.ElapsedMilliseconds / 1000);
            sw.Stop();
        }
        public void PathFindandCheck1(Object obj)
        {
            List<AgentClass> agents = (List<AgentClass>)obj;
            //List<AgentClass> agents = _agents;
            //别忘重新安置这段代码
            //nav.roadfilter.SetAreaCost(2, 1000);
            //var tile = songnav.tiledNavMesh.GetTileAt(0, 0, 0);

            
            for (int i = 0; i < agents.Count; i++)
            {
                pathFindedNums++;
                SharpNav.Pathfinding.Path path = songnav1.SmothPathfinding_paths(agents[i].positionNow, agents[i].positionTarget);
                //SharpNav.Pathfinding.Path path1 = songnav.SmothPathfinding_paths(agents[i-1].positionNow, agents[i-1].positionTarget);
                #region 方向
                //控制方向的
                //for (int p = 0; p < path.polys.Count - 1; p++)
                //{
                //    for (int cg = 0; cg < controlGates.Count; cg++)
                //    {
                //        if(p>=path.polys.Count || cg>=controlGates.Count)
                //        {
                //            Console.WriteLine(" P太大 ");
                //        }
                //        else if (path.polys[p].Id == controlGates[cg].npi1.Id)
                //        {
                //            if (path.polys[p + 1].Id == controlGates[cg].npi2.Id)
                //            {
                //                tile.Polys[controlGates[cg].poly1].Area = new Area(2);
                //                path.Clear();
                //                path = nav.SmothPathfinding_paths(_agents[i].positionNow, _agents[i].positionTarget);

                //                //Console.WriteLine(i + " 逆向");
                //            }
                //        }
                //    }
                //    for (int cg = 0; cg < controlGates.Count; cg++)
                //    {
                //        tile.Polys[controlGates[cg].poly1].Area = new Area(1);
                //    }
                //}
                #endregion

                for (int pi = 0; pi < path.Count; pi++) agents[i].polyIds.Add(path[pi].Id);
                //Console.WriteLine(instanceID+" poly点 "+i);
                agents[i].navPoints = songnav1.SmothPathfinding_Points(path);

                //for (int iii = 0; iii < _agents[i].navPoints.Count - 1; iii++)
                //{
                //    if (_agents[i].navPoints[iii].x_ == 0 && _agents[i].navPoints[iii].y_ == 0)
                //    {
                //        _agents[i].navPoints.RemoveAt(iii);
                //    }
                //    if (iii > 2 && MathHelper.abs(_agents[i].navPoints[iii]-_agents[i].navPoints[iii-1])<5)
                //    {
                //        _agents[i].navPoints.RemoveAt(iii);
                //        iii--;
                //    }
                //}

                //agents[i].navPoints.Add(agents[i].positionTarget);
                //把最后位置的目标也加进去
                var outPoint = _out[agents[i].outIndex];
                agents[i].navPoints.Add(new Vector2(outPoint.X, outPoint.Z));
                ////如果是在出口10，则把最后的目标点也加上
                //Vector2 out11 = new Vector2(215.21f,11.3f);
                //if( MathHelper.abs(agents[i].positionTarget-out11)<50)
                //{
                //    agents[i].navPoints.Add(out11);
                //}

                if (agents[i].navPoints[0].x_ == 0 && agents[i].navPoints[0].y_ == 0)
                {
                    Console.WriteLine(instanceID + "编号 " + i + " 路径计算出错");
                }
                if (agents[i].navPoints.Count > 2000)
                {
                    Console.WriteLine(instanceID + "编号 " + i + " 路径过多 " + agents[i].navPoints.Count);
                }
            }
        }

        public void PathFindandCheck2(Object obj)
        {
            List<AgentClass> agents = (List<AgentClass>)obj;
            //List<AgentClass> agents = _agents;
            //别忘重新安置这段代码
            //nav.roadfilter.SetAreaCost(2, 1000);
            //var tile = songnav.tiledNavMesh.GetTileAt(0, 0, 0);
            
            for (int i = 0; i < agents.Count; i++)
            {
                pathFindedNums++;
                SharpNav.Pathfinding.Path path = songnav2.SmothPathfinding_paths(agents[i].positionNow, agents[i].positionTarget);
                //SharpNav.Pathfinding.Path path1 = songnav.SmothPathfinding_paths(agents[i-1].positionNow, agents[i-1].positionTarget);
                #region 方向
                //控制方向的
                //for (int p = 0; p < path.polys.Count - 1; p++)
                //{
                //    for (int cg = 0; cg < controlGates.Count; cg++)
                //    {
                //        if(p>=path.polys.Count || cg>=controlGates.Count)
                //        {
                //            Console.WriteLine(" P太大 ");
                //        }
                //        else if (path.polys[p].Id == controlGates[cg].npi1.Id)
                //        {
                //            if (path.polys[p + 1].Id == controlGates[cg].npi2.Id)
                //            {
                //                tile.Polys[controlGates[cg].poly1].Area = new Area(2);
                //                path.Clear();
                //                path = nav.SmothPathfinding_paths(_agents[i].positionNow, _agents[i].positionTarget);

                //                //Console.WriteLine(i + " 逆向");
                //            }
                //        }
                //    }
                //    for (int cg = 0; cg < controlGates.Count; cg++)
                //    {
                //        tile.Polys[controlGates[cg].poly1].Area = new Area(1);
                //    }
                //}
                #endregion

                for (int pi = 0; pi < path.Count; pi++) agents[i].polyIds.Add(path[pi].Id);
                //Console.WriteLine(instanceID+" poly点 "+i);
                agents[i].navPoints = songnav2.SmothPathfinding_Points(path);

                //for (int iii = 0; iii < _agents[i].navPoints.Count - 1; iii++)
                //{
                //    if (_agents[i].navPoints[iii].x_ == 0 && _agents[i].navPoints[iii].y_ == 0)
                //    {
                //        _agents[i].navPoints.RemoveAt(iii);
                //    }
                //    if (iii > 2 && MathHelper.abs(_agents[i].navPoints[iii]-_agents[i].navPoints[iii-1])<5)
                //    {
                //        _agents[i].navPoints.RemoveAt(iii);
                //        iii--;
                //    }
                //}

                //agents[i].navPoints.Add(agents[i].positionTarget);
                //把最后位置的目标也加进去
                var outPoint = _out[agents[i].outIndex];
                agents[i].navPoints.Add(new Vector2(outPoint.X, outPoint.Z));
                ////如果是在出口10，则把最后的目标点也加上
                //Vector2 out11 = new Vector2(215.21f,11.3f);
                //if( MathHelper.abs(agents[i].positionTarget-out11)<50)
                //{
                //    agents[i].navPoints.Add(out11);
                //}

                if (agents[i].navPoints[0].x_ == 0 && agents[i].navPoints[0].y_ == 0)
                {
                    Console.WriteLine(instanceID + "编号 " + i + " 路径计算出错");
                }
                if (agents[i].navPoints.Count > 2000)
                {
                    Console.WriteLine(instanceID + "编号 " + i + " 路径过多 " + agents[i].navPoints.Count);
                }
            }
        }
        //重新规划
        public void ReplanNav()
        {
            //方法1
            //long replaytimebefore = sw.ElapsedMilliseconds;
            //    for (int i = 0; i < _agents.Count; i++)
            //    {
            //        try
            //        {
            //            songnav.SmothPathfinding_paths(_agents[i].positionNow, _agents[i].positionTarget);
            //            _agents[i].navPoints = songnav.SmothPathfinding_Points();
            //            //_agents[i].navPoints = nav.SmothPathfinding(_agents[i].positionNow, _agents[i].positionTarget);
            //            _agents[i].navPoints.Add(_agents[i].positionTarget);
            //        }
            //        catch (Exception e)
            //        {
            //            Console.WriteLine(e);
            //            Console.WriteLine("人数：" + _agents.Count + "  " + i);
            //            Console.WriteLine("多线程对_agents.count访问出错-replan");
            //            //throw e;
            //        }
            //    }
            //    //Console.WriteLine("新线程重新规划耗时： "+ (sw.ElapsedMilliseconds-replaytimebefore)/1000);


            //方法2
            //PathFindandCheck();

            //方法3
            //检查当前位置与第一个导航点，如果太大，就重新规划路径
            //for(int i = 0; i < _agents.Count; i++)
            //{
            //    if(MathHelper.abs(_agents[i].positionNow-_agents[i].navPoints[0])>8)//4米时去下一个导航点，8米时重新寻求导航点
            //    {
            //        try
            //        {
            //            songnav.SmothPathfinding_paths(_agents[i].positionNow, _agents[i].positionTarget);
            //            _agents[i].navPoints = songnav.SmothPathfinding_Points();
            //            //_agents[i].navPoints = nav.SmothPathfinding(_agents[i].positionNow, _agents[i].positionTarget);
            //            _agents[i].navPoints.Add(_agents[i].positionTarget);
            //        }
            //        catch (Exception e)
            //        {
            //            Console.WriteLine(e);
            //            Console.WriteLine("人数：" + _agents.Count + "  " + i);
            //            Console.WriteLine("多线程对_agents.count访问出错-replan");
            //            //throw e;
            //        }
            //    }

            //}


        }
        #endregion


        //出错!!
        public void OutpositionFileEnd()
        {
            continueSteps = false;
            FH.EndOutput(Settings.deltaTDefault, _agents.Count);

            ////开放一个新线程，把agentsPositionsforExodus所有位置输出文件
            //outForExodus();
        }

        //public void outForExodus()
        //{
        //    FileHelper FHforExodus;
        //    FHforExodus = new FileHelper(Sample.mainDirectory + instanceID +"_Exodus"+ ".txt");


        //    FHforExodus.Write(agentsPositionsforExodus.Count.ToString());//先输出疏散人数

        //    for (int i=0;i<agentsPositionsforExodus.Count;i++)
        //    {
        //        // 一个人开始  song
        //        FHforExodus.NewFrame(" ");//只是把frame++了而已
        //        FHforExodus.NewLine();//换行
        //        FHforExodus.Write("0 "+agentsPositionsforExodus[i].Count+ " 25 80 1.8 1 180 0.3");//先输出疏散人数
        //        for(int j=0;j<agentsPositionsforExodus[i].Count-1;j++)
        //        {
        //            FHforExodus.NewLine();//换行
        //            FHforExodus.Write(agentsPositionsforExodus[i][j].x_+" "+ agentsPositionsforExodus[i][j].y_);//先输出疏散人数
        //            FHforExodus.Write(" 0 0 "+j+" "+ (j+1) + " 1 1 0 180 0");//先输出疏散人数
        //        }
        //        FHforExodus.NewLine();//换行
        //        int jj = agentsPositionsforExodus[i].Count - 1;
        //        FHforExodus.Write(agentsPositionsforExodus[i][jj].x_ + " " + agentsPositionsforExodus[i][jj].y_);
        //        FHforExodus.Write(" 0 0 " + jj + " " + (jj + 1) + " 0 0 0 180 0");//先输出疏散人数
        //    }
        //    FHforExodus.EndOut();
        //}

        public void outForExodus(FileHelper FHforExodus)
        {
            
            //FHforExodus = new FileHelper(Sample.mainDirectory + instanceID + "_Exodus" + ".txt");
            //FHforExodus.Write(agentsPositionsforExodus.Count.ToString());//先输出疏散人数

            for (int i = 0; i < agentsPositionsforExodus.Count; i++)
            {
                // 一个人开始  song
                FHforExodus.NewFrame(" ");//只是把frame++了而已
                FHforExodus.NewLine();//换行
                FHforExodus.Write("0 " + agentsPositionsforExodus[i].Count + " 25 80 1.8 1 180 0.3");//先输出疏散人数
                for (int j = 0; j < agentsPositionsforExodus[i].Count - 1; j++)
                {
                    FHforExodus.NewLine();//换行
                    FHforExodus.Write(agentsPositionsforExodus[i][j].x_ + " " + agentsPositionsforExodus[i][j].y_);//先输出疏散人数
                    FHforExodus.Write(" 0 0 " + j + " " + (j + 1) + " 1 1 0 180 0");//先输出疏散人数
                }
                FHforExodus.NewLine();//换行
                int jj = agentsPositionsforExodus[i].Count - 1;
                FHforExodus.Write(agentsPositionsforExodus[i][jj].x_ + " " + agentsPositionsforExodus[i][jj].y_);
                FHforExodus.Write(" 0 0 " + jj + " " + (jj + 1) + " 0 0 0 180 0");//先输出疏散人数
            }
            
        }


        public Thread stepsThread;



    }
}
