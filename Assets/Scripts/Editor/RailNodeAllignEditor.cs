using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RailNodeAllign))]
public class RailNodeAllignEditor : Editor
{
    private Transform _transform;

    private void OnEnable()
    {
        // Get a reference to the specific object you want to monitor
        _transform = ((RailNodeAllign)target).transform;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        base.OnInspectorGUI();

        RailNodeAllign rna = (RailNodeAllign)target;

        // Check if the transform of the specific object has changed
        if (_transform.hasChanged)
        {
            // Do something here when the transform of the specific object has changed
            Debug.Log("Transform of specific object has changed");
            rna.MoveNodesToSwitch();
            _transform.hasChanged = false; // Reset the flag
        }
    }
}
