using UnityEngine;

namespace MoreMountains.Tools;

public class MMGizmo : MonoBehaviour
{
	public enum GizmoTypes
	{
		None = 0,
		Collider = 1,
		Position = 2
	}

	public enum DisplayModes
	{
		Always = 0,
		OnlyWhenSelected = 1
	}

	public enum PositionModes
	{
		Point = 0,
		Cube = 1,
		WireCube = 2,
		Sphere = 3,
		WireSphere = 4,
		Texture = 5,
		Arrows = 6,
		RightArrow = 7,
		UpArrow = 8,
		ForwardArrow = 9,
		Lines = 10,
		RightLine = 11,
		UpLine = 12,
		ForwardLine = 13
	}

	public enum TextModes
	{
		GameObjectName = 0,
		CustomText = 1,
		Position = 2,
		Rotation = 3,
		Scale = 4,
		Property = 5
	}

	public enum ColliderRenderTypes
	{
		Full = 0,
		Wire = 1
	}

	[Header("Modes")]
	[Tooltip("if this is true, gizmos will be displayed, if this is false, gizmos won't be displayed")]
	public bool DisplayGizmo = true;

	[Tooltip("what the gizmos should represent. Collider will show the bounds of the associated collider, Position will show the position of the object")]
	public GizmoTypes GizmoType = GizmoTypes.Position;

	[Tooltip("whether gizmos should always be displayed, or only when selected")]
	public DisplayModes DisplayMode;

	[Header("Settings")]
	[Tooltip("the color of the collider or position gizmo")]
	public Color GizmoColor = MMColors.ReunoYellow;

	[Tooltip("the shape of the gizmo when in position mode")]
	[MMEnumCondition("GizmoType", new int[] { 2 })]
	public PositionModes PositionMode;

	[Tooltip("the texture to display as a gizmo when in position & texture mode")]
	[MMEnumCondition("PositionMode", new int[] { 5 })]
	public Texture PositionTexture;

	[Tooltip("the size of the texture to display as a gizmo")]
	[MMEnumCondition("PositionMode", new int[] { 5 })]
	public Vector2 TextureSize = new Vector2(50f, 50f);

	[Tooltip("the size of the gizmo when in position mode")]
	[MMEnumCondition("GizmoType", new int[] { 2 })]
	public float PositionSize = 0.2f;

	[Tooltip("whether to display the collider gizmo as a wire or a full mesh")]
	[MMEnumCondition("GizmoType", new int[] { 1 })]
	public ColliderRenderTypes ColliderRenderType;

	[Tooltip("the distance from the scene view camera beyond which the gizmo won't be displayed")]
	public float ViewDistance = 20f;

	[Header("Offsets")]
	[Tooltip("an offset to apply when drawing a collider or position gizmo")]
	public Vector3 GizmoOffset = Vector3.zero;

	[Tooltip("whether or not to lock the position of the gizmo on the x axis, regardless of the position of the object")]
	public bool LockX;

	[Tooltip("the position at which to put the gizmo when locked on the x axis")]
	[MMCondition("LockX", true)]
	public float LockedX;

	[Tooltip("whether or not to lock the position of the gizmo on the y axis, regardless of the position of the object")]
	public bool LockY;

	[Tooltip("the position at which to put the gizmo when locked on the y axis")]
	[MMCondition("LockY", true)]
	public float LockedY;

	[Tooltip("whether or not to lock the position of the gizmo on the z axis, regardless of the position of the object")]
	public bool LockZ;

	[Tooltip("the position at which to put the gizmo when locked on the z axis")]
	[MMCondition("LockZ", true)]
	public float LockedZ;

	[Header("Text")]
	[Tooltip("whether or not to display text on that gizmo")]
	public bool DisplayText;

	[Tooltip("what to display as text for that gizmo (some custom text, the object's name, position, rotation, scale, or a target property)")]
	[MMCondition("DisplayText", true)]
	public TextModes TextMode;

	[Tooltip("when in CustomText mode, the text to display on that gizmo")]
	[MMEnumCondition("TextMode", new int[] { 1 })]
	public string TextToDisplay = "Some Text";

	[Tooltip("the offset to apply to the text")]
	[MMCondition("DisplayText", true)]
	public Vector3 TextOffset = new Vector3(0f, 0.5f, 0f);

	[Tooltip("what style to use for the text's font")]
	[MMCondition("DisplayText", true)]
	public FontStyle TextFontStyle;

	[Tooltip("the size of the text's font")]
	[MMCondition("DisplayText", true)]
	public int TextSize = 12;

	[Tooltip("the color in which to display the gizmo's text")]
	[MMCondition("DisplayText", true)]
	public Color TextColor = MMColors.ReunoYellow;

	[Tooltip("the color of the background behind the text")]
	[MMCondition("DisplayText", true)]
	public Color TextBackgroundColor = new Color(0f, 0f, 0f, 0.3f);

	[Tooltip("the padding to apply to the text's background")]
	[MMCondition("DisplayText", true)]
	public Vector4 TextPadding = new Vector4(5f, 0f, 5f, 0f);

	[Tooltip("the distance from the scene view camera beyond which the gizmo text won't be displayed")]
	[MMCondition("DisplayText", true)]
	public float TextMaxDistance = 14f;

	[Tooltip("when in Property mode, the property whose value to display on the gizmo")]
	public MMPropertyPicker TargetProperty;

	public bool Initialized { get; set; }

	public SphereCollider _sphereCollider { get; set; }

	public BoxCollider _boxCollider { get; set; }

	public MeshCollider _meshCollider { get; set; }

	public CircleCollider2D _circleCollider2D { get; set; }

	public BoxCollider2D _boxCollider2D { get; set; }

	public Vector3 _vector3Zero { get; set; }

	public Vector3 _newPosition { get; set; }

	public Vector2 _worldToGUIPosition { get; set; }

	public Rect _textureRect { get; set; }

	public GUIStyle _textGUIStyle { get; set; }

	public string _textToDisplay { get; set; }

	public bool _sphereColliderNotNull { get; set; }

	public bool _boxColliderNotNull { get; set; }

	public bool _meshColliderNotNull { get; set; }

	public bool _circleCollider2DNotNull { get; set; }

	public bool _boxCollider2DNotNull { get; set; }

	public bool _positionTextureNotNull { get; set; }

	protected virtual void Awake()
	{
		base.enabled = false;
	}
}
