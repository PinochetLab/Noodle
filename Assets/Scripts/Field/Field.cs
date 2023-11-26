using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChoiceMode {
	Letter,
	Word,
}

public class Field : MonoBehaviour {
	[SerializeField] private GameObject letterBlockPrefab;

	private LetterBlock[,] _letterBlocks;

	private static Field instance;

	private static Vector2Int neccessaryPosition;
	private static List<Vector2Int> finishedPositions = new ();
	private static List<Vector2Int> path;
	public static ChoiceMode ChoiceMode = ChoiceMode.Letter;

	private void Awake() {
		instance = this;
	}

	private void Start() {
		Build(5);
	}

	private static void SetChoiceMode(ChoiceMode choiceMode) {
		foreach (var letterBlock in instance._letterBlocks)
		{
			letterBlock.SetChoiceMode(choiceMode);
		}
		ChoiceMode = choiceMode;
	}

	public static void StartChooseWord(Vector2Int position) {
		path = new List<Vector2Int>();
		neccessaryPosition = position;
		SetChoiceMode(ChoiceMode.Word);
	}

	public static bool TryContinuePath(Vector2Int position) {
		if ( position != neccessaryPosition && !finishedPositions.Contains(position) ) return false;
		return instance._letterBlocks[position.x, position.y].TryContinuePath(ref path);
	}

	public void Build(int size) {
		_letterBlocks = new LetterBlock[size, size];
		for ( var i = 0; i < size; i++ ) {
			for ( var j = 0; j < size; j++ ) {
				var letterBlock = Instantiate(letterBlockPrefab, transform).GetComponent<LetterBlock>();
				letterBlock.Position = new Vector2Int(j, i);
				_letterBlocks[j, i] = letterBlock;
			}
		}

		var startWord = WordMaster.RandomWord(size);
		var h = size / 2;
		for ( var i = 0; i < size; i++ ) {
			_letterBlocks[i, h].SetLetter(startWord[i]);
			_letterBlocks[i, h].Finish();
			finishedPositions.Add(_letterBlocks[i, h].Position);
		}
	}
}
