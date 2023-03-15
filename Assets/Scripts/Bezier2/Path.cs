using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Path{

    [SerializeField, HideInInspector]
    List<Vector2> points;
    [SerializeField, HideInInspector]
    bool isClosed;
    [SerializeField, HideInInspector]
    bool autoSetControlPoints;

    public Path(Vector2 centre)
    {
        points = new List<Vector2>      //generates the first segment (first 4 points, centre+Vector2.left is the root point)
        {
            centre+Vector2.left,
            centre+(Vector2.left + Vector2.up)*.5f,
            centre+(Vector2.right + Vector2.down)*.5f,
            centre+Vector2.right
        };
    }

    public Vector2 this[int i]
    {
        get
        {
            return points[i];
        }
    }

    public bool IsClosed
    {
        get
        {
            return isClosed;
        }
        set
        {
            if (isClosed != value)
            {
                isClosed = value;
                if (isClosed)
                {
                    points.Add(points[points.Count - 1] * 2 - points[points.Count - 2]);    //Add one opposite to the last control point
                    points.Add(points[0] * 2 - points[1]);                   //points[0]=the first anchorpoint, points[1] is the first controls point
                    if (autoSetControlPoints)
                    {
                        AutoSetAnchorControlPoints(0);
                        AutoSetAnchorControlPoints(points.Count - 3);
                    }
                }
                else //if not closed remove the 2 control points that were created above
                {
                    points.RemoveRange(points.Count - 2, 2);
                    if (autoSetControlPoints)
                    {
                        AutoSetStartAndEndControlPoints();
                    }
                }
            }
        }
    }

    public bool AutoSetControlPoints
    {
        get
        {
            return autoSetControlPoints;
        }
        set
        {
            if(autoSetControlPoints != value)
            {
                autoSetControlPoints = value;
                if (autoSetControlPoints)
                {
                    AutoSetAllControlPoints();
                }
            }
        }
    }

    public int NumPoints
    {
        get
        {
            return points.Count;
        }
    }

    //Gets the number of segments, 1st segment has 4 points, all subsequent segments add an extra 3 nodes, 1 is added for the initial segment
    public int NumSegments
    {
        get
        {
            return points.Count/3;      //first segment is 4 nodes, subsequent segments add 3 additional nodes (4/3, 7/3, 10/3 = 1, 2, 3. 6/3, 9/3, 12/3 = 2, 3, 4)
        }
    }

    public void AddSegment(Vector2 anchorPos)
    {
        points.Add(points[points.Count-1]*2 - points[points.Count-2]);      //P3+(P3-P2) = P3*2 - P2
        points.Add((points[points.Count-1] + anchorPos)*.5f);        //(P4+P6)/2
        points.Add(anchorPos);                                      //targeted position

        if (autoSetControlPoints)
        {
            AutoSetAllAffectedControlPoints(points.Count - 1);
        }
    }

    public void SplitSegment(Vector2 anchorPos, int segmentIndex)
    {
        points.InsertRange(segmentIndex * 3 + 2, new Vector2[] { Vector2.zero, anchorPos, Vector2.zero });
        if (autoSetControlPoints)
        {
            AutoSetAllAffectedControlPoints(segmentIndex * 3 + 3);
        }
        else
        {
            AutoSetAnchorControlPoints(segmentIndex * 3 + 3);
        }
    }

    /*
     * i = selected node, can only delete anchor points, NOT control points
     * 
     * if (i!=0 || i!= points.Count), default: delete_nodes(i-1, i, i+1)
     * if (i==0){
     *      if(open): delete_nodes(0, 1, 2)
     *      if(closed): set{p[-1] = p[2]}, delete_nodes(0, 1, 2)
     * }
     * if(i==-1 && open): delete_nodes(i-2, i-1, i)
     */
    public void DeleteSegment(int anchorIndex)
    {
        if (NumSegments < 2 || isClosed && NumSegments < 1)         //if(Numsegments > 2 || !isClosed && NumSegments > 1)
        {
            return;
        }
        if(anchorIndex == 0)
        {
            if (isClosed)
            {
                points[points.Count - 1] = points[2];
            }
            points.RemoveRange(0, 3);
        }
        else if (anchorIndex == points.Count-1 && !isClosed)
        {
            points.RemoveRange(anchorIndex-2, 3);
        }
        else
        {
            points.RemoveRange(anchorIndex-1, 3);
        }
    }

    public Vector2[] GetPointsInSegment(int i)
    {
        return new Vector2[]{points[i*3], points[i*3+1], points[i*3+2], points[LoopIndex(i*3+3)]};
    }

    public void MovePoint(int i, Vector2 newPos)
    {
        Vector2 deltaMove = newPos - points[i];
        if (i % 3 == 0 || !autoSetControlPoints)    //can only move control points if auto set control points is off or (i%3==0) the point is an anchorpoint.
        {
            points[i] = newPos;

            if (autoSetControlPoints)
            {
                AutoSetAllAffectedControlPoints(i);
            }
            else
            {

                //anchorpoints are always at a multiple of 3 in the list
                if (i % 3 == 0)
                {
                    if (i + 1 < points.Count || isClosed)
                    {
                        points[LoopIndex(i + 1)] += deltaMove;
                    }
                    if (i - 1 >= 0 || isClosed)
                    {
                        points[LoopIndex(i - 1)] += deltaMove;
                    }
                }
                else    //What is this is NOT an achorpoint
                {
                    bool nextPointAnchor = (i + 1) % 3 == 0;

                    //Ternary operator; if(next point is an anchor) "then it is"(?) equal to i+2 otherwise "if not" (:) it is equal to i-2;
                    int correspondingControlIndex = (nextPointAnchor) ? i + 2 : i - 2;  //find the index of the other point attached to the anchor
                    int anchorIndex = (nextPointAnchor) ? i + 1 : i - 1;                //find the index of the anchor point

                    if (correspondingControlIndex >= 0 && correspondingControlIndex < points.Count || isClosed)
                    {
                        //move the correspondingControlIndex point while maintiang distance from anchorIndex point;
                        float distance = (points[LoopIndex(anchorIndex)] - points[LoopIndex(correspondingControlIndex)]).magnitude;
                        Vector2 direction = (points[LoopIndex(anchorIndex)] - newPos).normalized;
                        points[LoopIndex(correspondingControlIndex)] = points[LoopIndex(anchorIndex)] + direction * distance;
                    }
                }
            }
        }
    }

    public Vector2[] CalculateEvenlySpacePoints(float spacing, float resolution = 1)
    {
        List<Vector2> evenlySpacedPoints = new List<Vector2>();
        evenlySpacedPoints.Add(points[0]);
        Vector2 previousPoint = points[0];
        float distanceSinceLastPoint = 0;

        for (int SegIndex=0; SegIndex < NumSegments; SegIndex++)
        {
            Vector2[] p = GetPointsInSegment(SegIndex);
            float controlNetLength = Vector2.Distance(p[0], p[1]) + Vector2.Distance(p[1], p[2]) + Vector2.Distance(p[2], p[3]);
            float estimatedCurveLength = Vector2.Distance(p[0], p[3]) + controlNetLength / 2f;
            int divisions = Mathf.CeilToInt(estimatedCurveLength * resolution * 10);
            float t = 0;
            while(t <= 1)
            {
                t += 1f/divisions;
                Vector2 newPointOnCurve = Bezier2.EvaluateCubic(p[0], p[1], p[2], p[3], t);
                distanceSinceLastPoint += Vector2.Distance(previousPoint, newPointOnCurve);
                while (distanceSinceLastPoint >= spacing)
                {
                    float overshootDistance = distanceSinceLastPoint - spacing;
                    Vector2 newEvenlySpacedPoint = newPointOnCurve + (previousPoint - newPointOnCurve).normalized * overshootDistance;
                    evenlySpacedPoints.Add(newEvenlySpacedPoint);
                    distanceSinceLastPoint = overshootDistance;
                    previousPoint = newEvenlySpacedPoint;
                }
                previousPoint = newPointOnCurve;
            }
        }

        return evenlySpacedPoints.ToArray();
    }

    void AutoSetAllAffectedControlPoints(int updatedAnchorIndex)
    {
        for (int i =updatedAnchorIndex-3; i<=updatedAnchorIndex+3; i+=3)
        {
            if(i >=0 && i < points.Count || isClosed)
            {
                AutoSetAnchorControlPoints(LoopIndex(i));
            }
        }

        AutoSetStartAndEndControlPoints();
    }

    void AutoSetAllControlPoints()
    {
        for(int i = 0; i<points.Count; i += 3)
        {
            AutoSetAnchorControlPoints(i);
        }
        AutoSetStartAndEndControlPoints();
    }

    //Set the control points automatically for procedural generation
    //Get the position of the anchorpoint, and it's 2 attached points. Find the 2 distances from the anchorpoint and it's 2 connected nodes.
    //Normalize the Vectors, subtract the vectors from eachother
    void AutoSetAnchorControlPoints(int anchorIndex)
    {
        Vector2 anchorPos = points[anchorIndex];
        Vector2 dir = Vector2.zero;
        float[] neighbourDistances = new float[2];

        if (anchorIndex - 3 >= 0 || isClosed)
        {
            Vector2 offset = points[LoopIndex(anchorIndex - 3)] - anchorPos;
            dir += offset.normalized;
            neighbourDistances[0] = offset.magnitude;
        }
        if (anchorIndex + 3 >= 0 || isClosed)
        {
            Vector2 offset = points[LoopIndex(anchorIndex + 3)] - anchorPos;
            dir -= offset.normalized;
            neighbourDistances[1] = -offset.magnitude;
        }

        dir.Normalize();

        for (int i=0; i<2; i++)
        {
            int controlIndex = anchorIndex + i * 2 - 1;
            if(controlIndex >= 0 && controlIndex < points.Count || isClosed)
            {
                points[LoopIndex(controlIndex)] = anchorPos + dir * neighbourDistances[i] * .5f;
            }
        }
    }

    void AutoSetStartAndEndControlPoints()
    {
        if (!isClosed)
        {
            points[1] = (points[0] + points[2] * .5f);
            points[points.Count - 2] = (points[points.Count - 1] + points[points.Count - 3]) * .5f;
        }
    }

    //Loops the index variables so that points move relative to their position when the loop is closed. 
    int LoopIndex(int i)
    {
        return (i + points.Count) % points.Count;
    }
}
