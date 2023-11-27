using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ColorSchemeMaster : MonoBehaviour {
	[SerializeField] private ColorScheme firstPlayerScheme;
	[SerializeField] private ColorScheme secondPlayerScheme;

	private static ColorSchemeMaster _instance;

	private void Awake() {
		_instance = this;
	}

	public static ColorScheme GetScheme(bool isPlayerFirst) {
		return isPlayerFirst ? _instance.firstPlayerScheme : _instance.secondPlayerScheme;
	}
}
