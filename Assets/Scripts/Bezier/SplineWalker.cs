using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Code from here https://catlikecoding.com/unity/tutorials/curves-and-splines/
public class SplineWalker : MonoBehaviour {

    public BezierSpline spline;

    public float speed;

    private float progress;

    public bool lookForward;

    public bool loop;

    // Start is called before the first frame update
    void Start(){
        
    }

    // Update is called once per frame
    void Update(){
        progress += 0.01f * (speed*Time.deltaTime);
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
}
