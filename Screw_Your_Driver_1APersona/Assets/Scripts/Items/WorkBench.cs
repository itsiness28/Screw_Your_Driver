using UnityEngine;
using UnityEngine.SceneManagement;

public class WorkBench : MonoBehaviour, IInteractable
{
    [SerializeField] private Camera fpsCamera;
    [SerializeField] private Camera taskCamera;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject hotVar;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Interact(GameObject interactor)
    {
        Inventory inv = interactor.GetComponent<Inventory>();

        if (inv.toolSlot != ItemType.ToolB)
        {
            Debug.Log("Necesitas un destornillador.");
            return;
        }

        StartMinigame(interactor);
    }

    void StartMinigame(GameObject player)
    {
        fpsCamera.gameObject.SetActive(false);
        playerController.SetControl(false);
        hotVar.SetActive(false);
        MinigameSession.screwMinigameCompleted = false;
        MinigameSession.returnSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("ScrewMinigame");
    }
}
