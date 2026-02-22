using UnityEngine;
using TMPro;
using System.ComponentModel;

public class OrderUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI orderText;

    private void OnEnable()
    {
        OrderManager.OnOrderCreated += ShowOrder;
    }

    private void OnDisable()
    {
        OrderManager.OnOrderCreated -= ShowOrder;
        orderText.text = "";
    }

    private void ShowOrder(OrderInstance instance)
    {
        orderText.text = BuildText(instance);
        
    }
    private string BuildText(OrderInstance instance)
    {
        var data = instance.data;

        string result = "NEW ORDER\n";
        result += $"ID: {data.orderID}\n";
        result += $"Recipe: {data.recipe.recipeName}\n";
        result += "Ingredients:\n";

        foreach (var ingredient in data.recipe.ingredients)
        {
            result += $"- {ingredient}\n";
        }

        return result;
    }
}
