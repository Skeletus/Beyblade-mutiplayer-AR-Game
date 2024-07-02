using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class BeybladeGameManager : MonoBehaviourPunCallbacks
{
    [Header("UI")]
    [SerializeField] private GameObject UI_InformPanelGameObject;
    [SerializeField] private TextMeshProUGUI UI_InformText;

    #region UNITY Methods

    private void Start()
    {
        UI_InformPanelGameObject.SetActive(true);
        UI_InformText.text = "Search for games to battle";
    }

    private void CreateAndJoinRoom()
    {
        string randomRoomName = "Room" + Random.Range(0,1000);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;

        PhotonNetwork.CreateRoom(randomRoomName, roomOptions);
    }
    #endregion

    #region UI Callback Methods
    public void JoinRandomRoom()
    {
        UI_InformText.text = "Searching for available rooms...";
        PhotonNetwork.JoinRandomRoom();
    }


    #endregion

    #region Photon Callback Methods

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        UI_InformText.text = message;

        CreateAndJoinRoom();
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            UI_InformText.text = "Joined to: " + PhotonNetwork.CurrentRoom.Name
                + " . Waiting for other player";
        }
        Debug.Log(PhotonNetwork.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name
            + " player count: " + PhotonNetwork.CurrentRoom.PlayerCount);

        UI_InformText.text = newPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name
            + " player count: " + PhotonNetwork.CurrentRoom.PlayerCount;
    }

    #endregion
}
