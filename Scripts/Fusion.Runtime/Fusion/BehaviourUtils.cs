using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Fusion;

internal static class BehaviourUtils
{
	public struct DeferredJoin
	{
		public IEnumerable _enumerable;

		public override string ToString()
		{
			return string.Join(", ", _enumerable.Cast<object>());
		}
	}

	internal struct NameDeferred(Behaviour behaviour)
	{
		private Behaviour _behaviour = behaviour;

		public static explicit operator NameDeferred(Behaviour behaviour)
		{
			return new NameDeferred(behaviour);
		}

		public static implicit operator string(NameDeferred wrapper)
		{
			return wrapper.ToString();
		}

		public override string ToString()
		{
			if (IsNull(_behaviour))
			{
				return "(null)";
			}
			return IsAlive(_behaviour) ? _behaviour.name : "(destroyed)";
		}
	}

	internal struct DumpDeferred
	{
		private NetworkObject _object;

		private NetworkBehaviour _behaviour;

		public DumpDeferred(NetworkObject obj)
		{
			_object = obj;
			_behaviour = null;
		}

		public DumpDeferred(NetworkBehaviour obj)
		{
			_behaviour = obj;
			if (IsNull(obj))
			{
				_object = null;
			}
			else
			{
				_object = obj.Object;
			}
		}

		public override string ToString()
		{
			if (IsNull(_object))
			{
				return "(null)";
			}
			StringBuilder stringBuilder = new StringBuilder();
			_object.AddDebugMessagePrefix(stringBuilder, default(LogOptions), _object.IsSceneObject, addHashCode: true);
			if (IsNotNull(_behaviour))
			{
				stringBuilder.Append($" [{_behaviour.GetType().Name}:{_behaviour.ObjectIndex}]");
			}
			return stringBuilder.ToString();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNull(Behaviour obj)
	{
		return (object)obj == null;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNotNull(Behaviour obj)
	{
		return (object)obj != null;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsAlive(Behaviour obj)
	{
		return obj;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNotAlive(Behaviour obj)
	{
		return !obj;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsSame(Behaviour a, Behaviour b)
	{
		return (object)a == b;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsSameNotNull(Behaviour a, Behaviour b)
	{
		return (object)a != null && (object)a == b;
	}

	public static NameDeferred GetName(Behaviour obj)
	{
		return new NameDeferred(obj);
	}

	public static DumpDeferred GetDump(NetworkObject obj)
	{
		return new DumpDeferred(obj);
	}

	public static DumpDeferred GetDump(NetworkBehaviour obj)
	{
		return new DumpDeferred(obj);
	}

	public static DeferredJoin Join(IEnumerable objects)
	{
		return new DeferredJoin
		{
			_enumerable = objects
		};
	}
}
