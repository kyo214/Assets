using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class PlayerInputActions : IInputActionCollection2, IInputActionCollection, IEnumerable<InputAction>, IEnumerable, IDisposable
{
	public struct PlayerActions(PlayerInputActions wrapper)
	{
		private PlayerInputActions m_Wrapper = wrapper;

		public InputAction Look => m_Wrapper.m_Player_Look;

		public InputAction Move => m_Wrapper.m_Player_Move;

		public InputAction Interact => m_Wrapper.m_Player_Interact;

		public InputAction ShootAttack => m_Wrapper.m_Player_ShootAttack;

		public InputAction Reload => m_Wrapper.m_Player_Reload;

		public InputAction AimMode => m_Wrapper.m_Player_AimMode;

		public InputAction RotateLeftCam => m_Wrapper.m_Player_RotateLeftCam;

		public InputAction RotateRightCam => m_Wrapper.m_Player_RotateRightCam;

		public InputAction Inventory => m_Wrapper.m_Player_Inventory;

		public InputAction Map => m_Wrapper.m_Player_Map;

		public InputAction Menu => m_Wrapper.m_Player_Menu;

		public InputAction Skip => m_Wrapper.m_Player_Skip;

		public InputAction TabKill => m_Wrapper.m_Player_TabKill;

		public InputAction ZoomInCam => m_Wrapper.m_Player_ZoomInCam;

		public InputAction ZoomOutCam => m_Wrapper.m_Player_ZoomOutCam;

		public InputAction TiltUpCam => m_Wrapper.m_Player_TiltUpCam;

		public InputAction TiltDownCam => m_Wrapper.m_Player_TiltDownCam;

		public InputAction ChangeWeapon1 => m_Wrapper.m_Player_ChangeWeapon1;

		public InputAction ChangeWeapon2 => m_Wrapper.m_Player_ChangeWeapon2;

		public InputAction Throw => m_Wrapper.m_Player_Throw;

		public InputAction Heal => m_Wrapper.m_Player_Heal;

		public InputAction Dash => m_Wrapper.m_Player_Dash;

		public InputAction BossChangeAnim => m_Wrapper.m_Player_BossChangeAnim;

		public InputAction Run => m_Wrapper.m_Player_Run;

		public InputAction ZoomOutMap => m_Wrapper.m_Player_ZoomOutMap;

		public InputAction ZoomInMap => m_Wrapper.m_Player_ZoomInMap;

		public InputAction ChatWheel => m_Wrapper.m_Player_ChatWheel;

		public InputAction Debug => m_Wrapper.m_Player_Debug;

		public InputAction Debug1 => m_Wrapper.m_Player_Debug1;

		public InputAction Debug2 => m_Wrapper.m_Player_Debug2;

		public InputAction Debug4 => m_Wrapper.m_Player_Debug4;

		public InputAction Debug5 => m_Wrapper.m_Player_Debug5;

		public InputAction Debug6 => m_Wrapper.m_Player_Debug6;

		public InputAction Debug7 => m_Wrapper.m_Player_Debug7;

		public InputAction Copy => m_Wrapper.m_Player_Copy;

		public InputAction VoiceChat => m_Wrapper.m_Player_VoiceChat;

		public InputAction ChangeRangeWeapon => m_Wrapper.m_Player_ChangeRangeWeapon;

		public InputAction NavigateUI => m_Wrapper.m_Player_NavigateUI;

		public InputAction ShowCode => m_Wrapper.m_Player_ShowCode;

		public InputAction DropItem => m_Wrapper.m_Player_DropItem;

		public InputAction CombineItem => m_Wrapper.m_Player_CombineItem;

		public bool enabled => Get().enabled;

		public InputActionMap Get()
		{
			return m_Wrapper.m_Player;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(PlayerActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IPlayerActions instance)
		{
			if (instance != null && !m_Wrapper.m_PlayerActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_PlayerActionsCallbackInterfaces.Add(instance);
				Look.started += instance.OnLook;
				Look.performed += instance.OnLook;
				Look.canceled += instance.OnLook;
				Move.started += instance.OnMove;
				Move.performed += instance.OnMove;
				Move.canceled += instance.OnMove;
				Interact.started += instance.OnInteract;
				Interact.performed += instance.OnInteract;
				Interact.canceled += instance.OnInteract;
				ShootAttack.started += instance.OnShootAttack;
				ShootAttack.performed += instance.OnShootAttack;
				ShootAttack.canceled += instance.OnShootAttack;
				Reload.started += instance.OnReload;
				Reload.performed += instance.OnReload;
				Reload.canceled += instance.OnReload;
				AimMode.started += instance.OnAimMode;
				AimMode.performed += instance.OnAimMode;
				AimMode.canceled += instance.OnAimMode;
				RotateLeftCam.started += instance.OnRotateLeftCam;
				RotateLeftCam.performed += instance.OnRotateLeftCam;
				RotateLeftCam.canceled += instance.OnRotateLeftCam;
				RotateRightCam.started += instance.OnRotateRightCam;
				RotateRightCam.performed += instance.OnRotateRightCam;
				RotateRightCam.canceled += instance.OnRotateRightCam;
				Inventory.started += instance.OnInventory;
				Inventory.performed += instance.OnInventory;
				Inventory.canceled += instance.OnInventory;
				Map.started += instance.OnMap;
				Map.performed += instance.OnMap;
				Map.canceled += instance.OnMap;
				Menu.started += instance.OnMenu;
				Menu.performed += instance.OnMenu;
				Menu.canceled += instance.OnMenu;
				Skip.started += instance.OnSkip;
				Skip.performed += instance.OnSkip;
				Skip.canceled += instance.OnSkip;
				TabKill.started += instance.OnTabKill;
				TabKill.performed += instance.OnTabKill;
				TabKill.canceled += instance.OnTabKill;
				ZoomInCam.started += instance.OnZoomInCam;
				ZoomInCam.performed += instance.OnZoomInCam;
				ZoomInCam.canceled += instance.OnZoomInCam;
				ZoomOutCam.started += instance.OnZoomOutCam;
				ZoomOutCam.performed += instance.OnZoomOutCam;
				ZoomOutCam.canceled += instance.OnZoomOutCam;
				TiltUpCam.started += instance.OnTiltUpCam;
				TiltUpCam.performed += instance.OnTiltUpCam;
				TiltUpCam.canceled += instance.OnTiltUpCam;
				TiltDownCam.started += instance.OnTiltDownCam;
				TiltDownCam.performed += instance.OnTiltDownCam;
				TiltDownCam.canceled += instance.OnTiltDownCam;
				ChangeWeapon1.started += instance.OnChangeWeapon1;
				ChangeWeapon1.performed += instance.OnChangeWeapon1;
				ChangeWeapon1.canceled += instance.OnChangeWeapon1;
				ChangeWeapon2.started += instance.OnChangeWeapon2;
				ChangeWeapon2.performed += instance.OnChangeWeapon2;
				ChangeWeapon2.canceled += instance.OnChangeWeapon2;
				Throw.started += instance.OnThrow;
				Throw.performed += instance.OnThrow;
				Throw.canceled += instance.OnThrow;
				Heal.started += instance.OnHeal;
				Heal.performed += instance.OnHeal;
				Heal.canceled += instance.OnHeal;
				Dash.started += instance.OnDash;
				Dash.performed += instance.OnDash;
				Dash.canceled += instance.OnDash;
				BossChangeAnim.started += instance.OnBossChangeAnim;
				BossChangeAnim.performed += instance.OnBossChangeAnim;
				BossChangeAnim.canceled += instance.OnBossChangeAnim;
				Run.started += instance.OnRun;
				Run.performed += instance.OnRun;
				Run.canceled += instance.OnRun;
				ZoomOutMap.started += instance.OnZoomOutMap;
				ZoomOutMap.performed += instance.OnZoomOutMap;
				ZoomOutMap.canceled += instance.OnZoomOutMap;
				ZoomInMap.started += instance.OnZoomInMap;
				ZoomInMap.performed += instance.OnZoomInMap;
				ZoomInMap.canceled += instance.OnZoomInMap;
				ChatWheel.started += instance.OnChatWheel;
				ChatWheel.performed += instance.OnChatWheel;
				ChatWheel.canceled += instance.OnChatWheel;
				Debug.started += instance.OnDebug;
				Debug.performed += instance.OnDebug;
				Debug.canceled += instance.OnDebug;
				Debug1.started += instance.OnDebug1;
				Debug1.performed += instance.OnDebug1;
				Debug1.canceled += instance.OnDebug1;
				Debug2.started += instance.OnDebug2;
				Debug2.performed += instance.OnDebug2;
				Debug2.canceled += instance.OnDebug2;
				Debug4.started += instance.OnDebug4;
				Debug4.performed += instance.OnDebug4;
				Debug4.canceled += instance.OnDebug4;
				Debug5.started += instance.OnDebug5;
				Debug5.performed += instance.OnDebug5;
				Debug5.canceled += instance.OnDebug5;
				Debug6.started += instance.OnDebug6;
				Debug6.performed += instance.OnDebug6;
				Debug6.canceled += instance.OnDebug6;
				Debug7.started += instance.OnDebug7;
				Debug7.performed += instance.OnDebug7;
				Debug7.canceled += instance.OnDebug7;
				Copy.started += instance.OnCopy;
				Copy.performed += instance.OnCopy;
				Copy.canceled += instance.OnCopy;
				VoiceChat.started += instance.OnVoiceChat;
				VoiceChat.performed += instance.OnVoiceChat;
				VoiceChat.canceled += instance.OnVoiceChat;
				ChangeRangeWeapon.started += instance.OnChangeRangeWeapon;
				ChangeRangeWeapon.performed += instance.OnChangeRangeWeapon;
				ChangeRangeWeapon.canceled += instance.OnChangeRangeWeapon;
				NavigateUI.started += instance.OnNavigateUI;
				NavigateUI.performed += instance.OnNavigateUI;
				NavigateUI.canceled += instance.OnNavigateUI;
				ShowCode.started += instance.OnShowCode;
				ShowCode.performed += instance.OnShowCode;
				ShowCode.canceled += instance.OnShowCode;
				DropItem.started += instance.OnDropItem;
				DropItem.performed += instance.OnDropItem;
				DropItem.canceled += instance.OnDropItem;
				CombineItem.started += instance.OnCombineItem;
				CombineItem.performed += instance.OnCombineItem;
				CombineItem.canceled += instance.OnCombineItem;
			}
		}

		private void UnregisterCallbacks(IPlayerActions instance)
		{
			Look.started -= instance.OnLook;
			Look.performed -= instance.OnLook;
			Look.canceled -= instance.OnLook;
			Move.started -= instance.OnMove;
			Move.performed -= instance.OnMove;
			Move.canceled -= instance.OnMove;
			Interact.started -= instance.OnInteract;
			Interact.performed -= instance.OnInteract;
			Interact.canceled -= instance.OnInteract;
			ShootAttack.started -= instance.OnShootAttack;
			ShootAttack.performed -= instance.OnShootAttack;
			ShootAttack.canceled -= instance.OnShootAttack;
			Reload.started -= instance.OnReload;
			Reload.performed -= instance.OnReload;
			Reload.canceled -= instance.OnReload;
			AimMode.started -= instance.OnAimMode;
			AimMode.performed -= instance.OnAimMode;
			AimMode.canceled -= instance.OnAimMode;
			RotateLeftCam.started -= instance.OnRotateLeftCam;
			RotateLeftCam.performed -= instance.OnRotateLeftCam;
			RotateLeftCam.canceled -= instance.OnRotateLeftCam;
			RotateRightCam.started -= instance.OnRotateRightCam;
			RotateRightCam.performed -= instance.OnRotateRightCam;
			RotateRightCam.canceled -= instance.OnRotateRightCam;
			Inventory.started -= instance.OnInventory;
			Inventory.performed -= instance.OnInventory;
			Inventory.canceled -= instance.OnInventory;
			Map.started -= instance.OnMap;
			Map.performed -= instance.OnMap;
			Map.canceled -= instance.OnMap;
			Menu.started -= instance.OnMenu;
			Menu.performed -= instance.OnMenu;
			Menu.canceled -= instance.OnMenu;
			Skip.started -= instance.OnSkip;
			Skip.performed -= instance.OnSkip;
			Skip.canceled -= instance.OnSkip;
			TabKill.started -= instance.OnTabKill;
			TabKill.performed -= instance.OnTabKill;
			TabKill.canceled -= instance.OnTabKill;
			ZoomInCam.started -= instance.OnZoomInCam;
			ZoomInCam.performed -= instance.OnZoomInCam;
			ZoomInCam.canceled -= instance.OnZoomInCam;
			ZoomOutCam.started -= instance.OnZoomOutCam;
			ZoomOutCam.performed -= instance.OnZoomOutCam;
			ZoomOutCam.canceled -= instance.OnZoomOutCam;
			TiltUpCam.started -= instance.OnTiltUpCam;
			TiltUpCam.performed -= instance.OnTiltUpCam;
			TiltUpCam.canceled -= instance.OnTiltUpCam;
			TiltDownCam.started -= instance.OnTiltDownCam;
			TiltDownCam.performed -= instance.OnTiltDownCam;
			TiltDownCam.canceled -= instance.OnTiltDownCam;
			ChangeWeapon1.started -= instance.OnChangeWeapon1;
			ChangeWeapon1.performed -= instance.OnChangeWeapon1;
			ChangeWeapon1.canceled -= instance.OnChangeWeapon1;
			ChangeWeapon2.started -= instance.OnChangeWeapon2;
			ChangeWeapon2.performed -= instance.OnChangeWeapon2;
			ChangeWeapon2.canceled -= instance.OnChangeWeapon2;
			Throw.started -= instance.OnThrow;
			Throw.performed -= instance.OnThrow;
			Throw.canceled -= instance.OnThrow;
			Heal.started -= instance.OnHeal;
			Heal.performed -= instance.OnHeal;
			Heal.canceled -= instance.OnHeal;
			Dash.started -= instance.OnDash;
			Dash.performed -= instance.OnDash;
			Dash.canceled -= instance.OnDash;
			BossChangeAnim.started -= instance.OnBossChangeAnim;
			BossChangeAnim.performed -= instance.OnBossChangeAnim;
			BossChangeAnim.canceled -= instance.OnBossChangeAnim;
			Run.started -= instance.OnRun;
			Run.performed -= instance.OnRun;
			Run.canceled -= instance.OnRun;
			ZoomOutMap.started -= instance.OnZoomOutMap;
			ZoomOutMap.performed -= instance.OnZoomOutMap;
			ZoomOutMap.canceled -= instance.OnZoomOutMap;
			ZoomInMap.started -= instance.OnZoomInMap;
			ZoomInMap.performed -= instance.OnZoomInMap;
			ZoomInMap.canceled -= instance.OnZoomInMap;
			ChatWheel.started -= instance.OnChatWheel;
			ChatWheel.performed -= instance.OnChatWheel;
			ChatWheel.canceled -= instance.OnChatWheel;
			Debug.started -= instance.OnDebug;
			Debug.performed -= instance.OnDebug;
			Debug.canceled -= instance.OnDebug;
			Debug1.started -= instance.OnDebug1;
			Debug1.performed -= instance.OnDebug1;
			Debug1.canceled -= instance.OnDebug1;
			Debug2.started -= instance.OnDebug2;
			Debug2.performed -= instance.OnDebug2;
			Debug2.canceled -= instance.OnDebug2;
			Debug4.started -= instance.OnDebug4;
			Debug4.performed -= instance.OnDebug4;
			Debug4.canceled -= instance.OnDebug4;
			Debug5.started -= instance.OnDebug5;
			Debug5.performed -= instance.OnDebug5;
			Debug5.canceled -= instance.OnDebug5;
			Debug6.started -= instance.OnDebug6;
			Debug6.performed -= instance.OnDebug6;
			Debug6.canceled -= instance.OnDebug6;
			Debug7.started -= instance.OnDebug7;
			Debug7.performed -= instance.OnDebug7;
			Debug7.canceled -= instance.OnDebug7;
			Copy.started -= instance.OnCopy;
			Copy.performed -= instance.OnCopy;
			Copy.canceled -= instance.OnCopy;
			VoiceChat.started -= instance.OnVoiceChat;
			VoiceChat.performed -= instance.OnVoiceChat;
			VoiceChat.canceled -= instance.OnVoiceChat;
			ChangeRangeWeapon.started -= instance.OnChangeRangeWeapon;
			ChangeRangeWeapon.performed -= instance.OnChangeRangeWeapon;
			ChangeRangeWeapon.canceled -= instance.OnChangeRangeWeapon;
			NavigateUI.started -= instance.OnNavigateUI;
			NavigateUI.performed -= instance.OnNavigateUI;
			NavigateUI.canceled -= instance.OnNavigateUI;
			ShowCode.started -= instance.OnShowCode;
			ShowCode.performed -= instance.OnShowCode;
			ShowCode.canceled -= instance.OnShowCode;
			DropItem.started -= instance.OnDropItem;
			DropItem.performed -= instance.OnDropItem;
			DropItem.canceled -= instance.OnDropItem;
			CombineItem.started -= instance.OnCombineItem;
			CombineItem.performed -= instance.OnCombineItem;
			CombineItem.canceled -= instance.OnCombineItem;
		}

		public void RemoveCallbacks(IPlayerActions instance)
		{
			if (m_Wrapper.m_PlayerActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IPlayerActions instance)
		{
			foreach (IPlayerActions playerActionsCallbackInterface in m_Wrapper.m_PlayerActionsCallbackInterfaces)
			{
				UnregisterCallbacks(playerActionsCallbackInterface);
			}
			m_Wrapper.m_PlayerActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct UIActions(PlayerInputActions wrapper)
	{
		private PlayerInputActions m_Wrapper = wrapper;

		public InputAction Navigate => m_Wrapper.m_UI_Navigate;

		public InputAction Submit => m_Wrapper.m_UI_Submit;

		public InputAction Cancel => m_Wrapper.m_UI_Cancel;

		public InputAction Point => m_Wrapper.m_UI_Point;

		public InputAction Click => m_Wrapper.m_UI_Click;

		public InputAction ScrollWheel => m_Wrapper.m_UI_ScrollWheel;

		public InputAction MiddleClick => m_Wrapper.m_UI_MiddleClick;

		public InputAction RightClick => m_Wrapper.m_UI_RightClick;

		public InputAction TrackedDevicePosition => m_Wrapper.m_UI_TrackedDevicePosition;

		public InputAction TrackedDeviceOrientation => m_Wrapper.m_UI_TrackedDeviceOrientation;

		public InputAction LeftTab => m_Wrapper.m_UI_LeftTab;

		public InputAction RightTab => m_Wrapper.m_UI_RightTab;

		public InputAction Menu => m_Wrapper.m_UI_Menu;

		public InputAction Delete => m_Wrapper.m_UI_Delete;

		public InputAction Copy => m_Wrapper.m_UI_Copy;

		public InputAction ShowCode => m_Wrapper.m_UI_ShowCode;

		public InputAction ChangeLanguage => m_Wrapper.m_UI_ChangeLanguage;

		public InputAction Leaderboard => m_Wrapper.m_UI_Leaderboard;

		public bool enabled => Get().enabled;

		public InputActionMap Get()
		{
			return m_Wrapper.m_UI;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(UIActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IUIActions instance)
		{
			if (instance != null && !m_Wrapper.m_UIActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_UIActionsCallbackInterfaces.Add(instance);
				Navigate.started += instance.OnNavigate;
				Navigate.performed += instance.OnNavigate;
				Navigate.canceled += instance.OnNavigate;
				Submit.started += instance.OnSubmit;
				Submit.performed += instance.OnSubmit;
				Submit.canceled += instance.OnSubmit;
				Cancel.started += instance.OnCancel;
				Cancel.performed += instance.OnCancel;
				Cancel.canceled += instance.OnCancel;
				Point.started += instance.OnPoint;
				Point.performed += instance.OnPoint;
				Point.canceled += instance.OnPoint;
				Click.started += instance.OnClick;
				Click.performed += instance.OnClick;
				Click.canceled += instance.OnClick;
				ScrollWheel.started += instance.OnScrollWheel;
				ScrollWheel.performed += instance.OnScrollWheel;
				ScrollWheel.canceled += instance.OnScrollWheel;
				MiddleClick.started += instance.OnMiddleClick;
				MiddleClick.performed += instance.OnMiddleClick;
				MiddleClick.canceled += instance.OnMiddleClick;
				RightClick.started += instance.OnRightClick;
				RightClick.performed += instance.OnRightClick;
				RightClick.canceled += instance.OnRightClick;
				TrackedDevicePosition.started += instance.OnTrackedDevicePosition;
				TrackedDevicePosition.performed += instance.OnTrackedDevicePosition;
				TrackedDevicePosition.canceled += instance.OnTrackedDevicePosition;
				TrackedDeviceOrientation.started += instance.OnTrackedDeviceOrientation;
				TrackedDeviceOrientation.performed += instance.OnTrackedDeviceOrientation;
				TrackedDeviceOrientation.canceled += instance.OnTrackedDeviceOrientation;
				LeftTab.started += instance.OnLeftTab;
				LeftTab.performed += instance.OnLeftTab;
				LeftTab.canceled += instance.OnLeftTab;
				RightTab.started += instance.OnRightTab;
				RightTab.performed += instance.OnRightTab;
				RightTab.canceled += instance.OnRightTab;
				Menu.started += instance.OnMenu;
				Menu.performed += instance.OnMenu;
				Menu.canceled += instance.OnMenu;
				Delete.started += instance.OnDelete;
				Delete.performed += instance.OnDelete;
				Delete.canceled += instance.OnDelete;
				Copy.started += instance.OnCopy;
				Copy.performed += instance.OnCopy;
				Copy.canceled += instance.OnCopy;
				ShowCode.started += instance.OnShowCode;
				ShowCode.performed += instance.OnShowCode;
				ShowCode.canceled += instance.OnShowCode;
				ChangeLanguage.started += instance.OnChangeLanguage;
				ChangeLanguage.performed += instance.OnChangeLanguage;
				ChangeLanguage.canceled += instance.OnChangeLanguage;
				Leaderboard.started += instance.OnLeaderboard;
				Leaderboard.performed += instance.OnLeaderboard;
				Leaderboard.canceled += instance.OnLeaderboard;
			}
		}

		private void UnregisterCallbacks(IUIActions instance)
		{
			Navigate.started -= instance.OnNavigate;
			Navigate.performed -= instance.OnNavigate;
			Navigate.canceled -= instance.OnNavigate;
			Submit.started -= instance.OnSubmit;
			Submit.performed -= instance.OnSubmit;
			Submit.canceled -= instance.OnSubmit;
			Cancel.started -= instance.OnCancel;
			Cancel.performed -= instance.OnCancel;
			Cancel.canceled -= instance.OnCancel;
			Point.started -= instance.OnPoint;
			Point.performed -= instance.OnPoint;
			Point.canceled -= instance.OnPoint;
			Click.started -= instance.OnClick;
			Click.performed -= instance.OnClick;
			Click.canceled -= instance.OnClick;
			ScrollWheel.started -= instance.OnScrollWheel;
			ScrollWheel.performed -= instance.OnScrollWheel;
			ScrollWheel.canceled -= instance.OnScrollWheel;
			MiddleClick.started -= instance.OnMiddleClick;
			MiddleClick.performed -= instance.OnMiddleClick;
			MiddleClick.canceled -= instance.OnMiddleClick;
			RightClick.started -= instance.OnRightClick;
			RightClick.performed -= instance.OnRightClick;
			RightClick.canceled -= instance.OnRightClick;
			TrackedDevicePosition.started -= instance.OnTrackedDevicePosition;
			TrackedDevicePosition.performed -= instance.OnTrackedDevicePosition;
			TrackedDevicePosition.canceled -= instance.OnTrackedDevicePosition;
			TrackedDeviceOrientation.started -= instance.OnTrackedDeviceOrientation;
			TrackedDeviceOrientation.performed -= instance.OnTrackedDeviceOrientation;
			TrackedDeviceOrientation.canceled -= instance.OnTrackedDeviceOrientation;
			LeftTab.started -= instance.OnLeftTab;
			LeftTab.performed -= instance.OnLeftTab;
			LeftTab.canceled -= instance.OnLeftTab;
			RightTab.started -= instance.OnRightTab;
			RightTab.performed -= instance.OnRightTab;
			RightTab.canceled -= instance.OnRightTab;
			Menu.started -= instance.OnMenu;
			Menu.performed -= instance.OnMenu;
			Menu.canceled -= instance.OnMenu;
			Delete.started -= instance.OnDelete;
			Delete.performed -= instance.OnDelete;
			Delete.canceled -= instance.OnDelete;
			Copy.started -= instance.OnCopy;
			Copy.performed -= instance.OnCopy;
			Copy.canceled -= instance.OnCopy;
			ShowCode.started -= instance.OnShowCode;
			ShowCode.performed -= instance.OnShowCode;
			ShowCode.canceled -= instance.OnShowCode;
			ChangeLanguage.started -= instance.OnChangeLanguage;
			ChangeLanguage.performed -= instance.OnChangeLanguage;
			ChangeLanguage.canceled -= instance.OnChangeLanguage;
			Leaderboard.started -= instance.OnLeaderboard;
			Leaderboard.performed -= instance.OnLeaderboard;
			Leaderboard.canceled -= instance.OnLeaderboard;
		}

		public void RemoveCallbacks(IUIActions instance)
		{
			if (m_Wrapper.m_UIActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IUIActions instance)
		{
			foreach (IUIActions uIActionsCallbackInterface in m_Wrapper.m_UIActionsCallbackInterfaces)
			{
				UnregisterCallbacks(uIActionsCallbackInterface);
			}
			m_Wrapper.m_UIActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct InventoryUIActions(PlayerInputActions wrapper)
	{
		private PlayerInputActions m_Wrapper = wrapper;

		public InputAction LeftTab => m_Wrapper.m_InventoryUI_LeftTab;

		public InputAction CloseInteract => m_Wrapper.m_InventoryUI_CloseInteract;

		public InputAction RightTab => m_Wrapper.m_InventoryUI_RightTab;

		public InputAction SkillDescription => m_Wrapper.m_InventoryUI_SkillDescription;

		public InputAction Back => m_Wrapper.m_InventoryUI_Back;

		public bool enabled => Get().enabled;

		public InputActionMap Get()
		{
			return m_Wrapper.m_InventoryUI;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(InventoryUIActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IInventoryUIActions instance)
		{
			if (instance != null && !m_Wrapper.m_InventoryUIActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_InventoryUIActionsCallbackInterfaces.Add(instance);
				LeftTab.started += instance.OnLeftTab;
				LeftTab.performed += instance.OnLeftTab;
				LeftTab.canceled += instance.OnLeftTab;
				CloseInteract.started += instance.OnCloseInteract;
				CloseInteract.performed += instance.OnCloseInteract;
				CloseInteract.canceled += instance.OnCloseInteract;
				RightTab.started += instance.OnRightTab;
				RightTab.performed += instance.OnRightTab;
				RightTab.canceled += instance.OnRightTab;
				SkillDescription.started += instance.OnSkillDescription;
				SkillDescription.performed += instance.OnSkillDescription;
				SkillDescription.canceled += instance.OnSkillDescription;
				Back.started += instance.OnBack;
				Back.performed += instance.OnBack;
				Back.canceled += instance.OnBack;
			}
		}

		private void UnregisterCallbacks(IInventoryUIActions instance)
		{
			LeftTab.started -= instance.OnLeftTab;
			LeftTab.performed -= instance.OnLeftTab;
			LeftTab.canceled -= instance.OnLeftTab;
			CloseInteract.started -= instance.OnCloseInteract;
			CloseInteract.performed -= instance.OnCloseInteract;
			CloseInteract.canceled -= instance.OnCloseInteract;
			RightTab.started -= instance.OnRightTab;
			RightTab.performed -= instance.OnRightTab;
			RightTab.canceled -= instance.OnRightTab;
			SkillDescription.started -= instance.OnSkillDescription;
			SkillDescription.performed -= instance.OnSkillDescription;
			SkillDescription.canceled -= instance.OnSkillDescription;
			Back.started -= instance.OnBack;
			Back.performed -= instance.OnBack;
			Back.canceled -= instance.OnBack;
		}

		public void RemoveCallbacks(IInventoryUIActions instance)
		{
			if (m_Wrapper.m_InventoryUIActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IInventoryUIActions instance)
		{
			foreach (IInventoryUIActions inventoryUIActionsCallbackInterface in m_Wrapper.m_InventoryUIActionsCallbackInterfaces)
			{
				UnregisterCallbacks(inventoryUIActionsCallbackInterface);
			}
			m_Wrapper.m_InventoryUIActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct CharacterCustomizeActions(PlayerInputActions wrapper)
	{
		private PlayerInputActions m_Wrapper = wrapper;

		public InputAction RotateLeft => m_Wrapper.m_CharacterCustomize_RotateLeft;

		public InputAction RotateRight => m_Wrapper.m_CharacterCustomize_RotateRight;

		public InputAction Submit => m_Wrapper.m_CharacterCustomize_Submit;

		public InputAction Back => m_Wrapper.m_CharacterCustomize_Back;

		public bool enabled => Get().enabled;

		public InputActionMap Get()
		{
			return m_Wrapper.m_CharacterCustomize;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(CharacterCustomizeActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(ICharacterCustomizeActions instance)
		{
			if (instance != null && !m_Wrapper.m_CharacterCustomizeActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_CharacterCustomizeActionsCallbackInterfaces.Add(instance);
				RotateLeft.started += instance.OnRotateLeft;
				RotateLeft.performed += instance.OnRotateLeft;
				RotateLeft.canceled += instance.OnRotateLeft;
				RotateRight.started += instance.OnRotateRight;
				RotateRight.performed += instance.OnRotateRight;
				RotateRight.canceled += instance.OnRotateRight;
				Submit.started += instance.OnSubmit;
				Submit.performed += instance.OnSubmit;
				Submit.canceled += instance.OnSubmit;
				Back.started += instance.OnBack;
				Back.performed += instance.OnBack;
				Back.canceled += instance.OnBack;
			}
		}

		private void UnregisterCallbacks(ICharacterCustomizeActions instance)
		{
			RotateLeft.started -= instance.OnRotateLeft;
			RotateLeft.performed -= instance.OnRotateLeft;
			RotateLeft.canceled -= instance.OnRotateLeft;
			RotateRight.started -= instance.OnRotateRight;
			RotateRight.performed -= instance.OnRotateRight;
			RotateRight.canceled -= instance.OnRotateRight;
			Submit.started -= instance.OnSubmit;
			Submit.performed -= instance.OnSubmit;
			Submit.canceled -= instance.OnSubmit;
			Back.started -= instance.OnBack;
			Back.performed -= instance.OnBack;
			Back.canceled -= instance.OnBack;
		}

		public void RemoveCallbacks(ICharacterCustomizeActions instance)
		{
			if (m_Wrapper.m_CharacterCustomizeActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(ICharacterCustomizeActions instance)
		{
			foreach (ICharacterCustomizeActions characterCustomizeActionsCallbackInterface in m_Wrapper.m_CharacterCustomizeActionsCallbackInterfaces)
			{
				UnregisterCallbacks(characterCustomizeActionsCallbackInterface);
			}
			m_Wrapper.m_CharacterCustomizeActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct SkillDescriptionActions(PlayerInputActions wrapper)
	{
		private PlayerInputActions m_Wrapper = wrapper;

		public InputAction Back => m_Wrapper.m_SkillDescription_Back;

		public bool enabled => Get().enabled;

		public InputActionMap Get()
		{
			return m_Wrapper.m_SkillDescription;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(SkillDescriptionActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(ISkillDescriptionActions instance)
		{
			if (instance != null && !m_Wrapper.m_SkillDescriptionActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_SkillDescriptionActionsCallbackInterfaces.Add(instance);
				Back.started += instance.OnBack;
				Back.performed += instance.OnBack;
				Back.canceled += instance.OnBack;
			}
		}

		private void UnregisterCallbacks(ISkillDescriptionActions instance)
		{
			Back.started -= instance.OnBack;
			Back.performed -= instance.OnBack;
			Back.canceled -= instance.OnBack;
		}

		public void RemoveCallbacks(ISkillDescriptionActions instance)
		{
			if (m_Wrapper.m_SkillDescriptionActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(ISkillDescriptionActions instance)
		{
			foreach (ISkillDescriptionActions skillDescriptionActionsCallbackInterface in m_Wrapper.m_SkillDescriptionActionsCallbackInterfaces)
			{
				UnregisterCallbacks(skillDescriptionActionsCallbackInterface);
			}
			m_Wrapper.m_SkillDescriptionActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public interface IPlayerActions
	{
		void OnLook(InputAction.CallbackContext context);

		void OnMove(InputAction.CallbackContext context);

		void OnInteract(InputAction.CallbackContext context);

		void OnShootAttack(InputAction.CallbackContext context);

		void OnReload(InputAction.CallbackContext context);

		void OnAimMode(InputAction.CallbackContext context);

		void OnRotateLeftCam(InputAction.CallbackContext context);

		void OnRotateRightCam(InputAction.CallbackContext context);

		void OnInventory(InputAction.CallbackContext context);

		void OnMap(InputAction.CallbackContext context);

		void OnMenu(InputAction.CallbackContext context);

		void OnSkip(InputAction.CallbackContext context);

		void OnTabKill(InputAction.CallbackContext context);

		void OnZoomInCam(InputAction.CallbackContext context);

		void OnZoomOutCam(InputAction.CallbackContext context);

		void OnTiltUpCam(InputAction.CallbackContext context);

		void OnTiltDownCam(InputAction.CallbackContext context);

		void OnChangeWeapon1(InputAction.CallbackContext context);

		void OnChangeWeapon2(InputAction.CallbackContext context);

		void OnThrow(InputAction.CallbackContext context);

		void OnHeal(InputAction.CallbackContext context);

		void OnDash(InputAction.CallbackContext context);

		void OnBossChangeAnim(InputAction.CallbackContext context);

		void OnRun(InputAction.CallbackContext context);

		void OnZoomOutMap(InputAction.CallbackContext context);

		void OnZoomInMap(InputAction.CallbackContext context);

		void OnChatWheel(InputAction.CallbackContext context);

		void OnDebug(InputAction.CallbackContext context);

		void OnDebug1(InputAction.CallbackContext context);

		void OnDebug2(InputAction.CallbackContext context);

		void OnDebug4(InputAction.CallbackContext context);

		void OnDebug5(InputAction.CallbackContext context);

		void OnDebug6(InputAction.CallbackContext context);

		void OnDebug7(InputAction.CallbackContext context);

		void OnCopy(InputAction.CallbackContext context);

		void OnVoiceChat(InputAction.CallbackContext context);

		void OnChangeRangeWeapon(InputAction.CallbackContext context);

		void OnNavigateUI(InputAction.CallbackContext context);

		void OnShowCode(InputAction.CallbackContext context);

		void OnDropItem(InputAction.CallbackContext context);

		void OnCombineItem(InputAction.CallbackContext context);
	}

	public interface IUIActions
	{
		void OnNavigate(InputAction.CallbackContext context);

		void OnSubmit(InputAction.CallbackContext context);

		void OnCancel(InputAction.CallbackContext context);

		void OnPoint(InputAction.CallbackContext context);

		void OnClick(InputAction.CallbackContext context);

		void OnScrollWheel(InputAction.CallbackContext context);

		void OnMiddleClick(InputAction.CallbackContext context);

		void OnRightClick(InputAction.CallbackContext context);

		void OnTrackedDevicePosition(InputAction.CallbackContext context);

		void OnTrackedDeviceOrientation(InputAction.CallbackContext context);

		void OnLeftTab(InputAction.CallbackContext context);

		void OnRightTab(InputAction.CallbackContext context);

		void OnMenu(InputAction.CallbackContext context);

		void OnDelete(InputAction.CallbackContext context);

		void OnCopy(InputAction.CallbackContext context);

		void OnShowCode(InputAction.CallbackContext context);

		void OnChangeLanguage(InputAction.CallbackContext context);

		void OnLeaderboard(InputAction.CallbackContext context);
	}

	public interface IInventoryUIActions
	{
		void OnLeftTab(InputAction.CallbackContext context);

		void OnCloseInteract(InputAction.CallbackContext context);

		void OnRightTab(InputAction.CallbackContext context);

		void OnSkillDescription(InputAction.CallbackContext context);

		void OnBack(InputAction.CallbackContext context);
	}

	public interface ICharacterCustomizeActions
	{
		void OnRotateLeft(InputAction.CallbackContext context);

		void OnRotateRight(InputAction.CallbackContext context);

		void OnSubmit(InputAction.CallbackContext context);

		void OnBack(InputAction.CallbackContext context);
	}

	public interface ISkillDescriptionActions
	{
		void OnBack(InputAction.CallbackContext context);
	}

	private readonly InputActionMap m_Player;

	private List<IPlayerActions> m_PlayerActionsCallbackInterfaces = new List<IPlayerActions>();

	private readonly InputAction m_Player_Look;

	private readonly InputAction m_Player_Move;

	private readonly InputAction m_Player_Interact;

	private readonly InputAction m_Player_ShootAttack;

	private readonly InputAction m_Player_Reload;

	private readonly InputAction m_Player_AimMode;

	private readonly InputAction m_Player_RotateLeftCam;

	private readonly InputAction m_Player_RotateRightCam;

	private readonly InputAction m_Player_Inventory;

	private readonly InputAction m_Player_Map;

	private readonly InputAction m_Player_Menu;

	private readonly InputAction m_Player_Skip;

	private readonly InputAction m_Player_TabKill;

	private readonly InputAction m_Player_ZoomInCam;

	private readonly InputAction m_Player_ZoomOutCam;

	private readonly InputAction m_Player_TiltUpCam;

	private readonly InputAction m_Player_TiltDownCam;

	private readonly InputAction m_Player_ChangeWeapon1;

	private readonly InputAction m_Player_ChangeWeapon2;

	private readonly InputAction m_Player_Throw;

	private readonly InputAction m_Player_Heal;

	private readonly InputAction m_Player_Dash;

	private readonly InputAction m_Player_BossChangeAnim;

	private readonly InputAction m_Player_Run;

	private readonly InputAction m_Player_ZoomOutMap;

	private readonly InputAction m_Player_ZoomInMap;

	private readonly InputAction m_Player_ChatWheel;

	private readonly InputAction m_Player_Debug;

	private readonly InputAction m_Player_Debug1;

	private readonly InputAction m_Player_Debug2;

	private readonly InputAction m_Player_Debug4;

	private readonly InputAction m_Player_Debug5;

	private readonly InputAction m_Player_Debug6;

	private readonly InputAction m_Player_Debug7;

	private readonly InputAction m_Player_Copy;

	private readonly InputAction m_Player_VoiceChat;

	private readonly InputAction m_Player_ChangeRangeWeapon;

	private readonly InputAction m_Player_NavigateUI;

	private readonly InputAction m_Player_ShowCode;

	private readonly InputAction m_Player_DropItem;

	private readonly InputAction m_Player_CombineItem;

	private readonly InputActionMap m_UI;

	private List<IUIActions> m_UIActionsCallbackInterfaces = new List<IUIActions>();

	private readonly InputAction m_UI_Navigate;

	private readonly InputAction m_UI_Submit;

	private readonly InputAction m_UI_Cancel;

	private readonly InputAction m_UI_Point;

	private readonly InputAction m_UI_Click;

	private readonly InputAction m_UI_ScrollWheel;

	private readonly InputAction m_UI_MiddleClick;

	private readonly InputAction m_UI_RightClick;

	private readonly InputAction m_UI_TrackedDevicePosition;

	private readonly InputAction m_UI_TrackedDeviceOrientation;

	private readonly InputAction m_UI_LeftTab;

	private readonly InputAction m_UI_RightTab;

	private readonly InputAction m_UI_Menu;

	private readonly InputAction m_UI_Delete;

	private readonly InputAction m_UI_Copy;

	private readonly InputAction m_UI_ShowCode;

	private readonly InputAction m_UI_ChangeLanguage;

	private readonly InputAction m_UI_Leaderboard;

	private readonly InputActionMap m_InventoryUI;

	private List<IInventoryUIActions> m_InventoryUIActionsCallbackInterfaces = new List<IInventoryUIActions>();

	private readonly InputAction m_InventoryUI_LeftTab;

	private readonly InputAction m_InventoryUI_CloseInteract;

	private readonly InputAction m_InventoryUI_RightTab;

	private readonly InputAction m_InventoryUI_SkillDescription;

	private readonly InputAction m_InventoryUI_Back;

	private readonly InputActionMap m_CharacterCustomize;

	private List<ICharacterCustomizeActions> m_CharacterCustomizeActionsCallbackInterfaces = new List<ICharacterCustomizeActions>();

	private readonly InputAction m_CharacterCustomize_RotateLeft;

	private readonly InputAction m_CharacterCustomize_RotateRight;

	private readonly InputAction m_CharacterCustomize_Submit;

	private readonly InputAction m_CharacterCustomize_Back;

	private readonly InputActionMap m_SkillDescription;

	private List<ISkillDescriptionActions> m_SkillDescriptionActionsCallbackInterfaces = new List<ISkillDescriptionActions>();

	private readonly InputAction m_SkillDescription_Back;

	private int m_KeyboardSchemeIndex = -1;

	private int m_GamepadSchemeIndex = -1;

	public InputActionAsset asset { get; }

	public InputBinding? bindingMask
	{
		get
		{
			return asset.bindingMask;
		}
		set
		{
			asset.bindingMask = value;
		}
	}

	public ReadOnlyArray<InputDevice>? devices
	{
		get
		{
			return asset.devices;
		}
		set
		{
			asset.devices = value;
		}
	}

	public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

	public IEnumerable<InputBinding> bindings => asset.bindings;

	public PlayerActions Player => new PlayerActions(this);

	public UIActions UI => new UIActions(this);

	public InventoryUIActions InventoryUI => new InventoryUIActions(this);

	public CharacterCustomizeActions CharacterCustomize => new CharacterCustomizeActions(this);

	public SkillDescriptionActions SkillDescription => new SkillDescriptionActions(this);

	public InputControlScheme KeyboardScheme
	{
		get
		{
			if (m_KeyboardSchemeIndex == -1)
			{
				m_KeyboardSchemeIndex = asset.FindControlSchemeIndex("Keyboard");
			}
			return asset.controlSchemes[m_KeyboardSchemeIndex];
		}
	}

	public InputControlScheme GamepadScheme
	{
		get
		{
			if (m_GamepadSchemeIndex == -1)
			{
				m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
			}
			return asset.controlSchemes[m_GamepadSchemeIndex];
		}
	}

	public PlayerInputActions()
	{
		asset = InputActionAsset.FromJson("{\n    \"version\": 1,\n    \"name\": \"PlayerInputActions\",\n    \"maps\": [\n        {\n            \"name\": \"Player\",\n            \"id\": \"eb076904-3165-484f-8f63-1f6147f0379f\",\n            \"actions\": [\n                {\n                    \"name\": \"Look\",\n                    \"type\": \"Value\",\n                    \"id\": \"08989f03-cc0a-4a90-9b16-886f9f5bb0bf\",\n                    \"expectedControlType\": \"Vector2\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": true\n                },\n                {\n                    \"name\": \"Move\",\n                    \"type\": \"Value\",\n                    \"id\": \"15983c28-6dd5-4a29-a8bb-dd0aa317ebea\",\n                    \"expectedControlType\": \"Vector2\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": true\n                },\n                {\n                    \"name\": \"Interact\",\n                    \"type\": \"Button\",\n                    \"id\": \"8beeb413-82c1-44b2-9537-e7a7ebb7b84c\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Shoot/Attack\",\n                    \"type\": \"Button\",\n                    \"id\": \"144a51a2-193c-4ada-b12e-84c575906106\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": true\n                },\n                {\n                    \"name\": \"Reload\",\n                    \"type\": \"Button\",\n                    \"id\": \"e7cf15e2-ad7b-4517-8612-c4081d9d56bb\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Aim Mode\",\n                    \"type\": \"Button\",\n                    \"id\": \"18528773-e791-404b-9a0b-e3009efa272c\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": true\n                },\n                {\n                    \"name\": \"RotateLeftCam\",\n                    \"type\": \"Button\",\n                    \"id\": \"484ba22f-9177-4c89-b397-bebe2a39c5c4\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"RotateRightCam\",\n                    \"type\": \"Button\",\n                    \"id\": \"e8a100c7-5ced-4a09-bc8f-f65b4e036290\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Inventory\",\n                    \"type\": \"Button\",\n                    \"id\": \"267c39f2-8804-4baf-8d7b-3ed0b67d7d18\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Map\",\n                    \"type\": \"Button\",\n                    \"id\": \"9a0c703d-e703-459f-ad36-b3f0ea1a5202\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Menu\",\n                    \"type\": \"Button\",\n                    \"id\": \"f3777b70-64c8-4433-ac6c-a4c11426ccda\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Skip\",\n                    \"type\": \"Button\",\n                    \"id\": \"94f0a47e-a3ae-4437-a85d-53002b3f29df\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"TabKill\",\n                    \"type\": \"Button\",\n                    \"id\": \"0b7c77af-4188-4d8b-ab0b-dea3c0f80a43\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"ZoomInCam\",\n                    \"type\": \"Button\",\n                    \"id\": \"dcb1c909-353a-4b1f-8789-45e70e434505\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"ZoomOutCam\",\n                    \"type\": \"Button\",\n                    \"id\": \"8368b716-c738-4fb0-9864-25f39e63f16f\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"TiltUpCam\",\n                    \"type\": \"Button\",\n                    \"id\": \"dac548fd-4c5e-46a7-8d01-b0513f002948\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"TiltDownCam\",\n                    \"type\": \"Button\",\n                    \"id\": \"0dec0a36-3ac8-466b-a7f5-b2c06dbde797\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"ChangeWeapon1\",\n                    \"type\": \"Button\",\n                    \"id\": \"873d4fef-b2a3-4134-9580-360c11fdf555\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"ChangeWeapon2\",\n                    \"type\": \"Button\",\n                    \"id\": \"a6053338-1d56-4e53-8c31-854d65428821\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Throw\",\n                    \"type\": \"Button\",\n                    \"id\": \"55a99fad-02bb-41c7-9103-026ff9912d6f\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Heal\",\n                    \"type\": \"Button\",\n                    \"id\": \"6fde49bc-1db5-43f4-a8ce-f67969a8f0ae\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Dash\",\n                    \"type\": \"Button\",\n                    \"id\": \"858f97c3-ecc9-496e-9a16-a331e205243d\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"BossChangeAnim\",\n                    \"type\": \"Button\",\n                    \"id\": \"c5692c73-d132-4704-8605-03ff89f8c982\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Run\",\n                    \"type\": \"Button\",\n                    \"id\": \"41dae869-22f2-48b7-8e14-98536f97b97f\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"ZoomOutMap\",\n                    \"type\": \"Value\",\n                    \"id\": \"05e8ff51-1a2d-4d47-aff6-2cd9b1965034\",\n                    \"expectedControlType\": \"Axis\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": true\n                },\n                {\n                    \"name\": \"ZoomInMap\",\n                    \"type\": \"Value\",\n                    \"id\": \"1e04e0b5-d158-4c3c-b9c3-4f478b5c3666\",\n                    \"expectedControlType\": \"Axis\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": true\n                },\n                {\n                    \"name\": \"ChatWheel\",\n                    \"type\": \"Button\",\n                    \"id\": \"9220a840-073e-4f95-bcd2-25519690cf5b\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Debug\",\n                    \"type\": \"Button\",\n                    \"id\": \"5a02f573-27b5-44ac-9ebe-76bd689488d3\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Debug1\",\n                    \"type\": \"Button\",\n                    \"id\": \"16fdd9b0-3441-449c-b4cd-4f3b0a72f0a4\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Debug2\",\n                    \"type\": \"Button\",\n                    \"id\": \"5b59c5e6-bf08-4c79-b06a-e82fe5efefbe\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Debug4\",\n                    \"type\": \"Button\",\n                    \"id\": \"1e4a057e-f0b2-45a7-9dff-110efc2b8f26\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Debug5\",\n                    \"type\": \"Button\",\n                    \"id\": \"f700f66f-b681-417e-af14-9e2790ef1a8b\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Debug6\",\n                    \"type\": \"Button\",\n                    \"id\": \"483be0b6-1ed9-4eb5-bc28-56b0420a0648\",\n                    \"expectedControlType\": \"\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Debug7\",\n                    \"type\": \"Button\",\n                    \"id\": \"96a2c9b3-bc70-4311-8c33-7d3d73ebcb21\",\n                    \"expectedControlType\": \"\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Copy\",\n                    \"type\": \"Button\",\n                    \"id\": \"cba646e5-2f22-4abf-a673-c83822043c13\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"VoiceChat\",\n                    \"type\": \"Button\",\n                    \"id\": \"1681b0aa-c6bd-41dc-9116-f81453dbc4b8\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Change Range Weapon\",\n                    \"type\": \"Button\",\n                    \"id\": \"943fa754-ab76-4603-9f46-2bed99326b18\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"NavigateUI\",\n                    \"type\": \"PassThrough\",\n                    \"id\": \"ddc70e66-ee4d-4d45-ba32-836d9daa2406\",\n                    \"expectedControlType\": \"Vector2\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"ShowCode\",\n                    \"type\": \"Button\",\n                    \"id\": \"155b44bc-4678-4b4a-9b91-6cfde7477722\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"DropItem\",\n                    \"type\": \"Button\",\n                    \"id\": \"2b8aa7a8-7a15-44e8-88c1-1f10d30b5d17\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"CombineItem\",\n                    \"type\": \"Button\",\n                    \"id\": \"3e874cef-76ff-457b-afc2-43b2fd91984b\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                }\n            ],\n            \"bindings\": [\n                {\n                    \"name\": \"Keyboard\",\n                    \"id\": \"26fa4f6d-e774-4958-be8d-de5f260baa60\",\n                    \"path\": \"2DVector\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Move\",\n                    \"isComposite\": true,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"up\",\n                    \"id\": \"c1af8a2c-f592-49ab-a7c8-02a68592d903\",\n                    \"path\": \"<Keyboard>/w\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"Move\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"down\",\n                    \"id\": \"5ce38674-669f-4319-b481-8b193f63a09a\",\n                    \"path\": \"<Keyboard>/s\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"Move\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"left\",\n                    \"id\": \"99f36542-4969-4604-9ecd-33004c280c82\",\n                    \"path\": \"<Keyboard>/a\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"Move\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"right\",\n                    \"id\": \"ae162e24-0882-440b-9c49-f7f5a0404fd6\",\n                    \"path\": \"<Keyboard>/d\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"Move\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"932994d9-791d-4d23-be9f-cad7fae7725b\",\n                    \"path\": \"<Gamepad>/leftStick\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"Move\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"9fc9a528-9ebe-455c-859e-707bc8a56a6b\",\n                    \"path\": \"<Keyboard>/f\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"Interact\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"f4e50049-5d6e-4cfd-9128-8bd698618490\",\n                    \"path\": \"<Gamepad>/buttonSouth\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"Interact\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"55ca5d9b-b7d9-451f-910b-32758685c2ae\",\n                    \"path\": \"<Mouse>/leftButton\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"Shoot/Attack\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"e278b187-7dbc-4e5a-8d1b-6ce22cdfbc15\",\n                    \"path\": \"<Gamepad>/rightTrigger\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"Shoot/Attack\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"ccc9810b-cbd1-41e0-91bc-0c86441f88b0\",\n                    \"path\": \"<Mouse>/rightButton\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"Aim Mode\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"5f0db215-1bec-400c-b9b8-03116fabb21e\",\n                    \"path\": \"<Gamepad>/leftTrigger\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"Aim Mode\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"b9689a87-ab10-4ac4-9e2e-acac0a539dac\",\n                    \"path\": \"<Keyboard>/q\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"RotateLeftCam\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"52d94ef2-0c7e-4fd0-a1dd-019aae1306fc\",\n                    \"path\": \"<Gamepad>/dpad/left\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"RotateLeftCam\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"ae35c901-fee5-412d-a44a-96a3464b2e68\",\n                    \"path\": \"<Keyboard>/e\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"RotateRightCam\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"b9ef5541-fa12-4a2d-8ff3-c49530a39784\",\n                    \"path\": \"<Gamepad>/dpad/right\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"RotateRightCam\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"57e65193-d0aa-446d-aae4-fa04e34ef8cd\",\n                    \"path\": \"<Keyboard>/tab\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"Inventory\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"a15cdb86-7370-4432-903f-50a2a48bfb67\",\n                    \"path\": \"<Gamepad>/buttonNorth\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"Inventory\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"350e629c-9e6b-4ff3-be05-10f01ccd3f7f\",\n                    \"path\": \"<Mouse>/position\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"Look\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"c0be0b55-4280-46ea-b680-6b792455025b\",\n                    \"path\": \"<Gamepad>/rightStick\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"Look\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"b29e584d-0108-4fbf-baac-df7dc6f7b74b\",\n                    \"path\": \"<Keyboard>/m\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"Map\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"05b996f2-2463-453d-b3db-80e628a0826e\",\n                    \"path\": \"<Gamepad>/select\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"Map\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"c97f62b1-fb6e-416a-ae1b-075b9e59f48e\",\n                    \"path\": \"<Keyboard>/escape\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"Menu\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"5317a6b1-55b7-431a-b36d-05d1f208b596\",\n                    \"path\": \"<Gamepad>/start\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"Menu\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"ae9bd334-917e-4e80-afd6-7e39980cc5d8\",\n                    \"path\": \"<Keyboard>/r\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"Reload\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"ad8dfa4b-0b6a-44d6-a251-c1753952b9cd\",\n                    \"path\": \"<Gamepad>/buttonWest\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"Reload\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"3dd004c3-cc92-4ae1-a765-7c5d09b6f10c\",\n                    \"path\": \"<Keyboard>/space\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"Skip\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"bfac0531-fd5a-4edb-bdee-210357ead0c8\",\n                    \"path\": \"<Gamepad>/buttonWest\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"Skip\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"e3ac9ae8-5dd7-491e-9ab5-518df2c58de8\",\n                    \"path\": \"<Keyboard>/f1\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"TabKill\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"737b56f9-1883-4b08-a51b-bb7bcf63256e\",\n                    \"path\": \"<Gamepad>/dpad/up\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"TabKill\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"7ceb46b0-35a3-4f32-99d4-6cfa527f251e\",\n                    \"path\": \"<Keyboard>/numpadPlus\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"ZoomInCam\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"b2437fa4-72d8-4298-bff1-50a96d686cbc\",\n                    \"path\": \"<Keyboard>/equals\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"ZoomInCam\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"e17ac123-b0fc-4589-bde0-60ef7ca9956a\",\n                    \"path\": \"<Keyboard>/minus\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"ZoomOutCam\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"30f3f78e-3703-40d7-9b62-9711961f9212\",\n                    \"path\": \"<Keyboard>/t\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"TiltUpCam\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"c4ec4313-fe52-4138-9aab-a46a1fa5da3e\",\n                    \"path\": \"<Keyboard>/y\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"TiltDownCam\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"d1733ceb-8111-43e2-896c-a635bbdd4823\",\n                    \"path\": \"<Keyboard>/1\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"ChangeWeapon1\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"13dea271-7ca5-4179-bfdc-7e6c4ddb7e9a\",\n                    \"path\": \"<Keyboard>/2\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"ChangeWeapon2\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"8bed9849-c841-46ee-bb3f-0d63c70f27a3\",\n                    \"path\": \"<Keyboard>/g\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"Throw\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"a44d1b40-a188-4a09-9ad6-6ad1752478ba\",\n                    \"path\": \"<Gamepad>/leftShoulder\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"Throw\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"341453d5-0d3f-4fdd-b2f8-28e04eb8947b\",\n                    \"path\": \"<Keyboard>/h\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"Heal\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"3cac8c42-b5ba-4afc-9564-85f4ffee5b5b\",\n                    \"path\": \"<Gamepad>/buttonEast\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"Heal\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"b2aa1359-af48-495d-bbe0-6e4b566b6b05\",\n                    \"path\": \"<Keyboard>/space\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"Dash\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"ea5c0af6-9b61-4085-8aff-59c53582f29f\",\n                    \"path\": \"<Gamepad>/rightShoulder\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"Dash\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"4c8ec80e-5a38-4a79-97ef-6e4890c1e2a4\",\n                    \"path\": \"<Keyboard>/b\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"BossChangeAnim\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"800d0c0f-cef1-473c-93d9-9b2842396a1c\",\n                    \"path\": \"<Keyboard>/leftShift\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"Run\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"37be2b5d-45bf-4f9c-9171-967892995494\",\n                    \"path\": \"<Gamepad>/leftStickPress\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"Run\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"3df79c17-1726-4eef-a12a-8bb4f1312b13\",\n                    \"path\": \"<Mouse>/scroll/down\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"ZoomOutMap\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"49847484-347a-424d-9cd0-eb30369028a0\",\n                    \"path\": \"<Gamepad>/rightStick/down\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"ZoomOutMap\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"64d8df6c-4e20-40b7-8096-f4a06a8886a2\",\n                    \"path\": \"<Mouse>/scroll/up\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"ZoomInMap\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"9c879c59-6368-4053-8b06-c8d369acb665\",\n                    \"path\": \"<Gamepad>/rightStick/up\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"ZoomInMap\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"55dc7d8f-9513-4e49-8113-ac25e39fdc34\",\n                    \"path\": \"<Keyboard>/t\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"ChatWheel\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"36b6a5aa-58b0-4bf8-a91c-0be41888b753\",\n                    \"path\": \"<Gamepad>/dpad/down\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"ChatWheel\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"62f6b810-f586-42e8-8912-20d068c5065a\",\n                    \"path\": \"<Mouse>/middleButton\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"ChatWheel\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"7baeccef-e9fc-4c67-9615-13fd8e0385a4\",\n                    \"path\": \"<Keyboard>/f2\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard;Gamepad\",\n                    \"action\": \"Debug1\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"529d6ba8-03db-4ea0-b8a9-2754a97617f0\",\n                    \"path\": \"<Keyboard>/c\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"Copy\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"15c14b8f-418f-42c1-9982-1f44ff1f2a7c\",\n                    \"path\": \"<Gamepad>/select\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"Copy\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"435ab545-f746-4298-bcac-4d7060520b01\",\n                    \"path\": \"<Keyboard>/v\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"VoiceChat\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"1040809d-4fb6-4644-a2e1-3791ec68d904\",\n                    \"path\": \"<Gamepad>/rightStickPress\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"VoiceChat\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"938c057b-d221-4653-b899-7962b07b859a\",\n                    \"path\": \"<Keyboard>/z\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard;Gamepad\",\n                    \"action\": \"Change Range Weapon\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"f22f95bf-a06a-44f7-bad6-775fd26eaf85\",\n                    \"path\": \"<Gamepad>/dpad/up\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"Change Range Weapon\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"Keyboard\",\n                    \"id\": \"ba9be275-8030-4293-88a5-9fbe140c8cf6\",\n                    \"path\": \"2DVector\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"NavigateUI\",\n                    \"isComposite\": true,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"up\",\n                    \"id\": \"9470ea92-5a04-46e2-a7bb-704151044610\",\n                    \"path\": \"<Keyboard>/upArrow\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"NavigateUI\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"up\",\n                    \"id\": \"0bb8f988-ead6-4f72-aebb-141310c7db15\",\n                    \"path\": \"<Keyboard>/w\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"NavigateUI\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"down\",\n                    \"id\": \"63851edf-c8b6-4ebd-b181-00ca7b8ee70f\",\n                    \"path\": \"<Keyboard>/downArrow\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"NavigateUI\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"down\",\n                    \"id\": \"549c155e-832e-4dde-84c3-9a2067a2de72\",\n                    \"path\": \"<Keyboard>/s\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"NavigateUI\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"left\",\n                    \"id\": \"f139b9cd-0817-41da-82c0-08f45ff7dfd7\",\n                    \"path\": \"<Keyboard>/leftArrow\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"NavigateUI\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"left\",\n                    \"id\": \"0a135580-c5ff-4ec7-9c73-d9c8c79f28eb\",\n                    \"path\": \"<Keyboard>/a\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"NavigateUI\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"right\",\n                    \"id\": \"6ace6c90-b615-4250-a626-45a6d6d95bc8\",\n                    \"path\": \"<Keyboard>/rightArrow\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"NavigateUI\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"right\",\n                    \"id\": \"8e6d7b11-8782-4b96-aa06-c4ed5b193782\",\n                    \"path\": \"<Keyboard>/d\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"NavigateUI\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"Gamepad\",\n                    \"id\": \"ce416337-65c2-4f42-a61d-b9d2f2c9d04a\",\n                    \"path\": \"2DVector\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"NavigateUI\",\n                    \"isComposite\": true,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"up\",\n                    \"id\": \"f58c5662-a662-4ca1-9f5a-abb8478dfacc\",\n                    \"path\": \"<Gamepad>/leftStick/up\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"NavigateUI\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"up\",\n                    \"id\": \"9e8dc4c8-05dc-4c28-94b4-90fa8f1a60c1\",\n                    \"path\": \"<Gamepad>/rightStick/up\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"NavigateUI\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"down\",\n                    \"id\": \"4690f601-92b6-4424-b35d-26e6e3252c67\",\n                    \"path\": \"<Gamepad>/leftStick/down\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"NavigateUI\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"down\",\n                    \"id\": \"7f41689c-ace3-455e-87ff-698f7e906e14\",\n                    \"path\": \"<Gamepad>/rightStick/down\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"NavigateUI\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"left\",\n                    \"id\": \"f1707e6c-6694-4219-957c-d69134fa9f23\",\n                    \"path\": \"<Gamepad>/leftStick/left\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"NavigateUI\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"left\",\n                    \"id\": \"c0f89802-66e7-409a-8fc9-2dee041b7ee8\",\n                    \"path\": \"<Gamepad>/rightStick/left\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"NavigateUI\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"right\",\n                    \"id\": \"a6bbc13c-ec7f-44e6-8e6c-869a55bc652f\",\n                    \"path\": \"<Gamepad>/leftStick/right\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"NavigateUI\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"right\",\n                    \"id\": \"c57787e8-e0d6-4a17-a268-96255c3592ad\",\n                    \"path\": \"<Gamepad>/rightStick/right\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"NavigateUI\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"689d6024-9759-4985-bea1-b01571189202\",\n                    \"path\": \"<Gamepad>/dpad\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"NavigateUI\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"Joystick\",\n                    \"id\": \"d473a51d-b016-4fd9-853b-ee5ef871cfd4\",\n                    \"path\": \"2DVector\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"NavigateUI\",\n                    \"isComposite\": true,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"up\",\n                    \"id\": \"16c3672a-f4ae-4dc6-b14b-32e69ff34e02\",\n                    \"path\": \"<Joystick>/stick/up\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"NavigateUI\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"down\",\n                    \"id\": \"306d929e-004a-4104-a5a3-3296c7c27f47\",\n                    \"path\": \"<Joystick>/stick/down\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"NavigateUI\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"left\",\n                    \"id\": \"10e6bcca-66af-4577-9001-0ba7ac94f27f\",\n                    \"path\": \"<Joystick>/stick/left\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"NavigateUI\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"right\",\n                    \"id\": \"a6dff359-e010-4b7f-b6bf-0667d0b022a5\",\n                    \"path\": \"<Joystick>/stick/right\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"NavigateUI\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"1cddadd4-1006-453f-9d86-c46cc72ed03a\",\n                    \"path\": \"<Keyboard>/x\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"ShowCode\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"e6533596-27ae-407e-ad9a-5ffdf6e77ad5\",\n                    \"path\": \"<Gamepad>/dpad/up\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"ShowCode\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"4d14dc39-303c-4fe5-80ac-7fea8b1eca97\",\n                    \"path\": \"<Gamepad>/leftTrigger\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"DropItem\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"bf47c433-4c65-4949-9d3e-02c13fc16114\",\n                    \"path\": \"<Gamepad>/buttonWest\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"CombineItem\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"77c1c20f-2909-48ff-9cc6-c162190df468\",\n                    \"path\": \"<Keyboard>/f1\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"Debug\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"bbde6801-6da2-4a53-b5ed-509a11fbfbc6\",\n                    \"path\": \"<Keyboard>/f3\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard;Gamepad\",\n                    \"action\": \"Debug2\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"04950217-1197-443e-a16f-a38821f39b44\",\n                    \"path\": \"<Keyboard>/f5\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard;Gamepad\",\n                    \"action\": \"Debug4\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"ec0bb909-5436-4c36-9d6a-2eca38a0c7c7\",\n                    \"path\": \"<Keyboard>/f6\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard;Gamepad\",\n                    \"action\": \"Debug5\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"31072e07-32a3-4f26-98a7-c898c7f8c71e\",\n                    \"path\": \"<Keyboard>/f7\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard;Gamepad\",\n                    \"action\": \"Debug6\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"074785df-91ce-4ef0-ad36-4b8f6ca052b1\",\n                    \"path\": \"<Keyboard>/f8\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard;Gamepad\",\n                    \"action\": \"Debug7\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                }\n            ]\n        },\n        {\n            \"name\": \"UI\",\n            \"id\": \"7627c8a7-a449-4303-a93c-0448586b1bfb\",\n            \"actions\": [\n                {\n                    \"name\": \"Navigate\",\n                    \"type\": \"PassThrough\",\n                    \"id\": \"7a14d402-90b0-43d4-b9d8-b2331ce58481\",\n                    \"expectedControlType\": \"Vector2\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Submit\",\n                    \"type\": \"Button\",\n                    \"id\": \"102900bb-072c-426a-afc5-696d08de8c62\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Cancel\",\n                    \"type\": \"Button\",\n                    \"id\": \"6f61bb7e-6dd1-4923-b502-46a5866dee46\",\n                    \"expectedControlType\": \"\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Point\",\n                    \"type\": \"PassThrough\",\n                    \"id\": \"c4bbeb50-9f9b-4b68-afaf-2e1d655a55a3\",\n                    \"expectedControlType\": \"Vector2\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": true\n                },\n                {\n                    \"name\": \"Click\",\n                    \"type\": \"PassThrough\",\n                    \"id\": \"0b4117de-336d-49f5-b47a-345252a726e0\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": true\n                },\n                {\n                    \"name\": \"ScrollWheel\",\n                    \"type\": \"PassThrough\",\n                    \"id\": \"9f970f54-f260-40c5-b0d4-349c7f817945\",\n                    \"expectedControlType\": \"Vector2\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"MiddleClick\",\n                    \"type\": \"PassThrough\",\n                    \"id\": \"3fbf1b3f-bce1-45b5-bad2-61b929e422d5\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"RightClick\",\n                    \"type\": \"PassThrough\",\n                    \"id\": \"54b8324b-a608-448e-a693-91ff783f6d03\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"TrackedDevicePosition\",\n                    \"type\": \"PassThrough\",\n                    \"id\": \"0f97efd3-c94d-4725-8d04-e8041dcc78b9\",\n                    \"expectedControlType\": \"Vector3\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"TrackedDeviceOrientation\",\n                    \"type\": \"PassThrough\",\n                    \"id\": \"d7a0005a-1e3f-470a-a1bb-9498a42bda14\",\n                    \"expectedControlType\": \"Quaternion\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"LeftTab\",\n                    \"type\": \"Button\",\n                    \"id\": \"cd75b9d1-3727-4816-90b2-6e1300f41a07\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"RightTab\",\n                    \"type\": \"Button\",\n                    \"id\": \"9848813b-03b9-489b-bce9-537160a56619\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Menu\",\n                    \"type\": \"Button\",\n                    \"id\": \"d2f6ea33-9fa9-4527-a5a9-847dfefed5eb\",\n                    \"expectedControlType\": \"\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Delete\",\n                    \"type\": \"Button\",\n                    \"id\": \"6af2919b-21b6-470e-b64e-8b72bfd38d8f\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Copy\",\n                    \"type\": \"Button\",\n                    \"id\": \"b0952ae8-0e52-484c-bc90-9f3e09e8b78d\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"ShowCode\",\n                    \"type\": \"Button\",\n                    \"id\": \"f3137a40-7e90-4877-bce4-43f94de58787\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"ChangeLanguage\",\n                    \"type\": \"Button\",\n                    \"id\": \"fba0791e-b03e-42ea-a928-dd5e7034f597\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Leaderboard\",\n                    \"type\": \"Button\",\n                    \"id\": \"0a766963-1e81-4443-9807-15c79725f528\",\n                    \"expectedControlType\": \"\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                }\n            ],\n            \"bindings\": [\n                {\n                    \"name\": \"Keyboard\",\n                    \"id\": \"7b389e2a-9650-4c11-ad86-f173b3ae2c41\",\n                    \"path\": \"2DVector\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": true,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"up\",\n                    \"id\": \"5b677b0f-8180-4dab-866f-9bd76f627e7b\",\n                    \"path\": \"<Keyboard>/upArrow\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"up\",\n                    \"id\": \"b4d2182d-6d73-4a60-8993-0b6c2c1eb6cf\",\n                    \"path\": \"<Keyboard>/w\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"down\",\n                    \"id\": \"9d0ad1d5-dc9b-48ab-be14-45c45ac13e9d\",\n                    \"path\": \"<Keyboard>/downArrow\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"down\",\n                    \"id\": \"8dfc4077-f5b7-45e4-a885-5a21a534c00d\",\n                    \"path\": \"<Keyboard>/s\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"left\",\n                    \"id\": \"5281305a-d916-48a1-94d4-04cdc8bf21eb\",\n                    \"path\": \"<Keyboard>/leftArrow\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"left\",\n                    \"id\": \"6d58b74b-f136-4d73-84e4-24c69d0f9089\",\n                    \"path\": \"<Keyboard>/a\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"right\",\n                    \"id\": \"9d4eba02-992f-4e32-85bf-00eba0ef0c7a\",\n                    \"path\": \"<Keyboard>/rightArrow\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"right\",\n                    \"id\": \"f4035bb4-ee68-4725-8762-c0d341a5f872\",\n                    \"path\": \"<Keyboard>/d\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"Gamepad\",\n                    \"id\": \"96a9de66-42eb-4143-a83b-b360d39406de\",\n                    \"path\": \"2DVector\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": true,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"up\",\n                    \"id\": \"b8c7f8df-8c63-417f-bb9d-7935fceb0f54\",\n                    \"path\": \"<Gamepad>/leftStick/up\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \";Gamepad\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"up\",\n                    \"id\": \"597cf861-ab2a-4156-8e85-45812c8c5d92\",\n                    \"path\": \"<Gamepad>/rightStick/up\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \";Gamepad\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"down\",\n                    \"id\": \"50d80fd1-eb4f-46e3-872c-41a400fa13da\",\n                    \"path\": \"<Gamepad>/leftStick/down\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \";Gamepad\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"down\",\n                    \"id\": \"38c965d7-9ff9-4b64-ac05-215f5da8d5b2\",\n                    \"path\": \"<Gamepad>/rightStick/down\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \";Gamepad\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"left\",\n                    \"id\": \"fe92316a-a2e0-47c0-bd5a-ce4bc66f19ce\",\n                    \"path\": \"<Gamepad>/leftStick/left\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \";Gamepad\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"left\",\n                    \"id\": \"cab806ae-d1d9-4e4f-9ca9-347129420d67\",\n                    \"path\": \"<Gamepad>/rightStick/left\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \";Gamepad\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"right\",\n                    \"id\": \"5934ef62-0c91-40ec-b51a-d4ef4301404e\",\n                    \"path\": \"<Gamepad>/leftStick/right\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \";Gamepad\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"right\",\n                    \"id\": \"1c484fef-2ed3-4f5d-9c04-3f5b64cbe72e\",\n                    \"path\": \"<Gamepad>/rightStick/right\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \";Gamepad\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"7a1a744f-5ebc-4b04-b044-8d90e7ad7b57\",\n                    \"path\": \"<Gamepad>/dpad\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \";Gamepad\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"Joystick\",\n                    \"id\": \"e72afbe2-9f12-4baf-b781-9bdad976e410\",\n                    \"path\": \"2DVector\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": true,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"up\",\n                    \"id\": \"b80229f0-ec6b-4b0b-b2ec-a7fe62df07be\",\n                    \"path\": \"<Joystick>/stick/up\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Joystick\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"down\",\n                    \"id\": \"df40f03c-1501-4f2f-a911-b3ff13fd4108\",\n                    \"path\": \"<Joystick>/stick/down\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Joystick\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"left\",\n                    \"id\": \"02729a7b-5d8f-4765-8e7e-e892fd5f456f\",\n                    \"path\": \"<Joystick>/stick/left\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Joystick\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"right\",\n                    \"id\": \"dc30c469-1433-4d1a-adbc-3530b61f5faa\",\n                    \"path\": \"<Joystick>/stick/right\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Joystick\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"f4c94ff1-1eb6-49d8-953b-6d8aa3870add\",\n                    \"path\": \"<Keyboard>/enter\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"Submit\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"85ed6e15-7b11-40c6-9479-2b7257b159a7\",\n                    \"path\": \"<Gamepad>/buttonSouth\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"Submit\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"1cc70701-b824-4fe9-84f2-5ab4d85a1907\",\n                    \"path\": \"<Keyboard>/escape\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"Cancel\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"3489e346-50b0-4b97-beee-f7cacbeabc4e\",\n                    \"path\": \"<Gamepad>/buttonEast\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"Cancel\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"f1cbc5c6-df02-4474-b1b1-98e62579cb0b\",\n                    \"path\": \"<Mouse>/position\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard&Mouse;Keyboard\",\n                    \"action\": \"Point\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"7a1246fa-fd72-41ed-a022-7633ba647863\",\n                    \"path\": \"<Pen>/position\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard&Mouse;Keyboard\",\n                    \"action\": \"Point\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"921aa299-958b-45dd-a44b-09039c0f92fd\",\n                    \"path\": \"<Touchscreen>/touch*/position\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Touch;Keyboard\",\n                    \"action\": \"Point\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"15358af8-f4fc-4a55-8b34-55d3c026bb96\",\n                    \"path\": \"<Mouse>/leftButton\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard&Mouse;Keyboard\",\n                    \"action\": \"Click\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"c3642a21-a6e0-4482-9c63-98c5ba6b5775\",\n                    \"path\": \"<Pen>/tip\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard&Mouse;Keyboard\",\n                    \"action\": \"Click\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"fa76e334-02d0-4866-a11e-c777c1a1dc24\",\n                    \"path\": \"<Touchscreen>/touch*/press\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Touch;Keyboard\",\n                    \"action\": \"Click\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"5ea1f796-d841-424f-8285-471bf0785704\",\n                    \"path\": \"<XRController>/trigger\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"XR\",\n                    \"action\": \"Click\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"4ee86f5c-4dd3-46ab-a661-b8fea8b168dc\",\n                    \"path\": \"<Mouse>/scroll\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard&Mouse;Keyboard\",\n                    \"action\": \"ScrollWheel\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"c3ea61ee-4880-4c5d-98cb-36660f48e345\",\n                    \"path\": \"<Mouse>/middleButton\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard&Mouse;Keyboard\",\n                    \"action\": \"MiddleClick\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"0bf571d2-fb0e-426b-9e59-2513764a0d56\",\n                    \"path\": \"<Mouse>/rightButton\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard&Mouse;Keyboard\",\n                    \"action\": \"RightClick\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"87bd3a64-d172-4572-b8a9-28c2c1117a4e\",\n                    \"path\": \"<XRController>/devicePosition\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"XR\",\n                    \"action\": \"TrackedDevicePosition\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"a8ce31b0-eded-4743-9ac8-e228dc5bae21\",\n                    \"path\": \"<XRController>/deviceRotation\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"XR\",\n                    \"action\": \"TrackedDeviceOrientation\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"ea1c00ca-a19c-4e09-8eea-ea3811c14ae1\",\n                    \"path\": \"<Gamepad>/leftShoulder\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"LeftTab\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"61b0847c-c9de-49dd-bb7d-50018d083970\",\n                    \"path\": \"<Gamepad>/rightShoulder\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"RightTab\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"26356464-e6ae-4df6-8f9d-86aa0891bdb8\",\n                    \"path\": \"<Keyboard>/escape\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"Menu\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"a707110e-9785-412a-8cc6-0caa7d785d9a\",\n                    \"path\": \"<Gamepad>/start\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"Menu\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"5dac0a0c-bf72-4e3f-834d-198cc9359115\",\n                    \"path\": \"<Gamepad>/buttonWest\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"Delete\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"3e32cfcf-57ff-4fe4-8fbe-8ff365a63a3d\",\n                    \"path\": \"<Keyboard>/x\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Delete\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"9f5b2389-5f55-4e90-9ed1-0df11bbddbc8\",\n                    \"path\": \"<Keyboard>/c\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"Copy\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"b1488400-4789-4e1a-88ce-06fa75f1dbcb\",\n                    \"path\": \"<Gamepad>/select\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"Copy\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"9b1ca547-2a4d-4e56-9484-63f5e5761a88\",\n                    \"path\": \"<Keyboard>/x\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"ShowCode\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"a94d96f9-2f4f-4bf4-8113-aca18dae1ae1\",\n                    \"path\": \"<Gamepad>/dpad/up\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"ShowCode\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"c2a678a4-e762-4a98-86c6-23ebb1000b3c\",\n                    \"path\": \"<Keyboard>/f3\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"ChangeLanguage\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"662ea4ac-a31a-49e0-a3ee-a5d6bb92d8a3\",\n                    \"path\": \"<Gamepad>/rightTrigger\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \";Gamepad\",\n                    \"action\": \"Leaderboard\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                }\n            ]\n        },\n        {\n            \"name\": \"InventoryUI\",\n            \"id\": \"09d5df65-95c5-409b-8cfd-9849e134bc32\",\n            \"actions\": [\n                {\n                    \"name\": \"LeftTab\",\n                    \"type\": \"Button\",\n                    \"id\": \"2623e214-d689-4b3b-85e3-49f7cd3a219a\",\n                    \"expectedControlType\": \"\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"CloseInteract\",\n                    \"type\": \"Button\",\n                    \"id\": \"5ca498cd-95fe-4e58-8757-7583d81d4c05\",\n                    \"expectedControlType\": \"\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"RightTab\",\n                    \"type\": \"Button\",\n                    \"id\": \"cd7105d5-60c0-4935-812b-2929d16ceee1\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"SkillDescription\",\n                    \"type\": \"Button\",\n                    \"id\": \"b4aed8a8-0953-465c-b89e-79134dd6443c\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Back\",\n                    \"type\": \"Button\",\n                    \"id\": \"9d44df3b-893d-4f64-83a7-13933db9d061\",\n                    \"expectedControlType\": \"\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                }\n            ],\n            \"bindings\": [\n                {\n                    \"name\": \"\",\n                    \"id\": \"721733cd-2567-4573-826d-2df31f9b381a\",\n                    \"path\": \"<Gamepad>/leftShoulder\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"LeftTab\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"fea7466a-a5a8-4d8e-b678-b10ccfa85528\",\n                    \"path\": \"<Keyboard>/f\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \";Keyboard\",\n                    \"action\": \"CloseInteract\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"b8ca1d3e-924f-477e-98c5-93c56c3ae3ac\",\n                    \"path\": \"<Keyboard>/tab\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \";Keyboard\",\n                    \"action\": \"CloseInteract\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"e63d242c-e33d-451d-b305-356208eb465b\",\n                    \"path\": \"<Gamepad>/rightShoulder\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"RightTab\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"7e757bb6-a269-447f-a829-f90300154cfe\",\n                    \"path\": \"<Gamepad>/rightStickPress\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"SkillDescription\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"177ba3c7-11d6-42c5-afca-a13dd9808761\",\n                    \"path\": \"<Keyboard>/escape\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"Back\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"1f2623e7-24f9-40f4-b04f-a48e8e2d5c36\",\n                    \"path\": \"<Keyboard>/tab\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Back\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"a6e88206-b569-4e6a-abdf-720a16d0c70e\",\n                    \"path\": \"<Keyboard>/f\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Back\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"8b228ed3-c6ff-4168-86e7-92118ced56e3\",\n                    \"path\": \"<Gamepad>/buttonEast\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"Back\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                }\n            ]\n        },\n        {\n            \"name\": \"CharacterCustomize\",\n            \"id\": \"f4bb605c-f026-4416-a67b-4ca63329ed55\",\n            \"actions\": [\n                {\n                    \"name\": \"RotateLeft\",\n                    \"type\": \"Button\",\n                    \"id\": \"2200349a-b7d4-47cc-8d55-3e4033b41e9e\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"RotateRight\",\n                    \"type\": \"Button\",\n                    \"id\": \"186611ae-c673-48b0-87d7-bf0c00826ea5\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Submit\",\n                    \"type\": \"Button\",\n                    \"id\": \"347d5ae1-9a47-431f-9f11-0332f5f3ad01\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Back\",\n                    \"type\": \"Button\",\n                    \"id\": \"87b5d5ca-8231-4057-a2d7-99cca9b7392c\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                }\n            ],\n            \"bindings\": [\n                {\n                    \"name\": \"\",\n                    \"id\": \"d75b297d-1adf-4fb4-aae9-7be45b115e94\",\n                    \"path\": \"<Gamepad>/leftShoulder\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"RotateLeft\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"bde5a73e-ca62-4755-8200-f203e7c54776\",\n                    \"path\": \"<Gamepad>/rightShoulder\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"RotateRight\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"cf53c5b5-41f0-44af-a8aa-ee53a30d967d\",\n                    \"path\": \"<Keyboard>/escape\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"Back\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"766118c7-92da-4873-881b-954e2d7c4d27\",\n                    \"path\": \"<Gamepad>/buttonEast\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"Back\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"65e7c650-0f47-4aff-80a7-3f02af4467b3\",\n                    \"path\": \"<Keyboard>/enter\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard\",\n                    \"action\": \"Submit\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"07e2def6-6ff2-4048-ae16-2f5918376fc7\",\n                    \"path\": \"<Gamepad>/buttonSouth\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"Submit\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"5fd07646-a5f1-4ac4-b129-285b65243e50\",\n                    \"path\": \"<Keyboard>/f\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Submit\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                }\n            ]\n        },\n        {\n            \"name\": \"SkillDescription\",\n            \"id\": \"f80e6880-c31d-4f91-9ec7-eb2ac97f5708\",\n            \"actions\": [\n                {\n                    \"name\": \"Back\",\n                    \"type\": \"Button\",\n                    \"id\": \"f6ad1ae2-0892-42c9-9f2d-4fcf45622a91\",\n                    \"expectedControlType\": \"\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                }\n            ],\n            \"bindings\": [\n                {\n                    \"name\": \"\",\n                    \"id\": \"f43331eb-55e2-4045-b56e-183b5bb3d8a7\",\n                    \"path\": \"<Gamepad>/buttonEast\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Back\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"b418fa57-a98f-4d32-8e1c-bfaf6eb1716d\",\n                    \"path\": \"<Gamepad>/rightStickPress\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Back\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                }\n            ]\n        }\n    ],\n    \"controlSchemes\": [\n        {\n            \"name\": \"Keyboard\",\n            \"bindingGroup\": \"Keyboard\",\n            \"devices\": [\n                {\n                    \"devicePath\": \"<Keyboard>\",\n                    \"isOptional\": true,\n                    \"isOR\": false\n                },\n                {\n                    \"devicePath\": \"<Mouse>\",\n                    \"isOptional\": false,\n                    \"isOR\": false\n                }\n            ]\n        },\n        {\n            \"name\": \"Gamepad\",\n            \"bindingGroup\": \"Gamepad\",\n            \"devices\": [\n                {\n                    \"devicePath\": \"<Gamepad>\",\n                    \"isOptional\": true,\n                    \"isOR\": false\n                }\n            ]\n        }\n    ]\n}");
		m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
		m_Player_Look = m_Player.FindAction("Look", throwIfNotFound: true);
		m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
		m_Player_Interact = m_Player.FindAction("Interact", throwIfNotFound: true);
		m_Player_ShootAttack = m_Player.FindAction("Shoot/Attack", throwIfNotFound: true);
		m_Player_Reload = m_Player.FindAction("Reload", throwIfNotFound: true);
		m_Player_AimMode = m_Player.FindAction("Aim Mode", throwIfNotFound: true);
		m_Player_RotateLeftCam = m_Player.FindAction("RotateLeftCam", throwIfNotFound: true);
		m_Player_RotateRightCam = m_Player.FindAction("RotateRightCam", throwIfNotFound: true);
		m_Player_Inventory = m_Player.FindAction("Inventory", throwIfNotFound: true);
		m_Player_Map = m_Player.FindAction("Map", throwIfNotFound: true);
		m_Player_Menu = m_Player.FindAction("Menu", throwIfNotFound: true);
		m_Player_Skip = m_Player.FindAction("Skip", throwIfNotFound: true);
		m_Player_TabKill = m_Player.FindAction("TabKill", throwIfNotFound: true);
		m_Player_ZoomInCam = m_Player.FindAction("ZoomInCam", throwIfNotFound: true);
		m_Player_ZoomOutCam = m_Player.FindAction("ZoomOutCam", throwIfNotFound: true);
		m_Player_TiltUpCam = m_Player.FindAction("TiltUpCam", throwIfNotFound: true);
		m_Player_TiltDownCam = m_Player.FindAction("TiltDownCam", throwIfNotFound: true);
		m_Player_ChangeWeapon1 = m_Player.FindAction("ChangeWeapon1", throwIfNotFound: true);
		m_Player_ChangeWeapon2 = m_Player.FindAction("ChangeWeapon2", throwIfNotFound: true);
		m_Player_Throw = m_Player.FindAction("Throw", throwIfNotFound: true);
		m_Player_Heal = m_Player.FindAction("Heal", throwIfNotFound: true);
		m_Player_Dash = m_Player.FindAction("Dash", throwIfNotFound: true);
		m_Player_BossChangeAnim = m_Player.FindAction("BossChangeAnim", throwIfNotFound: true);
		m_Player_Run = m_Player.FindAction("Run", throwIfNotFound: true);
		m_Player_ZoomOutMap = m_Player.FindAction("ZoomOutMap", throwIfNotFound: true);
		m_Player_ZoomInMap = m_Player.FindAction("ZoomInMap", throwIfNotFound: true);
		m_Player_ChatWheel = m_Player.FindAction("ChatWheel", throwIfNotFound: true);
		m_Player_Debug = m_Player.FindAction("Debug", throwIfNotFound: true);
		m_Player_Debug1 = m_Player.FindAction("Debug1", throwIfNotFound: true);
		m_Player_Debug2 = m_Player.FindAction("Debug2", throwIfNotFound: true);
		m_Player_Debug4 = m_Player.FindAction("Debug4", throwIfNotFound: true);
		m_Player_Debug5 = m_Player.FindAction("Debug5", throwIfNotFound: true);
		m_Player_Debug6 = m_Player.FindAction("Debug6", throwIfNotFound: true);
		m_Player_Debug7 = m_Player.FindAction("Debug7", throwIfNotFound: true);
		m_Player_Copy = m_Player.FindAction("Copy", throwIfNotFound: true);
		m_Player_VoiceChat = m_Player.FindAction("VoiceChat", throwIfNotFound: true);
		m_Player_ChangeRangeWeapon = m_Player.FindAction("Change Range Weapon", throwIfNotFound: true);
		m_Player_NavigateUI = m_Player.FindAction("NavigateUI", throwIfNotFound: true);
		m_Player_ShowCode = m_Player.FindAction("ShowCode", throwIfNotFound: true);
		m_Player_DropItem = m_Player.FindAction("DropItem", throwIfNotFound: true);
		m_Player_CombineItem = m_Player.FindAction("CombineItem", throwIfNotFound: true);
		m_UI = asset.FindActionMap("UI", throwIfNotFound: true);
		m_UI_Navigate = m_UI.FindAction("Navigate", throwIfNotFound: true);
		m_UI_Submit = m_UI.FindAction("Submit", throwIfNotFound: true);
		m_UI_Cancel = m_UI.FindAction("Cancel", throwIfNotFound: true);
		m_UI_Point = m_UI.FindAction("Point", throwIfNotFound: true);
		m_UI_Click = m_UI.FindAction("Click", throwIfNotFound: true);
		m_UI_ScrollWheel = m_UI.FindAction("ScrollWheel", throwIfNotFound: true);
		m_UI_MiddleClick = m_UI.FindAction("MiddleClick", throwIfNotFound: true);
		m_UI_RightClick = m_UI.FindAction("RightClick", throwIfNotFound: true);
		m_UI_TrackedDevicePosition = m_UI.FindAction("TrackedDevicePosition", throwIfNotFound: true);
		m_UI_TrackedDeviceOrientation = m_UI.FindAction("TrackedDeviceOrientation", throwIfNotFound: true);
		m_UI_LeftTab = m_UI.FindAction("LeftTab", throwIfNotFound: true);
		m_UI_RightTab = m_UI.FindAction("RightTab", throwIfNotFound: true);
		m_UI_Menu = m_UI.FindAction("Menu", throwIfNotFound: true);
		m_UI_Delete = m_UI.FindAction("Delete", throwIfNotFound: true);
		m_UI_Copy = m_UI.FindAction("Copy", throwIfNotFound: true);
		m_UI_ShowCode = m_UI.FindAction("ShowCode", throwIfNotFound: true);
		m_UI_ChangeLanguage = m_UI.FindAction("ChangeLanguage", throwIfNotFound: true);
		m_UI_Leaderboard = m_UI.FindAction("Leaderboard", throwIfNotFound: true);
		m_InventoryUI = asset.FindActionMap("InventoryUI", throwIfNotFound: true);
		m_InventoryUI_LeftTab = m_InventoryUI.FindAction("LeftTab", throwIfNotFound: true);
		m_InventoryUI_CloseInteract = m_InventoryUI.FindAction("CloseInteract", throwIfNotFound: true);
		m_InventoryUI_RightTab = m_InventoryUI.FindAction("RightTab", throwIfNotFound: true);
		m_InventoryUI_SkillDescription = m_InventoryUI.FindAction("SkillDescription", throwIfNotFound: true);
		m_InventoryUI_Back = m_InventoryUI.FindAction("Back", throwIfNotFound: true);
		m_CharacterCustomize = asset.FindActionMap("CharacterCustomize", throwIfNotFound: true);
		m_CharacterCustomize_RotateLeft = m_CharacterCustomize.FindAction("RotateLeft", throwIfNotFound: true);
		m_CharacterCustomize_RotateRight = m_CharacterCustomize.FindAction("RotateRight", throwIfNotFound: true);
		m_CharacterCustomize_Submit = m_CharacterCustomize.FindAction("Submit", throwIfNotFound: true);
		m_CharacterCustomize_Back = m_CharacterCustomize.FindAction("Back", throwIfNotFound: true);
		m_SkillDescription = asset.FindActionMap("SkillDescription", throwIfNotFound: true);
		m_SkillDescription_Back = m_SkillDescription.FindAction("Back", throwIfNotFound: true);
	}

	~PlayerInputActions()
	{
	}

	public void Dispose()
	{
		UnityEngine.Object.Destroy(asset);
	}

	public bool Contains(InputAction action)
	{
		return asset.Contains(action);
	}

	public IEnumerator<InputAction> GetEnumerator()
	{
		return asset.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public void Enable()
	{
		asset.Enable();
	}

	public void Disable()
	{
		asset.Disable();
	}

	public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
	{
		return asset.FindAction(actionNameOrId, throwIfNotFound);
	}

	public int FindBinding(InputBinding bindingMask, out InputAction action)
	{
		return asset.FindBinding(bindingMask, out action);
	}
}
