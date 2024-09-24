using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour {
    [Header("Platform Settings")]
    public float speed = 0.05f;
    public float maxWaitTime = 0.5f;

    public bool canMove;
    public bool xAxis;
    public bool bothDirections;

    [Header("Platform Status")]
    public float currentWaitTime;

    [Header("Sprites")]
    public Sprite standingSprite;
    public Sprite movingSprite;

    private bool wonderDirection;

    private SpriteRenderer sr;

    void Start() {
        sr = GetComponent<SpriteRenderer>();

        if (!canMove)
            return;

        StartCoroutine(waitTimer());
    }

    void Update() {
        if (!canMove)
            return;

        if(currentWaitTime >= maxWaitTime) {
            sr.sprite = movingSprite;

            if(bothDirections) {
                if (wonderDirection) {
                    transform.position += transform.up * speed;
                    transform.position += transform.right * speed;
                } else {
                    transform.position += -transform.up * speed;
                    transform.position += -transform.right * speed;
                }
            } else if(xAxis) {
                if (wonderDirection)
                    transform.position += transform.right * speed;
                else
                    transform.position += -transform.right * speed;
            } else if(!xAxis) {
                if (wonderDirection)
                    transform.position += transform.up * speed;
                else
                    transform.position += -transform.up * speed;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if(collision.tag.Equals("Trigger")) {
            wonderDirection = !wonderDirection;

            currentWaitTime = 0;

            sr.sprite = standingSprite;
        }
    }

    IEnumerator waitTimer() {
        yield return new WaitForSeconds(0.1f);

        currentWaitTime += 0.1f;

        StartCoroutine(waitTimer());
    }
}
