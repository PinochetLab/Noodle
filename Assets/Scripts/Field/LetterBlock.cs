using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LetterBlock : MonoBehaviour {
	[SerializeField] private TMP_Text text;
	[SerializeField] private Button button;
	[SerializeField] private Color selectedColor;

	private static LetterBlock selected;

	private bool _isSelected;

	public Vector2Int Position { get; set; }

	private bool _isFinished;

	private static Vector2Int lastPosition = -Vector2Int.one;

	private void Awake() {
		text.gameObject.SetActive(false);
	}

	public void SetLetter(char letter) {
		text.gameObject.SetActive(true);
		text.text = letter.ToString().ToUpper();
	}

	public void Finish() {
		_isFinished = true;
		button.interactable = false;
	}

	public void SetChoiceMode(ChoiceMode choiceMode) {
		switch ( choiceMode ) {
			case ChoiceMode.Letter:
				if ( _isFinished ) {
					button.interactable = false;
				}
				break;
			case ChoiceMode.Word:
				button.interactable = true;
				break;
		}
	}

	public void Click() {
		switch ( Field.ChoiceMode ) {
			case ChoiceMode.Letter:
				Select();
				break;
			case ChoiceMode.Word:
				TryContinuePath();
				break;
		}
	}

	public void Select() {
		if (selected != null) selected.Deselect();
		selected = this;
		button.targetGraphic.color = selectedColor;
		_isSelected = true;
	}

	public void Deselect() {
		selected = null;
		button.targetGraphic.color = Color.white;
		_isSelected = false;
	}

	private void InsertLetterFromKeyboard(KeyCode keyCode) {
		SetLetter(keyCode.ToString()[0]);
		Deselect();
		button.targetGraphic.color = Color.cyan;
		Field.StartChooseWord(Position);
	}

	private void Update() {
		if ( !_isSelected ) return;
		if (Input.anyKeyDown)
		{
			for (KeyCode keyCode = KeyCode.A; keyCode <= KeyCode.Z; keyCode++)
			{
				if (Input.GetKeyDown(keyCode))
				{
					InsertLetterFromKeyboard(keyCode);
				}
			}
		}
	}

	public void TryContinuePath() {
		if ( Field.TryContinuePath(Position) ) {
			button.targetGraphic.color = Color.red;
		}
	}

	public bool TryContinuePath(ref List<Vector2Int> path) {
		if ( path.Count == 0 ) {
			path.Add(Position);
			return true;
		}
		if ( path.Contains(Position) ) return false;
		var lastPosition = path[^1];
		if ( (Position - lastPosition).magnitude != 1 ) return false;
		path.Add(Position);
		return true;
	}
}
