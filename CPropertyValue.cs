using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



//  CPropertyValue.cs
//  Author: Lu Zexi
//  2014-01-03



//property value calculate
public class CPropertyValue
{
    public enum eValueType
    {
        NONE = 0,    //无
        VALUE = 2,    //值
        RATE = 3,    //比率
        FINAL_RATE = 4, //最终值
    };

    private CPropertyValue	m_lpNext;   //下一个实例值
    private CPropertyValue	m_lpPrev;   //上一个实例值

    private eValueType m_eValueType;    //数值类型
    private float m_Value;  //实际值
    private float m_FinalValue; //最终值
    private float m_MaxData;    //最大值
    private float m_MinData;    //最小值

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //

    public CPropertyValue( float Value )
    {
        this.m_eValueType = eValueType.VALUE;
        this.m_Value = Value;
        this.m_FinalValue = Value;
        this.m_MinData = 0;
        this.m_MaxData = 0;
        this.m_lpNext = null;
        this.m_lpPrev = null;
    }
	
    /// <summary>
    /// 设置数据
    /// </summary>
    /// <param name="Value"></param>
    public void SetData( float Value )
    {
        m_Value = Value;

        if( null != GetRoot() )
        {
            GetRoot().Calculation();
        }
    }
    
    /// <summary>
    /// 设置最大值
    /// </summary>
    /// <param name="Value"></param>
    public void SetMaxData( float Value )
    {
        m_MaxData = Value;
    }

    /// <summary>
    /// 设置最小值
    /// </summary>
    /// <param name="Value"></param>
    public void SetMinData( float Value )
    {
        m_MinData = Value;
    }

    /// <summary>
    /// 加1
    /// </summary>
    /// <returns></returns>
    public bool Increment()
    {
        this.m_Value++;

        if( ( 0 != m_MaxData ) && ( m_MaxData <m_Value ) )
        {
            -- m_Value;

            return false;
        }
	
        Calculation();
		
        return true;
    }

    
    /// <summary>
    /// 增加值
    /// </summary>
    /// <param name="Value"></param>
    /// <returns></returns>
    public bool Increment( float Value )
    {
        m_Value += Value;

        if( ( 0 != m_MaxData ) && ( m_MaxData <m_Value ) )
        {
            m_Value -= Value;

            return false;
        }

        Calculation();

        return true;
    }

    /// <summary>
    /// 减1
    /// </summary>
    /// <returns></returns>
    public bool Decrement()
    {
        -- m_Value;

        if( ( 0 != m_MinData ) && ( m_MinData> m_Value ) )
        {
            ++ m_Value;

            return false;
        }

        Calculation();

        return true;
    }

    /// <summary>
    /// 减值
    /// </summary>
    /// <param name="Value"></param>
    /// <returns></returns>
    public bool Decrement( float Value )
    {
        m_Value -= Value;

        if( ( 0 != m_MinData ) && ( m_MinData> m_Value ) )
        {
            m_Value += Value;

            return false;
        }

        Calculation();

        return true;
    }
    
    /// <summary>
    /// 增加比率
    /// </summary>
    /// <param name="Value"></param>
    /// <returns></returns>
    public CPropertyValue AddRate( float Value )
    {
        return AddRate( new CPropertyValue( Value ) );
    }

    /// <summary>
    /// 增加值
    /// </summary>
    /// <param name="Value"></param>
    /// <returns></returns>
    public CPropertyValue AddValue( float Value )
    {
        return AddValue( new CPropertyValue( Value ) );
    }

    /// <summary>
    /// 增加比率
    /// </summary>
    /// <param name="lpValue"></param>
    /// <returns></returns>
    public CPropertyValue AddRate( CPropertyValue lpValue )
    {
        if( null == lpValue )
        {
            return null;
        }

        lpValue.m_eValueType = eValueType.RATE;

        return Add( lpValue );
    }

    /// <summary>
    /// 增加值
    /// </summary>
    /// <param name="lpValue"></param>
    /// <returns></returns>
    public CPropertyValue AddValue( CPropertyValue lpValue )
    {
        if( null == lpValue )
        {
            return null;
        }

        lpValue.m_eValueType =  eValueType.VALUE;

        return Add( lpValue );
    }

    /// <summary>
    /// 获取值
    /// </summary>
    /// <returns></returns>
    public float GetData()
    {
        return m_Value;
    }

    /// <summary>
    /// 获取最终值
    /// </summary>
    /// <returns></returns>
    public float GetFinalData()
    {
        return m_FinalValue;
    }

    
    /// <summary>
    /// 获取父节点
    /// </summary>
    /// <returns></returns>
    public CPropertyValue GetRoot()
    {
        CPropertyValue lpPrev = this;

        while( lpPrev != null )
        {
            if( null == lpPrev.m_lpPrev )
            {
                return lpPrev;
            }

            lpPrev = lpPrev.m_lpPrev;
        }

        return null;
    }

    /// <summary>
    /// 删除自身与链表连接
    /// </summary>
    private void Unlink()
    {
        CPropertyValue lpNext = m_lpNext;
        CPropertyValue lpPrev = m_lpPrev;

        CPropertyValue lpRoot = GetRoot();

        if( null != m_lpPrev )
        {
            m_lpPrev.m_lpNext = m_lpNext;
        }

        if( null != m_lpNext )
        {
            m_lpNext.m_lpPrev = m_lpPrev;
        }

        m_lpNext = m_lpPrev = null;

        if( null != lpRoot )
        {
            lpRoot.Calculation();
        }
    }

    /// <summary>
    /// 重新计算
    /// </summary>
    public void Calculation()
    {
        m_FinalValue = m_Value;

        CPropertyValue lpNextValue = m_lpNext;

        while( null != lpNextValue )
        {
            switch( lpNextValue.m_eValueType )
            {
                case eValueType.VALUE: m_FinalValue += lpNextValue.GetData(); break;
                case eValueType.RATE: m_FinalValue += m_Value * lpNextValue.GetData(); break;
            }

            lpNextValue = lpNextValue.m_lpNext;
        }
    }

    
    /// <summary>
    /// 清除所有数据
    /// </summary>
    public void Clear()
    {
        CPropertyValue lpNextValue = m_lpNext;

        while( null != lpNextValue )
        {
            CPropertyValue lpTmp = lpNextValue.m_lpNext;

            lpNextValue.Unlink();

            lpNextValue = lpTmp;
        }
    }

    /// <summary>
    /// 移除自身
    /// </summary>
    public void Remove()
    {
        CPropertyValue root = GetRoot();
        if( root != null )
            root.Remove(this);
    }

    /// <summary>
    /// 删除属性
    /// </summary>
    /// <param name="lpValue"></param>
    /// <returns></returns>
    public void Remove( CPropertyValue lpValue )
    {
        Remove(lpValue.m_Value, lpValue.m_eValueType);
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="value"></param>
    public void Remove(float value , eValueType type)
    {
        CPropertyValue cp = Find(value, type);
        if (cp != null)
        {
            cp.Unlink();
        }
    }

    /// <summary>
    /// 删除比率
    /// </summary>
    /// <param name="value"></param>
    public void RemoveRate(float value)
    {
        Remove(value, eValueType.RATE);
    }

    /// <summary>
    /// 删除值
    /// </summary>
    /// <param name="value"></param>
    public void RemoveValue(float value)
    {
        Remove(value, eValueType.VALUE);
    }

    /// <summary>
    /// 寻找比率
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public CPropertyValue Find(float value , eValueType type)
    {
        //向上遍历
        CPropertyValue lpPrev = this;
        while (lpPrev != null)
        {
            if (Math.Abs(lpPrev.m_Value - value)<0.00001f && (int)lpPrev.m_eValueType == (int)type)
            {
                return lpPrev;
            }

            if (null == lpPrev.m_lpPrev)
            {
                break;
            }
            lpPrev = lpPrev.m_lpPrev;
        }

        //向下遍历
        CPropertyValue lpnext = this;
        while (lpnext != null)
        {
            if (Math.Abs(lpnext.m_Value - value) < 0.000001f && (int)lpnext.m_eValueType == (int)type)
            {
                return lpnext;
            }

            if (null == lpnext.m_lpNext)
            {
                break;
            }
            lpnext = lpnext.m_lpNext;
        }

        return null;
    }

    /// <summary>
    /// 包含属性
    /// </summary>
    /// <param name="lp"></param>
    /// <returns></returns>
    public bool Contains( CPropertyValue lp )
    {
        //向上遍历
        CPropertyValue lpPrev = this;
        while (lpPrev != null)
        {
            if (lpPrev == lp)
                return true;

            if (null == lpPrev.m_lpPrev)
            {
                break;
            }
            lpPrev = lpPrev.m_lpPrev;
        }

        //向下遍历
        CPropertyValue lpnext = this;
        while (lpnext != null)
        {
            if (lpnext == lp)
                return true;

            if (null == lpnext.m_lpNext)
            {
                break;
            }
            lpnext = lpnext.m_lpNext;
        }

        return false;
    }

    /// <summary>
    /// 增加属性值
    /// </summary>
    /// <param name="lpValue"></param>
    /// <returns></returns>
    private CPropertyValue Add( CPropertyValue lpValue )
    {
        CPropertyValue lpLastPos = null;

        if( null != m_lpPrev )
        {
            return null;
        }

        lpLastPos = this;

        while( true )
        {
            if( null == lpLastPos.m_lpNext )
            {
                break;
            }

            lpLastPos = lpLastPos.m_lpNext;
        }
		

        if( null == lpLastPos.m_lpNext )
        {
            lpValue.m_lpPrev			= lpLastPos;
            lpLastPos.m_lpNext			= lpValue;			
        }

        Calculation();

        return lpValue;
    }	

    ////////////////////////////////////////////////////////////////////////////////////////////////////
}
