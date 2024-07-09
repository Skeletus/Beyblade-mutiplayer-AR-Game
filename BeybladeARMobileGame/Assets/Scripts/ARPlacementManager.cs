using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlacementManager : MonoBehaviour
{
    [SerializeField] private Camera m_ARCamera;
    [SerializeField] private GameObject battleArenaGameObject;

    private ARRaycastManager m_ARRaycastManager;
    private static List<ARRaycastHit> raycastHits = new List<ARRaycastHit>();

    private void Awake()
    {
        m_ARRaycastManager = GetComponent<ARRaycastManager>();
    }

    void Update()
    {
        Vector3 centerOfScreen = new Vector3(Screen.width /2, Screen.height/2);
        Ray ray = m_ARCamera.ScreenPointToRay(centerOfScreen);


        if (m_ARRaycastManager.Raycast(ray, raycastHits, TrackableType.PlaneWithinPolygon))
        {
            // intersection
            Pose hitPose = raycastHits[0].pose;

            Vector3 positionToBePlaced = hitPose.position;

            battleArenaGameObject.transform.position = positionToBePlaced;
        }
    }
}
