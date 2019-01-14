using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// kzlukos@gmail.com
// Broadcasts CLIENT information on the local network
public class CustomNetworkDiscovery : NetworkDiscovery {

    public System.Action<string> OnClientFound = delegate { };

    void Awake()
    {
        Initialize();
        DontDestroyOnLoad(gameObject);
    }

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        OnClientFound(fromAddress);
    }

}
