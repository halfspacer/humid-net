using UnityEngine;

namespace App.Scripts.Netcode.Base {
    public abstract class NetworkedMonobehaviour : MonoBehaviour {
        private NetworkManager _networkManager;
        public NetworkManager NetManager {
            get {
              if (_networkManager == null) {
                  _networkManager = Global.Instance.NetworkManager;
              }  
              return _networkManager;
            }
            private set => _networkManager = value;
        }
    }
}