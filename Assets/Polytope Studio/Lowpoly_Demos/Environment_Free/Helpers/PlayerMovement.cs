using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Elements")]
    public CharacterController controller;

    public float speed = 5f;

    public float gravity = -9.18f;

    public float jumpHeight = 3f;

    public Transform groundCheck;

    public float groundDistance = 0.4f;

    public LayerMask groundMask;

    private Vector3 velocity;

    private bool isGrounded;
    private void Update()
    {
        this.isGrounded = Physics.CheckSphere(this.groundCheck.position, this.groundDistance, this.groundMask);
        if (this.isGrounded && this.velocity.y < 0f)
        {
            this.velocity.y = -2f;
        }
        if (Input.GetKey("left shift") && this.isGrounded)
        {
            this.speed = 10f;
        }
        else
        {
            this.speed = 5f;
        }
        float axis = Input.GetAxis("Horizontal");
        float axis2 = Input.GetAxis("Vertical");
        Vector3 a = base.transform.right * axis + base.transform.forward * axis2;
        this.controller.Move(a * this.speed * Time.deltaTime);
        if (Input.GetButtonDown("Jump") && this.isGrounded)
        {
            this.velocity.y = Mathf.Sqrt(this.jumpHeight * -2f * this.gravity);
        }
        this.velocity.y = this.velocity.y + this.gravity * Time.deltaTime;
        this.controller.Move(this.velocity * Time.deltaTime);
    }


}
