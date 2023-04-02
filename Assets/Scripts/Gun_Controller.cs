using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Gun_Controller : MonoBehaviour{

    public GameObject projectile;
    public int railCount;

    private Spline spl;
    private Camera mainCam;
    private Vector3 mousePos;
    
    void Start(){
        spl = this.gameObject.GetComponentInParent<PathFollow>().SSC.spline;
        Debug.Log(spl);
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update(){
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        RotationCalc(mousePos);
        if (Input.GetMouseButtonDown(0)) {
            Shoot(mousePos);
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

    /**
     * Instantiates a projectile prefab instead of firing a ray. 
     */
    public void ShootProjectile()
    {
        GameObject bullet = (GameObject)Instantiate(projectile);
    }
}
