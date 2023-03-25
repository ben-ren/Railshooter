using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCreator : MonoBehaviour{

    [HideInInspector] public Path path;
    [HideInInspector] public Vector2[] pathPts = {};

    public Color anchorColor = Color.red;
    public Color controlColor = Color.white;
    public Color segmentColor = Color.black;
    public Color selectedSegmentColor = Color.yellow;

    public float anchorDiameter = .1f;
    public float controlDiameter = .05f;
    public bool displayControlPoints = true;
    public float pathSpacing = .1f;
    public float pathResolution = 1;

    public void Start()
    {
        pathPts = path.CalculateEvenlySpacePoints(pathSpacing, pathResolution);
    }

    public void CreatePath()
    {
        path = new Path(transform.position);
    }

    public void ShowPath()
    {
        foreach(Vector2 p in pathPts)
        {
            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            g.transform.position = p;
            g.transform.localScale = Vector3.one * pathSpacing * .5f;
        }
    }

    //Function that returns the index value for the pathPts Vector2 closest to a target Vector2.
    public int GetClosestPointIndexAtLocation(Vector2 position)         //suboptimal code. Better code uses k-d tree. 
    {
        float nearestDistance = float.MaxValue;
        Vector2 nearestPoint = position;
        //loop through path
        foreach (Vector2 v in pathPts)          //IMPORTANT FIX!!: Code currently searches through entire array, change to only search through points with a Range close to the position variable.
        {
            //set nearestPoint
            if (Vector2.Distance(position, v) < nearestDistance)
            {
                nearestDistance = Vector2.Distance(position, v);
                nearestPoint = v;
            }
        }
        return System.Array.IndexOf(pathPts, nearestPoint);
    }


    void Reset()
    {
        CreatePath();
    }
}
