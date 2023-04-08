using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class RailSwitch : MonoBehaviour
{
    public SpriteShapeController rootTrack;
    public SpriteShapeController[] inputTracks;
    public SpriteShapeController[] outputsTracks;
    private PathFollow obj;

    private Vector3 connectedRailPoint;     //The point on the rootRail that the rail is connected to. 

    public bool ObjectOnSwitch;
    private int selectedTrack;
    private Vector3 Direction;

    // Start is called before the first frame update
    void Start()
    {
        ObjectOnSwitch = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /**
     * Check for selectedTrack
     */
    void SelectTrack()
    {
        
    }

    /**
     * Set track switch direction. 
     * Set tangent and last node positions based on direction of selected output rails.
     * Spline spline = outputTracks[selectedTrack].spline;
     * spline.SetRightTangent(0, Direction)
     * spline.SetPosition(spline.length-1, 
     * 
     * For output tracks. 
     * outputTracks[selectedTrack].GetPointOnSpline(spline.length-1, .95f, spline.GetPosition(0), spline.GetPosition(2)))
     */

    /**
     * Check for the selectedTrack in outputTracks.
     * If outputTracks is empty default to connected root Track.
     */


    private void OnTriggerEnter2D(Collider2D collision)
    {
        ObjectOnSwitch = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ObjectOnSwitch = false;
    }
}
