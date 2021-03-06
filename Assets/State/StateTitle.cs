﻿using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class StateTitle : MonoBehaviour
{
    [SerializeField]
    GameObject menuPlayerPrefab;

    [SerializeField]
    GameObject mouseTowerPlayerPrefab;

    [SerializeField]
    GameObjectSet menuPlayers;

    [SerializeField]
    GameObjectSet towerPlayers;

    [SerializeField]
    GameObject towerScore;

    [SerializeField]
    Screen gameplay;

    public int LockedPlayers { get; set; }
    DeviceManager deviceManager;

    AudioManager audio;

    private void Awake()
    {
        GetComponent<AudioSource>().Play();
        
        LockedPlayers = 0;
        deviceManager = new DeviceManager();
        deviceManager.OnGamepadAdded += CreateMenuPlayer;
        deviceManager.OnGamepadRemoved += RemoveMenuPlayer;

        for (int i = 0; i < deviceManager.gamepads.Count; i++)
        {
            CreateMenuPlayer(deviceManager.gamepads[i]);
        }

        audio = FindObjectOfType<AudioManager>();
    }

    private void OnDestroy()
    {
        deviceManager.OnGamepadAdded -= CreateMenuPlayer;
        deviceManager.OnGamepadRemoved -= RemoveMenuPlayer;
    }

    private void RemoveMenuPlayer(Gamepad gamepad)
    {
        menuPlayers.Remove(new PlayerInputDecorator(menuPlayers).GetByDeviceId(gamepad.deviceId).gameObject);
    }

    private void CreateMenuPlayer(Gamepad gamepad)
    {
        GameObject menuPlayer = Instantiate(menuPlayerPrefab);
        menuPlayer.transform.SetParent(transform);
        menuPlayer.GetComponent<PlayerInput>().RestrictToDevice(gamepad);
    }

    public void OnPlayerLock(GameEvent gameEvent)
    {
        GameObject player = gameEvent.GameObject;
        MenuPlayer menuPlayer = player.GetComponent<MenuPlayer>();
        ObjectSpawner spawner = GetComponent<ObjectSpawner>();
        GameObject role = spawner.SpawnAtRandomPosition(menuPlayer.GetRolePrefab());
        role.GetComponent<PlayerColor>().color = player.GetComponent<PlayerColor>().color;
        role.GetComponent<PlayerInput>().RestrictToDevice(menuPlayer.GetComponent<PlayerInput>().device);
        if (++LockedPlayers >= deviceManager.gamepads.Count)
            StartGame();

        if (menuPlayer.isFish)
            audio.Play("Connect");
        else
            audio.Play("Disconnect");
    }

    private void StartGame()
    {
        if (towerPlayers.Count == 0)
            Instantiate(mouseTowerPlayerPrefab);

        towerScore.GetComponent<TowerScore>().InitializeScore();
        menuPlayers.RemoveAll();

        MenuPlayer.fishCount = 0;

        ScreenManager.Instance.ChangeToScreen(gameplay);
    }
}
