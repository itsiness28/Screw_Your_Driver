using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "RecipeData", menuName = "Orders/Recipe")]
public class RecipeData : ScriptableObject
{
    public string recipeName;
    public float baseTime;
    public Sprite icon;

    public List<string> ingredients;
}
