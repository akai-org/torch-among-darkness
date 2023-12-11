using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rigidbody2d;
    public int id { get; set; }
    public string userName { get; set; }

    // torch
    public GameObject torch;
    Transform torchTransform;
    CircleCollider2D torchLightTrigger;
    private Coroutine torchDamageCoroutine;
    int torchLightDamage = 1;

    // movement
    [SerializeField]
    float speed;
    Vector2 movement;

    // health
    public int maxHealth;
    public int health { get { return currentHealth; }}
    int currentHealth;
    bool isInvincible;

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        torch = GameObject.Find("Torch");
        torchTransform = torch.GetComponent<Transform>();
        torchLightTrigger = torch.GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        movement.x = Joystick.GetAnalogHorizontal(id, AnalogControls.Left);
        movement.y = Joystick.GetAnalogVertical(id, AnalogControls.Left);
        movement.x = Input.GetAxis("Horizontal"); //control override for debugging purposes
        movement.y = Input.GetAxis("Vertical");   //control override for debugging purposes
        movement.Normalize();

        if(Input.GetKeyDown(KeyCode.F))
        {
        PickUpTorch();
        }
    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * movement.x * Time.deltaTime;
        position.y = position.y + speed * movement.y * Time.deltaTime;
        rigidbody2d.MovePosition(position);
    }

    // torch
    void PickUpTorch()
    {   
        if(Vector2.Distance(transform.position, torchTransform.position) > 1f)
        {
            return;
        }
        if(torchTransform.parent == transform){
            torchTransform.parent = null;
            torchTransform.position = new Vector2(0, 0);
        }else{
            torchTransform.parent = transform;
            torchTransform.localPosition = new Vector2(0.1f, 0.1f);
        }
    }

    private void OnTriggerEnter2D(Collider2D torchLightTrigger)
    {
        torchLightDamage = 1;
        if(torchDamageCoroutine != null)
        {
            StopCoroutine(torchDamageCoroutine);
        }
    }

    private void OnTriggerExit2D(Collider2D torchLightTrigger) 
    {
        torchDamageCoroutine = StartCoroutine(TorchDamage());
    }

    IEnumerator TorchDamage()
    {
        while(true){
            yield return new WaitForSeconds(1f);
            ChangeHealth(-torchLightDamage);
            torchLightDamage++;
        }
    }


    // health
    public void ChangeHealth(int amount)
    {
        if(amount < 0){
            if(isInvincible)
            {
                return;
            }
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        print(currentHealth + "/" + maxHealth);
    }
}
