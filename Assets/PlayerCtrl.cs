using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    public Rigidbody rb;
    private FixedJoystick joystick;

    public float _movespeed;

    void Start()
    {
        joystick = FindObjectOfType<FixedJoystick>();
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector3 v = new Vector3(joystick.Horizontal * _movespeed, rb.velocity.y, joystick.Vertical * _movespeed);
        rb.velocity = transform.TransformDirection(v);
    }
}
