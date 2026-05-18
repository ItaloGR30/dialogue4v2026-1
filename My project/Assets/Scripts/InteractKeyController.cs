using System;
using UnityEngine;

public class InteractKeyController : MonoBehaviour
{
    private Vector3 Interactposition;
    private RectTransform Rect;
    private Camera mainCamera;
    
    
    private void OnEnable()
    {
        InteractOM.OnShowInteraction += showInteraction;
        InteractOM.InteractPosition += InteractPosition;
    }

    private void showInteraction(bool value)
    {
        gameObject.SetActive(value);
    }
    
    private void GetInteractPosition (Vector3 value)
    {
        Interactposition = value;
    }

    private void Update()
    {
        Rect.position = mainCamera
    }
}
