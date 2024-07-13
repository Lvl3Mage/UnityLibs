using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Fourier
{
	public class DFT
	{
		public class Coefficient
		{
			public int index;
			public Complex value;
			public Coefficient(int index, Complex value)
			{
				this.index = index;
				this.value = value;
			}
		}
		List<Coefficient> coefficients;
		Complex[] signalPoints;
		public DFT(Signal signal)
		{
			coefficients = new List<Coefficient>();
			signalPoints = signal.GetComplexPoints();
		}
		void ComputeCoefficient(int k)
		{
			Coefficient result = new Coefficient(k, 0);
			float w = 2 * Mathf.PI * k / signalPoints.Length;
			for (int n = 0; n < signalPoints.Length; n++){
				float x = w * n;
				Complex sample = signalPoints[n];
				result.value += sample * new Complex(Mathf.Cos(x), -Mathf.Sin(x));
			}

			result.value = Complex.Divide(result.value, 2*signalPoints.Length);
			coefficients.Add(result);
		}
		int GetCurrentFrequency()
		{
			int even = coefficients.Count % 2 == 0 ? 1 : -1;
			int k = (int)Mathf.Ceil(coefficients.Count / 2f) * even;
			return k;
		}
		public void ComputeCoefficients(int amount)
		{
			coefficients.Clear();
			for (int i = 0; i < amount; i++){
				int k = GetCurrentFrequency();
				ComputeCoefficient(k);
			}
		}
		public Coefficient[] GetCoefficients()
		{
			return coefficients.ToArray();
		}
	}
}