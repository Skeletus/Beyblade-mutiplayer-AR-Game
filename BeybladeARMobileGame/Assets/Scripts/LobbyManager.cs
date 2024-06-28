using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("LOGIN UI")]
    [SerializeField] private InputField playerNameInputField;

    #region METHODS
    #endregion

    #region UI Callback Methods
    public void OnEnterGameButtonClick()
    {
        string playerName = playerNameInputField.text;

        if(!string.IsNullOrEmpty(playerName))
        {
            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;

                PhotonNetwork.ConnectUsingSettings();
            }
        }
        else
        {
            Debug.Log("Player name is invalid or empty");
        }
    }
    #endregion

    #region Photon Callbacks Methods
    public override void OnConnected()
    {
        Debug.Log("we connectd to internet");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName +  " is connceted to photon server");
    }
    #endregion
}
