using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathManager
{
	private int	sUniquePathId;
	public Dictionary <int,PathObject> mDicPathAll;
	private static PathManager _instance;
	
	public const byte GRID_VALUE_CANNOT = 0;
	public static byte GRID_VALUE_GOOD = 10;
	public static byte GRID_VALUE_WALL = 100;
	public static byte GRID_VALUE_WALL_TARGETED = 11;
	public static byte GRID_ADD_VALUE_DIAGONAL = 4;
	//public const byte GRID_VALUE_GOOD_DIAGONAL = 14;
	//public const byte GRID_VALUE_WALL_DIAGONAL = 104;
	//public const byte GRID_VALUE_WALL_TARGETED_DIAGONAL = 24;

#if false
    public static void SetWall(byte[,] _pathArray, int _i, int _j, int hp)
    {
        int newHp = Mathf.Clamp(hp / 5, GRID_VALUE_WALL_TARGETED + 9, 255);
        _pathArray[_i, _j] = (byte)newHp;
    }

    public static bool IsWall(byte[,] _pathArray, int _i, int _j)
    {
        return _pathArray[_i, _j] >= GRID_VALUE_WALL_TARGETED;
    }
#endif

	public static PathManager Instance
	{
		get
		{
			if (_instance == null)
				_instance = new PathManager();
			return _instance;
		}
	}
	
	private PathManager()
	{
		mDicPathAll = new Dictionary<int, PathObject>();
		sUniquePathId = 0;
	}
	
	
	public PathObject GetPathById(int _id)
	{
		if(!mDicPathAll.ContainsKey(_id))
			return null;
		else
			return mDicPathAll[_id];
	}
	
	// if "ignoreObstacle" is true, pathfinding will ignore all obstacles except GRID_VALUE_CANNOT, grid is only by GRID_VALUE_CANNOT or by GRID_VALUE_GOOD
	public int CaculatePath(int _startX, int _startY, int _endX, int _endY, bool _ignoreObstacle, byte[,] _pathMap,GameObject GObj)
	{
        if (_pathMap[_startX, _startY] == GRID_VALUE_CANNOT ||
            _pathMap[_endX, _endY] == GRID_VALUE_CANNOT)
		{
			Debug.LogError("start point or end point is invalid when path finding.");
			return -1;
		}

        PathObject _path = new PathObject(_startX, _startY, _endX, _endY, _pathMap);
		bool Suc = _path.FindWalkPath(_ignoreObstacle,GObj);
		
		if(!Suc)
		{
			return -1;
		}
		else
		{
			return AddPath(_path);
		}
	}
	
	public int AddPath(PathObject _path)
	{
		int id = GetUniquePathId();
		mDicPathAll.Add(id,_path);
		return id;
	}
	
	public void RemovePath(int id)
	{
		if(mDicPathAll.ContainsKey(id))
			mDicPathAll.Remove(id);
	}
	
	public int GetUniquePathId()
	{
		int ret = sUniquePathId;
		sUniquePathId ++;
		return ret;
	}
	
	public int CurUniquepathId
	{
		get{return sUniquePathId;}
		set{sUniquePathId = value;}
	}
	
	/*public PathObject LoadOnePathFromData(int size,int[] _pathData)
	{		
		Point[]data = new Point[size];
		for(int i=0;i<size;i++)
		{
			data[i].X = _pathData[i*2];
			data[i].Y = _pathData[i*2+1];
		}
		Point start = data[0];
		Point end	= data[size-1];
		
		PathObject temp = new PathObject(start.X,start.Y,end.X,end.Y,CityMapManager.instance.sMapObjIdArrayLayerPathFinding);		
		temp.mPathData = data;
		temp.mStart = start;
		temp.mEnd = end;
		
		return temp;
	}*/
}
