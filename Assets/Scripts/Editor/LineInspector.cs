using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Line))]
public class LineInspector : Editor{
    private void OnSceneGUI(){
        Line line = target as Line; 

        //Changes the Handle's transform to be relative to the line object and thus relative to local space. 
        Transform handleTransform = line.transform;
        Quaternion handleRot = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;     //Set rotation to Unity's pivot rotation mode
		Vector3 p0 = handleTransform.TransformPoint(line.p0);
		Vector3 p1 = handleTransform.TransformPoint(line.p1);

        //draws the lines. By default Handles draw relative to the world space, ignoring the parent object's transforms. 
        Handles.color = Color.white;
        Handles.DrawLine(p0, p1);

        //Show the position handles for the 2 points
        Handles.DoPositionHandle(p0, handleRot);
        Handles.DoPositionHandle(p1, handleRot);

        //Creates gameObject handles that allow us to drag our points around the scene view.  
        EditorGUI.BeginChangeCheck();                            //starts code block to check for GUI changes
		p0 = Handles.DoPositionHandle(p0, handleRot);            //position handle for point 0 (p0)
		if (EditorGUI.EndChangeCheck()) {                        //returns whether or not the GUI state has changed
            Undo.RecordObject(line, "Move Point");                  //Allows us to undo drag operations, using Unity's Undo history.
            EditorUtility.SetDirty(line);                           //Gets Unity to ask the user to save changes before quitting
			line.p0 = handleTransform.InverseTransformPoint(p0);    //transforms position vector from World space to Local space
		}
		EditorGUI.BeginChangeCheck();
		p1 = Handles.DoPositionHandle(p1, handleRot);            //position handle for point 1 (p1)
		if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(line, "Move Point");
            EditorUtility.SetDirty(line);
			line.p1 = handleTransform.InverseTransformPoint(p1);
		}
    }
}
