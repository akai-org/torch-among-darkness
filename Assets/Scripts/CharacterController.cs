using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControler : MonoBehaviour
{
    protected Rigidbody2D rigidbody2d;

    // movement
    [SerializeField]
    protected float speed;
    protected Vector2 movement;

    // health
    [SerializeField]
    protected int maxHealth;
    int health { get { return currentHealth; }}
    protected int currentHealth;
    protected bool isInvincible;
    protected Coroutine invincibleCoroutine;
    protected bool isDead;

    // particle
    protected ParticleSystem particleSystemBlood;

    // Start is called before the first frame update
    protected void Start()
    {
        Transform bloodChild = GetComponentInChildren<Transform>().Find("Blood");
        particleSystemBlood = bloodChild.GetComponent<ParticleSystem>();
        
        rigidbody2d = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    protected void Update()
    {
        if(isDead)
        {
            return;
        }        
    }

    // Update
    protected void FixedUpdate()
    {
        movement.Normalize();

        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * movement.x * Time.deltaTime;
        position.y = position.y + speed * movement.y * Time.deltaTime;
        rigidbody2d.MovePosition(position);
    }

    // health
    public void ChangeHealth(int amount, int giveInvincibility = 0)
    {
        if(amount < 0){
            particleSystemBlood.Emit(50);
            if(isInvincible)
            {
                return;
            }
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        if(currentHealth <= 0)
        {
            Death();
        }
        SetInvincible(giveInvincibility);
        print(gameObject.tag + currentHealth + "/" + maxHealth);
    }

    virtual protected void Death()
    {
        particleSystemBlood.Emit(500);
        isDead = true;
        Destroy(gameObject, 1f);
        // override in child class
    }

    public void SetInvincible(float time = 1f)
    {
        if (invincibleCoroutine != null)
        {
            StopCoroutine(invincibleCoroutine);
        }
        invincibleCoroutine = StartCoroutine(InvincibleCoroutine(time));
    }
    protected IEnumerator InvincibleCoroutine(float time = 1f)
    {
        isInvincible = true;
        yield return new WaitForSeconds(time);
        isInvincible = false;
    }
}
