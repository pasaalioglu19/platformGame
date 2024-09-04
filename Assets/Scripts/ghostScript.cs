using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ghostScript : MonoBehaviour
{
    private SpriteRenderer rend;
    private PolygonCollider2D polygonCollider;

    private static float ghostSpeed = 3.25F;
    private bool goLeft = true;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        polygonCollider = GetComponent<PolygonCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (goLeft)
        {
            transform.position = transform.position + (Vector3.left * ghostSpeed) * Time.deltaTime;
        }
        else
        {
            transform.position = transform.position + (Vector3.right * ghostSpeed) * Time.deltaTime;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Left End"))
        {
            goLeft = false;
            rend.flipX = true;
            AdjustCollider();
        }
        else if (collision.gameObject.CompareTag("Right End"))
        {
            goLeft = true;
            rend.flipX = false;
            AdjustCollider();
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
    public static void IncreaseGlobalSpeed(float increaseSpeed)
    {
        if (increaseSpeed == 0)
        {
            ghostSpeed = 3.25F;
        }
        else
        {
            ghostSpeed *= increaseSpeed;
        }
    }
}
