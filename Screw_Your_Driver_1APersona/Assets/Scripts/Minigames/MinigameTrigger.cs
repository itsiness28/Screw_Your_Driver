using UnityEngine;

public class MinigameTrigger : MonoBehaviour, IInteractable
{
    public HammerMinigame manager;

    public void Interact(GameObject interactor)
    {
        manager.TryStartMinigame();
    }
}