using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// kzlukos@gmail.com
// Used to dertermine if application is running in server or client mode (or both)
public enum NetworkMode
{
    Server,
    Client,
    StandAlone
}