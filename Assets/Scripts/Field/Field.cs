using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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

	private Cell[,] _cells;

	private int _size;

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
		//UpdateColorScheme();
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
			var ratingTable = _isFirstPlayer ? firstRatingTable : secondRatingTable;
			ratingTable.AddWord(word);
			var letter = _necessaryCell.Letter;
			Cancel();
			_necessaryCell.SetLetter(letter);
			_necessaryCell.Finish();
			FinishedCells.Add(_necessaryCell);
			Debug.Log("yes");
			SwitchPlayer();
			UpdateSelectableCells();
		}
	}

	private void Update() {
		if ( Input.GetKeyDown(KeyCode.Return) ) {
			Finish();
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
	}

	private void CLearChildren() {
		var children = (from Transform child in transform select child.gameObject).ToList();
		children.ForEach(Destroy);
	}

	private void Build(int fieldSize) {
		CLearChildren();
		
		_size = fieldSize;
		_cells = new Cell[_size, _size];
		
		for ( var i = 0; i < _size; i++ ) {
			for ( var j = 0; j < _size; j++ ) {
				var letterBlock = Instantiate(letterBlockPrefab, transform).GetComponent<Cell>();
				letterBlock.Position = new Vector2Int(j, i);
				_cells[j, i] = letterBlock;
			}
		}

		var startWord = WordMaster.RandomWord(_size);
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

	private void SwitchPlayer() {
		_isFirstPlayer ^= true;
		UpdateColorScheme();
	}
}
