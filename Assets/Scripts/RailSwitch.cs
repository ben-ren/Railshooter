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
    private int index;                          // the current track node
    public float p = 0.0f;                     // the progress between 2 nodes

    public int selectedTrack;
    public bool ObjectOnSwitch;

    private void OnValidate()
    {
        AllignTrackSwitch();
    }

    // Start is called before the first frame update
    void Start()
    {
        AllignTrackSwitch();
        SetInputTrackLastNode(inputTracks);
        index = GetIndexOnSpline();
        p = GetProgressOnSplineSegment(connectedRailPoint, index);
        ObjectOnSwitch = false;
    }

    // Update is called once per frame
    void Update()
    {
        index = GetIndexOnSpline();
        SelectTrack(selectedTrack);
    }

    /**
     * Checks if the object entering the trigger has a PathFollow object and that the outputTrack's isn't null.
     * assigns the input track direction. 
     * 
     * @param track: the index of the selected track stored in the outputTracks array.
     */
    void SelectTrack(int track)
    {
        if (track == -1 && obj != null && obj.SSC == rootTrack)
        {
            return;
        }
        //else if (track == -1 && obj.SSC != rootTrack && obj != null && rootTrack != null && ObjectOnSwitch)
        else if(track == -1 && ObjectOnSwitch)
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
     * Calculates the index based on the railSwitch's position on the spline. 
     */
    public int GetClosestIndexOnSpline()
    {
        float minDistance = Mathf.Infinity;
        int closestIndex = -1;

        for (int i = 0; i < rootTrack.spline.GetPointCount(); i++)
        {
            Vector3 point = rootTrack.spline.GetPosition(i);
            float distance = Vector3.Distance(transform.position, point);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestIndex = i;
            }
        }

        return closestIndex;
    }

    /**
     * Calculates the index based on the railSwitch's position on the spline. 
     */
    public int GetIndexOnSpline()
    {
        int closestIndex = -1;

        for (int i = 0; i < rootTrack.spline.GetPointCount()-1; i++)
        {
            Vector3 point = rootTrack.spline.GetPosition(i);
            Vector3 nextPoint = rootTrack.spline.GetPosition(i+1);
            if (IsPointInArea(transform.position, point, nextPoint))
            {
                closestIndex = i;
            }
        }
        return closestIndex;
    }

    /**
     * Checks to see if the point lies between the 2 points instead of beyond them.
     */
    bool IsPointInArea(Vector3 point, Vector3 pointA, Vector3 pointB)
    {
        Vector3 vectorAB = pointB - pointA;
        Vector3 vectorATest = point - pointA;

        // If the dot product of vectorAB and vectorATest is less than zero, the test point is outside the area
        if (Vector3.Dot(vectorAB, vectorATest) < 0)
        {
            return false;
        }

        // If the test point is not outside the area, check the other side of the line defined by the two end points
        Vector3 vectorBTest = point - pointB;

        // If the dot product of vectorBA and vectorBTest is less than zero, the test point is outside the area
        if (Vector3.Dot(-vectorAB, vectorBTest) < 0)
        {
            return false;
        }

        // If the test point is not outside the area, it must be inside
        return true;
    }

    /**
     * Get's the progress of an object on a spline segment
     */
    public float GetProgressOnSplineSegment(Vector3 position, int index)
    {
        Vector3 segmentStart = rootTrack.spline.GetPosition(index);
        Vector3 segmentControl1 = rootTrack.spline.GetRightTangent(index);
        Vector3 segmentControl2 = rootTrack.spline.GetLeftTangent(index + 1);
        Vector3 segmentEnd = rootTrack.spline.GetPosition(index + 1);
        float segmentLength = GetBezierCurveSegmentLength(segmentStart, segmentControl1, segmentControl2, segmentEnd);
        float positionOnSegment = Vector3.Distance(segmentStart, position);
        float progressOnSegment = positionOnSegment / segmentLength;
        return progressOnSegment * 2;
    }

    /**
     * Calculates the Length of a selected BezierCurveSegment, using the 4 input points.
     */
    public float GetBezierCurveSegmentLength(Vector3 startPoint, Vector3 control1, Vector3 control2, Vector3 endPoint, int numIterations = 100)
    {
        float segmentLength = 0f;
        Vector3 previousPoint = startPoint;

        for (int i = 1; i <= numIterations; i++)
        {
            float t = i / (float)numIterations;
            Vector3 pointOnCurve = Mathf.Pow(1 - t, 3) * startPoint
                                + 3 * Mathf.Pow(1 - t, 2) * t * control1
                                + 3 * (1 - t) * Mathf.Pow(t, 2) * control2
                                + Mathf.Pow(t, 3) * endPoint;

            segmentLength += Vector3.Distance(previousPoint, pointOnCurve);
            previousPoint = pointOnCurve;
        }

        return segmentLength;
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
