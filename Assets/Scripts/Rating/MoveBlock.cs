using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoveBlock : MonoBehaviour {
	[SerializeField] private TMP_Text lengthText;
	[SerializeField] private TMP_Text wordText;

	public void SetWord(string word) {
		lengthText.text = word.Length.ToString();
		wordText.text = word;
	}
}
