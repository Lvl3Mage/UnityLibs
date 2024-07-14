using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Fourier;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class VectorVisualizer : MonoBehaviour
{
	[SerializeField] SpriteRenderer spriteRenderer;
	[SerializeField] TrailRenderer trailRenderer;
	public void Visualize(Vector2 origin, Vector2 direction)
	{
		transform.position = new Vector3(origin.x, origin.y, transform.position.z);
		transform.localScale = Vector3.one * direction.magnitude;
		transform.rotation = Quaternion.Euler(0,0,Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
	}
	public void SetOpacity(float opacity)
	{
		Color color = spriteRenderer.color;
		color.a = opacity;
		spriteRenderer.color = color;
	}
	public void ToggleVisibility(bool visible)
	{
		spriteRenderer.enabled = visible;
	}
	public void ToggleTrail(bool visible)
	{
		trailRenderer.enabled = visible;
	}
	public void SetTrailOpacity(float opacity)
	{
		Color color = trailRenderer.startColor;
		color.a = opacity;
		trailRenderer.startColor = color;
		trailRenderer.endColor = color;
	}
	public void SetTrailDistance(float distance)
	{
		trailRenderer.time = distance;
	}

	public void SetZAxis(float positionZ)
	{
		transform.position = new Vector3(transform.position.x, transform.position.y, positionZ);
	}
}
