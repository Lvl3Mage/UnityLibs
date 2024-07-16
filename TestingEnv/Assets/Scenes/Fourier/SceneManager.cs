using System;
using System.Collections;
using System.Collections.Generic;
using CameraManagement2D;
using UnityEngine;
using UnityEngine.Serialization;

namespace Fourier
{
public class SceneManager : MonoBehaviour
{
	[SerializeField] CameraManager cameraManager;
	[SerializeField] SignalRenderer signalPreview;
	[SerializeField] SignalCollector signalCollector;
	[SerializeField] DFTVisualizer visualizer;
	[SerializeField] int dftMaxSamples = 500;
	[SerializeField] int dftMinSamples = 3;
	int dftSamples;
	Signal currentSignal;
	[SerializeField] UIManager uiManager;
	[SerializeField] SettingsManager settingsManager;
	SceneStep[] sceneSteps;
	int currentStepIndex = 0;
	
	void Start()
	{
		UIManager manager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
		sceneSteps = new SceneStep[]{
			new SceneStep(
				manager,
				"Collecting Signal", 
				"Click and drag to draw a signal",
				"Continue to DFT visualization",
				() => {
					
					settingsManager.ToggleAutoApplyOverrides(false);
					signalPreview.Clear();
					visualizer.Clear();
					signalPreview.SetVisibility(true);
					signalPreview.SetOpacity(1);
					signalCollector.StartCollection();
				},
				() => {
					cameraManager.SwitchToController(0);
					TryAdvanceStep();
				},
				signalCollector.outputValid
			),
			new SceneStep(
				manager,
				"Displaying DFT", 
				"Play around with the DFT configuration and see the results ",
				"Capture a different signal",
				() => {
					currentSignal = signalCollector.EndCollection();
					settingsManager.ToggleAutoApplyOverrides(true);
					CreateVisualization(currentSignal);
				},
				TryAdvanceStep
			),
			
		};
		sceneSteps[currentStepIndex].Start();
		return;

		void TryAdvanceStep()
		{
			if (Input.GetKeyDown(KeyCode.Return)){
				AdvanceStep();
			}
		}

		
	}
	public bool IsVisualizing()
	{
		return currentStepIndex == 1;
	}
	public void SetDFTSamples(float value)
	{
		dftSamples = (int)(dftMinSamples + value * (dftMaxSamples - dftMinSamples));
		if(IsVisualizing()){
			CreateVisualization(currentSignal);
		}
		
	}

	void CreateVisualization(Signal signal)
	{
		DFT dft = new(signal, dftSamples);
		visualizer.VizualiseDFT(dft, signal);
	}

	void AdvanceStep()
	{
		if (!sceneSteps[currentStepIndex].IsValid()){
			return;
		}
		currentStepIndex++;
		currentStepIndex %= sceneSteps.Length;
		sceneSteps[currentStepIndex].Start();
	}
	
	void Update()
	{
		sceneSteps[currentStepIndex].Update();
	}

	struct SceneStep
	{
		string actionName;
		string tip;
		string continueMessage;
		Action startAction;
		Action updateAction;
		Func<bool> validFunc;
		UIManager manager;
		public SceneStep(UIManager manager, string actionName, string tip, string continueMessage, Action startAction, Action updateAction = null, Func<bool> validFunc = null)
		{
			this.actionName = actionName;
			this.continueMessage = continueMessage;
			this.tip = tip;
			this.startAction = startAction;
			this.manager = manager;
			this.updateAction = updateAction;
			this.validFunc = validFunc;
		}

		public void Start()
		{
			manager.SetAction(actionName, continueMessage, tip);
			startAction();
		}

		public void Update()
		{
			if (updateAction != null){
				updateAction();
			}
		}

		public bool IsValid()
		{
			return validFunc == null || validFunc();
		}
	}
}
	
}

