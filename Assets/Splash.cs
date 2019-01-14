using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splash : MonoBehaviour {

	void Start()
	{
		int month = System.DateTime.Now.Month;
		int lastMonthChecked = PlayerPrefs.GetInt ("lastMonth", 0);
		if(!PersistentData.Instance.DEBBUGER && (lastMonthChecked==0 || month != lastMonthChecked))
			UnityEngine.SceneManagement.SceneManager.LoadScene("001_Password");
		else
			UnityEngine.SceneManagement.SceneManager.LoadScene("001_Oculus");
	}
}
