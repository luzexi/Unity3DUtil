using System;
using System.Collections.Generic;

interface IPathFinder
{
    #region Properties
    bool Stopped
    {
        get;
    }

    HeuristicFormula Formula
    {
        get;
        set;
    }

    bool Diagonals
    {
        get;
        set;
    }

    bool HeavyDiagonals
    {
        get;
        set;
    }

    int HeuristicEstimate
    {
        get;
        set;
    }

    bool PunishChangeDirection
    {
        get;
        set;
    }

    bool TieBreaker
    {
        get;
        set;
    }

    int SearchLimit
    {
        get;
        set;
    }

    double CompletedTime
    {
        get;
        set;
    }

    #endregion

    #region Methods
    void FindPathStop();
    List<PathFinderNode> FindPath(Point start, Point end, bool _ignoreObstacle);
    #endregion

}

