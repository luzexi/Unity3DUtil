using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//Sigleton with Mono code
public class CSingletonMono<T> : MonoBehaviour , ResetInterface
	where T : MonoBehaviour , ResetInterface
{
	public bool CanBeReset = true;

	private static T m_sInstance;
	public static T instance
	{
		get
		{
			if (m_sInstance == null)
			{
				m_sInstance = (T)FindObjectOfType (typeof(T));
 
				if (m_sInstance == null)
				{
					GameObject obj = new GameObject(typeof(T).Name);
					m_sInstance = obj.AddComponent<T>();
					DontDestroyOnLoad(obj);
				}
				ResetClass.sAllSingleMono.Add(m_sInstance);
			}
 
			return m_sInstance;
		}
	}

	public static void ResetAll()
	{
		for(int i = 0 ; i<ResetClass.sAllSingleMono.Count ;)
		{
			if(!(ResetClass.sAllSingleMono[i] as ResetInterface).Reset())
			{
				i++;
			}
		}
	}

	//destroy all memory of data
	public virtual bool Reset()
	{
		if(m_sInstance == null) return false;
		if(!CanBeReset) return true;

		ResetClass.sAllSingleMono.Remove(m_sInstance);
		GameObject.Destroy(m_sInstance.gameObject);
		m_sInstance = default(T);
		return true;
	}

	public virtual void Awake()
	{
		if(m_sInstance == null)
		{
			m_sInstance = this as T;
		}
	}

	public virtual void OnDestroy()
	{
		if( m_sInstance == this ){
			m_sInstance = default(T);
		}
	}

	public static bool IsValid()
	{
		return ( m_sInstance != null ) ;
	}
}