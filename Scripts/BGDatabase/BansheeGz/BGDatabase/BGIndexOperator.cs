using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public abstract class BGIndexOperator
{
	internal abstract void GetResult<T>(List<T> result, BGIndexStorage storage) where T : BGEntity;
}
