using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Progress;

public class ScrewMinigameController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform screwVisual;

    [Header("Ajustes")]
    [SerializeField] private float requiredProgress = 100f;
    [SerializeField] private float turnSpeed = 40f;
    [SerializeField] private float visualRotationMultiplier = 4f;

    private float currentProgress;
    private bool completed;

    void Update()
    {
        if (completed) return;

        float mouseX = Input.GetAxis("Mouse X");

        // Dirección correcta: izquierda
        if (mouseX < 0f)
        {
            float amount = -mouseX * turnSpeed * Time.deltaTime;
            currentProgress += amount;

            screwVisual.Rotate(0f, +amount * visualRotationMultiplier, 0f);

            if (currentProgress >= requiredProgress)
            {
                CompleteMinigame();
            }
        }
        // Dirección incorrecta: derecha
        //else if (mouseX > 0f)
        //{
        //    float amount = mouseX * turnSpeed * 0.5f * Time.deltaTime;
        //    currentProgress -= amount;
        //    currentProgress = Mathf.Max(0f, currentProgress);

        //    screwVisual.Rotate(0f, -amount * visualRotationMultiplier, 0f );
        //}
    }

    void CompleteMinigame()
    {
        completed = true;
        MinigameSession.screwMinigameCompleted = true;
        SceneManager.LoadScene(MinigameSession.returnSceneName);
    }
}
