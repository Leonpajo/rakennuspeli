using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class DeliveryManager : MonoBehaviour
{
    [Header("Spawner")]
    public ItemSpawner itemSpawner;

    [Header("Player")]
    public Transform player;

    [Header("House Locations")]
    public Transform fishHouse;
    public Transform chickenHouse;
    public Transform breadHouse;

    [Header("Settings")]
    public float deliveryDistance = 3f;
    public float itemRespawnDistance = 25f;

    [Header("UI")]
    public TMP_Text scoreboardText;
    public TMP_Text notificationText;
    public float notificationTime = 2f;

    private FoodType currentDelivery;

    private int fishDelivered;
    private int chickenDelivered;
    private int breadDelivered;

    private bool isCompletingDelivery;
    private bool isRespawningItem;

    void Start()
    {
        UpdateScoreboard();

        if (notificationText != null)
            notificationText.gameObject.SetActive(false);

        PickNewDelivery();
    }

    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            TryDeliver();
            return;
        }

        CheckLostItem();
    }

    void PickNewDelivery()
    {
        isCompletingDelivery = false;
        isRespawningItem = false;

        currentDelivery = (FoodType)Random.Range(0, 3);

        itemSpawner.SpawnFood(currentDelivery);

        ShowNotification("Pick up the " + currentDelivery + "!");
    }

    void TryDeliver()
    {
        if (isRespawningItem) return;

        GameObject food = FindCurrentFood();
        if (food == null) return;

        FoodItem foodItem = food.GetComponent<FoodItem>();
        if (foodItem == null) return;
        if (foodItem.foodType != currentDelivery) return;

        Transform correctHouse = GetHouseForFood(currentDelivery);

        float distance = Vector3.Distance(
            food.transform.position,
            correctHouse.position
        );

        if (distance <= deliveryDistance)
        {
            isCompletingDelivery = true;
            isRespawningItem = true;

            AddScore(currentDelivery);

            Destroy(food);
            itemSpawner.ClearCurrentItem();

            UpdateScoreboard();

            ShowNotification("Item delivered! You can pick the next one up.");

            CancelInvoke(nameof(RespawnCurrentDelivery));
            Invoke(nameof(PickNewDelivery), 1f);

            return;
        }
    }

    void CheckLostItem()
    {
        if (isCompletingDelivery) return;
        if (isRespawningItem) return;
        if (player == null) return;

        GameObject food = FindCurrentFood();

        if (food == null)
        {
            StartRespawn("");
            return;
        }

        float distanceFromPlayer = Vector3.Distance(
            player.position,
            food.transform.position
        );

        if (distanceFromPlayer > itemRespawnDistance)
        {
            Destroy(food);
            itemSpawner.ClearCurrentItem();

            StartRespawn("Item was too far away and respawned!");
        }
    }

    GameObject FindCurrentFood()
    {
        GameObject[] foods = GameObject.FindGameObjectsWithTag("Food");

        foreach (GameObject food in foods)
        {
            if (food == null) continue;

            FoodItem foodItem = food.GetComponent<FoodItem>();
            if (foodItem == null) continue;

            if (foodItem.foodType == currentDelivery)
                return food;
        }

        return null;
    }

    void StartRespawn(string message)
    {
        isRespawningItem = true;

        itemSpawner.ClearCurrentItem();

        if (!string.IsNullOrEmpty(message))
        {
            ShowNotification(message);
        }

        Invoke(nameof(RespawnCurrentDelivery), 0.5f);
    }

    void RespawnCurrentDelivery()
    {
        itemSpawner.SpawnFood(currentDelivery);
        isRespawningItem = false;
    }

    void AddScore(FoodType foodType)
    {
        if (foodType == FoodType.Fish)
            fishDelivered++;

        if (foodType == FoodType.Chicken)
            chickenDelivered++;

        if (foodType == FoodType.Bread)
            breadDelivered++;
    }

    void UpdateScoreboard()
    {
        if (scoreboardText == null) return;

        scoreboardText.text =
            "Delivered\n" +
            "Fish: " + fishDelivered + "\n" +
            "Chicken: " + chickenDelivered + "\n" +
            "Bread: " + breadDelivered;
    }

    void ShowNotification(string message)
    {
        if (notificationText == null) return;

        StopAllCoroutines();
        StartCoroutine(NotificationRoutine(message));
    }

    IEnumerator NotificationRoutine(string message)
    {
        notificationText.gameObject.SetActive(true);
        notificationText.text = message;

        yield return new WaitForSeconds(notificationTime);

        notificationText.gameObject.SetActive(false);
    }

    Transform GetHouseForFood(FoodType foodType)
    {
        if (foodType == FoodType.Fish)
            return fishHouse;

        if (foodType == FoodType.Chicken)
            return chickenHouse;

        return breadHouse;
    }
}