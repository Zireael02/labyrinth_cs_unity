using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

public class FirstPersonMovement : MonoBehaviour
{
    #region
    public float speed = 5;
    public float _speedRotation = 3;
    public Text countText;
    private int count;

    public HealthBar mHealthBar;

    public HUD HUD;

    private Animator _animator;

    public event EventHandler PlayerDied;

    [Tooltip("Amount of health")]
    public int Health = 100;

    private int startHealth;

    private readonly bool mCanTakeDamage = true;

    [Header("Running")]
    public bool canRun = true;
    public bool IsRunning { get; private set; }
    public float runSpeed = 9;
    public KeyCode runningKey = KeyCode.LeftShift;

    [Header("Fences")]
    public GameObject fance0;
    public GameObject fance1;
    public GameObject fance2;
    public GameObject fance3;
    public GameObject fance4;
    public GameObject fance5;

    new Rigidbody rigidbody;
    /// <summary> Functions to override movement speed. Will use the last added override. </summary>
    public List<System.Func<float>> speedOverrides = new();
    #endregion


    void Start()
    {
        _animator = GetComponent<Animator>();

        //mHealthBar = HUD.transform.Find("Bars_Panel/HealthBar").GetComponent<HealthBar>();
        mHealthBar.Min = 0;
        mHealthBar.Max = Health;
        startHealth = Health;
        mHealthBar.SetValue(Health);

        count = 0;

        // Run the SetCountText function to update the UI (see below)
        SetCountText();
    }

    public bool IsDead
    {
        get
        {
            return Health == 0;
        }
    }

    public void Rehab(int amount)
    {
        Health += amount;
        if (Health > startHealth)
        {
            Health = startHealth;
        }

        mHealthBar.SetValue(Health);
    }

    public void TakeDamage(int amount)
    {
        if (!mCanTakeDamage)
            return;

        Health -= amount;
        if (Health < 0)
            Health = 0;

        mHealthBar.SetValue(Health);

        if (IsDead)
        {
            Die();
        }

    }

    private void Die()
    {
        _animator.SetTrigger("death");

        if (PlayerDied != null)
        {
            PlayerDied(this, EventArgs.Empty);
        }
    }


    void Awake()
    {
        // Get the rigidbody on this.
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Update IsRunning from input.
        IsRunning = canRun && Input.GetKey(runningKey);

        // Get targetMovingSpeed.
        float targetMovingSpeed = IsRunning ? runSpeed : speed;
        if (speedOverrides.Count > 0)
        {
            targetMovingSpeed = speedOverrides[speedOverrides.Count - 1]();
        }

        // Get targetVelocity from input.
        Vector2 targetVelocity = new(Input.GetAxis("Horizontal") * targetMovingSpeed, Input.GetAxis("Vertical") * targetMovingSpeed);

        // Apply movement.
        rigidbody.velocity = transform.rotation * new Vector3(targetVelocity.x, rigidbody.velocity.y, targetVelocity.y);
    }



    // When this game object intersects a collider with 'is trigger' checked, 
    // store a reference to that collider in a variable named 'other'..
    void OnTriggerEnter(Collider other)
    {
        // ..and if the game object we intersect has the tag 'Pick Up' assigned to it..
        if (other.gameObject.CompareTag("Key"))
        {
            // Make the other game object (the pick up) inactive, to make it disappear
            other.gameObject.SetActive(false);

            // Add one to the score variable 'count'
            count ++;

            // Run the 'SetCountText()' function (see below)
            SetCountText();
        }

        if (other.gameObject.CompareTag("Health Bottle"))
        {
            // Make the other game object (the pick up) inactive, to make it disappear
            other.gameObject.SetActive(false);

            Health += 25;
            mHealthBar.SetValue(Health);
        }
    }

    // Create a standalone function that can update the 'countText' UI and check if the required amount to win has been achieved
    void SetCountText()
    {
        // Update the text field of our 'countText' variable
        countText.text = "Count: " + count.ToString();

        // Check if our 'count' is equal to or exceeded 12
        if (count == 1)
        {
            Destroy(fance0);
            Destroy(fance1);
        }
        else if (count == 2)
        {
            Destroy(fance2);
        }
        else if (count == 3)
        {
            Destroy(fance3);
        }
        else if (count == 4)
        {
            Destroy(fance4);
        }
        else
        {
            Destroy(fance5);
        }
    }
}