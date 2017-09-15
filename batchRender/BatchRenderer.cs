using UnityEngine;
using System.Collections.Generic;

public class BatchRenderer : MonoBehaviour
{
    private int updateCounter = 0;
    public static void PlantBatchRenderer(GameObject _go, int _stablePeriod = 1, int _batchStablePeriod = 1)
    {
        // return;
        MeshRenderer[] meshRenders = _go.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < meshRenders.Length; ++i)
        {
            MeshRenderer meshR = meshRenders[i];
            if (meshR.gameObject.GetComponent<Animation>() != null)
                continue;
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
                        br.m_batchStablePeriod = _batchStablePeriod;
                    }
                }
            }
            else
            {
                br.m_stablePeriod = _stablePeriod;
                br.m_batchStablePeriod = _batchStablePeriod;
            }
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
    public static void _UpdateBatchRemove(GameObject go)
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

    public static void _UpdateBatchAdd(GameObject go)
    {
        BatchRenderer[] batchs = go.GetComponentsInChildren<BatchRenderer>();
        // batchs._Update();
        for(int i = 0 ; i<batchs.Length ; i++)
        {
            if(batchs[i].mState == BatchState.RemoveBatch)
            {
                batchs[i].mState = BatchState.AddBatch;
                BatchRendererManager.Instance.AddRenderer(batchs[i], batchs[i].m_batchStablePeriod);
            }
        }
    }

    public MeshRenderer m_meshRenderer = null;
    public MeshFilter m_meshFilter = null;

    //List<BatchRendererUnit> m_units = new List<BatchRendererUnit>();
    
    Vector3 m_position = new Vector3();
    Quaternion m_rotation = new Quaternion();
    Vector2 m_texOffset = new Vector2();
    int m_stableCount = 0;
    public int m_stablePeriod = 1;
    public int m_batchStablePeriod = 1;

    public enum BatchState
    {
        None,
        AddBatch,
        RemoveBatch,
    }
    float mRenderStartTime = 0;
    bool mInitRender = false;
    public BatchState mState = BatchState.None;
    //public List<BatchRendererUnit> Units
    //{
    //    get
    //    {
    //        return m_units;
    //    }
    //}
    
    void Start()
    {
        if (transform.root.name == "All UI")
            return;
        m_meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (m_meshRenderer.sharedMaterials.Length > 1)
        {
            Debug.LogError("More than one material: " + m_meshRenderer.gameObject.name);
        }
        if (m_meshRenderer.sharedMaterial != null)
        {
            MeshFilter meshF = m_meshRenderer.gameObject.GetComponent<MeshFilter>();
            if (meshF != null)
            {
                //if (meshF.sharedMesh.subMeshCount > 1)
                //{
                //    Debug.LogError("More than one subMesh: " + meshF.gameObject.name);
                //}
                m_meshFilter = meshF;
                //m_unit = new BatchRendererUnit(meshR, meshF.mesh);
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
        m_stableCount = 0;
    }

    public void DisableBatch()
    {
        if (BatchRendererManager.Instance != null && BatchRendererManager.Instance.enabled)
            BatchRendererManager.Instance.RemoveRenderer(this);
        m_stableCount = 0;
        enabled = false;
    }
    public void ResetBatch()
    {
        m_stableCount = 0;
    }

    void Update()
    {
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

                BatchRendererManager.Instance.AddRenderer(this, m_batchStablePeriod);
                mState = BatchState.AddBatch;
            }

        }
    }


    void _Update()
    {
        if (updateCounter++ > 5)
        {
            updateCounter = 0;

            if (m_meshRenderer != null && m_meshFilter != null && m_meshFilter.sharedMesh != null && BatchRendererManager.Instance != null)
            {
                Vector3 curPosition = transform.position;
                Quaternion curRotation = transform.rotation;
                Vector2 curTexOffset = Vector2.zero;
                if (m_meshRenderer.material != null)
                    curTexOffset = m_meshRenderer.material.GetTextureOffset("_MainTex");
                //if ((curPosition - m_position).sqrMagnitude > 0.00001f) // moving
                if (curPosition != m_position || curRotation != m_rotation || curTexOffset != m_texOffset)
                {
                    if (m_stableCount > m_stablePeriod && m_stablePeriod > 0) // if period is 0, it will be added in this frame, no need to remove
                    {
                        BatchRendererManager.Instance.RemoveRenderer(this);
                    }
                    m_stableCount = 0;
                }
                m_position = curPosition;
                m_rotation = curRotation;
                m_texOffset = curTexOffset;

                if (m_stableCount < m_stablePeriod)
                {
                    ++m_stableCount;
                }
                else if (m_stableCount == m_stablePeriod)
                {
                    BatchRendererManager.Instance.AddRenderer(this, m_batchStablePeriod);
                    ++m_stableCount;
                }
                else
                {// nothing, it's stable now
                }
            }

        }
		
    }
}

//public class BatchRendererUnit
//{
    
//    public BatchRendererUnit(MeshRenderer _meshRenderer, Mesh _mesh)
//    {
//        m_meshRenderer = _meshRenderer;
//        m_mesh = _mesh;
//    }
//}