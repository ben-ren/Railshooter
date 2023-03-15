using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun_Controller : MonoBehaviour{
    private Camera mainCam;
    private Vector3 mousePos;
    
    void Start(){
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update(){
        rotationCalc();
    }

    void rotationCalc(){
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        float z_rot = Mathf.Rad2Deg*Mathf.Atan2((mousePos.y - transform.position.y), (mousePos.x - transform.position.x)) - 90;     //360 deree rotation
        //float z_rot = Mathf.Rad2Deg*Mathf.Atan(-(mousePos.x - transform.position.x) / (mousePos.y - transform.position.y));       //180 degree rotation
        transform.rotation = Quaternion.Euler(0, 0, z_rot);
    }

    //Shoots a rail from the gun
    void ShootRail(){
        //Creates a rail prefab
        //1st/origin node is at train position
        //4th/last/target node is at gun target position
        //the other 2 node vector positions are calculated using the origin node, last node, Gun_Controller.rotationCalc() and Train_Controller.RotationCalculation().
        //This complex calculation is used to provide a curved track when the train is firing at an angle. 
    }
}
