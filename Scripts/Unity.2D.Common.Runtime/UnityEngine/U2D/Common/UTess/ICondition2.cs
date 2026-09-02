namespace UnityEngine.U2D.Common.UTess;

internal interface ICondition2<in T, in U>
{
	bool Test(T x, U y, ref float t);
}
