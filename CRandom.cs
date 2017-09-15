
using UnityEngine;

//	CRandom.cs
//	Author: Lu Zexi
//	2014-12-07



//random
public class CRandom
{
	/// <summary>
    /// 由概率集得到1次随机落在何处
    /// </summary>
    /// <param name="perLst"></param>
    /// <returns></returns>
    public static int BET(params float[] perLst)
    {
        return RANDOM_BET(1, perLst)[0];
    }

    /// <summary>
    /// 由指定概率集得到N次随机落在何处
    /// </summary>
    /// <param name="num"></param>
    /// <param name="perLst"></param>
    /// <returns></returns>
    public static int[] BET(int num, params float[] perLst)
    {
        return RANDOM_BET(num, perLst);
    }

    /// <summary>
    /// 由指定概率集得到N次随机落在何处
    /// </summary>
    /// <param name="num"></param>
    /// <param name="perLst"></param>
    /// <returns></returns>
    public static int[] RANDOM_BET(int num, float[] perLst)
    {
        if (perLst == null || num <= 0)
            return null;

        int typeNum = perLst.Length;
        int[] selectPos = new int[num];
        float[] vecRandom = new float[num];

        for (int i = 0; i < num; i++)
        {
            selectPos[i] = -1;
            vecRandom[i] = UnityEngine.Random.Range(0,1f);
        }

        for (int i = 0; i < num; i++)
        {
            float sumPos = 0;
            for (int j = (int)UnityEngine.Random.Range(0,typeNum) , k = 0; k < perLst.Length; k++, j++)
            {
                sumPos += perLst[j % typeNum];
                //Debug.Log("sum + " + sumPos + " -- " + perLst[j % typeNum] + " -- " + vecRandom[i]);
                if (sumPos >= vecRandom[i])
                {
                    selectPos[i] = j % typeNum;
                    break;
                }
            }

        }

        return selectPos;
    }
}