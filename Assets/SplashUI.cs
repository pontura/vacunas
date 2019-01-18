using System.Collections;
using System.Security.Cryptography;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SplashUI : MonoBehaviour {

	[SerializeField] Text textField;

	void Awake()
	{		
		Events.OnKeyboardText +=OnKeyboardText;
	}
	void OnDestroy()
	{
		Events.OnKeyboardText -=OnKeyboardText;
	}
	void OnKeyboardText(string text)
	{
		textField.text = text;
	}

}
