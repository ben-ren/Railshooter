using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathWalker : MonoBehaviour
{
    public PathCreator pathCreator;

    public int speed;

    public int minSpeed;

    public int maxSpeed;

    public bool lookForward;

    public bool loop;

    protected int i = 0;

    // Update is called once per frame
    void Update() {
        MoveLogic();
    }

    //calculates which point to move towards. 
    protected void PathPointProgressInt(int spd) {
        i += spd;

        //if loop is active then upon reaching end of array, restart index.
        if (loop)
        {
            if (i > pathCreator.pathPts.Length - 1)
            {
                i = 1;
            }
            else if (i <= 0)
            {
                i = pathCreator.pathPts.Length - 1;
            }
        }
    }

    void TestFunction()
    {
        Debug.Log(pathCreator.GetClosestPointIndexAtLocation(this.transform.position));
    }

    protected void MoveLogic()
    {
        PathPointProgressInt(speed);
        transform.localPosition = Vector3.Lerp(pathCreator.pathPts[i - 1], pathCreator.pathPts[i], speed);
        if (lookForward)
        {   //if (i<length) then i else 0 => (i<length) ? i+1 : 0. Fixes Out Of Bounds Array bug
            RotationCalculation(pathCreator.pathPts[(i < pathCreator.pathPts.Length - 1) ? i + 1 : 0]);
        }
    }

    //Modulo math: a % b = c -> c = a - a/b * b
    
    //calculates the progress taken towards the next point. Code from here https://catlikecoding.com/unity/tutorials/curves-and-splines/
    /* Legacy Code
     * -----------
     * progress += 0.01f * (speed* Time.deltaTime);
     * i += 1 + Mathf.FloorToInt(0.01f * rate * Time.deltaTime);   //calculates index target relative to speed. if 0.01f*rate*Time.deltaTime > 1 {scale by 1.}
     * i -= 1 + Mathf.FloorToInt(0.01f * -rate * Time.deltaTime);  //inverts index calculation for reverse movement.
     */

    //Uses the A-tangent of the curve to calculate the rotation of the Z-axis. The LookTowards function rotates along the Y-axis unfortunately. 
    protected void RotationCalculation(Vector3 coords)
    {
        float z_rot = Mathf.Rad2Deg * Mathf.Atan2((coords.y - transform.position.y), (coords.x - transform.position.x))-90;     //360 degree rotation
        transform.rotation = Quaternion.Euler(0, 0, z_rot);
    }
}
