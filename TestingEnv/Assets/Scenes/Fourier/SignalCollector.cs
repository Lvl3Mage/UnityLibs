using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace Fourier
{
public class SignalCollector : MonoBehaviour
{
	Signal currentSignal;
	bool collecting = false;
	[SerializeField] SignalRenderer renderer;
	[SerializeField] DFTVisualizer visualizer;


	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space)){
			if (collecting){
				Signal signal = EndCollection();
				DFT dft = new DFT(signal);
				dft.ComputeCoefficients(300);
				visualizer.VizualiseDFT(dft);
			}
			else{
				StartCollection();
				
			}
		}
		if (!collecting){
			return;
		}
		Vector2 mousePos = WorldCamera.GetWorldMousePos();
		if(!Input.GetMouseButton(0)){
			return;
		}
		if (Input.GetMouseButtonDown(0)){
			currentSignal.ConnectTo(mousePos);
		}
		currentSignal.AddPoint(mousePos); 
		renderer.RenderSignal(currentSignal);
					
	}

	void StartCollection()
	{
		currentSignal = new Signal();
		collecting = true;
	}

	Signal EndCollection()
	{
		currentSignal.EnforceCycle();
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

public class Signal
{
	List<SignalPoint> points = new List<SignalPoint>();
	
	public void AddPoints(Vector2[] newPoints, bool drawn = true)
	{
		foreach (var point in newPoints){
			points.Add(new SignalPoint(point, drawn));
		}
	}
	public void AddPoint(Vector2 newPoint, bool drawn = true)
	{
		points.Add(new SignalPoint(newPoint, drawn));
	}
	float EaseInOutCubic(float x) {
		return x < 0.5f ? 4 * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 3) / 2;
	}
	public void ConnectTo(Vector2 target, bool draw = false, float sampleDensity = 30)
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
			sampleT = EaseInOutCubic(sampleT);
			Vector2 position = Vector2.Lerp(lastPoint, target, sampleT);
			newPoints[i] = position;
		}
		AddPoints(newPoints, draw);
	}

	public void EnforceCycle()
	{
		if(points.Count < 3){
			return;
		}
		Vector2 firstPoint = points[0].position;
		ConnectTo(firstPoint);
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

	public Complex[] GetComplexPoints()
	{
		Complex[] complexPoints = new Complex[points.Count];
		for (int i = 0; i < points.Count; i++){
			SignalPoint point = points[i];
			complexPoints[i] = new Complex(point.position.x, point.position.y);
		}

		return complexPoints;
	}
	public SignalPoint[][] GetDrawnSegments()
	{
		List<SignalPoint[]> segments = new List<SignalPoint[]>();
		List<SignalPoint> currentSegment = new List<SignalPoint>();

		for (int i = 0; i < points.Count; i++){
			SignalPoint point = points[i];
			if (i == 0){
				if(point.drawn){
					currentSegment.Add(point);
				}
				continue;
			}
			SignalPoint prevPoint = points[i - 1];
			if (prevPoint.drawn){
				AddToCurrentSegment(point);
			}
			else{
				FinishCurrentSegment();
				AddToCurrentSegment(point);
			}
		}
		FinishCurrentSegment();
		return segments.ToArray();

		void AddToCurrentSegment(SignalPoint point)
		{
			if (!point.drawn){
				return;
			}
			currentSegment.Add(point);
		}

		void FinishCurrentSegment()
		{
			if (currentSegment.Count <= 0){
				return;
			}
			segments.Add(currentSegment.ToArray());
			currentSegment.Clear();
		}
	}
}
}

