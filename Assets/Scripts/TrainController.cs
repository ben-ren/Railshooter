using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainController : PathFollow
{
    //Automatically runs the Start & update functions 
    
    /**
     * Allows input of distinct controller code into PathFollow parent script
     */
    protected override void Logic()
    {
        base.Logic();
        Test();
    }

    void Test()
    {
        Debug.Log("test successful");
    }
}
