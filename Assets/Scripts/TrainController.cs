using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainController : PathWalker
{
    void Update() {
        TrainControls();
        MoveLogic();
    }

    void TrainControls(){
        if (Input.anyKeyDown){
            speed += (int)Input.GetAxisRaw("Vertical");
            speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
        }
    }
}
