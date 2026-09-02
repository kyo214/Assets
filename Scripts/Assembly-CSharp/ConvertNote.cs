using TMPro;
using UnityEngine;

public class ConvertNote : MonoBehaviour
{
	public TextMeshProUGUI textMesh;

	public string initText;

	private void Start()
	{
		textMesh = GetComponent<TextMeshProUGUI>();
		if (textMesh != null && UIGameManager.Instance != null)
		{
			initText = textMesh.text;
			textMesh.text = UIGameManager.Instance.ConvertNote(textMesh.text);
			UIGameManager.Instance.arrConvertedText.Add(this);
		}
	}
}
