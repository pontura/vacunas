using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.VR;
using UnityEngine.SceneManagement;

// kzlukos@gmail.com
// NetworkManager derived class which implements network callbacks and defines connecting/disconnecting procedures
public class CustomNetworkManager : NetworkManager {

	// Singleton
	private static CustomNetworkManager _instance;
	public static CustomNetworkManager Instance
	{
		get { return _instance; }
	}

	//
    [SerializeField]
    private CustomNetworkDiscovery networkDiscovery;
    public NetworkMode NetworkMode { get; private set; }


    //
    private void OnSceneLoadedHandler(Scene scene, LoadSceneMode mode)
    {
		if(scene.name == offlineScene)
            EstablishConnection();
		
		if (scene.name == onlineScene && NetworkMode == NetworkMode.Server) 
		{
			if (!networkDiscovery.running) {
				networkDiscovery.StartAsServer ();
			}
		}
    }

    //
    private void Start()
    {
        _instance = this;
		DontDestroyOnLoad(gameObject);
		SceneManager.sceneLoaded += OnSceneLoadedHandler;
		EstablishConnection ();
    }

    //
    private void EstablishConnection()
    {

        if (AppController.Instance.Standalone)
        { 
            StartStandalone();
        }
        else
        {

            #if UNITY_EDITOR
                StartInClientMode();

            #elif UNITY_STANDALONE
                StartInServerMode();

            #else
                if (UnityEngine.XR.XRSettings.enabled)
                {
                    StartInClientMode();
                }
                else
                {
                    StartInServerMode();
                }
            #endif

        }
    }

    //
    public void StartInServerMode()
    {
        NetworkMode = NetworkMode.Server;
        StartServer();
    }

    //
    public void StartInClientMode()
    {
        NetworkMode = NetworkMode.Client;
        networkDiscovery.Initialize ();
		networkDiscovery.OnClientFound += OnClientFoundHandler;
		networkDiscovery.StartAsClient ();
    }

    //
    public void StartStandalone()
    {
        NetworkMode = NetworkMode.StandAlone;
        StartServer();
    }

    //
    public void OnClientFoundHandler(string serverIP)
    {
        networkAddress = serverIP;
        networkDiscovery.OnClientFound -= OnClientFoundHandler;
        networkDiscovery.StopBroadcast();
        StartClient();
    }

	//
	public override void OnServerDisconnect(NetworkConnection conn)
    {
		base.OnServerDisconnect (conn);
        StopServer();
    }
		

}

