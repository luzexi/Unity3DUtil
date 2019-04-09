using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GameObjectPool<T>
    where T : MonoBehaviour
{
    // Object pool to avoid allocations.
    protected ObjectPool<T> sGameObjectPool = new ObjectPool<T>(OnGet, OnRelease);
    protected GameObject sRoot = null;

    public GameObjectPool()
    {
        SetCreateFunction(GenerateAsset);
    }

    public T Get()
    {
        T obj = sGameObjectPool.Get();
        obj.transform.parent = null;
        return obj;
    }

    public void Release(T toRelease)
    {
        if(sRoot == null)
        {
            sRoot = new GameObject("GameObjPool-" + this.GetType().ToString());
        }
        toRelease.transform.parent = sRoot.transform;
        sGameObjectPool.Release(toRelease);
    }

    protected virtual T GenerateAsset()
    {
        return default(T);
    }

    protected void SetCreateFunction(ObjectPool<T>.CreateT _create_function)
    {
        sGameObjectPool.m_ActionOnCreate = _create_function;
    }

    private static void OnGet(T obj)
    {
        obj.gameObject.SetActive(true);
    }

    public static void OnRelease(T obj)
    {
        obj.gameObject.SetActive(false);
    }
}


