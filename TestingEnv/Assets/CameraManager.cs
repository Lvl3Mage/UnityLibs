using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CameraManagement
{
	public class CameraManager : MonoBehaviour
	{
		[SerializeField] CameraController[] cameraControllers;
		int activeIndex = 0;
		bool active = true;
		public void SetActive(int index)
		{
			activeIndex = index;
		}
		void LateUpdate()
		{
			if(!active){return;}
			cameraControllers[activeIndex].MoveCamera();
		}

		public void ToggleActive(bool value)
		{
			active = value;
		}
		public void SetRespondToInput(bool value)
		{
			foreach (var controller in cameraControllers){
				controller.SetRespondToInput(value);
			}
		}
	}
}