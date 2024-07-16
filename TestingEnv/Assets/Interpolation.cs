﻿using UnityEngine;

namespace Utils
{
	public static class Interpolation
    {
    	/// <summary>
    	/// Framerate independent decay between 2 values
    	/// </summary>
    	/// <param name="from">Start value</param>
    	/// <param name="to">Target value</param>
    	/// <param name="speed">Speed of the decay, usually a value between 1 and 25</param>
    	/// <param name="deltaTime">Delta time since the last update</param>
    	/// <returns></returns>
    	public static float Decay(float from, float to, float speed, float deltaTime)
    	{
    		return to+(from-to)*Mathf.Exp(-speed*deltaTime);
    	}
	    /// <summary>
    	/// Framerate independent decay between 2 values
    	/// </summary>
    	/// <param name="from">Start value</param>
    	/// <param name="to">Target value</param>
    	/// <param name="speed">Speed of the decay, usually a value between 1 and 25</param>
    	/// <param name="deltaTime">Delta time since the last update</param>
    	/// <returns></returns>
    	public static float DecayZoom(float from, float to, float speed, float deltaTime)
    	{
		    float logA = Mathf.Log(from);
		    float logB = Mathf.Log(to);
    		float result = logB+(logA-logB)*Mathf.Exp(-speed*deltaTime);
		    return Mathf.Exp(result);
    	}
    	/// <summary>
    	/// Framerate independent decay between 2 angles
    	/// </summary>
    	/// <param name="from">Start angle</param>
    	/// <param name="to">Target angle</param>
    	/// <param name="speed">Speed of the decay, usually a value between 1 and 25</param>
    	/// <param name="deltaTime">Delta time since the last update</param>
    	/// <returns></returns>
    	public static float DecayAngle(float from, float to, float speed, float deltaTime)
    	{
    		float delta = Mathf.DeltaAngle(from,to);
    		float target = from + delta;
    		return Decay(from, target, speed, deltaTime);
    
    	}
    	/// <summary>
    	/// Framerate independent decay between 2 vectors
    	/// </summary>
    	/// <param name="from">Start vector</param>
    	/// <param name="to">Target vectos</param>
    	/// <param name="speed">Speed of the decay, usually a value between 1 and 25</param>
    	/// <param name="deltaTime">Delta time since the last update</param>
    	/// <returns></returns>
    	public static Vector2 Decay(Vector2 from, Vector2 to, float speed, float deltaTime)
    	{
    		return to+(from-to)*Mathf.Exp(-speed*deltaTime);
    	}
    	/// <summary>
    	/// Framerate independent decay between 2 vectors
    	/// </summary>
    	/// <param name="from">Start vector</param>
    	/// <param name="to">Target vector</param>
    	/// <param name="speed">Speed of the decay</param>
    	/// <param name="deltaTime">Delta time since the last update</param>
    	/// <returns></returns>
    	public static Vector3 Decay(Vector3 from, Vector3 to, float speed, float deltaTime)
    	{
    		return to+(from-to)*Mathf.Exp(-speed*deltaTime);
    	}
	    
    }
}