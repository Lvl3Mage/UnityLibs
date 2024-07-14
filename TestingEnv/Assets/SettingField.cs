using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourier
{
	public class SettingField : MonoBehaviour
	{
		[SerializeField] string field;
		SettingsManager settingsManager;
		void Start()
		{
			settingsManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<SettingsManager>();
		}
		public void SetBool(bool value)
		{
			settingsManager.SetBool(field, value);
		}
		public void SetInt(int value)
		{
			settingsManager.SetInt(field, value);
		}
		public void SetFloat(float value)
		{
			settingsManager.SetFloat(field, value);
		}
	}
	
}
