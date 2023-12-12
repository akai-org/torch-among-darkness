using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerControllerHub : MonoBehaviour
{
    public float speed = 5f;
    public Rigidbody2D body;
    public Vector2 movement;
    public Animator animator;

    public TMP_Text hpText;

    public int HP = 100;

    public static PlayerControllerHub Instance { get; private set; }

    void Start()
    {
        Instance = this;
    }

    void Update(){
        hpText.text = "HP: "+HP;
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        }

    void FixedUpdate(){
        body.MovePosition(body.position + movement * speed * Time.fixedDeltaTime);
    }
}
