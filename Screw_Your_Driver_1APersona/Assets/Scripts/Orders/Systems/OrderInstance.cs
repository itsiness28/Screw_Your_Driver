using UnityEngine;
using System.Collections;

public class OrderInstance : MonoBehaviour
{
    public OrderData data;
    private OrderManager manager;

    // Initializes this order instance and starts its countdown timer
    public void Init(OrderData newData, OrderManager orderManager)
    {
        data = newData;
        manager = orderManager;
        StartCoroutine(OrderTimer());
    }

    // Handles the order countdown over time
    private IEnumerator OrderTimer()
    {
        while (data.remainingTime > 0f)
        {
            data.remainingTime -= Time.deltaTime;
            yield return null;
        }

        FailOrder();
    }

    // Marks the order as failed and notifies the OrderManager
    void FailOrder()
    {
        data.state = OrderState.Failed;
        manager.OnOrderFinished(this);
    }

    // Marks the order as completed and notifies the OrderManager
    public void CompleteOrder()
    {
        data.state = OrderState.Completed;
        manager.OnOrderFinished(this);
    }
}

