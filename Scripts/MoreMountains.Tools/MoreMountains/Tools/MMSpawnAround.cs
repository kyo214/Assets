using UnityEngine;

namespace MoreMountains.Tools;

public static class MMSpawnAround
{
	public static void ApplySpawnAroundProperties(GameObject instantiatedObj, MMSpawnAroundProperties props, Vector3 origin)
	{
		instantiatedObj.transform.position = SpawnAroundPosition(props, origin);
		instantiatedObj.transform.rotation = SpawnAroundRotation(props);
		instantiatedObj.transform.localScale = SpawnAroundScale(props);
	}

	public static Vector3 SpawnAroundPosition(MMSpawnAroundProperties props, Vector3 origin)
	{
		Vector3 vector = default;
		if (props.Shape == MMSpawnAroundProperties.MMSpawnAroundShapes.Sphere)
		{
			float num = Random.Range(props.MinimumSphereRadius, props.MaximumSphereRadius);
			vector = Vector3.Cross(Random.insideUnitSphere, props.NormalToSpawnPlane);
			vector.Normalize();
			vector *= num;
		}
		else
		{
			float num2 = Random.Range(props.MinimumCubeBaseSize.x, props.MaximumCubeBaseSize.x);
			vector.x = Random.Range(0f - num2, num2) / 2f;
			float num3 = Random.Range(props.MinimumCubeBaseSize.y, props.MaximumCubeBaseSize.y);
			vector.y = Random.Range(0f - num3, num3) / 2f;
			float num4 = Random.Range(props.MinimumCubeBaseSize.z, props.MaximumCubeBaseSize.z);
			vector.z = Random.Range(0f - num4, num4) / 2f;
			vector = Vector3.Cross(vector, props.NormalToSpawnPlane);
		}
		float num5 = Random.Range(props.MinimumNormalAxisOffset, props.MaximumNormalAxisOffset);
		if (props.UseNormalAxisOffsetCurve)
		{
			float time = 0f;
			if (num5 != 0f)
			{
				time = ((!props.InvertNormalOffsetCurve) ? MMMaths.Remap(num5, props.MinimumNormalAxisOffset, props.MaximumNormalAxisOffset, 0f, 1f) : MMMaths.Remap(num5, props.MinimumNormalAxisOffset, props.MaximumNormalAxisOffset, 1f, 0f));
			}
			float x = props.NormalOffsetCurve.Evaluate(time);
			x = MMMaths.Remap(x, 0f, 1f, props.NormalOffsetCurveRemapZero, props.NormalOffsetCurveRemapOne);
			vector *= x;
		}
		vector += props.NormalToSpawnPlane.normalized * num5;
		return vector + origin;
	}

	public static Vector3 SpawnAroundScale(MMSpawnAroundProperties props)
	{
		return MMMaths.RandomVector3(props.MinimumScale, props.MaximumScale);
	}

	public static Quaternion SpawnAroundRotation(MMSpawnAroundProperties props)
	{
		return Quaternion.Euler(MMMaths.RandomVector3(props.MinimumRotation, props.MaximumRotation));
	}

	public static void DrawGizmos(MMSpawnAroundProperties props, Vector3 origin, int quantity, float size, Color gizmosColor)
	{
		Gizmos.color = gizmosColor;
		for (int i = 0; i < quantity; i++)
		{
			Gizmos.DrawCube(SpawnAroundPosition(props, origin), SpawnAroundScale(props) * size);
		}
	}
}
