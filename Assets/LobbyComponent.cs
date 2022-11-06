using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyComponent : MonoBehaviour {
    [SerializeField] private TMP_Text lobbyName;
    [SerializeField] private TMP_Text lobbyPlayers;

    public void SetLobby(LobbyData data) {
        lobbyName.text = data.name;
        lobbyPlayers.text = $"{data.playerCount}/{data.maxPlayers}";
    }
}