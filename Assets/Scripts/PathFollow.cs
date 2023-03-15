using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PathFollow : MonoBehaviour
{
    public SpriteShapeController SSC;   //Controller component
    public float speed;                 //The speed the object travels from start to target

    private Spline _spline;              //Spline component
    private Vector3 targetPos;          //Object target position
    private Vector3 startPos;          //Object target position
    private float progress;             //progress between points on spline
    private int i;

    void Start()
    {
        _spline = SSC.spline;                            //Assigns the SpriteShape's spline to Spline variable for checking. 
        transform.position = _spline.GetPosition(0);     //Assign object's starting position
        startPos = _spline.GetPosition(0);              //Assign initial start position
        targetPos = _spline.GetPosition(1);              //Assign initial target position
        i = 0;                                          //first index position
    }

    // Update is called once per frame
    void Update()
    {
        float prog = Progress();
        Vector3 newPos = GetPointOnSpline(i, prog, startPos, targetPos);
        Vector3 newLook = GetPointOnSpline(i, prog+0.01f, startPos, targetPos);

        MoveAlongSpline(newPos);
        RotationCalc(newLook);
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
     * Object's transform.position follows the SpriteShape's spline nodes. Upon reaching a particular node the 
     * object will then have it's index iterate by 1 to target the next node in the spline. 
     */
    void MoveAlongSpline(Vector3 newPos)
    {
        //transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        transform.position = newPos;

        if (ReachedPoint(targetPos) && ReachedSplineEnd(i, 2)) 
        {
            i++;
            startPos = _spline.GetPosition(i);
            targetPos = _spline.GetPosition(i + 1);
        }
    }

    public Vector2 GetPointOnSpline(int i, float progress, Vector3 start, Vector3 target)
    {
        Vector2 _p0 = new(start.x, start.y);
        Vector2 _p1 = new(target.x, target.y);
        Vector2 _rt = _p0 + new Vector2(_spline.GetRightTangent(i).x, _spline.GetRightTangent(i).y);
        Vector2 _lt = _p1 + new Vector2(_spline.GetLeftTangent(i+1).x, _spline.GetLeftTangent(i+1).y);

        return BezierUtility.BezierPoint(
            new Vector2(_p0.x, _p0.y),
            new Vector2(_rt.x, _rt.y),
            new Vector2(_lt.x, _lt.y),
            new Vector2(_p1.x, _p1.y),
            progress
        );
    }

    float Progress()
    {
        if (progress < 1.0)
        {
            progress += speed*.1f*Time.deltaTime;
        }
        else if(!FinalPointReached())
        {
            progress = 0.0f;
        }
        return Mathf.Clamp01(progress);
    }

    bool ReachedSplineEnd(int i, int offset)
    {
        return i < (_spline.GetPointCount() - offset);
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
    bool FinalPointReached()
    {
        return transform.position == _spline.GetPosition(_spline.GetPointCount() - 1);
    }
}
