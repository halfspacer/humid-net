using System.Collections;
using System.Collections.Generic;
using System.Linq;
using App.Scripts.Netcode.Base;
using App.Scripts.Utils;
using UnityEngine;

public class Global : Singleton<Global> {
    public NetworkManager NetworkManager;

    protected override void Awake() {
        base.Awake();
        if (NetworkManager != null) {
            return;
        }

        NetworkManager = FindObjectsOfType<NetworkManager>().FirstOrDefault(x => x.enabled);
        if (NetworkManager != null) {
            return;
        }

        Debug.LogError("No NetworkManager found");
    }
}