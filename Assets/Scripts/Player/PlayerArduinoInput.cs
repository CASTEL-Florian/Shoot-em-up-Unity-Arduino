using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArduinoInput : MonoBehaviour
{
    private PlayerControls controls;
    [SerializeField] private CharacterMovement controller;
    [SerializeField] private List<Shooting> shootings;
    [SerializeField] private Avatar avatar;
    private int currentShooting = 0;
    private float speed;
    [SerializeField] private HitManager hitManager;
    [SerializeField] GameObject pauseMenu;
    private Vector2 move;
    [SerializeField] private float maxAngle = 45;

    private void Start()
    {
        speed = avatar.GetSpeed();
    }
    private void Awake()
    {
        controls = new PlayerControls();
        controls.Player.SwitchWeapon.performed += ctx => SwitchWeapon();
        controls.Player.Pause.performed += ctx => Pause();

    }
    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        shootings[currentShooting].SetActive(false);
        controls.Disable();
    }

    private void Update()
    {
        shootings[currentShooting].SetActive(controls.Player.Shoot.ReadValue<float>() == 1);
    }
    private void FixedUpdate()
    {
        if (controller.IsDashing())
            return;
        controller.Move(move * speed);
    }

    private void SwitchWeapon()
    {
        shootings[currentShooting].SetActive(false);
        currentShooting += 1;
        if (currentShooting >= shootings.Count)
            currentShooting = 0;
    }


    private void Pause()
    {
        Time.timeScale = 1 - Time.timeScale;
        if (Time.timeScale == 0)
            pauseMenu.SetActive(true);
        else
            pauseMenu.SetActive(false);
    }

    public Vector2 SetMove(Vector3 angles)
    {
        angles.x = Mathf.Clamp(angles.x, -maxAngle, maxAngle);
        angles.y = Mathf.Clamp(angles.y, -maxAngle, maxAngle);
        angles.z = Mathf.Clamp(angles.x, -maxAngle, maxAngle);
        move = new Vector2(angles.x / maxAngle, angles.y/maxAngle);

        Vector2 speedVector = move + new Vector2(1, 0);
        speedVector.x = speedVector.x / 2;
        return speedVector;
    }
}
