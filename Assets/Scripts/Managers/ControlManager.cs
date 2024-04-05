
using UnityEngine;

public class ControlManager : MonoBehaviour
{
    public static ControlManager instance { private set; get; }

    [SerializeField] private KeyCode forwardKey = default;
    [SerializeField] private KeyCode backwardKey = default;
    [SerializeField] private KeyCode leftKey = default;
    [SerializeField] private KeyCode rightKey = default;
    [SerializeField] private KeyCode upKey = default;
    [SerializeField] private KeyCode downKey = default;
    [SerializeField] private KeyCode boostKey = default;
    [SerializeField] private KeyCode primaryWeaponKey = default;

    public readonly EventVariable<ControlManager, KeyState> boostKeyState;
    public readonly EventVariable<ControlManager, KeyState> primaryWeaponKeyState;

    private ControlManager()
    {
        boostKeyState = new EventVariable<ControlManager, KeyState>(this, KeyState.Released);
        primaryWeaponKeyState = new EventVariable<ControlManager, KeyState>(this, KeyState.Released);
    }

    public void Initialize()
    {
        instance = this;
    }

    private void Update()
    {
        Update_Key(boostKeyState, boostKey);
        Update_Key(primaryWeaponKeyState, primaryWeaponKey);
    }

    private void Update_Key(EventVariable<ControlManager, KeyState> eventVariable, KeyCode keyCode)
    {
        if (Input.GetKeyDown(keyCode))
            eventVariable.value = KeyState.Down;
        else if (Input.GetKey(keyCode))
            eventVariable.value = KeyState.Pressed;
        else if (Input.GetKeyUp(keyCode))
            eventVariable.value = KeyState.Up;
        else
            eventVariable.value = KeyState.Released;
    }

    public Vector3 GetMoveDirection()
    {
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(forwardKey))
            direction += Vector3.forward;

        if (Input.GetKey(backwardKey))
            direction += Vector3.back;

        if (Input.GetKey(leftKey))
            direction += Vector3.left;

        if (Input.GetKey(rightKey))
            direction += Vector3.right;

        if (Input.GetKey(upKey))
            direction += Vector3.up;

        if (Input.GetKey(downKey))
            direction += Vector3.down;

        return direction.normalized;
    }

    public bool IsFiringPrimaryWeapon()
    {
        return primaryWeaponKeyState.value == KeyState.Down || primaryWeaponKeyState.value == KeyState.Pressed;
    }

    public bool StartedBoosting()
    {
        return Input.GetKeyDown(boostKey);
    }

    public bool IsBoosting()
    {
        return Input.GetKey(boostKey);
    }

    public bool StoppedBoosting()
    {
        return Input.GetKeyUp(boostKey);
    }    
}