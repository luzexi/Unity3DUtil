using System.Collections;
using System.Collections.Generic;

//Sigleton
public class CSingleton<T> : ResetInterface
    where T : ResetInterface , new()
{
	public bool CanBeReset = true;

	private static T m_sInstance;
	public static T instance
	{
		get
		{
			if(m_sInstance == null)
			{
				m_sInstance = new T();
				ResetClass.sAllSingle.Add(m_sInstance);
			}
			return m_sInstance;
		}
	}

	public static void ResetAll()
	{
		for(int i = 0 ; i<ResetClass.sAllSingle.Count ;)
		{
			if(!(ResetClass.sAllSingle[i] as ResetInterface).Reset())
			{
				i++;
			}
		}
	}

	//destroy all memory of data
	public virtual bool Reset()
	{
		if(m_sInstance == null) return false;
		if(!CanBeReset) return false;

		ResetClass.sAllSingle.Remove(m_sInstance);
		m_sInstance = default(T);
		return true;
	}
}

public interface ResetInterface
{
	bool Reset();
}

public class ResetClass
{
	public static List<object> sAllSingle = new List<object>();
	public static List<object> sAllSingleMono = new List<object>();
}