using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void DeadEventHandler();

public class player : Character
{

    private static player instance;

    // Is an Event when trigger will show the player is dead to the AI
    public event DeadEventHandler Dead;

    public static player Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<player>();
            }
            
            return instance;
        }

        
    }      

    [SerializeField]
    private Transform[] groundPoints;


    [SerializeField]
    private bool airControl;

    [SerializeField]
    private float jumpForce;

    [SerializeField]
    private float groundRadius;

    private bool immortal = false;

    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private float immortalTime;

    [SerializeField]
    private LayerMask whatIsGround;
       
    public Rigidbody2D MyRigidbody { get; set; }    

    public bool Slide { get; set; }

    public bool Jump { get; set; }

    public bool OnGround { get; set; }

    public override bool IsDead
    {
        get
        {
            if (health <= 0)
            {
                OnDead();
            }
            
            return health <= 0;
        }
    }

    private Vector2 startPos;

    
  
    // Use this for initialization
     public override void Start()
    {
        base.Start();

        spriteRenderer = GetComponent<SpriteRenderer>();
        MyRigidbody = GetComponent<Rigidbody2D>();
        
        startPos = transform.position;
    }

    void Update()
    {
        if (!TakingDamage && !IsDead)
        {
            if (transform.position.y <= -10f)
            {
                Death();
            }
            HandleInput();
        }
        
    }

    // Update is called once per frame
    // Fixed at 50 FPS
    void FixedUpdate()
    {
        if (!TakingDamage && !IsDead)
        {
            float horizontal = Input.GetAxis("Horizontal");
            HandleMovement(horizontal);

            OnGround = IsGrounded();

            HandleInput();

            Flip(horizontal);

            HandleLayers();
        }
        

    }

    public void OnDead()
    {
        if (Dead != null)
        {
            Dead();
        }
    }


    private void HandleMovement(float horizontal)
    {
        if(MyRigidbody.velocity.y < 0)
        {
            MyAnimator.SetBool("land", true);
        }
        if(!Attack && !Slide && (OnGround || airControl))
        {
            MyRigidbody.velocity = new Vector2(horizontal * movementSpeed, MyRigidbody.velocity.y);
        }
        if(Jump && MyRigidbody.velocity.y == 0)
        {
            MyRigidbody.AddForce(new Vector2(0, jumpForce));
        }
        MyAnimator.SetFloat("speed", Mathf.Abs(horizontal));
    }
    // makes sprite attack


    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MyAnimator.SetTrigger("jump");
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            MyAnimator.SetTrigger("attack");
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            MyAnimator.SetTrigger("slide");
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            MyAnimator.SetTrigger("throw");            
        }
    }


    private void Flip(float horizontal)
    {
        // Saying if the player is facing right or left
        if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight)
        {
            ChangeDirection();
        }
    }

    private bool IsGrounded()
    {
        if (MyRigidbody.velocity.y <= 0)
        {
            foreach (Transform point in groundPoints)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(point.position, groundRadius, whatIsGround);

                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].gameObject != gameObject)
                    {

                        return true;
                    }
                }

            }
        }
        return false;
    }
    private void HandleLayers()
    {
        if (!OnGround)
        {
            // sets the layer to one
            MyAnimator.SetLayerWeight(1, 1);

        }
        else
        {
            
            MyAnimator.SetLayerWeight(1, 0);
        }
    }
    public override void ThrowKnife(int value)
    {
        if(!OnGround && value == 1 || OnGround && value == 0)
        {
            base.ThrowKnife(value);
        }
    }

    private IEnumerator IndicateImmortal()
    {
        while (immortal)
        {
            // will run as long as we are immortal
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(.1f);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(.1f);
            
        }
    }

    public override IEnumerator TakeDamage()
    {
        
        if (!immortal)
        {
            
            health -= 10;
            if (!IsDead)
            {
                MyAnimator.SetTrigger("damage");
                immortal = true;
                StartCoroutine(IndicateImmortal());
                yield return new WaitForSeconds(immortalTime);

                immortal = false;

            }
            else
            {
                MyAnimator.SetLayerWeight(1, 0);
                MyAnimator.SetTrigger("die");
            }
        }
        
    }

    public override void Death()
    {
        MyRigidbody.velocity = Vector2.zero;
        MyAnimator.SetTrigger("idle");
        health = 40;
        transform.position = startPos;
    }
}
