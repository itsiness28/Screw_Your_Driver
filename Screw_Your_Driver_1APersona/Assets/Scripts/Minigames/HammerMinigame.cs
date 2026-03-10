using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class HammerMinigame : MonoBehaviour
{
    [Header("Referencias")]
    public Inventory inventory;
    public GameObject minigameUI;
    public GameObject mensajeErrorUI;
    public GameObject completadoUI;

    [Header("Herramienta requerida")]
    public ItemType requiredTool = ItemType.ToolA;

    [Header("Clavos y Slots")]
    public RectTransform[] clavos;
    public RectTransform[] slots;
    public int golpesNecesarios = 4;

    private bool[] clavoColocado;
    private int[] golpesActuales;

    // Scripts que se desactivarán temporalmente
    private MonoBehaviour[] cameraScripts;
    private MonoBehaviour[] playerScripts;

    void Start()
    {
        clavoColocado = new bool[clavos.Length];
        golpesActuales = new int[clavos.Length];

        for (int i = 0; i < clavos.Length; i++)
        {
            int index = i;
            AddDragEvents(clavos[i], index);
            AddClickEvents(clavos[i], index);
        }
    }

    // ---------------------------------------------------------
    // ABRIR MINIJUEGO
    // ---------------------------------------------------------
    public void TryStartMinigame()
    {
        if (!inventory.HasTool(requiredTool))
        {
            if (mensajeErrorUI != null)
                mensajeErrorUI.SetActive(true);

            return;
        }

        minigameUI.SetActive(true);

        // ---------------------------------------------------------
        // BLOQUEAR CÁMARA
        // ---------------------------------------------------------
        cameraScripts = Camera.main.GetComponents<MonoBehaviour>();
        foreach (var script in cameraScripts)
        {
            script.enabled = false;
        }

        // ---------------------------------------------------------
        // BLOQUEAR MOVIMIENTO DEL JUGADOR
        // ---------------------------------------------------------
        playerScripts = FindObjectsOfType<MonoBehaviour>();
        foreach (var script in playerScripts)
        {
            if (script.GetType().Name.Contains("Movement") ||
                script.GetType().Name.Contains("Controller") ||
                script.GetType().Name.Contains("Look"))
            {
                script.enabled = false;
            }
        }

        // ---------------------------------------------------------
        // ACTIVAR CURSOR
        // ---------------------------------------------------------
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // ---------------------------------------------------------
    // CERRAR MINIJUEGO
    // ---------------------------------------------------------
    public void CloseMinigame()
    {
        minigameUI.SetActive(false);
        completadoUI.SetActive(false);
        // Restaurar cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Reactivar scripts de cámara
        if (cameraScripts != null)
        {
            foreach (var script in cameraScripts)
                script.enabled = true;
        }

        // Reactivar scripts del jugador
        if (playerScripts != null)
        {
            foreach (var script in playerScripts)
            {
                if (script.GetType().Name.Contains("Movement") ||
                    script.GetType().Name.Contains("Controller") ||
                    script.GetType().Name.Contains("Look"))
                {
                    script.enabled = true;
                }
            }
        }
    }

    // ---------------------------------------------------------
    // COLOCAR CLAVO EN SLOT
    // ---------------------------------------------------------
    void TryPlaceClavo(RectTransform clavo, int index)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            float distancia = Vector2.Distance(clavo.anchoredPosition, slots[i].anchoredPosition);

            if (distancia < 40f)
            {
                clavo.anchoredPosition = slots[i].anchoredPosition;
                clavoColocado[index] = true;
                return;
            }
        }

        clavoColocado[index] = false;
    }

    // ---------------------------------------------------------
    // COMPROBAR COMPLETADO
    // ---------------------------------------------------------
    void CheckCompletion()
    {
        for (int i = 0; i < golpesActuales.Length; i++)
        {
            if (golpesActuales[i] < golpesNecesarios)
                return;
        }

        completadoUI.SetActive(true);

        // Cerrar minijuego después de 2 segundos
        StartCoroutine(CloseAfterDelay(1f));
    }

    private IEnumerator CloseAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        CloseMinigame();
    }

    // ---------------------------------------------------------
    // EVENTOS DE ARRASTRE
    // ---------------------------------------------------------
    void AddDragEvents(RectTransform clavo, int index)
    {
        EventTrigger trigger = clavo.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = clavo.gameObject.AddComponent<EventTrigger>();

        // BEGIN DRAG
        EventTrigger.Entry begin = new EventTrigger.Entry();
        begin.eventID = EventTriggerType.BeginDrag;
        begin.callback.AddListener((data) =>
        {
            clavo.GetComponent<CanvasGroup>().blocksRaycasts = false;
        });
        trigger.triggers.Add(begin);

        // DRAG
        EventTrigger.Entry drag = new EventTrigger.Entry();
        drag.eventID = EventTriggerType.Drag;
        drag.callback.AddListener((data) =>
        {
            PointerEventData d = (PointerEventData)data;

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)clavo.parent,
                d.position,
                d.pressEventCamera,
                out localPoint
            );

            clavo.anchoredPosition = localPoint;
        });
        trigger.triggers.Add(drag);

        // END DRAG
        EventTrigger.Entry end = new EventTrigger.Entry();
        end.eventID = EventTriggerType.EndDrag;
        end.callback.AddListener((data) =>
        {
            clavo.GetComponent<CanvasGroup>().blocksRaycasts = true;
            TryPlaceClavo(clavo, index);
        });
        trigger.triggers.Add(end);
    }

    // ---------------------------------------------------------
    // EVENTOS DE CLICK (MARTILLAR)
    // ---------------------------------------------------------
    void AddClickEvents(RectTransform clavo, int index)
    {
        EventTrigger trigger = clavo.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = clavo.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry click = new EventTrigger.Entry();
        click.eventID = EventTriggerType.PointerClick;
        click.callback.AddListener((data) =>
        {
            if (!clavoColocado[index]) return;

            golpesActuales[index]++;

            if (golpesActuales[index] >= golpesNecesarios)
                CheckCompletion();
        });

        trigger.triggers.Add(click);
    }
}