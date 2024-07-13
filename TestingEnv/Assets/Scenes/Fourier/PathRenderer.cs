using System.Collections;
using System.Collections.Generic;
using Fourier;
using UnityEngine;

public class PathRenderer : MonoBehaviour
{
	[SerializeField] LineRenderer lineRenderer;

	public void DrawPoints(Vector2[] points)
	{
		lineRenderer.positionCount = points.Length;
		Vector3[] linePoints = new Vector3[points.Length];
		for (int i = 0; i < points.Length; i++){
			Vector2 point = points[i];
			linePoints[i] = new Vector3(point.x, point.y, 0);
		}

		lineRenderer.SetPositions(linePoints);
	}
	public void DrawPoints(SignalPoint[] points)
	{
		lineRenderer.positionCount = points.Length;
		Vector3[] linePoints = new Vector3[points.Length];
		for (int i = 0; i < points.Length; i++){
			SignalPoint point = points[i];
			linePoints[i] = new Vector3(point.position.x, point.position.y, 0);
		}

		lineRenderer.SetPositions(linePoints);
	}
		
}
