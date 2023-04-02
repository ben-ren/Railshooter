using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PathFollow))]
public class TrainController : MonoBehaviour
{
    private PathFollow logic;
    private float speed;

    void Start()
    {
        
    }

    void Update() 
    {
        SetSpeed(5);
        logic.speed = this.speed;
    }

    /**
     * Changes which spline is used for the PathFollow from the closest TrackSwitch
     */
    void SetActiveSpline()
    {

    }

    void SetSpeed(float speed)
    {
        this.speed = speed;
    }
}
