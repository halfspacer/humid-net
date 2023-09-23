using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using App.Scripts.Netcode.Base;
using App.Scripts.Netcode.Helpers;
using App.Scripts.Netcode.Interfaces;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

public class LiteNetLibManager : NetworkManager, IInitialize, IUninitialize, ITick {
    private NetManager _client;
    private Queue<ReceivedData> _receivedData = new Queue<ReceivedData>();
    private NetPeer _localPeer;

    public void Initialize(Action<ResultData> onComplete = null) {
        var listener = new EventBasedNetListener();
        _client = new NetManager(listener);
        if (_client.Start(9050)) {
            Debug.Log("Server started");
            
            listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) => {
                var data = dataReader.RawData;
                _receivedData.Enqueue(new ReceivedData {
                    success = true,
                    fromUserId = fromPeer.EndPoint.ToString(),
                    data = data
                });
            };
            
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
            
            //Run again to also initialize as a client
            Initialize(onComplete);
        } else {
            Debug.Log("Client start failed");
            _client.Start();
            listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) => {
                var data = dataReader.RawData;
                _receivedData.Enqueue(new ReceivedData {
                    success = true,
                    fromUserId = fromPeer.EndPoint.ToString(),
                    data = data
                });
            };

            listener.PeerConnectedEvent += peer => {
                
                OnPlayersChanged?.Invoke();
            };
            
            listener.PeerDisconnectedEvent += (peer, info) => {
                OnPlayersChanged?.Invoke();
            };
            
            _localPeer = _client.Connect("localhost" /* host ip or name */, 9050 /* port */, "SomeConnectionKey" /* text key or NetDataWriter */);
            Debug.Log("Initialized");

            onComplete?.Invoke(new ResultData {
                result = Results.Success,
                message = "Initialized"
            });
        }
    }

    public Action<ResultData> OnInitializeComplete { get; set; }

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

    public override event Action OnPlayersChanged;

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
         _client.ConnectedPeerList.Where(x => x.EndPoint.ToString() == userId).ToList().ForEach(x => {
             var deliveryMethod = reliability switch {
                 PacketReliability.UnreliableUnordered => DeliveryMethod.Unreliable,
                 PacketReliability.ReliableUnordered => DeliveryMethod.ReliableUnordered,
                 PacketReliability.ReliableOrdered => DeliveryMethod.ReliableOrdered,
                 _ => DeliveryMethod.ReliableOrdered
             };

             var writer = new NetDataWriter();
             writer.Put(data);
             x.Send(writer, deliveryMethod);
         });
    }

    private protected override void SendToAllInternal(byte[] data, PacketReliability reliability) {
        _client.SendToAll(data, reliability switch {
            PacketReliability.UnreliableUnordered => DeliveryMethod.Unreliable,
            PacketReliability.ReliableUnordered => DeliveryMethod.ReliableUnordered,
            PacketReliability.ReliableOrdered => DeliveryMethod.ReliableOrdered,
            _ => DeliveryMethod.ReliableOrdered
        });
    }

    protected override int GetPacketQueueSize() => _receivedData.Count;

    private protected override ReceivedData ReceiveInternal() {
        return _receivedData.Count > 0 ? _receivedData.Dequeue() : new ReceivedData();
    }

    public override IEnumerable<string> GetRemotePlayerIds() => _client.ConnectedPeerList.Select(x => x.EndPoint.ToString());
    public override string GetLocalPlayerId() => _localPeer.EndPoint.ToString();

    public override string GetHostId() => "localhost"; 
}
