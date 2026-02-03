using UnityEngine;

public class CameraFollowWorld : MonoBehaviour
{
    public Transform target;

    // Offset FIJO en coordenadas del mundo
    private Vector3 worldOffset = new Vector3(0f, 7f, -15f);

    void LateUpdate()
    {
        if (target == null) return;

        // Posición: sigue al jugador sin usar su rotación
        transform.position = target.position + worldOffset;

        // Rotación: fija, nunca hereda la del jugador
        transform.rotation = Quaternion.Euler(20f, 0f, 0f);
    }
}
