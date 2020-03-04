﻿using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class MenuPlayer : MonoBehaviour
{
    [SerializeField]
    GameObject fishPlayerPrefab;

    [SerializeField]
    GameObject gamepadTowerPlayerPrefab;

    [SerializeField]
    GameObjectSet menuPlayers;

    [SerializeField]
    GameEvent OnPlayerLock;

    [SerializeField]
    GameEvent OnPlayerSwitch;

    public bool isFish;
    public static int fishCount;
    PlayerInput playerInput;

    private void Start()
    {
        if (isFish) fishCount++;
        playerInput = GetComponent<PlayerInput>();
        playerInput.playerControls.Gamepad.PressDPad.performed += OnSwitchRole;
        playerInput.playerControls.Gamepad.PressButtonSouth.performed += OnLockRole;
    }

    public void OnLockRole(InputAction.CallbackContext obj)
    {
        playerInput.enabled = false;
        OnPlayerLock.Raise(gameObject);
    }

    public void OnSwitchRole(InputAction.CallbackContext ctx)
    {
        int towerCount = menuPlayers.Count - fishCount;

        if (isFish)
        {
            // There has to be at least one fish 
            // There can be no more than 2 tower players
            if (fishCount <= 1 || towerCount >= 2)
                return;
            else
                fishCount--;
        }

        isFish = !isFish;
        OnPlayerSwitch.Raise(gameObject);
    }

    public GameObject GetRolePrefab()
    {
        if (isFish)
            return fishPlayerPrefab;
        else
            return gamepadTowerPlayerPrefab;
    }
}
