using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectionManager : MonoBehaviour
{
    [SerializeField] private Transform playerSwitcherTransform;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;

    #region UNITY METHODS
    #endregion

    #region UI Callback Methods
    public void NextPlayer()
    {
        nextButton.enabled = false;
        prevButton.enabled = false;

        StartCoroutine(Rotate(Vector3.up, playerSwitcherTransform, 90, 1.0f));
    }

    public void PreviousPlayer()
    {
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
