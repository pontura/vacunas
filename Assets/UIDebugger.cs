using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDebugger : Singleton<UIDebugger>
{
	public GameObject panel;
	public Text field;

	void Start()
	{
		Reset ();
	}
	public void SetField(string text) {
		CancelInvoke ();
		panel.SetActive (true);
		field.text = text;
		Invoke ("Reset", 1);
	}
	void Reset()
	{
		field.text = "";
		panel.SetActive (false);
	}
}
