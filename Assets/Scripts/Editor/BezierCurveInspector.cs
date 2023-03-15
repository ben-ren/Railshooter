using UnityEditor;
using UnityEngine;

public static class Bezier {
    public static Vector3 GetPoint (Vector3 p0, Vector3 p1, Vector3 p2, float t) {
		return Vector3.Lerp(Vector3.Lerp(p0, p1, t), Vector3.Lerp(p1, p2, t), t);
	}
}

[CustomEditor(typeof(BezierCurve))]
public class BezierCurveInspector : Editor{

    private const int lineSteps = 10;
    private const float directionScale = 0.5f;

    private BezierCurve curve;
    private Transform handleTransform;
    private Quaternion handleRot;

    private void OnSceneGUI(){
        curve = target as BezierCurve;
        //Changes the Handle's transform to be relative to the line object and thus relative to local space. 
        handleTransform = curve.transform;
        handleRot = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;     //Set rotation to Unity's pivot rotation mode
		
        Vector3 p0 = ShowPoint(0);
        Vector3 p1 = ShowPoint(1);
        Vector3 p2 = ShowPoint(2);
        Vector3 p3 = ShowPoint(3);

        //draws the lines. By default, Handles draw relative to the world space, ignoring the parent object's transforms. 
        Handles.color = Color.grey;
        Handles.DrawLine(p0, p1);
        Handles.DrawLine(p2, p3);

        ShowDirections();
        Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
    }

    //Moves the curve inspector display to separate function
    private void ShowDirections(){
        Handles.color = Color.green;
        Vector3 point = curve.GetPoint(0f);
        Handles.DrawLine(point, point + curve.GetDirection(0f) * directionScale);
        for(int i = 1; i <= lineSteps; i++){
            point = curve.GetPoint(i / (float)lineSteps);
            Handles.DrawLine(point, point + curve.GetDirection(i / (float)lineSteps) * directionScale);
        }
    }

    private Vector3 ShowPoint(int index){
        Vector3 point = handleTransform.TransformPoint(curve.points[index]);
        EditorGUI.BeginChangeCheck();                           //starts code block to check for GUI changes
        point = Handles.DoPositionHandle(point, handleRot);     //Displays the chosen point's position handle
        if(EditorGUI.EndChangeCheck()){
            Undo.RecordObject(curve, "Move Point");                                     //saves change to undo history
            EditorUtility.SetDirty(curve);                                              //asks to save before quitting
            curve.points[index] = handleTransform.InverseTransformPoint(point);         //transforms position vector from World space to Local space
        }
        return point;
    }
}