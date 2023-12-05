using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerHub : MonoBehaviour
{
    public float speed = 5f;
    public Rigidbody2D body;
    public Vector2 movement;
    public Animator animator;

    void Update(){
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        }

    void FixedUpdate(){
        body.MovePosition(body.position + movement * speed * Time.fixedDeltaTime);
    }
}
