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
     * The player's inputs used to control the actions of the train's PathFollow script.
     */
    void Controls()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 newPos = new(mousePos.x, mousePos.y, 0.0f);
            AddNode(newPos);
            SetLastPoint();
        }
    }
}
