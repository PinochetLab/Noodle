using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum ChoiceMode {
	Letter,
	Word,
}

public class Field : MonoBehaviour {
	[SerializeField] private GameObject letterBlockPrefab;
	[SerializeField] private Button cancelButton;
	[SerializeField] private Button finishButton;
	[SerializeField] private RatingTable firstRatingTable;
	[SerializeField] private RatingTable secondRatingTable;
	[SerializeField] private GridLayoutGroup gridLayoutGroup;

	[SerializeField] private TMP_Text text;
	[SerializeField] private UnityEvent onWin;

	private Cell[,] _cells;

	private int _size;
	private int _moveCount;
	private int _currentMoveCount;

	private bool _isFirstPlayer = true;

	private static Field _instance;

	private static Cell _necessaryCell;
	private static readonly List<Cell> FinishedCells = new ();
	private static List<Cell> _path;
	public static ChoiceMode ChoiceMode = ChoiceMode.Letter;

	private void Awake() {
		_instance = this;
		cancelButton.interactable = false;
		finishButton.interactable = false;
	}

	private void Start() {
		Build(5);
	}

	private bool CanSelect(int x, int y) {
		if ( _cells[x, y].Finished ) return false;
		if ( x > 0 && _cells[x - 1, y].Finished ) return true;
		if ( x < _size - 1 && _cells[x + 1, y].Finished ) return true;
		if ( y > 0 && _cells[x, y - 1].Finished ) return true;
		if ( y < _size - 1 && _cells[x, y + 1].Finished ) return true;
		return false;
	}

	private void UpdateSelectableCells() {
		UpdateColorScheme();
		for ( var i = 0; i < _size; i++ ) {
			for ( var j = 0; j < _size; j++ ) {
				_cells[i, j].Deselect();
				_cells[i, j].SetSelectable(CanSelect(i, j));
			}
		}
	}

	private static bool CanUseInPath(int x, int y) {
		var v = new Vector2Int(x, y);
		return v == _necessaryCell.Position || FinishedCells.Contains(_instance._cells[x, y]);
	}

	private void UpdateAvailableCells() {
		UpdateColorScheme();
		for ( var i = 0; i < _size; i++ ) {
			for ( var j = 0; j < _size; j++ ) {
				if ( !CanUseInPath(i, j) ) {
					_cells[i, j].SetSelectable(false);
					continue;
				}
				var cell = _cells[i, j];
				if ( _path.Contains(cell) || (_path.Count > 0 && !Cell.AreNeighbours(_path[^1], cell))) {
					_cells[i, j].SetSelectable(false);
					continue;
				}
				_cells[i, j].SetSelectable(true);
			}
		}
	}

	public static void StartChooseWord(Cell cell) {
		_path = new List<Cell>();
		_necessaryCell = cell;
		ChoiceMode = ChoiceMode.Word;
		_instance.UpdateAvailableCells();
		//Debug.Log(WordFinder.Find(_instance._size, _instance._cells, _necessaryCell));
	}

	public static bool TryContinuePath(Cell cell) {
		if ( cell != _necessaryCell && !FinishedCells.Contains(cell) ) return false;
		var result = cell.TryContinuePath(ref _path);
		if ( result ) {
			_instance.cancelButton.interactable = true;
			_instance.finishButton.interactable = _path.Count >= 3;
			_instance.UpdateAvailableCells();
		}
		return result;
	}

	public void Cancel() {
		var len = _path.Count;
		for ( var i = 0; i < len; i++ ) {
			CancelLastCell();
		}
		_necessaryCell.Deselect();
		_necessaryCell.Clear();
		ChoiceMode = ChoiceMode.Letter;
		UpdateSelectableCells();
	}

	public void Finish() {
		if ( ChoiceMode == ChoiceMode.Letter ) return;
		var word = new string(_path.Select(cell => cell.Letter).ToArray()).ToLower();
		if ( WordMaster.Exists(word) ) {
			if ( !WordMaster.WasUsed(word) ) {
				if ( _path.Contains(_necessaryCell) ) {
					WordMaster.Add(word);
					var ratingTable = _isFirstPlayer ? firstRatingTable : secondRatingTable;
                    ratingTable.AddWord(word);
                    var letter = _necessaryCell.Letter;
                    Cancel();
                    _necessaryCell.SetLetter(letter);
                    _necessaryCell.Finish();
                    FinishedCells.Add(_necessaryCell);
                    SwitchPlayer();
                    UpdateSelectableCells();
                    _currentMoveCount++;
                    if ( _currentMoveCount >= _moveCount ) {
	                    var s1 = firstRatingTable.Score;
	                    var s2 = secondRatingTable.Score;
	                    if ( s1 == s2 ) text.text = "Draw";
	                    else if ( s1 > s2 ) text.text = "Player 1 won";
	                    else text.text = "Player 2 won";
	                    onWin.Invoke();
                    }
				}
				else {
					ErrorHandle.Push(word, ComposeError.UnusedLetter);
				}
			}
			else {
				ErrorHandle.Push(word, ComposeError.WasComposed);
			}
		}
		else {
			ErrorHandle.Push(word, ComposeError.DoesNotExist);
		}
	}

	private void Update() {
		if (ChoiceMode == ChoiceMode.Letter) return;
		if ( Input.GetKeyDown(KeyCode.Return) ) {
			Finish();
		}
		if ( Input.GetKeyDown(KeyCode.Escape) ) {
			Cancel();
		}
	}

	public static void CancelLastCell() {
		if ( _path.Count == 0 ) return;
		_path[^1].RemoveFromPath();
		_path.RemoveAt(_path.Count - 1);
		if ( _path.Count == 0 ) {
			_necessaryCell.Clear();
			_necessaryCell.Deselect();
			ChoiceMode = ChoiceMode.Letter;
			_instance.UpdateSelectableCells();
			return;
		}
		_path[^1].SetAsPathEnd();
		_instance.UpdateAvailableCells();
	}

	private void ClearChildren() {
		var children = (from Transform child in transform select child.gameObject).ToList();
		children.ForEach(Destroy);
	}

	public void Build(int fieldSize) {
		gridLayoutGroup.cellSize = gridLayoutGroup.GetComponent<RectTransform>().sizeDelta / fieldSize;
		ClearChildren();
		WordMaster.Clear();
		
		_size = fieldSize;
		_moveCount = _size * (_size - 1);
		_currentMoveCount = 0;
		firstRatingTable.Clear();
		firstRatingTable.SetSize(_size);
		secondRatingTable.Clear();
		secondRatingTable.SetSize(_size);
		_cells = new Cell[_size, _size];
		
		for ( var i = 0; i < _size; i++ ) {
			for ( var j = 0; j < _size; j++ ) {
				var letterBlock = Instantiate(letterBlockPrefab, transform).GetComponent<Cell>();
				letterBlock.Position = new Vector2Int(j, i);
				_cells[j, i] = letterBlock;
			}
		}

		var startWord = WordMaster.RandomWord(_size);
		WordMaster.Add(startWord);
		var h = _size / 2;
		
		for ( var i = 0; i < _size; i++ ) {
			_cells[i, h].SetLetter(startWord[i]);
			_cells[i, h].Finish();
			FinishedCells.Add(_cells[i, h]);
		}
		
		UpdateColorScheme();
		UpdateSelectableCells();
	}

	private void UpdateColorScheme() {
		var scheme = ColorSchemeMaster.GetScheme(_isFirstPlayer);
		for ( var i = 0; i < _size; i++ ) {
			for ( var j = 0; j < _size; j++ ) {
				_cells[i, j].ApplyColorScheme(scheme);
			}
		}
	}

	public void Quit() {
		Application.Quit();
	}

	private void SwitchPlayer() {
		_isFirstPlayer ^= true;
		UpdateColorScheme();
	}
}
