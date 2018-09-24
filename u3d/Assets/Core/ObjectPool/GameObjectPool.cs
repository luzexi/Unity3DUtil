using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public static class GameObjectPool
{
    // Object pool to avoid allocations.
    private static readonly ObjectPool<GameObject> sGameObjectPool = new ObjectPool<GameObject>(OnGet,OnRelease);
    private static GameObject sRoot = null;

    public static GameObject Get()
    {
        GameObject obj = sGameObjectPool.Get();
        obj.transform.parent = null;
        return obj;
    }

    public static void Release(GameObject toRelease)
    {
        if(sRoot == null)
        {
            sRoot = new GameObject("GameObjectPool");
            GameObject.DontDestroyOnLoad(sRoot);
        }
        toRelease.transform.parent = sRoot.transform;
        sGameObjectPool.Release(toRelease);
    }

    private static void OnGet(GameObject obj)
    {
        obj.SetActive(true);
    }

    public static void OnRelease(GameObject obj)
    {
        obj.SetActive(false);
    }
}


