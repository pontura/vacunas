using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudiosManager : MonoBehaviour {

	public AudioSource audioSource;

	[Serializable]
	public class Audios
	{
		public PersistentData.languages lang;
		public AudioClip langName;
		public AudioClip howmany;
		public AudioClip selectLang;
		public AudioClip ok;
		public AudioClip one;
		public AudioClip two;
		public AudioClip three;
		public AudioClip four;
		public AudioClip five;
		public AudioClip six;
	}

	public enum AudioType
	{
		langName,
		howmany,
		selectLang,
		ok,
		one,
		two,
		three,
		four,
		five,
		six
	}
	public Audios[] audios;

	public void PlayAudio(AudioType audioType)
	{
		Audios audios = GetAudioDataByLang (PersistentData.Instance.lang);
		AudioClip audioClip = null;
		switch (audioType) {
		case AudioType.langName:
			audioClip = audios.langName;
			break;
		case AudioType.howmany:
			audioClip = audios.howmany;
			break;
		case AudioType.selectLang:
			audioClip = audios.selectLang;
			break;
		case AudioType.ok:
			audioClip = audios.ok;
			break;
		case AudioType.one:
			audioClip = audios.one;
			break;
		case AudioType.two:
			audioClip = audios.two;
			break;
		case AudioType.three:
			audioClip = audios.three;
			break;
		case AudioType.four:
			audioClip = audios.four;
			break;
		case AudioType.five:
			audioClip = audios.five;
			break;
		case AudioType.six:
			audioClip = audios.six;
			break;
		}
		if(audioClip!=null)
		{
			audioSource.clip = audioClip;
			audioSource.Play ();
		}
	}

	public Audios GetAudioDataByLang(PersistentData.languages lang)
	{
		foreach(Audios audioData in audios)
		{
			if (audioData.lang == lang)
				return audioData;
		}
		Debug.LogError ("No existe el audio del lang " + lang);
		return null;
	}
}
