using UnityEngine;
using System.Collections;

public class TextureTileAnimationWholeMesh : MonoBehaviour
{

    public int XTile = 1;
    public int YTile = 1;
    public float interval = 0.05F;
    public bool loop = true;
    public KeyCode key = KeyCode.F;

    float m_timer = 0;
    int index = 0;
    int tileCount = 1;
    Material _mat;
    // Use this for initialization
    void Start()
    {
        tileCount = XTile * YTile;  
    }

    void OnEnable()
    {
        index = 0;
        m_timer = interval;
    }
	
	void OnDestroy()
	{
		if (_mat != null)
		{
			GameObject.DestroyImmediate(_mat);
		}
	}

    // Update is called once per frame
    Vector2 tileOffset = new Vector2();
    void Update()
    {
        /*if (renderer.enabled == false)
        {
            if (Input.GetKeyDown(key))
            {
                index = 0;
                renderer.enabled = true;
            }
        }
        else*/
        {
            m_timer -= Time.deltaTime;
            //
            if (m_timer < 0)
            {
                while (m_timer < 0)
                {
                    if (index < (tileCount - 1))
                    {
                        index++;
                    }
                    else
                    {
                        if (loop)
                        {
                            index = 0;
                        }
                        else
                        {
                            GetComponent<Renderer>().enabled = false;
                        }
                    }
                    m_timer += interval;
                }
                int x = index % XTile;
                int y = index / XTile;
                tileOffset.x = x / (float)XTile;
                tileOffset.y = -y / (float)YTile;
                GetComponent<Renderer>().material.mainTextureOffset = tileOffset;
				_mat = GetComponent<Renderer>().material;
                //m_timer = interval;
            }
        }
    }
}
