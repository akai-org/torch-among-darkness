using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterController : CharacterControler
{
    public int id { get; set; }
    public string userName { get; set; }

    // torch
    GameObject torch;
    Transform torchTransform;
    CircleCollider2D torchLightTrigger;
    Coroutine torchDamageCoroutine;
    int torchLightDamage;

    new void Start()
    {
        base.Start();

        torchLightDamage = 1;
        torch = GameObject.Find("Torch");
        torchTransform = torch.GetComponent<Transform>();
        torchLightTrigger = torch.GetComponent<CircleCollider2D>();
    }

    new void Update()
    {
        base.Update();

        movement.x = 0;
        movement.y = 0;
        if (Joystick.GetButton(id, GameControls.ArrowUp))
        {
           movement.y = 1;
        }
        if (Joystick.GetButton(id, GameControls.ArrowDown))
        {
           movement.y = -1;
        }
        if (Joystick.GetButton(id, GameControls.ArrowLeft))
        {
           movement.x = -1;
        }
        if (Joystick.GetButton(id, GameControls.ArrowRight))
        {
           movement.x = 1;
        }

        //movement.x = Joystick.GetAnalogHorizontal(id, AnalogControls.Left);
        //movement.y = Joystick.GetAnalogVertical(id, AnalogControls.Left);

        //movement.x = Input.GetAxis("Horizontal"); //control override for debugging purposes
        //movement.y = Input.GetAxis("Vertical");   //control override for debugging purposes

        if(Input.GetKeyDown(KeyCode.F))
        {
        PickUpTorch();
        Debug.Log("f key pressed");
        }
    }

    // torch
    private void PickUpTorch()
    {   
        if(Vector2.Distance(transform.position, torchTransform.position) > 1f)
        {
            return;
        }
        if(torchTransform.parent == transform){
            torchTransform.parent = null;
            //torchTransform.position = new Vector2(0, 0);
        }else{
            torchTransform.parent = transform;
            torchTransform.localPosition = new Vector2(0.15f, 0.45f);
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
        
        if(torchDamageCoroutine != null)
        {
            StopCoroutine(torchDamageCoroutine);
        }

        torchDamageCoroutine = StartCoroutine(TorchDamage());
    }

    private IEnumerator TorchDamage()
    {
        while(true){
            yield return new WaitForSeconds(1f);
            ChangeHealth(-torchLightDamage);
            torchLightDamage++;
        }
    }

    // health
    override protected void Death()
    {
        isDead = true;
        //Destroy(gameObject);
        // override in child class
    }
    public void Respawn()
    {
        isDead = false;
        currentHealth = maxHealth;
        SetInvincible(3f); //invincibility frames
    }
}
