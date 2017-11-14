﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character {

    private IEnemyState currentState;

    public GameObject Target { get; set; }

    [SerializeField]
    private float meleeRange;
    [SerializeField]
    private float throwRange;



    public bool InMeleeRange
    {
        get
        {
            if (Target != null)
            {
                return Vector2.Distance(transform.position, Target.transform.position) <= meleeRange;
            }
            else
            {
                return false;
            }
        }
    }
    public bool InThrowRange
    {
        get
        {
            if (Target != null)
            {
                return Vector2.Distance(transform.position, Target.transform.position) <= throwRange;
            }
            else
            {
                return false;
            }
        }
    }

    public override bool IsDead
    {
        get
        {
            return health <= 0;
        }
    }



    private Vector2 startPos;


    // Use this for initialization
    public override void Start ()
    {
        base.Start();
        GetComponent<Rigidbody2D>().isKinematic = true;
        player.Instance.Dead += new DeadEventHandler(RemoveTarget);

        ChangeState(new IdleState());
        startPos = transform.position;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!IsDead)
        {
            if (!TakingDamage)
            {
                currentState.Execute();
            }          
            LookAtTarget();
        }

        
	}

    public void RemoveTarget()
    {
        Target = null;
        ChangeState(new PatrolState());
    }
    

    private void LookAtTarget()
    {
        if(Target != null)
        {
            // checks to see if target is on the left or right
            float xDir = Target.transform.position.x - transform.position.x;

            if (xDir <0 && facingRight || xDir > 0 && !facingRight)
            {
                //changes direction
                ChangeDirection();
            }
        }
    }

    public void ChangeState(IEnemyState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
            
        }
        currentState = newState;
        currentState.Enter(this);
    }

    public void Move()
    {
        if (!Attack)
        {
            MyAnimator.SetFloat("speed", 1);

            transform.Translate(GetDirection() * (movementSpeed * Time.deltaTime));
        }
    }
    public Vector2 GetDirection()
    {
        return facingRight ? Vector2.right : Vector2.left;
    }
     public override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        currentState.OnTriggerEnter(other);   
    }

    public override IEnumerator TakeDamage()
    {
        
        health -= 10;

        if (!IsDead)
        {
            MyAnimator.SetTrigger("damage");
        }
        else
        {
            MyAnimator.SetTrigger("die");
        }

        yield return null;
    }

    public override void Death()
    {

        MyAnimator.ResetTrigger("die");
        MyAnimator.SetTrigger("idle");
        health = 20;
        transform.position = startPos;
    }
}
