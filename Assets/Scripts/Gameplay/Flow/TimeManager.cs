using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// kzlukos@gmail.com
// Object responsible for changeing game state over time 
// (executed accordingly to the scenario defined by TimeSection objects collection)
public class TimeManager : Singleton<TimeManager>
{
    // Standalone only
    //[SerializeField] float autoplayTime = 10f; 
    [SerializeField] NarrationController narrationController;
    [Header("Time sections")]
    [SerializeField] TimeSection[] _timeSections;

	public Data data;

    public System.Action<int> onRepetitionsNumberChange = delegate { };
    public System.Action<bool> onPauseToggle = delegate { };
    public System.Func<bool> customConditions;

	public int inPause = 0;

    #region PROPERTIES
    //
    public bool Pause
    {
        get { return _pause; }
        set
        {				
            _pause = value;

			if (inPause == 1 || inPause == 2) {
				GameController.Instance.CurrentState = GameState.TransferSoundsFading;
				_pause = false;
				inPause = 0;
			} 

            onPauseToggle(_pause);

        }
    }

    //
    public bool Lock { get; set; }

    //
	public int Repetitions;
	public GameObject masker;

    #endregion

    int _repetitions;
    bool _pause = false;
    IEnumerator _sectionWaitCoroutine;

	public bool started;
	int vaccineID;

	void Start()
	{
		SoundFXManager.Instance.OnSoundFX ("start");

		GameController.Instance.OnStateChange += OnStateChangeHandler;
		InputManager.Instance.OnInput += OnInput;

		Reset ();
	}
	public void Reset()
	{
		vaccineID = 0;
		started = false;
		masker.SetActive (false);
	}
	public void Init()
	{
		started = true;
		Repetitions = PersistentData.Instance.num_of_vaccines;
		Pause = false;
		Lock = false;

		Invoke ("AutoStartCoroutine", 2);
		Invoke ("Delayed", 0.5f);
	}
	void Delayed()
	{
		Pause = false;
		Lock = false;
	}
	void OnDestroy()
	{
		InputManager.Instance.OnInput -= OnInput;
		GameController.Instance.OnStateChange -= OnStateChangeHandler;
		CancelInvoke ();
	}
	void OnInput(InputManager.types type)
	{
		if (!started)
			return;
//		if(type == InputManager.types.GATILLO_HOLD)
//		{
//			SoundFXManager.Instance.OnSoundFX ("reset");
//			UnityEngine.XR.InputTracking.Recenter();
//		}
		if(type == InputManager.types.PAD_DOWN)
		{
			Pause = !Pause;
			if(_pause)
				SoundFXManager.Instance.OnSoundFX ("unpause");
			else
				SoundFXManager.Instance.OnSoundFX ("pause");

		} else if(type == InputManager.types.PAD_HOLD)
		{
			ResetScene ();
		}
	}
	void ResetScene()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene("002_Main");
	}
	private void OnStateChangeHandler(GameState prev, GameState current) 
	{
		if (current == GameState.RepeatLast)
			inPause = 1;
		else if (current == GameState.RepeatNewVaccine3)
			inPause = 2;
		else if (current == GameState.None)
			return;
		else if (current == GameState.Restart) {
			//Reset ();
			ResetScene ();
			//GameController.Instance.CurrentState = GameState.None;   
			//data.Restart ();
			return;
		} else if (current == GameState.Init) {
			inPause = 0;
			Lock = false;
		}

        if (_sectionWaitCoroutine != null)
            StopCoroutine(_sectionWaitCoroutine);

		TimeSection ts = FindTimeSection (current);
		_sectionWaitCoroutine = WaitForSectionEnd(ts);
        StartCoroutine(_sectionWaitCoroutine);

		print (current + " time: " + ts.duration + " ts: " + ts);
    }

	//
	private IEnumerator WaitForSectionEnd(TimeSection timeSection) 
	{		
		// Wait for min number of secods given by the timeSection
		yield return new WaitForSeconds (timeSection.duration);

		// Check if narration controller is playing
		yield return new WaitUntil (() => !narrationController.IsPlaying);

		// Check if paused
		if (timeSection.state == timeSection.fallbackState || timeSection.fallbackState == GameState.None) {
			//yield return new WaitUntil (() => (!Pause && !Lock));
			yield return new WaitUntil (() => (!Pause));
		}
		//print ("END: " + GameController.Instance.CurrentState);
        if ((GameController.Instance.CurrentState == GameState.TransferSoundsFading) && Repetitions > 1)
        {
			print (GameController.Instance.CurrentState + " : vaccineID " + vaccineID + "  Repetitions " + Repetitions);
			if (vaccineID == 0)
				if(Repetitions ==2)
					GameController.Instance.CurrentState = GameState.PausaCorta;
				else
					GameController.Instance.CurrentState = GameState.Repeat;
			else if (vaccineID == 1)
            {
				if(Repetitions ==2)
					GameController.Instance.CurrentState = GameState.Medal;
				else
					GameController.Instance.CurrentState = GameState.RepeatLast;
			} else if(vaccineID == 2)
            {
				if (Repetitions == 2)
					GameController.Instance.CurrentState = GameState.Medal;
				else {
					GameController.Instance.CurrentState = GameState.RepeatNewVaccine;
				}
            }
			else if(vaccineID == 3)
			{
				if(Repetitions ==3)
					GameController.Instance.CurrentState = GameState.Medal;
				else
					GameController.Instance.CurrentState = GameState.RepeatNewVaccine2;
			}
			else if(vaccineID == 4)
			{
				if(Repetitions ==4)
					GameController.Instance.CurrentState = GameState.Medal;
				else
					GameController.Instance.CurrentState = GameState.RepeatNewVaccine3;
			}
			else if(vaccineID == 5)
			{
				GameController.Instance.CurrentState = GameState.NewVaccineDone3;
			}
			else if(vaccineID == 6)
			{
				if(Repetitions ==5)
					GameController.Instance.CurrentState = GameState.Medal;
				else
					GameController.Instance.CurrentState = GameState.NewVaccineDone4;
			}
			else if(vaccineID == 7)
			{
				if(Repetitions ==6)
					GameController.Instance.CurrentState = GameState.Medal;
			}
			vaccineID++;
        }
        else
        {
            GameController.Instance.CurrentState = Pause ? timeSection.fallbackState : timeSection.nextState;
        }

    }

    void AutoStartCoroutine()
    {
		Pause = false;
        GameController.Instance.CurrentState = GameState.Init;        
    }

    private TimeSection FindTimeSection(GameState state) 
	{
		return System.Array.Find (_timeSections, section => section.state == state);
	}

	void OnApplicationFocus(bool hasFocus)
	{
		if (PersistentData.Instance.DEBBUGER)
			return;
		
		masker.SetActive (true);
		ResetScene ();
	}

}