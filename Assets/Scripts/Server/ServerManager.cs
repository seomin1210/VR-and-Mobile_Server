using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ServerManager : MonoBehaviourPunCallbacks
{
    private GameObject _playerPrefab = null;

    void Awake()
    {
        ConnectToServer();
    }

    // Try Connect to Server
    private void ConnectToServer()
    {
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Try connect to Server");
    }

    // Connected Call this fuction
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Server");
        base.OnConnectedToMaster();
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 10;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;

        PhotonNetwork.JoinOrCreateRoom("Room 1", roomOptions, TypedLobby.Default);
    }

    // Player Join Room
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a room");
        base.OnJoinedRoom();
        // Player Create
        _playerPrefab = PhotonNetwork.Instantiate("Network Player", transform.position, transform.rotation);
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Left a room");
        base.OnLeftRoom();
        PhotonNetwork.Destroy(_playerPrefab);
    }

    // Other Player join Room
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("A New ohter Player joined the room");
        base.OnPlayerEnteredRoom(newPlayer);
    }

    // Disconnected Server, call this function
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected Server");
        base.OnDisconnected(cause);
        Application.Quit();
    }
}
