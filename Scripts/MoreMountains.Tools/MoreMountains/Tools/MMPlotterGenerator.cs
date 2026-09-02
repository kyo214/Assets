using System.Reflection;
using UnityEngine;

namespace MoreMountains.Tools;

public class MMPlotterGenerator : MonoBehaviour
{
	public MMPlotter PlotterPrefab;

	public Vector2 Spacing;

	public float VerticalOddSpacing;

	public int RowLength;

	[Header("Materials")]
	public Material LinearMaterial;

	public Material QuadraticMaterial;

	public Material CubicMaterial;

	public Material QuarticMaterial;

	public Material QuinticMaterial;

	public Material SinusoidalMaterial;

	public Material BounceMaterial;

	public Material OverheadMaterial;

	public Material ExponentialMaterial;

	public Material ElasticMaterial;

	public Material CircularMaterial;

	protected Vector2 _position;

	[MMInspectorButton("GeneratePlotters")]
	public bool GeneratePlottersButton;

	protected virtual void Awake()
	{
		Time.timeScale = 0f;
		GeneratePlotters();
	}

	protected virtual void GeneratePlotters()
	{
		base.transform.MMDestroyAllChildren();
		BindingFlags bindingAttr = BindingFlags.Static | BindingFlags.Public;
		MethodInfo[] methods = typeof(MMTweenDefinitions).GetMethods(bindingAttr);
		int num = 0;
		int num2 = 0;
		float num3 = 0f;
		for (int i = 0; i < methods.Length; i++)
		{
			_position.x = (float)num2 * Spacing.x;
			_position.y = num3;
			MMPlotter mMPlotter = Object.Instantiate(PlotterPrefab);
			mMPlotter.transform.SetParent(base.transform);
			mMPlotter.transform.localPosition = _position;
			mMPlotter.TweenMethodIndex = i;
			string text = mMPlotter.TweenName(mMPlotter.TweenMethodIndex);
			mMPlotter.gameObject.name = text;
			Material material = LinearMaterial;
			if (text.Contains("Linear"))
			{
				material = LinearMaterial;
			}
			if (text.Contains("Quadratic"))
			{
				material = QuadraticMaterial;
			}
			if (text.Contains("Cubic"))
			{
				material = CubicMaterial;
			}
			if (text.Contains("Quartic"))
			{
				material = QuarticMaterial;
			}
			if (text.Contains("Quintic"))
			{
				material = QuinticMaterial;
			}
			if (text.Contains("Sinusoidal"))
			{
				material = SinusoidalMaterial;
			}
			if (text.Contains("Bounce"))
			{
				material = BounceMaterial;
			}
			if (text.Contains("Overhead"))
			{
				material = OverheadMaterial;
			}
			if (text.Contains("Exponential"))
			{
				material = ExponentialMaterial;
			}
			if (text.Contains("Elastic"))
			{
				material = ElasticMaterial;
			}
			if (text.Contains("Circular"))
			{
				material = CircularMaterial;
			}
			mMPlotter.SetMaterial(material);
			mMPlotter.GetMethodsList();
			mMPlotter.DrawGraph();
			if (num2 >= RowLength - 1)
			{
				num2 = 0;
				num++;
				num3 = ((num % 2 != 0) ? (num3 + Spacing.y) : (num3 + (Spacing.y + VerticalOddSpacing)));
			}
			else
			{
				num2++;
			}
		}
	}
}
