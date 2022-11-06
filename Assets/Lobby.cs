using System.Collections;
using System.Collections.Generic;
using System.Linq;
using App.Scripts.Netcode.Base;
using App.Scripts.Netcode.Helpers;
using NaughtyAttributes;
using UnityEngine;

public class Lobby : NetworkedMonobehaviour {
    [SerializeField] private GameObject lobbyPrefab;
    [SerializeField] private Transform lobbyParent;
    private void Start() {
        NetManager.GetLobbyList(10, OnLobbyList);
        void OnLobbyList(Results results, List<LobbyData> lobbies) {
            if (results == Results.Success) {
                if (lobbies.Count <= 0) {
                    Debug.Log("No lobbies found");
                }
                
                //Destroy all existing lobbies
                foreach (Transform child in lobbyParent) {
                    Destroy(child.gameObject);
                }

                foreach (var lobby in lobbies) {
                    AddLobby(lobby);
                }
            }
            else {
                Debug.Log("Failed to get lobbies");
            }
        }
    }

    private void AddLobby(LobbyData lobby) {
        var lobbyObject = Instantiate(lobbyPrefab, lobbyParent);
        var lobbyComponent = lobbyObject.GetComponent<LobbyComponent>();
        lobbyComponent.SetLobby(lobby);
    }

    [Button("Refresh")]
    public void RefreshServerList() {
        Start();
    }
    
    [Button("Create Lobby (Debug)")]
    public void CreateLobbyDebug() {
        NetManager.CreateLobby("Test Lobby #" + Random.Range(0, 1000), (results, lobbyData) => {
            AddLobby(lobbyData);
        });
    }

    public override void OnDataReceived(NetworkManager.ReceivedData networkEvent) {
        
    }
}