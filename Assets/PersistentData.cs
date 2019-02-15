using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentData : Singleton<PersistentData> {

	public bool DEBBUGER;
	public int num_of_vaccines;
	public AudiosManager audios;
	public bool langSelected;
	public languages lang;
	public ServerLogin serverLogin;
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
		serverLogin = GetComponent<ServerLogin> ();
		serverLogin.Init ();
		DontDestroyOnLoad (this.gameObject);
	}

}
