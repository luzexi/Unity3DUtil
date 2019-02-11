using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// graphic math
public class GMath {
    // the side of line for point
    public enum LinePointSide {
        ON_LINE = 0,
        LEFT_SIDE = 1,
        RIGHT_SIDE = 2,
    };

    private const float EPSILON = 1e-005f; //最小常量

    private static Vector2 _TempConvexFirst; //temp point

    ////////////////////////////////

    // check line cross
    public static bool CheckLineCross (Vector2 sp1, Vector2 ep1, Vector2 sp2, Vector2 ep2) {
        if (Math.Max (sp1.x, ep1.x) < Math.Min (sp2.x, ep2.x)) {
        return false;
        }
        if (Math.Min (sp1.x, ep1.x) > Math.Max (sp2.x, ep2.x)) {
            return false;
        }
        if (Math.Max (sp1.y, ep1.y) < Math.Min (sp2.y, ep2.y)) {
            return false;
        }
        if (Math.Min (sp1.y, ep1.y) > Math.Max (sp2.y, ep2.y)) {
            return false;
        }

        float temp1 = LineCrossProduct ((sp1 - sp2), (ep2 - sp2)) * LineCrossProduct ((ep2 - sp2), (ep1 - sp2));
        float temp2 = LineCrossProduct ((sp2 - sp1), (ep1 - sp1)) * LineCrossProduct ((ep1 - sp1), (ep2 - sp1));

        if ((temp1 >= 0) && (temp2 >= 0)) {
            return true;
        }

        return false;
    }

    public static float LineCrossProduct (Vector2 p1, Vector2 p2) {
        return (p1.x * p2.y - p1.y * p2.x);
    }

    ///////////

    // check point on which side of line
    public static LinePointSide CheckLinePointSide (Vector2 start, Vector2 end, Vector2 point) {
        Vector2 vectorA = end - start;
        Vector2 vectorB = point - start;

        float crossResult = LineCrossProduct (vectorA, vectorB);
        if (IsEqualZero (crossResult))
            return LinePointSide.ON_LINE;
        else if (crossResult < 0)
            return LinePointSide.RIGHT_SIDE;
        else
            return LinePointSide.LEFT_SIDE;
    }

    ///////////////////////////////

    ///////////////////////////////

    // check rect collide or cross
    static public bool IsRectCollide(Vector2[] rect1,Vector2[] rect2)
    {
        return IsRectCollideSAT(rect1,rect2) && IsRectCollideSAT(rect2,rect1);
    }

    static private bool IsRectCollideSAT (Vector2[] rect1, Vector2[] rect2) {
        // Debug.Assert (rect1.Length == 4, "rect1's point num is wrong");
        // Debug.Assert (rect2.Length == 4, "rect2's point num is wrong");

        //旋转轴
        Vector2[] axises = new Vector2[2];
        axises[0] = new Vector2 (rect1[3].x - rect1[0].x, rect1[3].y - rect1[0].y);
        axises[1] = new Vector2 (rect1[1].x - rect1[0].x, rect1[1].y - rect1[0].y);

        axises[0].Normalize ();
        axises[1].Normalize ();

        float scalar_max_1 = 0;
        float scalar_min_1 = 0;
        float scalar_max_2 = 0;
        float scalar_min_2 = 0;

        float[] scalar_1 = new float[2];
        float[] scalar_2 = new float[4];

        for (int i = 0; i < 2; ++i) {
            //第一个矩形的投影
            for (int p = 0; p < 2; ++p) {
                scalar_1[p] = Vector2.Dot (axises[i], rect1[p * 2]);
            }

            //第二个矩阵的投影
            for (int p = 0; p < 4; ++p) {
                scalar_2[p] = Vector2.Dot (axises[i], rect2[p]);
            }

            RectCollideGetMinMax (scalar_1, out scalar_min_1, out scalar_max_1);
            RectCollideGetMinMax (scalar_2, out scalar_min_2, out scalar_max_2);

            if (scalar_max_2 < scalar_min_1 || scalar_max_1 < scalar_min_2)
                return false;
        }
        return true;
    }

    static private void RectCollideGetMinMax (float[] arrs, out float min, out float max) {
        if (arrs.Length == 0) {
            min = 0;
            max = 0;
        } else {
            min = arrs[0];
            max = arrs[0];

            if (arrs.Length == 1)
                return;

            for (int i = 1; i < arrs.Length; ++i) {
                if (arrs[i] > max)
                    max = arrs[i];
                if (arrs[i] < min)
                    min = arrs[i];
            }
        }
    }

    /////////////////////////

    // //polygon union
    // static public List<Vector2> PolygonUnion (List<Vector2> _polygon1, Vector2[] _polygon) {
    //     List<Vector2> _polygon2 = new List<Vector2> (_polygon);

    //     if (_polygon1 == null) return _polygon2;
    //     if (_polygon1.Count == 0) return _polygon2;

    //     // List<Vector2> result = new List<Vector2>();

    //     // _polygon1.AddRange(polygon2);
    //     // return _polygon1;
    //     // return result;
    //     EPPZ.Geometry.Model.Polygon polygon1 = EPPZ.Geometry.Model.Polygon.PolygonWithPointList (_polygon1);
    //     EPPZ.Geometry.Model.Polygon polygon2 = EPPZ.Geometry.Model.Polygon.PolygonWithPointList (_polygon2);
    //     polygon1.AddPolygon (polygon2);
    //     EPPZ.Geometry.Model.Polygon polygon_union = polygon1.UnionPolygon ();

    //     return new List<Vector2> (polygon_union.points);
    // }

    ///////////////////////////

    // is point in polygon
    public static bool PointInPolygon (List<Vector2> Polyg, Vector2 a) {
        int intersectNum = 0;
        bool EndIntersect = false, SecondLoop = false;
        float SegStartX = 0;

        for (int i = 0; i < Polyg.Count; i++) {
            int j = i + 1;
            if (i == Polyg.Count - 1) j = 0;
            if (a == Polyg[i] || a == Polyg[j]) return true;

            float Min_X = Math.Min (Polyg[i].x, Polyg[j].x);
            float Max_X = Math.Max (Polyg[i].x, Polyg[j].x);
            if (a.x < Min_X || a.x > Max_X) continue;

            if (a.y == Polyg[i].y && a.y == Polyg[j].y) return true;

            float X_dif = (float) (Polyg[j].x - Polyg[i].x);
            if (!IsEqualZero (X_dif)) {
                float _y = Polyg[i].y +
                    (a.x - Polyg[i].x) * (Polyg[j].y - Polyg[i].y) / X_dif;

                if (a.y <= _y) {
                    if (Polyg[i].x == a.x && Polyg[j].x == a.x) continue;

                    if (Polyg[j].x == a.x) {
                        SegStartX = Polyg[i].x;
                        EndIntersect = true;
                    } else {
                        if (EndIntersect) {
                            if (a.x > Math.Min (SegStartX, Polyg[j].x) &&
                                a.x < Math.Max (SegStartX, Polyg[j].x)) {
                                intersectNum++;
                                if (SecondLoop) { intersectNum--; break; }
                            } else if (SecondLoop) { intersectNum--; break; }
                            EndIntersect = false;
                        } else intersectNum++;
                    }
                }
            }

            if (j == Polyg.Count - 1 && EndIntersect) {
                i = -1;
                SecondLoop = true;
            }
        }

        if (intersectNum % 2 == 1) return true;
        else return false;
    }

    /////////////////////////

    // Clock wise operation
    public static void CW (List<Vector2> vex) {
        CQuickSort.Sort<Vector2> (vex, 0, vex.Count - 1, ConvexCompare1);
        _TempConvexFirst = vex[0];
        CQuickSort.Sort<Vector2> (vex, 1, vex.Count - 1, CWCompare);
    }

    // Counter clock wise operation
    public static void CCW (List<Vector2> vex) {
        CQuickSort.Sort<Vector2> (vex, 0, vex.Count - 1, ConvexCompare1);
        _TempConvexFirst = vex[0];
        CQuickSort.Sort<Vector2> (vex, 1, vex.Count - 1, ConvexCompare2);
    }

    static int CWCompare (Vector2 a, Vector2 b) {
        float res = Mathf.Atan2 (b.y - _TempConvexFirst.y, b.x - _TempConvexFirst.x) - Mathf.Atan2 (a.y - _TempConvexFirst.y, a.x - _TempConvexFirst.x);
        if (IsEqualZero (res)) {
            float tmp = a.x - b.x;
            if (IsEqualZero (tmp)) return 0;
            if (tmp > 0) return 1;
            else return -1;
        }

        if (res > 0)
            return 1;
        return -1;
    }

    ////////////////////////
    // 1.把所有点放在二维坐标系中，则纵坐标最小的点一定是凸包上的点
    // 2.把所有点的坐标平移一下，使 P0 作为原点
    // 3.计算各个点相对于 P0 的幅角 α ，按从小到大的顺序对各个点排序。当 α 相同时，距离 P0 比较近的排在前面。我们由几何知识可以知道，结果中第一个点 P1 和最后一个点一定是凸包上的点。 
    // （以上是准备步骤，以下开始求凸包） 
    // 以上，我们已经知道了凸包上的第一个点 P0 和第二个点 P1，我们把它们放在栈里面。现在从步骤3求得的那个结果里，把 P1 后面的那个点拿出来做当前点，即 P2 。接下来开始找第三个点：
    // 4.连接P0和栈顶的那个点，得到直线 L 。看当前点是在直线 L 的右边还是左边。如果在直线的右边就执行步骤5；如果在直线上，或者在直线的左边就执行步骤6。
    // 5.如果在右边，则栈顶的那个元素不是凸包上的点，把栈顶元素出栈。执行步骤4。
    // 6.当前点是凸包上的点，把它压入栈，执行步骤7。
    // 7.检查当前的点 P2 是不是步骤3那个结果的最后一个元素。是最后一个元素的话就结束。如果不是的话就把 P2 后面那个点做当前点，返回步骤4。
    // 最后，栈中的元素就是凸包上的点了。

    // convex
    public static List<Vector2> ConvexHull (List<Vector2> vex) {
        List<Vector2> result = new List<Vector2> (16);

        CQuickSort.Sort<Vector2> (vex, 0, vex.Count - 1, ConvexCompare1);
        for (int i = 1; i < vex.Count - 1;) {
            Vector2 v1 = vex[i - 1];
            Vector2 v2 = vex[i];
            Vector2 v3 = v1 - v2;
            if (IsEqualZero (v3)) {
                vex.RemoveAt (i);
            } else {
                i++;
            }
        }
        _TempConvexFirst = vex[0];
        result.Add (vex[0]);

        CQuickSort.Sort<Vector2> (vex, 1, vex.Count - 1, ConvexCompare2);
        result.Add (vex[1]);

        for (int i = 2; i < vex.Count; i++) {
            // Debug.LogError("index " + i);
            while (ConvexPointCross (result[result.Count - 2], result[result.Count - 1], vex[i]) < 0) {
                result.RemoveAt (result.Count - 1);
            }
            result.Add (vex[i]);
        }
        return result;
    }

    static float ConvexPointCross (Vector2 a, Vector2 b, Vector2 c) //计算叉积
    {
        return (b.x - a.x) * (c.y - a.y) - (c.x - a.x) * (b.y - a.y);
    }

    static int ConvexCompare1 (Vector2 a, Vector2 b) {
        if (IsEqualZero (a.y - b.y)) {
            float tmp = a.x - b.x;
            if (IsEqualZero (tmp)) return 0;
            if (tmp > 0) return 1;
            else return -1;
        } else {
            float tmp = a.y - b.y;
            if (IsEqualZero (tmp)) return 0;
            if (tmp > 0) return 1;
            else return -1;
        }
    }

    static int ConvexCompare2 (Vector2 a, Vector2 b) {
        float res = Mathf.Atan2 (a.y - _TempConvexFirst.y, a.x - _TempConvexFirst.x) - Mathf.Atan2 (b.y - _TempConvexFirst.y, b.x - _TempConvexFirst.x);
        if (IsEqualZero (res)) {
            float tmp = a.x - b.x;
            if (IsEqualZero (tmp)) return 0;
            if (tmp > 0) return 1;
            else return -1;
        }

        if (res > 0)
            return 1;
        return -1;
    }

    ////////////////////////

    // is equal zero
    public static bool IsEqualZero (Vector2 data) {
        if (IsEqualZero (data.x) && IsEqualZero (data.y))
            return true;
        else
            return false;
    }

    // is equal zero
    public static bool IsEqualZero (float data) {
        if (Math.Abs (data) <= EPSILON)
            return true;
        else
            return false;
    }

    //曼哈顿距离
    public static float ManHattanDistance (Vector2 vec1, Vector2 vec2) {
        return Mathf.Abs (vec1.x - vec2.x) + Mathf.Abs (vec1.y - vec2.y);
    }

    //计算交点,需要有误差允许
    public static bool Intersection(Vector2 a, Vector2 b, Vector2 c, Vector2 d,out Vector2 result) {
        float epsilon = 0.01f;

        // 三角形abc 面积的2倍  
        var area_abc = (a.x - c.x) * (b.y - c.y) - (a.y - c.y) * (b.x - c.x);
        if(Mathf.Abs(area_abc) <= epsilon)  //c在ab上，允许有误差
        {
            result = c;
            return true;
        }
        // 三角形abd 面积的2倍  
        var area_abd = (a.x - d.x) * (b.y - d.y) - (a.y - d.y) * (b.x - d.x);
        if(Mathf.Abs(area_abd) <= epsilon)  //d在ab上，允许有误差
        {
            result = d;
            return true;
        }

        // 三角形cda 面积的2倍  
        var area_cda = (c.x - a.x) * (d.y - a.y) - (c.y - a.y) * (d.x - a.x);
        if(Mathf.Abs(area_cda) <= epsilon)   //a在cd上，允许有误差
        {
            result = a;
            return true;
        }
        // 三角形cdb 面积的2倍  
        var area_cdb = area_cda + area_abc - area_abd;
        if(Mathf.Abs(area_cdb) <= epsilon)    //b在cd上，允许有误差
        {
            result = b;
            return true;
        }

        // 面积符号相同则两点在线段同侧,不相交
        if (area_abc * area_abd > 0) 
        {
            result = Vector2.zero;
            return false;
        }

        if (area_cda * area_cdb > 0) 
        {
            result = Vector2.zero;
            return false;
        }

        //计算交点坐标  
        var t = area_cda / (area_abd - area_abc);
        var dx = t * (b.x - a.x);
        var dy = t * (b.y - a.y);
        result = new Vector2 (a.x + dx, a.y + dy);
        return true;
    }

    //获取point点到线段上最近的点
    public static Vector2 GetClosestPointInLine(Vector2 startPos,Vector2 endPos,Vector2 point)
    {
        Vector2 line = endPos - startPos;
        Vector2 edge = point - startPos;

        float d = line.x*line.x + line.y*line.y;    // qp线段长度的平方
        float t = Vector2.Dot(edge,line);
        Vector2 intersection;
        if (t < 0 || d <= 0)
        {
            t = 0;     // 最短距离即为 pt点 和 start点之间的距离。
            intersection = startPos;
        }
        else if (t >= d)
        {
            t = 1;     // 当t（r）> 1时，最短距离即为 pt点 和 q点（B点和P点）之间的距离。
            intersection = endPos;
        }
        else
        {
            float scale = t/d;
            intersection = (1-scale)*startPos + scale * endPos;
        }
        return intersection;
    }

    // 返回点到线段最短距离的平方
    public static float DistancePointToLine(Vector2 startPos,Vector2 endPos,Vector2 point)
    {
        Vector2 line = endPos - startPos;
        Vector2 edge = point - startPos;

        float d = line.x*line.x + line.y*line.y;    // qp线段长度的平方
        float t = Vector2.Dot(edge,line);
        Vector2 dis;
        if (t < 0 || d <= 0)
        {
            t = 0;     // 最短距离即为 pt点 和 start点之间的距离。
            dis = startPos;
        }
        else if (t >= d)
        {
            t = 1;     // 当t（r）> 1时，最短距离即为 pt点 和 q点（B点和P点）之间的距离。
            dis = endPos;
        }
        else
        {
            float scale = t/d;
            dis = (1-scale)*startPos + scale * endPos;
        }
        dis = dis - point;
        //长度平方
        return dis.sqrMagnitude;
    }

    // //找最优的造路路径
    // //现在已知存在的问题：
    // //   1. 从次短边穿过的路无效，原因是根本没判断
    // //   2. 生成两段直线后，靠近end的直线递归了，前一段没有，导致前一段可能穿过其他建筑
    // //   3. 每段之间的连接点有问题，需要特殊处理
    // public static void GetCanSitRoute(Vector2 _start,Vector2 _end,List<Vector2[]> buildingList, ref int index,ref List<Vector2> route)
    // {
    //     bool isCross;
    //     Vector2 newStart = Vector2.zero;
    //     Vector2[] rect;
    //     Debug.Log("GetCanSitRoute:index:"+index);
    //     for(int i = index;i<buildingList.Count;++i)
    //     {
    //         isCross = false;
    //         rect = buildingList[i];
    //         int closest1 = 0;
    //         int closest2 = 0;
    //         float minDis1 = -1f;
    //         float minDis2 = -1f;
    //         for(int v=0;v<4;++v)
    //         {
    //             float dis = Vector2.Distance(rect[v],_start);
    //             if(minDis1 == -1)
    //             {
    //                 minDis1 = dis;
    //                 closest1 = v;
    //             }
    //             else if(dis < minDis1)
    //             {
    //                 minDis2 = minDis1;
    //                 closest2 = closest1;
    //                 minDis1 = dis;
    //                 closest1 = v;
    //             }
    //             else if(minDis2 == -1 || dis<minDis2)
    //             {
    //                 minDis2 = dis;
    //                 closest2 = v;
    //             }
    //         }
    //         if(GMath.CheckLineCross(_start,_end,rect[closest1],rect[closest2]))
    //         {
    //             isCross = true;
    //             float project_left = Vector2.Dot(rect[closest1]-_start,_end-_start);
    //             float project_right = Vector2.Dot(rect[closest2]-_start,_end-_start);

    //             int newStartIndex = 0;
    //             int newStart_left = 0;
    //             int newStart_right = 0;
    //             float ratio_left = 0.3f;
    //             float ratio_right = 0.2f;

    //             if(project_left>project_right)
    //             {
    //                 newStartIndex = closest1;
    //                 newStart_left = closest2;
    //                 newStart_right = (2*closest1 - closest2+4)%4;
    //             }
    //             else
    //             {
    //                 newStartIndex = closest2;
    //                 newStart_left = closest1;
    //                 newStart_right = (2*closest2 - closest1+4)%4;
    //             }
    //             Debug.Log("BuildingManager:236:"+newStart_right);
    //             newStart = rect[newStartIndex] + (rect[newStartIndex] - rect[newStart_left]).normalized*ratio_left + (rect[newStartIndex] - rect[newStart_right]).normalized*ratio_right;
    //         }

    //         if(isCross)
    //         {
    //             route.Add(newStart);
    //             //TODO: 选一个最近的点 Veritces[?]
    //             index = i;
    //             GetCanSitRoute(newStart,_end,buildingList,ref index,ref route);
    //             break;
    //         }
    //     }
    // }


    //3D -> 2D
    public static Vector2 PauseVec3ToVec2(Vector3 vec)
    {
        return new Vector2(vec.x,vec.z);
    }

    //2D -> 3D
    public static Vector3 PauseVec2ToVec3(Vector2 vec)
    {
        return new Vector3(vec.x,0,vec.y);
    }

    // public static Vector3[] PausePathFromVec2ToVec3(List<Vector2> path)
    // {
    //     if(path == null)
    //         return null;

    //     Vector3[] result =new Vector3[path.Count];
    //     RaycastHit hitInfo;
    //     for(int i=0;i<path.Count;++i)
    //     {
    //         Vector2 pos = path[i];
    //         //计算Y轴高度
    //         Vector3 origin = new Vector3(pos.x,Consts.MAX_TERRAIN_HEIGHT,pos.y);
    //         Physics.Raycast (origin, Vector3.down, out hitInfo, Mathf.Infinity, 1 << Consts.TERRAIN_LAYER);
    //         result[i] = new Vector3(pos.x,hitInfo.point.y,pos.y);
    //     }
    //     return result;
    // }

    // public static float CalculateTime(Vector3[] _path,float _speed)
    // {
    //     float length = 0f;
    //     if(_path == null || _path.Length == 0 || _speed == 0f)
    //         return 0f;
    //     for(int i=1;i<_path.Length;++i)
    //     {
    //         length +=Vector2.Distance(_path[i-1].xz(),_path[i].xz());
    //     }
    //     return length/_speed;
    // }

}