using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum PathCondition
{
	eGood = 0,
	eWallInPath,
	eNoWay,
}

public class PathObject
{
	public 	Point[]					mPathData;
	public	Point					mStart;
	public	Point					mEnd;
	public  Point					mFirstWall;
	public  int						mFirstWallIndex;
	//public	static byte[,]			mMapData;
	//public	static byte[,]			mMapDataBig;
	public	byte[,]					mMapData;
	public  bool 					USE_FAST = true;
	private IPathFinder 			mPathFinder;
	public PathCondition			mPathCondition;
	//private PerformanceWatcher mPerformanceWatcher = new PerformanceWatcher();
	
	public PathObject(int _startX, int _startY, int _endX, int _endY, byte[,] _mapData)
	{		
		mStart	= new Point(_startX,_startY);
		mEnd	= new Point(_endX,_endY);
		mMapData = _mapData;
	}
	
	// if "ignoreObstacle" is true, pathfinding will ignore all obstacles except GRID_VALUE_CANNOT, grid is only by GRID_VALUE_CANNOT or by GRID_VALUE_GOOD
	public bool FindWalkPath(bool _ignoreObstacle,GameObject Obj)
	{		
		if(USE_FAST)
		{
			mPathFinder = new PathFinderFast(mMapData);
		}
		else
		{
			mPathFinder = new PathFinder(mMapData);
		}
		
		mPathFinder.Formula                 = HeuristicFormula.Manhattan;

        mPathFinder.Diagonals               = true;

        mPathFinder.HeavyDiagonals          = true;

        mPathFinder.HeuristicEstimate       = 3;

        mPathFinder.PunishChangeDirection   = false;

        mPathFinder.TieBreaker              = false;

        mPathFinder.SearchLimit             = 10000;
		
		List<PathFinderNode> path = mPathFinder.FindPath(mStart, mEnd, _ignoreObstacle);

		if (path == null)
		{
			mPathFinder = null;
			mPathCondition = PathCondition.eNoWay;
			return false;
		}
		else
		{
			Point mFirstPoint = new Point(0,0);
			int pathCount = path.Count;
			mPathData = new Point[pathCount];
			mPathCondition = PathCondition.eGood;
			for(int i=0;i<pathCount;i++)
			{
				int pathX = path[pathCount - 1 - i].X;
				int pathY = path[pathCount - 1 - i].Y;
				mPathData[i] = new Point(pathX, pathY);
				
				// define the first wall the path met
				if (!_ignoreObstacle && 
				    mPathCondition == PathCondition.eGood && 
				    mMapData[pathX, pathY] != PathManager.GRID_VALUE_GOOD)
				{
					mPathCondition = PathCondition.eWallInPath;

					if (mFirstWall == null)
					{
						mFirstWall = new Point(pathX, pathY); 
						mFirstWallIndex = i-1; 
					}
				}
			}  
			// ForTest(mPathData.Length,mPathData,null,Obj);
			mPathFinder = null;
			return true;
		}
	}

	void ForTest(int pathCount,Point[] mPathData,Point mFirstWall,GameObject Obj)
	{
		// if((!Constants.USE_AI_PATH_DEBUG)|| 
		//    (BattleManager.instance.GetMode() == BattleManager.BattleMode.eNone)||
		//    (MenuManager.Instance.FindMenu<Screen_main_ui_handler>() != null))
		// 	{PathLineManager.instance.ClearAll();return;}

		Vector3[] PathVec3 = new Vector3[pathCount];
		for(int x=0;x<pathCount;x++)
		{ 
			float vx = ((float)mPathData[x].X)/2.0f;
			float vy = ((float)mPathData[x].Y)/2.0f;
			float Height = 1.5f;
			if((null != mFirstWall) && (mPathData[x].X == mFirstWall.X) && (mPathData[x].Y == mFirstWall.Y)){Height = 3.0f;}
			PathVec3[x] = new Vector3(vx,Height,vy);
		} 
		// PathLineManager.instance.DrawPath(PathVec3,Obj);
	}

	public void CutPathToFirstWall(int amount,GameObject Obj)
	{
		if (mFirstWall != null && mFirstWallIndex < mPathData.Length - 1)
		{
			int newPathLenght = mFirstWallIndex+1-amount;
			if (newPathLenght < 1)
			{
				newPathLenght = 1;
			}
			Point[] newPathData = new Point[newPathLenght];
			for (int i=0; i<newPathLenght; i++)
			{
				newPathData[i] = mPathData[i];
			}
			mPathData = newPathData;
		}
		// ForTest(mPathData.Length,mPathData,null,Obj);
	}
}












