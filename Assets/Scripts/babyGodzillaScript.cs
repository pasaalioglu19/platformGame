using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class babyGodzillaScript : MonoBehaviour
{
    public characterScript character;
    public LogicScript logic;
    private SpriteRenderer rend;
    private PolygonCollider2D polygonCollider;
    public Rigidbody2D myRigidbody;
    private Sprite godzillaUlty;
    private Sprite originalSprite;
    private static float godzillaSpeed = 1.95F;
    private bool goLeft = true;
    private bool canMove = false;
    private bool isJump = false;
    private Animator anim;
    public AudioSource beamAudio;
    public AudioSource knifeStab;

    private int ulty_count = 0;
    private float timer = 0;
    private float ulty_time = 4F;


    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        originalSprite = rend.sprite;
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
        godzillaUlty = Resources.Load<Sprite>("monsters/godzilla/godzilla_baby_angry");
        polygonCollider = GetComponent<PolygonCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            Vector3 direction = goLeft ? Vector3.left : Vector3.right;
            transform.position += direction * godzillaSpeed * Time.deltaTime;

            if (timer > ulty_time && ulty_count <= 2)
            {
                StartCoroutine(godzillaUltyCoroutine());

            }
            else
            {
                timer += Time.deltaTime;
            }
        }
        else if (isJump)
        {
            transform.position += Vector3.up * godzillaSpeed * Time.deltaTime;
        }   
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canMove)
        {
            if (collision.gameObject.CompareTag("Left End"))
            {
                if (ulty_count == 3)
                {
                    Destroy(collision.gameObject);
                }
                else
                {
                    goLeft = false;
                    rend.flipX = true;
                    AdjustCollider();
                }
            }
            else if (collision.gameObject.CompareTag("Right End"))
            {
                goLeft = true;
                rend.flipX = false;
                AdjustCollider();
            }
        }
        if (collision.gameObject.CompareTag("Sword"))
        {
            canMove = false;
            StartCoroutine(DeadCoroutine());
        }
    }

private void AdjustCollider()
    {
        Vector2[] points = polygonCollider.points;
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = new Vector2(-points[i].x, points[i].y);
        }
        polygonCollider.points = points;
    }
    public void setCanMove()
    {
        canMove = true;
    }

    public static void IncreaseGlobalSpeed(float increaseSpeed)
    {
        if (increaseSpeed == 0)
        {
            godzillaSpeed = 1.95F;
        }
        else
        {
            godzillaSpeed *= increaseSpeed;
        }
    }


    private IEnumerator LastRunCoroutine()
    {
        if(rend.color != Color.red)
        {
            polygonCollider.isTrigger = true;
            goLeft = true;
            godzillaSpeed *= 3;
            yield return new WaitForSeconds(1.7F);
            canMove = false;
        }
    }
    private IEnumerator godzillaUltyCoroutine()
    {
        ulty_count++;
        timer = 0;
        canMove = false;
        isJump = true;
        yield return new WaitForSeconds(0.45F);
        isJump = false;
        myRigidbody.gravityScale = 1.5F;
        yield return new WaitForSeconds(0.30F);
        myRigidbody.gravityScale = 0;
        rend.sprite = godzillaUlty;

        for (int i = 0; i < 10; i++)
        {
            int range = character.getTargetLocation();
            int loc = Random.Range(range*2-1, range*2+1);
            beamAudio.Play();

            anim.SetInteger("isUlty", loc);
            if (loc <= 4)
            {
                yield return new WaitForSeconds(0.4f + (loc - 1) * 0.06f);
            }
            else
            {
                yield return new WaitForSeconds(1.04f + (loc - 5) * 0.06f);
            }
            
            anim.SetInteger("isUlty", 0);

            yield return new WaitForSeconds(0.25F);
            beamAudio.Stop();
        }

        rend.sprite = originalSprite;
        canMove = true;
        if (ulty_count == 3)
        {
            canMove = false;
            logic.swordVisible();
            yield return new WaitForSeconds(3F);
            canMove = true;
            StartCoroutine(LastRunCoroutine());
        }
    }

    private IEnumerator DeadCoroutine()
    {
        StartCoroutine(character.knifeStabCoroutine());
        knifeStab.Play();
        rend.color = Color.red;
        yield return new WaitForSeconds(1F);
        myRigidbody.gravityScale = 1;
        yield return new WaitForSeconds(6F);
        logic.gameWin();
        Destroy(gameObject);
    }
}
