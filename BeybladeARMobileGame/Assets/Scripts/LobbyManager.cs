using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

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
    [SerializeField] private Text connectionStatusText;
    [SerializeField] private bool showConnectionStatus = false;

    #region Unity METHODS
    private void Start()
    {
        ActivateOnlyLoginScreen();
    }

    private void Update()
    {
        if(showConnectionStatus)
        {
            connectionStatusText.text = "Connection status: " + PhotonNetwork.NetworkClientState;

        }
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

        showConnectionStatus = true;
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
    public void OnEnterGameButtonClicked()
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

    public void OnQuickMatchButtonClicked()
    {
        //SceneManager.LoadScene("Scene_Loading");
        SceneLoader.Instance.LoadScene("Scene_PlayerSelection");
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
