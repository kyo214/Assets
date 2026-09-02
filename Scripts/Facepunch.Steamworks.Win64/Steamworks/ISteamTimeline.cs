using System;
using System.Runtime.InteropServices;
using Steamworks.Data;

namespace Steamworks;

internal class ISteamTimeline : SteamInterface
{
	public const string Version = "STEAMTIMELINE_INTERFACE_V004";

	internal ISteamTimeline(bool IsGameServer)
	{
		SetupInterface(IsGameServer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
	internal static extern IntPtr SteamAPI_SteamTimeline_v004();

	public override IntPtr GetUserInterfacePointer()
	{
		return SteamAPI_SteamTimeline_v004();
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamTimeline_SetTimelineTooltip")]
	private static extern void _SetTimelineTooltip(IntPtr self, IntPtr pchDescription, float flTimeDelta);

	internal void SetTimelineTooltip(string pchDescription, float flTimeDelta)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchDescription);
		_SetTimelineTooltip(Self, utf8StringToNative.Pointer, flTimeDelta);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamTimeline_ClearTimelineTooltip")]
	private static extern void _ClearTimelineTooltip(IntPtr self, float flTimeDelta);

	internal void ClearTimelineTooltip(float flTimeDelta)
	{
		_ClearTimelineTooltip(Self, flTimeDelta);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamTimeline_SetTimelineGameMode")]
	private static extern void _SetTimelineGameMode(IntPtr self, TimelineGameMode eMode);

	internal void SetTimelineGameMode(TimelineGameMode eMode)
	{
		_SetTimelineGameMode(Self, eMode);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamTimeline_AddInstantaneousTimelineEvent")]
	private static extern TimelineEventHandle _AddInstantaneousTimelineEvent(IntPtr self, IntPtr pchTitle, IntPtr pchDescription, IntPtr pchIcon, uint unIconPriority, float flStartOffsetSeconds, TimelineEventClipPriority ePossibleClip);

	internal TimelineEventHandle AddInstantaneousTimelineEvent(string pchTitle, string pchDescription, string pchIcon, uint unIconPriority, float flStartOffsetSeconds, TimelineEventClipPriority ePossibleClip)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchTitle);
		using Utf8StringToNative utf8StringToNative2 = new Utf8StringToNative(pchDescription);
		using Utf8StringToNative utf8StringToNative3 = new Utf8StringToNative(pchIcon);
		return _AddInstantaneousTimelineEvent(Self, utf8StringToNative.Pointer, utf8StringToNative2.Pointer, utf8StringToNative3.Pointer, unIconPriority, flStartOffsetSeconds, ePossibleClip);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamTimeline_AddRangeTimelineEvent")]
	private static extern TimelineEventHandle _AddRangeTimelineEvent(IntPtr self, IntPtr pchTitle, IntPtr pchDescription, IntPtr pchIcon, uint unIconPriority, float flStartOffsetSeconds, float flDuration, TimelineEventClipPriority ePossibleClip);

	internal TimelineEventHandle AddRangeTimelineEvent(string pchTitle, string pchDescription, string pchIcon, uint unIconPriority, float flStartOffsetSeconds, float flDuration, TimelineEventClipPriority ePossibleClip)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchTitle);
		using Utf8StringToNative utf8StringToNative2 = new Utf8StringToNative(pchDescription);
		using Utf8StringToNative utf8StringToNative3 = new Utf8StringToNative(pchIcon);
		return _AddRangeTimelineEvent(Self, utf8StringToNative.Pointer, utf8StringToNative2.Pointer, utf8StringToNative3.Pointer, unIconPriority, flStartOffsetSeconds, flDuration, ePossibleClip);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamTimeline_StartRangeTimelineEvent")]
	private static extern TimelineEventHandle _StartRangeTimelineEvent(IntPtr self, IntPtr pchTitle, IntPtr pchDescription, IntPtr pchIcon, uint unPriority, float flStartOffsetSeconds, TimelineEventClipPriority ePossibleClip);

	internal TimelineEventHandle StartRangeTimelineEvent(string pchTitle, string pchDescription, string pchIcon, uint unPriority, float flStartOffsetSeconds, TimelineEventClipPriority ePossibleClip)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchTitle);
		using Utf8StringToNative utf8StringToNative2 = new Utf8StringToNative(pchDescription);
		using Utf8StringToNative utf8StringToNative3 = new Utf8StringToNative(pchIcon);
		return _StartRangeTimelineEvent(Self, utf8StringToNative.Pointer, utf8StringToNative2.Pointer, utf8StringToNative3.Pointer, unPriority, flStartOffsetSeconds, ePossibleClip);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamTimeline_UpdateRangeTimelineEvent")]
	private static extern void _UpdateRangeTimelineEvent(IntPtr self, TimelineEventHandle ulEvent, IntPtr pchTitle, IntPtr pchDescription, IntPtr pchIcon, uint unPriority, TimelineEventClipPriority ePossibleClip);

	internal void UpdateRangeTimelineEvent(TimelineEventHandle ulEvent, string pchTitle, string pchDescription, string pchIcon, uint unPriority, TimelineEventClipPriority ePossibleClip)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchTitle);
		using Utf8StringToNative utf8StringToNative2 = new Utf8StringToNative(pchDescription);
		using Utf8StringToNative utf8StringToNative3 = new Utf8StringToNative(pchIcon);
		_UpdateRangeTimelineEvent(Self, ulEvent, utf8StringToNative.Pointer, utf8StringToNative2.Pointer, utf8StringToNative3.Pointer, unPriority, ePossibleClip);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamTimeline_EndRangeTimelineEvent")]
	private static extern void _EndRangeTimelineEvent(IntPtr self, TimelineEventHandle ulEvent, float flEndOffsetSeconds);

	internal void EndRangeTimelineEvent(TimelineEventHandle ulEvent, float flEndOffsetSeconds)
	{
		_EndRangeTimelineEvent(Self, ulEvent, flEndOffsetSeconds);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamTimeline_RemoveTimelineEvent")]
	private static extern void _RemoveTimelineEvent(IntPtr self, TimelineEventHandle ulEvent);

	internal void RemoveTimelineEvent(TimelineEventHandle ulEvent)
	{
		_RemoveTimelineEvent(Self, ulEvent);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamTimeline_DoesEventRecordingExist")]
	private static extern SteamAPICall_t _DoesEventRecordingExist(IntPtr self, TimelineEventHandle ulEvent);

	internal CallResult<SteamTimelineEventRecordingExists_t> DoesEventRecordingExist(TimelineEventHandle ulEvent)
	{
		return new CallResult<SteamTimelineEventRecordingExists_t>(_DoesEventRecordingExist(Self, ulEvent), base.IsServer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamTimeline_StartGamePhase")]
	private static extern void _StartGamePhase(IntPtr self);

	internal void StartGamePhase()
	{
		_StartGamePhase(Self);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamTimeline_EndGamePhase")]
	private static extern void _EndGamePhase(IntPtr self);

	internal void EndGamePhase()
	{
		_EndGamePhase(Self);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamTimeline_SetGamePhaseID")]
	private static extern void _SetGamePhaseID(IntPtr self, IntPtr pchPhaseID);

	internal void SetGamePhaseID(string pchPhaseID)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchPhaseID);
		_SetGamePhaseID(Self, utf8StringToNative.Pointer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamTimeline_DoesGamePhaseRecordingExist")]
	private static extern SteamAPICall_t _DoesGamePhaseRecordingExist(IntPtr self, IntPtr pchPhaseID);

	internal CallResult<SteamTimelineGamePhaseRecordingExists_t> DoesGamePhaseRecordingExist(string pchPhaseID)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchPhaseID);
		return new CallResult<SteamTimelineGamePhaseRecordingExists_t>(_DoesGamePhaseRecordingExist(Self, utf8StringToNative.Pointer), base.IsServer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamTimeline_AddGamePhaseTag")]
	private static extern void _AddGamePhaseTag(IntPtr self, IntPtr pchTagName, IntPtr pchTagIcon, IntPtr pchTagGroup, uint unPriority);

	internal void AddGamePhaseTag(string pchTagName, string pchTagIcon, string pchTagGroup, uint unPriority)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchTagName);
		using Utf8StringToNative utf8StringToNative2 = new Utf8StringToNative(pchTagIcon);
		using Utf8StringToNative utf8StringToNative3 = new Utf8StringToNative(pchTagGroup);
		_AddGamePhaseTag(Self, utf8StringToNative.Pointer, utf8StringToNative2.Pointer, utf8StringToNative3.Pointer, unPriority);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamTimeline_SetGamePhaseAttribute")]
	private static extern void _SetGamePhaseAttribute(IntPtr self, IntPtr pchAttributeGroup, IntPtr pchAttributeValue, uint unPriority);

	internal void SetGamePhaseAttribute(string pchAttributeGroup, string pchAttributeValue, uint unPriority)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchAttributeGroup);
		using Utf8StringToNative utf8StringToNative2 = new Utf8StringToNative(pchAttributeValue);
		_SetGamePhaseAttribute(Self, utf8StringToNative.Pointer, utf8StringToNative2.Pointer, unPriority);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamTimeline_OpenOverlayToGamePhase")]
	private static extern void _OpenOverlayToGamePhase(IntPtr self, IntPtr pchPhaseID);

	internal void OpenOverlayToGamePhase(string pchPhaseID)
	{
		using Utf8StringToNative utf8StringToNative = new Utf8StringToNative(pchPhaseID);
		_OpenOverlayToGamePhase(Self, utf8StringToNative.Pointer);
	}

	[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamTimeline_OpenOverlayToTimelineEvent")]
	private static extern void _OpenOverlayToTimelineEvent(IntPtr self, TimelineEventHandle ulEvent);

	internal void OpenOverlayToTimelineEvent(TimelineEventHandle ulEvent)
	{
		_OpenOverlayToTimelineEvent(Self, ulEvent);
	}
}
