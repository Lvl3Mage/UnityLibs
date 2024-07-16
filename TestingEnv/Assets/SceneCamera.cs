using UnityEngine;
using UnityEngine.Serialization;

namespace CameraManagement2D
{
[RequireComponent(typeof(Camera))]
public class SceneCamera : MonoBehaviour
{
	static SceneCamera instance;
	[Tooltip("Activate the camera on start")]
	[SerializeField] bool startActive = true;
	[FormerlySerializedAs("attachedComponents")]
	[Tooltip("Components that should be enabled/disabled with the camera.")]
	[SerializeField] Behaviour[] attachedBehaviours; 
	Camera camera;
	void Awake()
	{
		camera = GetComponent<Camera>();


		if (!startActive) {
			camera.enabled = false;
			return;
		}
		if(instance != null){
			Debug.LogError("An instance of WorldCamera already exists! Ensure only one camera has the 'startActive' flag set.");
			// Destroy(this);
		}
		instance = this;
	}
	public void SwitchTo(){
		if(instance != null){
			instance.ToggleActive(false);
		}
		this.ToggleActive(true);
	}

	void ToggleActive(bool active){
		camera.enabled = active;
		foreach (var component in attachedBehaviours){
			component.enabled = active;
		}
		instance = active ? this : null;
	}
	public static Vector2 GetWorldMousePos(){
		return instance.camera.ScreenToWorldPoint(Input.mousePosition);
	}
	public static Camera GetCamera(){
		return instance.camera;
	}
	public static bool PointInView(Vector2 point){
		Vector3 screenPoint = instance.camera.WorldToViewportPoint(point);
		return screenPoint.x is > 0 and < 1 && screenPoint.y is > 0 and < 1;
	}

}
}
