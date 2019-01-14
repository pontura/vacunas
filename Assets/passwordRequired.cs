using System.Collections;
using System.Security.Cryptography;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class passwordRequired : MonoBehaviour {

	public string[] passwords;
	int month;
	[SerializeField] Text passwordInput;
	[SerializeField] Text resultText;

	[SerializeField] string waitText = "Attempting to log in...";

	private void Start()
	{
		month = System.DateTime.Now.Month;
		if (PlayerPrefs.HasKey(PlayerPrefKeys.UserPassword))
		{
			passwordInput.text = PlayerPrefs.GetString(PlayerPrefKeys.UserPassword);
		}

	}
	public void Submit()
	{
		SignIn ();
	}
	public void SignIn()
	{
		resultText.text = waitText;
		Invoke ("Checked", 2);
	}
	void Checked()
	{		
		string pass = passwordInput.text.ToUpper();
		passwordInput.text = "";
		resultText.text = "PASS: " + pass + " MONTH: " + (month-1);

		if(pass == passwords[month-1])
			Done();
		else
			Wrong();
	}
	void Done()
	{
		PlayerPrefs.SetInt ("lastMonth", month);
		resultText.text = "Welcome!";
		Invoke ("DoneReady", 2);
	}
	void DoneReady()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene ("001_Oculus");
	}
	void Wrong()
	{
		resultText.text = "Wrong password";
		Invoke ("WrongReady", 2);
	}
	void WrongReady()
	{
		resultText.text = "";
	}
	public void StoreCreditionals()
	{
		PlayerPrefs.SetString(PlayerPrefKeys.UserPassword, passwordInput.text);

	}


}
