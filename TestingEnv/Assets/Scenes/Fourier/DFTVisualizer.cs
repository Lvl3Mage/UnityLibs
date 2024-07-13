using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Fourier;
using UnityEngine;
using UnityEngine.Serialization;

public class DFTVisualizer : MonoBehaviour
{
	[FormerlySerializedAs("sampleVisualizerPrefab")] [SerializeField] DFTCoefficientVisualizer coefficientVisualizerPrefab;
	[SerializeField] float timeScale = 0.01f;
	
	DFTCoefficientVisualizer[] sampleVisualizers = new DFTCoefficientVisualizer[0];
	public void VizualiseDFT(DFT dft){
		for (int i = 0; i < sampleVisualizers.Length; i++){
			Destroy(sampleVisualizers[i].gameObject);
		}
		DFT.Coefficient[] dftSamples = dft.GetCoefficients();
		sampleVisualizers = new DFTCoefficientVisualizer[dftSamples.Length];
		for (int i = 0; i < dftSamples.Length; i++){
			sampleVisualizers[i] = Instantiate(coefficientVisualizerPrefab);
			DFTCoefficientVisualizer parent = i > 0 ? sampleVisualizers[i-1] : null;
			sampleVisualizers[i].VisualizeSample(dftSamples[i], parent);
			sampleVisualizers[i].ComputeState();
		}
		
	}
	void Update()
	{
		if (sampleVisualizers == null){
			return;
		}
		for (int i = 0; i < sampleVisualizers.Length; i++){
			sampleVisualizers[i].Rotate(Time.deltaTime*timeScale);
			sampleVisualizers[i].ComputeState();
		}
	}
}
