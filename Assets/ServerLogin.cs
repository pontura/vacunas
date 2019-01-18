using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ServerLogin : MonoBehaviour {

	public string usernameInserted;
	public string passwordInserted;
	public string dispositivo;

	public bool CheckIfDeviceIsStillRegistered;
	public bool ResetDevice;
	public string url = "http://www.pontura.com/dermic/";
	public string deviceName;
	public int expiredYear;
	public int expiredMonth;
	public int expiredDay;

	public int nowYear;
	public int nowMonth;
	public int nowDay;

	bool hasInternet = false;

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
		
		LoadDataSaved ();

		LoopForInternet ();
		SetDebbugText ("Loading data");
		Invoke ("Delayed", 3);

		Events.OnKeyboardFieldEntered += OnKeyboardFieldEntered;
	}
	void LoadDataSaved()
	{
		dispositivo = PlayerPrefs.GetString ("dispositivo");
		deviceName = PlayerPrefs.GetString ("deviceName");
		expiredYear = PlayerPrefs.GetInt ("expiredYear", expiredYear);
		expiredMonth = PlayerPrefs.GetInt ("expiredMonth", expiredMonth);
		expiredDay = PlayerPrefs.GetInt ("expiredDay", expiredDay);
	}
	void Delayed()
	{		
		if (dispositivo != "")
		{
			if (!hasInternet) {
				SetDebbugText ("Please, connect your device to continue");
				CheckIfDeviceIsStillRegistered = true;
			} else {
				CancelInvoke ();
				StartCoroutine( CheckingIfDeviceIsStillRegistered () );
			}
		} else {		
			CancelInvoke ();	
			CheckExpirationValue ();
		}
	}
	void Connected()
	{
		if (CheckIfDeviceIsStillRegistered) {
			SetDebbugText ("Checking device...");
			StartCoroutine( CheckingIfDeviceIsStillRegistered () );
		}
	}
	void CheckExpirationValue()
	{
		expired = CheckExpiration ();
		if (expired) {
			SetDebbugText ("Please, re-enter user and password");
			GotoLogin ();
		} else if (expiredYear != 0) {
			SetDebbugText ("Licence expiration date: " + expiredYear + "/" + expiredMonth + "/" + expiredDay);
			GotoMain ();
		} else
			GotoLogin ();
	}

	public bool expired;
	bool CheckExpiration()
	{
		nowYear = DateTime.Now.Year;
		nowMonth = DateTime.Now.Month;
		nowDay = DateTime.Now.Day;

		LoadDataSaved ();

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
//		if (deviceName.Length > 1) {
//			if (expired) 
//				LoopForInternet ();
//		} else
//			LoopForInternet ();
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
		Invoke ("LoopForInternet", 2);

		StartCoroutine (checkInternetConnection ());

		if(!hasInternet)
			SetDebbugText ("No internet connection, please connect your device and re-open the app");
		else
			SetDebbugText ("Internet connected!");

		if (hasInternet)
			Connected ();
	}
	void DeviceRegistered()
	{
		Events.OnKeyboardTitle( "Device registered: " + deviceName);
		GotoMain ();
	}
	public void Login()
	{
//		if (deviceName.Length > 0 && !expired ) {
//			SetDebbugText ("Device already registered: " + deviceName);
//		}
//		else 
		if (usernameInserted == "" || passwordInserted == "") {
			SetDebbugText ("Username or password incorrect");
			GotoLogin ();
		}
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
	string GetUniqueID()
	{
		int a = UnityEngine.Random.Range (1, 10000);
		int b = UnityEngine.Random.Range (1, 10000);
		int c = UnityEngine.Random.Range (1, 10000);
		return jsonLoginResult.result [0].nombre + "_" + a.ToString () + b.ToString () + c.ToString ();
	}
	IEnumerator Register()
	{
		dispositivo = GetUniqueID();
		PlayerPrefs.SetString ("dispositivo", dispositivo);
		string post_url = url + "app_register.php" + "?cliente_id=" + jsonLoginResult.result[0].id + "&password=" + passwordInserted + "&dispositivo=" + dispositivo ;
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
			SaveName(hs_post.text); 
		}
	}
	void SaveName(string _deviceName)
	{	
		deviceName = _deviceName;
		PlayerPrefs.SetString ("deviceName", deviceName);
		DeviceRegistered ();
		SetLicencia( jsonLoginResult.result [0].licencia );	
	}
	void SetLicencia(string licencia)
	{
		string[] dates = licencia.Split ("-" [0]);
		Events.OnKeyboardText("New Licence: " + licencia);

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
	void GotoMain()
	{
		CancelInvoke ();
		LoadScene ("002_Main", 1);
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

	IEnumerator CheckingIfDeviceIsStillRegistered()
	{
		string post_url = url + "app_check_device.php?dispositivo=" + dispositivo;

		SetDebbugText("Checking if your device is still registered...");

		WWW hs_post = new WWW(post_url);
		yield return hs_post;

		if (hs_post.error != null)
		{
			SetDebbugText("Error: " + hs_post.error);
			GotoLogin ();
		} else if (hs_post.text == "ok")
		{
			CheckExpirationValue ();
		} else
		{
			SetDebbugText("Your device is no longer registered. ID: " + dispositivo + hs_post.text);
			PlayerPrefs.DeleteAll ();
			GotoLogin ();
		}
	}


	IEnumerator checkInternetConnection(){
		WWW www = new WWW("http://google.com");
		yield return www;
		if (www.error != null) {
			hasInternet = false;
		} else {
			hasInternet = true;;
		}
	} 


}


