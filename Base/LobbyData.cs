using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct LobbyData {
    public string name;
    public int playerCount;
    public int maxPlayers;
    public string mapName;
    public string uuid;
    public string hostId;
    
    public LobbyData(string name, int playerCount, int maxPlayers, string mapName, string hostAddress) {
        this.name = name;
        this.playerCount = playerCount;
        this.maxPlayers = maxPlayers;
        this.mapName = mapName;
        this.uuid = hostAddress;
        this.hostId = hostAddress;
    }
}
