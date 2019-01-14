using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour {

	public states state;
	public enum states
	{
		IDLE,
		CUANTAS,
		WAIT_FOR_CUANTAS,
		DONE
	}
	public TimeManager timeManager;

	bool isDone;

	int num = 1;
	float timer;

	void Start()
	{
		InputManager.Instance.OnInput += OnInput;
		Restart ();
	}
	void Update()
	{
		timer += Time.deltaTime;
	}
	public void Restart()
	{
		timer = 0;
		isDone = false;
		StartCoroutine( Cuantas(true) );
	}

	void SwipeRight()
	{
		if (state == states.CUANTAS)
			SetNum (1);
	}
	void SwipeLeft()
	{
		if (state == states.CUANTAS)
			SetNum (-1);
	}
	void SetNum(int qty)
	{
		num += qty;
		if (num < 1) 
			num = 1;
		else if (num >6) 
			num = 6;

		PlayNum (num);
	}
	void PlayNum(int num)
	{
		if(num==1)
			PersistentData.Instance.audios.PlayAudio (AudiosManager.AudioType.one);	
		if(num==2)
			PersistentData.Instance.audios.PlayAudio (AudiosManager.AudioType.two);	
		if(num==3)
			PersistentData.Instance.audios.PlayAudio (AudiosManager.AudioType.three);	
		if(num==4)
			PersistentData.Instance.audios.PlayAudio (AudiosManager.AudioType.four);	
		if(num==5)
			PersistentData.Instance.audios.PlayAudio (AudiosManager.AudioType.five);
		if(num==6)
			PersistentData.Instance.audios.PlayAudio (AudiosManager.AudioType.six);
	}
	IEnumerator Cuantas(bool languageReady = false)
	{		
		yield return new WaitForSeconds (2);
		PersistentData.Instance.audios.PlayAudio (AudiosManager.AudioType.howmany);
		yield return new WaitForSeconds (2);
		PersistentData.Instance.audios.PlayAudio (AudiosManager.AudioType.one);
		state = states.CUANTAS;
	}
	IEnumerator Done()
	{		
		isDone = true;
		state = states.DONE;
		PersistentData.Instance.audios.PlayAudio (AudiosManager.AudioType.ok);
		PersistentData.Instance.num_of_vaccines = num;
		yield return new WaitForSeconds (1.2f);
		PlayNum (num);
		yield return new WaitForSeconds (2);
		NextScene ();
	}

	states lastState ;
	bool CheckChangeState(states newState)
	{
		if (lastState == newState)
			return false;
		lastState = newState;
		return true;
	}

	void OnDestroy()
	{
		InputManager.Instance.OnInput -= OnInput;
	}
	
	void OnInput (InputManager.types type) 
	{		
		if (isDone)
			return;
		switch (type) {
		case InputManager.types.SWIPE_RIGHT:
			SwipeRight ();
			break;
		case InputManager.types.SWIPE_LEFT:
			SwipeLeft ();
			break;
		case InputManager.types.GATILLO_DOWN:
			if (timer > 4)
				StartCoroutine( Done() );
			break;
		}
	}
	void NextScene()
	{
		timeManager.Init ();
	}

}
