using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour {
    [Header("Scripts")]
    public PlayerManager playerManager;

    void OnTriggerEnter2D(Collider2D collision) {
        if (!GetComponent<Animator>().GetBool("IsAttacking"))
            return;

        if (collision.transform.tag.Equals("Enemy")) {
            Destroy(collision.gameObject);
        }
    }
}
