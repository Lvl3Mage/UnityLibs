using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourier
{
public class SignalRenderer : MonoBehaviour
{
	
	[SerializeField] PathRenderer pathRendererPrefab;
	List<PathRenderer> pathRendererPool = new List<PathRenderer>();
	public void RenderSignal(Signal signal, bool drawAll = false)
	{
		SignalPoint[][] segments;
		if (drawAll){
			segments = new[]{signal.GetPoints()};
		}
		else{
			segments = signal.GetDrawnSegments();
		}

		for (int i = 0; i < segments.Length; i++){
			SignalPoint[] segment = segments[i];
			PathRenderer pathRenderer;
			if(i < pathRendererPool.Count){
				pathRenderer = pathRendererPool[i];
			}
			else{
				pathRenderer = Instantiate(pathRendererPrefab);
				pathRendererPool.Add(pathRenderer);
			}
			pathRenderer.DrawPoints(segment);
		}
	}
}
	
}
