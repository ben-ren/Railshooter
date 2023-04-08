using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainController : PathFollow
{
    void Start()
    {
        
    }

    void Update() 
    {
        SetSpeed(5);
    }

    /**
     * Input control that let's the player choose their next track direction, relative to their current rotation.
     * 
     * Options include
     *  0. straight
     *  1. left
     *  2. right
     */
    void TrackSelect()
    {

    }

    void SetSpeed(float speed)
    {
        this.speed = speed;
    }
}
