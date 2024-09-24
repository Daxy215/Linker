using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    [Header("Bullet Settings")]
    public float maxTimeAlive = 3;

    [Header("Bullet Status")]
    public float currentTimeAlive;

    void Start() {
        StartCoroutine(aliveTimer());
    }

    void Update() {
        transform.position += transform.right * 0.05f;

        if(currentTimeAlive >= maxTimeAlive) {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if(collision.transform.tag.Equals("Player")) {
            PlayerManager pm = collision.gameObject.GetComponent<PlayerManager>();

            pm.damage();
        }

        Destroy(gameObject);
    }

    IEnumerator aliveTimer() {
        yield return new WaitForSeconds(0.1f);

        currentTimeAlive += 0.1f;

        StartCoroutine(aliveTimer());
    }
}
