using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class TrackController : MonoBehaviour
{
    public PathFollow path;
    public GameObject prefab;
    public Gun_Controller gun;

    /**
     * Create's new RailSwitch/TrackSwitch on the Rail_Root
     * RailSwitch Instantiated at GetPointOnSpline(i, Progress() + x, start, end)
     * Last node on SpriteShape attached to the AssignTarget variable.
     */

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CreateTrack(path, gun.AssignTarget());
        }
    }

    /* 
     * Instantiates a RailSwitch prefab at a position on the PathFollow
     * with it's track leading to a Vector3 target position 
     */
    void CreateTrack(PathFollow path, Vector3 target)
    {
        Vector3 start = GetPathVector(path, 0);
        Vector3 end = GetPathVector(path, 1);
        GameObject track = (GameObject)Instantiate(prefab, path.GetPointOnSpline(path.GetIndex(), path.GetProgress(), start, end), Quaternion.identity);
        track.GetComponentInChildren<SpriteShapeController>().spline.SetPosition(1, target);
        int splineLength = track.GetComponentInChildren<SpriteShapeController>().spline.GetPointCount();
        for (int i=0; i<splineLength; i++)
        {
            track.GetComponentInChildren<SpriteShapeController>().spline.SetTangentMode(i, ShapeTangentMode.Continuous);
        }
    }

    Vector3 GetPathVector(PathFollow path, int indexOffset)
    {
        return path.SSC.spline.GetPosition(path.GetIndex() + indexOffset);
    }
}
