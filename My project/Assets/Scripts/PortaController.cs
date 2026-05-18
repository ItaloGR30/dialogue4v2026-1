using UnityEngine;

public class PortaController : MonoBehaviour
{
    public Animator anim;
    private bool isOpen;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !isInteractable)
        {
            InteractOM.OnInteract += OpenClose;
            isInteractable = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && !isInteractacle)
        {
            InteractOM.OnInteract -= OpenClose;
            isInteractacle = false;
        }
    }
    
    private void OpenClose()
    {
        if (!isOpen)
        {
            anim.Play("Porta abrindo");
            isOpen = true;
        }
        else
        {
            anim.Play("PortaFechando");
            isOpen = false;
        }
    }
}
