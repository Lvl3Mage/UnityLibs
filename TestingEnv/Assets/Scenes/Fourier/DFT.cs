using System.Numerics;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Vector2 = UnityEngine.Vector2;

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
			public Vector2 GetPosition(float time)
			{
				Complex rotated = Complex.FromPolarCoordinates(value.Magnitude, value.Phase + time * 2 * Mathf.PI * index);
				return new Vector2((float)rotated.Real, (float)rotated.Imaginary);
			}
		}
		Coefficient[] coefficients;
		public DFT(Signal signal, int depth)
		{
			Complex[] signalPoints = signal.GetComplexPoints();
			ComputeCoefficients(depth, signalPoints);
		}
		Coefficient ComputeCoefficient(int k, Complex[] signalPoints)
		{
			Coefficient result = new(k, 0);
			float w = 2 * Mathf.PI * k / signalPoints.Length;
			for (int n = 0; n < signalPoints.Length; n++){
				float x = w * n;
				Complex sample = signalPoints[n];
				result.value += sample * new Complex(Mathf.Cos(x), -Mathf.Sin(x));
			}

			result.value = Complex.Divide(result.value, signalPoints.Length);
			return result;
		}
		int GetFrequency(int i)
		{
			int even = i % 2 == 0 ? 1 : -1;
			int k = (int)Mathf.Ceil(i / 2f) * even;
			return k;
		}

		void ComputeCoefficients(int amount, Complex[] signalPoints)
		{
			coefficients = new Coefficient[amount];
#if UNITY_WEBGL && !UNITY_EDITOR
			for (int i = 0; i < amount; i++){
				int k = GetFrequency(i);
				coefficients[i] = ComputeCoefficient(k, signalPoints);
			}
			
#else
			Parallel.For(0, amount, (i) => {
				int k = GetFrequency(i);
				coefficients[i] = ComputeCoefficient(k, signalPoints);
			});
#endif
		}
		public Vector2 ComputeOrigin(int coefficientIndex, float time)
		{
			Vector2 result = Vector2.zero;
			if(coefficientIndex > coefficients.Length){
				Debug.LogError("Coefficient index out of range");
			}
			for (int i = 0; i < coefficientIndex; i++){
				result += coefficients[i].GetPosition(time);
			}
			return result;
		}

		public Vector2[] ComputePath(int sampleCount)
		{
			int depth = coefficients.Length;
			Vector2[] result = new Vector2[sampleCount];
			float invSampleCount = 1f / (float)sampleCount;
#if UNITY_WEBGL && !UNITY_EDITOR
			for (int i = 0; i < sampleCount; i++){
				float time = i*invSampleCount;
				result[i] = ComputeOrigin(depth, time);
			}
			
#else
			Parallel.For(0, sampleCount, i => {
				float time = i*invSampleCount;
				result[i] = ComputeOrigin(depth, time);
			});
#endif
			return result;
		}

		public Vector2[] ComputePositions(float time)
		{
			Vector2[] result = new Vector2[coefficients.Length+1];
			result[0] = Vector2.zero;
			for (int i = 1; i < result.Length; i++){
				result[i] = result[i-1] + coefficients[i-1].GetPosition(time);
			}

			return result;

		}
		public int GetDepth()
		{
			return coefficients.Length;
		}
	}
}