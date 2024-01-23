using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float attackSpeed = 1f;
    [SerializeField] private int attackDamage = 1;
    float attackCooldown = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(attackCooldown < 0f)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                animator.SetTrigger("MeleeAttack");
                attackCooldown = 1f / attackSpeed;
            }
        }else{
            attackCooldown -= Time.deltaTime;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.GetComponent<EnemyCharacterController>() == true)
        {
            other.gameObject.GetComponent<EnemyCharacterController>().ChangeHealth(-attackDamage);
        }
    }
}
