using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class LineMesh
{
	public class MeshData
	{
		public Vector3[] mVertices = new Vector3[4];
		public int[] mTriangles = new int[6]; // 2 triangle and 3 vertex
	}

	public class LineData
	{
		public Vector3 mStartPos;
		public Vector3 mEndPos;
		public float mWidth;
	}

	public static Mesh MakeMeshLine(List<LineData> _lstLine)
	{
		List<MeshData> lstMeshdata = new List<MeshData>(_lstLine.Count);
		for(int i = 0 ; i<_lstLine.Count ; i++)
		{
			MeshData _meshdata = MakeMeshData(_lstLine[i].mStartPos, _lstLine[i].mEndPos, _lstLine[i].mWidth);
			lstMeshdata.Add(_meshdata);
		}
		return CombineMeshData(lstMeshdata);
	}

	public static MeshData MakeMeshData(Vector3 _startPos, Vector3 _endPos, float _width)
	{
		MeshData m     = new MeshData();

		Vector3 _forward = (_endPos - _startPos).normalized;
		Vector3 _right = Vector3.Cross(_forward, new Vector3(0,0,1));

		m.mVertices[0] = _startPos + _right * _width;
		m.mVertices[1] = _startPos + _right * _width * -1;
		m.mVertices[2] = _endPos + _right * _width;
		m.mVertices[3] = _endPos + _right * _width * -1;

		m.mTriangles[0] = 0;
		m.mTriangles[1] = 1;
		m.mTriangles[2] = 2;

		m.mTriangles[3] = 1;
		m.mTriangles[4] = 3;
		m.mTriangles[5] = 2;

		return m;
	}

	public static Mesh CombineMeshData(List<MeshData> lst)
	{
		Vector3[] _vertices = new Vector3[lst.Count * 4];
		int[] _triangles = new int[lst.Count * 6];
		Color[] _colors = new Color[_vertices.Length];

		for(int i = 0 ; i<lst.Count ; i++)
		{
			_vertices[i*4+0] = lst[i].mVertices[0];
			_vertices[i*4+1] = lst[i].mVertices[1];
			_vertices[i*4+2] = lst[i].mVertices[2];
			_vertices[i*4+3] = lst[i].mVertices[3];

			_triangles[i*6+0] = i*4+0;
			_triangles[i*6+1] = i*4+1;
			_triangles[i*6+2] = i*4+2;

			_triangles[i*6+3] = i*4+1;
			_triangles[i*6+4] = i*4+3;
			_triangles[i*6+5] = i*4+2;
		}

		for(int i = 0 ; i<_colors.Length ; i++)
		{
			_colors[i] = Color.blue;
		}

		Mesh mesh = new Mesh();
		mesh.vertices = _vertices;
        mesh.triangles = _triangles;
		mesh.colors = _colors;
		mesh.RecalculateTangents();
		return mesh;
	}
}
