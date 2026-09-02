using System;

namespace Fusion;

internal static class CallbackInterfaceInvoker
{
	public static void IBeforeCopyPreviousState(SimulationBehaviourUpdater updater)
	{
		try
		{
			int callbackCount = updater.GetCallbackCount(typeof(IBeforeCopyPreviousState));
			for (int i = 0; i < callbackCount; i++)
			{
				SimulationBehaviour head;
				using (updater.GetCallbackHead(typeof(IBeforeCopyPreviousState), i, out head))
				{
					while (BehaviourUtils.IsNotNull(head))
					{
						SimulationBehaviour next = head.Next;
						if (head.CanReceiveCallback)
						{
							((IBeforeCopyPreviousState)head).BeforeCopyPreviousState();
						}
						head = next;
					}
				}
			}
		}
		catch (Exception exn)
		{
			Log.Exception(exn);
		}
	}

	public static void IBeforeClientPredictionReset(SimulationBehaviourUpdater updater)
	{
		try
		{
			int callbackCount = updater.GetCallbackCount(typeof(IBeforeClientPredictionReset));
			for (int i = 0; i < callbackCount; i++)
			{
				SimulationBehaviour head;
				using (updater.GetCallbackHead(typeof(IBeforeClientPredictionReset), i, out head))
				{
					while (BehaviourUtils.IsNotNull(head))
					{
						SimulationBehaviour next = head.Next;
						if (head.CanReceiveCallback)
						{
							((IBeforeClientPredictionReset)head).BeforeClientPredictionReset();
						}
						head = next;
					}
				}
			}
		}
		catch (Exception exn)
		{
			Log.Exception(exn);
		}
	}

	public static void IAfterClientPredictionReset(SimulationBehaviourUpdater updater)
	{
		try
		{
			int callbackCount = updater.GetCallbackCount(typeof(IAfterClientPredictionReset));
			for (int i = 0; i < callbackCount; i++)
			{
				SimulationBehaviour head;
				using (updater.GetCallbackHead(typeof(IAfterClientPredictionReset), i, out head))
				{
					while (BehaviourUtils.IsNotNull(head))
					{
						SimulationBehaviour next = head.Next;
						if (head.CanReceiveCallback)
						{
							((IAfterClientPredictionReset)head).AfterClientPredictionReset();
						}
						head = next;
					}
				}
			}
		}
		catch (Exception exn)
		{
			Log.Exception(exn);
		}
	}

	public static void IBeforeUpdateRemotePrefabs(SimulationBehaviourUpdater updater)
	{
		try
		{
			int callbackCount = updater.GetCallbackCount(typeof(IBeforeUpdateRemotePrefabs));
			for (int i = 0; i < callbackCount; i++)
			{
				SimulationBehaviour head;
				using (updater.GetCallbackHead(typeof(IBeforeUpdateRemotePrefabs), i, out head))
				{
					while (BehaviourUtils.IsNotNull(head))
					{
						SimulationBehaviour next = head.Next;
						if (head.CanReceiveCallback)
						{
							((IBeforeUpdateRemotePrefabs)head).BeforeUpdateRemotePrefabs();
						}
						head = next;
					}
				}
			}
		}
		catch (Exception exn)
		{
			Log.Exception(exn);
		}
	}

	public static void IAfterUpdateRemotePrefabs(SimulationBehaviourUpdater updater)
	{
		try
		{
			int callbackCount = updater.GetCallbackCount(typeof(IAfterUpdateRemotePrefabs));
			for (int i = 0; i < callbackCount; i++)
			{
				SimulationBehaviour head;
				using (updater.GetCallbackHead(typeof(IAfterUpdateRemotePrefabs), i, out head))
				{
					while (BehaviourUtils.IsNotNull(head))
					{
						SimulationBehaviour next = head.Next;
						if (head.CanReceiveCallback)
						{
							((IAfterUpdateRemotePrefabs)head).AfterUpdateRemotePrefabs();
						}
						head = next;
					}
				}
			}
		}
		catch (Exception exn)
		{
			Log.Exception(exn);
		}
	}

	public static void IBeforeTick(SimulationBehaviourUpdater updater)
	{
		try
		{
			int callbackCount = updater.GetCallbackCount(typeof(IBeforeTick));
			for (int i = 0; i < callbackCount; i++)
			{
				SimulationBehaviour head;
				using (updater.GetCallbackHead(typeof(IBeforeTick), i, out head))
				{
					while (BehaviourUtils.IsNotNull(head))
					{
						SimulationBehaviour next = head.Next;
						if (head.CanReceiveCallback)
						{
							((IBeforeTick)head).BeforeTick();
						}
						head = next;
					}
				}
			}
		}
		catch (Exception exn)
		{
			Log.Exception(exn);
		}
	}

	public static void IAfterTick(SimulationBehaviourUpdater updater)
	{
		try
		{
			int callbackCount = updater.GetCallbackCount(typeof(IAfterTick));
			for (int i = 0; i < callbackCount; i++)
			{
				SimulationBehaviour head;
				using (updater.GetCallbackHead(typeof(IAfterTick), i, out head))
				{
					while (BehaviourUtils.IsNotNull(head))
					{
						SimulationBehaviour next = head.Next;
						if (head.CanReceiveCallback)
						{
							((IAfterTick)head).AfterTick();
						}
						head = next;
					}
				}
			}
		}
		catch (Exception exn)
		{
			Log.Exception(exn);
		}
	}

	public static void IBeforeAllTicks(SimulationBehaviourUpdater updater, bool resimulation, int tickCount)
	{
		try
		{
			int callbackCount = updater.GetCallbackCount(typeof(IBeforeAllTicks));
			for (int i = 0; i < callbackCount; i++)
			{
				SimulationBehaviour head;
				using (updater.GetCallbackHead(typeof(IBeforeAllTicks), i, out head))
				{
					while (BehaviourUtils.IsNotNull(head))
					{
						SimulationBehaviour next = head.Next;
						if (head.CanReceiveCallback)
						{
							((IBeforeAllTicks)head).BeforeAllTicks(resimulation, tickCount);
						}
						head = next;
					}
				}
			}
		}
		catch (Exception exn)
		{
			Log.Exception(exn);
		}
	}

	public static void IAfterAllTicks(SimulationBehaviourUpdater updater, bool resimulation, int tickCount)
	{
		try
		{
			int callbackCount = updater.GetCallbackCount(typeof(IAfterAllTicks));
			for (int i = 0; i < callbackCount; i++)
			{
				SimulationBehaviour head;
				using (updater.GetCallbackHead(typeof(IAfterAllTicks), i, out head))
				{
					while (BehaviourUtils.IsNotNull(head))
					{
						SimulationBehaviour next = head.Next;
						if (head.CanReceiveCallback)
						{
							((IAfterAllTicks)head).AfterAllTicks(resimulation, tickCount);
						}
						head = next;
					}
				}
			}
		}
		catch (Exception exn)
		{
			Log.Exception(exn);
		}
	}

	public static void IPlayerJoined(SimulationBehaviourUpdater updater, PlayerRef player)
	{
		try
		{
			int callbackCount = updater.GetCallbackCount(typeof(IPlayerJoined));
			for (int i = 0; i < callbackCount; i++)
			{
				SimulationBehaviour head;
				using (updater.GetCallbackHead(typeof(IPlayerJoined), i, out head))
				{
					while (BehaviourUtils.IsNotNull(head))
					{
						SimulationBehaviour next = head.Next;
						if (head.CanReceiveCallback)
						{
							((IPlayerJoined)head).PlayerJoined(player);
						}
						head = next;
					}
				}
			}
		}
		catch (Exception exn)
		{
			Log.Exception(exn);
		}
	}

	public static void IPlayerLeft(SimulationBehaviourUpdater updater, PlayerRef player)
	{
		try
		{
			int callbackCount = updater.GetCallbackCount(typeof(IPlayerLeft));
			for (int i = 0; i < callbackCount; i++)
			{
				SimulationBehaviour head;
				using (updater.GetCallbackHead(typeof(IPlayerLeft), i, out head))
				{
					while (BehaviourUtils.IsNotNull(head))
					{
						SimulationBehaviour next = head.Next;
						if (head.CanReceiveCallback)
						{
							((IPlayerLeft)head).PlayerLeft(player);
						}
						head = next;
					}
				}
			}
		}
		catch (Exception exn)
		{
			Log.Exception(exn);
		}
	}

	public static void IAfterPhysicsStep(SimulationBehaviourUpdater updater)
	{
		try
		{
			int callbackCount = updater.GetCallbackCount(typeof(IAfterPhysicsStep));
			for (int i = 0; i < callbackCount; i++)
			{
				SimulationBehaviour head;
				using (updater.GetCallbackHead(typeof(IAfterPhysicsStep), i, out head))
				{
					while (BehaviourUtils.IsNotNull(head))
					{
						SimulationBehaviour next = head.Next;
						if (head.CanReceiveCallback)
						{
							((IAfterPhysicsStep)head).AfterPhysicsStep();
						}
						head = next;
					}
				}
			}
		}
		catch (Exception exn)
		{
			Log.Exception(exn);
		}
	}

	public static void IBeforePhysicsStep(SimulationBehaviourUpdater updater)
	{
		try
		{
			int callbackCount = updater.GetCallbackCount(typeof(IBeforePhysicsStep));
			for (int i = 0; i < callbackCount; i++)
			{
				SimulationBehaviour head;
				using (updater.GetCallbackHead(typeof(IBeforePhysicsStep), i, out head))
				{
					while (BehaviourUtils.IsNotNull(head))
					{
						SimulationBehaviour next = head.Next;
						if (head.CanReceiveCallback)
						{
							((IBeforePhysicsStep)head).BeforePhysicsStep();
						}
						head = next;
					}
				}
			}
		}
		catch (Exception exn)
		{
			Log.Exception(exn);
		}
	}

	public static void IBeforeHitboxRegistration(SimulationBehaviourUpdater updater)
	{
		try
		{
			int callbackCount = updater.GetCallbackCount(typeof(IBeforeHitboxRegistration));
			for (int i = 0; i < callbackCount; i++)
			{
				SimulationBehaviour head;
				using (updater.GetCallbackHead(typeof(IBeforeHitboxRegistration), i, out head))
				{
					while (BehaviourUtils.IsNotNull(head))
					{
						SimulationBehaviour next = head.Next;
						if (head.CanReceiveCallback)
						{
							((IBeforeHitboxRegistration)head).BeforeHitboxRegistration();
						}
						head = next;
					}
				}
			}
		}
		catch (Exception exn)
		{
			Log.Exception(exn);
		}
	}

	public static void IAfterPhysicsSyncTransforms2D(SimulationBehaviourUpdater updater)
	{
		try
		{
			int callbackCount = updater.GetCallbackCount(typeof(IAfterPhysicsSyncTransforms2D));
			for (int i = 0; i < callbackCount; i++)
			{
				SimulationBehaviour head;
				using (updater.GetCallbackHead(typeof(IAfterPhysicsSyncTransforms2D), i, out head))
				{
					while (BehaviourUtils.IsNotNull(head))
					{
						SimulationBehaviour next = head.Next;
						if (head.CanReceiveCallback)
						{
							((IAfterPhysicsSyncTransforms2D)head).AfterPhysicsSyncTransforms2D();
						}
						head = next;
					}
				}
			}
		}
		catch (Exception exn)
		{
			Log.Exception(exn);
		}
	}

	public static void IAfterPhysicsSyncTransforms3D(SimulationBehaviourUpdater updater)
	{
		try
		{
			int callbackCount = updater.GetCallbackCount(typeof(IAfterPhysicsSyncTransforms3D));
			for (int i = 0; i < callbackCount; i++)
			{
				SimulationBehaviour head;
				using (updater.GetCallbackHead(typeof(IAfterPhysicsSyncTransforms3D), i, out head))
				{
					while (BehaviourUtils.IsNotNull(head))
					{
						SimulationBehaviour next = head.Next;
						if (head.CanReceiveCallback)
						{
							((IAfterPhysicsSyncTransforms3D)head).AfterPhysicsSyncTransforms3D();
						}
						head = next;
					}
				}
			}
		}
		catch (Exception exn)
		{
			Log.Exception(exn);
		}
	}

	public static void IAfterUpdate(SimulationBehaviourUpdater updater)
	{
		try
		{
			int callbackCount = updater.GetCallbackCount(typeof(IAfterUpdate));
			for (int i = 0; i < callbackCount; i++)
			{
				SimulationBehaviour head;
				using (updater.GetCallbackHead(typeof(IAfterUpdate), i, out head))
				{
					while (BehaviourUtils.IsNotNull(head))
					{
						SimulationBehaviour next = head.Next;
						if (head.CanReceiveCallback)
						{
							((IAfterUpdate)head).AfterUpdate();
						}
						head = next;
					}
				}
			}
		}
		catch (Exception exn)
		{
			Log.Exception(exn);
		}
	}

	public static void IBeforeUpdate(SimulationBehaviourUpdater updater)
	{
		try
		{
			int callbackCount = updater.GetCallbackCount(typeof(IBeforeUpdate));
			for (int i = 0; i < callbackCount; i++)
			{
				SimulationBehaviour head;
				using (updater.GetCallbackHead(typeof(IBeforeUpdate), i, out head))
				{
					while (BehaviourUtils.IsNotNull(head))
					{
						SimulationBehaviour next = head.Next;
						if (head.CanReceiveCallback)
						{
							((IBeforeUpdate)head).BeforeUpdate();
						}
						head = next;
					}
				}
			}
		}
		catch (Exception exn)
		{
			Log.Exception(exn);
		}
	}

	public static void ISceneLoadDone(SimulationBehaviourUpdater updater)
	{
		try
		{
			int callbackCount = updater.GetCallbackCount(typeof(ISceneLoadDone));
			for (int i = 0; i < callbackCount; i++)
			{
				SimulationBehaviour head;
				using (updater.GetCallbackHead(typeof(ISceneLoadDone), i, out head))
				{
					while (BehaviourUtils.IsNotNull(head))
					{
						SimulationBehaviour next = head.Next;
						if (head.CanReceiveCallback)
						{
							((ISceneLoadDone)head).SceneLoadDone();
						}
						head = next;
					}
				}
			}
		}
		catch (Exception exn)
		{
			Log.Exception(exn);
		}
	}

	public static void ISceneLoadStart(SimulationBehaviourUpdater updater)
	{
		try
		{
			int callbackCount = updater.GetCallbackCount(typeof(ISceneLoadStart));
			for (int i = 0; i < callbackCount; i++)
			{
				SimulationBehaviour head;
				using (updater.GetCallbackHead(typeof(ISceneLoadStart), i, out head))
				{
					while (BehaviourUtils.IsNotNull(head))
					{
						SimulationBehaviour next = head.Next;
						if (head.CanReceiveCallback)
						{
							((ISceneLoadStart)head).SceneLoadStart();
						}
						head = next;
					}
				}
			}
		}
		catch (Exception exn)
		{
			Log.Exception(exn);
		}
	}

	public static void IAfterHostMigration(SimulationBehaviourUpdater updater)
	{
		try
		{
			int callbackCount = updater.GetCallbackCount(typeof(IAfterHostMigration));
			for (int i = 0; i < callbackCount; i++)
			{
				SimulationBehaviour head;
				using (updater.GetCallbackHead(typeof(IAfterHostMigration), i, out head))
				{
					while (BehaviourUtils.IsNotNull(head))
					{
						SimulationBehaviour next = head.Next;
						if (head.CanReceiveCallback)
						{
							((IAfterHostMigration)head).AfterHostMigration();
						}
						head = next;
					}
				}
			}
		}
		catch (Exception exn)
		{
			Log.Exception(exn);
		}
	}
}
