using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Common.ScriptableObjects;
using Doozy.Runtime.Global;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Containers.Internal;
using Doozy.Runtime.UIManager.Orientation;
using Doozy.Runtime.UIManager.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Doozy.Runtime.UIManager.Containers;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(GraphicRaycaster))]
[RequireComponent(typeof(RectTransform))]
[AddComponentMenu("UI/Containers/UIView")]
public class UIView : UIContainerComponent<UIView>
{
	[ClearOnReload]
	private static SignalStream s_stream;

	public UIViewId Id;

	[SerializeField]
	private TargetOrientation TargetOrientation;

	public static SignalStream stream => s_stream ?? (s_stream = SignalsService.GetStream("UIContainer", "UIView"));

	public static IEnumerable<UIView> visibleViews => UIContainerComponent<UIView>.database.Where((UIView view) => view.isVisible || view.isShowing);

	public static IEnumerable<UIView> hiddenViews => UIContainerComponent<UIView>.database.Where((UIView view) => view.isHidden || view.isHiding);

	private SignalReceiver receiver { get; set; }

	public TargetOrientation targetOrientation
	{
		get
		{
			return TargetOrientation;
		}
		set
		{
			TargetOrientation = value;
		}
	}

	private static bool useOrientationDetection => SingletonRuntimeScriptableObject<UIManagerSettings>.instance.UseOrientationDetection;

	private static DetectedOrientation currentDeviceOrientation => SingletonBehaviour<OrientationDetector>.instance.currentOrientation;

	private bool canShow
	{
		get
		{
			if (!useOrientationDetection)
			{
				return true;
			}
			if (currentDeviceOrientation == DetectedOrientation.Unknown)
			{
				return false;
			}
			return targetOrientation switch
			{
				TargetOrientation.Any => true, 
				TargetOrientation.Portrait => currentDeviceOrientation == DetectedOrientation.Portrait, 
				TargetOrientation.Landscape => currentDeviceOrientation == DetectedOrientation.Landscape, 
				_ => throw new ArgumentOutOfRangeException(), 
			};
		}
	}

	public UIView()
	{
		Id = new UIViewId();
	}

	protected override void Awake()
	{
		base.Awake();
		receiver = new SignalReceiver().SetOnSignalCallback(ProcessSignal);
		stream.ConnectReceiver(receiver);
		ConnectToOrientationDetector();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		stream.DisconnectReceiver(receiver);
		DisconnectFromOrientationDetector();
	}

	private void ProcessSignal(Signal signal)
	{
		if (!signal.hasValue || !(signal.valueAsObject is UIViewSignalData uIViewSignalData) || ((UIContainer.multiplayerMode & base.hasMultiplayerInfo) && uIViewSignalData.playerIndex != base.playerIndex))
		{
			return;
		}
		if (uIViewSignalData.globalCommand)
		{
			Execute(uIViewSignalData.execute);
		}
		else if (uIViewSignalData.viewCategory.Equals(Id.Category))
		{
			if (uIViewSignalData.categoryCommand)
			{
				Execute(uIViewSignalData.execute);
			}
			else if (uIViewSignalData.viewName.Equals(Id.Name))
			{
				Execute(uIViewSignalData.execute);
			}
		}
	}

	private void Execute(ShowHideExecute execute)
	{
		if (useOrientationDetection && currentDeviceOrientation == DetectedOrientation.Unknown)
		{
			StartCoroutine(Coroutiner.DelayExecutionToTheNextFrame(() =>
			{
				Execute(execute);
			}));
			return;
		}
		switch (execute)
		{
		case ShowHideExecute.Show:
			if (canShow)
			{
				Show();
			}
			else
			{
				InstantHide(triggerCallbacks: false);
			}
			break;
		case ShowHideExecute.Hide:
			Hide();
			break;
		case ShowHideExecute.InstantShow:
			if (canShow)
			{
				InstantShow();
			}
			else
			{
				InstantHide(triggerCallbacks: false);
			}
			break;
		case ShowHideExecute.InstantHide:
			InstantHide();
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case ShowHideExecute.ReverseShow:
		case ShowHideExecute.ReverseHide:
			break;
		}
	}

	private void ConnectToOrientationDetector()
	{
		if (useOrientationDetection && SingletonBehaviour<OrientationDetector>.instance != null)
		{
			SingletonBehaviour<OrientationDetector>.instance.OnOrientationChanged.AddListener(OnOrientationChanged);
		}
	}

	private void DisconnectFromOrientationDetector()
	{
		if (useOrientationDetection && SingletonBehaviour<OrientationDetector>.instance != null)
		{
			SingletonBehaviour<OrientationDetector>.instance.OnOrientationChanged.RemoveListener(OnOrientationChanged);
		}
	}

	private void OnOrientationChanged(DetectedOrientation orientation)
	{
		switch (orientation)
		{
		case DetectedOrientation.Unknown:
			break;
		case DetectedOrientation.Portrait:
			if (targetOrientation == TargetOrientation.Landscape && (base.isVisible | base.isShowing))
			{
				InstantHide();
				Coroutiner.ExecuteAtEndOfFrame(() =>
				{
					Show(Id.Category, Id.Name);
				});
			}
			break;
		case DetectedOrientation.Landscape:
			if (targetOrientation == TargetOrientation.Portrait && (base.isVisible | base.isShowing))
			{
				InstantHide();
				Coroutiner.ExecuteAtEndOfFrame(() =>
				{
					Show(Id.Category, Id.Name);
				});
			}
			break;
		default:
			throw new ArgumentOutOfRangeException("orientation", orientation, null);
		}
	}

	protected override void RunBehaviour(ContainerBehaviour behaviour)
	{
		if (!useOrientationDetection)
		{
			base.RunBehaviour(behaviour);
		}
		else if (currentDeviceOrientation != DetectedOrientation.Unknown)
		{
			switch (behaviour)
			{
			case ContainerBehaviour.Disabled:
				break;
			case ContainerBehaviour.InstantHide:
				VisibilityState = VisibilityState.Visible;
				InstantHide();
				break;
			case ContainerBehaviour.InstantShow:
				if (canShow)
				{
					VisibilityState = VisibilityState.Hidden;
					InstantShow();
				}
				else
				{
					InstantHide(triggerCallbacks: false);
				}
				break;
			case ContainerBehaviour.Hide:
				VisibilityState = VisibilityState.Visible;
				Hide();
				break;
			case ContainerBehaviour.Show:
				InstantHide(triggerCallbacks: false);
				if (canShow)
				{
					StartCoroutine(Coroutiner.DelayExecution(Show, 2));
				}
				break;
			default:
				throw new ArgumentOutOfRangeException("behaviour", behaviour, null);
			}
		}
		else
		{
			StartCoroutine(Coroutiner.DelayExecutionToTheNextFrame(() =>
			{
				RunBehaviour(behaviour);
			}));
		}
	}

	public static IEnumerable<UIView> GetViews(string category, string name)
	{
		return from view in UIContainerComponent<UIView>.database
			where view.Id.Category.Equals(category)
			where view.Id.Name.Equals(name)
			select view;
	}

	public static IEnumerable<UIView> GetAllViewsInCategory(string category)
	{
		return UIContainerComponent<UIView>.database.Where((UIView view) => view.Id.Category.Equals(category));
	}

	internal static void Toggle(string category, string name, ShowHideExecute execute, int playerIndex)
	{
		stream.SendSignal(new UIViewSignalData(category, name, execute, playerIndex));
	}

	public static void Show(string category, string name, bool instant, int playerIndex)
	{
		Toggle(category, name, instant ? ShowHideExecute.InstantShow : ShowHideExecute.Show, playerIndex);
	}

	public static void Show(string category, string name, bool instant = false)
	{
		Show(category, name, instant, UIContainer.inputSettings.defaultPlayerIndex);
	}

	public static void ShowCategory(string category, bool instant, int playerIndex)
	{
		Show(category, string.Empty, instant, playerIndex);
	}

	public static void ShowCategory(string category, bool instant = false)
	{
		ShowCategory(category, instant, UIContainer.inputSettings.defaultPlayerIndex);
	}

	public static void Hide(string category, string name, bool instant, int playerIndex)
	{
		Toggle(category, name, (!instant) ? ShowHideExecute.Hide : ShowHideExecute.InstantHide, playerIndex);
	}

	public static void Hide(string category, string name, bool instant = false)
	{
		Hide(category, name, instant, UIContainer.inputSettings.defaultPlayerIndex);
	}

	public static void HideCategory(string category, bool instant, int playerIndex)
	{
		Hide(category, string.Empty, instant, playerIndex);
	}

	public static void HideCategory(string category, bool instant = false)
	{
		HideCategory(category, instant, UIContainer.inputSettings.defaultPlayerIndex);
	}

	public static void HideAllViews(bool instant, int playerIndex)
	{
		stream.SendSignal(new UIViewSignalData(string.Empty, string.Empty, (!instant) ? ShowHideExecute.Hide : ShowHideExecute.InstantHide, playerIndex));
	}

	public static void HideAllViews(bool instant = false)
	{
		stream.SendSignal(new UIViewSignalData(string.Empty, string.Empty, (!instant) ? ShowHideExecute.Hide : ShowHideExecute.InstantHide, UIContainer.inputSettings.defaultPlayerIndex));
	}

	public static IEnumerable<UIView> GetViews(UIViewId.InGame id)
	{
		return GetViews("InGame", id.ToString());
	}

	public static void Show(UIViewId.InGame id, bool instant = false)
	{
		Show("InGame", id.ToString(), instant);
	}

	public static void Hide(UIViewId.InGame id, bool instant = false)
	{
		Hide("InGame", id.ToString(), instant);
	}

	public static IEnumerable<UIView> GetViews(UIViewId.MainMenu id)
	{
		return GetViews("MainMenu", id.ToString());
	}

	public static void Show(UIViewId.MainMenu id, bool instant = false)
	{
		Show("MainMenu", id.ToString(), instant);
	}

	public static void Hide(UIViewId.MainMenu id, bool instant = false)
	{
		Hide("MainMenu", id.ToString(), instant);
	}
}
