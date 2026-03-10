using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    [Header("Configuración")]
    [Tooltip("Distancia de snap en píxeles")]
    public float snapDistance = 60f;
    [Tooltip("Segundos para el cooldown entre golpes (evita clicks muy rápidos)")]
    public float hammerCooldown = 0.12f;
    [Tooltip("Velocidad de interpolación al magnetizar/mover el clavo")]
    public float snapLerpSpeed = 12f;
    [Tooltip("Color de previsualización cuando el slot es válido")]
    public Color slotHighlightColor = Color.yellow;
    [Tooltip("Color cuando el slot está ocupado por otro clavo")]
    public Color slotOccupiedColor = new Color(1f, 0.5f, 0.5f);

    [Tooltip("Scripts a desactivar cuando el minijuego está abierto (más seguro que FindObjects...)")]
    public MonoBehaviour[] scriptsToDisable;

    [Tooltip("Componentes concretos que controlan la cámara/input (p. ej. MouseLook, CameraController)")]
    public MonoBehaviour[] cameraInputScripts;

    private bool[] clavoColocado;
    private int[] golpesActuales;

    // Scripts que se desactivarán temporalmente (fallback)
    private MonoBehaviour[] cameraScripts;
    private bool[] previousCameraEnabled;

    // Scripts del jugador (se usaban pero faltaba su declaración)
    private MonoBehaviour[] playerScripts;

    // Si se usan cameraInputScripts, guardamos su estado para restaurar
    private bool[] previousCameraInputEnabled;

    // Scripts del jugador provistos en inspector
    private bool[] previousPlayerEnabled;

    private int[] slotOcupadoPorClavo;

    // Previene que dos clavos ocupen el mismo slot: mapa slot -> clavo
    private int[] clavoEnSlot;

    private Vector2[] initialPositions;
    private float[] lastHitTime;

    private bool isOpen = false;

    // Snapping / preview state
    private int previewSlotIndex = -1;
    private int draggingIndex = -1;
    private Color[] originalSlotColors;

    void Start()
    {
        if (clavos == null) clavos = new RectTransform[0];
        if (slots == null) slots = new RectTransform[0];

        clavoColocado = new bool[clavos.Length];
        golpesActuales = new int[clavos.Length];
        lastHitTime = new float[clavos.Length];

        slotOcupadoPorClavo = new int[clavos.Length];
        for (int i = 0; i < slotOcupadoPorClavo.Length; i++)
            slotOcupadoPorClavo[i] = -1; // -1 = no colocado
                
        clavoEnSlot = new int[slots.Length];
        for (int i = 0; i < clavoEnSlot.Length; i++)
            clavoEnSlot[i] = -1;

        initialPositions = new Vector2[clavos.Length];
        for (int i = 0; i < clavos.Length; i++)
        {
            if (clavos[i] != null)
                initialPositions[i] = clavos[i].anchoredPosition;
            else
                initialPositions[i] = Vector2.zero;
        }

        // Cache original slot colors (Image or RawImage)
        originalSlotColors = new Color[slots.Length];
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null) { originalSlotColors[i] = Color.white; continue; }
            var raw = slots[i].GetComponentInChildren<RawImage>();
            if (raw != null) { originalSlotColors[i] = raw.color; continue; }
            var img = slots[i].GetComponentInChildren<Image>();
            if (img != null) { originalSlotColors[i] = img.color; continue; }
            originalSlotColors[i] = Color.white;
        }

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
        if (isOpen) return;

        if (inventory == null || !inventory.HasTool(requiredTool))
        {
            if (mensajeErrorUI != null)
                mensajeErrorUI.SetActive(true);

            return;
        }

        minigameUI.SetActive(true);
        isOpen = true;

        // ---------------------------------------------------------
        // BLOQUEAR CÁMARA
        // ---------------------------------------------------------
        if (Camera.main != null)
        {
            cameraScripts = Camera.main.GetComponents<MonoBehaviour>();
            foreach (var script in cameraScripts)
            {
                script.enabled = false;
            }
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
        if (!isOpen) return;

        if (minigameUI != null)
            minigameUI.SetActive(false);

        if (completadoUI != null)
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
        if (clavo == null || index < 0 || index >= clavos.Length) return;

        int nearest = FindNearestSlotIndex(clavo.anchoredPosition);
        if (nearest != -1)
        {
            float distancia = Vector2.Distance(clavo.anchoredPosition, slots[nearest].anchoredPosition);
            if (distancia <= snapDistance)
            {
                // Si el slot está ocupado por otro clavo, no hacemos snap (podemos usar swap si se desea)
                if (clavoEnSlot[nearest] != -1 && clavoEnSlot[nearest] != index)
                {
                    // devolver a posición inicial suavemente
                    StartCoroutine(SmoothMoveTo(clavo, initialPositions[index], 0.25f));
                    ClearSlotPreview();
                    return;
                }

                // Liberar slot anterior (si lo tenía)
                int prevSlot = slotOcupadoPorClavo[index];
                if (prevSlot != -1 && prevSlot < clavoEnSlot.Length)
                    clavoEnSlot[prevSlot] = -1;

                // Snap suave al slot
                clavoColocado[index] = true;
                slotOcupadoPorClavo[index] = nearest;
                clavoEnSlot[nearest] = index;
                StartCoroutine(SmoothMoveTo(clavo, slots[nearest].anchoredPosition, 0.18f));
                ClearSlotPreview();
                return;
            }
        }

        // No ha hecho snap: devolver a posición inicial suavemente
        StartCoroutine(SmoothMoveTo(clavo, initialPositions[index], 0.22f));
        // limpiar preview
        ClearSlotPreview();

        int oldSlot = slotOcupadoPorClavo[index];
        if (oldSlot != -1 && oldSlot < clavoEnSlot.Length)
            clavoEnSlot[oldSlot] = -1;

        slotOcupadoPorClavo[index] = -1;
        clavoColocado[index] = false;
    }

    // Busca el índice del slot más cercano (sin comprobar ocupación)
    int FindNearestSlotIndex(Vector2 position)
    {
        int best = -1;
        float bestDist = float.MaxValue;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null) continue;
            float d = Vector2.Distance(position, slots[i].anchoredPosition);
            if (d < bestDist)
            {
                bestDist = d;
                best = i;
            }
        }
        return best;
    }

    // Suaviza movimiento a una posición objetivo en tiempo dado
    IEnumerator SmoothMoveTo(RectTransform clavo, Vector2 target, float duration)
    {
        if (clavo == null)
            yield break;

        Vector2 start = clavo.anchoredPosition;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float f = Mathf.Clamp01(t / duration);
            clavo.anchoredPosition = Vector2.Lerp(start, target, Mathf.SmoothStep(0f, 1f, f));
            yield return null;
        }
        clavo.anchoredPosition = target;
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

        if (completadoUI != null)
            completadoUI.SetActive(true);

        // Cerrar minijuego después de 1 segundo
        StartCoroutine(CloseAfterDelay(1f));
    }

    private IEnumerator CloseAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        CloseMinigame();
    }

    // ---------------------------------------------------------
    // EVENTOS DE ARRASTRE (ahora con preview / magnetismo)
    // ---------------------------------------------------------
    void AddDragEvents(RectTransform clavo, int index)
    {
        if (clavo == null) return;

        EventTrigger trigger = clavo.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = clavo.gameObject.AddComponent<EventTrigger>();

        // Evitar duplicados removiendo entradas de estos tipos
        trigger.triggers.RemoveAll(e =>
            e.eventID == EventTriggerType.BeginDrag ||
            e.eventID == EventTriggerType.Drag ||
            e.eventID == EventTriggerType.EndDrag
        );

        // Asegurar CanvasGroup
        var cg = clavo.GetComponent<CanvasGroup>();
        if (cg == null) cg = clavo.gameObject.AddComponent<CanvasGroup>();

        // BEGIN DRAG
        EventTrigger.Entry begin = new EventTrigger.Entry();
        begin.eventID = EventTriggerType.BeginDrag;
        begin.callback.AddListener((data) =>
        {
            cg.blocksRaycasts = false;
            draggingIndex = index;
            previewSlotIndex = -1;
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

            // Movimiento base del clavo
            clavo.anchoredPosition = localPoint;

            // Previsualización / magnetismo: encontrar nearest slot y aplicar efecto si está cerca
            int nearest = FindNearestSlotIndex(localPoint);
            if (nearest != -1)
            {
                float dist = Vector2.Distance(localPoint, slots[nearest].anchoredPosition);
                if (dist <= snapDistance)
                {
                    // Resaltar slot (si está libre o es el mismo clavo)
                    if (clavoEnSlot[nearest] == -1 || clavoEnSlot[nearest] == index)
                    {
                        PreviewSlot(nearest, true);
                        // Magnetismo suave hacia el slot
                        clavo.anchoredPosition = Vector2.Lerp(clavo.anchoredPosition, slots[nearest].anchoredPosition, Time.deltaTime * snapLerpSpeed);
                    }
                    else
                    {
                        // Slot ocupado por otro clavo: marcar como ocupado
                        PreviewSlot(nearest, false);
                    }
                }
                else
                {
                    // fuera de rango: limpiar preview
                    ClearSlotPreview();
                }
            }
            else
            {
                ClearSlotPreview();
            }
        });
        trigger.triggers.Add(drag);

        // END DRAG
        EventTrigger.Entry end = new EventTrigger.Entry();
        end.eventID = EventTriggerType.EndDrag;
        end.callback.AddListener((data) =>
        {
            cg.blocksRaycasts = true;
            // En EndDrag hacemos la colocación definitiva (con suave animación)
            TryPlaceClavo(clavo, index);
            draggingIndex = -1;
            previewSlotIndex = -1;
            ClearSlotPreview();
        });
        trigger.triggers.Add(end);
    }

    // Muestra previsualización en slot: valid=true slot libre, false ocupado
    void PreviewSlot(int slotIndex, bool valid)
    {
        if (slotIndex < 0 || slotIndex >= slots.Length) return;
        if (previewSlotIndex == slotIndex) return; // ya está mostrando ese slot

        // Restaurar color anterior del preview previo
        ClearSlotPreview();

        previewSlotIndex = slotIndex;
        var raw = slots[slotIndex].GetComponentInChildren<RawImage>();
        if (raw != null)
        {
            raw.color = valid ? slotHighlightColor : slotOccupiedColor;
            return;
        }
        var img = slots[slotIndex].GetComponentInChildren<Image>();
        if (img != null)
        {
            img.color = valid ? slotHighlightColor : slotOccupiedColor;
            return;
        }
    }

    void ClearSlotPreview()
    {
        if (previewSlotIndex == -1) return;
        int i = previewSlotIndex;
        if (i >= 0 && i < slots.Length && slots[i] != null)
        {
            var raw = slots[i].GetComponentInChildren<RawImage>();
            if (raw != null) raw.color = originalSlotColors[i];
            var img = slots[i].GetComponentInChildren<Image>();
            if (img != null) img.color = originalSlotColors[i];
        }
        previewSlotIndex = -1;
    }

    // ---------------------------------------------------------
    // EVENTOS DE CLICK (MARTILLAR)
    // ---------------------------------------------------------
    void AddClickEvents(RectTransform clavo, int index)
    {
        if (clavo == null) return;

        EventTrigger trigger = clavo.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = clavo.gameObject.AddComponent<EventTrigger>();

        // Evitar duplicados removiendo PointerClick previos
        trigger.triggers.RemoveAll(e => e.eventID == EventTriggerType.PointerClick);

        EventTrigger.Entry click = new EventTrigger.Entry();
        click.eventID = EventTriggerType.PointerClick;
        click.callback.AddListener((data) =>
        {
            if (!clavoColocado[index]) return;

            // Cooldown simple para evitar clicks excesivos
            if (Time.time - lastHitTime[index] < hammerCooldown) return;
            lastHitTime[index] = Time.time;

            golpesActuales[index]++;

            if (golpesActuales[index] >= golpesNecesarios)
            {
                CambiarColorSlot(index);
                CheckCompletion();
            }
        });

        trigger.triggers.Add(click);
    }

    void CambiarColorSlot(int index)
    {
        if (index < 0 || index >= slotOcupadoPorClavo.Length) return;

        int slotReal = slotOcupadoPorClavo[index];

        if (slotReal == -1)
        {
            Debug.LogWarning("El clavo " + index + " no está colocado en ningún slot.");
            return;
        }

        if (slotReal < 0 || slotReal >= slots.Length || slots[slotReal] == null)
        {
            Debug.LogWarning("Slot inválido: " + slotReal);
            return;
        }

        // Intentar RawImage
        var raw = slots[slotReal].GetComponentInChildren<RawImage>();
        if (raw != null)
        {
            raw.color = Color.green;
            return;
        }

        // Intentar Image
        var img = slots[slotReal].GetComponentInChildren<Image>();
        if (img != null)
        {
            img.color = Color.green;
            return;
        }

        Debug.LogWarning("El slot " + slotReal + " no tiene RawImage ni Image.");
    }
}