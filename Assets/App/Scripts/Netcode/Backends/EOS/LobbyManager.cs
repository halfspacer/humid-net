using System;
using System.Collections;
using System.Collections.Generic;
using Epic.OnlineServices;
using Epic.OnlineServices.Lobby;
using Epic.OnlineServices.Platform;
using UnityEngine;

public class LobbyManager {
    private PlatformInterface platformInterface;
    private ProductUserId localUserId;
    public LobbyManager(PlatformInterface platformInterface, ProductUserId userId) {
        this.platformInterface = platformInterface;
        localUserId = userId;
    }
    
    public void CreateLobby(CreateLobbyOptions lobbyOptions, Action<CreateLobbyCallbackInfo> callback) {
        platformInterface.GetLobbyInterface().CreateLobby(ref lobbyOptions, null, (ref CreateLobbyCallbackInfo data) => {
            callback?.Invoke(data);
        });
    }
    
    public void JoinLobby(JoinLobbyOptions lobbyOptions, Action<JoinLobbyCallbackInfo> callback) {
        platformInterface.GetLobbyInterface().JoinLobby(ref lobbyOptions, null, (ref JoinLobbyCallbackInfo data) => {
            callback?.Invoke(data);
        });
    }
    
    public void LeaveLobby(LeaveLobbyOptions lobbyOptions, Action<LeaveLobbyCallbackInfo> callback) {
        platformInterface.GetLobbyInterface().LeaveLobby(ref lobbyOptions, null, (ref LeaveLobbyCallbackInfo data) => {
            callback?.Invoke(data);
        });
    }
    
    public void DestroyLobby(DestroyLobbyOptions lobbyOptions, Action<DestroyLobbyCallbackInfo> callback) {
        platformInterface.GetLobbyInterface().DestroyLobby(ref lobbyOptions, null, (ref DestroyLobbyCallbackInfo data) => {
            callback?.Invoke(data);
        });
    }
    
    public void SetLobbyAttribute(UpdateLobbyModificationOptions options, LobbyModificationAddAttributeOptions attribute, Action onComplete) {
        var updateOptions = options;
        var attributeOptions = attribute;
        var baseAttribute = new LobbyModificationAddAttributeOptions {
            Attribute = new AttributeData {
                Key = "TYPE",
                Value = "default"
            },
            Visibility = LobbyAttributeVisibility.Public
        };
        
        platformInterface.GetLobbyInterface().UpdateLobbyModification(ref updateOptions, out var lobbyModification);
        lobbyModification.AddAttribute(ref attributeOptions);
        lobbyModification.AddAttribute(ref baseAttribute);
        var updateLobbyOptions = new UpdateLobbyOptions {
            LobbyModificationHandle = lobbyModification
        };
        platformInterface.GetLobbyInterface().UpdateLobby(ref updateLobbyOptions, null, (ref UpdateLobbyCallbackInfo data) => {
            lobbyModification.Release();
            onComplete?.Invoke();
        });
    }
    
    public void GetLobbies(CreateLobbySearchOptions lobbySearchOptions, Action<List<LobbyDetails>> lobbies) {
        platformInterface.GetLobbyInterface().CreateLobbySearch(ref lobbySearchOptions, out var lobbySearchHandle);
        var lobbySearchFindOptions = new LobbySearchFindOptions {
            LocalUserId = localUserId
        };
        var lobbySearchSetParameterOptions = new LobbySearchSetParameterOptions {
            Parameter = new AttributeData {
                Key = "TYPE",
                Value = "default"
            },
            ComparisonOp = ComparisonOp.Equal
        };
        lobbySearchHandle.SetParameter(ref lobbySearchSetParameterOptions);
        lobbySearchHandle.Find(ref lobbySearchFindOptions, null, (ref LobbySearchFindCallbackInfo data) => {
            if (data.ResultCode == Result.Success) {
                var lobbySearchResultCount = new LobbySearchGetSearchResultCountOptions();
                var count = lobbySearchHandle.GetSearchResultCount(ref lobbySearchResultCount);
                var lobbyDetailHandles = new List<LobbyDetails>();
                for (uint i = 0; i < count; i++) {
                    var lobbySearchCopySearchResultByIndexOptions = new LobbySearchCopySearchResultByIndexOptions {
                        LobbyIndex = i
                    };
                    var lobbyHandle = lobbySearchHandle.CopySearchResultByIndex(ref lobbySearchCopySearchResultByIndexOptions, out var lobbyDetailsHandle);
                    lobbyDetailHandles.Add(lobbyDetailsHandle);
                }
                
                lobbies?.Invoke(lobbyDetailHandles);
            }
            else {
                Debug.LogError("Failed to find lobbies");
                lobbies?.Invoke(null);
            }
            
            lobbySearchHandle.Release();
        });
    }
}