using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace StylizedWater;

[ExecuteAlways]
[DisallowMultipleComponent]
[AddComponentMenu("Effects/Planar Reflections")]
[HelpURL("https://alexander-ameye.gitbook.io/stylized-water/")]
public class PlanarReflections : MonoBehaviour
{
	private class PlanarReflectionSettingData
	{
		private readonly bool fog;

		private readonly int maximumLODLevel;

		private readonly float lodBias;

		public PlanarReflectionSettingData()
		{
			fog = RenderSettings.fog;
			maximumLODLevel = QualitySettings.maximumLODLevel;
			lodBias = QualitySettings.lodBias;
		}

		public void Set()
		{
			GL.invertCulling = true;
			RenderSettings.fog = false;
			QualitySettings.maximumLODLevel = 1;
			QualitySettings.lodBias = lodBias * 0.5f;
		}

		public void Restore()
		{
			GL.invertCulling = false;
			RenderSettings.fog = fog;
			QualitySettings.maximumLODLevel = maximumLODLevel;
			QualitySettings.lodBias = lodBias;
		}
	}

	[Range(0.01f, 1f)]
	public float renderScale = 1f;

	public LayerMask reflectionLayer = -1;

	public bool reflectSkybox;

	public GameObject reflectionTarget;

	[Range(-2f, 3f)]
	public float reflectionPlaneOffset;

	private static Camera _reflectionCamera;

	private UniversalAdditionalCameraData cameraData;

	private static RenderTexture _reflectionTexture;

	private RenderTextureDescriptor previousDescriptor;

	private readonly int _planarReflectionTextureId = Shader.PropertyToID("_PlanarReflectionTexture");

	public bool hideReflectionCamera;

	public static event Action<ScriptableRenderContext, Camera> BeginPlanarReflections;

	private void OnEnable()
	{
		RenderPipelineManager.beginCameraRendering += DoPlanarReflections;
		reflectionLayer = -17;
	}

	private void OnDisable()
	{
		CleanUp();
		RenderPipelineManager.beginCameraRendering -= DoPlanarReflections;
	}

	private void OnDestroy()
	{
		CleanUp();
		RenderPipelineManager.beginCameraRendering -= DoPlanarReflections;
	}

	private void CleanUp()
	{
		if ((bool)_reflectionCamera)
		{
			_reflectionCamera.targetTexture = null;
			SafeDestroyObject(_reflectionCamera.gameObject);
		}
		if ((bool)_reflectionTexture)
		{
			RenderTexture.ReleaseTemporary(_reflectionTexture);
		}
	}

	private void SafeDestroyObject(UnityEngine.Object obj)
	{
		if (Application.isEditor)
		{
			UnityEngine.Object.DestroyImmediate(obj);
		}
		else
		{
			UnityEngine.Object.Destroy(obj);
		}
	}

	private void UpdateReflectionCamera(Camera realCamera)
	{
		if (_reflectionCamera == null)
		{
			_reflectionCamera = InitializeReflectionCamera();
		}
		Vector3 vector = Vector3.zero;
		Vector3 up = Vector3.up;
		if (reflectionTarget != null)
		{
			vector = reflectionTarget.transform.position + Vector3.up * reflectionPlaneOffset;
			up = reflectionTarget.transform.up;
		}
		UpdateCamera(realCamera, _reflectionCamera);
		_reflectionCamera.gameObject.hideFlags = (hideReflectionCamera ? HideFlags.HideAndDontSave : HideFlags.DontSave);
		float w = 0f - Vector3.Dot(up, vector);
		Vector4 plane = new Vector4(up.x, up.y, up.z, w);
		Matrix4x4 reflectionMatrix = Matrix4x4.identity;
		reflectionMatrix *= Matrix4x4.Scale(new Vector3(1f, -1f, 1f));
		CalculateReflectionMatrix(ref reflectionMatrix, plane);
		Vector3 position = ReflectPosition(realCamera.transform.position - new Vector3(0f, vector.y * 2f, 0f));
		_reflectionCamera.transform.forward = Vector3.Scale(realCamera.transform.forward, new Vector3(1f, -1f, 1f));
		_reflectionCamera.worldToCameraMatrix = realCamera.worldToCameraMatrix * reflectionMatrix;
		Vector4 clipPlane = CameraSpacePlane(_reflectionCamera, vector - Vector3.up * 0.1f, up, 1f);
		Matrix4x4 projectionMatrix = realCamera.CalculateObliqueMatrix(clipPlane);
		_reflectionCamera.projectionMatrix = projectionMatrix;
		_reflectionCamera.cullingMask = reflectionLayer;
		_reflectionCamera.transform.position = position;
		_reflectionCamera.useOcclusionCulling = true;
	}

	private void UpdateCamera(Camera src, Camera dest)
	{
		if (dest == null)
		{
			return;
		}
		dest.CopyFrom(src);
		dest.useOcclusionCulling = false;
		if (dest.gameObject.TryGetComponent<UniversalAdditionalCameraData>(out var component))
		{
			component.renderShadows = false;
			if (reflectSkybox)
			{
				dest.clearFlags = CameraClearFlags.Skybox;
				return;
			}
			dest.clearFlags = CameraClearFlags.Color;
			dest.backgroundColor = Color.black;
		}
	}

	private Camera InitializeReflectionCamera()
	{
		GameObject gameObject = new GameObject("", typeof(Camera));
		gameObject.name = "Reflection Camera [" + gameObject.GetInstanceID() + "]";
		UniversalAdditionalCameraData obj = gameObject.AddComponent(typeof(UniversalAdditionalCameraData)) as UniversalAdditionalCameraData;
		obj.requiresColorOption = CameraOverrideOption.Off;
		obj.requiresDepthOption = CameraOverrideOption.Off;
		obj.SetRenderer(0);
		Transform transform = base.transform;
		Camera component = gameObject.GetComponent<Camera>();
		component.transform.SetPositionAndRotation(transform.position, transform.rotation);
		component.depth = -10f;
		component.enabled = false;
		return component;
	}

	private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
	{
		Matrix4x4 worldToCameraMatrix = cam.worldToCameraMatrix;
		Vector3 lhs = worldToCameraMatrix.MultiplyPoint(pos);
		Vector3 rhs = worldToCameraMatrix.MultiplyVector(normal).normalized * sideSign;
		return new Vector4(rhs.x, rhs.y, rhs.z, 0f - Vector3.Dot(lhs, rhs));
	}

	private RenderTextureDescriptor GetDescriptor(Camera camera, float pipelineRenderScale)
	{
		int width = (int)Mathf.Max((float)camera.pixelWidth * pipelineRenderScale * renderScale);
		int height = (int)Mathf.Max((float)camera.pixelHeight * pipelineRenderScale * renderScale);
		RenderTextureFormat colorFormat = (camera.allowHDR ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default);
		RenderTextureDescriptor result = new RenderTextureDescriptor(width, height, colorFormat, 16);
		result.autoGenerateMips = true;
		result.useMipMap = true;
		return result;
	}

	private void CreateReflectionTexture(Camera camera)
	{
		RenderTextureDescriptor descriptor = GetDescriptor(camera, UniversalRenderPipeline.asset.renderScale);
		if (_reflectionTexture == null)
		{
			_reflectionTexture = RenderTexture.GetTemporary(descriptor);
			previousDescriptor = descriptor;
		}
		else if (!descriptor.Equals(previousDescriptor))
		{
			if ((bool)_reflectionTexture)
			{
				RenderTexture.ReleaseTemporary(_reflectionTexture);
			}
			_reflectionTexture = RenderTexture.GetTemporary(descriptor);
			previousDescriptor = descriptor;
		}
		_reflectionCamera.targetTexture = _reflectionTexture;
	}

	private void DoPlanarReflections(ScriptableRenderContext context, Camera camera)
	{
		if (camera.cameraType != CameraType.Reflection && camera.cameraType != CameraType.Preview && (bool)reflectionTarget)
		{
			UpdateReflectionCamera(camera);
			CreateReflectionTexture(camera);
			PlanarReflectionSettingData planarReflectionSettingData = new PlanarReflectionSettingData();
			planarReflectionSettingData.Set();
			BeginPlanarReflections?.Invoke(context, _reflectionCamera);
			if (_reflectionCamera.WorldToViewportPoint(reflectionTarget.transform.position).z < 100000f)
			{
				UniversalRenderPipeline.RenderSingleCamera(context, _reflectionCamera);
			}
			planarReflectionSettingData.Restore();
			Shader.SetGlobalTexture(_planarReflectionTextureId, _reflectionTexture);
		}
	}

	public static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMatrix, Vector4 plane)
	{
		reflectionMatrix.m00 = 1f - 2f * plane[0] * plane[0];
		reflectionMatrix.m01 = -2f * plane[0] * plane[1];
		reflectionMatrix.m02 = -2f * plane[0] * plane[2];
		reflectionMatrix.m03 = -2f * plane[3] * plane[0];
		reflectionMatrix.m10 = -2f * plane[1] * plane[0];
		reflectionMatrix.m11 = 1f - 2f * plane[1] * plane[1];
		reflectionMatrix.m12 = -2f * plane[1] * plane[2];
		reflectionMatrix.m13 = -2f * plane[3] * plane[1];
		reflectionMatrix.m20 = -2f * plane[2] * plane[0];
		reflectionMatrix.m21 = -2f * plane[2] * plane[1];
		reflectionMatrix.m22 = 1f - 2f * plane[2] * plane[2];
		reflectionMatrix.m23 = -2f * plane[3] * plane[2];
		reflectionMatrix.m30 = 0f;
		reflectionMatrix.m31 = 0f;
		reflectionMatrix.m32 = 0f;
		reflectionMatrix.m33 = 1f;
	}

	public static Vector3 ReflectPosition(Vector3 pos)
	{
		return new Vector3(pos.x, 0f - pos.y, pos.z);
	}

	public static bool Compare(Vector2 a, Vector2 b)
	{
		if (a.x == b.x)
		{
			return a.y == b.y;
		}
		return false;
	}
}
