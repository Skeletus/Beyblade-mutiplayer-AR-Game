using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class ARPlacementAndPlaneDetectionController : MonoBehaviour
{
    [SerializeField] private GameObject placeButton;
    [SerializeField] private GameObject adjustButton;
    [SerializeField] private GameObject searchForGameButton;
    [SerializeField] private TextMeshProUGUI informUIPanelText;

    private ARPlaneManager m_ARPlaneManager;
    private ARPlacementManager m_ARPlacementManager;

    private void Awake()
    {
        m_ARPlaneManager = GetComponent<ARPlaneManager>();
        m_ARPlacementManager = GetComponent<ARPlacementManager>();
    }

    private void Start()
    {
        placeButton.SetActive(true);
        adjustButton.SetActive(false);
        searchForGameButton.SetActive(false);

        informUIPanelText.text = "Move phone to detect planes and place battle arena";
    }

    public void DisableARPlacementAndDetection()
    {
        m_ARPlaneManager.enabled = false;
        m_ARPlacementManager.enabled = false;

        SetAllPlanesActiveOrDesactive(false);

        placeButton.SetActive(false);
        adjustButton.SetActive(true);
        searchForGameButton.SetActive(true);

        informUIPanelText.text = "Well done, now searching for games...";
    }

    public void EnableARPlacementAndPlaneDetection()
    {
        m_ARPlaneManager.enabled = true;
        m_ARPlacementManager.enabled = true;

        SetAllPlanesActiveOrDesactive(true);

        placeButton.SetActive(true);
        adjustButton.SetActive(false);
        searchForGameButton.SetActive(false);

        informUIPanelText.text = "Move phone to detect planes and place battle arena";
    }

    private void SetAllPlanesActiveOrDesactive(bool value)
    {
        foreach(var plane in m_ARPlaneManager.trackables)
        {
            plane.gameObject.SetActive(value);
        }
    }

}
