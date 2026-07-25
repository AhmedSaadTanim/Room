using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

public class SimulatorToggle : MonoBehaviour
{
    public GameObject simulator;

    void OnEnable()
    {
        InputDevices.deviceConnected += OnChanged;
        InputDevices.deviceDisconnected += OnChanged;
        Refresh();
    }

    void OnDisable()
    {
        InputDevices.deviceConnected -= OnChanged;
        InputDevices.deviceDisconnected -= OnChanged;
    }

    void OnChanged(InputDevice d) => Refresh();

    void Refresh()
    {
        var devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.HeadMounted, devices);
        simulator.SetActive(devices.Count == 0);
    }
}