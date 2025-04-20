using System;
using Cinemachine;
using UnityEngine;

public class JoystickCameraControl : MonoBehaviour
{
    [Header("Joystick Settings")]
    [SerializeField]
    private RectTransform joystickOutline;
    [SerializeField]
    private RectTransform joysticKnob;
    [Header("Joystick Movement Settings")]
    [SerializeField]
    private float moveFactor = 1f;
    private Vector3 clickedPosition;
    private Vector3 move;
    private bool canControl;
    private float maxdistanceOfKnob = 0.3f;
    [Header("Camera Rotation Settings")]
    public float rotationSpeed = 2f;
    [SerializeField]
    private CinemachineVirtualCamera virtualCamera;
    [SerializeField]
    private Transform player;
    private Transform cameraTransform;
    public RectTransform JoystickOutline
    {
        get
        {
            return this.joystickOutline;
        }
    }

    public RectTransform JoysticKnob
    {
        get
        {
            return this.joysticKnob;
        }
    }

    private void Start()
    {
        if (this.virtualCamera != null)
        {
            this.cameraTransform = this.virtualCamera.transform;
        }
        this.HideJoystick();
    }

    private void Update()
    {
        if (this.canControl)
        {
            this.ControlJoystick();
        }
        if (this.cameraTransform != null)
        {
            Vector3 moveVector = this.GetMoveVector();
            if (moveVector.magnitude > 0.1f)
            {
                this.RotateCamera(moveVector);
                this.RotatePlayer(moveVector);
            }
        }
    }

    public void ClickOnJoystrickZoneCallback()
    {
        if (Input.touchCount > 0)
        {
            this.clickedPosition = Input.GetTouch(0).position;
        }
        else
        {
            this.clickedPosition = Input.mousePosition;
        }
        this.joystickOutline.position = this.clickedPosition;
        this.joysticKnob.position = this.joystickOutline.position;
        this.ShowJoystick();
        this.canControl = true;
    }

    private void ShowJoystick()
    {
        this.joystickOutline.gameObject.SetActive(true);
    }

    private void HideJoystick()
    {
        this.joystickOutline.gameObject.SetActive(false);
        this.canControl = false;
        this.move = Vector3.zero;
    }

    private void ControlJoystick()
    {
        Vector3 a;
        if (Input.touchCount > 0)
        {
            a = Input.GetTouch(0).position;
        }
        else
        {
            a = Input.mousePosition;
        }
        Vector3 vector = a - this.clickedPosition;
        float num = vector.magnitude * this.moveFactor / (float)Screen.width;
        num = Mathf.Min(num, this.joystickOutline.rect.width / 2f);
        this.move = vector.normalized * this.maxdistanceOfKnob * num;
        Vector3 position = this.clickedPosition + this.move;
        this.joysticKnob.position = position;
        if (Input.touchCount == 0 && !Input.GetMouseButton(0))
        {
            this.HideJoystick();
        }
    }

    public Vector3 GetMoveVector()
    {
        return this.move;
    }

    private void RotateCamera(Vector3 moveInput)
    {
        float d = moveInput.x * this.rotationSpeed * Time.deltaTime;
        float d2 = moveInput.y * this.rotationSpeed * Time.deltaTime;
        this.cameraTransform.Rotate(Vector3.up * d, Space.World);
        this.cameraTransform.Rotate(Vector3.left * d2, Space.Self);
    }

    private void RotatePlayer(Vector3 moveInput)
    {
        float d = moveInput.x * this.rotationSpeed * Time.deltaTime;
        if (this.player != null)
        {
            this.player.Rotate(Vector3.up * d, Space.World);
        }
    }

    public void ClickOnCameraJoystrickZoneCallback()
    {
        this.clickedPosition = Input.mousePosition;
        this.joystickOutline.position = this.clickedPosition;
        this.joysticKnob.position = this.joystickOutline.position;
        this.ShowJoystick();
        this.canControl = true;
    }


}
