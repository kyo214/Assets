using System;

namespace BansheeGz.BGDatabase;

public class BGEventsDelegatesHolder<T> where T : EventArgs
{
	public EventHandler<T> Handler;
}
