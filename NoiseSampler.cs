[System.Serializable]
public class NoiseSampler
{
	[SerializeField] float noiseScale;
	[SerializeField] Vector2 scrollSpeed;
	[SerializeField] Vector2 seed;
	[SerializeField] float minValue;
	[SerializeField] float maxValue;
	public float SampleAt(Vector2 position){
		Vector2 noiseCoords = position/noiseScale + scrollSpeed*Time.time +seed;
		float val = MathUtils.TransformRange(Mathf.PerlinNoise(noiseCoords.x, noiseCoords.y),0,1,minValue,maxValue);
		return val;
	}
}