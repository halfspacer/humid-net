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
        
        private protected abstract void CreateLobbyInternal(string lobbyName, Action<Results> callback = null);
        public void CreateLobby(string lobbyName, Action<Results> callback = null) {
            if (!_isInitialized || !_isAuthenticated) {
                Debug.Log("Network manager is not yet initialized or authenticated");
                _callQueue.Enqueue(new CallQueue {
                    time = DateTime.Now,
                    callback = () => CreateLobby(lobbyName, callback)
                });
                return;
            }
            
            CreateLobbyInternal(lobbyName, callback);
        }
    }
}
