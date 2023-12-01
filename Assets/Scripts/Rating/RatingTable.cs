using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RatingTable : MonoBehaviour {
	[SerializeField] private GameObject moveBlockPrefab;
	[SerializeField] private TMP_Text totalScoreText;
	[SerializeField] private Transform placeForSpawn;

	private int _totalScore;

	private void Awake() {
		Clear();
	}

	public void SetSize(int size) {
		var n = size * (size - 1) / 2;
		var gridLg = placeForSpawn.GetComponent<GridLayoutGroup>();
		var a = (800 - (n - 1) * gridLg.spacing.y) / n;
		var cellSize = gridLg.cellSize;
		cellSize.y = a;
		gridLg.cellSize = cellSize;
	}

	public int Score => _totalScore;

	public void Clear() {
		_totalScore = 0;
		totalScoreText.text = _totalScore.ToString();
		while ( placeForSpawn.childCount > 0 ) {
			DestroyImmediate(placeForSpawn.GetChild(0).gameObject);
		}
	}

	public void AddWord(string word) {
		_totalScore += word.Length;
		totalScoreText.text = _totalScore.ToString();
		var moveBlock = Instantiate(moveBlockPrefab, placeForSpawn).GetComponent<MoveBlock>();
		moveBlock.SetWord(word);
	}
}
