using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro : MonoBehaviour {

	public states state;
	public enum states
	{
		IDLE,
		WAIT_FOR_LANGUAGE,
		DONE
	}

	int num = 1;
	void Start()
	{
		InputManager.Instance.OnInput += OnInput;

		if (!PersistentData.Instance.langSelected)
			StartCoroutine( WaitForLanguage());
	
	}

	IEnumerator WaitForLanguage()
	{		
		state = states.WAIT_FOR_LANGUAGE;
		yield return new WaitForSeconds (0.2f);
		PersistentData.Instance.audios.PlayAudio (AudiosManager.AudioType.selectLang);
		yield return new WaitForSeconds (1.5f);
		PersistentData.Instance.langSelected = true;
		PersistentData.Instance.audios.PlayAudio (AudiosManager.AudioType.langName);
		num = 0;
	}

	void SwipeRight()
	{
		if (state == states.WAIT_FOR_LANGUAGE)
			SetLangNum (1);
	}
	void SwipeLeft()
	{
		if (state == states.WAIT_FOR_LANGUAGE)
			SetLangNum (-1);
	}
	void SetLangNum(int qty)
	{
		PersistentData.Instance.langSelected = true;
		StopAllCoroutines ();
		num += qty;
		if (num > 4) 
			num = 4;
		else if (num < 0) 
			num = 0;



		////////////////////HACK
		num = 1;
		////////////////////////
		/// 

		if (num == 0) 
			PersistentData.Instance.lang = PersistentData.languages.EN;
		else  if (num == 1) 
			PersistentData.Instance.lang = PersistentData.languages.ES;
		else  if (num == 2) 
			PersistentData.Instance.lang = PersistentData.languages.PO;
		else  if (num == 3) 
			PersistentData.Instance.lang = PersistentData.languages.FR;
		else  if (num == 4) 
			PersistentData.Instance.lang = PersistentData.languages.AR;

		SayLanguage ();
	}
	void SayLanguage()
	{
		PersistentData.Instance.audios.PlayAudio (AudiosManager.AudioType.langName);
	}

	IEnumerator Done()
	{		
		state = states.IDLE;
		PersistentData.Instance.audios.PlayAudio (AudiosManager.AudioType.ok);
		yield return new WaitForSeconds (1);
		NextScene ();
	}
	void OnDestroy()
	{
		InputManager.Instance.OnInput -= OnInput;
	}

	void OnInput (InputManager.types type) 
	{
		
		switch (type) {
		case InputManager.types.SWIPE_RIGHT:
			SwipeRight ();
			break;
		case InputManager.types.SWIPE_LEFT:
			SwipeLeft ();
			break;
		case InputManager.types.GATILLO_DOWN:
			if(!PersistentData.Instance.langSelected)
			{
				WaitForLanguage();
			} else
				StartCoroutine (Done());
			break;
		case InputManager.types.PAD_HOLD:
			Reset ();
			break;
		}
	}
	void NextScene()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene ("002_Main");
	}
	void Reset()
	{
		PersistentData.Instance.langSelected = false;
		UnityEngine.SceneManagement.SceneManager.LoadScene ("001_Oculus");
	}

}
