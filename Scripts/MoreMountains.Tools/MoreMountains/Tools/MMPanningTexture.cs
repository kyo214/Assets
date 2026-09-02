using UnityEngine;
using UnityEngine.UI;

namespace MoreMountains.Tools;

[AddComponentMenu("More Mountains/Tools/VFX/PanningTexture")]
public class MMPanningTexture : MonoBehaviour
{
	[MMInformation("This script will let you pan a texture on an attached Renderer.", MMInformationAttribute.InformationType.Info, false)]
	public bool TextureShouldPan = true;

	public Vector2 Speed = new Vector2(10f, 10f);

	public string SortingLayerName = "Above";

	[Tooltip("the property name, for example _MainTex")]
	public string MaterialPropertyName = "_MainTex_ST";

	[Tooltip("the index of the material")]
	public int MaterialIndex;

	protected RawImage _rawImage;

	protected Renderer _renderer;

	protected Vector4 _position = Vector4.one;

	protected Vector4 _speed;

	protected MaterialPropertyBlock _propertyBlock;

	protected virtual void Start()
	{
		_renderer = GetComponent<Renderer>();
		if (_renderer != null && !string.IsNullOrEmpty(SortingLayerName))
		{
			_renderer.sortingLayerName = SortingLayerName;
			_propertyBlock = new MaterialPropertyBlock();
			_renderer.GetPropertyBlock(_propertyBlock);
		}
		_position.x = _renderer.sharedMaterials[MaterialIndex].GetVector(MaterialPropertyName).x;
		_position.y = _renderer.sharedMaterials[MaterialIndex].GetVector(MaterialPropertyName).y;
		_rawImage = GetComponent<RawImage>();
		_speed = new Vector4(0f, 0f, Speed.x, Speed.y);
	}

	protected virtual void Update()
	{
		if (TextureShouldPan && (!(_rawImage == null) || !(_renderer == null)))
		{
			_speed.z = Speed.x;
			_speed.w = Speed.y;
			_position += _speed / 300f * Time.deltaTime;
			if (_position.z > 1f)
			{
				_position.z--;
			}
			if (_position.w > 1f)
			{
				_position.w--;
			}
			if (_renderer != null)
			{
				_renderer.GetPropertyBlock(_propertyBlock);
				_propertyBlock.SetVector(MaterialPropertyName, _position);
				_renderer.SetPropertyBlock(_propertyBlock, MaterialIndex);
			}
			if (_rawImage != null)
			{
				_rawImage.material.mainTextureOffset = _position;
			}
		}
	}
}
