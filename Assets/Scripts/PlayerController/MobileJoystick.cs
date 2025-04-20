using System;
using UnityEngine;

public class MobileJoystick : MonoBehaviour
{
    // Các biến thành phần
    [Header("Elements")]
    [SerializeField]
    private RectTransform joystickOutline;
    [SerializeField]
    private RectTransform joystickKnob;
    [SerializeField]
    private Transform playerTransform;

    [Header("Settings")]
    [SerializeField]
    private float moveFactor = 1f;

    private Vector2 clickedPosition;
    private Vector2 move;
    private float maxDistanceOfKnob = 100f;
    [SerializeField]
    private float rotationSpeed = 1500f;

    private bool isRotatingCamera;
    private bool isRightMouseHeld;

    // Các phương thức khởi tạo
    private void Start()
    {
        this.HideJoystick();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            this.CheckIsRightMouseDown();
        }
        if (this.isRightMouseHeld && Input.GetMouseButton(1))
        {
            this.RotateCamera();
        }
        if (Input.GetMouseButtonUp(1))
        {
            this.isRightMouseHeld = false;
            this.isRotatingCamera = false;
            this.HideJoystick();
        }
    }

    private void RotateCamera()
    {
        this.isRotatingCamera = true;
        Vector2 vector = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - this.clickedPosition;
        if (vector.magnitude > this.maxDistanceOfKnob)
        {
            vector = vector.normalized * this.maxDistanceOfKnob;
        }
        this.joystickKnob.position = this.clickedPosition + vector;
        this.move = Vector2.zero;
        if (this.playerTransform != null && vector != Vector2.zero)
        {
            float yAngle = vector.x * this.rotationSpeed * Time.deltaTime;
            this.playerTransform.Rotate(0f, yAngle, 0f);
        }
    }


    private void CheckIsRightMouseDown()
    {
        this.isRightMouseHeld = true;
        this.clickedPosition = Input.mousePosition;
        this.joystickOutline.position = this.clickedPosition;
        this.joystickKnob.position = this.clickedPosition;
    }

    public void ShowJoystick()
    {
        this.joystickOutline.gameObject.SetActive(true);
    }

    private void HideJoystick()
    {
        this.joystickOutline.gameObject.SetActive(false);
        this.move = Vector2.zero;
    }

    public Vector2 GetMoveVector()
    {
        if (this.isRotatingCamera)
        {
            return Vector2.zero;
        }
        return this.move;
    }

  
}
