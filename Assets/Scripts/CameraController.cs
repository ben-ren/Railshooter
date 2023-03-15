using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public TrainController obj;
    private float obj_y;
    [HideInInspector]
    private Rigidbody2D rb2d;
    private new Camera camera;
    private float offset;

    private float newSpeed;
    private float oldSpeed;

    void Awake() {
        rb2d = this.gameObject.GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start() {
        camera = this.gameObject.GetComponent<Camera>();
        offset = camera.orthographicSize;
        oldSpeed = Mathf.Abs(obj.speed) + offset;
    }

    // Update is called once per frame
    void FixedUpdate() {
        CameraLock();
        CameraZoom();
    }

    Vector2 VerticalCalc(float y) {
        return new Vector2(0, y);
    }

    void CameraLock() {
        //if train rotation between -45 to 45 OR 135 to 225 (in quadrant 1 or 3) then apply obj_y

        obj_y = obj.transform.position.y;
        transform.position = VerticalCalc(obj_y);

        //if train rotation between -45 to 135 OR 45 to 225 (in quadrant 2 or 4) then apply obj_x

    }

    void CameraZoom() {
        camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, offset, offset + Mathf.Abs(obj.speed)+1);
        SpeedCheck();
        camera.orthographicSize += SlideValue()*Mathf.Clamp(Mathf.Abs(obj.speed), 2 ,5);
    }

    float SlideValue()
    {
        newSpeed = offset + Mathf.Abs(obj.speed);
        float temp;
        if (newSpeed > oldSpeed)    //Accelerating
        {
            temp = 1;
        }
        else if (newSpeed < oldSpeed)    //Decelerating
        {
            temp = -1;
        }
        else
        {
            temp = 0;
        }
        return temp*Time.deltaTime;
    }

    void SpeedCheck()
    {
        if(IsBetween(camera.orthographicSize, offset + Mathf.Abs(obj.speed)-0.05f, offset + Mathf.Abs(obj.speed) + 0.05f))
        {
            oldSpeed = newSpeed;
            camera.orthographicSize = offset + Mathf.Abs(obj.speed);
        }
        if (!IsBetween(camera.orthographicSize, (Mathf.Abs(obj.speed)+offset) - 2, (Mathf.Abs(obj.speed)+offset) + 2))
        {
            camera.orthographicSize = Mathf.Abs(obj.speed) + offset;
        }
    }

    bool IsBetween(float checkValue, float lowerBound, float upperBound)
    {
        return (checkValue >= Mathf.Min(lowerBound, upperBound) && checkValue <= Mathf.Max(lowerBound, upperBound));
    }
}