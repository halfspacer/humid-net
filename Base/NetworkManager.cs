using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using App.Modules.Netcode.Utils;
using App.Scripts.Netcode.Helpers;
using App.Scripts.Netcode.Interfaces;
using UnityEngine;

namespace App.Scripts.Netcode.Base {
    public abstract class NetworkManager : MonoBehaviour {
        private bool _isInitialized;
        private bool _isAuthenticated;
        private LobbyData _currentLobby;
        private List<NetworkedMonobehaviour> _networkedMonobehaviours = new List<NetworkedMonobehaviour>();
        private NetSerializer _serializer = new NetSerializer();
        public abstract event Action OnPlayersChanged;
        public IEnumerable players = new List<string>();

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
                            Debug.LogError("Failed to initialize network manager: " + results.message);
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
            players = GetRemotePlayerIds();
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

        private byte[] AppendUserId(byte[] data) {
            //Append the user id to the beginning of the data
            var playerId = GetLocalPlayerId();
            var playerIdBytesFromString = Encoding.UTF8.GetBytes(playerId);
            var dataWithId = new byte[playerIdBytesFromString.Length + 1 + data.Length];
            dataWithId[0] = (byte) playerIdBytesFromString.Length;
            Array.Copy(playerIdBytesFromString, 0, dataWithId, 1, playerIdBytesFromString.Length);
            Array.Copy(data, 0, dataWithId, playerIdBytesFromString.Length + 1, data.Length);
            return dataWithId;
        }

        private protected abstract void SendToAllInternal(byte[] data, PacketReliability reliability);
        public void SendToAll(byte[] data, PacketReliability reliability) {
            if (!_isInitialized || !_isAuthenticated || string.IsNullOrEmpty(_currentLobby.uuid)) {
                return;
            }
            
            if (GetHostId() != GetLocalPlayerId()) {
                var newDataWithId = AppendUserId(data);
                //Add byte 255 to the start of the data to indicate that this is a broadcast message for the host, and
                //add reliability to the second byte
                var newData = new byte[newDataWithId.Length + 2];
                newData[0] = 255;
                newData[1] = (byte) reliability;
                var host = GetHostId();
                Array.Copy(newDataWithId, 0, newData, 2, newDataWithId.Length);
                Send(host, newData, reliability);
                return;
            }
            
            var dataWithId = AppendUserId(data);
            SendToAllInternal(dataWithId, reliability);
        }
        
        public struct ReceivedData {
            public bool success;
            public string fromUserId;
            public byte[] data;
        }
        
        private ReceivedData _badData = new ReceivedData {
            success = false
        };
        
        private ReceivedData _noData = new ReceivedData {
            success = true
        };
        
        protected abstract int GetPacketQueueSize();
        
        private protected abstract ReceivedData ReceiveInternal();
        private void Receive() {
            if (!_isInitialized || !_isAuthenticated || string.IsNullOrEmpty(_currentLobby.uuid)) {
                return;
            }

            while (GetPacketQueueSize() > 0) {
                var receivedData = ReceiveInternal();
                if (!receivedData.success) {
                    Debug.Log("Failed to receive data");
                    return;
                }
                
                if (receivedData.data == null || receivedData.data.Length == 0) {
                    return;
                }

                //Only accept packets from the host if not the host ourselves
                if (GetHostId() != GetLocalPlayerId() && receivedData.fromUserId != GetHostId()) return;
                
                bool IsHostAndPacketBroadcast() => receivedData.data[0] == 255 && GetHostId() == GetLocalPlayerId() &&
                                receivedData.fromUserId != GetLocalPlayerId();
                
                //Check if first byte is 255
                byte[] newData;
                if (IsHostAndPacketBroadcast()) {
                    //Second byte is the packet type
                    var packetType = (PacketReliability) receivedData.data[1];
                    
                    //Strip first two bytes
                    newData = new byte[receivedData.data.Length - 2];

                    Array.Copy(receivedData.data, 2, newData, 0, receivedData.data.Length - 2);
                    receivedData.data = newData;
                    
                    foreach (var player in GetRemotePlayerIds()) {
                        if (player == receivedData.fromUserId) continue;
                        Send(player, newData, packetType);
                    }
                }
                
                var userIdLength = (int) receivedData.data[0];

                //Strip first byte and the user id
                newData = new byte[receivedData.data.Length - userIdLength - 1];

                //Get the user id
                var userId = Encoding.UTF8.GetString(receivedData.data, 1, userIdLength);
                
                Array.Copy(receivedData.data, 1 + userIdLength, newData, 0, newData.Length);
                
                receivedData.fromUserId = userId;
                receivedData.data = newData;

                foreach (var networkedMonobehaviour in _networkedMonobehaviours) { 
                    networkedMonobehaviour.OnDataReceived(receivedData);
                } 
            } 
        }

        public abstract IEnumerable<string> GetRemotePlayerIds();
        public abstract string GetLocalPlayerId();
        public abstract string GetHostId();
    }
}
