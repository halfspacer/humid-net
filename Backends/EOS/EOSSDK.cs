using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using App.Scripts.Netcode.Base;
using App.Scripts.Netcode.Helpers;
using App.Scripts.Netcode.Interfaces;
using Epic.OnlineServices;
using Epic.OnlineServices.Auth;
using Epic.OnlineServices.Connect;
using Epic.OnlineServices.Lobby;
using Epic.OnlineServices.Logging;
using Epic.OnlineServices.P2P;
using Epic.OnlineServices.Platform;
using UnityEditor;
using UnityEngine;
using Credentials = Epic.OnlineServices.Connect.Credentials;
using LoginCallbackInfo = Epic.OnlineServices.Connect.LoginCallbackInfo;
using LoginOptions = Epic.OnlineServices.Connect.LoginOptions;

namespace App.Scripts.Netcode.Backends.EOS {
    // ReSharper disable once InconsistentNaming
    public class EOSSDK : NetworkManager, ITick, IAuthenticate, IInitialize, IUninitialize {
        private const float PlatformTickInterval = 0.1f;

        private static PlatformInterface _platformInterface;

        // Set these values as appropriate. For more information, see the Developer Portal documentation.
        [SerializeField] private string productName = "MyUnityApplication";
        [SerializeField] private string productVersion = "1.0";
        [SerializeField] private string productId = "";
        [SerializeField] private string sandboxId = "";
        [SerializeField] private string deploymentId = "";
        [SerializeField] private string clientId = "";
        [SerializeField] private string clientSecret = "";
        private float _platformTickTimer;
        private ProductUserId _localUserId;
        private string _localUserIdString;
        private HashSet<ProductUserId> _remoteUserIds = new HashSet<ProductUserId>();
        private HashSet<string> _remoteUserIdStrings = new HashSet<string>();
        private ProductUserId _lobbyOwnerUserId;
        private string _lobbyOwnerUserIdString;
        private P2PInterface _p2PInterface;
        [SerializeField] private DebugAccount debugAccount;

        enum DebugAccount {
            test_user_cred,
            test_user_cred2,
            None
        }

        // If we're in editor, we should dynamically load and unload the SDK between play sessions.
        // This allows us to initialize the SDK each time the game is run in editor.
#if UNITY_EDITOR
        [DllImport("Kernel32.dll")]
        private static extern IntPtr LoadLibrary(string lpLibFileName);

        [DllImport("Kernel32.dll")]
        private static extern int FreeLibrary(IntPtr hLibModule);

        [DllImport("Kernel32.dll")]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        private IntPtr _libraryPointer;
#endif
        public void Tick() {
            if (_platformInterface != null) {
                _platformTickTimer += Time.deltaTime;

                if (_platformTickTimer >= PlatformTickInterval) {
                    _platformTickTimer = 0;
                    _platformInterface.Tick();
                }
            }
        }
        
        private void AddUserIdMapping(ProductUserId remoteUserId) {
            if (remoteUserId == null) {
                return;
            }
            _remoteUserIds.Add(remoteUserId);
            _remoteUserIdStrings.Add(remoteUserId.ToString());
        }
        
        private void RemoveUserIdMapping(ProductUserId remoteUserId) {
            if (remoteUserId == null) {
                return;
            }
            _remoteUserIds.Remove(remoteUserId);
            _remoteUserIdStrings.Remove(remoteUserId.ToString());
        }
        
        private void SetLocalUserIdMapping(ProductUserId localUserId) {
            _localUserId = localUserId;
            _localUserIdString = localUserId?.ToString();
        }

        private void SetLobbyOwnerUserIdMapping(ProductUserId lobbyOwnerUserId) {
            _lobbyOwnerUserId = lobbyOwnerUserId;
            _lobbyOwnerUserIdString = lobbyOwnerUserId?.ToString();
        }
        
        public void Authenticate(Action<ResultData> callback) {
            var deviceID = SystemInfo.deviceUniqueIdentifier;

            if (Debug.isDebugBuild) {
                
            }
            
            //Limit the length of the device ID to 32 characters or less
            if (deviceID.Length > 32) {
                deviceID = deviceID[..32];
            }
            
            var loginOptions = new LoginOptions {
                Credentials = new Credentials {
                    Token = null,
                    Type = ExternalCredentialType.DeviceidAccessToken
                },
                UserLoginInfo = new UserLoginInfo {
                    DisplayName = deviceID
                }
            };

            var createDeviceIdOptions = new CreateDeviceIdOptions {
                DeviceModel = SystemInfo.deviceType.ToString(),
            };

            if (debugAccount != DebugAccount.None) {
                    var debugLoginOptions = new Epic.OnlineServices.Auth.LoginOptions {
                        Credentials = new Epic.OnlineServices.Auth.Credentials {
                            Id = "localhost:4000",
                            Type = LoginCredentialType.Developer,
                            Token = Enum.GetName(debugAccount.GetType(), debugAccount)
                        }
                    };
                    Debug.Log("Logging in with debug credentials");

                    _platformInterface.GetAuthInterface().Login(ref debugLoginOptions, null,
                        (ref Epic.OnlineServices.Auth.LoginCallbackInfo info) => {
                            if (info.ResultCode == Result.Success) {
                                Debug.Log("Successfully logged in");
                                
                                var copyUserAuthTokenOptions = new CopyUserAuthTokenOptions();
                                _platformInterface.GetAuthInterface().CopyUserAuthToken(ref copyUserAuthTokenOptions, info.LocalUserId,
                                    out var userAuthToken);

                                var debugLoginOptions = new LoginOptions {
                                    Credentials = new Credentials {
                                        Token = userAuthToken?.AccessToken,
                                        Type = ExternalCredentialType.Epic
                                    },
                                    UserLoginInfo = null
                                };
                                
                                        _platformInterface.GetConnectInterface().Login(ref debugLoginOptions, null,
                                    (ref LoginCallbackInfo info) => {
                                        if (info.ResultCode == Result.Success) {
                                            Debug.Log("Successfully logged in");
                                            SetLocalUserIdMapping(info.LocalUserId);
                                            AddEventHandlers();
                                            callback?.Invoke(new ResultData {
                                                result = Results.Success,
                                                message = "Successfully logged in"
                                            });
                                        } else if (info.ResultCode == Result.InvalidUser) {
                                            var options = new CreateUserOptions {
                                                ContinuanceToken = info.ContinuanceToken
                                            };
                                            
                                            _platformInterface.GetConnectInterface().CreateUser(ref options, null, (ref CreateUserCallbackInfo info) => {
                                                if (info.ResultCode == Result.Success) {
                                                    Debug.Log("Successfully created user");
                                                    SetLocalUserIdMapping(info.LocalUserId);
                                                    AddEventHandlers();
                                                    callback?.Invoke(new ResultData {
                                                        result = Results.Success,
                                                        message = "Successfully logged in"
                                                    });
                                                } else {
                                                    callback?.Invoke(new ResultData {
                                                        result = Results.Failure,
                                                        message = "Failed to create user"
                                                    });
                                                }
                                            });
                                        }
                                        else {
                                            callback?.Invoke(new ResultData {
                                                result = Results.Failure,
                                                message = "Failed to login"
                                            });
                                        }
                                    });
                            }
                            else {
                                callback?.Invoke(new ResultData {
                                    result = Results.Failure,
                                    message = "Failed to login"
                                });
                            }
                        });
            }
            else {
                _platformInterface.GetConnectInterface().CreateDeviceId(ref createDeviceIdOptions, null,
                    (ref CreateDeviceIdCallbackInfo info) => {
                        if (info.ResultCode == Result.DuplicateNotAllowed) {
                            Debug.Log("Device ID already exists");
                        }
                        else if (info.ResultCode == Result.Success) {
                            Debug.Log("Successfully created device id");
                        }
                        else {
                            callback?.Invoke(new ResultData {
                                result = Results.Failure,
                                message = "Failed to create device id"
                            });
                            return;
                        }

                        _platformInterface.GetConnectInterface().Login(ref loginOptions, null,
                            (ref LoginCallbackInfo data) => {
                                if (data.ResultCode == Result.Success) {
                                    SetLocalUserIdMapping(data.LocalUserId);
                                    callback?.Invoke(new ResultData {
                                        result = Results.Success,
                                        message = "Login success"
                                    });
                                    AddEventHandlers();
                                }
                                else {
                                    callback?.Invoke(new ResultData {
                                        result = Results.Failure,
                                        message = "Login failed"
                                    });
                                }
                            });
                    });
            }
        }

        public void Initialize(Action<ResultData> onComplete) {
#if UNITY_EDITOR

            //Find the library path
            var pathToLibrary = AssetDatabase.FindAssets(Config.LibraryName)
                .Select(AssetDatabase.GUIDToAssetPath)
                .FirstOrDefault();
            
            _libraryPointer = LoadLibrary(pathToLibrary);
            if (_libraryPointer == IntPtr.Zero) {
                throw new Exception("Failed to load library" + pathToLibrary);
            }

            Bindings.Hook(_libraryPointer, GetProcAddress);
#endif
            
            var initializeOptions = new InitializeOptions {
                ProductName = productName,
                ProductVersion = productVersion
            };

            var initializeResult = PlatformInterface.Initialize(ref initializeOptions);
            if (initializeResult != Result.Success) {
                onComplete?.Invoke(new ResultData {
                    result = Results.Failure,
                    message = "Failed to initialize platform: " + initializeResult
                });
            }

            // The SDK outputs lots of information that is useful for debugging.
            // Make sure to set up the logging interface as early as possible: after initializing.
            LoggingInterface.SetLogLevel(LogCategory.AllCategories, LogLevel.Warning);
            LoggingInterface.SetCallback((ref LogMessage logMessage) => Debug.Log(logMessage.Message));

            var options = new Options {
                ProductId = productId,
                SandboxId = sandboxId,
                DeploymentId = deploymentId,
                ClientCredentials = new ClientCredentials {
                    ClientId = clientId,
                    ClientSecret = clientSecret
                }
            };

            _platformInterface = PlatformInterface.Create(ref options);
            if (_platformInterface == null) {
                onComplete?.Invoke(new ResultData {
                    result = Results.Failure,
                    message = "Failed to create platform"
                });
            } else {
                _p2PInterface = _platformInterface.GetP2PInterface();
                onComplete?.Invoke(new ResultData {
                    result = Results.Success,
                    message = "Platform created"
                });
            }
        }

        private ulong _addNotifyLobbyMemberStatusReceivedHandle;
        private ulong _addNotifyPeerConnectionClosedHandle;
        private ulong _addNotifyPeerConnectionRequestHandle;
        private void AddEventHandlers() {
            var notifyLobbyMemberStatusUpdateReceivedOptions = new AddNotifyLobbyMemberStatusReceivedOptions();
            _addNotifyLobbyMemberStatusReceivedHandle = _platformInterface.GetLobbyInterface().AddNotifyLobbyMemberStatusReceived(ref notifyLobbyMemberStatusUpdateReceivedOptions, null, OnLobbyMemberUpdate);
            var notifyPeerConnectionClosedOptions = new AddNotifyPeerConnectionClosedOptions {
                LocalUserId = _localUserId,
                SocketId = new SocketId {
                    SocketName = "ChangeMe"
                }
            };
            _addNotifyPeerConnectionClosedHandle = _platformInterface.GetP2PInterface().AddNotifyPeerConnectionClosed(ref notifyPeerConnectionClosedOptions, null, OnPeerConnectionClosed);
            var notifyPeerConnectionRequestOptions = new AddNotifyPeerConnectionRequestOptions {
                LocalUserId = _localUserId,
                SocketId = new SocketId {
                    SocketName = "ChangeMe"
                }
            };
            
            _addNotifyPeerConnectionRequestHandle = _platformInterface.GetP2PInterface().AddNotifyPeerConnectionRequest(ref notifyPeerConnectionRequestOptions, null, OnPeerConnectionRequest);
        }
        
        private void RemoveEventHandlers() {
            _platformInterface.GetLobbyInterface().RemoveNotifyLobbyMemberStatusReceived(_addNotifyLobbyMemberStatusReceivedHandle);
            _platformInterface.GetP2PInterface().RemoveNotifyPeerConnectionClosed(_addNotifyPeerConnectionClosedHandle);
            _platformInterface.GetP2PInterface().RemoveNotifyPeerConnectionRequest(_addNotifyPeerConnectionRequestHandle);
        }
        
        private void OnLobbyMemberUpdate(ref LobbyMemberStatusReceivedCallbackInfo lobbyMemberUpdateReceivedCallbackInfo) {
            Debug.Log("Lobby member update received");
            switch (lobbyMemberUpdateReceivedCallbackInfo.CurrentStatus) {
                case LobbyMemberStatus.Joined:
                    AddUserIdMapping(lobbyMemberUpdateReceivedCallbackInfo.TargetUserId);
                    Ping();
                    break;
                case LobbyMemberStatus.Left:
                    RemoveUserIdMapping(lobbyMemberUpdateReceivedCallbackInfo.TargetUserId);
                    break;
                case LobbyMemberStatus.Disconnected:
                    RemoveUserIdMapping(lobbyMemberUpdateReceivedCallbackInfo.TargetUserId);
                    break;
                case LobbyMemberStatus.Kicked:
                    RemoveUserIdMapping(lobbyMemberUpdateReceivedCallbackInfo.TargetUserId);
                    break;
                case LobbyMemberStatus.Promoted:
                    SetLobbyOwnerUserIdMapping(lobbyMemberUpdateReceivedCallbackInfo.TargetUserId);
                    break;
                case LobbyMemberStatus.Closed:
                    RemoveUserIdMapping(lobbyMemberUpdateReceivedCallbackInfo.TargetUserId);
                    break;
                default:
                    break;
            }
            OnPlayersChanged?.Invoke();
        }
        
        private void OnPeerConnectionClosed (ref OnRemoteConnectionClosedInfo peerConnectionClosedCallbackInfo) {
            Debug.Log("Peer connection closed " + peerConnectionClosedCallbackInfo.RemoteUserId);
            _remoteUserIds.Remove(peerConnectionClosedCallbackInfo.RemoteUserId);
            OnPlayersChanged?.Invoke();
        }
        
        private void OnPeerConnectionRequest (ref OnIncomingConnectionRequestInfo peerConnectionRequestCallbackInfo) {
            Debug.Log("Peer connection request " + peerConnectionRequestCallbackInfo.RemoteUserId);
            AddUserIdMapping(peerConnectionRequestCallbackInfo.RemoteUserId);
            OnPlayersChanged?.Invoke();
            
            var acceptConnectionOptions = new AcceptConnectionOptions {
                LocalUserId = _localUserId,
                RemoteUserId = peerConnectionRequestCallbackInfo.RemoteUserId,
                SocketId = new SocketId {
                    SocketName = "ChangeMe"
                }
            };
            
            _p2PInterface.AcceptConnection(ref acceptConnectionOptions);
        }

        public void Uninitialize(Action<ResultData> onComplete = null) {
            if (_platformInterface != null) {
                RemoveEventHandlers();
                _platformInterface.Release();
                _platformInterface = null;
                PlatformInterface.Shutdown();
            }

#if UNITY_EDITOR
            if (_libraryPointer == IntPtr.Zero) {
                return;
            }

            Bindings.Unhook();

            // Free until the module ref count is 0
            while (FreeLibrary(_libraryPointer) != 0) {
            }

            _libraryPointer = IntPtr.Zero;
#endif
        }

        private LobbyManager lobbyManager;
        private const string LobbyNameKey = "LOBBYNAME";

        public override event Action OnPlayersChanged;

        private protected override void GetLobbyListInternal(int maxResults = 10, Action<Results, List<LobbyData>> callback = null) {
            lobbyManager ??= new LobbyManager(_platformInterface, _localUserId);
            OnPlayersChanged?.Invoke();
            var createLobbySearchOptions = new CreateLobbySearchOptions {
                MaxResults = (uint) maxResults
            };
            lobbyManager.GetLobbies(createLobbySearchOptions, (lobbies) => {
                if (lobbies == null) {
                    callback?.Invoke(Results.Failure, null);
                    return;
                }
                
                var lobbyData = new List<LobbyData>();
                foreach (var lobby in lobbies) {
                    var lobbyDetailsCopyInfoOptions = new LobbyDetailsCopyInfoOptions();
                    lobby.CopyInfo(ref lobbyDetailsCopyInfoOptions, out var lobbyDetailsInfo);
                    var lobbyDataItem = new LobbyData {
                        uuid = lobbyDetailsInfo?.LobbyId,
                        maxPlayers = (int) (lobbyDetailsInfo?.MaxMembers ?? 0),
                        playerCount = (int) (lobbyDetailsInfo?.MaxMembers ?? 0) - (int) (lobbyDetailsInfo?.AvailableSlots ?? 0)
                    };
                    
                    var lobbyDetailsCopyAttributeOptions = new LobbyDetailsCopyAttributeByKeyOptions() {
                        AttrKey = LobbyNameKey
                    };
                    lobby.CopyAttributeByKey(ref lobbyDetailsCopyAttributeOptions, out var lobbyDetailsAttribute);
                    lobbyDataItem.name = lobbyDetailsAttribute?.Data?.Value.AsUtf8;
                    if (string.IsNullOrEmpty(lobbyDataItem.name)) {
                        lobbyDataItem.name = "Unnamed Lobby";
                    }

                    //Only add lobbies that are not empty (When someone leaves a lobby, it is not removed from the list)
                    if (lobbyDataItem.playerCount != 0) {
                        lobbyData.Add(lobbyDataItem);
                    }

                    lobby.Release();
                }
                
                callback?.Invoke(Results.Success, lobbyData);
            });
        }

        private void Ping() {
            foreach (var userId in _remoteUserIds) {
                SendInternal(userId.ToString(), Array.Empty<byte>(), PacketReliability.ReliableUnordered);
            }
        }

        private protected override void CreateLobbyInternal(string lobbyName, Action<Results, LobbyData> callback = null) {
            lobbyManager ??= new LobbyManager(_platformInterface, _localUserId);

            var createLobbyOptions = new CreateLobbyOptions {
                LocalUserId = _localUserId,
                MaxLobbyMembers = 10,
                PermissionLevel = LobbyPermissionLevel.Publicadvertised,
                BucketId = "default"
            };

            lobbyManager.CreateLobby(createLobbyOptions, info => {
                if (info.ResultCode != Result.Success) {
                    callback?.Invoke(Results.Failure, new LobbyData());
                    return;
                }
                SetLobbyOwnerUserIdMapping(_localUserId);
                var updateLobbyModificationOptions = new UpdateLobbyModificationOptions {
                    LocalUserId = _localUserId,
                    LobbyId = info.LobbyId
                };
                var attribute = new LobbyModificationAddAttributeOptions {
                    Attribute = new AttributeData {
                        Key = LobbyNameKey,
                        Value = lobbyName
                    },
                    Visibility = LobbyAttributeVisibility.Public
                };
                lobbyManager.SetLobbyAttribute(updateLobbyModificationOptions, attribute, () => {
                    lobbyManager.GetLobby(info.LobbyId, (lobby) => {
                        var data = new LobbyData {
                            uuid = info.LobbyId,
                            name = lobbyName,
                            maxPlayers = 10,
                            playerCount = 1,
                            hostId = _localUserId.ToString()
                        };
                        
                        SetLobbyOwnerUserIdMapping(_localUserId);
                        lobby.Release();
                        callback?.Invoke(Results.Success, data);
                    });
                });
            });
        }

        private protected override void JoinLobbyInternal(string lobbyId, Action<Results, LobbyData> callback = null) {
            lobbyManager ??= new LobbyManager(_platformInterface, _localUserId);

            lobbyManager.GetLobby(lobbyId, details => {
                var joinLobbyOptions = new JoinLobbyOptions {
                    LobbyDetailsHandle = details,
                    LocalUserId = _localUserId,
                    PresenceEnabled = false,
                    LocalRTCOptions = null
                };
                lobbyManager.JoinLobby(joinLobbyOptions, info => {
                    if (info.ResultCode != Result.Success) {
                        callback?.Invoke(Results.Failure, new LobbyData());
                        return;
                    }
                    
                    lobbyManager.GetLobby(lobbyId, lobby => {
                        var lobbyDetailsCopyInfoOptions = new LobbyDetailsCopyInfoOptions();
                        lobby.CopyInfo(ref lobbyDetailsCopyInfoOptions, out var lobbyDetailsInfo);
                        var lobbyDataItem = new LobbyData {
                            uuid = lobbyDetailsInfo?.LobbyId,
                            maxPlayers = (int) (lobbyDetailsInfo?.MaxMembers ?? 0),
                            playerCount = (int) (lobbyDetailsInfo?.MaxMembers ?? 0) - (int) (lobbyDetailsInfo?.AvailableSlots ?? 0),
                            hostId = lobbyDetailsInfo?.LobbyOwnerUserId.ToString()
                        };

                        SetLobbyOwnerUserIdMapping(lobbyDetailsInfo?.LobbyOwnerUserId);
                        
                        var lobbyMemberCountOptions = new LobbyDetailsGetMemberCountOptions();
                        var memberCount = lobby.GetMemberCount(ref lobbyMemberCountOptions);
                        
                        for (var i = 0; i < memberCount; i++) {
                            var getMemberByIndexOptions = new LobbyDetailsGetMemberByIndexOptions {
                                MemberIndex = (uint) i
                            };
                            var prodUserId = lobby.GetMemberByIndex(ref getMemberByIndexOptions);
                            if (prodUserId != null && prodUserId != _localUserId) {
                                AddUserIdMapping(prodUserId);
                            }
                        }
                        Ping();
                        
                        Debug.Log("Lobby owner: " + _lobbyOwnerUserId);
                        lobby.Release();
                        callback?.Invoke(Results.Success, lobbyDataItem);
                    });
                });
            });
        }

        private protected override void LeaveLobbyInternal(string lobbyId, Action<Results> callback = null) {
            lobbyManager ??= new LobbyManager(_platformInterface, _localUserId);

            var leaveLobbyOptions = new LeaveLobbyOptions {
                LocalUserId = _localUserId,
                LobbyId = lobbyId
            };
            lobbyManager.LeaveLobby(leaveLobbyOptions, info => {
                if (info.ResultCode != Result.Success) {
                    callback?.Invoke(Results.Failure);
                    return;
                }
                
                callback?.Invoke(Results.Success);
            });
        }

        private protected override void InitP2PInternal(Action<Results> callback = null) {
            
        }

        private SendPacketOptions _sendOptions = new SendPacketOptions {
            LocalUserId = null,
            RemoteUserId = null,
            SocketId = new SocketId {
                SocketName = "ChangeMe"
            },
            Channel = 0,
            Data = default,
            AllowDelayedDelivery = false,
            Reliability = Epic.OnlineServices.P2P.PacketReliability.ReliableOrdered,
            DisableAutoAcceptConnection = false,
        };
        
        private protected override void SendInternal(string userId, byte[] data, PacketReliability reliability) {
            var remoteUser = ProductUserId.FromString(userId);
            var packetReliability = ConvertPacketReliability(reliability);

            _sendOptions.LocalUserId = _localUserId;
            _sendOptions.RemoteUserId = remoteUser;
            _sendOptions.Data = data;
            _sendOptions.Reliability = packetReliability;

            _p2PInterface.SendPacket(ref _sendOptions);
        }
        
        private void SendInternal(ProductUserId remoteUser, byte[] data, PacketReliability reliability) {
            var packetReliability = ConvertPacketReliability(reliability);
            _sendOptions.LocalUserId = _localUserId;
            _sendOptions.RemoteUserId = remoteUser;
            _sendOptions.Data = data;
            _sendOptions.Reliability = packetReliability;

            _p2PInterface.SendPacket(ref _sendOptions);
        }

        private static Epic.OnlineServices.P2P.PacketReliability ConvertPacketReliability(PacketReliability reliability) {
            var packetReliability = reliability switch {
                PacketReliability.UnreliableUnordered => Epic.OnlineServices.P2P.PacketReliability.UnreliableUnordered,
                PacketReliability.ReliableOrdered => Epic.OnlineServices.P2P.PacketReliability.ReliableOrdered,
                PacketReliability.ReliableUnordered => Epic.OnlineServices.P2P.PacketReliability.ReliableUnordered,
                _ => Epic.OnlineServices.P2P.PacketReliability.ReliableOrdered
            };
            return packetReliability;
        }

        private protected override void SendToAllInternal(byte[] data, PacketReliability reliability) {
            foreach (var remoteUser in _remoteUserIds) {
                SendInternal(remoteUser, data, reliability);
            }
        }

        private readonly ReceivedData _badData = new ReceivedData {
            success = false,
            fromUserId = null,
            data = new byte[] {
            }
        };

        private readonly ReceivedData _noData = new ReceivedData {
            success = true,
            fromUserId = null,
            data = new byte[] {
            }
        };

        private GetNextReceivedPacketSizeOptions _getNextReceivedPacketSizeOptions = new GetNextReceivedPacketSizeOptions {
            LocalUserId = null,
            RequestedChannel = null
        };

        private GetPacketQueueInfoOptions _getPackedQueueInfoOptions = new GetPacketQueueInfoOptions();

        protected override int GetPacketQueueSize() {
            _p2PInterface.GetPacketQueueInfo(ref _getPackedQueueInfoOptions, out var packetQueueInfo);
            return (int) packetQueueInfo.IncomingPacketQueueCurrentPacketCount;
        }

        private protected override ReceivedData ReceiveInternal() {
            if (GetPacketQueueSize() <= 0) {
                return _noData;
            }
            
            if (_getNextReceivedPacketSizeOptions.LocalUserId == null) {
                _getNextReceivedPacketSizeOptions.LocalUserId = _localUserId;
            }

            _p2PInterface.GetNextReceivedPacketSize(ref _getNextReceivedPacketSizeOptions, out var sizeInfo);
            var receiveOptions = new ReceivePacketOptions {
                LocalUserId = _localUserId,
                MaxDataSizeBytes = 8192,
                RequestedChannel = null
            };
            
            var data = new byte[sizeInfo];
            var dataSegment = new ArraySegment<byte>(data);
            var result = _p2PInterface.ReceivePacket(ref receiveOptions, out var peerId,
                out var socketId, out var outChannel, dataSegment, out var bytesWritten);
            
            if (bytesWritten <= 0) {
                return _noData;
            }
            
            if (result != Result.Success) {
                return _badData;
            }
            
            return new ReceivedData {
                success = true,
                fromUserId = peerId.ToString(),
                data = dataSegment.ToArray()
            };
        }

        public override IEnumerable<string> GetRemotePlayerIds() => _remoteUserIdStrings;

        public override string GetLocalPlayerId() => _localUserIdString;

        public override string GetHostId() => _lobbyOwnerUserIdString;
    }
}