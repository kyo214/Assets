using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Modules.UIInGame.Scripts;

public class CompassControllerUI : MonoBehaviour
{
	[SerializeField]
	private Image _image;

	private CameraGame _cameraGame;

	private void Start()
	{
		if ((bool)CameraGame.Instance)
		{
			OnCameraRotateEvent(CameraGame.Instance.camRotate);
			CameraGame.Instance.OnCameraRotateEvent += OnCameraRotateEvent;
		}
	}

	private void OnEnable()
	{
		if ((bool)CameraGame.Instance)
		{
			OnCameraRotateEvent(CameraGame.Instance.camRotate);
		}
	}

	private void OnDestroy()
	{
		if ((bool)CameraGame.Instance)
		{
			CameraGame.Instance.OnCameraRotateEvent -= OnCameraRotateEvent;
		}
	}

	private void OnCameraRotateEvent(int angle)
	{
		if (GlobalSaveData.instance.optionData.autoMinimap == 0)
		{
			_image.rectTransform.DOLocalRotate(new Vector3(0f, 0f, 0f), 0.3f);
			for (int i = 0; i < _image.transform.childCount; i++)
			{
				_image.transform.GetChild(i).transform.DOLocalRotate(new Vector3(0f, 0f, 0f), 0f);
			}
		}
		else
		{
			_image.rectTransform.DOLocalRotate(new Vector3(0f, 0f, angle), 0.3f);
			for (int j = 0; j < _image.transform.childCount; j++)
			{
				_image.transform.GetChild(j).transform.DOLocalRotate(new Vector3(0f, 0f, -angle), 0f);
			}
		}
	}
}
