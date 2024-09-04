using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class characterScript : MonoBehaviour
{
    public babyGodzillaScript babyGodzilla;
    public GameObject currentSword;
    public LogicScript logic;
    public cameraScript cameraObj;
    public healthScript healObj;
    public Rigidbody2D myRigidbody; //why can not be private? It gave error
    public PolygonCollider2D[] myCollider;
    private SpriteRenderer[] childRenderers;
    public AudioSource noVoice;
    public AudioSource teleport;
    public AudioSource jump;

    private Animator anim;

    private Vector3 spawnpoint0 = new Vector3(87.75F, -20.5F, 0);

    private float moveSpeed = 2.5F;
    private float jumpForce = 10.0F;
    private int targetLocation = 4;
    private bool charIsAlive = true;
    private bool isGrounded = true;
    private bool canTakeDamage = true;
    private bool charCanMove = true;
    private Vector2 movement;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
        childRenderers = transform.GetComponentsInChildren<SpriteRenderer>();
        myCollider = transform.GetComponentsInChildren<PolygonCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (charIsAlive)
        {
            if (charCanMove)
            {
                if (Input.GetKey(KeyCode.F) && anim.GetBool("isSword"))
                {
                    StartCoroutine(ThrowSwordCoroutine());
                }
                HandleMovement();
                HandleJump();
            }
        }
        else
        {
            HandleDeath();
        }
    }

    void HandleMovement()
    {
        movement = Vector2.zero;
        anim.SetBool("isRunning", false);

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            if (!childRenderers[0].flipX)
            {
                FlipCharacter(true);
            }
            movement += Vector2.left;
            if(isGrounded)
                anim.SetBool("isRunning", true);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            if (childRenderers[0].flipX)
            {
                FlipCharacter(false);
            }
            movement += Vector2.right;
            if (isGrounded)
                anim.SetBool("isRunning", true);
        }

        myRigidbody.velocity = new Vector2(movement.x * moveSpeed, myRigidbody.velocity.y);
    }

    void HandleJump()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow)) && isGrounded)
        {
            jump.Play();
            myRigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
        }
    }

    void HandleDeath()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;
    }

    void FlipCharacter(bool flip)
    {
        childRenderers[0].flipX = !childRenderers[0].flipX;
        if (flip)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
            transform.position = transform.position + new Vector3(0.367F, 0, 0);
            cameraObj.setFixCamera(false);

        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            transform.position = transform.position - new Vector3(0.367F, 0, 0);
            cameraObj.setFixCamera(true);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (charIsAlive)
        {
            if (collision.gameObject.CompareTag("Monster") && canTakeDamage)
            {
                foreach (SpriteRenderer renderer in childRenderers)
                {
                    renderer.color = Color.red;
                }
                if (healObj.getHeal() == 1) // character is dead
                {
                    StartCoroutine(DeadCoroutine());
                }
                else
                {
                    StartCoroutine(InvulnerabilityCoroutine());
                }
                healObj.takeDamage();
            }

            else if (collision.gameObject.CompareTag("Monster Speed Up"))
            {
                Destroy(collision.gameObject);
                ghostScript.IncreaseGlobalSpeed(1.18f); 
            }

            else if (collision.gameObject.CompareTag("Fight Scene"))
            {

                Vector3 newPosition = spawnpoint0 + new Vector3(6.7F, 3.3F, -10);
                cameraObj.setMiddleCamera(newPosition);
                Destroy(collision.gameObject);
                StartCoroutine(FightSceneCoroutine());
            }
            else if (collision.gameObject.CompareTag("Restart"))
            {
                logic.gameOver();
            }
            else if (collision.gameObject.CompareTag("Big Monster"))
            {
                foreach (SpriteRenderer renderer in childRenderers)
                {
                    renderer.color = Color.red;
                }
                StartCoroutine(DeadCoroutine());
                healObj.takeDamage();
                healObj.takeDamage();
                healObj.takeDamage();
            }
            else if (collision.gameObject.CompareTag("SwordObj"))
            {
                anim.SetBool("isSword", true);
                Destroy(collision.gameObject);
            }
            else if (collision.gameObject.CompareTag("TargetLoc0"))
            {
                targetLocation = 4;
            }
            else if (collision.gameObject.CompareTag("TargetLoc1"))
            {
                targetLocation = 3;
            }
            else if (collision.gameObject.CompareTag("TargetLoc2"))
            {
                targetLocation = 2;
            }
            else if (collision.gameObject.CompareTag("TargetLoc3"))
            {
                targetLocation = 1;
            }
        }
    }

    public int getTargetLocation()
    {
        return targetLocation;
    }
    public IEnumerator knifeStabCoroutine()
    {
        anim.SetBool("isThrowSword", false);  
        anim.enabled = false;
        yield return new WaitForSeconds(1F);
        Rigidbody2D x = currentSword.GetComponent<Rigidbody2D>();
        x.gravityScale = 1;
        currentSword.transform.parent = null;
        yield return new WaitForSeconds(0.8F);
        anim.enabled = true;
    }

    private IEnumerator FightSceneCoroutine()
    {
        teleport.Play();
        transform.position = spawnpoint0;
        charCanMove = false;
        myRigidbody.velocity = Vector2.zero;
        anim.SetBool("isRunning", false);
        logic.fightAudio();
        yield return new WaitForSeconds(4f);
        babyGodzilla.setCanMove();
        charCanMove = true;
    }

    private IEnumerator ThrowSwordCoroutine()
    {
        anim.SetBool("isThrowSword", true);
        charCanMove = false;
        anim.SetBool("isSword", false);
        yield return new WaitForSeconds(2f);
        anim.SetBool("isThrowSword", false);
        charCanMove = true;
    }

    private IEnumerator InvulnerabilityCoroutine()
    {
        canTakeDamage = false;

        for (int i = 0; i < 7; i++)
        {
            yield return new WaitForSeconds(0.25f);
            foreach (SpriteRenderer renderer in childRenderers)
            {
                renderer.color = i % 2 == 0 ? Color.white : Color.red;
            }
        }

        yield return new WaitForSeconds(0.25F);
        canTakeDamage = true;
    }

    private IEnumerator DeadCoroutine()
    {
        noVoice.Play();
        cameraObj.setTarget();
        charIsAlive = false;
        myRigidbody.gravityScale = 0;
        yield return new WaitForSeconds(0.35F);
        foreach (PolygonCollider2D colliderer in myCollider)
        {
            colliderer.enabled = false;
        }
        myRigidbody.gravityScale = 1;
        yield return new WaitForSeconds(1.5F);
        Destroy(gameObject);
    }
}
