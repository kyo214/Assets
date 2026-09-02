using UnityEngine;

namespace _Modules.UILobby.Scripts;

public class PentagramLampController : MonoBehaviour
{
	[SerializeField]
	private PentagramLamp[] _pentagramLamps;

	public int Length => _pentagramLamps.Length;

	public void Init(int life)
	{
		for (int num = _pentagramLamps.Length - 1; num >= 0; num--)
		{
			_pentagramLamps[num]?.SetActive(num < life);
			_pentagramLamps[num]?.SetActiveParentExplosion(num < life);
		}
	}

	public void GetPentagramLamp()
	{
		_pentagramLamps = base.gameObject.GetComponentsInChildren<PentagramLamp>();
	}
}
