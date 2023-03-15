using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailSwitch : MonoBehaviour{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Instantiate RailSwitch objects onto start and end nodes of Splines.

    //Splines generated within range of a RailSwitch object hitbox (at start or end node) automatically attaches end node to switch position.

    //RailSwitch automatically attaches to Start node. 
    
    //If train object is in RailSwitch object hitbox AT START NODE, player can change bool for switching to attached track. 

}
