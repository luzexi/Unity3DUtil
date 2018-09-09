using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public class PoolObject : MonoBehaviour
{
	public enum STATE
	{
		NONE,
		USING,
	}
	public STATE mState = STATE.NONE;
	public string mKeyName = "";
}

