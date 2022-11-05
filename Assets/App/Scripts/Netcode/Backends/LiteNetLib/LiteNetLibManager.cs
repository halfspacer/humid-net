using System;
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
}
