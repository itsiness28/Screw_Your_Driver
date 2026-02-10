using UnityEngine;

[CreateAssetMenu(fileName = "RecipeData", menuName = "Orders/Recipe")]

// ScriptableObject that defines the base data of a recipe (shared, read-only during gameplay)
public class RecipeData : ScriptableObject
{
    public string recipeName;
    public float baseTime;
    public Sprite icono;
}
