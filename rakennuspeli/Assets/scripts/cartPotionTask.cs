// CartPotionTask.cs

using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
public class CartPotionTask : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Potion Task")]
    public int potionsNeeded = 5;

    [Header("Village Delivery")]
    public Transform villageGoal;
    public float deliveryDistance = 5f;

    [Header("UI")]
    public TMP_Text taskText;
    public TMP_Text notificationText;

    [Header("Carry Settings")]
    public float holdDistance = 3f;
    public float holdSmoothSpeed = 10f;

    private int potionsInside;

    private bool taskStarted;
    private bool cartCanMove;
    private bool gameComplete;

    private Rigidbody rb;
    private Collider cartCollider;

    private bool beingCarried;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cartCollider = GetComponent<Collider>();

        rb.useGravity = true;

        UpdateUI();
    }

    public void StartPotionTask()
    {
        taskStarted = true;

        UpdateUI();
    }

    void Update()
    {
        if (!taskStarted) return;

        if (gameComplete) return;

        HandleCarry();

        CheckVillageDelivery();
    }

    void HandleCarry()
    {
        if (!cartCanMove) return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            beingCarried = !beingCarried;

            if (cartCollider != null)
                cartCollider.enabled = !beingCarried;
        }

        if (!beingCarried) return;

        Vector3 forwardFlat = player.forward;
        forwardFlat.y = 0f;
        forwardFlat.Normalize();

        Vector3 targetPosition =
            player.position + forwardFlat * holdDistance;

        targetPosition.y = transform.position.y;

        rb.MovePosition(Vector3.Lerp(
            transform.position,
            targetPosition,
            Time.deltaTime * holdSmoothSpeed
        ));

        Quaternion flatRotation = Quaternion.Euler(
            0f,
            player.eulerAngles.y + 180f,
            0f
        );

        rb.MoveRotation(flatRotation);
    }

    void CheckVillageDelivery()
    {
        if (!cartCanMove) return;
        if (villageGoal == null) return;

        float distance = Vector3.Distance(
            transform.position,
            villageGoal.position
        );

        if (distance <= deliveryDistance)
        {
            gameComplete = true;

            if (notificationText != null)
            {
                notificationText.gameObject.SetActive(true);

                notificationText.text =
                    "GAME COMPLETE\nCart delivered to village!";
            }

            if (taskText != null)
            {
                taskText.text =
                    "GAME COMPLETE\nYou delivered the potion cart!";
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!taskStarted) return;

        bool isPotion =
            other.CompareTag("Potion") ||
            other.transform.root.CompareTag("Potion");

        if (!isPotion) return;

        potionsInside++;

        Destroy(other.transform.root.gameObject);

        if (potionsInside >= potionsNeeded)
        {
            cartCanMove = true;

            ShowNotification(
                "Cart full! Carry it to the village."
            );
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        if (taskText == null) return;

        if (!taskStarted)
        {
            taskText.text = "";
            return;
        }

        taskText.text =
            "Task:\nFill cart with potions\n\n" +
            "Potions: " +
            potionsInside +
            "/" +
            potionsNeeded;

        if (cartCanMove)
        {
            taskText.text +=
                "\n\nCarry cart to windmill";
        }
    }

    void ShowNotification(string message)
    {
        if (notificationText == null) return;

        notificationText.gameObject.SetActive(true);

        notificationText.text = message;
    }
}