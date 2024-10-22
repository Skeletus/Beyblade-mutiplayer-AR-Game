using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class PlayerSelectionManager : MonoBehaviour
{
    [SerializeField] private Transform playerSwitcherTransform;
    

    [SerializeField] private int playerSelectionNumber;

    [SerializeField] private GameObject[] beybladeModels;

    [Header("UI")]
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    [SerializeField] private TextMeshProUGUI playerModelTypeText;
    [SerializeField] private GameObject UI_Selection;
    [SerializeField] private GameObject UI_AfterSelection;

    #region UNITY METHODS
    private void Start()
    {
        UI_Selection.SetActive(true);
        UI_AfterSelection.SetActive(false);

        playerSelectionNumber = 0;
    }

    private void DisablePrevAndNextButtons()
    {
        nextButton.enabled = false;
        prevButton.enabled = false;
    }

    private void EnablePrevAndNextButtons()
    {
        nextButton.enabled = true;
        prevButton.enabled = true;
    }

    private void ChangeBeybladeModelTypeText()
    {
        if (playerSelectionNumber == 0 || playerSelectionNumber == 1)
        {
            // atack type
            playerModelTypeText.text = "Attack";
        }
        else
        {
            // defense type
            playerModelTypeText.text = "Defense";
        }
    }
    #endregion

    #region UI Callback Methods
    public void NextPlayer()
    {
        playerSelectionNumber += 1;

        if (playerSelectionNumber >= beybladeModels.Length)
        {
            playerSelectionNumber = 0;
        }
        //Debug.Log(playerSelectionNumber);

        DisablePrevAndNextButtons();

        StartCoroutine(Rotate(Vector3.up, playerSwitcherTransform, 90, 1.0f));

        ChangeBeybladeModelTypeText();
    }

    public void PreviousPlayer()
    {
        playerSelectionNumber -= 1;

        if (playerSelectionNumber < 0)
        {
            playerSelectionNumber = beybladeModels.Length - 1;
        }
        //Debug.Log(playerSelectionNumber);

        DisablePrevAndNextButtons();

        StartCoroutine(Rotate(Vector3.up, playerSwitcherTransform, -90, 1.0f));

        ChangeBeybladeModelTypeText();
    }

    public void OnSelectButtonClicked()
    {
        UI_Selection.SetActive(false);
        UI_AfterSelection.SetActive(true);

        ExitGames.Client.Photon.Hashtable playerSelectionProperties = 
            new ExitGames.Client.Photon.Hashtable { {MultiplayerARBeybladeGame.PLAYER_SELECTION_NUMBER, playerSelectionNumber} };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProperties);
    }

    public void OnReSelectButtonClicked()
    {
        UI_Selection.SetActive(true);
        UI_AfterSelection.SetActive(false);
    }

    public void OnBattleButtonClicked()
    {
        SceneLoader.Instance.LoadScene("Scene_Gameplay");
    }

    public void OnBackButtonClicked()
    {
        SceneLoader.Instance.LoadScene("Scene_Lobby");
    }

    #endregion

    IEnumerator Rotate(Vector3 axis, Transform transformToRotate,
        float angle, float duration = 1f)
    {
        Quaternion originalRotation = transformToRotate.rotation;
        Quaternion finalRotation = transformToRotate.rotation * Quaternion.Euler(axis*angle);

        float elapsedTime = 0.0f;
        while(elapsedTime < duration)
        {
            transformToRotate.rotation = Quaternion.Slerp(originalRotation, finalRotation, elapsedTime/duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transformToRotate.rotation = finalRotation;

        EnablePrevAndNextButtons();
    }
}
