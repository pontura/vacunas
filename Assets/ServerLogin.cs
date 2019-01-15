using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ServerLogin : MonoBehaviour {

	public string usernameInserted;
	public string passwordInserted;

	public bool ResetDevice;
	public string url = "http://www.pontura.com/dermic/";
	public string deviceName;
	public int expiredYear;
	public int expiredMonth;
	public int expiredDay;

	public int nowYear;
	public int nowMonth;
	public int nowDay;

	bool hasInternet = true;

	JsonLoginData jsonLoginResult;

	[Serializable]
	public class JsonLoginData
	{
		public List<JsonLoginResult> result;
	}
	[Serializable]
	public class JsonLoginResult
	{
		public string id;
		public string total;
		public string nombre;
		public string password;
		public string licencia;
	}
	void Start()
	{
		if(ResetDevice)
			PlayerPrefs.DeleteAll ();
		
		CheckExpirationValue ();

		Events.OnKeyboardFieldEntered += OnKeyboardFieldEntered;
	}
	void CheckExpirationValue()
	{
		expired = CheckExpiration ();
		if (expired) {
			SetDebbugText ("Licence expired!");
			GotoLogin ();
		} else if (expiredYear != 0) {
			SetDebbugText ("Licence expiration date: " + expiredYear + "/" + expiredMonth + "/" + expiredDay);
			LoadScene ("002_Main", 3);
		}
	}
	public bool expired;
	bool CheckExpiration()
	{
		nowYear = DateTime.Now.Year;
		nowMonth = DateTime.Now.Month;
		nowDay = DateTime.Now.Day;

		deviceName = PlayerPrefs.GetString ("deviceName");
		expiredYear = PlayerPrefs.GetInt ("expiredYear", expiredYear);
		expiredMonth = PlayerPrefs.GetInt ("expiredMonth", expiredMonth);
		expiredDay = PlayerPrefs.GetInt ("expiredDay", expiredDay);

		if (deviceName.Length > 0) {
			if (expiredYear > nowYear)
				return false;
			else if (expiredYear >= nowYear && expiredMonth > nowMonth)
				return false;
			else if (expiredYear >= nowYear && expiredMonth >= nowMonth && expiredDay >= nowDay)
				return false;
			return true;
		}
		return false;
	}

	public void Init()
	{
		if (deviceName.Length > 1) {
			if (expired) 
				LoopForInternet ();
		} else
			LoopForInternet ();
	}
	void OnKeyboardFieldEntered(string text)
	{
		CancelInvoke ();
		if (usernameInserted == "") {
			usernameInserted = text;
			LoadScene("001_Password", 0);
		} else if (passwordInserted == "") {
			passwordInserted = text;
			Login ();
		}			
	}
	void LoopForInternet()
	{
		Invoke ("LoopForInternet", 1);
		if (Application.internetReachability == NetworkReachability.NotReachable && hasInternet)
			SwitchInternetConnection ();
		else if(Application.internetReachability != NetworkReachability.NotReachable && !hasInternet)
			SwitchInternetConnection ();		
	}
	void SwitchInternetConnection()
	{
		hasInternet = !hasInternet;
		if(!hasInternet)
			SetDebbugText ("No internet connection, please connect your device");
		else
			SetDebbugText ("Internet connected!");
	}
	void DeviceRegistered()
	{
		Events.OnKeyboardTitle( "Device registered: " + deviceName);
		LoadScene ("002_Main", 3);
	}
	public void Login()
	{
		if (deviceName.Length > 0 && !expired ) {
			SetDebbugText ("Device already registered: " + deviceName);
		}
		else if(usernameInserted =="" || passwordInserted == "")
			SetDebbugText ("Username or password not inserted");
		else
			StartCoroutine(LoginDone());
	}
	IEnumerator LoginDone()
	{
		string post_url = url + "app_login.php" + "?nombre=" + WWW.EscapeURL(usernameInserted) + "&password=" + WWW.EscapeURL(passwordInserted);

		SetDebbugText("Processing data");

		WWW hs_post = new WWW(post_url);
		yield return hs_post;

		if (hs_post.error != null)
		{
			SetDebbugText("Error: " + hs_post.error);
			GotoLogin ();
		}else
		{
			SetResult(hs_post.text); 
		}
	}
	void SetResult(string text)
	{
		jsonLoginResult = JsonUtility.FromJson<JsonLoginData> (text);
		if (jsonLoginResult == null || jsonLoginResult.result.Count == 0) {
			SetDebbugText ("Login failed");
			GotoLogin ();
		} else if (deviceName.Length > 1) {
			SetLicencia( jsonLoginResult.result [0].licencia );	
		} else {
			StartCoroutine (Register ());
		}
	}
	void SetDebbugText(string text)
	{
		CancelInvoke ();
		Invoke ("ResetField", 5);
		Events.OnKeyboardText( text );
	}
	void ResetField()
	{
		Events.OnKeyboardText( "" );
	}
	IEnumerator Register()
	{
		string post_url = url + "app_register.php" + "?cliente_id=" + jsonLoginResult.result[0].id + "&password=" + passwordInserted;
		Debug.Log (post_url);
		WWW hs_post = new WWW(post_url);
		yield return hs_post;

		if (hs_post.error != null)
		{
			SetDebbugText("Error: " + hs_post.error);
			GotoLogin ();
		}else if(hs_post.text == "error")
		{
			SetDebbugText("There was an error trying to register the device");
			GotoLogin ();
		}else if(hs_post.text == "full")
		{
			SetDebbugText("Too many devices (" + jsonLoginResult.result [0].total + " registered)");
			GotoLogin ();
		}else
		{
			deviceName = hs_post.text;
			SaveName(deviceName); 
		}
	}
	void SaveName(string deviceName)
	{	
		PlayerPrefs.SetString ("deviceName", deviceName);
		DeviceRegistered ();
		SetLicencia( jsonLoginResult.result [0].licencia );	
	}
	void SetLicencia(string licencia)
	{
		string[] dates = licencia.Split ("-" [0]);
		Debug.Log ("licencia: " + licencia);

		if (dates.Length > 1) {
			expiredYear = int.Parse (dates [0]);
			expiredMonth = int.Parse (dates [1]);
			expiredDay = int.Parse (dates [2]);

			PlayerPrefs.SetInt ("expiredYear", expiredYear);
			PlayerPrefs.SetInt ("expiredMonth", expiredMonth);
			PlayerPrefs.SetInt ("expiredDay", expiredDay);
			CheckExpirationValue ();
		}
	}
	void GotoLogin()
	{
		CancelInvoke ();
		Invoke ("Restart", 3);
	}
	void Restart()
	{
		usernameInserted = "";
		passwordInserted = "";
		LoadScene ("001_Password", 1);
	}
	void LoadScene(string sceneName, float delay)
	{
		CancelInvoke ();
		StartCoroutine (LoadSceneCoroutine(sceneName, delay));
	}
	IEnumerator LoadSceneCoroutine(string sceneName, float delay)
	{		
		yield return new WaitForSeconds (delay);
		UnityEngine.SceneManagement.SceneManager.LoadScene (sceneName);
	}
}
