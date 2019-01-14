using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// kzlukos@gmail.com
// Enables & disables particle effect
[RequireComponent(typeof(AudioSource))]
public class TransferEffect : GameStateObserver
{

    [SerializeField] AudioSource startSoundAudioSource;
    [SerializeField] AudioSourceFadeVolume loopAudioSource;
    [SerializeField] private ParticleSystem[] transferParticleSystems;

    //
    void Awake()
    {
        foreach (ParticleSystem particleSystem in transferParticleSystems)
            particleSystem.Stop();
    }

    //
    override protected void OnStateChangeHandler(GameState prev, GameState current)
    {
        StopAllCoroutines();
        foreach (ParticleSystem particleSystem in transferParticleSystems)
            particleSystem.Stop();

        switch(current)
        {
			case GameState.Transfer2Lights:
            case GameState.Transfer:
                StartCoroutine(TransferCoroutine(0f));
                break;
            case GameState.Repeat:
                StartCoroutine(TransferCoroutine(4f));
            	 break;
			case GameState.NewVaccineDone:
				StartCoroutine(TransferCoroutine(2f));
				break;
			case GameState.NewVaccineDone2:
                StartCoroutine(TransferCoroutine(2f));
                break;
			case GameState.RepeatLast2:
				StartCoroutine(TransferCoroutine(2f));
				break;
			case GameState.RepeatNewVaccine4:
				StartCoroutine(TransferCoroutine(2f));
				break;
			case GameState.NewVaccineDone4:
				StopPlayingSounds();
				break;
			case GameState.RepeatNewVaccine2:
				StopPlayingSounds();
				break;
			case GameState.RepeatNewVaccine3:
				StopPlayingSounds();
				break;
			case GameState.Conclusion:
				StopPlayingSounds();
				break;
            case GameState.TransferSoundsFading:
                StopPlayingSounds();
                break;
			case GameState.Init:
                StopPlayingSounds();
                break;
        }

    }

    //
    void PlaySounds()
    {
        startSoundAudioSource.Play();
        loopAudioSource.Play = true;
    }

    //
    void StopPlayingSounds()
    {
        startSoundAudioSource.Stop();
        loopAudioSource.Play = false;
    }

    //
    private IEnumerator TransferCoroutine(float delay)
    {

        yield return new WaitForSeconds(delay);

        PlaySounds();

        foreach (ParticleSystem particleSystem in transferParticleSystems)
            particleSystem.Play();
    }


}
