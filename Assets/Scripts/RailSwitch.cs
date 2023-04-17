using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

public class RailSwitch : MonoBehaviour
{
    public SpriteShapeController rootTrack;
    public EdgeCollider2D edgeCollider;
    public SpriteShapeController[] inputTracks;
    public SpriteShapeController[] outputsTracks;

    private PathFollow obj;
    private Vector3 connectedRailPoint;     //The point on the rootRail that the rail is connected to. 

    //The stored location data of the connection point on the rootTrack;
    public int index;                          // the current track node
    private float p = 0.0f;                     // the progress between 2 nodes

    public bool ObjectOnSwitch;
    public int selectedTrack;
    private Vector3 Direction;

    private void OnValidate()
    {
        AllignTrackSwitch();
    }

    // Start is called before the first frame update
    void Start()
    {
        AllignTrackSwitch();
        SetInputTrackLastNode(inputTracks);
        p = GetProgressOnSplineSegment(connectedRailPoint, index);
        ObjectOnSwitch = false;
    }

    // Update is called once per frame
    void Update()
    {
        SelectTrack(selectedTrack);
    }

    /**
     * Checks if the object entering the trigger has a PathFollow object and that the outputTrack's isn't null.
     * assigns the input track direction. 
     * 
     * TODO - Optimize code to remove null reference error, code is clearly checking outputTracks[-1] even when return should avoid that. 
     * 
     * @param track: the index of the selected track stored in the outputTracks array.
     */
    void SelectTrack(int track)
    {
        if (track == -1 && obj != null && obj.SSC == rootTrack)
        {
            return;
        }
        else if (track == -1 && obj.SSC != rootTrack && obj != null && rootTrack != null && ObjectOnSwitch)
        {
            obj.SetNewTrack(rootTrack, index, p);
            ObjectOnSwitch = false;
            return;
        }
        if(obj != null && outputsTracks != null && ObjectOnSwitch)
        {
            obj.SetNewTrack(outputsTracks[track], 0, 0.0f);
            ObjectOnSwitch = false;
            return;
        }
    }

    /**
     * Get's the progress of an object on a spline segment
     */
    public float GetProgressOnSplineSegment(Vector3 position, int segmentIndex, int subdivisions = 100)
    {
        if (segmentIndex < 0 || segmentIndex >= (rootTrack.spline.GetPointCount() - 1))
        {
            Debug.LogError("Invalid segment index: " + segmentIndex);
            return 0f;
        }

        Vector3 p0 = rootTrack.spline.GetPosition(segmentIndex);
        Vector3 p1 = rootTrack.spline.GetRightTangent(segmentIndex);
        Vector3 p2 = rootTrack.spline.GetLeftTangent(segmentIndex + 1);
        Vector3 p3 = rootTrack.spline.GetPosition(segmentIndex + 1);

        List<Vector3> points = Enumerable.Range(0, subdivisions + 1)
            .Select(n => BezierUtility.BezierPoint(p0, p1, p2, p3, (float)n / subdivisions))
            .ToList();

        float closestDistance = points.Min(p => Vector3.Distance(position, p));
        float progress = points.FindIndex(p => Vector3.Distance(position, p) == closestDistance) / (float)subdivisions;

        return progress;
    }


    /**
     * orient the direction of the switch's child RotaryPoint using the direction of the connected root_rail at that position.
     * 
     * To calculate the direction of the vector v = (x,y), use the formula theta = arctan(y/x)
     */
    void SwitchDirection()
    {

    }

    /**
     * Set's the last node of all the input rail's in the inputTracks[] array to the connectedRailPoint position.
     */
    void SetInputTrackLastNode(SpriteShapeController[] tracks)
    {
        if (tracks==null)
        {
            return;
        }
        for (int i=0; i<tracks.Length; i++)
        {
            Spline spline = tracks[i].spline;
            int size = spline.GetPointCount();
            spline.SetPosition(size-1, connectedRailPoint);
        }
    }

    void AllignTrackSwitch()
    {
        if (edgeCollider != null)
        {
            // Get the closest point on the edge collider to our object's position
            Vector2 closestPoint = edgeCollider.ClosestPoint(transform.position);

            // Move our object to the closest point on the edge collider
            transform.position = closestPoint;
            connectedRailPoint = closestPoint;
        }
    }

    /**
     * Checks 
     */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ObjectOnSwitch = true;
        obj = collision.gameObject.GetComponent<PathFollow>();
    }

    //accesses PathFollow using obj = collision.gameObject.GetComponent<PathFollow>()
    private void OnTriggerExit2D(Collider2D collision)
    {
        ObjectOnSwitch = false;
        obj = null;
    }
}
