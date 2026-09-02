using System;
using System.Collections.Generic;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Signals;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Input;

public static class InputStream
{
	public const string k_StreamCategory = "Input";

	public const string k_StreamName = "InputStream";

	[ClearOnReload]
	private static SignalStream s_stream;

	public const string k_NavigateStreamCategory = "Navigate";

	public const string k_NavigateLeft = "Left";

	public const string k_NavigateRight = "Right";

	public const string k_NavigateUp = "Up";

	public const string k_NavigateDown = "Down";

	[ClearOnReload]
	private static SignalStream s_navigateLeftStream;

	[ClearOnReload]
	private static SignalStream s_navigateRightStream;

	[ClearOnReload]
	private static SignalStream s_navigateUpStream;

	[ClearOnReload]
	private static SignalStream s_navigateDownStream;

	public const string k_CustomInputActionStreamCategory = "CustomInputAction";

	[ClearOnReload]
	private static Dictionary<string, SignalStream> s_customInputActionSignalStreams;

	public static SignalStream stream => s_stream ?? (s_stream = SignalsService.GetStream("Input", "InputStream"));

	public static SignalStream navigateLeftStream => s_navigateLeftStream ?? (s_navigateLeftStream = SignalsService.GetStream("Navigate", "Left"));

	public static SignalStream navigateRightStream => s_navigateRightStream ?? (s_navigateRightStream = SignalsService.GetStream("Navigate", "Right"));

	public static SignalStream navigateUpStream => s_navigateUpStream ?? (s_navigateUpStream = SignalsService.GetStream("Navigate", "Up"));

	public static SignalStream navigateDownStream => s_navigateDownStream ?? (s_navigateDownStream = SignalsService.GetStream("Navigate", "Down"));

	private static Dictionary<string, SignalStream> customInputActionSignalStreams => s_customInputActionSignalStreams ?? (s_customInputActionSignalStreams = new Dictionary<string, SignalStream>());

	[ClearOnReload]
	private static SignalReceiver inputStreamReceiver { get; set; }

	private static bool connected { get; set; }

	private static void ConnectToInputStream()
	{
		stream.ConnectReceiver(inputStreamReceiver);
		connected = true;
	}

	private static void DisconnectFromInputStream()
	{
		stream.DisconnectReceiver(inputStreamReceiver);
		connected = false;
	}

	public static void Start()
	{
		if (connected)
		{
			return;
		}
		inputStreamReceiver = new SignalReceiver();
		inputStreamReceiver.SetOnSignalCallback((Signal signal) =>
		{
			if (signal.hasValue && signal.valueAsObject is InputSignalData data)
			{
				switch (data.inputActionName)
				{
				case UIInputActionName.Navigate:
					Navigate(data);
					break;
				case UIInputActionName.CustomActionName:
					CustomInputAction(data);
					break;
				default:
					throw new ArgumentOutOfRangeException();
				case UIInputActionName.Point:
				case UIInputActionName.Click:
				case UIInputActionName.MiddleClick:
				case UIInputActionName.RightClick:
				case UIInputActionName.ScrollWheel:
				case UIInputActionName.Submit:
				case UIInputActionName.Cancel:
				case UIInputActionName.TrackedDevicePosition:
				case UIInputActionName.TrackedDeviceOrientation:
					break;
				}
			}
		});
		ConnectToInputStream();
	}

	public static void Stop()
	{
		if (connected)
		{
			DisconnectFromInputStream();
			inputStreamReceiver = null;
		}
	}

	private static void Navigate(InputSignalData data)
	{
		Vector2 vector = data.callbackContext.ReadValue<Vector2>();
		if (vector.x < 0f)
		{
			navigateLeftStream.SendSignal(data);
		}
		else if (vector.x > 0f)
		{
			navigateRightStream.SendSignal(data);
		}
		else if (vector.y < 0f)
		{
			navigateDownStream.SendSignal(data);
		}
		else if (vector.y > 0f)
		{
			navigateUpStream.SendSignal(data);
		}
	}

	private static void CustomInputAction(InputSignalData data)
	{
		string name = data.callbackContext.action.name;
		if (!customInputActionSignalStreams.TryGetValue(name, out var value))
		{
			value = SignalsService.GetStream("CustomInputAction", name);
			customInputActionSignalStreams.Add(name, value);
		}
		value.SendSignal(data);
	}
}
