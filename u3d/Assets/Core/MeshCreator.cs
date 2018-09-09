using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshCreator  {
	
	static Mesh meshBuff;
	
	// this function is used not to create a new mesh, but use exist mesh object, change its vertices, uv etc.
	// after function end, should be followed with EndMesh()
	// if not exist mesh, invoke straightly createLine() or createQuad()
	public static void BeginMesh(Mesh buff)
	{
		meshBuff = buff;
        ClearQuadBuff();
	}

	public static void EndMesh()
	{
		meshBuff = null;
        ClearQuadBuff();
	}
	
	#region Signel Line
	// create a single line ,start, end is the line point in 3D
	// up is line's normal
	public static Mesh createLine(Vector3 start, Vector3 end, Vector3 up, float lineWidth)
	{
		Vector3 side = Vector3.Cross(up, end-start);
		side.Normalize();

		Vector3 a = start + side * (lineWidth / 2);
		Vector3 b = start + side * (lineWidth / -2);
		Vector3 c = end + side * (lineWidth / 2);
		Vector3 d = end + side * (lineWidth / -2);
		
		Mesh mesh = createQuad(a, b, d, c);
		mesh.name = "Line";
		return mesh;
	}
	#endregion
		
	#region Multiple Lines
	
	static List<Vector3> lineVectors = new List<Vector3>();
	
	// if want to use createLines function, should invoke this in the beginning
	public static void BeginLine()
	{
		ClearLineBuff();
	}
	
	// if want to use createLines function, should invoke this after BeginLine()
	public static void AddLinePoint(Vector3 point)
	{
		lineVectors.Add(point);
	}
	
	// createLines function, clear line point's buffer, not public
	private static void ClearLineBuff()
	{
		lineVectors.Clear();
	}
	
	// createLines function
	// up is line's normal
	public static Mesh createLines(Vector3 up, float lineWidth, int lineNumber)
	{
		Mesh mesh = meshBuff? meshBuff: new Mesh();
		int nodeCount = lineVectors.Count;
		if (nodeCount > 1 && nodeCount > lineNumber)
		{
			int quadCount = lineNumber;
			Vector3[] vertices = new Vector3[4 * quadCount];
			
			Vector2[] uv = new Vector2[4 * quadCount];
			int[] triangles = new int[6 * quadCount];
			
			Vector3 start, end, a, b, c, d, side;
			int pointIndex = 0;
			for (int i=0; i<quadCount; i++)
			{
				if (pointIndex >= nodeCount - 1)
				{
					break;
				}
				
				start = lineVectors[pointIndex];
				end = lineVectors[pointIndex+1];
				pointIndex++;
				
				// invalid point for breaking different quads
				if (end.x == -100 && end.y == -100 && end.z == -100)
				{
					pointIndex++;
					i--;
					continue;
				}
				
				side = Vector3.Cross(up, end-start);
				side.Normalize();

                //end -= side * lineWidth;

				a = start + side * lineWidth;
				b = start;
				c = end + side * lineWidth;
				d = end;
				
				vertices[i * 4] = a;
				vertices[i * 4 + 1] = b;
				vertices[i * 4 + 2] = d;
				vertices[i * 4 + 3] = c;
				
				uv[i * 4] = new Vector2(0, 0);
				uv[i * 4 + 1] = new Vector2(1, 0);
				uv[i * 4 + 2] = new Vector2(1, 1);
				uv[i * 4 + 3] = new Vector2(0, 1);
				
				triangles[i * 6] = i * 4;
				triangles[i * 6 + 1] = i * 4 + 1;
				triangles[i * 6 + 2] = i * 4 + 2;
				triangles[i * 6 + 3] = i * 4 + 2;
				triangles[i * 6 + 4] = i * 4 + 3;
				triangles[i * 6 + 5] = i * 4;
			}
			
			mesh.Clear();
			mesh.vertices = vertices;
			mesh.uv = uv;
			mesh.triangles = triangles;
			mesh.RecalculateNormals();
			mesh.name = "Lines";
		}
		
		ClearLineBuff();
		return mesh;
	}
	
	// create a Quad's line mesh
	// the function will clear and create new line points 
	public static Mesh createQuadLine(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Vector3 up, float lineWidth)
	{
		BeginLine();
		AddLinePoint(v1);
		AddLinePoint(v2);
		AddLinePoint(v3);
		AddLinePoint(v4);
		AddLinePoint(v1);
		
		Mesh mesh = createLines(up, lineWidth, 4);
		mesh.name = "QuadLine";
		return mesh;
	}
	
	#endregion
	
	#region Singal Quad
	// create a single quad, v1 v2 v3 v4 should be in order of clockwise
	public static Mesh createQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
	{
		Mesh mesh = meshBuff? meshBuff: new Mesh();
		mesh.vertices = new Vector3[4] {v1, v2, v3, v4};
		mesh.uv = new Vector2[4] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1)};
		mesh.triangles = new int[6] {0, 1, 2, 2, 3, 0};
		mesh.RecalculateNormals();
		mesh.name = "Quad";
		return mesh;
	}
	#endregion
	
	#region Multi Quads
	
	static List<Rect> lineRects = new List<Rect>();
    static List<Color> lineColors = new List<Color>();
	
	// if want to use createLines function, should invoke this in the beginning
	public static void BeginMultiQuads()
	{
		ClearQuadBuff();
	}
	
	// if want to use createLines function, should invoke this after BeginLine()
	public static void AddQuad(Rect rect)
	{
		lineRects.Add(rect);
	}

    public static void AddQuad(Rect rect, Color color)
    {
        AddQuad(rect, color, color, color, color);
    }
    public static void AddQuad(Rect rect, Color color1, Color color2, Color color3, Color color4)
    {
        lineRects.Add(rect);
        lineColors.Add(color1);
        lineColors.Add(color2);
        lineColors.Add(color3);
        lineColors.Add(color4);
    }
	
	// createLines function, clear line point's buffer, not public
	private static void ClearQuadBuff()
	{
		lineRects.Clear();
        lineColors.Clear();
	}
	
	// up is Quad's normal
	public static Mesh createQuads(Vector3 up)
	{
		Mesh mesh = meshBuff? meshBuff: new Mesh();
		int quadCount = lineRects.Count;
		if (quadCount > 0)
		{
			Vector3[] vertices = new Vector3[4 * quadCount];
			Vector2[] uv = new Vector2[4 * quadCount];
			int[] triangles = new int[6 * quadCount];
			Rect rect;
			Vector3 v1, v2, v3, v4;

			for (int i=0; i<quadCount; i++)
			{
				rect = lineRects[i];
				
				v1 = new Vector3(rect.x, 0, rect.y);
				v2 = new Vector3(rect.x, 0, rect.y + rect.height);
				v3 = new Vector3(rect.x + rect.width, 0, rect.y + rect.height);
				v4 = new Vector3(rect.x + rect.width, 0, rect.y);
				
				vertices[i * 4] = v1;
				vertices[i * 4 + 1] = v2;
				vertices[i * 4 + 2] = v3;
				vertices[i * 4 + 3] = v4;
				
				uv[i * 4] = new Vector2(0, 0);
				uv[i * 4 + 1] = new Vector2(1, 0);
				uv[i * 4 + 2] = new Vector2(1, 1);
				uv[i * 4 + 3] = new Vector2(0, 1);
				
				triangles[i * 6] = i * 4;
				triangles[i * 6 + 1] = i * 4 + 1;
				triangles[i * 6 + 2] = i * 4 + 2;
				triangles[i * 6 + 3] = i * 4 + 2;
				triangles[i * 6 + 4] = i * 4 + 3;
				triangles[i * 6 + 5] = i * 4;
			}
			
			mesh.Clear();
			mesh.vertices = vertices;
			mesh.uv = uv;

            if (4 * quadCount == lineColors.Count)
            {
                mesh.colors = lineColors.ToArray();
            }

			mesh.triangles = triangles;
			mesh.RecalculateNormals();
			mesh.name = "Quads";
		}
		
		ClearQuadBuff();
		return mesh;
	}
	
	#endregion
	
	#region Tile Area
	
	// create a tile-based mesh, used for rendering a special colored area
	// currently tile mesh is x*z area
	public static Mesh createTileArea(float startx, float startz, float tilex, float tilez, int xCount, int zCount)
	{
		Mesh mesh = meshBuff? meshBuff: new Mesh();
		
		int quadCount = xCount * zCount;
		Vector3[] vertices = new Vector3[4 * quadCount];
		Vector2[] uv = new Vector2[4 * quadCount];
		int[] triangles = new int[6 * quadCount];
			
		float sx = startx;
		float sz = startz;
		int vertex_index = 0;
		int triangle_index = 0;
		for (int i=0; i<xCount; i++)
		{
			for (int j=0; j<zCount; j++)	
			{
				vertices[vertex_index] = new Vector3(sx, 0, sz);
				vertices[vertex_index+1] = new Vector3(sx, 0, sz + tilez);
				vertices[vertex_index+2] = new Vector3(sx + tilex, 0, sz + tilez);
				vertices[vertex_index+3] = new Vector3(sx + tilex, 0, sz);
				
				uv[vertex_index] = new Vector2(0, 0);
				uv[vertex_index+1] = new Vector2(1, 0);
				uv[vertex_index+2] = new Vector2(1, 1);
				uv[vertex_index+3] = new Vector2(0, 1);
				
				triangles[triangle_index] = vertex_index;
				triangles[triangle_index + 1] = vertex_index + 1;
				triangles[triangle_index + 2] = vertex_index + 2;
				triangles[triangle_index + 3] = vertex_index + 2;
				triangles[triangle_index + 4] = vertex_index + 3;
				triangles[triangle_index + 5] = vertex_index;
				
				vertex_index += 4;
				triangle_index += 6;
				
				sz += tilez;
			}
			sx += tilex;
			sz = startz;
		}
	
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.triangles = triangles;

		mesh.RecalculateNormals();
		mesh.name = "TileMesh";
		
		return mesh;
	}
	
	// set vertices color for tile-based mesh, used for rendering a "able/unable" colored area
	// colors: all colors in array
	// colorIndex: a tile is one color
	// order: (x0,z0) (x0,z1)...(x0,zn)... (x1,z0) (x1,z1)...(x1,zn)...(xm,z0)...(xm,zn)
	public static void setVerticesColorForTileMesh(Color[] _colors, byte[] _colorIndex)
	{
		if (meshBuff == null)
			return;
		if (meshBuff.vertices.Length != _colorIndex.Length * 4)
			return;
		
		int vertexCount = meshBuff.vertices.Length;
		int tileCount = _colorIndex.Length;
		Color[] colors = new Color[vertexCount];
		int vertexIndex = 0;
		Color color;
		for (int i=0; i<tileCount; i++)
		{
			color = _colors[_colorIndex[i]];
			colors[vertexIndex++] = color;
			colors[vertexIndex++] = color;
			colors[vertexIndex++] = color;
			colors[vertexIndex++] = color;
		}
		meshBuff.colors = colors;
	}
	
	#endregion
	
	
}
