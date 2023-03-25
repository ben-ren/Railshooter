using UnityEngine;

////Code from here https://catlikecoding.com/unity/tutorials/curves-and-splines/
public static class Bezier {
	
    public static Vector3 GetPoint (Vector3 p0, Vector3 p1, Vector3 p2, float t) {
		t = Mathf.Clamp01(t);
		float oneMinusT = 1f - t;
		return //B(t) = (1 - t)^2 * P0 + 2*(1 - t)*t*P1 + t^2*P2.
			oneMinusT * oneMinusT * p0 + 2f * oneMinusT * t * p1 + t * t * p2;
	}

	//This function produces lines tangent to the curve, which can be interpreted as the speed with which we move along the curve.
	public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, float t){
		return //B'(t) = 2*(1 - t)*(P1 - P0) + 2*t*(P2 - P1)
			2f * (1f - t) * (p1 - p0) + 2f * t * (p2 - p1);
	}

	public static Vector3 GetPoint (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
		t = Mathf.Clamp01(t);
		float oneMinusT = 1f - t;
		return //B(t) = (1 - t)^3 * P0 + 3 * (1 - t)^2 * t * P1 + 3 * (1 - t) * t2 * P2 + t3 * P3
			oneMinusT * oneMinusT * oneMinusT * p0 + 3f * oneMinusT * oneMinusT * t * p1 + 3f * oneMinusT * t * t * p2 + t * t * t * p3;
	}
	
	public static Vector3 GetFirstDerivative (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
		t = Mathf.Clamp01(t);
		float oneMinusT = 1f - t;
		return //B'(t) = 3*(1 - t)^2 * (P1 - P0) + 6 * (1 - t) * t * (P2 - P1) + 3 * t2 * (P3 - P2).
			3f * oneMinusT * oneMinusT * (p1 - p0) + 6f * oneMinusT * t * (p2 - p1) + 3f * t * t * (p3 - p2);
	}
}