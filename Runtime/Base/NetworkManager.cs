using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using App.Scripts.Netcode.Helpers;
using App.Scripts.Netcode.Interfaces;
using Epic.OnlineServices;
using Epic.OnlineServices.Lobby;
using Epic.OnlineServices.P2P;
using UnityEngine;
using UnityEngine.Events;
using Utils;

namespace App.Scripts.Netcode.Base {
    public abstract class NetworkManager : MonoBehaviour {
        private bool _isInitialized;
        private bool _isAuthenticated;
        private LobbyData _currentLobby;
        private List<NetworkedMonobehaviour> _networkedMonobehaviours = new List<NetworkedMonobehaviour>();
        private List<INetUpdate> _iNetUpdates = new List<INetUpdate>();
        private List<IHostChangedEvent> _iHostChangedEvents = new List<IHostChangedEvent>();
        private List<IUserChangedEvent> _iUserChangedEvents = new List<IUserChangedEvent>();
        private NetSerializer _serializer = new NetSerializer();
        public abstract event Action OnPlayersChanged;
        public IEnumerable<ProductUserId> players = new List<ProductUserId>();
        [SerializeField] private UnityEvent<string> onInitializeComplete;
        [SerializeField] private UnityEvent<string> onAuthenticateComplete;

        public void Subscribe<T>(T networkedMonobehaviour) where T : NetworkedMonobehaviour {
            _networkedMonobehaviours.Add(networkedMonobehaviour);
            if (networkedMonobehaviour is INetUpdate) {
                _iNetUpdates.Add(networkedMonobehaviour as INetUpdate);
            }
            
            if (networkedMonobehaviour is IHostChangedEvent) {
                _iHostChangedEvents.Add(networkedMonobehaviour as IHostChangedEvent);
            }
            
            if (networkedMonobehaviour is IUserChangedEvent) {
                _iUserChangedEvents.Add(networkedMonobehaviour as IUserChangedEvent);
            }
        }
        
        public void Unsubscribe<T>(T networkedMonobehaviour) where T : NetworkedMonobehaviour {
            _networkedMonobehaviours.Remove(networkedMonobehaviour);
            if (networkedMonobehaviour is INetUpdate) {
                _iNetUpdates.Remove(networkedMonobehaviour as INetUpdate);
            }
            
            if (networkedMonobehaviour is IHostChangedEvent) {
                _iHostChangedEvents.Remove(networkedMonobehaviour as IHostChangedEvent);
            }
            
            if (networkedMonobehaviour is IUserChangedEvent) {
                _iUserChangedEvents.Remove(networkedMonobehaviour as IUserChangedEvent);
            }
        }

        private void OnEnable() {
            StartCoroutine(InitializeCoroutine());
            IEnumerator InitializeCoroutine() {
                //Keep alive
                DontDestroyOnLoad(this);

                //Initialize if class implements IIInitialize
                if (this is IInitialize) {
                    (this as IInitialize)?.Initialize((results) => {
                        if (results.result == Results.Success) {
                            _isInitialized = true;
                            (this as IInitialize)?.OnInitializeComplete?.Invoke(results);
                        }
                        else {
                            Debug.LogError("Failed to initialize network manager: " + results.message);
                            (this as IInitialize)?.OnInitializeComplete?.Invoke(results);
                            onInitializeComplete?.Invoke(results.message);
                        }
                    });
                }
                else {
                    _isInitialized = true;
                    (this as IInitialize)?.OnInitializeComplete?.Invoke(new ResultData() {result = Results.Success});
                    onInitializeComplete?.Invoke("Network Manager Successfully Initialized");
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
                            (this as IAuthenticate)?.OnAuthenticateComplete?.Invoke(results);
                            onAuthenticateComplete?.Invoke("Network Manager Successfully Authenticated: " + results.message);
                            onInitializeComplete?.Invoke("Network Manager Successfully Authenticated: " + results.message);
                        }
                        else {
                            Debug.LogError("Failed to authenticate");
                            (this as IAuthenticate)?.OnAuthenticateComplete?.Invoke(results);
                            onAuthenticateComplete?.Invoke("Failed to authenticate: " + results.message);
                        }
                    });
                }
                else {
                    _isAuthenticated = true;
                    (this as IAuthenticate)?.OnAuthenticateComplete?.Invoke(new ResultData() {result = Results.Success});
                    onAuthenticateComplete?.Invoke("Network Manager Successfully Authenticated");
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
                
                //Start update loop
                StartCoroutine(DoUpdate());
            }
        }

        private IEnumerator DoUpdate() {
            while (true) {
                if (_isInitialized && _isAuthenticated) {
                    Receive();
                }
                yield return null;
            }
        }
        
        private void OnDisable() {
            if (!gameObject.activeSelf) return;
            //Uninitialize if class implements IUninitialize
            (this as IUninitialize)?.Uninitialize();
        }

        private void OnApplicationQuit() {
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
                if (results == Results.Success) {
                    _currentLobby = lobby;
                }

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
        
        private protected abstract void SendInternal(ref ProductUserId userId, ref byte[] data, ref PacketReliability reliability);
        public void Send(ref ProductUserId userId, ref byte[] data, ref PacketReliability reliability) {
            if (!_isInitialized || !_isAuthenticated || string.IsNullOrEmpty(_currentLobby.uuid)) {
                return;
            }

            SendInternal(ref userId, ref data, ref reliability);
        }

        private protected abstract void SendToAllInternal(ref ArraySegment<byte> data, ref PacketReliability reliability);
        public void SendToAll(ref ArraySegment<byte> data, ref PacketReliability reliability) {
            if (!_isInitialized || !_isAuthenticated || string.IsNullOrEmpty(_currentLobby.uuid)) {
                return;
            }

            SendToAllInternal(ref data, ref reliability);
        }
        
        protected abstract int GetPacketQueueSize();

        private protected abstract bool ReceiveInternal(out Result result, out ProductUserId id, out ArraySegment<byte> data);
        private void Receive() {
            if (!_isInitialized || !_isAuthenticated || string.IsNullOrEmpty(_currentLobby.uuid)) {
                return;
            }

            while (GetPacketQueueSize() > 0) {
                if (!ReceiveInternal(out var result, out var fromUserId, out var bytes)) {
                    return;
                }
                
                if (bytes.Count == 0) {
                    return;
                }
                
                foreach (var networkedMonobehaviour in _networkedMonobehaviours) {
                    networkedMonobehaviour.OnDataReceivedInternal(ref result, ref fromUserId, ref bytes);
                } 
            } 
        }

        protected void NetworkUpdate() {
            if (!_isInitialized || !_isAuthenticated) {
                return;
            }
            
            foreach (var networkedMonobehaviour in _networkedMonobehaviours) {
                networkedMonobehaviour.ClearSerializerBuffer();
                
                if (networkedMonobehaviour is INetUpdate update) {
                    update.OnNetworkUpdate();
                }
            }
        }

        public abstract void GetRemotePlayerIds(out IEnumerable<ProductUserId> ids);
        public abstract bool GetLocalPlayerId(out ProductUserId id);
        public abstract bool GetHostId(out ProductUserId id);
        public abstract float GetTickRate();
        protected void BroadcastHostUpdate(ProductUserId newHost) {
            if (!_isInitialized || !_isAuthenticated) {
                return;
            }
            
            foreach (var hostChangedEvent in _iHostChangedEvents) {
                hostChangedEvent.OnHostChanged(newHost);
            }
        }
        protected void BroadcastUserUpdate(ProductUserId user, LobbyMemberStatus status) {
            if (!_isInitialized || !_isAuthenticated) {
                return;
            }
            
            foreach (var userChangedEvent in _iUserChangedEvents) {
                if (status == LobbyMemberStatus.Joined) {
                    userChangedEvent.OnUserJoined(user);
                }
                else {
                    userChangedEvent.OnUserLeft(user, status);
                }
            }
        }
    }
}
