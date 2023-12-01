using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ErrorHandle : MonoBehaviour {
	[SerializeField] private CanvasGroup canvasGroup;
	[SerializeField] private TMP_Text text;

	private static ErrorHandle _instance;

	private void Awake() {
		canvasGroup.alpha = 0;
		_instance = this;
	}

	public static void Push(string word, ComposeError error) {
		_instance.StopAllCoroutines();
		_instance.StartCoroutine(_instance.Push());
		_instance.text.text = error switch {
			ComposeError.DoesNotExist => "Word " + word.ToUpper() + " does not exists!",
			ComposeError.WasComposed => "Word " + word.ToUpper() + " is already composed!",
			ComposeError.UnusedLetter => "Word does not contains added letter!",
			_ => throw new ArgumentOutOfRangeException(nameof(error), error, null)
		};
	}

	private IEnumerator Push() {
		canvasGroup.alpha = 1;
		yield return new WaitForSeconds(1);
		const int n = 30;
		const float dt = 1f;
		for ( var i = 0; i <= n; i++ ) {
			var t = (float)i / n;
			canvasGroup.alpha = Mathf.Lerp(1, 0, t);
			yield return new WaitForSeconds(dt / (n + 1));
		}
	}
}
