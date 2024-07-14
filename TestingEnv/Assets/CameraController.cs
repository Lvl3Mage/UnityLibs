using UnityEngine;

namespace CameraManagement
{
	public class CameraController : MonoBehaviour
	{
		[SerializeField] protected bool active = true;
		protected bool respondToInput = true;
		[SerializeField] bool controlledExternally = false;
		public void SetRespondToInput(bool respondToInput)
		{
			this.respondToInput = respondToInput;
		}
		public void MoveCamera()
		{
			if (!controlledExternally){
				Debug.LogError("Camera Controller is not controlled externally");
				return;
			}

			CameraUpdate();
		}
		public void LateUpdate()
		{
			if (controlledExternally){
				return;
			}
			if (!active)
			{
				return;
			}
			CameraUpdate();
		}
		protected virtual void CameraUpdate()
		{
			throw new System.NotImplementedException();	
		}
	}
}