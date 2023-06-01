using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class TrainController : PathFollow
{
    SpriteShapeController shape;
    Vector3 target;
    float dist;
    float step;
    public bool trackShift;

    //Automatically runs the Start & update functions 
    
    /**
     * Allows input of distinct controller code into PathFollow parent script
     */
    protected override void Logic()
    {
        base.Logic();
        Train();
    }

    /**
     * Action taken specific to train gameObjects, whether player or AI controlled.
     */
    protected virtual void Train()
    {
        if (trackShift)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, step);
            SetLastPoint();
        }
        if (ComparePositions())
        {
            trackShift = false;
            stop = false;
        }
    }

    /**
     * compare's the current position with the target position, used to activate and deactivate the trackShift
     */
    bool ComparePositions()
    {
        return this.transform.position == target;
    }

    /**
     * Checks if the current collider's RailSwitch is in the correct state to trigger the trackShift.
     */
    bool CheckSwitchState(Collider2D col)
    {
        int track = col.gameObject.GetComponent<RailSwitch>().selectedTrack;    //check currentTrack
        shape = col.gameObject.GetComponent<RailSwitch>().rootTrack;            //check for rootTrack

        return track != -1 || track == -1 && this.SSC != shape;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        target = collision.gameObject.transform.position;
        
        if (collision.gameObject.CompareTag("TrackSwitch") && !ComparePositions() && CheckSwitchState(collision))
        {
            stop = true;
            trackShift = true;
            
            dist = (this.transform.position - collision.gameObject.transform.position).magnitude;
            step = Time.deltaTime * (speed / dist);
        }
    }

}
