using UnityEngine;

namespace MoreMountains.Tools;

[RequireComponent(typeof(Camera))]
[AddComponentMenu("More Mountains/Tools/Camera/MMCameraAspectRatio")]
public class MMCameraAspectRatio : MonoBehaviour
{
	public Vector2 AspectRatio = Vector2.zero;

	protected Camera _camera;

	protected virtual void Start()
	{
		Initialization();
	}

	protected virtual void Initialization()
	{
		if (AspectRatio == Vector2.zero)
		{
			return;
		}
		_camera = base.gameObject.GetComponent<Camera>();
		if (!(_camera == null))
		{
			float num = AspectRatio.x / AspectRatio.y;
			float num2 = (float)Screen.width / (float)Screen.height / num;
			if (num2 >= 1f)
			{
				float num3 = 1f / num2;
				Rect rect = _camera.rect;
				rect.width = num3;
				rect.height = 1f;
				rect.x = (1f - num3) / 2f;
				rect.y = 0f;
				_camera.rect = rect;
			}
			else
			{
				Rect rect2 = _camera.rect;
				rect2.width = 1f;
				rect2.height = num2;
				rect2.x = 0f;
				rect2.y = (1f - num2) / 2f;
				_camera.rect = rect2;
			}
		}
	}
}
