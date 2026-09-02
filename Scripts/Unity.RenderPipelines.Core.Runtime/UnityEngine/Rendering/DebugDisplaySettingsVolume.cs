using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.Rendering;

public class DebugDisplaySettingsVolume : IDebugDisplaySettingsData, IDebugDisplaySettingsQuery
{
	private static class Styles
	{
		public static readonly GUIContent none = new GUIContent("None");

		public static readonly GUIContent editorCamera = new GUIContent("Editor Camera");
	}

	private static class Strings
	{
		public static readonly string none = "None";

		public static readonly string camera = "Camera";

		public static readonly string parameter = "Parameter";

		public static readonly string component = "Component";

		public static readonly string debugViewNotSupported = "N/A";

		public static readonly string parameterNotOverrided = "-";

		public static readonly string volumeInfo = "Volume Info";

		public static readonly string gameObject = "GameObject";

		public static readonly string resultValue = "Result";

		public static readonly string resultValueTooltip = "The interpolated result value of the parameter. This value is used to render the camera.";

		public static readonly string globalDefaultValue = "Graphics Settings";

		public static readonly string globalDefaultValueTooltip = "Default value for this parameter, defined by the Default Volume Profile in Global Settings.";

		public static readonly string qualityLevelValue = "Quality Settings";

		public static readonly string qualityLevelValueTooltip = "Override value for this parameter, defined by the Volume Profile in the current SRP Asset.";

		public static readonly string global = "Global";

		public static readonly string local = "Local";

		public static readonly string volumeProfile = "Volume Profile";
	}

	internal static class WidgetFactory
	{
		private struct VolumeParameterChain
		{
			public DebugUI.Widget.NameAndTooltip nameAndTooltip;

			public VolumeProfile volumeProfile;

			public VolumeComponent volumeComponent;

			public Volume volume;
		}

		private static DebugUI.Value s_EmptyDebugUIValue = new DebugUI.Value
		{
			getter = () => string.Empty
		};

		public static DebugUI.EnumField CreateComponentSelector(SettingsPanel panel, Action<DebugUI.Field<int>, int> refresh)
		{
			int num = 0;
			List<GUIContent> list = new List<GUIContent> { Styles.none };
			List<int> list2 = new List<int> { num++ };
			foreach (var item in panel.data.volumeDebugSettings.volumeComponentsPathAndType)
			{
				GUIContent gUIContent = new GUIContent();
				(gUIContent.text, _) = item;
				list.Add(gUIContent);
				list2.Add(num++);
			}
			return new DebugUI.EnumField
			{
				displayName = Strings.component,
				getter = () => panel.data.volumeDebugSettings.selectedComponent,
				setter = (int value) =>
				{
					panel.data.volumeDebugSettings.selectedComponent = value;
				},
				enumNames = list.ToArray(),
				enumValues = list2.ToArray(),
				getIndex = () => panel.data.volumeComponentEnumIndex,
				setIndex = (int value) =>
				{
					panel.data.volumeComponentEnumIndex = value;
				},
				onValueChanged = refresh
			};
		}

		public static DebugUI.ObjectPopupField CreateCameraSelector(SettingsPanel panel, Action<DebugUI.Field<Object>, Object> refresh)
		{
			return new DebugUI.ObjectPopupField
			{
				displayName = Strings.camera,
				getter = () => panel.data.volumeDebugSettings.selectedCamera,
				setter = (Object value) =>
				{
					Camera[] array = panel.data.volumeDebugSettings.cameras.ToArray();
					panel.data.volumeDebugSettings.selectedCameraIndex = Array.IndexOf(array, value as Camera);
				},
				getObjects = () => panel.data.volumeDebugSettings.cameras,
				onValueChanged = refresh
			};
		}

		private static DebugUI.Widget CreateVolumeParameterWidget(string name, bool isResultParameter, VolumeParameter param, Func<bool> isHiddenCallback = null)
		{
			return new DebugUI.Value();
		}

		private static VolumeComponent GetSelectedVolumeComponent(VolumeProfile profile, Type selectedType)
		{
			if (profile != null)
			{
				foreach (VolumeComponent component in profile.components)
				{
					if (component.GetType() == selectedType)
					{
						return component;
					}
				}
			}
			return null;
		}

		private static List<VolumeParameterChain> GetResolutionChain(DebugDisplaySettingsVolume data)
		{
			List<VolumeParameterChain> list = new List<VolumeParameterChain>();
			Type selectedComponentType = data.volumeDebugSettings.selectedComponentType;
			if (selectedComponentType == null)
			{
				return list;
			}
			VolumeManager instance = VolumeManager.instance;
			VolumeComponent component = (data.volumeDebugSettings.selectedCameraVolumeStack ?? instance.stack).GetComponent(selectedComponentType);
			if (component == null)
			{
				return list;
			}
			VolumeParameterChain item = new VolumeParameterChain
			{
				nameAndTooltip = new DebugUI.Widget.NameAndTooltip
				{
					name = Strings.resultValue,
					tooltip = Strings.resultValueTooltip
				},
				volumeComponent = component
			};
			list.Add(item);
			Volume[] volumes = data.volumeDebugSettings.GetVolumes();
			foreach (Volume volume in volumes)
			{
				VolumeProfile volumeProfile = (volume.HasInstantiatedProfile() ? volume.profile : volume.sharedProfile);
				VolumeComponent selectedVolumeComponent = GetSelectedVolumeComponent(volumeProfile, selectedComponentType);
				if (selectedVolumeComponent != null)
				{
					VolumeParameterChain item2 = new VolumeParameterChain
					{
						nameAndTooltip = new DebugUI.Widget.NameAndTooltip
						{
							name = volumeProfile.name,
							tooltip = volumeProfile.name
						},
						volumeProfile = volumeProfile,
						volumeComponent = selectedVolumeComponent,
						volume = volume
					};
					list.Add(item2);
				}
			}
			return list;
		}

		public static DebugUI.Table CreateVolumeTable(DebugDisplaySettingsVolume data)
		{
			DebugUI.Table table = new DebugUI.Table
			{
				displayName = Strings.parameter,
				isReadOnly = true
			};
			List<VolumeParameterChain> resolutionChain = GetResolutionChain(data);
			if (resolutionChain.Count == 0)
			{
				return table;
			}
			GenerateTableRows(table, resolutionChain);
			GenerateTableColumns(table, data, resolutionChain);
			float timer = 0f;
			float refreshRate = 0.2f;
			Volume[] volumes = data.volumeDebugSettings.GetVolumes();
			table.isHiddenCallback = () =>
			{
				timer += Time.deltaTime;
				if (timer >= refreshRate)
				{
					if (data.volumeDebugSettings.selectedCamera != null)
					{
						SetTableColumnVisibility(data, table);
						Volume[] volumes2 = data.volumeDebugSettings.GetVolumes();
						if (!Enumerable.SequenceEqual(volumes, volumes2))
						{
							volumes = volumes2;
							DebugManager.instance.ReDrawOnScreenDebug();
						}
					}
					timer = 0f;
				}
				return false;
			};
			return table;
		}

		private static void SetTableColumnVisibility(DebugDisplaySettingsVolume data, DebugUI.Table table)
		{
			List<VolumeParameterChain> resolutionChain = GetResolutionChain(data);
			for (int i = 1; i < resolutionChain.Count; i++)
			{
				bool flag = true;
				if (resolutionChain[i].volume != null)
				{
					flag = data.volumeDebugSettings.VolumeHasInfluence(resolutionChain[i].volume);
				}
				else
				{
					flag = resolutionChain[i].volumeComponent.active;
					if (flag)
					{
						bool flag2 = false;
						foreach (VolumeParameter parameter in resolutionChain[i].volumeComponent.parameterList)
						{
							if (parameter.overrideState)
							{
								flag2 = true;
								break;
							}
						}
						flag &= flag2;
					}
				}
				table.SetColumnVisibility(i, flag);
			}
		}

		private static void GenerateTableColumns(DebugUI.Table table, DebugDisplaySettingsVolume data, List<VolumeParameterChain> resolutionChain)
		{
			for (int i = 0; i < resolutionChain.Count; i++)
			{
				VolumeParameterChain chain = resolutionChain[i];
				int num = -1;
				if (chain.volume != null)
				{
					((DebugUI.Table.Row)table.children[++num]).children.Add(new DebugUI.Value
					{
						nameAndTooltip = chain.nameAndTooltip,
						getter = () =>
						{
							string obj = (chain.volume.isGlobal ? Strings.global : Strings.local);
							float volumeWeight = data.volumeDebugSettings.GetVolumeWeight(chain.volume);
							return obj + " (" + volumeWeight * 100f + "%)";
						},
						refreshRate = 0.2f
					});
					((DebugUI.Table.Row)table.children[++num]).children.Add(new DebugUI.ObjectField
					{
						displayName = string.Empty,
						getter = () => chain.volume
					});
				}
				else
				{
					((DebugUI.Table.Row)table.children[++num]).children.Add(new DebugUI.Value
					{
						nameAndTooltip = chain.nameAndTooltip,
						getter = () => string.Empty
					});
					((DebugUI.Table.Row)table.children[++num]).children.Add(s_EmptyDebugUIValue);
				}
				((DebugUI.Table.Row)table.children[++num]).children.Add((chain.volumeProfile != null) ? ((DebugUI.Widget)new DebugUI.ObjectField
				{
					displayName = string.Empty,
					getter = () => chain.volumeProfile
				}) : ((DebugUI.Widget)s_EmptyDebugUIValue));
				((DebugUI.Table.Row)table.children[++num]).children.Add(s_EmptyDebugUIValue);
				bool isResultParameter = i == 0;
				for (int num2 = 0; num2 < chain.volumeComponent.parameterList.Count; num2++)
				{
					VolumeParameter param = chain.volumeComponent.parameterList[num2];
					((DebugUI.Table.Row)table.children[++num]).children.Add(CreateVolumeParameterWidget(chain.nameAndTooltip.name, isResultParameter, param));
				}
			}
		}

		private static void GenerateTableRows(DebugUI.Table table, List<VolumeParameterChain> resolutionChain)
		{
			DebugUI.Table.Row item = new DebugUI.Table.Row
			{
				displayName = Strings.volumeInfo,
				opened = true
			};
			table.children.Add(item);
			DebugUI.Table.Row item2 = new DebugUI.Table.Row
			{
				displayName = Strings.gameObject
			};
			table.children.Add(item2);
			DebugUI.Table.Row item3 = new DebugUI.Table.Row
			{
				displayName = Strings.volumeProfile
			};
			table.children.Add(item3);
			DebugUI.Table.Row item4 = new DebugUI.Table.Row
			{
				displayName = string.Empty
			};
			table.children.Add(item4);
			VolumeComponent volumeComponent = resolutionChain[0].volumeComponent;
			for (int i = 0; i < volumeComponent.parameterList.Count; i++)
			{
				_ = volumeComponent.parameterList[i];
				string displayName = i.ToString();
				table.children.Add(new DebugUI.Table.Row
				{
					displayName = displayName
				});
			}
		}
	}

	[DisplayInfo(name = "Volume", order = int.MaxValue)]
	internal class SettingsPanel : DebugDisplaySettingsPanel<DebugDisplaySettingsVolume>
	{
		private DebugUI.Table m_VolumeTable;

		public SettingsPanel(DebugDisplaySettingsVolume data)
			: base(data)
		{
			AddWidget(WidgetFactory.CreateComponentSelector(this, (DebugUI.Field<int> _, int __) =>
			{
				Refresh();
			}));
			AddWidget(WidgetFactory.CreateCameraSelector(this, (DebugUI.Field<Object> _, Object __) =>
			{
				Refresh();
			}));
		}

		private void Refresh()
		{
			DebugUI.Panel panel = DebugManager.instance.GetPanel(PanelName);
			if (panel != null)
			{
				bool flag = false;
				if (m_VolumeTable != null)
				{
					flag = true;
					panel.children.Remove(m_VolumeTable);
				}
				if (m_Data.volumeDebugSettings.selectedComponent > 0 && m_Data.volumeDebugSettings.selectedCamera != null)
				{
					flag = true;
					m_VolumeTable = WidgetFactory.CreateVolumeTable(m_Data);
					AddWidget(m_VolumeTable);
					panel.children.Add(m_VolumeTable);
				}
				if (flag)
				{
					DebugManager.instance.ReDrawOnScreenDebug();
				}
			}
		}
	}

	internal int volumeComponentEnumIndex;

	public IVolumeDebugSettings2 volumeDebugSettings { get; }

	public bool AreAnySettingsActive => false;

	public bool IsPostProcessingAllowed => true;

	public bool IsLightingActive => true;

	public DebugDisplaySettingsVolume(IVolumeDebugSettings2 volumeDebugSettings)
	{
		this.volumeDebugSettings = volumeDebugSettings;
	}

	public bool TryGetScreenClearColor(ref Color color)
	{
		return false;
	}

	public IDebugDisplaySettingsPanelDisposable CreatePanel()
	{
		return new SettingsPanel(this);
	}
}
