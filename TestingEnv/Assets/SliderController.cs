using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;

public class SliderController : MonoBehaviour
{
	[SerializeField] Slider slider;
	[SerializeField] bool interpolate;
	[SerializeField] [ConditionalField(nameof(interpolate))] float interpolationSpeed = 2;
	public void SetRange(float min, float max){
		slider.minValue = min;
		slider.maxValue = max;
	}
	public void SetValue(float value){
		if(interpolate){
			targetValue = value;
		}
		else{
			slider.value = value;
		}
	}

	//interpolation
	float targetValue;

	void Update()
	{
		if(!interpolate){return;}
		slider.value = Mathf.Lerp(slider.value, targetValue, interpolationSpeed*Time.deltaTime * (slider.maxValue - slider.minValue));
	}
}
