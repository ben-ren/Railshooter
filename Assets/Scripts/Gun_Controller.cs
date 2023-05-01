using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Gun_Controller : MonoBehaviour{

    public GameObject projectile;
    public GameObject prefab;
    public PathFollow path;
    public int railCount;

    private Spline spl;
    private Camera mainCam;
    private Vector3 mousePos;
    
    void Start(){
        spl = this.gameObject.GetComponentInParent<PathFollow>().SSC.spline;
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update(){
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        RotationCalc(mousePos);
        if (Input.GetMouseButtonDown(0)) {
            Shoot(mousePos);
            CreateTrack(path, AssignTarget());
        }
    }

    void RotationCalc(Vector3 pos){
        float z_rot = Mathf.Rad2Deg*Mathf.Atan2((pos.y - transform.position.y), (pos.x - transform.position.x)) - 90;     //360 deree rotation
        //float z_rot = Mathf.Rad2Deg*Mathf.Atan(-(pos.x - transform.position.x) / (pos.y - transform.position.y));       //180 degree rotation
        transform.rotation = Quaternion.Euler(0, 0, z_rot);
    }

    /**
     * Assign the mouse position as the target
     */
    public Vector3 AssignTarget()
    {
        return mousePos;
    }

    /**
     * Assign an input GameObject as the target
     */
    public Vector3 AssignTarget(GameObject obj)
    {
        return obj.transform.position;
    }

    /**
     * Fires a ray in a straight line to the target position
     */
    public void Shoot(Vector3 target)
    {

    }

    /**
     * Fires a ray in a straight line towards target position cutting off at length of range.
     */
    public void Shoot(Vector3 target, float range)
    {

    }

    /* 
     * Instantiates a RailSwitch prefab at a position on the PathFollow
     * with it's track leading to a Vector3 target position 
     */
    void CreateTrack(PathFollow path, Vector3 target)
    {
        Vector3 start = GetPathVector(path, 0);
        Vector3 end = GetPathVector(path, 1);
        Vector3 pos = path.GetPointOnSpline(path.GetIndex(), path.GetIndex() + 1, path.GetProgress(), start, end);
        GameObject track = (GameObject)Instantiate(prefab, pos, Quaternion.identity);
        track.GetComponentInChildren<SpriteShapeController>().spline.SetPosition(1, target);
        int splineLength = track.GetComponentInChildren<SpriteShapeController>().spline.GetPointCount();
        for (int i = 0; i < splineLength; i++)
        {
            track.GetComponentInChildren<SpriteShapeController>().spline.SetTangentMode(i, ShapeTangentMode.Continuous);
        }
    }

    Vector3 GetPathVector(PathFollow path, int indexOffset)
    {
        return path.SSC.spline.GetPosition(path.GetIndex() + indexOffset);
    }

    /**
     * Instantiates a projectile prefab instead of firing a ray. 
     */
    public void ShootProjectile()
    {
        GameObject bullet = (GameObject)Instantiate(projectile);
    }
}
