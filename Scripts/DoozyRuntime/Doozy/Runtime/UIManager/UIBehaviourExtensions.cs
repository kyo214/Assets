using Doozy.Runtime.Signals;
using UnityEngine;

namespace Doozy.Runtime.UIManager;

public static class UIBehaviourExtensions
{
	public static T SetSignalSource<T>(this T target, GameObject signalSource) where T : UIBehaviour
	{
		target.receiver.signalSource = signalSource;
		return target;
	}

	public static T SetBehaviourName<T>(this T target, UIBehaviour.Name behaviourName) where T : UIBehaviour
	{
		target.receiver.SetProviderId(UIBehaviour.GetProvideId(behaviourName));
		return target;
	}
}
