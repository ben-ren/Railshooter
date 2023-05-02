using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PathFollow : MonoBehaviour
{
    public SpriteShapeController SSC;   //Controller component
    public float speed;                 //The speed the object travels from start to target

    private Spline _spline;             //Spline component
    private Vector3 targetPos;          //Object target position
    private Vector3 startPos;           //Object start position
    private Vector3 splineStart;        //spline start position
    private Vector3 lastPos;            //spline last position
    private float progress;             //progress between points on spline
    private float distance;             //distance between points on the curve
    private Vector3 parentOffset;       //the offset between the parent object & the SpriteShapeController
    private int i;
    public bool stop;                  //Controls when the gameObject moves;

    void Start()
    {
        ParentCheck();
        _spline = SSC.spline;                            //Assigns the SpriteShape's spline to Spline variable for checking. 
        transform.position = parentOffset + _spline.GetPosition(0);     //Assign object's starting position
        splineStart = parentOffset + _spline.GetPosition(0);              //Assign initial start position
        startPos = parentOffset + _spline.GetPosition(0);              //Assign initial start position
        targetPos = parentOffset + _spline.GetPosition(1);              //Assign initial target position
        lastPos = parentOffset + _spline.GetPosition(_spline.GetPointCount()-1);              //Assign final target position
        i = 0;                                          //first index position
    }

    // Update is called once per frame
    void Update()
    {
        ParentCheck();
        Logic();
    }

    /**
     * Logic code used to dictate how child classes use the PathFollow script
     */
    protected virtual void Logic() 
    {
        int length = _spline.GetPointCount();
        int start = i % length;
        int end = (i + 1) % length;
        if (FinalPointReached() && _spline.isOpenEnded)
        {
            stop = true;
        }

        if (!stop) {
            //calculates distance between points along curve.
            distance = CurveDistance(start, end, 0.0f, .99f);
            float prog = Progress();

            Vector3 newPos = GetPointOnSpline(start, end, prog, startPos, targetPos);
            Vector3 newLook = GetPointOnSpline(start, end, prog + 0.01f, startPos, targetPos);

            MoveAlongSpline(newPos, length);
            RotationCalc(newLook);
        }
    }

    /**
     * Object's transform.position follows the SpriteShape's spline nodes. Upon reaching a particular node the 
     * object will then have it's index iterate by 1 to target the next node in the spline. 
     */
    void MoveAlongSpline(Vector3 newPos, int len)
    {
        transform.position = newPos;
        //Sets the start postion, targetposition and iterates to next spline node if the object has reached the target node
        if (ReachedPoint(targetPos))
        {
            i++;
            startPos = parentOffset + _spline.GetPosition(i % len);
            targetPos = parentOffset + _spline.GetPosition((i+1) % len);
        }
    }

    /**
     * Get the current index
     */
    public int GetIndex()
    {
        return this.i;
    }

    /**
     * Get the current progress
     */
    public float GetProgress()
    {
        return this.progress;
    }

    /**
     * Checks for the presence of a parent object. 
     * If the SpriteShape is attached to a parent object it add's that transform offset to all instance's of startPos & targetPos
     */
    void ParentCheck()
    {
        if (SSC.transform.parent !=null)
        {
            parentOffset = SSC.transform.parent.transform.position;
        }
    }

    /**
     * Calculates the approximate length of the curve. 
     */
    public float CurveDistance(int startIndex, int endIndex, float startOffset, float steps)
    {
        float dist = 0f;
        for (float p = startOffset; p<steps; p+=0.01f)
        {
            Vector2 current = GetPointOnSpline(startIndex, endIndex, p, startPos, targetPos);
            Vector2 next = GetPointOnSpline(startIndex, endIndex, p+0.01f, startPos, targetPos);
            dist += Vector2.Distance(current, next);
        }
        return dist;
    }

    /**
     * Calculates the rotation of the object's transform using an arctangent, 
     * comparing between it's current transform.position and target position.
     */
    void RotationCalc(Vector3 target)
    {
        float z_rot = Mathf.Rad2Deg * Mathf.Atan2((target.y - transform.position.y), (target.x - transform.position.x)) - 90;     //360 deree rotation
        transform.rotation = Quaternion.Euler(0, 0, z_rot);
    }

    /**
     * Changes the assigned SpriteShapeController by getting the collision with the RailSwitch and resets the progress & index fields
     * 
     * BUG: Fix jitter when Train switches tracks.
     */
    public void SetNewTrack(SpriteShapeController track, int index, float p)
    {
        this.SSC = track;
        this._spline = track.spline;
        i = index;
        progress = p;
        startPos = track.spline.GetPosition(i);             // Assign new initial start position as connectionPoint
        targetPos = track.spline.GetPosition(i+1);            // Assign initial target position using offset of connectionPoint
    }

    /**
     * Get a point on the spline from start of node (i) along curve at position (progress)
     */
    public Vector2 GetPointOnSpline(int firstIndex, int secondIndex, float progress, Vector3 start, Vector3 target)
    {
        Vector2 _p0 = new(start.x, start.y);
        Vector2 _p1 = new(target.x, target.y);
        Vector2 _rt = _p0 + new Vector2(_spline.GetRightTangent(firstIndex).x, _spline.GetRightTangent(firstIndex).y);
        Vector2 _lt = _p1 + new Vector2(_spline.GetLeftTangent(secondIndex).x, _spline.GetLeftTangent(secondIndex).y);

        return BezierUtility.BezierPoint(
            new Vector2(_p0.x, _p0.y),
            new Vector2(_rt.x, _rt.y),
            new Vector2(_lt.x, _lt.y),
            new Vector2(_p1.x, _p1.y),
            progress
        );
    }

    /**
     * Calculates the index based on the railSwitch's position on the spline. 
     */
    public int GetClosestIndexOnSpline(Vector3 target)
    {
        float minDistance = Mathf.Infinity;
        int closestIndex = -1;

        for (int i = 0; i < _spline.GetPointCount(); i++)
        {
            Vector3 point = _spline.GetPosition(i);
            float distance = Vector3.Distance(target, point);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestIndex = i;
            }
        }

        return closestIndex;
    }


    /**
     * Calculates the current progress along the spline to the targetPos
     */
    float Progress()
    {
        if (progress < 1.0)
        {
            progress += Time.deltaTime * (speed/distance);
        }
        else
        {
            progress = 0.0f;
        }
        
        return Mathf.Clamp01(progress);
    }

    /**
     * Checks to see if player position has reached target point
     */
    bool ReachedPoint(Vector3 target)
    {
        return transform.position == target;
    }

    /**
     * Checks to see if player position has reached final point on spline
     */
    public bool FinalPointReached()
    {
        return transform.position == lastPos;
    }
}
