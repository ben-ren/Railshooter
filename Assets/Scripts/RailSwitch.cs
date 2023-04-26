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
    private float p = 0.0f;                     // the progress between 2 nodes

    public int selectedTrack;
    public bool ObjectOnSwitch;
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
        index = GetIndexOnSpline();
        p = GetProgressOnSplineSegment(connectedRailPoint, index);
        ObjectOnSwitch = false;
        Debug.Log(GetIndexOnSpline() + " | " + gameObject.name);
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
    public int GetIndexOnSpline()
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
                //BUG - This is recursive since it relies on p which relies on index itself.
                if (p > 0.51f)
                {
                    closestIndex--;
                }
            }
        }

        return closestIndex;
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

    bool CheckInRange(Vector3 p, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector3 AB = p1 - p0;
        Vector3 BC = p2 - p1;
        Vector3 CD = p3 - p2;
        Vector3 DA = p0 - p3;

        // Check whether the point is inside the quadrilateral by checking the sign of the cross products
        bool inside =
            Vector3.Dot(Vector3.Cross(AB, p - p0), Vector3.Cross(AB, p1 - p0)) >= 0f &&
            Vector3.Dot(Vector3.Cross(BC, p - p1), Vector3.Cross(BC, p2 - p1)) >= 0f &&
            Vector3.Dot(Vector3.Cross(CD, p - p2), Vector3.Cross(CD, p3 - p2)) >= 0f &&
            Vector3.Dot(Vector3.Cross(DA, p - p3), Vector3.Cross(DA, p0 - p3)) >= 0f;

        return inside;
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
