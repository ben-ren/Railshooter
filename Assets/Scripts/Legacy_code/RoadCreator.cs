using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PathCreator))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class RoadCreator : MonoBehaviour {

    [Range(.05f, 1.5f)] public float spacing = 1;
    public float roadWidth = 1;
    public bool autoUpdate;
    public float tiling = 1;

    public void UpdateRoad()
    {
        Path path = GetComponent<PathCreator>().path;
        Vector2[] points = path.CalculateEvenlySpacePoints(spacing);
        GetComponent<MeshFilter>().mesh = CreateRoadMesh(points, path.IsClosed);

        int textureRepeat = Mathf.RoundToInt(tiling * points.Length * spacing * .05f);
        GetComponent<MeshRenderer>().sharedMaterial.mainTextureScale = new Vector2(1, textureRepeat);
    }

    Mesh CreateRoadMesh(Vector2[] points, bool isClosed)
    {
        //number of vertices = 2n
        Vector3[] verts = new Vector3[points.Length * 2];
        //UV coordinates
        Vector2[] uvs = new Vector2[verts.Length];
        //number of triangles = 2(n - 1)
        int numTris = 2 * (points.Length - 1) + ((isClosed) ? 2 : 0);   //Add additional tri if bezier isClosed.
        int[] tris = new int[numTris * 3];
        int vertIndex = 0;
        int triIndex = 0;

        for(int i=0; i< points.Length; i++)
        {
            //if it's not the last point or the first point we want the forward direction to be an average between the 2 points. 
            Vector2 forwardDirection = Vector2.zero;
            if (i < points.Length -1 || isClosed)
            {
                forwardDirection += points[(i + 1)%points.Length] - points[i];
            }
            if (i > 0 || isClosed)
            {
                //add points.Length to (i-1) to ensure that the index is always positive
                forwardDirection += points[i] - points[(i-1 + points.Length)%points.Length];        //negative numbers won't get wrapped around in C#
            }
            forwardDirection.Normalize();
            Vector2 left = new Vector2(-forwardDirection.y, forwardDirection.x);

            //set mesh vertices
            verts[vertIndex] = points[i] + left * roadWidth * .5f;
            verts[vertIndex + 1] = points[i] - left * roadWidth * .5f;

            //set uv coordinates
            float completionPercent = i / (float)(points.Length - 1);
            float v = 1 - Mathf.Abs(2 * completionPercent - 1);     //fixes artifact tearing when a closed loop uvs switch from the path end to path start by moving it to the center of the path
            uvs[vertIndex] = new Vector2(0, v);
            uvs[vertIndex+1] = new Vector2(1, v);

            if (i < points.Length - 1 || isClosed)
            {
                tris[triIndex] = vertIndex;
                tris[triIndex + 1] = (vertIndex + 2) % verts.Length;
                tris[triIndex + 2] = vertIndex + 1;

                tris[triIndex + 3] = vertIndex + 1;
                tris[triIndex + 4] = (vertIndex + 2) % verts.Length;
                tris[triIndex + 5] = (vertIndex + 3) % verts.Length;
            }

            vertIndex += 2;

            triIndex += 6;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.uv = uvs;

        return mesh;
    }
}
