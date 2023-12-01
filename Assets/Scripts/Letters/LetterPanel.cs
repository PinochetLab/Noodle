using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterPanel : MonoBehaviour {
	[SerializeField] private GameObject buttonPrefab;
	[SerializeField] private GameObject panel;
	[SerializeField] private RectTransform rectTransform;

	private static LetterPanel _instance;

	private void Awake() {
		_instance = this;
		panel.SetActive(false);

		for ( var c = 'a'; c <= 'z'; c++ ) {
			var letterButton = Instantiate(buttonPrefab, rectTransform).GetComponent<LetterButton>();
			letterButton.SetLetter(c);
		}
	}

	public static void StartChoose(Cell cell) {
		LetterButton.Cell = cell;
		_instance.panel.SetActive(true);
		_instance.rectTransform.anchoredPosition = Input.mousePosition + new Vector3(1, -1) * 30;
	}

	public void Cancel() {
		LetterButton.Cell.Deselect();
		panel.SetActive(false);
	}

	public static void EndChoose() {
		_instance.panel.SetActive(false);
	}
}
