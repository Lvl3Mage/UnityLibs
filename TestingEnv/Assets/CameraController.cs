using UnityEngine;

namespace CameraManagement
{
	public class CameraController : MonoBehaviour
	{
		[SerializeField] protected bool active = true;
		public void SetActive(bool active)
		{
			bool oldState = this.active;
			this.active = active;
			if (oldState != active){
				OnActiveChanged(oldState, active);
			}
		}
		protected virtual void OnActiveChanged(bool oldState, bool newState)
		{
			//throw new System.NotImplementedException();
		}
		void LateUpdate()
		{
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