using System.Collections;
using System.Security.Cryptography;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour {

	[SerializeField] Text titleField;
	[SerializeField] Text textField;

	void Start()
	{		
		PersistentData.Instance.serverLogin.Init ();
		if (PersistentData.Instance.serverLogin.usernameInserted == "")
			titleField.text = "Insert your username";
		else
			titleField.text = "Insert your password";
		
		Events.OnKeyboardText +=OnKeyboardText;
		Events.OnKeyboardTitle +=OnKeyboardTitle;
	}
	void OnDestroy()
	{
		Events.OnKeyboardText -=OnKeyboardText;
		Events.OnKeyboardTitle -=OnKeyboardTitle;
	}

	bool submited;
	public void Submit()
	{
		if (submited)
			return;

		submited = true;

		titleField.text = "";
		Events.OnKeyboardFieldEntered ( textField.text);
	}
	void OnKeyboardTitle(string text)
	{
		titleField.text = text;
	}
	void OnKeyboardText(string text)
	{
		textField.text = text;
	}

}
