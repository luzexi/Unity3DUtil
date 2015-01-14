using UnityEngine;
using System.Collections;

//	CSingletonMono.cs
//	Author: Lu Zexi
//	2014-12-07


//Sigleton with Mono code
public class CSingletonMono<T> : MonoBehaviour
	where T : MonoBehaviour
{
	private static T m_sInstance;
	public static T sInstance
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
				}
			}
 
			return m_sInstance;
		}
	} 

	void OnDestroy()
	{
		if( m_sInstance == this ){
			m_sInstance = default(T);
		}
	}

	public static bool IsValid()
	{
		return ( sInstance != null ) ;
	}
}