using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    [Header("Aiming")]
    public Camera mainCamera;

    [Header("Barrel Setup")]
    public Transform barrelEnd;

    [Header("Red Bullet")]
    public GameObject RedbulletPrefab;
    public float RedbulletForce = 50f;
    public float RedfireRate = 0.5f;
    public float RednextFireTime = 0f;

    [Header("Blue Bullet")]
    public GameObject BluebulletPrefab;
    public float BluebulletForce = 50f;
    public float BluefireRate = 0.5f;
    public float BluenextFireTime = 0f;

    [Header("Current Gun Mode")]
    public Image CurrentGunMode; // Reference to the UI image for displaying the current gun mode
    private Color redColor = new Color(1f, 0.082f, 0f); // FF1500 Red
    private Color blueColor = new Color(0f, 0.106f, 1f); // 001BFF Blue

    [Header("player")]
    public Transform playerTransform; // Reference to the player transform, needed for teleportation

    int currentMode = 1; // Default to red mode
    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGamePaused())
            return; // Exit if the game is paused

        AimAtMouse();

        if (Input.GetMouseButtonDown(1))
        {
            SwitchGunMode(); //Change gun mode on right mouse button click
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (currentMode == 1 && Time.time >= RednextFireTime)
            {
                FireRed();
                RednextFireTime = Time.time + RedfireRate;
            }
            else if (currentMode == 2 && Time.time >= BluenextFireTime)
            {
                FireBlue();
                BluenextFireTime = Time.time + BluefireRate;
            }
        }
    }
    void AimAtMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane screenPlane = new Plane(-mainCamera.transform.forward, transform.position);

        if (screenPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 direction = hitPoint - transform.position;
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
    void FireRed()
    {
        GameObject Redbullet = Instantiate(RedbulletPrefab, barrelEnd.position, barrelEnd.rotation);

        Rigidbody bulletRB = Redbullet.GetComponent<Rigidbody>();

        Vector3 forceDirection = barrelEnd.forward; // Get the forward direction of the barrel
        forceDirection.z = 0;
        bulletRB.AddForce(forceDirection.normalized * RedbulletForce, ForceMode.Impulse);
    }
    void FireBlue()
    {
        GameObject Bluebullet = Instantiate(BluebulletPrefab, barrelEnd.position, barrelEnd.rotation);
        Bluebullet.GetComponent<BlueBullet>().SetPlayer(playerTransform); // Set the player transform for teleportation

        Rigidbody bulletRB = Bluebullet.GetComponent<Rigidbody>();

        Vector3 forceDirection = barrelEnd.forward;
        forceDirection.z = 0;
        bulletRB.AddForce(forceDirection.normalized * BluebulletForce, ForceMode.Impulse);
    }
    void SwitchGunMode()
    {
        currentMode = (currentMode == 1) ? 2 : 1; // Toggle between modes
        SwitchGunModeUI();
    }
    void SwitchGunModeUI()
    {
        CurrentGunMode.color = (currentMode == 1) ? redColor : blueColor; // Toggle between modes
    }
}

