using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtils
{
	public static float TransformRange(float value, float min, float max, float newMin, float newMax){
		return ( (value - min) / (max - min) ) * (newMax - newMin) + newMin;
	}
	public static float ValueDecay(float val, float maxValue, float minValue, float halfPoint)
	{
		float top = maxValue - minValue;
		float bottom = val*(1/halfPoint) + 1;
		return top/bottom + minValue;
	}
	public static float ValueFalloff01(float val, float a){//01Range
		val = Mathf.Clamp01(val);
		
		float top = -val+1;
		float c = Mathf.Clamp(-((1/(a-1))+1),a,0);
		float bottom = c*val+1;
		return top/bottom;
	}
	public static float ValueActivation01(float val, float slope){//01Range
		val = Mathf.Clamp01(val);

		bool inverted = false;
		if(slope < 0){
			inverted = true;
			slope *= -1;
		}

		float elevated = Mathf.Pow(val,slope);
		float invertedElev = Mathf.Pow(1-val, slope);
		float result = elevated/(elevated+invertedElev);

		if(inverted){
			result = 1 - result;
		}
		return result;
	}
}
