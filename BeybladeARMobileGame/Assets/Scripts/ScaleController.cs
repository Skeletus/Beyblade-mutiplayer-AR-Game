using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class ScaleController : MonoBehaviour
{
    [SerializeField] private Slider scaleSlider;

    private XROrigin m_XROrigin;

    private void Awake()
    {
        m_XROrigin = GetComponent<XROrigin>();
    }

    private void Start()
    {
        scaleSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    public void OnSliderValueChanged(float value)
    {
        if (scaleSlider != null)
        {
            m_XROrigin.transform.localScale = Vector3.one / value;
        }
    }
}
