using System;
using System.Collections;
using System.Collections.Generic;
using App.Scripts.Netcode.Helpers;
using App.Scripts.Netcode.Interfaces;
using UnityEngine;

namespace App.Scripts.Netcode.Base {
    public abstract class NetworkManager : MonoBehaviour {
        private bool _isInitialized;
        private bool _isAuthenticated;
        private LobbyData _currentLobby;
        private List<NetworkedMonobehaviour> _networkedMonobehaviours = new List<NetworkedMonobehaviour>();
        
        public void Subscribe<T>(T networkedMonobehaviour) where T : NetworkedMonobehaviour {
            _networkedMonobehaviours.Add(networkedMonobehaviour);
        }
        
        public void Unsubscribe<T>(T networkedMonobehaviour) where T : NetworkedMonobehaviour {
            _networkedMonobehaviours.Remove(networkedMonobehaviour);
        }

        private void OnEnable() {
            StartCoroutine(InitializeCoroutine());
            IEnumerator InitializeCoroutine() {
                //Keep alive
                DontDestroyOnLoad(this);

                //Cache as ITick if class implements ITick (Called in Update so we don't want to cast it repeatedly)
                if (this is ITick) {
                    _tick = this as ITick;
                }

                //Initialize if class implements IIInitialize
                if (this is IInitialize) {
                    (this as IInitialize)?.Initialize((results) => {
                        if (results.result == Results.Success) {
                            _isInitialized = true;
                        }
                        else {
                            Debug.LogError("Failed to initialize network manager");
                        }
                    });
                }
                else {
                    _isInitialized = true;
                }

                //Wait for initialization
                while (!_isInitialized) {
                    yield return null;
                }

                //Authenticate if class implements IAuthenticate
                if (this is IAuthenticate) {
                    (this as IAuthenticate)?.Authenticate(results => {
                        if (results.result == Results.Success) {
                            Debug.Log("Authenticated");
                            _isAuthenticated = true;
                        }
                        else {
                            Debug.LogError("Failed to authenticate");
                        }
                    });
                }
                else {
                    _isAuthenticated = true;
                }

                //Wait for authentication
                while (!_isAuthenticated) {
                    yield return null;
                }

                //Process _callQueue
                while (_callQueue.Count > 0) {
                    var call = _callQueue.Dequeue();
                    var callTime = call.time;
                    //If call is older than 5 seconds, ignore it
                    if (DateTime.Now - callTime > TimeSpan.FromSeconds(5)) {
                        continue;
                    }

                    //Call method
                    call.callback?.Invoke();
                }
            }
        }

        private ITick _tick;
        private void Update() {
            _tick?.Tick();
            Receive();
        }
        
        private void OnDisable() {
            if (!gameObject.activeSelf) return;
            //Uninitialize if class implements IUninitialize
            (this as IUninitialize)?.Uninitialize();
        }
        
        private protected abstract void GetLobbyListInternal(int maxResults = 10, Action<Results, List<LobbyData>> callback = null);
        private struct CallQueue {
            public DateTime time;
            public Action callback;
        }
        private Queue<CallQueue> _callQueue = new Queue<CallQueue>();
        public void GetLobbyList(int maxResults = 10, Action<Results, List<LobbyData>> callback = null) {
            if (!_isInitialized || !_isAuthenticated) {
                Debug.Log("Network manager is not yet initialized or authenticated");
                _callQueue.Enqueue(new CallQueue {
                    time = DateTime.Now,
                    callback = () => GetLobbyList(maxResults, callback)
                });
                return;
            }
            
            GetLobbyListInternal(10, callback);
        }
        
        private protected abstract void CreateLobbyInternal(string lobbyName, Action<Results, LobbyData> callback = null);
        public void CreateLobby(string lobbyName, Action<Results, LobbyData> callback = null) {
            if (!_isInitialized || !_isAuthenticated) {
                _callQueue.Enqueue(new CallQueue {
                    time = DateTime.Now,
                    callback = () => CreateLobby(lobbyName, callback)
                });
                return;
            }
            
            CreateLobbyInternal(lobbyName, ((results, data) => {
                //Set current lobby
                _currentLobby = data;
                
                //Call callback
                callback?.Invoke(results, data);
            }));
        }
        
        private protected abstract void JoinLobbyInternal(string lobbyId, Action<Results, LobbyData> callback = null);
        public void JoinLobby(string lobbyId, Action<Results, LobbyData> callback = null) {
            if (!_isInitialized || !_isAuthenticated) {
                _callQueue.Enqueue(new CallQueue {
                    time = DateTime.Now,
                    callback = () => JoinLobby(lobbyId, callback)
                });
                return;
            }
            
            JoinLobbyInternal(lobbyId, ((results, lobby) => {
                //Set current lobby
                _currentLobby = lobby;

                //Call callback
                callback?.Invoke(results, lobby);
            }));
        }
        
        private protected abstract void LeaveLobbyInternal(string lobbyId, Action<Results> callback = null);
        public void LeaveLobby(string lobbyId, Action<Results> callback = null) {
            if (!_isInitialized || !_isAuthenticated) {
                _callQueue.Enqueue(new CallQueue {
                    time = DateTime.Now,
                    callback = () => LeaveLobby(lobbyId, callback)
                });
                return;
            }
            
            LeaveLobbyInternal(lobbyId, ((results) => {
                //Set current lobby to new lobby
                _currentLobby = new LobbyData();

                //Call callback
                callback?.Invoke(results);
            }));
        }
        
        private protected abstract void InitP2PInternal(Action<Results> callback = null);
        public void InitP2P(Action<Results> callback = null) {
            if (!_isInitialized || !_isAuthenticated) {
                _callQueue.Enqueue(new CallQueue {
                    time = DateTime.Now,
                    callback = () => InitP2P(callback)
                });
                return;
            }
            
            InitP2PInternal(callback);
        }
        
        private protected abstract void SendInternal(string userId, byte[] data, PacketReliability reliability);
        public void Send(string userId, byte[] data, PacketReliability reliability) {
            if (!_isInitialized || !_isAuthenticated || string.IsNullOrEmpty(_currentLobby.uuid)) {
                return;
            }
            
            SendInternal(userId, data, reliability);
        }
        
        private protected abstract void SendToAllInternal(byte[] data, PacketReliability reliability);
        public void SendToAll(byte[] data, PacketReliability reliability) {
            if (!_isInitialized || !_isAuthenticated || string.IsNullOrEmpty(_currentLobby.uuid)) {
                return;
            }
            
            SendToAllInternal(data, reliability);
        }
        
        public struct ReceivedData {
            public bool success;
            public string fromUserId;
            public byte[] data;
        }
        
        private ReceivedData _badData = new ReceivedData {
            success = false
        };
        
        private protected abstract ReceivedData ReceiveInternal();
        private void Receive() {
            if (!_isInitialized || !_isAuthenticated || string.IsNullOrEmpty(_currentLobby.uuid)) {
                return;
            }
            
            var receivedData = ReceiveInternal();
            if (!receivedData.success) {
                return;
            }

            foreach (var networkedMonobehaviour in _networkedMonobehaviours) {
                networkedMonobehaviour.OnDataReceived(receivedData);
            }
        }
    }
}
