using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentData : MonoBehaviour {

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
	public static PersistentData Instance { get; protected set; }

	void Awake()
	{
		Instance = this;
		print ("______ arranca");
		DontDestroyOnLoad (this.gameObject);
		audios = GetComponent<AudiosManager> ();
		serverLogin = GetComponent<ServerLogin> ();
		serverLogin.Init ();
	}
	void OnDestroy()
	{
		print ("se rompe");
	}

}
