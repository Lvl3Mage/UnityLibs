using System;
using System.Collections;
using System.Collections.Generic;
using CameraManagement;
using UnityEngine;

namespace Fourier
{
public class SettingsManager : MonoBehaviour
{
	bool autoApplyOverrides = false;
	Dictionary<string, Setting<bool>> boolSettings = new Dictionary<string, Setting<bool>>{
		
	};
	Dictionary<string, Setting<int>> intSettings = new Dictionary<string, Setting<int>>{
		
	};

	Dictionary<string, Setting<float>> floatSettings;

	public void SetFloat(string key, float value)
	{
		floatSettings[key].apply(value);
	}

	public void SetInt(string key, int value)
	{
		intSettings[key].apply(value);
	}

	public void SetBool(string key, bool value)
	{
		boolSettings[key].apply(value);
	}
	void Awake()
	{
		boolSettings = new Dictionary<string, Setting<bool>>{
			{"vectorVisibility", new Setting<bool>((value) => dftVisualizer.SetVectorVisibility(value), true)},
			{"hideFirst", new Setting<bool>((value) => dftVisualizer.SetVisualizeFirst(!value), true)},
			{"followDFT", new Setting<bool>((value) => {
				if (sceneManager.IsVisualizing()){
					cameraManager.SetActive(value ? 1 : 0);
				}
			}, false)},
			{"showPreview", new Setting<bool>((value) => {
				if (sceneManager.IsVisualizing()){
					inputSignalPreview.SetVisibility(value);
				}
			}, true)},
			{"showPath", new Setting<bool>((value) => {
				dftVisualizer.SetPathVisibility(value);
			}, true)},
			{"showTrail", new Setting<bool>((value) => {
				dftVisualizer.SetTrailVisibility(value);
			}, false)},
			
		};
		intSettings = new Dictionary<string, Setting<int>>{
			
		};
		floatSettings = new Dictionary<string, Setting<float>>{
			{"visualizationDepth", new Setting<float>((value) => sceneManager.SetDFTSamples(EaseSine(value)), 0.6f)},
			{"timeScale", new Setting<float>((value) => {
				float absValue = Mathf.Abs(value);
				absValue = EaseSine(absValue);
				dftVisualizer.SetTimeScale(Mathf.Sign(value)*absValue);
			}, 0.1f)},
			{"vectorStartOpacity", new Setting<float>((value)=> dftVisualizer.SetVectorStartOpacity(value), 1f)},
			{"vectorEndOpacity", new Setting<float>((value)=> dftVisualizer.SetVectorEndOpacity(value), 1f)},
			{"vectorOpacityPower", new Setting<float>((value) => dftVisualizer.SetVectorOpacityPower(value), 1f)},
			{"cursorSmoothness", new Setting<float>((value)=>signalCollector.SetSmoothingSteps(value), 0.2f)},
			{"inputOpacity", new Setting<float>((value) => {
				if (sceneManager.IsVisualizing()){
					inputSignalPreview.SetOpacity(value);
				}
			}, 0.4f)},
			{"pathOpacity", new Setting<float>((value) => {
				dftVisualizer.SetPathOpacity(value);
			}, 1)},
			{"trailOpacity", new Setting<float>((value) => {
				dftVisualizer.SetTrailOpacity(value);
			}, 1)},
			{"trailDistance", new Setting<float>((value) => {
				dftVisualizer.SetTrailDistance(value);
			}, 5)},
		}; 
	}
	float EaseSine(float x)
	{
		 return 1 - Mathf.Cos((x * Mathf.PI) / 2);
	}

	class Setting<T>
	{
		public readonly Action<T> apply;
		public T value;
		public Setting(Action<T> externalApply, T startValue){
			apply = (newValue) => {
				value = newValue;
				externalApply(newValue);
			};
			apply(startValue);
		}
	}
	[SerializeField] CameraManager cameraManager;
	[SerializeField] DFTVisualizer dftVisualizer;
	[SerializeField] SceneManager sceneManager;
	[SerializeField] SignalCollector signalCollector;
	[SerializeField] SignalRenderer inputSignalPreview;
	void ApplySettings()
	{
		foreach (var setting in boolSettings){
			setting.Value.apply(setting.Value.value);
		}
		foreach (var setting in intSettings){
			setting.Value.apply(setting.Value.value);
		}
		foreach (var setting in floatSettings){
			setting.Value.apply(setting.Value.value);
		}
	}

	public void ToggleAutoApplyOverrides(bool value)
	{
		autoApplyOverrides = value;
		if (autoApplyOverrides){
			ApplySettings();
		}
	}
}
	
}