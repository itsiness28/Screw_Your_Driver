using UnityEngine;

public class NailsRespawnPickup : MonoBehaviour
{
    public int nailsToGive = 3;
    public float respawnTime = 5f;

    private bool available = true;
    private Renderer rend;
    private Collider col;

    void Start()
    {
        rend = GetComponent<Renderer>();
        col = GetComponent<Collider>();
    }

    public bool TryPickup(Inventory inv)
    {
        if (!available)
            return false;

        inv.AddNails(nailsToGive);


        StartCoroutine(RespawnRoutine());
        return true;
    }

    private System.Collections.IEnumerator RespawnRoutine()
    {
        available = false;

        // Esperar
        yield return new WaitForSeconds(respawnTime);

        available = true;
    }
}
