using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using CameraManagement2D;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Vector2 = UnityEngine.Vector2;

namespace Fourier
{
public class SignalCollector : MonoBehaviour, IPointerMoveHandler, IPointerDownHandler, IPointerUpHandler
{
	Signal currentSignal;
	bool collecting = false;
	[SerializeField] SignalRenderer signalPreview;
	
	[SerializeField] int minSmoothingSteps = 1;
	[SerializeField] int maxSmoothingSteps = 60;
	public void SetSmoothingSteps(float value)
	{
		int steps = (int)Mathf.Lerp(minSmoothingSteps, maxSmoothingSteps, value);
		smoothingSteps = steps;
	}
	int smoothingSteps = 20;
	Vector2[] mousePosBuffer;
	int mousePosBufferIndex;
	bool mouseDown = false;
	public void OnPointerDown(PointerEventData eventData)
	{
		if (!collecting){
			return;
		}
		if(eventData.button == PointerEventData.InputButton.Left){
			mouseDown = true;
			BeginSegment();
			currentSignal.ConnectTo(target:GetAveragePos(), easing:Ease);
		}
	}
	public void OnPointerUp(PointerEventData eventData)
	{
		if(eventData.button == PointerEventData.InputButton.Left){
			mouseDown = false;
		}
	}

	Vector2 GetAveragePos()
	{
		Vector2 averagePos = new Vector2(0, 0);
		foreach (var pos in mousePosBuffer){
			averagePos += pos;
		}
		averagePos /= mousePosBuffer.Length;
		return averagePos;
	}
	public void OnPointerMove(PointerEventData eventData)
	{
		if (!collecting){
			return;
		}
		if(!mouseDown){
			return;
		}
		Debug.Log("Pointer move");
		Vector2 mousePos = SceneCamera.GetWorldMousePos();
		mousePosBuffer[mousePosBufferIndex] = mousePos;
		mousePosBufferIndex++;
		mousePosBufferIndex %= mousePosBuffer.Length;
		
		currentSignal.AddPoint(GetAveragePos());
		
		signalPreview.RenderSignal(currentSignal);
	}
	public bool outputValid()
	{
		return currentSignal.GetPoints().Length > 0;
	}

	static float Ease(float t)
	{
		return t < 0.5f ? 4 * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 3) / 2;
	}
	void BeginSegment()
	{
		mousePosBufferIndex = 0;
		mousePosBuffer = new Vector2[smoothingSteps];
		Array.Fill<Vector2>(mousePosBuffer, SceneCamera.GetWorldMousePos());
	}
	public void StartCollection()
	{
		currentSignal = new Signal();
		collecting = true;
	}

	public Signal EndCollection()
	{
		currentSignal.EnforceCycle(easing:Ease);
		collecting = false;
		return currentSignal;
	}

	void OnDrawGizmos()
	{
		if(currentSignal == null){
			return;
		}
		Vector2[] points = currentSignal.GetPositions();
		Gizmos.color = Color.red;
		foreach (Vector2 point in points){
			// Gizmos.DrawSphere(new Vector3(point.x,point.y, 0), 0.1f);
		}
	}

}
public class SignalPoint
{
	public Vector2 position;
	public bool drawn;

	public SignalPoint(Vector2 position, bool drawn = false)
	{
		this.position = position;
		this.drawn = drawn;
	}
}

public class SignalSegment
{
	public readonly Vector2[] points;
	public readonly int endIndex;
	public SignalSegment(Vector2[] points, int endIndex)
	{
		this.points = points;
		this.endIndex = endIndex;
	}
}

public class Signal
{
	List<SignalPoint> points = new List<SignalPoint>();

	public void Reserve(int totalAmount)
	{
		points.Capacity = totalAmount;
	}
	public void AddPoints(Vector2[] newPoints, bool drawn = true)
	{
		foreach (var point in newPoints){
			points.Add(new SignalPoint(point, drawn));
		}
	}

	public bool IsTimeDrawn(float time)
	{
		int timeIndex = (int)(time * points.Count);
		return points[timeIndex].drawn;
	}
	public void CloseSegment()
	{
		AddPoint(points[points.Count-1].position, false);
	}
	public void AddPoint(Vector2 newPoint, bool drawn = true)
	{
		points.Add(new SignalPoint(newPoint, drawn));
	}
	public void ConnectTo(Vector2 target, bool draw = false, float sampleDensity = 30, Func<float, float> easing = null)
	{
		if(points.Count == 0){
			return;
		}
		Vector2 lastPoint = points[points.Count - 1].position;
		float distance = Vector2.Distance(lastPoint, target);
		int samples = (int)(distance * sampleDensity);
		Vector2[] newPoints = new Vector2[samples];
		for (int i = 0; i < samples; i++){
			float sampleT = i/(float)samples;
			if (easing != null){
				sampleT = easing(sampleT);
			}
			Vector2 position = Vector2.Lerp(lastPoint, target, sampleT);
			newPoints[i] = position;
		}
		AddPoints(newPoints, draw);
	}

	public void EnforceCycle(float sampleDensity = 30, Func<float, float> easing = null)
	{
		if(points.Count < 3){
			return;
		}
		Vector2 firstPoint = points[0].position;
		ConnectTo(target:firstPoint, sampleDensity:sampleDensity, easing:easing);
	}

	public Vector2[] GetPositions()
	{
		Vector2[] positions = new Vector2[points.Count];
		for (int i = 0; i < points.Count; i++){
			positions[i] = points[i].position;
		}
		return positions;
	}
	public SignalPoint[] GetPoints()
	{
		return points.ToArray();
	}
	public int GetPointCount()
	{
		return points.Count;
	}

	public Complex[] GetComplexPoints()
	{
		Complex[] complexPoints = new Complex[points.Count];
		for (int i = 0; i < points.Count; i++){
			SignalPoint point = points[i];
			complexPoints[i] = new Complex(point.position.x, point.position.y);
		}

		return complexPoints;
	}
	public SignalSegment[] GetDrawnSegments()
	{
		List<SignalSegment> segments = new();
		List<Vector2> currentSegmentPoints = new();
		for (int i = 0; i < points.Count-1; i++){
			SignalPoint point = points[i];
			AddToCurrentSegment(point);
			if(!point.drawn && points[i+1].drawn){
				AddSegment(currentSegmentPoints.ToArray(), i - 1);//Todo check if end index is correct
				currentSegmentPoints.Clear();
			}
		}
		
		AddToCurrentSegment(points[points.Count-1]);
		AddSegment(currentSegmentPoints.ToArray(), points.Count - 1);
		if (segments.Count > 0){
			Debug.Log($"Segment length: {segments[0].points.Length} Segment end: {segments[0].endIndex}");
		}
		return segments.ToArray();

		void AddToCurrentSegment(SignalPoint point)
		{
			if (!point.drawn){
				return;
			}
			currentSegmentPoints.Add(point.position);
		}

		void AddSegment(Vector2[] signalPoints, int end)
		{
			if (signalPoints.Length <= 0){
				return;
			}
			segments.Add(new SignalSegment(signalPoints, end));
		}
	}
}
}

