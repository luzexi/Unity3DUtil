using UnityEngine;
using System.Collections.Generic;

public class BatchRenderer : MonoBehaviour
{
    private int updateCounter = 0;
    public MeshRenderer m_meshRenderer = null;
    public MeshFilter m_meshFilter = null;
    
    Vector3 m_position = new Vector3();
    Quaternion m_rotation = new Quaternion();
    Vector2 m_texOffset = new Vector2();
    public int m_stablePeriod = 1;

    public enum BatchState
    {
        None,
        AddBatch,
        RemoveBatch,
    }
    float mRenderStartTime = 0;
    bool mInitRender = false;
    public BatchState mState = BatchState.None;
    bool mDelayBatch = false;

    public static void DelayBatch(GameObject _go)
    {
        BatchRenderer[] batch_renders = _go.GetComponentsInChildren<BatchRenderer>();
        for(int i = 0 ; i<batch_renders.Length ; i++)
        {
            batch_renders[i].mDelayBatch = true;
        }
    }

    public static void UnDelayBatch(GameObject _go)
    {
        BatchRenderer[] batch_renders = _go.GetComponentsInChildren<BatchRenderer>();
        for(int i = 0 ; i<batch_renders.Length ; i++)
        {
            batch_renders[i].mDelayBatch = false;
        }
    }
    
    public static void PlanBatchRenderer(GameObject _go, int _stablePeriod = 1)
    {
        LoopEachBatch(_go.transform);
    }

    public static void PlanAddBatchRenderer(GameObject _go, int _stablePeriod = 1)
    {
        MeshRenderer[] meshRenders = _go.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < meshRenders.Length; ++i)
        {
            MeshRenderer meshR = meshRenders[i];
            // if (meshR.gameObject.GetComponent<Animation>() != null)
            //     continue;
            // if (meshR.gameObject.GetComponent<Animator>() != null)
            //     continue;
            BatchRenderer br = meshR.gameObject.GetComponent<BatchRenderer>();
            if (br == null)
            {
                if (meshR.sharedMaterials.Length > 1)
                {
                    Debug.LogError("More than one material: " + meshR.gameObject.name);
                }
                if (meshR.sharedMaterial != null)
                {
                    MeshFilter meshF = meshR.gameObject.GetComponent<MeshFilter>();
                    if (meshF.sharedMesh != null)
                    {
                        if (meshF.sharedMesh.subMeshCount > 1)
                        {
                            Debug.LogError("More than one subMesh: " + meshF.gameObject.name);
                        }
                        br = meshF.gameObject.AddComponent<BatchRenderer>();
                        br.m_stablePeriod = _stablePeriod;
                    }
                }
            }
            else
            {
                br.m_stablePeriod = _stablePeriod;
            }
        }
    }

    public static void LoopEachBatch(Transform _transform)
    {
        for(int i = 0 ; i<_transform.childCount ; i++)
        {
            Transform child = _transform.GetChild(i);
            if(child.GetComponent<Animator>() != null)
            {
                continue;
            }
            if(child.GetComponent<MeshRenderer>() != null)
            {
                PlanAddBatchRenderer(child.gameObject,10);
                continue;
            }
            LoopEachBatch(child);
        }
    }

    //will be change when be attack destroy,
    //drag to move, build cancel remove,
    //remove trash, upgrade finished
    //cancel move, cancel mutil walls move
    //edit base switch, edit base remove all
    //edit base drag, edit base release, edit base cancel
    //edit base erase mode remove building
    //edit switch to home
    //edit base exit
    //edit base add new building quickly
    public static void BatchRemoveNow(GameObject go)
    {
        BatchRenderer[] batchs = go.GetComponentsInChildren<BatchRenderer>();
        // batchs._Update();
        for(int i = 0 ; i<batchs.Length ; i++)
        {
            if(batchs[i].mState == BatchState.AddBatch)
            {
                batchs[i].mState = BatchState.RemoveBatch;
                BatchRendererManager.Instance.RemoveRenderer(batchs[i]);
            }
        }
    }

    public static void BatchAddNow(GameObject go)
    {
        BatchRenderer[] batchs = go.GetComponentsInChildren<BatchRenderer>();
        // batchs._Update();
        for(int i = 0 ; i<batchs.Length ; i++)
        {
            if(batchs[i].mState == BatchState.RemoveBatch)
            {
                batchs[i].mState = BatchState.AddBatch;
                BatchRendererManager.Instance.AddRenderer(batchs[i], batchs[i].m_stablePeriod);
            }
        }
    }

    void Awake()
    {
        //
    }
    
    void Start()
    {
        if(m_meshRenderer == null)
        {
            m_meshRenderer = gameObject.GetComponent<MeshRenderer>();
        }
        if (m_meshRenderer.sharedMaterials.Length > 1)
        {
            Debug.LogError("More than one material: " + m_meshRenderer.gameObject.name);
        }
        if (m_meshRenderer.sharedMaterial != null)
        {
            MeshFilter meshF = m_meshRenderer.gameObject.GetComponent<MeshFilter>();
            if (meshF != null)
            {
                m_meshFilter = meshF;
            }
        }
        mRenderStartTime = Time.time;
    }

    void OnDestroy()
    {
        OnDisable();
    }

    public void OnDisable()
    {
        if (BatchRendererManager.Instance != null && BatchRendererManager.Instance.enabled)
            BatchRendererManager.Instance.RemoveRenderer(this);
    }

    public void OnEnable()
    {
        if(mInitRender)
        {
            if (BatchRendererManager.Instance != null && BatchRendererManager.Instance.enabled)
            {
                BatchRendererManager.Instance.AddRenderer(this);
            }
        }
    }

    public void DisableBatch()
    {
        if (BatchRendererManager.Instance != null && BatchRendererManager.Instance.enabled)
            BatchRendererManager.Instance.RemoveRenderer(this);
    }

    void Update()
    {
        if(mDelayBatch) return;
        if(mInitRender) return;
        if (Time.time - mRenderStartTime > 0.3f)
        {
            mInitRender = true;

            if (m_meshRenderer != null && m_meshFilter != null && m_meshFilter.sharedMesh != null && BatchRendererManager.Instance != null)
            {
                Vector3 curPosition = transform.position;
                Quaternion curRotation = transform.rotation;
                Vector2 curTexOffset = Vector2.zero;
                // if (m_meshRenderer.material != null)
                //     curTexOffset = m_meshRenderer.material.GetTextureOffset("_MainTex");
                // //if ((curPosition - m_position).sqrMagnitude > 0.00001f) // moving
                // if (curPosition != m_position || curRotation != m_rotation || curTexOffset != m_texOffset)
                // {
                //     if (m_stableCount > m_stablePeriod && m_stablePeriod > 0) // if period is 0, it will be added in this frame, no need to remove
                //     {
                //         BatchRendererManager.Instance.RemoveRenderer(this);
                //     }
                //     m_stableCount = 0;
                // }
                m_position = curPosition;
                m_rotation = curRotation;
                m_texOffset = curTexOffset;

                BatchRendererManager.Instance.AddRenderer(this, m_stablePeriod);
                mState = BatchState.AddBatch;
            }

        }
    }
}
