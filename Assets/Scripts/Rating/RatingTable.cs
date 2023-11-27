using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RatingTable : MonoBehaviour {
	[SerializeField] private GameObject moveBlockPrefab;
	[SerializeField] private TMP_Text totalScoreText;
	[SerializeField] private Transform placeForSpawn;

	private int _totalScore;

	private void Awake() {
		Clear();
	}

	private void Clear() {
		_totalScore = 0;
		totalScoreText.text = _totalScore.ToString();
		while ( placeForSpawn.childCount > 1 ) {
			DestroyImmediate(placeForSpawn.GetChild(1).gameObject);
		}
	}

	public void AddWord(string word) {
		_totalScore += word.Length;
		totalScoreText.text = _totalScore.ToString();
		var moveBlock = Instantiate(moveBlockPrefab, placeForSpawn).GetComponent<MoveBlock>();
		moveBlock.SetWord(word);
	}
}
