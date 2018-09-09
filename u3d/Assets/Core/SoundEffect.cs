using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundEffect : MonoBehaviour
{	
	public AudioSource mAudioSource;
	public AudioSource mBgmAudioSource;

	//===== bgm
	// private float lastTime = 0;
	// private int updateCount = 0;
	// private const int updateInterval = 30;
	private float mBgmVol = 0;
	public float BgmVol
	{
		get
		{
			return mBgmVol;
		}
	}
	// private int trackIndex = 0;
	// private float muteTime = 30;
	private const float MAX_VOLUME = 1;
	private const float FADE_SPEED = 0.3f;
	private float mCurFadeSpeed = 0.3f;
	private bool mIsFading = false;
	private AudioClip mNextClip;
	//=======

	private bool mEnableSfx = true;
	public bool EnableSfx
	{
		get
		{
			return mEnableSfx;
		}
		set
		{
			mEnableSfx = value;
		}
	}
	private bool mEnableBgm = true;
	public bool EnableBgm
	{
		get
		{
			return mEnableBgm;
		}
		set
		{
			mEnableBgm = value;
		}
	}
	
	private Dictionary<string,AudioClip> mSfxs = new Dictionary<string,AudioClip>();
	private Dictionary<string,AudioClip> mBgms = new Dictionary<string,AudioClip>();
	
	void Awake()
	{
		if(mAudioSource == null)
			mAudioSource = GetComponent<AudioSource>();
	}

	void Update()
	{
		if (mIsFading)
		{
			mBgmVol += Time.deltaTime * mCurFadeSpeed;

			if(mCurFadeSpeed > 0)
			{
				if (mBgmVol > MAX_VOLUME)
				{
					mBgmVol = MAX_VOLUME;
					mIsFading = false;
				}
			}
			else
			{
				if (mBgmVol < 0)
				{
					mBgmVol = 0;
					mIsFading = false;
				}
			}
            mBgmAudioSource.volume = mBgmVol;
            if(mBgmVol <= 0)
            {
            	StopBgm();
            }
		}
		else
		{
			if(mNextClip != null)
			{
				FadeIn(mNextClip);
				mNextClip = null;
			}
		}
	}
	
	//================================= sfx
	public void ClearSfx()
	{
		mSfxs.Clear();
	}

	public void StopSfx()
	{
		if(mAudioSource != null)
		{
			mAudioSource.Stop();
		}
	}

    public void PlaySfx(string _clipFileName)
    {
        AudioClip clip;
        if(!mSfxs.TryGetValue(_clipFileName, out clip))
        {
        	clip = Resources.Load("sound/"+_clipFileName) as AudioClip;
        	mSfxs.Add(_clipFileName, clip);
        }
        PlaySfx(clip);
    }

	public void PlaySfx(int _clipId)
	{
		// TableSoundInfo.Data data = ClientTableDataManager.Instance.GetSoundData((int)_clipId);
	}

	public void PlaySfx(AudioClip _clip)
	{
		// Debug.LogError("error clip name " + _clip.name);
    	// return;   
        if (mAudioSource && _clip && mEnableSfx)
        {
        	mAudioSource.PlayOneShot(_clip);
        }
	}

	//======================= bgm
	public void PlayBgm(string _filename)
	{
		AudioClip clip;

		if(!mBgms.TryGetValue(_filename, out clip))
		{
			clip = Resources.Load("sound/"+_filename) as AudioClip;
			mBgms.Add(_filename,clip);
		}
		FadeIn(clip);
	}

	public void PlayBgmFadeOutIn(string _filename)
	{
		AudioClip clip;

		if(!mBgms.TryGetValue(_filename, out clip))
		{
			clip = Resources.Load("sound/"+_filename) as AudioClip;
			mBgms.Add(_filename,clip);
		}
		FadeOutIn(clip);
	}

	public void ClearBgm()
	{
		mBgms.Clear();
	}

	public void StopBgm()
	{
		if(mBgmAudioSource != null)
		{
			mBgmAudioSource.Stop();
		}
	}

	public void FadeIn(AudioClip _clip)
	{
        if (_clip && mEnableBgm)
        {
            mBgmVol = 0;
            mBgmAudioSource.volume = mBgmVol;
            mBgmAudioSource.loop = true;
            mBgmAudioSource.clip = _clip;
            mBgmAudioSource.Play();

            mIsFading = true;
            mCurFadeSpeed = FADE_SPEED;
        }
	}

	public void FadeOutIn(AudioClip _clip)
	{
		FadeOut();
		mNextClip = _clip;
	}
	
	public void FadeOut()
	{
		mIsFading = true;
        mCurFadeSpeed = FADE_SPEED * -1f;
	}
}

