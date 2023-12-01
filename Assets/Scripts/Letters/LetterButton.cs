using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LetterButton : MonoBehaviour {
	[SerializeField] private TMP_Text text;

	public static Cell Cell { get; set; }
	private char _letter;

	public void SetLetter(char letter) {
		_letter = letter;
		text.text = _letter.ToString().ToUpper();
	}

	public void OnClick() {
		Cell.SetLetterFromKeyboard(_letter);
	}
}
