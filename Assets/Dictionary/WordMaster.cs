using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WordMaster : MonoBehaviour {
	
	[SerializeField] private TextAsset textAsset;
    private readonly SortedSet<string> _words = new ();
    private readonly Dictionary<int, List<string>> _wordsByLen = new ();

    private static WordMaster instance;

    private void Awake() {
	    instance = this;
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

    public static bool Exists(string word) => instance._words.Contains(word);

    public static string RandomWord(int length) {
	    var words = instance._wordsByLen[length];
	    var index = Random.Range(0, words.Count);
	    return words[index];
    }
}
