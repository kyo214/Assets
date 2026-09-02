namespace Fusion.LagCompensation;

public struct PositionRotationQuery
{
	public Hitbox Hitbox;

	public static Query CreateQuery(Hitbox hb, int tick, int? tickTo = null, float? alpha = null)
	{
		return new Query
		{
			Type = QueryType.PositionRotation,
			Options = ((tickTo.HasValue & alpha.HasValue) ? HitOptions.SubtickAccuracy : HitOptions.None),
			Tick = tick,
			TickTo = tickTo,
			Alpha = alpha,
			PositionRotation = new PositionRotationQuery
			{
				Hitbox = hb
			}
		};
	}

	public static Query CreateQuery(Hitbox hb, PlayerRef player, bool subTickAccuracy = false)
	{
		return new Query
		{
			Type = QueryType.PositionRotation,
			Player = player,
			Options = (subTickAccuracy ? HitOptions.SubtickAccuracy : HitOptions.None),
			PositionRotation = new PositionRotationQuery
			{
				Hitbox = hb
			}
		};
	}
}
