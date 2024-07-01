using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectionManager : MonoBehaviour
{
    [SerializeField] private Transform playerSwitcherTransform;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;

    [SerializeField] private int playerSelectionNumber;

    [SerializeField] private GameObject[] beybladeModels;

    #region UNITY METHODS
    private void Start()
    {
        playerSelectionNumber = 0;
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
        Debug.Log(playerSelectionNumber);

        nextButton.enabled = false;
        prevButton.enabled = false;

        StartCoroutine(Rotate(Vector3.up, playerSwitcherTransform, 90, 1.0f));
    }

    public void PreviousPlayer()
    {
        playerSelectionNumber -= 1;

        if (playerSelectionNumber < 0)
        {
            playerSelectionNumber = beybladeModels.Length - 1;
        }
        Debug.Log(playerSelectionNumber);

        nextButton.enabled = false;
        prevButton.enabled = false;

        StartCoroutine(Rotate(Vector3.up, playerSwitcherTransform, -90, 1.0f));
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

        nextButton.enabled = true;
        prevButton.enabled = true;
    }
}
