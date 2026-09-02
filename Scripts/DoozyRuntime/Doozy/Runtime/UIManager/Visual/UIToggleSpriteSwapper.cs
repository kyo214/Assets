using System.Collections.Generic;
using Doozy.Runtime.Reactor.Targets;
using Doozy.Runtime.Reactor.Ticker;
using Doozy.Runtime.UIManager.Animators;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.UIManager.Visual;

[AddComponentMenu("UI/Components/Addons/UIToggle SpriteSwapper")]
public class UIToggleSpriteSwapper : BaseUIToggleAnimator
{
	[SerializeField]
	private ReactorSpriteTarget SpriteTarget;

	[SerializeField]
	private Sprite OnSprite;

	[SerializeField]
	private Sprite OffSprite;

	public ReactorSpriteTarget spriteTarget => SpriteTarget;

	public bool hasSpriteTarget => SpriteTarget != null;

	public Sprite onSprite
	{
		get
		{
			return OnSprite;
		}
		set
		{
			OnSprite = value;
			if (base.controller.isOn && hasSpriteTarget)
			{
				SpriteTarget.SetSprite(OnSprite);
			}
		}
	}

	public Sprite offSprite
	{
		get
		{
			return OffSprite;
		}
		set
		{
			OffSprite = value;
			if (!base.controller.isOn && hasSpriteTarget)
			{
				SpriteTarget.SetSprite(OffSprite);
			}
		}
	}

	protected override bool onAnimationIsActive
	{
		get
		{
			if (base.hasController && hasSpriteTarget && onSprite != null)
			{
				return spriteTarget.sprite == onSprite;
			}
			return false;
		}
	}

	protected override bool offAnimationIsActive
	{
		get
		{
			if (base.hasController && hasSpriteTarget && offSprite != null)
			{
				return spriteTarget.sprite == offSprite;
			}
			return false;
		}
	}

	protected override UnityAction playOnAnimation => () =>
	{
		if (hasSpriteTarget)
		{
			SpriteTarget.SetSprite(onSprite);
		}
	};

	protected override UnityAction playOffAnimation => () =>
	{
		if (hasSpriteTarget)
		{
			SpriteTarget.SetSprite(offSprite);
		}
	};

	protected override UnityAction reverseOnAnimation => () =>
	{
		if (hasSpriteTarget)
		{
			SpriteTarget.SetSprite(offSprite);
		}
	};

	protected override UnityAction reverseOffAnimation => () =>
	{
		if (hasSpriteTarget)
		{
			SpriteTarget.SetSprite(onSprite);
		}
	};

	protected override UnityAction instantPlayOnAnimation => () =>
	{
		if (hasSpriteTarget)
		{
			SpriteTarget.SetSprite(onSprite);
		}
	};

	protected override UnityAction instantPlayOffAnimation => () =>
	{
		if (hasSpriteTarget)
		{
			SpriteTarget.SetSprite(offSprite);
		}
	};

	protected override UnityAction stopOnAnimation => () =>
	{
	};

	protected override UnityAction stopOffAnimation => () =>
	{
	};

	protected override UnityAction addResetToOnStateCallback => () =>
	{
	};

	protected override UnityAction removeResetToOnStateCallback => () =>
	{
	};

	protected override UnityAction addResetToOffStateCallback => () =>
	{
	};

	protected override UnityAction removeResetToOffStateCallback => () =>
	{
	};

	public void FindTarget()
	{
		if (!(SpriteTarget != null))
		{
			SpriteTarget = ReactorSpriteTarget.FindTarget(base.gameObject);
			UpdateSettings();
		}
	}

	protected override void Awake()
	{
		FindTarget();
		UpdateSettings();
		base.Awake();
	}

	public override void UpdateSettings()
	{
		if (hasSpriteTarget && base.hasController)
		{
			SpriteTarget.SetSprite(base.controller.isOn ? onSprite : offSprite);
		}
	}

	public override void StopAllReactions()
	{
	}

	public override void ResetToStartValues(bool forced = false)
	{
	}

	public override List<Heartbeat> SetHeartbeat<Theartbeat>()
	{
		return null;
	}
}
