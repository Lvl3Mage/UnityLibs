using System.Collections;
using System.Collections.Generic;
using CameraManagement2D;
using UnityEngine;

namespace Fourier
{
public class DFTCamera : CameraController
{
        [SerializeField] float scrollSpeed = 0.5f;
        [SerializeField] Vector2 sizeClamp = new Vector2(0.1f,10);
        [SerializeField] DFTVisualizer dftVisualizer;

        CameraState targetState;
        protected override CameraState CalculateCameraState ()
        {
	        if(targetState.IsEmpty()){
		        targetState = CameraState.FromCamera(controllerCamera);
	        }
	        
        	if (useUserInput){
		        targetState = targetState.ExponentialZoom(-Input.mouseScrollDelta.y * scrollSpeed)
			        .ClampedZoom(sizeClamp);
	        }
	        
        	Vector2 endPosition = dftVisualizer.GetEndPosition();
	        targetState = targetState.WithPosition(endPosition);
        	return targetState;
        }
}
	
}
