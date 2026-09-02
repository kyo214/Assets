using System.Collections.Generic;

namespace Fusion.KCC;

public sealed class KCCNetworkCollisions : KCCNetworkProperty<KCCNetworkContext>
{
	private int _maxCount;

	public KCCNetworkCollisions(KCCNetworkContext context, int maxCount)
		: base(context, 1 + maxCount * 4)
	{
		_maxCount = maxCount;
	}

	public unsafe override void Read(int* ptr)
	{
		KCCData data = Context.Data;
		NetworkRunner runner = Context.KCC.Runner;
		data.Collisions.Clear();
		int num = *ptr;
		int* ptr2 = ptr + 1;
		for (int i = 0; i < num; i++)
		{
			KCCNetworkID networkID = KCCNetworkUtility.ReadNetworkID(ptr2);
			ptr2 += 4;
			if (networkID.IsValid)
			{
				data.Collisions.Add(KCCNetworkID.GetNetworkObject(runner, networkID), networkID);
			}
		}
	}

	public unsafe override void Write(int* ptr)
	{
		KCCData data = Context.Data;
		int num = 0;
		int* ptr2 = ptr + 1;
		List<KCCCollision> all = data.Collisions.All;
		int i = 0;
		for (int count = all.Count; i < count; i++)
		{
			KCCCollision kCCCollision = all[i];
			if (kCCCollision.NetworkID.IsValid)
			{
				KCCNetworkUtility.WriteNetworkID(ptr2, kCCCollision.NetworkID);
				ptr2 += 4;
				num++;
				if (num >= _maxCount)
				{
					break;
				}
			}
		}
		*ptr = num;
	}

	public override void Interpolate(InterpolationData interpolationData)
	{
	}
}
