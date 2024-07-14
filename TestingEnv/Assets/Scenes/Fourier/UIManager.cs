using System;
using System.Collections;
using System.Collections.Generic;
using CameraManagement;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Fourier
{
	public class UIManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		[SerializeField] TextController actionTitle;
		[SerializeField] TextController actionContinueText;
		[SerializeField] TextController tipText;
		[SerializeField] Animator optionsAnimator;
		[SerializeField] CameraManager cameraManager;
		public void SetAction(string title, string continueText, string tip = "")
		{
			actionTitle.Set(title);
			actionContinueText.Set(continueText);
			tipText.Set(tip);
		}
		public void ToggleOptions()
		{
			optionsAnimator.SetBool("Open", !optionsAnimator.GetBool("Open"));
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			cameraManager.SetRespondToInput(false);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			cameraManager.SetRespondToInput(true);
		}

		void Update()
		{
			if(Input.GetKeyDown(KeyCode.Escape)){
				ToggleOptions();
			}
		}
	}
	
}

