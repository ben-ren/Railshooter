using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Gun_Controller : MonoBehaviour{

    public int railCount;
    public float range;

    private PathFollow train;
    private Vector3 target;

    private Camera mainCam;
    private Vector3 mousePos;
    
    void Start(){
        train = GetComponentInParent<PathFollow>();
        target = Vector2.up * range;
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update(){
        target = transform.parent.position + (GetMouseDirection() * range);
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        RotationCalc(mousePos);
        if (railCount > 0) {
            ExtendRail();
        }
    }

    void RotationCalc(Vector3 pos){
        float z_rot = Mathf.Rad2Deg*Mathf.Atan2((pos.y - transform.position.y), (pos.x - transform.position.x)) - 90;     //360 deree rotation
        //float z_rot = Mathf.Rad2Deg*Mathf.Atan(-(pos.x - transform.position.x) / (pos.y - transform.position.y));       //180 degree rotation
        transform.rotation = Quaternion.Euler(0, 0, z_rot);
    }

    
    /**
     * Extends the current rail segment when reaching the end of the rail
     */
    void ExtendRail()
    {
        if ((train.GetIndex() == train.SSC.spline.GetPointCount() - 2) && (train.GetProgress() > 0.8f) && (train.GetProgress() < 1f))
        {
            Vector3 newPos = new(target.x, target.y, 0.0f);
            AddNode(newPos);
            train.SetLastPoint();
            TangentAllign(train.SSC.spline.GetPointCount() - 1);

        }
    }

    /**
     * Get's the direction the mouse/gun is facing
     */
    Vector3 GetMouseDirection()
    {
        return (mousePos - transform.parent.position).normalized * 2;
    }

    /**
     * Adds a node with the continuous tangent mode to the end of the spline.
     */
    void AddNode(Vector3 newPoint)
    {
        int pointCount = train.SSC.spline.GetPointCount(); // Get the number of existing points on the spline
        train.SSC.spline.SetTangentMode(pointCount - 1, ShapeTangentMode.Continuous); // Set the tangent mode of the last point to continuous
        train.SSC.spline.InsertPointAt(pointCount, newPoint);      // Add the new point to the end of the spline
        train.SSC.BakeCollider(); // Bake the collider to update its shape
        train.SSC.BakeMesh(); // Bake the mesh to update its shape
    }

    /**
     * Set the new node's tangent's to properly allign rails
     */
    void TangentAllign(int index)
    {
        //SSC.spline.SetTangentMode(index - 1, ShapeTangentMode.Continuous);
        Vector3 A = train.SSC.spline.GetPosition(index);      //end point
        Vector3 B = train.SSC.spline.GetPosition(index - 1);  //start point
        Vector3 C = train.SSC.spline.GetPosition(index - 2);  //previous penultimate point

        Vector3 direction = (B - C).normalized;     //the direction the node at that location is trending towards

        //CHECK - distance may need to be divided by 2 for edge cases.
        float distance = Vector3.Distance(B, A);    //the distance between the original end point and the new end point

        //Sets the tangents of the old end node.
        train.SSC.spline.SetLeftTangent(index - 1, -direction);
        train.SSC.spline.SetRightTangent(index - 1, direction * distance);

        //Set the left tangent of the new added node.
        train.SSC.spline.SetTangentMode(index, ShapeTangentMode.Continuous); // Set the tangent mode of the last point to continuous
        train.SSC.spline.SetLeftTangent(index, -GunWorldDirection() * (distance/2));         //set tangent to the opposite direction of the current gun rotation
    }

    /**
     * Calculates the direction the the gun is facing as a vector instead of as a rotation
     */
    Vector3 GunWorldDirection()
    {
        // Get the z-rotation value
        float zRotation = transform.rotation.eulerAngles.z;

        // Convert z-rotation to direction vector
        Vector3 direction = Quaternion.Euler(0f, 0f, zRotation) * Vector3.up;

        return direction;
    }

    /*
     * Fires a Raycast to the childTarget's location
     * Checks if ray collides with an edgecollider's that are assigned with the "track" tag.
     * 
     * If true set RailSwitch with AssignedToStart set to true to the collision location. 
     * Update spline to location target.
     */
    void CheckRailCollision()
    {

    }
}
