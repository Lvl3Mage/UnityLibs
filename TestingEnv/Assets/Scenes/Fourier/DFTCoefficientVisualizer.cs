using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Fourier;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class DFTCoefficientVisualizer : MonoBehaviour
{
	DFTCoefficientVisualizer parent;
	Complex sample;
	int k;
	public void VisualizeSample(DFT.Coefficient coefficient, DFTCoefficientVisualizer parent)
	{
		this.k = coefficient.index;
		this.sample = coefficient.value;
		this.parent = parent;
	}

	public void ComputeState()
	{
		Vector2 startPos = Vector2.zero;
		if (parent){
			startPos = parent.GetChildPosition();
		}
		transform.position = startPos + new Vector2((float)sample.Real, (float)sample.Imaginary);
		transform.localScale = Vector3.one * (float)sample.Magnitude*2;
		transform.rotation = Quaternion.Euler(0,0,Mathf.Atan2((float)sample.Imaginary, (float)sample.Real) * Mathf.Rad2Deg);
	}
	public void Rotate(float amount)
	{
		sample = Complex.FromPolarCoordinates(sample.Magnitude, sample.Phase + amount*2*Mathf.PI*k);
	}
	public Vector2 GetChildPosition()
	{
		return (Vector2)transform.position + new Vector2((float)sample.Real, (float)sample.Imaginary);
	}
}
