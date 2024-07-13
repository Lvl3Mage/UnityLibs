using System.Collections;
using System.Collections.Generic;
using CameraManagement;
using UnityEngine;

public class CameraPan : CameraController
{
	public void Reset(Vector2 localPosition, float zoom){
		targetPosition = localPosition;
		transform.localPosition = new Vector3(localPosition.x,localPosition.y,-10);
		targetSize = zoom;
		camera.orthographicSize = zoom;
	}
	Camera camera;
	[SerializeField] float scrollSpeed = 0.5f;
	[SerializeField] float scrollLerpSpeed = 5;
	[SerializeField] Vector2 sizeClamp = new Vector2(0.1f,10);
	[SerializeField] float panLerpSpeed = 10;
	[SerializeField] float panSpeed = 1.5f;
	[SerializeField] Vector2 HorizontalClamp = new Vector2(-10,10);
	[SerializeField] Vector2 VerticalClamp = new Vector2(-10,10);

	float targetSize;
	// Start is called before the first frame update
	void Awake()
	{
		camera = gameObject.GetComponent<Camera>();
		targetSize = camera.orthographicSize;
		lastCursorPosition = camera.ScreenToWorldPoint(Input.mousePosition);
		targetPosition = transform.localPosition;
	}
	Vector2 lastCursorPosition;
	Vector3 targetPosition;
	// Update is called once per frame
	protected override void CameraUpdate()
	{
		
		targetSize -= Input.mouseScrollDelta.y*scrollSpeed*targetSize;
		targetSize = Mathf.Clamp(targetSize,sizeClamp.x,sizeClamp.y);
		camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, targetSize, scrollLerpSpeed*Time.deltaTime);
		Vector2 cursorPosition = camera.ScreenToWorldPoint(Input.mousePosition);
		if(Input.GetMouseButtonDown(1)){
			lastCursorPosition = cursorPosition;
		}
		else if(Input.GetMouseButton(1)){
			Vector2 cursorDelta = cursorPosition - lastCursorPosition;
			targetPosition -= new Vector3(cursorDelta.x, cursorDelta.y, 0)*panSpeed;
			targetPosition.x = Mathf.Clamp(targetPosition.x, HorizontalClamp.x, HorizontalClamp.y);
			targetPosition.y = Mathf.Clamp(targetPosition.y, VerticalClamp.x, VerticalClamp.y);
			lastCursorPosition = cursorPosition;
		}
		targetPosition.z = transform.localPosition.z;
		transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, panLerpSpeed*Time.deltaTime);	
		
		
	}
	void OnDrawGizmos(){
		Gizmos.color = Color.red;
		Vector3 reference = Vector3.zero;
		if (transform.parent){
			reference = transform.parent.position;
		}

		Vector3[] corners = new Vector3[]{
			reference + new Vector3(HorizontalClamp.x, VerticalClamp.x, 0),
			reference + new Vector3(HorizontalClamp.y, VerticalClamp.x, 0),
			reference + new Vector3(HorizontalClamp.y, VerticalClamp.y, 0),
			reference + new Vector3(HorizontalClamp.x, VerticalClamp.y, 0)
		};
		Gizmos.DrawLine(corners[0],corners[1]);
		Gizmos.DrawLine(corners[1],corners[2]);
		Gizmos.DrawLine(corners[2],corners[3]);
		Gizmos.DrawLine(corners[3],corners[0]);
       }
}
// Vector2 cursorPosition = Input.mousePosition;
// cursorPosition.x /= Screen.width;
// cursorPosition.y /= Screen.height;
// cursorPosition -= new Vector2(0.5f,0.5f);
// Debug.Log(cursorPosition);