﻿using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine;

/*
 * Handles Gamepad Join/Removal. Invokes Events.
 */
public class DeviceManager
{
    public delegate void GamepadChange(Gamepad gamepad);
    public event GamepadChange OnGamepadAdded;
    public event GamepadChange OnGamepadRemoved;
    public List<Gamepad> gamepads = new List<Gamepad>();

    public DeviceManager()
    {
        foreach (var device in InputSystem.devices)
        {
            if (device is Gamepad) gamepads.Add((Gamepad) device);
        }

        InputSystem.onDeviceChange += (device, change) => DeviceChanged(device, change);
    }

    ~DeviceManager()
    {
        InputSystem.onDeviceChange -= (device, change) => DeviceChanged(device, change);
    }

    void DeviceChanged(InputDevice device, InputDeviceChange change)
    {
        if (!(device is Gamepad))
            return;

        Gamepad gamepad = (Gamepad)device;

        switch (change)
        {
            case InputDeviceChange.Disconnected:
            case InputDeviceChange.Reconnected:
                break;
            case InputDeviceChange.Added:
                gamepads.Add(gamepad);
                OnGamepadAdded?.Invoke(gamepad);
                break;
            case InputDeviceChange.Removed:
                gamepads.Remove(gamepad);
                OnGamepadRemoved?.Invoke(gamepad);
                break;
            default:
                break;
        }
    }
}
