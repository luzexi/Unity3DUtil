using UnityEngine;
using System.Text;

public struct SecureString
{
    string m_value;
    int m_code;

    public SecureString(string _str)
    {
        if (_str == null)
        {
            m_value = null;
            m_code = 0;
            return;
        }
        byte[] b = Encoding.UTF8.GetBytes(_str);
        m_value = System.Convert.ToBase64String(b);
        m_code = _str.GetHashCode();
    }
    static SecureString NormalToSecure(string _str)
    {
        return new SecureString(_str);
    }
    static string SecureToNormal(SecureString _str)
    {
        if (_str.m_value == null)
            return null;
        byte[] b = System.Convert.FromBase64String(_str.m_value);
        string result = Encoding.UTF8.GetString(b, 0, b.Length);
        if (_str.m_code != result.GetHashCode())
        {
            Application.Quit();
            throw new UnityException("s problem");
        }
        return result;
    }
    public static implicit operator SecureString(string _str)
    {
        return NormalToSecure(_str);
    }
    public static implicit operator string(SecureString _str)
    {
        return SecureToNormal(_str);
    }
    public override string ToString()
    {
        return SecureToNormal(this);
    }
    public char[] ToCharArray()
    {
        return SecureToNormal(this).ToCharArray();
    }

    public static bool operator !=(SecureString _left, SecureString _right)
    {
        return _left.m_value != _right.m_value;
    }
    public static bool operator ==(SecureString _left, SecureString _right)
    {
        return _left.m_value == _right.m_value;
    }
}

public struct SecureFloat
{
    SecureInt m_value;

    public SecureFloat(float _float)
    {
        byte[] bs = System.BitConverter.GetBytes(_float);
        int i = System.BitConverter.ToInt32(bs, 0);
        m_value = new SecureInt(i);
    }
    static SecureFloat NormalToSecure(float _f)
    {
        return new SecureFloat(_f);
    }
    static float SecureToNormal(SecureFloat _f)
    {
        int i = _f.m_value;
        byte[] bs = System.BitConverter.GetBytes(i);
        return System.BitConverter.ToSingle(bs, 0);
    }
    public static implicit operator SecureFloat(float _f)
    {
        return NormalToSecure(_f);
    }
    public static implicit operator float(SecureFloat _f)
    {
        return SecureToNormal(_f);
    }
    public override string ToString()
    {
        return SecureToNormal(this).ToString();
    }
}

public struct SecureInt
{
    static class SecureIntRandom
    {
        static uint seed = (uint)(System.DateTime.Now.Millisecond % 0xffffff);
        static uint m_w = seed;    /* must not be zero, nor 0x464fffff */
        static uint m_z = ~seed;    /* must not be zero, nor 0x9068ffff */

        static uint get_random()
        {
            //if (seed == 0)
            //{
            //    seed = 
            //    m_w = seed;
            //    m_z = ~seed;
            //}
            m_z = 36969 * (m_z & 65535) + (m_z >> 16);
            m_w = 18000 * (m_w & 65535) + (m_w >> 16);
            uint result = (m_z << 16) + m_w;  /* 32-bit result */
            return result;
        }
        public static int Int(int min, int max)
        {
            if (max <= min) return min;

            int random = (int)get_random();
            random = random < 0 ? -random : random;
            return min + random % (max - min);
        }
        
    }


    const int UPPER = 65536 * 1024;
    const int LOWER = 65536;
    static int R = SecureIntRandom.Int(UPPER / 3, UPPER * 2 / 3);

    long m_value;
    int m_random;

    int V
    {
        get
        {
            int random = ~m_random - R;
//#if UNITY_EDITOR
            if (m_value % random != 0)
            {
                Application.Quit();
                throw new UnityException("si 0 problem");
            }
                //Debug.LogError("SecureInt problem");
//#endif
            return (int)(m_value / random);
        }
        set
        {
			m_value = value;
            int random = SecureIntRandom.Int(LOWER, UPPER);
            m_value *= random;
            m_random = ~(random + R);
        }
    }
    public int CompareTo(SecureInt _s)
    {
        return V.CompareTo(_s.V);
    }
    public SecureInt(int _init)
    {
        m_value = 0;
        m_random = SecureIntRandom.Int(LOWER, UPPER);
        V = _init;
    }
    public static implicit operator SecureInt(int _value)
    {
        return new SecureInt(_value);
    }
    public static implicit operator int(SecureInt _value)
    {
        return _value.V;
    }

    // public static implicit operator EBuildingType(SecureInt _value)
    // {
    //     return (EBuildingType)_value.V;
    // }
    // public static implicit operator ResType(SecureInt _value)
    // {
    //     return (ResType)_value.V;
    // }
    // public static implicit operator UnitManager.UnitID(SecureInt _value)
    // {
    //     return (UnitManager.UnitID)_value.V;
    // }
    // public static implicit operator InuDefine.EInuSkill(SecureInt _value)
    // {
    //     return (InuDefine.EInuSkill)_value.V;
    // }
    // public static implicit operator HeroManager.HeroUnlockType(SecureInt _value)
    // {
    //     return (HeroManager.HeroUnlockType)_value.V;
    // }
    // public static implicit operator HeroManager.HeroID(SecureInt _value)
    // {
    //     return (HeroManager.HeroID)_value.V;
    // }
    
    
    
    public static int operator /(SecureInt _left, int _right)
    {
        return _left.V / _right;
    }
    public static int operator /(int _left, SecureInt _right)
    {
        return _left / _right.V;
    }
    public static int operator /(SecureInt _left, SecureInt _right)
    {
        return _left.V / _right.V;
    }
    //public static bool operator >(long _left, SecureInt right)
    //{
    //    return _left > right.V;
    //}
    //public static bool operator <(long _left, SecureInt right)
    //{
    //    return _left < right.V;
    //}
    public static bool operator >(SecureInt _left, SecureInt right)
    {
        return _left.V > right.V;
    }
    public static bool operator >(int _left, SecureInt right)
    {
        return _left > right.V;
    }
    public static bool operator <(int _left, SecureInt right)
    {
        return _left < right.V;
    }
    public static bool operator <(SecureInt _left, SecureInt right)
    {
        return _left.V < right.V;
    }
    public static bool operator >(SecureInt _left, int right)
    {
        return _left.V > right;
    }
    public static bool operator <(SecureInt _left, int right)
    {
        return _left.V < right;
    }

    public static bool operator >=(int _left, SecureInt right)
    {
        return _left >= right.V;
    }
    public static bool operator <=(int _left, SecureInt right)
    {
        return _left <= right.V;
    }
    public static bool operator >=(SecureInt _left, int right)
    {
        return _left.V >= right;
    }
    public static bool operator <=(SecureInt _left, int right)
    {
        return _left.V <= right;
    }
    public static bool operator >=(SecureInt _left, SecureInt right)
    {
        return _left.V >= right.V;
    }
    public static bool operator <=(SecureInt _left, SecureInt right)
    {
        return _left.V <= right.V;
    }


    
    

    
    
    public static int operator +(int _left, SecureInt _right)
    {
        return _left + _right.V;
    }
    public static int operator +(SecureInt _left, int _right)
    {
        return _left.V + _right;
    }
    public static int operator +(SecureInt _left, SecureInt _right)
    {
        return _left.V + _right.V;
    }

    public static int operator -(int _left, SecureInt _right)
    {
        return _left - _right.V;
    }
    public static int operator -(SecureInt _left, SecureInt _right)
    {
        return _left.V - _right.V;
    }
    public static int operator -(SecureInt _left, int _right)
    {
        return _left.V - _right;
    }
    public static bool operator ==(SecureInt _left, int _right)
    {
        return _left.V == _right;
    }
    public static bool operator ==(SecureInt _left, SecureInt _right)
    {
        return _left.V == _right.V;
    }
    public static bool operator !=(SecureInt _left, int _right)
    {
        return _left.V != _right;
    }
    public static bool operator !=(SecureInt _left, SecureInt _right)
    {
        return _left.V != _right.V;
    }

    public static int operator *(SecureInt _left, int _right)
    {
        return _left.V * _right;
    }
    public static int operator *(int _left, SecureInt _right)
    {
        return _left * _right.V;
    }
    public static int operator *(SecureInt _left, SecureInt _right)
    {
        return _left.V * _right.V;
    }

    public static SecureInt operator ++(SecureInt _s)
    {
        return new SecureInt(_s.V + 1);
    }
    public static SecureInt operator --(SecureInt _s)
    {
        return new SecureInt(_s.V - 1);
    }


    public override string ToString()
    {
        return V.ToString();
    }
}