using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(SpriteShapeController))]
public class RailNodeAllign : MonoBehaviour
{
    public Transform StartNode;
    public Transform EndNode;
    private Vector3 parentOffset;

    private SpriteShapeController s;

    private void Start()
    {
        MoveNodesToSwitch();
    }

    /**
     * Attaches the first node of the SpriteShapeController's spline to the StartNode 
     * and the last node of the SpriteShapeController's spline to the EndNode. 
     */
    public void MoveNodesToSwitch()
    {
        parentOffset = this.transform.parent.position;

        s = this.GetComponent<SpriteShapeController>();
        Spline spline = s.spline;

        //set first node of Spline to StartNode location
        spline.SetPosition(0, StartNode.position - parentOffset);

        //set last node of spline to EndNode location
        int size = spline.GetPointCount();
        spline.SetPosition(size - 1, EndNode.position - parentOffset);
    }
}
