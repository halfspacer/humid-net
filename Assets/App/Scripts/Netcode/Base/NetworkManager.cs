using System.Collections;
using App.Scripts.Netcode.Helpers;
using App.Scripts.Netcode.Interfaces;
using UnityEngine;

namespace App.Scripts.Netcode.Base {
    public abstract class NetworkManager : MonoBehaviour {
        private IEnumerator Start() {
            var isInitialized = false;
            if (this is IInitialize) {
                (this as IInitialize)?.Initialize((results) => {
                    if (results.result == Results.Success) {
                        isInitialized = true;
                    } else {
                        Debug.LogError("Failed to initialize network manager");
                    }
                });
            }
            else {
                isInitialized = true;
            }
        
            while (!isInitialized) {
                yield return null;
            }
        
            var isAuthenticated = false;
            if (this is IAuthenticate) {
                (this as IAuthenticate)?.Authenticate(results => {
                    if (results.result == Results.Success) {
                        Debug.Log("Authenticated");
                        isAuthenticated = true;
                    } else {
                        Debug.LogError("Failed to authenticate");
                    }
                });
            }
            else {
                isAuthenticated = true;
            }
        
            while (!isAuthenticated) {
                yield return null;
            }
            
            if (this is ITick) {
                _tick = this as ITick;
            }
        }

        private ITick _tick;
        private void Update() {
            _tick?.Tick();
        }
        
        private void OnDestroy() {
            (this as IUninitialize)?.Uninitialize();
        }
    }
}
