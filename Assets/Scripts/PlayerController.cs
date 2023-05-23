using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PlayerController : TrainController
{
    protected override void Train()
    {
        base.Train();
        Controls();
    }

    /**
     * The player's inputs used to control the actions of the train's PathFollow script.
     */
    void Controls()
    {
        
    }
}
