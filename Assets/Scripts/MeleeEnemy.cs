using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityHFSM;  // Import UnityHFSM


public class MeleeEnemy : EnemyCharacterController
{
    // Declare the finite state machine
    private StateMachine fsm;

    // Parameters (can be changed in the inspector)
    [SerializeField] private float searchSpotRange = 10;
    [SerializeField] private float searchTime = 2;  // in seconds++
    [SerializeField] private Vector2[] patrolPoints;

    // Internal fields
    private Text stateDisplayText;
    private int patrolDirection = 1;
    [SerializeField] private PlayerCharacterController charController;
    private Vector2 lastSeenPlayerPosition;

    // Helper methods (depend on how your scene has been set up)
    private Vector2 playerPosition => GameObject.FindGameObjectsWithTag("Player")[0].transform.position;
    private float distanceToPlayer => Vector2.Distance(playerPosition, transform.position);

    new void Start()
    {
        base.Start();

        animator = GetComponent<Animator>();
        stateDisplayText = GetComponentInChildren<Text>();

        fsm = new StateMachine();

        // Fight FSM
        var fightFsm = new HybridStateMachine(
            beforeOnLogic: state => MoveTowards(playerPosition, attackSpeed, minDistance: 1),
            needsExitTime: true
        );

        fightFsm.AddState("Wait");
        fightFsm.AddState("Telegraph");
        fightFsm.AddState("Hit",
            onEnter: state => {
                charController.ChangeHealth(-attackDamage);
                // TODO: Cause damage to player if in range.
            }
        );

        // Because the exit transition should have the highest precedence,
        // it is added before the other transitions.
        fightFsm.AddExitTransition("Wait");

        fightFsm.AddTransition(new TransitionAfter("Wait", "Telegraph", 0.5f));
        fightFsm.AddTransition(new TransitionAfter("Telegraph", "Hit", 0.42f));
        fightFsm.AddTransition(new TransitionAfter("Hit", "Wait", 0.5f));

        // Root FSM
        fsm.AddState("Patrol", new CoState(this, Patrol, loop: false));
        fsm.AddState("Chase", new State(
            onLogic: state => {
            MoveTowards(playerPosition, speed);
            //Debug.Log("test");
            }
        ));
        fsm.AddState("Fight", fightFsm);
        fsm.AddState("Search", new CoState(this, Search, loop: false));

        fsm.SetStartState("Patrol");

        fsm.AddTriggerTransition("PlayerSpotted", "Patrol", "Chase");
        fsm.AddTwoWayTransition("Chase", "Fight", t => distanceToPlayer <= attackRange);
        fsm.AddTransition("Chase", "Search",
            t => distanceToPlayer > searchSpotRange,
            onTransition: t => lastSeenPlayerPosition = playerPosition);
        fsm.AddTransition("Search", "Chase", t => distanceToPlayer <= searchSpotRange);
        fsm.AddTransition(new TransitionAfter("Search", "Patrol", searchTime));

        fsm.Init();
    }

    new void Update()
    {
        if(isDead)
        {
            return;
        }
        fsm.OnLogic();
        //stateDisplayText.text = fsm.GetActiveHierarchyPath();
        //Debug.Log(gameObject.transform.position);
    }

    // Triggers the `PlayerSpotted` event.
    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player"))
        {
            charController = other.gameObject.GetComponent<PlayerCharacterController>() as PlayerCharacterController;
            fsm.Trigger("PlayerSpotted");
        }
    }

    private void MoveTowards(Vector2 target, float speed, float minDistance=0)
    {
        //Debug.Log(target);
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            Mathf.Max(0, Mathf.Min(speed * Time.deltaTime, Vector2.Distance(transform.position, target) - minDistance))
        );
    }

    private IEnumerator MoveToPosition(Vector2 target, float speed, float tolerance=0.05f)
    {
        while (Vector2.Distance(transform.position, target) > tolerance)
        {
            MoveTowards(target, speed);
            // Wait one frame.
            yield return null;
        }
    }

    private IEnumerator Patrol()
    {
        int currentPointIndex = FindClosestPatrolPoint();

        while (true)
        {
            yield return MoveToPosition(patrolPoints[currentPointIndex], speed);

            // Wait at each patrol point.
            yield return new WaitForSeconds(3);

            currentPointIndex += patrolDirection;

            // Once the bot reaches the end or the beginning of the patrol path,
            // it reverses the direction.
            if (currentPointIndex >= patrolPoints.Length || currentPointIndex < 0)
            {
                currentPointIndex = Mathf.Clamp(currentPointIndex, 0, patrolPoints.Length-1);
                patrolDirection *= -1;
            }
        }
    }

    private int FindClosestPatrolPoint()
    {
        float minDistance = Vector2.Distance(transform.position, patrolPoints[0]);
        int minIndex = 0;

        for (int i = 1; i < patrolPoints.Length; i ++)
        {
            float distance = Vector2.Distance(transform.position, patrolPoints[i]);
            if (distance < minDistance)
            {
                minDistance = distance;
                minIndex = i;
            }
        }

        return minIndex;
    }

    private IEnumerator Search()
    {
        yield return MoveToPosition(lastSeenPlayerPosition, speed);

        while (true)
        {
            yield return new WaitForSeconds(2);

            yield return MoveToPosition(
                (Vector2)transform.position + Random.insideUnitCircle * 10,
                speed
            );
        }
    }
}

