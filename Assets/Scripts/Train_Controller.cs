using System;                       //This is needed to use Math class functions. Note: All Math class functions start with a capital (e.g Math.Min())
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train_Controller : MonoBehaviour
{
    /*
     * Redesign Train_Controller to extend PathWalker class
     * Add function to control speed variable
     */

    //Calculate train movement within camera
    public BezierSpline spline;
    public float duration;
    public bool lookForward;
    public bool loop;

    public float speed;

    private float progress;

    private float x;
    private float y;
    
    private Vector2 direction;
    private Rigidbody2D rb2d;

    void Awake(){
        rb2d = this.gameObject.GetComponent<Rigidbody2D>();
    }

    void Start(){
        
    }

    void FixedUpdate(){
        trackFollow();
    }

    void moveCalc(){
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        direction = new Vector2(x,y);
        direction.Normalize();
    }

    void trackFollow(){
        //modify progress to apply velocity based on the Camera speed.
        progress += 0.01f * (speed*Time.deltaTime);
        //Debug.Log("progress:" + progress + ", Time stamp: " + Time.deltaTime);
        if(progress > 1f){
            progress = 1f;
            if(loop){
                progress = 0f;  //resets spline progress for loops
            }
        }
        Vector3 position = spline.GetPoint(progress);
        transform.localPosition = position;
        if(lookForward){
            RotationCalculation(position + spline.GetDirection(progress));
        }
    }

    void RotationCalculation(Vector3 coords){
        float z_rot = Mathf.Rad2Deg*Mathf.Atan2((coords.y - transform.position.y), (coords.x - transform.position.x)) - 90;     //360 deree rotation
        transform.rotation = Quaternion.Euler(0, 0, z_rot);
    }
    
    void OnCollisionEnter2D(Collision2D col){
        //calculate track switching using spline start nodes with collision hulls

        //also calculate enemy damage calculation using collision hulls    

    }
}