using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BezierSpline))]
public class BezierSplineInspector : Editor{

    private const int stepsPerCurve = 10;
    private const float directionScale = 0.5f;
    private const float handleSize = 0.04f;
    private const float pickSize = 0.06f;

    private int selectedIndex = -1;

    private BezierSpline spline;
    private Transform handleTransform;
    private Quaternion handleRot;

    private void OnSceneGUI(){
        spline = target as BezierSpline;
        //Changes the Handle's transform to be relative to the line object and thus relative to local space. 
        handleTransform = spline.transform;
        handleRot = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;     //Set rotation to Unity's pivot rotation mode
		
        Vector3 p0 = ShowPoint(0);
        for(int i=1; i<spline.ControlPointCount; i += 3){
            Vector3 p1 = ShowPoint(i);
            Vector3 p2 = ShowPoint(i+1);
            Vector3 p3 = ShowPoint(i+2);

            //draws the lines. By default, Handles draw relative to the world space, ignoring the parent object's transforms. 
            Handles.color = Color.grey;
            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p2, p3);

            Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
            p0 = p3;
        }
        ShowDirections();
    }

    //Moves the spline inspector display to separate function
    private void ShowDirections(){
        Handles.color = Color.green;
        Vector3 point = spline.GetPoint(0f);
        Handles.DrawLine(point, point + spline.GetDirection(0f) * directionScale);
        int steps = stepsPerCurve * spline.CurveCount;
        for(int i = 1; i <= steps; i++){
            point = spline.GetPoint(i / (float)steps);
            Handles.DrawLine(point, point + spline.GetDirection(i / (float)steps) * directionScale);
        }
    }

    private static Color[] modeColors = {
        Color.white,
        Color.yellow,
        Color.cyan
    };

    private Vector3 ShowPoint(int index){

        Vector3 point = handleTransform.TransformPoint(spline.GetControlPoint(index));
        float size = HandleUtility.GetHandleSize(point);

        if (index == 0) {
			size *= 2f;
		}
        
        Handles.color = modeColors[(int)spline.GetControlPointMode(index)];

        if(Handles.Button(point, handleRot, size * handleSize, size * pickSize, Handles.DotHandleCap)){
            selectedIndex = index;
            Repaint();
        }

        if(selectedIndex == index){
            EditorGUI.BeginChangeCheck();                           //starts code block to check for GUI changes
            point = Handles.DoPositionHandle(point, handleRot);     //Displays the chosen point's position handle
            if(EditorGUI.EndChangeCheck()){
                Undo.RecordObject(spline, "Move Point");                                     //saves change to undo history
                EditorUtility.SetDirty(spline);                                              //asks to save before quitting
                spline.SetControlPoint(index, handleTransform.InverseTransformPoint(point));         //transforms position vector from World space to Local space
            }
        }
        return point;
    }

    public override void OnInspectorGUI(){
        //DrawDefaultInspector();       //Allows for direct access to the array in the inspector. 
        spline = target as BezierSpline;
        EditorGUI.BeginChangeCheck();
		bool loop = EditorGUILayout.Toggle("Loop", spline.Loop);
		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(spline, "Toggle Loop");
			EditorUtility.SetDirty(spline);
			spline.Loop = loop;
		}
        if (selectedIndex >= 0 && selectedIndex < spline.ControlPointCount) {
			DrawSelectedPointInspector();
		}
        if(GUILayout.Button("Add Curve")){
            Undo.RecordObject(spline, "Add Curve");
            spline.AddCurve();
            EditorUtility.SetDirty(spline);
        }
    }

    private void DrawSelectedPointInspector() {
		GUILayout.Label("Selected Point");
		EditorGUI.BeginChangeCheck();
		Vector3 point = EditorGUILayout.Vector3Field("Position", spline.GetControlPoint(selectedIndex));
		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(spline, "Move Point");
			EditorUtility.SetDirty(spline);
			spline.SetControlPoint(selectedIndex, point);
		}
        EditorGUI.BeginChangeCheck();
		BezierControlPointMode mode = (BezierControlPointMode) EditorGUILayout.EnumPopup("Mode", spline.GetControlPointMode(selectedIndex));    
		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(spline, "Change Point Mode");
			spline.SetControlPointMode(selectedIndex, mode);
			EditorUtility.SetDirty(spline);
		}
	}
}