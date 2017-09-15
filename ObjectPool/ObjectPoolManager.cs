using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ObjectPoolManager
{
	static ObjectPoolManager sInstance;
	
	public static ObjectPoolManager instance
	{
		get
		{
			if (sInstance == null)
			{
				sInstance = new ObjectPoolManager();
			}
			return sInstance;
		}
	}

	// cache the bullet type Ammo, Dynamic Pool
    Dictionary<string, List<PoolObject>> mObjCache = new Dictionary<string, List<PoolObject>>();
    public delegate GameObject GetResources(string name, bool ignoreConfig);
	
	// root of all ammos
	GameObject mRoot = null;
    int m_uniqueIdCounter = 0;
	
	public GameObject root
	{
		get
		{
			if (mRoot == null)
			{
				mRoot = new GameObject("ObjectPool");
                if (null != mRoot)
                {
                    mRoot.name = "objRoot";
                    mRoot.transform.parent = null;
                    mRoot.transform.localRotation = Quaternion.identity;
                }
			}
			return mRoot;
		}
	}
	
	public void Clear()
	{
		mObjCache.Clear();
		if (mRoot)
		{
			GameObject.Destroy(mRoot);
			mRoot = null;
		}
        m_uniqueIdCounter = 0;
	}

	int GetObjectId()
    {
        int result = m_uniqueIdCounter++;
        return result;
    }

    public void DestroyObj(PoolObject obj)
    {
    	if(obj == null) return;
    	string objName = obj.mKeyName;
    	List<PoolObject> objs = null;
    	if (!mObjCache.ContainsKey(objName))
        {
        	return;
        }
        objs = mObjCache[objName];
        for(int i = 0 ; i<objs.Count ;)
        {
        	if(objs[i] == null || objs[i].gameObject == null)
        	{
        		objs.Remove(objs[i]);
        	}
        	else
        	{
        		i++;
        	}
        }

        obj.mState = PoolObject.STATE.NONE;
        obj.gameObject.SetActive(false);
        if(root)
        {
        	obj.transform.SetParent(root.transform);
        }
    }

    // public GameObject GetEffectInPool(string effectName,bool ignoreConfig)
    // {
    // 	return GetObjectInPool("effect|"+effectName,
    // 		InuResources.GetEffectInstanceEx, ignoreConfig);
    // }

    // public GameObject GetMsicInPool(string msicName,bool ignoreConfig)
    // {
    // 	return GetObjectInPool("msic|"+msicName,
    // 		InuResources.GetInuMiscInstanceEx, ignoreConfig);
    // }

    // public GameObject GetApAddEffectInPool(GetResources func, bool ignoreConfig = false)
    // {
    // 	return GetObjectInPool("ap|apPoint",
    // 		func, ignoreConfig);
    // }

    GameObject GetObjectInPool(string objName, GetResources func, bool ignoreConfig)
    {
        List<PoolObject> objs;
        PoolObject obj = null;
        string objInstanceName = objName.Split('|')[1];

        if (mObjCache.ContainsKey(objName))
        {
            objs = mObjCache[objName];
        }
        else
        {
            objs = new List<PoolObject>();
            mObjCache.Add(objName, objs);
        }

        // find the available ammo for bullet pool
        for (int i = 0; i < objs.Count; i++)
        {
            if (objs[i].mState == PoolObject.STATE.NONE) 
            {
                obj = objs[i];
                if (null != obj)
                    obj.gameObject.SetActive(true);
                break;
            }
        }
        // if cannot find the available one in pool, create a new one
        if (obj == null)
        {
            GameObject go = func(objInstanceName,ignoreConfig);
            if (go)
            {
                obj = go.GetComponent<PoolObject>();
                if (obj)
                {
                    obj.name = objInstanceName;
                    obj.mKeyName = objName;
                    // if (root)
                    //     obj.transform.SetParent(root.transform);
                    objs.Add(obj);
                }
                else
                {
                	return go;
                }
            }
        }

        if(obj != null)
        {
        	obj.mState = PoolObject.STATE.USING;
        	return obj.gameObject;
        }
        return null;
    }

}