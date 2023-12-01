using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class WordFinder {
	private static int _size;
	private static Cell[,] _cells;
	private static Cell _necessaryCell;
	private static string _bestWord;
	
	public static string Find(int size, Cell[,] cells, Cell necessaryCell) {
		_size = size;
		_cells = cells;
		_necessaryCell = necessaryCell;
		_bestWord = "";
		for ( var i = 0; i < _size; i++ ) {
			for ( var j = 0; j < _size; j++ ) {
				var path = new List<Cell>();
				Dfs(i, j, ref path);
			}
		}
		return _bestWord;
	}

	private static void Dfs(int i, int j, ref List<Cell> path) {
		if ( i < 0 || i >= _size || j < 0 || j >= _size ) return;
		if ( path.Contains(_cells[i, j]) ) return;
		if ( !_cells[i, j].Finished && _necessaryCell != _cells[i, j] ) return;
		var word = new string(path.Select(cell => cell.Letter).ToArray()).ToLower();
		if ( word.Length > _bestWord.Length && path.Contains(_necessaryCell) && WordMaster.CanCompose(word) ) {
			_bestWord = word;
		}

		path.Add(_cells[i, j]);
		var count = path.Count;
		Dfs(i + 1, j, ref path);
		path.RemoveRange(count, path.Count - count);
		Dfs(i - 1, j, ref path);
		path.RemoveRange(count, path.Count - count);
		Dfs(i, j + 1, ref path);
		path.RemoveRange(count, path.Count - count);
		Dfs(i, j - 1, ref path);
		path.RemoveRange(count, path.Count - count);
	}
}
