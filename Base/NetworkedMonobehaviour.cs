using System.Collections;
using App.Modules.Netcode.Utils;
using UnityEngine;

namespace App.Scripts.Netcode.Base {
    public abstract class NetworkedMonobehaviour : MonoBehaviour {
        private NetworkManager _networkManager;
        protected NetSerializer serializer;
        private const float UpdateInterval = 0.1f;
        private readonly WaitForSeconds _updateIntervalWait = new(UpdateInterval);

        protected NetworkManager NetManager {
            get {
              if (_networkManager == null) {
                  _networkManager = Global.Instance.NetworkManager;
              }  
              return _networkManager;
            }
            private set => _networkManager = value;
        }

        public void OnEnable() {
            NetManager.Subscribe(this);
            StartCoroutine(NetworkUpdate());
        }
        
        public void OnDisable() {
            NetManager.Unsubscribe(this);
            StopCoroutine(NetworkUpdate());
        }
        
        public abstract void OnDataReceived(NetworkManager.ReceivedData networkEvent);
        
        protected abstract void OnNetworkUpdate();
        
        private IEnumerator NetworkUpdate() {
            while (true) {
                OnNetworkUpdate();
                yield return _updateIntervalWait;
            }
        }

        protected void Send(byte[] data, PacketReliability reliability) => NetManager.SendToAll(data, reliability);
    }
}