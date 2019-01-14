using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//kzlukos@gmail.com
//Synchronizes state over network
public class StateNetworkSynchronizer : NetworkBehaviour
{
    public System.Action<GameState> OnServerStateChange = delegate { };

    // 
    [ClientRpc]
    public void RpcUpdateClientState(GameState state)
    {
		print (" RpcUpdateClientState State: " + state);
        OnServerStateChange(state);
    }

}
