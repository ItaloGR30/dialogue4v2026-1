using System;
using UnityEngine;

public static class InteractOM
{
    public static event Action OnInteract;
    
    public static void Interact()
    {
        OnInteract?.Invoke();
    }

    public static event Action<bool> OnShowInteraction;
    public static void ShowInteraction(bool value)
    {
        OnShowInteraction?.Invoke(value);
    }
    public static event Action<Vector3> ;
    public static void HideInteraction(bool value)
    {}
}
