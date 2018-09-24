using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//  2013-11-26
//  CMathCurve.cs
//  Lu Zexi
//  数学工具


/// <summary>
/// 数学曲线类
/// </summary>
public class CMathCurve
{

    /// <summary>
    /// 直线
    /// </summary>
    /// <param name="t">初始值与最大值之间的值</param>
    /// <param name="b">初始值</param>
    /// <param name="c">初始值与最大值间隔</param>
    /// <param name="d">最大值</param>
    /// <returns></returns>
    public static float Linear(float t, float b, float c, float d)
    {
        return c * t / d + b;
    }

    /// <summary>
    /// 指数外曲线
    /// </summary>
    /// <param name="t">初始值与最大值之间的值</param>
    /// <param name="b">初始值</param>
    /// <param name="c">初始值与最大值间隔</param>
    /// <param name="d">最大值</param>
    /// <returns></returns>
    public static float ExponentialOut(float t, float b, float c, float d)
    {
        if (t == d)
        {
            return b + c;
        }
        else
        {
            return c * (-1f * Mathf.Pow(2, -10f * (t / d)) + 1) + b;
        }
    }

    /// <summary>
    /// 指数内曲线
    /// </summary>
    /// <param name="t">初始值与最大值之间的值</param>
    /// <param name="b">初始值</param>
    /// <param name="c">初始值与最大值间隔</param>
    /// <param name="d">最大值</param>
    /// <returns></returns>
    public static float ExponentialIn(float t, float b, float c, float d)
    {
        if (t == 0)
        {
            return b;
        }
        else
        {
            return c * Mathf.Pow(2, 10f * (t / d - 1)) + b;
        }
    }

    /// <summary>
    /// 弹性外曲线
    /// </summary>
    /// <param name="t">初始值与最大值之间的值</param>
    /// <param name="b">初始值</param>
    /// <param name="c">初始值与最大值间隔</param>
    /// <param name="d">最大值</param>
    /// <param name="a">a为弹性系数</param>
    /// <param name="p">p为弹性常量</param>
    /// <returns></returns>
    public static float ElasticOut(float t, float b, float c, float d, float a, float p)
    {
        float s = 0;
        if (t == 0)
            return b;
        t /= d;
        if (Mathf.Abs(t - 1f) < 0.00001f)
            return b + c;
        if (p == 0)
            p = d * 0.3f;
        if (a == 0 || a < Mathf.Abs(c))
        {
            a = c;
            s = p / 4f;
        }
        else
        {
            s = p / (2f * Mathf.PI) * Mathf.Asin(c / a);
        }

        return (a * Mathf.Pow(2, -10f * t)) * Mathf.Sin((t * d - s) * (2 * Mathf.PI) / p) + c + b;
    }

    /// <summary>
    /// 弹性内外曲线
    /// </summary>
    /// <param name="t">初始值与最大值之间的值</param>
    /// <param name="b">初始值</param>
    /// <param name="c">初始值与最大值间隔</param>
    /// <param name="d">最大值</param>
    /// <returns></returns>
    public static float ElasticInOut(float t, float b, float c, float d)
    {
        if (t == 0)
            return b;
        if (t == d)
            return b + c;
        if ((t /= d / 2f) < 1f)
        {
            return c / 2f * Mathf.Pow(2f, 10f * (t - 1f)) + b;
        }
        return c / 2f * (-Mathf.Pow(2f, -10f * --t) + 2f) + b;
    }

    /// <summary>
    /// 2次内曲线
    /// </summary>
    /// <param name="t">初始值与最大值之间的值</param>
    /// <param name="b">初始值</param>
    /// <param name="c">初始值与最大值间隔</param>
    /// <param name="d">最大值</param>
    /// <returns></returns>
    public static float QuadraticIn(float t, float b, float c, float d)
    {
        return c * (t /= d) * t + b;
    }

    /// <summary>
    /// 2次外曲线
    /// </summary>
    /// <param name="t">初始值与最大值之间的值</param>
    /// <param name="b">初始值</param>
    /// <param name="c">初始值与最大值间隔</param>
    /// <param name="d">最大值</param>
    /// <returns></returns>
    public static float QuadraticOut(float t, float b, float c, float d)
    {
        return -c * (t /= d) * (t - 2) + b;
    }

    /// <summary>
    /// 2次内外曲线
    /// </summary>
    /// <param name="t">初始值与最大值之间的值</param>
    /// <param name="b">初始值</param>
    /// <param name="c">初始值与最大值间隔</param>
    /// <param name="d">最大值</param>
    /// <returns></returns>
    public static float QuadraticInOut(float t, float b, float c, float d)
    {
        if ((t /= d / 2) < 1) return c / 2 * t * t + b;
        return -c / 2 * ((--t) * (t - 2) - 1) + b;
    }

    /// <summary>
    /// 三次方内曲线
    /// </summary>
    /// <param name="t">初始值与最大值之间的值</param>
    /// <param name="b">初始值</param>
    /// <param name="c">初始值与最大值间隔</param>
    /// <param name="d">最大值</param>
    /// <returns></returns>
    public static float CubicIn(float t, float b, float c, float d)
    {
        return c * (t /= d) * t * t + b;
    }

    /// <summary>
    /// 三次方内外曲线
    /// </summary>
    /// <param name="t">初始值与最大值之间的值</param>
    /// <param name="b">初始值</param>
    /// <param name="c">初始值与最大值间隔</param>
    /// <param name="d">最大值</param>
    /// <returns></returns>
    public static float CubicOut(float t, float b, float c, float d)
    {
        return c * ((t = t / d - 1) * t * t + 1) + b;
    }

    /// <summary>
    /// 三次方内外曲线
    /// </summary>
    /// <param name="t">初始值与最大值之间的值</param>
    /// <param name="b">初始值</param>
    /// <param name="c">初始值与最大值间隔</param>
    /// <param name="d">最大值</param>
    /// <returns></returns>
    public static float CubicInOut(float t, float b, float c, float d)
    {
        if ((t /= d / 2) < 1) return c / 2 * t * t * t + b;
        return c / 2 * ((t -= 2) * t * t + 2) + b;
    }

    /// <summary>
    /// 四次方内曲线
    /// </summary>
    /// <param name="t">初始值与最大值之间的值</param>
    /// <param name="b">初始值</param>
    /// <param name="c">初始值与最大值间隔</param>
    /// <param name="d">最大值</param>
    /// <returns></returns>
    public static float QuarticIn(float t, float b, float c, float d)
    {
        return c * (t /= d) * t * t * t + b;
    }

    /// <summary>
    /// 四次方外曲线
    /// </summary>
    /// <param name="t">初始值与最大值之间的值</param>
    /// <param name="b">初始值</param>
    /// <param name="c">初始值与最大值间隔</param>
    /// <param name="d">最大值</param>
    /// <returns></returns>
    public static float QuarticOut(float t, float b, float c, float d)
    {
        return -c * ((t = t / d - 1) * t * t * t - 1) + b;
    }

    /// <summary>
    /// 四次方内外曲线
    /// </summary>
    /// <param name="t">初始值与最大值之间的值</param>
    /// <param name="b">初始值</param>
    /// <param name="c">初始值与最大值间隔</param>
    /// <param name="d">最大值</param>
    /// <returns></returns>
    public static float QuarticInOut(float t, float b, float c, float d)
    {
        if ((t /= d / 2) < 1) return c / 2 * t * t * t * t + b;
        return -c / 2 * ((t -= 2) * t * t * t - 2) + b;
    }

    /// <summary>
    /// 五次方内曲线
    /// </summary>
    /// <param name="t">初始值与最大值之间的值</param>
    /// <param name="b">初始值</param>
    /// <param name="c">初始值与最大值间隔</param>
    /// <param name="d">最大值</param>
    /// <returns></returns>
    public static float QuinticIn(float t, float b, float c, float d)
    {
        return c * (t /= d) * t * t * t + b;
    }

    /// <summary>
    /// 五次方外曲线
    /// </summary>
    /// <param name="t">初始值与最大值之间的值</param>
    /// <param name="b">初始值</param>
    /// <param name="c">初始值与最大值间隔</param>
    /// <param name="d">最大值</param>
    /// <returns></returns>
    public static float QuinticOut(float t, float b, float c, float d)
    {
        return -c * ((t = t / d - 1) * t * t * t - 1) + b;
    }

    /// <summary>
    /// 五次方内外曲线
    /// </summary>
    /// <param name="t">初始值与最大值之间的值</param>
    /// <param name="b">初始值</param>
    /// <param name="c">初始值与最大值间隔</param>
    /// <param name="d">最大值</param>
    /// <returns></returns>
    public static float QuinticInOut(float t, float b, float c, float d)
    {
        if ((t /= d / 2) < 1) return c / 2 * t * t * t * t + b;
        return -c / 2 * ((t -= 2) * t * t * t - 2) + b;
    }

    /// <summary>
    /// 回曲线IN
    /// </summary>
    /// <param name="t">初始值与最大值之间的值</param>
    /// <param name="b">初始值</param>
    /// <param name="c">初始值与最大值间隔</param>
    /// <param name="d">最大值</param>
    /// <param name="s"></param>
    /// <returns></returns>
    public static float BackIn(float t, float b,float c,float d,float s)
    {
		if (s == 0) s = 1.70158f;
		return c*(t/=d)*t*((s+1)*t - s) + b;
	}

    /// <summary>
    /// Back曲线OUT
    /// </summary>
    /// <param name="t">初始值与最大值之间的值</param>
    /// <param name="b">初始值</param>
    /// <param name="c">初始值与最大值间隔</param>
    /// <param name="d">最大值</param>
    /// <param name="s"></param>
    /// <returns></returns>
	public static float BackOut(float t,float b,float c,float d,float s)
    {
		if (s == 0) s = 1.70158f;
		return c*((t=t/d-1)*t*((s+1)*t + s) + 1) + b;
	}

    /// <summary>
    /// Back曲线INOUT
    /// </summary>
    /// <param name="t">初始值与最大值之间的值</param>
    /// <param name="b">初始值</param>
    /// <param name="c">初始值与最大值间隔</param>
    /// <param name="d">最大值</param>
    /// <param name="s"></param>
    /// <returns></returns>
	public static float BackInOut(float t,float b,float c,float d,float s)
    {
		if (s == 0) s = 1.70158f;
		if ((t/=d/2) < 1) return c/2*(t*t*(((s*=(1.525f))+1)*t - s)) + b;
		return c/2*((t-=2)*t*(((s*=(1.525f))+1)*t + s) + 2) + b;
	}

    /// <summary>
    /// 弹跳OUT曲线
    /// </summary>
    /// <param name="t"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <param name="d"></param>
    /// <returns></returns>
    public static float BounceOut( float t, float b, float c, float d)
    {
		if ((t/=d) < (1/2.75)) {
			return c*(7.5625f*t*t) + b;
		} else if (t < (2/2.75)) {
			return c*(7.5625f*(t-=(1.5f/2.75f))*t + .75f) + b;
		} else if (t < (2.5f/2.75f)) {
			return c*(7.5625f*(t-=(2.25f/2.75f))*t + .9375f) + b;
		} else {
			return c*(7.5625f*(t-=(2.625f/2.75f))*t + .984375f) + b;
		}
	}

    /// <summary>
    /// 三次赛贝尔曲线
    /// </summary>
    /// <param name="point1">pos1</param>
    /// <param name="point2">pos2</param>
    /// <param name="point3">pos3</param>
    /// <param name="t">时间</param>
    /// <returns></returns>
    public static Vector3 Curve_Bezier3(Vector3 point1, Vector3 point2, Vector3 point3, float t)
    {
        float t2 = t * t;
        float tm1 = 1.0f - t;
        float tm12 = tm1 * tm1;

        Vector3 value = point1 * tm12 + 2.0f * point2 * tm1 * t + point3 * t2;

        return value;
    }

    /// <summary>
    /// 弧线曲线
    /// </summary>
    /// <param name="point1"></param>
    /// <param name="point2"></param>
    /// <param name="point3"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Vector3 Curve(Vector3 start, Vector3 end , float heigh , float rate )
    {
        if (rate > 1)
        {
            return end;
        }
        Vector3 pos = Vector3.Lerp(start, end, rate);
        float rate1 = rate*2f;

        float y = 0;
        if (rate1 > 0.7f) //下落
        {
            rate1 = (rate1-0.7f)/1.3f;
            rate1 = BounceOut(rate1, 0, 1f, 1f);
            y = Mathf.Lerp(heigh + start.y , end.y , rate1);
        }
        else //上升
        {
            rate1 = CubicOut(rate1, 0, 1f, 1f);
            y = Mathf.Lerp(start.y, start.y + heigh, rate1);
        }
        pos.y = y;
        return pos;
    }

    /// <summary>
    /// 四次赛贝尔曲线
    /// </summary>
    /// <param name="point1">Pos1</param>
    /// <param name="point2">Pos2</param>
    /// <param name="point3">Pos3</param>
    /// <param name="point4">Pos4</param>
    /// <param name="t">时间</param>
    /// <returns></returns>
    public static Vector3 Curve_Bezier4(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;
        float tm1 = 1.0f - t;
        float tm12 = tm1 * tm1;
        float tm13 = tm12 * tm1;

        Vector3 value = point1 * tm13 + 3.0f * point2 * t * tm12 + 3.0f * point3 * t2 * tm1 + point4 * t3;

        return value;
    }


}
