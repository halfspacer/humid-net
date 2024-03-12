using System;
using System.Collections;
using Epic.OnlineServices;
using Epic.OnlineServices.P2P;
using UnityEngine;
using Utils;

namespace App.Scripts.Netcode.Base {
    public abstract class NetworkedMonobehaviour : MonoBehaviour {
        private NetworkManager networkManager;
        protected NetSerializer serializer = new NetSerializer();
        protected float updateInterval => netManager.GetTickRate();
        protected PacketReliability reliable = PacketReliability.ReliableOrdered;
        protected PacketReliability unreliable = PacketReliability.UnreliableUnordered;
        protected PacketReliability reliableUnordered = PacketReliability.ReliableUnordered;
        
        public void ClearSerializerBuffer() {
            serializer.ReturnBuffers();
        }

        protected NetworkManager netManager {
            get {
              if (networkManager == null) {
                  networkManager = Global.Instance.NetworkManager;
              }  
              return networkManager;
            }
            private set => networkManager = value;
        }

        public virtual void OnEnable() {
            netManager.Subscribe(this);
        }
        
        public virtual void OnDisable() {
            netManager.Unsubscribe(this);
        }

        public abstract void OnDataReceived(ref Result result, ref ProductUserId fromUserId,
            ref ArraySegment<byte> bytes);
        
        public void OnDataReceivedInternal(ref Result result, ref ProductUserId fromUserId, ref ArraySegment<byte> bytes) {
            serializer.Reset(true);
            OnDataReceived(ref result, ref fromUserId, ref bytes);
        }

        protected void Send(ref ArraySegment<byte> data, ref PacketReliability reliability) {
            netManager.SendToAll(ref data, ref reliability);
        }
    }
}