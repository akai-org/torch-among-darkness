using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacterController : CharacterControler
{
    protected Animator animator;

    //Attack
    [SerializeField] protected int attackDamage = 1;
    [SerializeField] protected float attackSpeed = 2;
    [SerializeField] protected float attackRange = 3;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }
}
