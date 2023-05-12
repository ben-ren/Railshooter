using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PlayerController : TrainController
{
    private Vector3 mousePos;
    

    protected override void Train()
    {
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        base.Train();
        Controls();
    }

    /**
     * Adds a node with the continuous tangent mode to the end of the spline.
     */
    void AddNode(Vector3 newPoint)
    {
        int pointCount = SSC.spline.GetPointCount(); // Get the number of existing points on the spline
        SSC.spline.SetTangentMode(pointCount - 1, ShapeTangentMode.Continuous); // Set the tangent mode of the last point to continuous
        SSC.spline.InsertPointAt(pointCount, newPoint);      // Add the new point to the end of the spline
        SSC.BakeCollider(); // Bake the collider to update its shape
        SSC.BakeMesh(); // Bake the mesh to update its shape
    }

    /**
     * Set the new node's tangent's to properly allign rails
     */
    void TangentAllign(int index)
    {
        //SSC.spline.SetTangentMode(index - 1, ShapeTangentMode.Continuous);
        Vector3 A = SSC.spline.GetPosition(index);      //end point
        Vector3 B = SSC.spline.GetPosition(index - 1);  //start point
        Vector3 C = SSC.spline.GetPosition(index - 2);  //previous penultimate point
        
        Vector3 direction = (B-C).normalized;     //the direction the node at that location is trending towards

        //CHECK - distance may need to be divided by 2 for edge cases.
        float distance = Vector3.Distance(B, A);    //the distance between the original end point and the new end point

        //Sets the tangents of the old end node.
        SSC.spline.SetLeftTangent(index - 1, -direction);
        SSC.spline.SetRightTangent(index - 1, direction * distance);
        
    }

    /**
     * The player's inputs used to control the actions of the train's PathFollow script.
     * 
     * TODO - Replace mousePos with a public Transform that takes in a gameObject alligned with the RailGun.
     */
    void Controls()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 newPos = new(mousePos.x, mousePos.y, 0.0f);
            AddNode(newPos);
            SetLastPoint();
            TangentAllign(_spline.GetPointCount()-1);
        }
    }
}
