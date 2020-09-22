namespace Mapbox.Examples
{
	using UnityEngine;

	public class CameraBillboard : MonoBehaviour
	{
		public Camera _camera;

		public void Start()
		{
			_camera = Camera.main;
		}

		void Update()
		{
			if (_camera == null) {
				_camera = Camera.main;
			}

			transform.LookAt(transform.position + _camera.transform.rotation * Vector3.forward, _camera.transform.rotation * Vector3.up);
		}
	}
}