using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//kzlukos@gmail.com
//Queries server for selected language and replaces main (debug) camera with its own camera
public class NetworkPlayer : NetworkBehaviour {

    //
    void Start()
    {
		Camera cameraComponent = transform.GetComponentInChildren<Camera>();
        foreach (Camera camera in FindObjectsOfType<Camera>()) {
            if (camera != cameraComponent)
                Destroy(camera.gameObject);
        }
		if (isLocalPlayer)
			CmdClientInit ();
    }

	[Command]
	private void CmdClientInit() 
	{
		//GameController.Instance.CurrentState = GameState.Init;
		//Settings.Instance.NetworkPushSettings ();
	}

}
