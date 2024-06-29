using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("LOGIN UI")]
    [SerializeField] private InputField playerNameInputField;
    [SerializeField] private GameObject ui_LoginGameObject;

    [Header("Lobby UI")]
    [SerializeField] private GameObject ui_LobbyGameObject;
    [SerializeField] private GameObject ui_3DGameObject;

    [Header("Connection Status UI")]
    [SerializeField] private GameObject ui_ConnectionStatusGameObject;

    #region Unity METHODS
    private void Start()
    {
        ActivateOnlyLoginScreen();
    }

    private void ActivateOnlyLoginScreen()
    {
        ui_LobbyGameObject.SetActive(false);
        ui_3DGameObject.SetActive(false);
        ui_ConnectionStatusGameObject.SetActive(false);

        ui_LoginGameObject.SetActive(true);
    }
    private void ActivateOnlyConnectionScreen()
    {
        ui_LobbyGameObject.SetActive(false);
        ui_3DGameObject.SetActive(false);
        ui_LoginGameObject.SetActive(false);

        ui_ConnectionStatusGameObject.SetActive(true);
    }
    private void ActivateOnlyLobbyScreen()
    {
        ui_LobbyGameObject.SetActive(true);
        ui_3DGameObject.SetActive(true);

        ui_LoginGameObject.SetActive(false);
        ui_ConnectionStatusGameObject.SetActive(false);
    }

    #endregion

    #region UI Callback Methods
    public void OnEnterGameButtonClick()
    {
        string playerName = playerNameInputField.text;

        if(!string.IsNullOrEmpty(playerName))
        {
            ActivateOnlyConnectionScreen();

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
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " is connceted to photon server");
        ActivateOnlyLobbyScreen();
    }

    #endregion
}
