using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentData : Singleton<PersistentData> {

	public bool DEBBUGER;
	public int num_of_vaccines;
	public AudiosManager audios;
	public bool langSelected;
	public languages lang;

	public enum languages
	{
		EN,
		ES,
		PO,
		FR,
		AR
	}
	void Start()
	{
		audios = GetComponent<AudiosManager> ();
		DontDestroyOnLoad (this.gameObject);


//		if(!DEBBUGER && (lastMonthChecked==0 || month != lastMonthChecked))
//			UnityEngine.SceneManagement.SceneManager.LoadScene("001_Password");
//		else
//			UnityEngine.SceneManagement.SceneManager.LoadScene("001_Oculus");
	}

}
