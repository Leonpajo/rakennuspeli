using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Food Prefabs")]
    public GameObject fishPrefab;
    public GameObject chickenPrefab;
    public GameObject breadPrefab;

    private GameObject currentItem;

    public void SpawnFood(FoodType foodType)
    {
        if (currentItem != null) return;

        GameObject prefabToSpawn = null;

        if (foodType == FoodType.Fish)
            prefabToSpawn = fishPrefab;
        else if (foodType == FoodType.Chicken)
            prefabToSpawn = chickenPrefab;
        else if (foodType == FoodType.Bread)
            prefabToSpawn = breadPrefab;

        if (prefabToSpawn == null)
        {
            Debug.LogWarning("Missing prefab for " + foodType);
            return;
        }

        currentItem = Instantiate(
            prefabToSpawn,
            transform.position,
            transform.rotation
        );

        if (currentItem.GetComponent<Rigidbody>() == null)
            currentItem.AddComponent<Rigidbody>();
    }

    public void ClearCurrentItem()
    {
        currentItem = null;
    }
}