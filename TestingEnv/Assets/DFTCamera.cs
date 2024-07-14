using System.Collections;
using System.Collections.Generic;
using CameraManagement;
using UnityEngine;

namespace Fourier
{
public class DFTCamera : CameraController
{
		Camera camera;
        [SerializeField] float scrollSpeed = 0.5f;
        [SerializeField] float scrollLerpSpeed = 5;
        [SerializeField] Vector2 sizeClamp = new Vector2(0.1f,10);
        [SerializeField] DFTVisualizer dftVisualizer;
        float targetSize;
        // Start is called before the first frame update
        void Awake()
        {
        	camera = gameObject.GetComponent<Camera>();
        	targetSize = camera.orthographicSize;
        }
        // Update is called once per frame
        protected override void CameraUpdate()
        {
        	if (respondToInput){
        		targetSize -= Input.mouseScrollDelta.y * scrollSpeed * targetSize;
        	}
        	targetSize = Mathf.Clamp(targetSize,sizeClamp.x,sizeClamp.y);
        	camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, targetSize, scrollLerpSpeed*Time.deltaTime);
        	Vector2 endPosition = dftVisualizer.GetEndPosition();
	        transform.localPosition = new Vector3(endPosition.x,endPosition.y,transform.localPosition.z);
        	
        }
}
	
}
