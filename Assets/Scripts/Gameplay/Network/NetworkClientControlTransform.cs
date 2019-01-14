using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//kzlukos@gmail.com
//
[NetworkSettings(channel = 0, sendInterval = 0.04f)]
public class NetworkClientControlTransform : NetworkBehaviour {

	[SerializeField] private Transform controlledTransform; //reference must point on the same object in both client & server
	[SerializeField] private bool synchronizePosition = true;
	[SerializeField] private bool synchronizeRotation = true;

    //
    private void Update()
    {
        if(isLocalPlayer) 
		{
            if(synchronizePosition)
				CmdSyncPosition(controlledTransform.position);
            if(synchronizeRotation)
				CmdSyncRotation(controlledTransform.rotation);
        }
    }

    //
    [Command]
    void CmdSyncRotation(Quaternion rotation)
    {
		controlledTransform.rotation = rotation;
    }

    //
    [Command]
    void CmdSyncPosition(Vector3 position)
    {
		controlledTransform.position = position;
    }

}
