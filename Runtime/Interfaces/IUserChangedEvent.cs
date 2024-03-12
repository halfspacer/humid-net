using Epic.OnlineServices;
using Epic.OnlineServices.Lobby;

public interface IUserChangedEvent {
    public void OnUserJoined(ProductUserId newUser);
    public void OnUserLeft(ProductUserId newHost, LobbyMemberStatus reason);
}
