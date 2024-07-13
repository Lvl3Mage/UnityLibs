using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCamera : MonoBehaviour
{
	static WorldCamera instance;
	Camera camera;
	void Awake()
	{
		camera = GetComponent<Camera>();
		if(instance != null){
			Debug.LogError("An instance of WorldCamera already exists!");
			Destroy(this);
			return;
		}
		instance = this;
	}
	public static Vector2 GetWorldMousePos(){
		return instance.camera.ScreenToWorldPoint(Input.mousePosition);
	}
	public static Camera GetCamera(){
		return instance.camera;
	}

}
