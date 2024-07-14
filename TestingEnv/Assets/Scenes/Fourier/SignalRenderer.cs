using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fourier
{
public class SignalRenderer : MonoBehaviour
{
	
	[SerializeField] PathRenderer pathRendererPrefab;
	List<PathRenderer> pathRendererPool = new List<PathRenderer>();
	SignalSegment[] segments;
	public void RenderSignal(Signal signal)
	{
		segments = signal.GetDrawnSegments();
		if(segments.Length < pathRendererPool.Count){
			for (int i = pathRendererPool.Count-1; i >= segments.Length; i--){
				Destroy(pathRendererPool[i].gameObject);
				pathRendererPool.RemoveAt(i);
			}
		}
		for (int i = 0; i < segments.Length; i++){
			Vector2[] segmentPoints = segments[i].points;
			PathRenderer pathRenderer;
			if(i < pathRendererPool.Count){
				pathRenderer = pathRendererPool[i];
			}
			else{
				pathRenderer = Instantiate(pathRendererPrefab);
				pathRenderer.transform.position = transform.position;
				pathRenderer.SetOpacity(opacity);
				pathRenderer.SetVisibility(visibility);
				pathRendererPool.Add(pathRenderer);
			}
			// Vector2[] visiblePoints = segmentPoints.Take(Mathf.CeilToInt(times[i] * segmentPoints.Length)).ToArray();
			pathRenderer.DrawPoints(segmentPoints);
		};
	}
	public void Clear()
	{
		foreach (var pathRenderer in pathRendererPool){
			Destroy(pathRenderer.gameObject);
		}
		pathRendererPool.Clear();
	}
	float opacity = 1;
	public void SetOpacity(float a)
	{
		opacity = a;
		foreach (var pathRenderer in pathRendererPool){
			pathRenderer.SetOpacity(a);
		}
	}
	bool visibility = true;
	public void SetVisibility(bool value)
	{
		visibility = value;
		foreach (var pathRenderer in pathRendererPool){
			pathRenderer.SetVisibility(value);
		}
	}

	// float[] GetLocalSegmentTimes(float time)
	// {
	// 	int lastIndex = segments[segments.Length - 1].endIndex;
	// 	float[] segmentTimes = new float[segments.Length];
	// 	int segmentStart = 0;
	// 	for (int i = 0; i < segments.Length; i++){
	// 		float localTime = Mathf.InverseLerp(segmentStart, segmentStart + segments[i].points.Length, time*lastIndex);
	// 		segmentTimes[i] = localTime;
	// 		segmentStart = segments[i].endIndex;
	// 	}
	//
	// 	return segmentTimes;
	// }
}
	
}
