namespace UnityEngine.Rendering.Universal;

public class DebugDisplaySettingsMaterial : IDebugDisplaySettingsData, IDebugDisplaySettingsQuery
{
	public enum AlbedoDebugValidationPreset
	{
		DefaultLuminance = 0,
		BlackAcrylicPaint = 1,
		DarkSoil = 2,
		WornAsphalt = 3,
		DryClaySoil = 4,
		GreenGrass = 5,
		OldConcrete = 6,
		RedClayTile = 7,
		DrySand = 8,
		NewConcrete = 9,
		WhiteAcrylicPaint = 10,
		FreshSnow = 11,
		BlueSky = 12,
		Foliage = 13,
		Custom = 14
	}

	private struct AlbedoDebugValidationPresetData
	{
		public string name;

		public Color color;

		public float minLuminance;

		public float maxLuminance;
	}

	private static class Strings
	{
		public const string AlbedoSettingsContainerName = "Albedo Settings";

		public const string MetallicSettingsContainerName = "Metallic Settings";

		public static readonly DebugUI.Widget.NameAndTooltip MaterialOverride = new DebugUI.Widget.NameAndTooltip
		{
			name = "Material Override",
			tooltip = "Use the drop-down to select a Material property to visualize on every GameObject on screen."
		};

		public static readonly DebugUI.Widget.NameAndTooltip VertexAttribute = new DebugUI.Widget.NameAndTooltip
		{
			name = "Vertex Attribute",
			tooltip = "Use the drop-down to select a 3D GameObject attribute, like Texture Coordinates or Vertex Color, to visualize on screen."
		};

		public static readonly DebugUI.Widget.NameAndTooltip MaterialValidationMode = new DebugUI.Widget.NameAndTooltip
		{
			name = "Material Validation Mode",
			tooltip = "Debug and validate material properties."
		};

		public static readonly DebugUI.Widget.NameAndTooltip ValidationPreset = new DebugUI.Widget.NameAndTooltip
		{
			name = "Validation Preset",
			tooltip = "Validate using a list of preset surfaces and inputs based on real-world surfaces."
		};

		public static readonly DebugUI.Widget.NameAndTooltip AlbedoCustomColor = new DebugUI.Widget.NameAndTooltip
		{
			name = "Target Color",
			tooltip = "Custom target color for albedo validation."
		};

		public static readonly DebugUI.Widget.NameAndTooltip AlbedoMinLuminance = new DebugUI.Widget.NameAndTooltip
		{
			name = "Min Luminance",
			tooltip = "Any values set below this field are invalid and appear red on screen."
		};

		public static readonly DebugUI.Widget.NameAndTooltip AlbedoMaxLuminance = new DebugUI.Widget.NameAndTooltip
		{
			name = "Max Luminance",
			tooltip = "Any values set above this field are invalid and appear blue on screen."
		};

		public static readonly DebugUI.Widget.NameAndTooltip AlbedoHueTolerance = new DebugUI.Widget.NameAndTooltip
		{
			name = "Hue Tolerance",
			tooltip = "Validate a material based on a specific hue."
		};

		public static readonly DebugUI.Widget.NameAndTooltip AlbedoSaturationTolerance = new DebugUI.Widget.NameAndTooltip
		{
			name = "Saturation Tolerance",
			tooltip = "Validate a material based on a specific Saturation."
		};

		public static readonly DebugUI.Widget.NameAndTooltip MetallicMinValue = new DebugUI.Widget.NameAndTooltip
		{
			name = "Min Value",
			tooltip = "Any values set below this field are invalid and appear red on screen."
		};

		public static readonly DebugUI.Widget.NameAndTooltip MetallicMaxValue = new DebugUI.Widget.NameAndTooltip
		{
			name = "Max Value",
			tooltip = "Any values set above this field are invalid and appear blue on screen."
		};
	}

	internal static class WidgetFactory
	{
		internal static DebugUI.Widget CreateMaterialOverride(SettingsPanel panel)
		{
			return new DebugUI.EnumField
			{
				nameAndTooltip = Strings.MaterialOverride,
				autoEnum = typeof(DebugMaterialMode),
				getter = () => (int)panel.data.materialDebugMode,
				setter = (int value) =>
				{
					panel.data.materialDebugMode = (DebugMaterialMode)value;
				},
				getIndex = () => (int)panel.data.materialDebugMode,
				setIndex = (int value) =>
				{
					panel.data.materialDebugMode = (DebugMaterialMode)value;
				}
			};
		}

		internal static DebugUI.Widget CreateVertexAttribute(SettingsPanel panel)
		{
			return new DebugUI.EnumField
			{
				nameAndTooltip = Strings.VertexAttribute,
				autoEnum = typeof(DebugVertexAttributeMode),
				getter = () => (int)panel.data.vertexAttributeDebugMode,
				setter = (int value) =>
				{
					panel.data.vertexAttributeDebugMode = (DebugVertexAttributeMode)value;
				},
				getIndex = () => (int)panel.data.vertexAttributeDebugMode,
				setIndex = (int value) =>
				{
					panel.data.vertexAttributeDebugMode = (DebugVertexAttributeMode)value;
				}
			};
		}

		internal static DebugUI.Widget CreateMaterialValidationMode(SettingsPanel panel)
		{
			return new DebugUI.EnumField
			{
				nameAndTooltip = Strings.MaterialValidationMode,
				autoEnum = typeof(DebugMaterialValidationMode),
				getter = () => (int)panel.data.materialValidationMode,
				setter = (int value) =>
				{
					panel.data.materialValidationMode = (DebugMaterialValidationMode)value;
				},
				getIndex = () => (int)panel.data.materialValidationMode,
				setIndex = (int value) =>
				{
					panel.data.materialValidationMode = (DebugMaterialValidationMode)value;
				},
				onValueChanged = (DebugUI.Field<int> _, int _) =>
				{
					DebugManager.instance.ReDrawOnScreenDebug();
				}
			};
		}

		internal static DebugUI.Widget CreateAlbedoPreset(SettingsPanel panel)
		{
			return new DebugUI.EnumField
			{
				nameAndTooltip = Strings.ValidationPreset,
				autoEnum = typeof(AlbedoDebugValidationPreset),
				getter = () => (int)panel.data.albedoValidationPreset,
				setter = (int value) =>
				{
					panel.data.albedoValidationPreset = (AlbedoDebugValidationPreset)value;
				},
				getIndex = () => (int)panel.data.albedoValidationPreset,
				setIndex = (int value) =>
				{
					panel.data.albedoValidationPreset = (AlbedoDebugValidationPreset)value;
				},
				onValueChanged = (DebugUI.Field<int> _, int _) =>
				{
					DebugManager.instance.ReDrawOnScreenDebug();
				}
			};
		}

		internal static DebugUI.Widget CreateAlbedoCustomColor(SettingsPanel panel)
		{
			return new DebugUI.ColorField
			{
				nameAndTooltip = Strings.AlbedoCustomColor,
				getter = () => panel.data.albedoCompareColor,
				setter = (Color value) =>
				{
					panel.data.albedoCompareColor = value;
				},
				isHiddenCallback = () => panel.data.albedoValidationPreset != AlbedoDebugValidationPreset.Custom
			};
		}

		internal static DebugUI.Widget CreateAlbedoMinLuminance(SettingsPanel panel)
		{
			return new DebugUI.FloatField
			{
				nameAndTooltip = Strings.AlbedoMinLuminance,
				getter = () => panel.data.albedoMinLuminance,
				setter = (float value) =>
				{
					panel.data.albedoMinLuminance = value;
				},
				incStep = 0.01f
			};
		}

		internal static DebugUI.Widget CreateAlbedoMaxLuminance(SettingsPanel panel)
		{
			return new DebugUI.FloatField
			{
				nameAndTooltip = Strings.AlbedoMaxLuminance,
				getter = () => panel.data.albedoMaxLuminance,
				setter = (float value) =>
				{
					panel.data.albedoMaxLuminance = value;
				},
				incStep = 0.01f
			};
		}

		internal static DebugUI.Widget CreateAlbedoHueTolerance(SettingsPanel panel)
		{
			return new DebugUI.FloatField
			{
				nameAndTooltip = Strings.AlbedoHueTolerance,
				getter = () => panel.data.albedoHueTolerance,
				setter = (float value) =>
				{
					panel.data.albedoHueTolerance = value;
				},
				incStep = 0.01f,
				isHiddenCallback = () => panel.data.albedoValidationPreset == AlbedoDebugValidationPreset.DefaultLuminance
			};
		}

		internal static DebugUI.Widget CreateAlbedoSaturationTolerance(SettingsPanel panel)
		{
			return new DebugUI.FloatField
			{
				nameAndTooltip = Strings.AlbedoSaturationTolerance,
				getter = () => panel.data.albedoSaturationTolerance,
				setter = (float value) =>
				{
					panel.data.albedoSaturationTolerance = value;
				},
				incStep = 0.01f,
				isHiddenCallback = () => panel.data.albedoValidationPreset == AlbedoDebugValidationPreset.DefaultLuminance
			};
		}

		internal static DebugUI.Widget CreateMetallicMinValue(SettingsPanel panel)
		{
			return new DebugUI.FloatField
			{
				nameAndTooltip = Strings.MetallicMinValue,
				getter = () => panel.data.metallicMinValue,
				setter = (float value) =>
				{
					panel.data.metallicMinValue = value;
				},
				incStep = 0.01f
			};
		}

		internal static DebugUI.Widget CreateMetallicMaxValue(SettingsPanel panel)
		{
			return new DebugUI.FloatField
			{
				nameAndTooltip = Strings.MetallicMaxValue,
				getter = () => panel.data.metallicMaxValue,
				setter = (float value) =>
				{
					panel.data.metallicMaxValue = value;
				},
				incStep = 0.01f
			};
		}
	}

	[DisplayInfo(name = "Material", order = 2)]
	internal class SettingsPanel : DebugDisplaySettingsPanel<DebugDisplaySettingsMaterial>
	{
		public SettingsPanel(DebugDisplaySettingsMaterial data)
			: base(data)
		{
			AddWidget(DebugDisplaySettingsCommon.WidgetFactory.CreateMissingDebugShadersWarning());
			AddWidget(new DebugUI.Foldout
			{
				displayName = "Material Filters",
				flags = DebugUI.Flags.FrequentlyUsed,
				isHeader = true,
				opened = true,
				children = 
				{
					WidgetFactory.CreateMaterialOverride(this),
					WidgetFactory.CreateVertexAttribute(this)
				}
			});
			AddWidget(new DebugUI.Foldout
			{
				displayName = "Material Validation",
				isHeader = true,
				opened = true,
				children = 
				{
					WidgetFactory.CreateMaterialValidationMode(this),
					(DebugUI.Widget)new DebugUI.Container
					{
						displayName = "Albedo Settings",
						isHiddenCallback = () => data.materialValidationMode != DebugMaterialValidationMode.Albedo,
						children = 
						{
							WidgetFactory.CreateAlbedoPreset(this),
							WidgetFactory.CreateAlbedoCustomColor(this),
							WidgetFactory.CreateAlbedoMinLuminance(this),
							WidgetFactory.CreateAlbedoMaxLuminance(this),
							WidgetFactory.CreateAlbedoHueTolerance(this),
							WidgetFactory.CreateAlbedoSaturationTolerance(this)
						}
					},
					(DebugUI.Widget)new DebugUI.Container
					{
						displayName = "Metallic Settings",
						isHiddenCallback = () => data.materialValidationMode != DebugMaterialValidationMode.Metallic,
						children = 
						{
							WidgetFactory.CreateMetallicMinValue(this),
							WidgetFactory.CreateMetallicMaxValue(this)
						}
					}
				}
			});
		}
	}

	private AlbedoDebugValidationPresetData[] m_AlbedoDebugValidationPresetData = new AlbedoDebugValidationPresetData[15]
	{
		new AlbedoDebugValidationPresetData
		{
			name = "Default Luminance",
			color = new Color(127f / 255f, 127f / 255f, 127f / 255f),
			minLuminance = 0.01f,
			maxLuminance = 0.9f
		},
		new AlbedoDebugValidationPresetData
		{
			name = "Black Acrylic Paint",
			color = new Color(56f / 255f, 56f / 255f, 56f / 255f),
			minLuminance = 0.03f,
			maxLuminance = 0.07f
		},
		new AlbedoDebugValidationPresetData
		{
			name = "Dark Soil",
			color = new Color(1f / 3f, 61f / 255f, 49f / 255f),
			minLuminance = 0.05f,
			maxLuminance = 0.14f
		},
		new AlbedoDebugValidationPresetData
		{
			name = "Worn Asphalt",
			color = new Color(91f / 255f, 91f / 255f, 91f / 255f),
			minLuminance = 0.1f,
			maxLuminance = 0.15f
		},
		new AlbedoDebugValidationPresetData
		{
			name = "Dry Clay Soil",
			color = new Color(137f / 255f, 120f / 255f, 0.4f),
			minLuminance = 0.15f,
			maxLuminance = 0.35f
		},
		new AlbedoDebugValidationPresetData
		{
			name = "Green Grass",
			color = new Color(123f / 255f, 131f / 255f, 74f / 255f),
			minLuminance = 0.16f,
			maxLuminance = 0.26f
		},
		new AlbedoDebugValidationPresetData
		{
			name = "Old Concrete",
			color = new Color(135f / 255f, 8f / 15f, 131f / 255f),
			minLuminance = 0.17f,
			maxLuminance = 0.3f
		},
		new AlbedoDebugValidationPresetData
		{
			name = "Red Clay Tile",
			color = new Color(197f / 255f, 125f / 255f, 100f / 255f),
			minLuminance = 0.23f,
			maxLuminance = 0.33f
		},
		new AlbedoDebugValidationPresetData
		{
			name = "Dry Sand",
			color = new Color(177f / 255f, 167f / 255f, 132f / 255f),
			minLuminance = 0.2f,
			maxLuminance = 0.45f
		},
		new AlbedoDebugValidationPresetData
		{
			name = "New Concrete",
			color = new Color(185f / 255f, 182f / 255f, 175f / 255f),
			minLuminance = 0.32f,
			maxLuminance = 0.55f
		},
		new AlbedoDebugValidationPresetData
		{
			name = "White Acrylic Paint",
			color = new Color(227f / 255f, 227f / 255f, 227f / 255f),
			minLuminance = 0.75f,
			maxLuminance = 0.85f
		},
		new AlbedoDebugValidationPresetData
		{
			name = "Fresh Snow",
			color = new Color(243f / 255f, 243f / 255f, 243f / 255f),
			minLuminance = 0.85f,
			maxLuminance = 0.95f
		},
		new AlbedoDebugValidationPresetData
		{
			name = "Blue Sky",
			color = new Color(93f / 255f, 123f / 255f, 157f / 255f),
			minLuminance = new Color(93f / 255f, 123f / 255f, 157f / 255f).linear.maxColorComponent - 0.05f,
			maxLuminance = new Color(93f / 255f, 123f / 255f, 157f / 255f).linear.maxColorComponent + 0.05f
		},
		new AlbedoDebugValidationPresetData
		{
			name = "Foliage",
			color = new Color(91f / 255f, 108f / 255f, 65f / 255f),
			minLuminance = new Color(91f / 255f, 108f / 255f, 65f / 255f).linear.maxColorComponent - 0.05f,
			maxLuminance = new Color(91f / 255f, 108f / 255f, 65f / 255f).linear.maxColorComponent + 0.05f
		},
		new AlbedoDebugValidationPresetData
		{
			name = "Custom",
			color = new Color(127f / 255f, 127f / 255f, 127f / 255f),
			minLuminance = 0.01f,
			maxLuminance = 0.9f
		}
	};

	private AlbedoDebugValidationPreset m_AlbedoValidationPreset;

	private float m_AlbedoHueTolerance = 0.104f;

	private float m_AlbedoSaturationTolerance = 0.214f;

	public AlbedoDebugValidationPreset albedoValidationPreset
	{
		get
		{
			return m_AlbedoValidationPreset;
		}
		set
		{
			m_AlbedoValidationPreset = value;
			AlbedoDebugValidationPresetData albedoDebugValidationPresetData = m_AlbedoDebugValidationPresetData[(int)value];
			albedoMinLuminance = albedoDebugValidationPresetData.minLuminance;
			albedoMaxLuminance = albedoDebugValidationPresetData.maxLuminance;
			albedoCompareColor = albedoDebugValidationPresetData.color;
		}
	}

	public float albedoMinLuminance { get; set; } = 0.01f;

	public float albedoMaxLuminance { get; set; } = 0.9f;

	public float albedoHueTolerance
	{
		get
		{
			if (m_AlbedoValidationPreset != AlbedoDebugValidationPreset.DefaultLuminance)
			{
				return m_AlbedoHueTolerance;
			}
			return 1f;
		}
		set
		{
			m_AlbedoHueTolerance = value;
		}
	}

	public float albedoSaturationTolerance
	{
		get
		{
			if (m_AlbedoValidationPreset != AlbedoDebugValidationPreset.DefaultLuminance)
			{
				return m_AlbedoSaturationTolerance;
			}
			return 1f;
		}
		set
		{
			m_AlbedoSaturationTolerance = value;
		}
	}

	public Color albedoCompareColor { get; set; } = new Color(127f / 255f, 127f / 255f, 127f / 255f, 1f);

	public float metallicMinValue { get; set; }

	public float metallicMaxValue { get; set; } = 0.9f;

	public DebugMaterialValidationMode materialValidationMode { get; set; }

	public DebugMaterialMode materialDebugMode { get; set; }

	public DebugVertexAttributeMode vertexAttributeDebugMode { get; set; }

	public bool AreAnySettingsActive
	{
		get
		{
			if (materialDebugMode == DebugMaterialMode.None && vertexAttributeDebugMode == DebugVertexAttributeMode.None)
			{
				return materialValidationMode != DebugMaterialValidationMode.None;
			}
			return true;
		}
	}

	public bool IsPostProcessingAllowed => !AreAnySettingsActive;

	public bool IsLightingActive => !AreAnySettingsActive;

	public bool TryGetScreenClearColor(ref Color color)
	{
		return false;
	}

	IDebugDisplaySettingsPanelDisposable IDebugDisplaySettingsData.CreatePanel()
	{
		return new SettingsPanel(this);
	}
}
