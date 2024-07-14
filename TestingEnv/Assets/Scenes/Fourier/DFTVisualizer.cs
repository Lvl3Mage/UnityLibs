using System;
using System.Collections;
using System.Collections.Generic;
using Fourier;
using UnityEngine;
using UnityEngine.Serialization;

public class DFTVisualizer : MonoBehaviour
{
	[SerializeField] VectorVisualizer coefficientVisualizerPrefab;
	[SerializeField] SignalRenderer dftSignalRenderer;
	List<VectorVisualizer> visualizerPool = new List<VectorVisualizer>();
	DFT dft;
	Signal dftSignal;
	float time = 0;
	bool vectorVisibility = true;
	float vectorStartOpacity;
	float vectorEndOpacity;
	float vectorOpacityPower;
	bool pathVisibility = true;
	float pathOpacity = 1;
	bool visualizeFirst = false;
	float timeScale;
	public void SetTimeScale(float newTimeScale){
		timeScale = newTimeScale;
	}
	public void SetVisualizeFirst(bool value){
		visualizeFirst = value;
	}
	bool trailVisibility;
	float trailOpacity;
	float trailDistance;
	public void SetTrailVisibility(bool value){
		trailVisibility = value;
        if(visualizerPool.Count == 0){return;}
		visualizerPool[visualizerPool.Count-1].ToggleTrail(value);
	}
	public void SetTrailOpacity(float value){
		trailOpacity = value;
        if(visualizerPool.Count == 0){return;}
		visualizerPool[visualizerPool.Count-1].SetTrailOpacity(value);
	}
	public void SetTrailDistance(float value){
		trailDistance = value;
        if(visualizerPool.Count == 0){return;}
		visualizerPool[visualizerPool.Count-1].SetTrailDistance(value);
	}
	public void VizualiseDFT(DFT newDFT, Signal targetSignal){
		time = 0;
		dft = newDFT;
		ResizePool(dft.GetDepth());
		
		
		int samples = targetSignal.GetPointCount();
		Vector2[] signalPoints = dft.ComputePath(samples);
		dftSignal = new Signal();
		
		dftSignal.Reserve(samples);
		for (int i = 0; i < signalPoints.Length; i++){
			dftSignal.AddPoint(signalPoints[i], targetSignal.IsTimeDrawn(i / (float)signalPoints.Length));
		}
		dftSignalRenderer.RenderSignal(dftSignal);
		
		dftSignalRenderer.SetVisibility(pathVisibility);
		dftSignalRenderer.SetOpacity(pathOpacity);
	}
	void ResizePool(int newSize){
		if (newSize > visualizerPool.Count){
			for (int i = visualizerPool.Count; i < newSize; i++){
				visualizerPool.Add(Instantiate(coefficientVisualizerPrefab));
			}
			foreach (var visualizer in visualizerPool){
				visualizer.ToggleTrail(false);
			}
		}
		else if (newSize < visualizerPool.Count){
			for (int i = visualizerPool.Count - 1; i >= newSize; i--){
				Destroy(visualizerPool[i].gameObject);
				visualizerPool.RemoveAt(i);
			}
		}
		
		SetTrailVisibility(trailVisibility);
		SetTrailOpacity(trailOpacity);
		SetTrailDistance(trailDistance);
		SetVectorVisibility(vectorVisibility);
		SetVectorOpacity(vectorStartOpacity, vectorEndOpacity, vectorOpacityPower);
		
	}
	void Update()
	{
		if (dft == null){
			return;
		}
		if(visualizerPool.Count == 0){return;}
		time += timeScale * Time.deltaTime;
		time %= 1;
		Vector2[] points = dft.ComputePositions(time);
		for (int i = 0; i < visualizerPool.Count; i++){
			Vector2 origin = points[i];
			Vector2 direction = points[i+1] - origin;
			visualizerPool[i].Visualize(origin, direction);
		}

		visualizerPool[0].ToggleVisibility(visualizeFirst);
	}

	public void Clear(){
		foreach (VectorVisualizer t in visualizerPool){
			Destroy(t.gameObject);
		}
		visualizerPool.Clear();
		dftSignalRenderer.Clear();
	}
	public Vector2 GetEndPosition()
	{
		if (dft == null){
			return Vector2.zero;
		}
		return dft.ComputeOrigin(visualizerPool.Count, time);
	}
	public void SetVectorStartOpacity(float start)
	{
		vectorStartOpacity = start;
		SetVectorOpacity(vectorStartOpacity, vectorEndOpacity, vectorOpacityPower);
	}
	public void SetVectorEndOpacity(float end)
	{
		vectorEndOpacity = end;
		SetVectorOpacity(vectorStartOpacity, vectorEndOpacity, vectorOpacityPower);
	}
	public void SetVectorOpacityPower(float power)
	{
		vectorOpacityPower = power;
		SetVectorOpacity(vectorStartOpacity, vectorEndOpacity, vectorOpacityPower);
	}
	void SetVectorOpacity(float start, float end, float power)
	{
		float scaledPower = Mathf.Lerp(0,-50,power);
		bool order = start < end;
		for (int i = 0; i < visualizerPool.Count; i++){
			VectorVisualizer visualizer = visualizerPool[i];
			float t = Mathf.InverseLerp(0, visualizerPool.Count-1, i);
			float a = Mathf.Lerp(start, end, Falloff(t,scaledPower));
			visualizer.SetOpacity(a);
			float orderValue = order ? t : 1-t;
			visualizer.SetZAxis(transform.position.z+orderValue);
		}
		float Falloff(float x, float p)
		{
			float c = Mathf.Max(Mathf.Min(-(1f / (p - 1f) + 1f), 0), p);
			return x/(c*(1-x)+1);
		}
	}
	public void SetVectorVisibility(bool visible)
	{
		vectorVisibility = visible;
		foreach (VectorVisualizer t in visualizerPool){
			t.ToggleVisibility(visible);
		}
	}
	public void SetPathOpacity(float a)
	{
		pathOpacity = a;
		dftSignalRenderer.SetOpacity(a);
	}
	public void SetPathVisibility(bool visible)
	{
		pathVisibility = visible;
		dftSignalRenderer.SetVisibility(visible);
	}
}
