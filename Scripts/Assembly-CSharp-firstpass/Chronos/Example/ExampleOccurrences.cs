using UnityEngine;

namespace Chronos.Example;

[RequireComponent(typeof(Renderer))]
public class ExampleOccurrences : ExampleBaseBehaviour
{
	private class ChangeColorOccurrence : Occurrence
	{
		private Material material;

		private Color newColor;

		private Color previousColor;

		public ChangeColorOccurrence(Material material, Color newColor)
		{
			this.material = material;
			this.newColor = newColor;
		}

		public override void Forward()
		{
			previousColor = material.color;
			material.color = newColor;
		}

		public override void Backward()
		{
			material.color = previousColor;
		}
	}

	private void Start()
	{
		Material material = GetComponent<Renderer>().material;
		base.time.Do(repeatable: true, new ChangeColorOccurrence(material, Color.yellow));
		base.time.Plan(5f, repeatable: true, new ChangeColorOccurrence(material, Color.blue));
		base.time.Plan(7f, repeatable: true, new ChangeColorOccurrence(material, Color.green));
		base.time.Plan(10f, repeatable: true, new ChangeColorOccurrence(material, Color.red));
		Occurrence occurrence = base.time.Plan(3f, repeatable: true, new ChangeColorOccurrence(material, Color.magenta));
		base.time.Cancel(occurrence);
	}
}
