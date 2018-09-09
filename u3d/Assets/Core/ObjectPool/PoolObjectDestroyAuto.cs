using UnityEngine;
using System.Collections;
using System.Collections.Generic;



[RequireComponent(typeof(PoolObject))]
public class PoolObjectDestroyAuto : MonoBehaviour
{
	public float Destroy_Time = 1;
	private PoolObject mPoolObj;
    private bool mStart = false;
    private float mStartTime = 0;

	void Start()
    {
    	mPoolObj = GetComponent<PoolObject>();
    }

    void OnEnable()
    {
        mStartTime = Time.time;
        mStart = true;
    	// StartCoroutine(StartDestroy());
    }

    void Update()
    {
        if(!mStart)
            return;
        float difTime = Time.time - mStartTime;
        if(difTime > Destroy_Time)
        {
            mStart = false;
            ObjectPoolManager.instance.DestroyObj(mPoolObj);
        }
    }


    // IEnumerator StartDestroy()
    // {
    //     yield return new WaitForSeconds(Destroy_Time);
    //     ObjectPoolManager.instance.DestroyObj(mPoolObj);
    // }
}

