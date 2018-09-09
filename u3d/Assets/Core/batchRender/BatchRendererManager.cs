using UnityEngine;
using System.Collections.Generic;

public class BatchRendererManager : MonoBehaviour
{
    static BatchRendererManager sInstance;

    public static BatchRendererManager Instance
    {
        get
        {
            if(sInstance == null)
            {
                GameObject go = new GameObject("BatchRenderer");
                sInstance = go.AddComponent<BatchRendererManager>();
            }
            return sInstance;
        }
    }

    class _Batch
    {
        BatchRendererManager m_manager;
        HashSet<BatchRenderer> m_units = new HashSet<BatchRenderer>();
        GameObject m_go = null;
        MeshFilter m_meshFilter = null;
        MeshRenderer m_meshRenderer = null;
        int m_stablePeriod = 1;
		int m_VertexCount = 0;
        enum Status
        {
            Unstable,
            Stable,
        }
        GameStateMachine<Status> m_sm = new GameStateMachine<Status>();
		public int VertexCount  { get {return m_VertexCount;} }
        public _Batch(BatchRendererManager _manager)
        {
            m_manager = _manager;
            m_sm.AddState(Status.Unstable, new _State_Unstable(this));
            m_sm.AddState(Status.Stable, new _State_Stable(this));
            m_sm.ChangeState(Status.Unstable);
        }
        public _Batch(BatchRendererManager _manager, int _stablePeriod)
            : this(_manager)
        {
            m_stablePeriod = _stablePeriod;
        }
        public void DumpInfo()
        {
            foreach (BatchRenderer u in m_units)
                Debug.Log(u.m_meshRenderer.gameObject.name);
            
        }
        public int UnitCount()
        {
            return m_units.Count;
        }
        public void AddUnit(BatchRenderer _unit)
        {
            m_units.Add(_unit);
			m_VertexCount += _unit.m_meshFilter.sharedMesh.vertexCount;
            m_sm.ChangeState(Status.Unstable);
        }
        public bool RemoveUnit(BatchRenderer _unit)
        {
            if (m_units.Remove(_unit))
            {
				m_VertexCount -= _unit.m_meshFilter.sharedMesh.vertexCount;
                if (m_stablePeriod > 0 &&_unit.m_stablePeriod > 0 && _unit.m_meshRenderer != null)
                    _unit.m_meshRenderer.enabled = true;
                m_sm.ChangeState(Status.Unstable);

				return true;
            }
			return false;
        }
        public void Update()
        {
            m_sm.Update();
        }
        public void Construct(Material _material)
        {
            if (m_go == null)
            {
                m_go = new GameObject(_material.name);
                m_go.transform.parent = m_manager.transform;
                m_go.transform.localPosition = Vector2.zero;
                m_go.transform.localRotation = Quaternion.identity;
            
                m_meshFilter = m_go.AddComponent<MeshFilter>();
                m_meshRenderer = m_go.AddComponent<MeshRenderer>();
            }
            m_meshRenderer.sharedMaterial = _material;
        }
        public void Destroy()
        {
            if (m_meshFilter != null && m_meshFilter.sharedMesh != null)
            {
                Object.Destroy(m_meshFilter.sharedMesh);
                m_meshFilter.sharedMesh = null;
            }
            if (m_go != null && m_go.activeSelf)
            {
                Object.Destroy(m_go);
            }
        }
        public void Group()
        {
            if (m_units.Count > 1)
            {
                CombineInstance[] combine = new CombineInstance[m_units.Count];
                int i = 0;
                foreach (BatchRenderer u in m_units)
                {
                    if (i == 0)
                    {
                        Construct(u.m_meshRenderer.sharedMaterial);
                    }
                    combine[i].mesh = u.m_meshFilter.sharedMesh;
                    combine[i].transform = u.m_meshRenderer.transform.localToWorldMatrix;
                    combine[i].subMeshIndex = 0;
                    u.m_meshRenderer.enabled = false;
                    ++i;
                }
                //if (m_meshFilter.mesh != null)
                //{
                //    Object.Destroy(m_meshFilter.mesh);
                //}
                if (m_meshFilter.sharedMesh == null)
                    m_meshFilter.sharedMesh = new Mesh();
                m_meshFilter.sharedMesh.CombineMeshes(combine);
                m_meshRenderer.enabled = true;
            }
        }
        public void Ungroup()
        {
            //if (m_meshFilter != null && m_meshFilter.mesh != null)
            //{
            //    Object.Destroy(m_meshFilter.mesh);
            //    m_meshFilter.mesh = null;
            //}
            if (m_stablePeriod > 0)
            {
                int i = 0;
                foreach (BatchRenderer u in m_units)
                {
                    if (i == 0)
                    {
                        Construct(u.m_meshRenderer.sharedMaterial);
                        ++i;
                    }
                    u.m_meshRenderer.enabled = true;
                }
                if (m_meshRenderer != null) m_meshRenderer.enabled = false;
            }
        }
        class _State_Unstable : GameStateMachine<Status>.GameState
        {
            _Batch m_batch;
            int m_count = 0;
            public _State_Unstable(_Batch _batch)
            {
                m_batch = _batch;
            }
            public override void Enter()
            {
                m_count = 0;
            }
            public override void Update()
            {
                ++m_count;
                if (m_count >= m_batch.m_stablePeriod)
                {
                    m_fsm.ChangeState(Status.Stable);
                }
            }
        }
        class _State_Stable : GameStateMachine<Status>.GameState
        {
            _Batch m_batch;
            public _State_Stable(_Batch _batch)
            {
                m_batch = _batch;
            }
            public override void Enter()
            {
                m_batch.Group();
            }
            public override void Exit()
            {
                m_batch.Ungroup();
            }
        }

    }
	Dictionary<string, List<_Batch> > m_materialTable = new Dictionary<string, List<_Batch> >();

    void Start()
    {
        transform.position = Vector3.zero;
        Object.DontDestroyOnLoad(this);
    }
    void LateUpdate()
    {
        foreach (var pair in m_materialTable)
        {
			foreach (_Batch b in pair.Value)
				b.Update();
            //if (pair.Key.name.ToLower().Contains("training_com"))
            //{
            //    Debug.Log("----------------------" + pair.Key.name);
            //    pair.Value.DumpInfo();
            //}
        }
    }
    public void AddRenderer(BatchRenderer _batchRenderer, int _stablePeriod = -1)
    {
        BatchRenderer u = _batchRenderer;
        
		_Batch batch;
		List<_Batch> batchList;
		if (m_materialTable.TryGetValue(u.m_meshRenderer.sharedMaterial.name, out batchList))
        {
			int i = batchList.Count - 1;
            while (i >= 0)
            {
                batch = batchList[i];
				if (batch.VertexCount + _batchRenderer.m_meshFilter.sharedMesh.vertexCount < ushort.MaxValue)
                {
                    batch.AddUnit(u);
                    break;
                }
                --i;
            }
            if (i < 0) // all full, need a new batch
            {
				batch = new _Batch(this, _stablePeriod);
                batch.AddUnit(_batchRenderer);
                batchList.Add(batch);
            }
        }
        else
        {
            batch = new _Batch(this, _stablePeriod);
            batch.AddUnit(u);
			batchList = new List<_Batch>();
			batchList.Add(batch);
			m_materialTable.Add(u.m_meshRenderer.sharedMaterial.name, batchList);
        }
    }

    public void RemoveRenderer(BatchRenderer _batchRenderer)
    {  
        List<string> toBeRemoved = new List<string>();
		foreach (var pair in m_materialTable)
        {
            List<_Batch> batchList = pair.Value;
            for (int i = 0 ; i < batchList.Count; i++)
            {
                _Batch batch = batchList[i];
                if (batch != null)
                {
                    if(batch.RemoveUnit(_batchRenderer))
                    {
                        if (batch.UnitCount() == 0)
                        {
                            batch.Destroy();
                            batchList.RemoveAt(i);
                        }
                    }
                }
                else
                {
                    batchList.RemoveAt(i);
                }
            }
            if (pair.Value.Count == 0)
                toBeRemoved.Add(pair.Key);
        }
        foreach (string name in toBeRemoved)
        {
            m_materialTable.Remove(name);
        }
    }

    public void UngroupAll()
    {
        foreach (var pair in m_materialTable)
        {
			foreach (_Batch b in pair.Value)
            {
				b.Ungroup();
            }
            // pair.Value.Group();
        }
    }

    public void GroupAll()
    {
        foreach (var pair in m_materialTable)
        {
			foreach (_Batch b in pair.Value)
            {
				b.Group();
            }
            // pair.Value.Group();
        }
    }

    public void DestroyAll()
    {
        foreach (var pair in m_materialTable)
        {
            foreach (_Batch b in pair.Value)
            {
                b.Destroy();
            }
            // pair.Value.Group();
        }
        m_materialTable.Clear();
    }

}
