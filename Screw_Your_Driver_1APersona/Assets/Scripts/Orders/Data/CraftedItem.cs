using UnityEngine;

public class CraftedItem : MonoBehaviour
{
    public RecipeData recipe;
}
//Cuando acabe el minijuego y instanciemos el prefab habra que meter una logica tipo:
//GameObject furniture = Instantiate(prefab);

//CraftedItem crafted = furniture.GetComponent<CraftedItem>();
//crafted.recipe = recipeCompleted;