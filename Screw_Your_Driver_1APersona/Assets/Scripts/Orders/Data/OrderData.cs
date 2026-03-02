using UnityEngine;

[System.Serializable]
public class OrderData // Runtime data representing a specific active order
{
    public int orderID;
    public RecipeData recipe;
    public float maxTime;
    public float remainingTime;
    public OrderState state;
}