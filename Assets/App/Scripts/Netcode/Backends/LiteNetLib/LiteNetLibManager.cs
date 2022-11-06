using System;
using System.Collections.Generic;
using System.Threading;
using App.Scripts.Netcode.Base;
using App.Scripts.Netcode.Helpers;
using App.Scripts.Netcode.Interfaces;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

public class LiteNetLibManager : NetworkManager, IInitialize, IUninitialize, ITick {
    private NetManager _client;
    
    public void Initialize(Action<ResultData> onComplete = null) {
        var listener = new EventBasedNetListener();
        _client = new NetManager(listener);
        if (_client.Start(9050)) {
            Debug.Log("Server started");
            
            listener.PeerConnectedEvent += peer => {
                Debug.Log(("We got connection: {0}", peer.EndPoint)); // Show peer ip
                var writer = new NetDataWriter();                 // Create writer class
                writer.Put("Hello client!");                                // Put some string
                peer.Send(writer, DeliveryMethod.ReliableOrdered);             // Send with reliability
            };
            
            listener.ConnectionRequestEvent += request => {
                if(_client.ConnectedPeersCount < 10 /* max connections */)
                    request.AcceptIfKey("SomeConnectionKey");
                else
                    request.Reject();
            };
        } else {
            Debug.Log("Client start failed");
            _client.Start();
            listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) => {
                var stringData = dataReader.GetString(100); // Read string with max length 100
                Debug.Log(stringData);
                dataReader.Recycle();
            };
            var netPeer = _client.Connect("localhost" /* host ip or name */, 9050 /* port */, "SomeConnectionKey" /* text key or NetDataWriter */);
        }

        Debug.Log("Initialized");

        onComplete?.Invoke(new ResultData {
            result = Results.Success,
            message = "Initialized"
        });
    }

    public void Tick() {
        _client.PollEvents();
    }

    public void Uninitialize(Action<ResultData> onComplete = null) {
        _client.Stop();
        onComplete?.Invoke(new ResultData {
            result = Results.Success,
            message = "Uninitialized"
        });
    }

    private protected override void GetLobbyListInternal(int maxResults = 10, Action<Results, List<LobbyData>> callback = null) {
        var list = new List<LobbyData>();
        var lobby = new LobbyData {
            name = "Untitled Lobby",
            maxPlayers = 10,
            mapName = "N/A",
            uuid = "localhost",
            playerCount = 1
        };
        list.Add(lobby);
        callback?.Invoke(Results.Success, list);
    }

    private protected override void CreateLobbyInternal(string lobbyName, Action<Results, LobbyData> callback = null) {
        callback?.Invoke(Results.Failure, new LobbyData());
    }
    
    private protected override void JoinLobbyInternal(string lobbyId, Action<Results, LobbyData> callback = null) {
        callback?.Invoke(Results.Failure, new LobbyData());
    }

    private protected override void LeaveLobbyInternal(string lobbyId, Action<Results> callback = null) {
        callback?.Invoke(Results.Failure);
    }

    private protected override void InitP2PInternal(Action<Results> callback = null) {
        callback?.Invoke(Results.Failure);
    }

    private protected override void SendInternal(string userId, byte[] data, PacketReliability reliability) {
         
    }

    private protected override void SendToAllInternal(byte[] data, PacketReliability reliability) {
        
    }

    private protected override ReceivedData ReceiveInternal() {
        return new ReceivedData {
            success = false,
            fromUserId = null,
            data = new byte[] {
            }
        };
    }
}
