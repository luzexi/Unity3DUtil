

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
            vecRandom[i] = RANDOM_ONE();
        }

        for (int i = 0; i < num; i++)
        {
            float sumPos = 0;
            for (int j = RANDOM(0,typeNum) , k = 0; k < perLst.Length; k++, j++)
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

    /// <summary>
    /// 随机圆内点
    /// </summary>
    /// <returns></returns>
    public static Vector2 RANDOM_IN_CIRCLE()
    {
        return UnityEngine.Random.insideUnitCircle;
    }

    /// <summary>
    /// 随机球内点
    /// </summary>
    /// <returns></returns>
    public static Vector3 RANDOM_IN_SPHERE()
    {
        return UnityEngine.Random.insideUnitSphere;
    }

    /// <summary>
    /// 随机球上点
    /// </summary>
    /// <returns></returns>
    public static Vector3 RANDOM_ON_SPHERE()
    {
        return UnityEngine.Random.onUnitSphere;
    }

    /// <summary>
    /// 随机0-1浮点数
    /// </summary>
    /// <returns></returns>
    public static float RANDOM_ONE()
    {
        return RANDOM(0f, 1f);
    }

    /// <summary>
    /// 随机范围内浮点数
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static float RANDOM(float min, float max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    /// <summary>
    /// 随机范围内整数
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static int RANDOM(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }
}