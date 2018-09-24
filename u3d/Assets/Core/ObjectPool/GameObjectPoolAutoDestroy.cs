using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GameObjectPoolAutoDestroy : MonoBehaviour
{
	public float Destroy_Time = 1;
    private bool mStart = false;
    private float mStartTime = 0;

    void OnEnable()
    {
        mStartTime = Time.time;
        mStart = true;
    }

    void Update()
    {
        if(!mStart)
            return;
        float difTime = Time.time - mStartTime;
        if(difTime > Destroy_Time)
        {
            mStart = false;
            GameObjectPool.Release(this.gameObject);
        }
    }
}

