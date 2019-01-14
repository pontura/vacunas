using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.Networking;

// kzlukos@gmail.com
// NetworkBehaviour which updates local player settings via OnServerChangeSettings()
public class SettingsNetworkSynchronizer : NetworkBehaviour {

	public System.Action<Lang, float> OnServerChangeSettings = delegate { };

    // 
    [ClientRpc]
	public void RpcUpdateClientSettings(Lang lang, float audioVolume)
    {
		OnServerChangeSettings(lang, audioVolume);
    }

    // 
    [ClientRpc]
    public void RpcResetView()
    {
        UnityEngine.XR.InputTracking.Recenter();
    }

}
