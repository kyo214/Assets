using System;
using UnityEngine.Rendering.Universal;

namespace Lux_SRP_GrassDisplacement;

public class GrassDisplacementRenderFeature : ScriptableRendererFeature
{
	[Serializable]
	public enum RTDisplacementSize
	{
		_128 = 0x80,
		_256 = 0x100,
		_512 = 0x200,
		_1024 = 0x400
	}

	[Serializable]
	public class GrassDisplacementSettings
	{
		public RTDisplacementSize Resolution = RTDisplacementSize._256;

		public float Size = 20f;

		public bool ShiftRenderTex;
	}

	public GrassDisplacementSettings settings = new GrassDisplacementSettings();

	private GrassDisplacementPass m_GrassDisplacementPass;

	public override void Create()
	{
		m_GrassDisplacementPass = new GrassDisplacementPass();
		m_GrassDisplacementPass.renderPassEvent = RenderPassEvent.BeforeRendering;
		m_GrassDisplacementPass.m_Resolution = (int)settings.Resolution;
		m_GrassDisplacementPass.m_Size = settings.Size;
		m_GrassDisplacementPass.m_ShiftRenderTex = settings.ShiftRenderTex;
	}

	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		m_GrassDisplacementPass.Setup(in renderingData);
		renderer.EnqueuePass(m_GrassDisplacementPass);
	}
}
