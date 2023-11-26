using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WordMaster : MonoBehaviour {
	
	[SerializeField] private TextAsset textAsset;
    private SortedSet<string> _words = new ();
    private Dictionary<int, List<string>> _wordsByLen = new ();

    private void Awake() {
	    Precalculate();
    }

    private void Precalculate() {
	    char[] separators = { '\r', '\n' };
	    var words = textAsset.text.Split(separators, StringSplitOptions.RemoveEmptyEntries);
	    foreach (var word in words) {
		    if ( !_wordsByLen.ContainsKey(word.Length) ) _wordsByLen[word.Length] = new List<string>();
		    _wordsByLen[word.Length].Add(word);
		    _words.Add(word);
	    }
    }

    public bool Exists(string word) => _words.Contains(word);

    public string RandomWord(int length) {
	    var words = _wordsByLen[length];
	    var index = Random.Range(0, words.Count);
	    return words[index];
    }
}
