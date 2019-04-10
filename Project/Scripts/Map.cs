// Copyright (c) 2013, 2015 Robert Rouhani <robert.rouhani@gmail.com> and other contributors (see CONTRIBUTORS file).
// Licensed under the MIT License - https://raw.github.com/Robmaister/SharpNav/master/LICENSE

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

using SharpNav.Geometry;



namespace Simulate
{

    public class Line
    {
        public Vector3 point1;
        public Vector3 point2;
        public Vector3 point3;
        public bool valid;//�����غ������Ч���൱��ȥ���ظ���

        public Line(Vector3 p1, Vector3 p2)
        {
            point1 = p1;
            point2 = p2;
            valid = true;
        }

        public override string ToString()
        {
            return point1.ToString() + "------" + point2.ToString();
        }

        //���ص��������
        public static bool operator ==(Line Line1, Line Line2)
        {
            return ((Line1.point1 == Line2.point1 && Line1.point2 == Line2.point2) || (Line1.point1 == Line2.point2 && Line1.point2 == Line2.point1));
        }

        public static bool operator !=(Line Line1, Line Line2)
        {
            return !((Line1.point1 == Line2.point1 && Line1.point2 == Line2.point2) || (Line1.point1 == Line2.point2 && Line1.point2 == Line2.point1));
        }
    }



    /// <summary>
    /// Parses a model in .obj format.
    /// </summary>
    public class Map
    {
        private static readonly char[] lineSplitChars = { ' ' };

        public List<Triangle3> tris;
        private List<Vector3> norms;

        public List<Vector3> _v;//�ṹ���Ǻܺ�v���������ţ����������ߵı��
        public List<Vector3> _f;//�ṹ���Ǻܺ����������ţ����������ߵı��
        public List<Vector2> _out;

        /// <summary>
        /// Initializes a new instance of the <see cref="Map"/> class.
        /// </summary>
        /// <param name="path">The path of the .obj file to parse.</param>
        public Map(string path)
        {
            tris = new List<Triangle3>();
            norms = new List<Vector3>();
            List<Vector3> tempVerts = new List<Vector3>();
            List<Vector3> tempNorms = new List<Vector3>();

            _v = new List<Vector3>();
            _f = new List<Vector3>();
            _out = new List<Vector2>();

            using (StreamReader reader = new StreamReader(path))
            {
                string file = reader.ReadToEnd();
                int areaNov = 0;
                foreach (string l in file.Split('\n'))
                {
                    //trim any extras
                    string tl = l;
                    
                    int commentStart = l.IndexOf("#");
                    if (commentStart != -1)
                        tl = tl.Substring(0, commentStart);
                    tl = tl.Trim();

                    string[] line = tl.Split(lineSplitChars, StringSplitOptions.RemoveEmptyEntries);
                    if (line == null || line.Length == 0)
                        continue;

                    switch (line[0])
                    {
                        case "v":
                            if (line.Length < 4)
                                continue;

                            Vector3 v;
                            if (!TryParseVec(line, 1, 2, 3, out v)) continue;

                            //v.X += 10;
                            //v.Z -= 10;

                            tempVerts.Add(v);
                            _v.Add(v);//song
                            break;
                        case "vn":
                            if (line.Length < 4)
                                continue;

                            Vector3 n;
                            if (!TryParseVec(line, 1, 2, 3, out n)) continue;
                            tempNorms.Add(n);
                            break;
                        case "f":
                            if (line.Length < 4)
                                continue;
                            else if (line.Length == 4)
                            {
                                //int v0, v1, v2;
                                //int n0, n1, n2;
                                //if (!int.TryParse(line[1].Split('/')[0], out v0)) continue;
                                //if (!int.TryParse(line[2].Split('/')[0], out v1)) continue;
                                //if (!int.TryParse(line[3].Split('/')[0], out v2)) continue;
                                //if (!int.TryParse(line[1].Split('/')[2], out n0)) continue;
                                //if (!int.TryParse(line[2].Split('/')[2], out n1)) continue;
                                //if (!int.TryParse(line[3].Split('/')[2], out n2)) continue;

                                //v0 -= 1;
                                //v1 -= 1;
                                //v2 -= 1;
                                //n0 -= 1;
                                //n1 -= 1;
                                //n2 -= 1;

                                //tris.Add(new Triangle3(tempVerts[v0], tempVerts[v1], tempVerts[v2]));
                                //norms.Add(tempNorms[n0]);
                                //norms.Add(tempNorms[n1]);
                                //norms.Add(tempNorms[n2]);

                                tris.Add(new Triangle3(tempVerts[int.Parse(line[1]) - 1], tempVerts[int.Parse(line[2]) - 1], tempVerts[int.Parse(line[3]) - 1]));
                                _f.Add(new Vector3(int.Parse(line[1]) - 1, int.Parse(line[2]) - 1, int.Parse(line[3]) - 1));//song
                            }
                            else
                            {
                                int v0, n0;
                                if (!int.TryParse(line[1].Split('/')[0], out v0)) continue;
                                if (!int.TryParse(line[1].Split('/')[2], out n0)) continue;

                                v0 -= 1;
                                n0 -= 1;

                                for (int i = 2; i < line.Length - 1; i++)
                                {
                                    int vi, vii;
                                    int ni, nii;
                                    if (!int.TryParse(line[i].Split('/')[0], out vi)) continue;
                                    if (!int.TryParse(line[i + 1].Split('/')[0], out vii)) continue;
                                    if (!int.TryParse(line[i].Split('/')[2], out ni)) continue;
                                    if (!int.TryParse(line[i + 1].Split('/')[2], out nii)) continue;

                                    vi -= 1;
                                    vii -= 1;
                                    ni -= 1;
                                    nii -= 1;

                                    tris.Add(new Triangle3(tempVerts[v0], tempVerts[vi], tempVerts[vii]));
                                    //norms.Add(tempNorms[n0]);
                                    //norms.Add(tempNorms[ni]);
                                    //norms.Add(tempNorms[nii]);
                                }
                            }
                            break;
                        case "outs":
                            _out.Add(new Vector2(float.Parse(line[1]) , float.Parse(line[2])));
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Gets an array of the triangles in this model.
        /// </summary>
        /// <returns></returns>
        public Triangle3[] GetTriangles()
        {
            return tris.ToArray();
        }

        /// <summary>
        /// Gets an array of the normals in this model.
        /// </summary>
        /// <returns></returns>
        public Vector3[] GetNormals()
        {
            return norms.ToArray();
        }

        private bool TryParseVec(string[] values, int x, int y, int z, out Vector3 v)
        {
            v = Vector3.Zero;

            if (!float.TryParse(values[x], NumberStyles.Any, CultureInfo.InvariantCulture, out v.X))
                return false;
            if (!float.TryParse(values[y], NumberStyles.Any, CultureInfo.InvariantCulture, out v.Y))
                return false;
            if (!float.TryParse(values[z], NumberStyles.Any, CultureInfo.InvariantCulture, out v.Z))
                return false;

            return true;
        }
    }
}





//// Copyright (c) 2013, 2015 Robert Rouhani <robert.rouhani@gmail.com> and other contributors (see CONTRIBUTORS file).
//// Licensed under the MIT License - https://raw.github.com/Robmaister/SharpNav/master/LICENSE

//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.IO;

//using SharpNav.Geometry;

//#if OPENTK
//using OpenTK;
//#endif

////Doesn't compile if in an unsupported configuration
//#if STANDALONE || OPENTK

//namespace songNav
//{
//	/// <summary>
//	/// Parses a model in .obj format.
//	/// </summary>
//	public class ObjModel
//	{
//		private static readonly char[] lineSplitChars = { ' ' };

//		private List<Triangle3> tris;
//		private List<Vector3> norms;

//		/// <summary>
//		/// Initializes a new instance of the <see cref="ObjModel"/> class.
//		/// </summary>
//		/// <param name="path">The path of the .obj file to parse.</param>
//		public ObjModel(string path)
//		{
//			tris = new List<Triangle3>();
//			norms = new List<Vector3>();
//			List<Vector3> tempVerts = new List<Vector3>();
//			List<Vector3> tempNorms = new List<Vector3>();

//			using (StreamReader reader = new StreamReader(path))
//			{
//				string file = reader.ReadToEnd();
//				foreach (string l in file.Split('\n'))
//				{
//					//trim any extras
//					string tl = l;
//					int commentStart = l.IndexOf("#");
//					if (commentStart != -1)
//						tl = tl.Substring(0, commentStart);
//					tl = tl.Trim();

//					string[] line = tl.Split(lineSplitChars, StringSplitOptions.RemoveEmptyEntries);
//					if (line == null || line.Length == 0)
//						continue;

//					switch (line[0])
//					{
//						case "v":
//							if (line.Length < 4)
//								continue;

//							Vector3 v;
//							if (!TryParseVec(line, 1, 2, 3, out v)) continue;
//							tempVerts.Add(v);
//							break;
//						case "vn":
//							if (line.Length < 4)
//								continue;

//							Vector3 n;
//							if (!TryParseVec(line, 1, 2, 3, out n)) continue;
//							tempNorms.Add(n);
//							break;
//						case "f":
//							if (line.Length < 4)
//								continue;
//							else if (line.Length == 4)
//							{
//                                //int v0, v1, v2;
//                                //int n0, n1, n2;
//                                //if (!int.TryParse(line[1].Split('/')[0], out v0)) continue;
//                                //if (!int.TryParse(line[2].Split('/')[0], out v1)) continue;
//                                //if (!int.TryParse(line[3].Split('/')[0], out v2)) continue;
//                                //if (!int.TryParse(line[1].Split('/')[2], out n0)) continue;
//                                //if (!int.TryParse(line[2].Split('/')[2], out n1)) continue;
//                                //if (!int.TryParse(line[3].Split('/')[2], out n2)) continue;

//                                //v0 -= 1;
//                                //v1 -= 1;
//                                //v2 -= 1;
//                                //n0 -= 1;
//                                //n1 -= 1;
//                                //n2 -= 1;

//                                //tris.Add(new Triangle3(tempVerts[v0], tempVerts[v1], tempVerts[v2]));
//                                ////norms.Add(tempNorms[n0]);
//                                ////norms.Add(tempNorms[n1]);
//                                ////norms.Add(tempNorms[n2]);
//                                tris.Add(new Triangle3(tempVerts[int.Parse(line[1])-1], tempVerts[int.Parse(line[2]) - 1], tempVerts[int.Parse(line[3]) - 1]));
//                            }
//							else
//							{
//								int v0, n0;
//								if (!int.TryParse(line[1].Split('/')[0], out v0)) continue;
//								if (!int.TryParse(line[1].Split('/')[2], out n0)) continue;

//								v0 -= 1;
//								n0 -= 1;

//								for (int i = 2; i < line.Length - 1; i++)
//								{
//									int vi, vii;
//									int ni, nii;
//									if (!int.TryParse(line[i].Split('/')[0], out vi)) continue;
//									if (!int.TryParse(line[i + 1].Split('/')[0], out vii)) continue;
//									if (!int.TryParse(line[i].Split('/')[2], out ni)) continue;
//									if (!int.TryParse(line[i + 1].Split('/')[2], out nii)) continue;

//									vi -= 1;
//									vii -= 1;
//									ni -= 1;
//									nii -= 1;

//									tris.Add(new Triangle3(tempVerts[v0], tempVerts[vi], tempVerts[vii]));
//									//norms.Add(tempNorms[n0]);
//									//norms.Add(tempNorms[ni]);
//									//norms.Add(tempNorms[nii]);
//								}
//							}
//							break;
//					}
//				}
//			}
//		}

//		/// <summary>
//		/// Gets an array of the triangles in this model.
//		/// </summary>
//		/// <returns></returns>
//		public Triangle3[] GetTriangles()
//		{
//			return tris.ToArray();
//		}

//		/// <summary>
//		/// Gets an array of the normals in this model.
//		/// </summary>
//		/// <returns></returns>
//		public Vector3[] GetNormals()
//		{
//			return norms.ToArray();
//		}

//		private bool TryParseVec(string[] values, int x, int y, int z, out Vector3 v)
//		{
//			v = Vector3.Zero;

//			if (!float.TryParse(values[x], NumberStyles.Any, CultureInfo.InvariantCulture, out v.X))
//				return false;
//			if (!float.TryParse(values[y], NumberStyles.Any, CultureInfo.InvariantCulture, out v.Y))
//				return false;
//			if (!float.TryParse(values[z], NumberStyles.Any, CultureInfo.InvariantCulture, out v.Z))
//				return false;

//			return true;
//		}
//	}
//}

//#endif
