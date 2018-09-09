using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTest : MonoBehaviour
{
	public SoundEffect soundEffect;

	// Use this for initialization
	void Start ()
	{	
	}
	
	// Update is called once per frame
	void Update ()
	{
		//	
	}

	void OnGUI()
	{
		GUI.Label(new Rect(200, 10, 150, 40), "bgm vol " + soundEffect.BgmVol);

		if(GUI.Button(new Rect(10, 10, 150, 100), "play sfx"))
		{
			soundEffect.PlaySfx("Acquire_Quest");
		}

		if (GUI.Button(new Rect(10, 110, 150, 100), "play bgm"))
		{
			soundEffect.PlayBgm("Master Menu Theme");
		}

		if (GUI.Button(new Rect(10, 210, 150, 100), "play bgm next"))
		{
			Debug.LogError("bgm next");
			soundEffect.PlayBgmFadeOutIn("Master Menu Theme");
		}
	}
}
