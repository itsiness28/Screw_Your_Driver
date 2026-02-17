using System.Collections.Generic;
using UnityEngine;

// Manages the lifecycle of orders: spawning, tracking active orders,
// and removing them when completed or failed
public class OrderManager : MonoBehaviour
{
    [Header("Order Settings")]
    public int maxActiveOrders = 3;
    public float timeBetweenOrders = 5f;

    [Header("Recipes")]
    public List<RecipeData> availableRecipes;

    [Header("Runtime")]
    public List<OrderInstance> activeOrders = new List<OrderInstance>();

    private float spawnTimer;
    private int nextOrderID = 0;

    [SerializeField] private OrderInstance orderPrefab;

    //momentanio aqui
    private Camera playerCamera;

    void Update()
    {
        spawnTimer += Time.deltaTime;

        //if (spawnTimer >= timeBetweenOrders)
        //{
        //    TrySpawnOrder();
        //    spawnTimer = 0f;
        //}
    }
    //
    private void OnTriggerStay(Collider other)
    {
        Debug.Log("Detectando algo");

        if (other.CompareTag("Player"))
        {
            Debug.Log("Jugador dentro");

            if (Input.GetKeyDown(KeyCode.E) && IsLookingAtObject())
            {
                Debug.Log("E presionada");
                TrySpawnOrder();
            }
        }
    }

    // Attempts to spawn a new order if the maximum number of active orders
    // has not been reached
    void TrySpawnOrder()
    {
        if (activeOrders.Count >= maxActiveOrders)
            return;

        SpawnOrder();
    }

    // Creates a new order with random recipe data and initializes its instance
    void SpawnOrder()
    {
        RecipeData recipe = GetRandomRecipe();
        if (recipe == null) return;

        OrderData data = new OrderData
        {
            orderID = GenerateOrderID(),
            recipe = recipe,
            maxTime = recipe.baseTime,
            remainingTime = recipe.baseTime,
            state = OrderState.Active
        };

        Debug.Log($"[OrderManager] Spawned order {data.orderID} ({recipe.recipeName})");

        OrderInstance instance = CreateOrderInstance(data);
        activeOrders.Add(instance);
    }

    // Instantiates and initializes an OrderInstance in the scene
    OrderInstance CreateOrderInstance(OrderData data)
    {
        OrderInstance instance = Instantiate(orderPrefab);
        instance.Init(data, this);
        return instance;
    }

    // Called by an OrderInstance when it finishes (completed or failed)
    public void OnOrderFinished(OrderInstance instance)
    {
        Debug.Log($"[OrderManager] Order {instance.data.orderID} removed");

        activeOrders.Remove(instance);
        Destroy(instance.gameObject);
    }

    // Returns a random recipe from the available recipes list
    RecipeData GetRandomRecipe()
    {
        if (availableRecipes == null || availableRecipes.Count == 0)
        {
            Debug.LogError("No recipes available in OrderManager.");
            return null;
        }

        int index = Random.Range(0, availableRecipes.Count);
        return availableRecipes[index];
    }

    // Generates a unique incremental ID for each order
    int GenerateOrderID()
    {
        return nextOrderID++;
    }

    bool IsLookingAtObject()
    {
        //Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        //RaycastHit hit;

        //if (Physics.Raycast(ray, out hit, 3f))
        //{
        //    if (hit.collider == this.GetComponent<Collider>())
        //    {
        //        return true;
        //    }
        //}

        //return false;
    }
}