using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour {
	[SerializeField] private TMP_Text text;
	[SerializeField] private Button button;
	[SerializeField] private GameObject cancelButtonGameObject;

	private bool _isSelected;
	private static Cell _lastSelected;
	private Graphic _graphic;
	private char _letter;
	private readonly Color _lastColor = Color.white;
	private Color _finishedColor = Color.white;

	public Vector2Int Position { get; set; }

	public char Letter => _letter;

	public bool Finished { get; private set; }

	public void Finish() {
		Finished = true;
		_graphic.color = _finishedColor;
	}
	public void SetSelectable(bool selectable) {
		button.interactable = selectable;

		var color = _graphic.color;
		color.a = selectable ? 1 : 0.75f;
		_graphic.color = color;
	}

	private void Awake() {
		text.gameObject.SetActive(false);
		_graphic = button.targetGraphic;
		CancelButtonSetActive(false);
	}

	private void CancelButtonSetActive(bool active) {
		cancelButtonGameObject.SetActive(active);
	}

	public void SetLetter(char letter) {
		_letter = letter;
		text.gameObject.SetActive(true);
		text.text = letter.ToString().ToUpper();
	}

	public void Clear() {
		text.gameObject.SetActive(false);
	}

	public void Click() {
		if (Field.ChoiceMode == ChoiceMode.Letter) Select();
		else TryContinuePath();
	}

	private void Select() {
		if (_lastSelected != null) _lastSelected.Deselect();
		_lastSelected = this;
		_graphic.color = button.colors.pressedColor;
		_isSelected = true;
	}

	public void Deselect() {
		_lastSelected = null;
		if (!Finished) _graphic.color = Color.white;
		_isSelected = false;
	}

	private void SetLetterFromKeyboard(KeyCode keyCode) {
		SetLetter(keyCode.ToString()[0]);
		_graphic.color = button.colors.pressedColor;
		_isSelected = false;
		Field.StartChooseWord(this);
	}

	private void Update() {
		if ( !_isSelected ) return;
		if (Input.anyKeyDown)
		{
			for (KeyCode keyCode = KeyCode.A; keyCode <= KeyCode.Z; keyCode++)
			{
				if (Input.GetKeyDown(keyCode))
				{
					SetLetterFromKeyboard(keyCode);
				}
			}
		}
	}

	private void TryContinuePath() {
		if ( Field.TryContinuePath(this) ) {
			_graphic.color = button.colors.pressedColor;
		}
	}

	public void ApplyColorScheme(ColorScheme scheme) {
		var colorBlock = button.colors;
		colorBlock.pressedColor = Field.ChoiceMode == ChoiceMode.Letter ? scheme.selectedColor : scheme.pathColor;
		button.colors = colorBlock;
		_finishedColor = scheme.finishedColor;
	}

	public static bool AreNeighbours(Cell cell1, Cell cell2) {
		return Vector2Int.Distance(cell1.Position, cell2.Position) == 1;
	}

	public bool TryContinuePath(ref List<Cell> path) {
		if ( path.Count == 0 ) {
			path.Add(this);
			path[^1].CancelButtonSetActive(true);
			return true;
		}
		if ( path.Contains(this) ) return false;
		if ( !AreNeighbours(this, path[^1]) ) return false;
		if (path.Count > 0) path[^1].CancelButtonSetActive(false);
		path.Add(this);
		path[^1].CancelButtonSetActive(true);
		return true;
	}

	public void RemoveFromPath() {
		_graphic.color = _lastColor;
		CancelButtonSetActive(false);
	}

	public void SetAsPathEnd() {
		CancelButtonSetActive(true);
	}

	public void Cancel() {
		Field.CancelLastCell();
	}
}
