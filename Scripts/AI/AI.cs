using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour {
    public enum CurrentState { 
        idle,
        walking,
        aiming
    }

    [Header("AI Settings")]
    public float movementSpeed = 0.05f;
    public float timeTillNextFire = 0.5f;
    public bool canWalk;

    [Header("AI Status")]
    public float currentTimeTillNextFire;
    public bool walkDirection;

    public CurrentState currentState = CurrentState.idle;

    [Header("Prefabs")]
    public GameObject hand;
    public GameObject bulletSpawnArea;
    public GameObject bullet;

    [Header("Scripts")]
    public Animator animator;

    private PlayerManager pm;

    void Start() {
        StartCoroutine(shootingTimer());

        animator = GetComponent<Animator>();

        if (canWalk)
            currentState = CurrentState.walking;
        else
            currentState = CurrentState.idle;

        Physics2D.queriesStartInColliders = false;
    }

    void Update() {
        hand.SetActive(currentState == CurrentState.aiming);
        
        switch(currentState) {
            case CurrentState.idle:
                animator.SetInteger("currentState", 0);

                break;
            case CurrentState.walking:
                if (!canWalk) {
                    currentState = CurrentState.idle;
                    
                    return;
                }

                animator.SetInteger("currentState", 1);

                if(canWalk) {
                    if (walkDirection)
                        transform.position += transform.right * movementSpeed;
                    else
                        transform.position += transform.right * movementSpeed;
                }

                break;
            case CurrentState.aiming:
                animator.SetInteger("currentState", 2);

                if (pm == null) {
                    currentState = CurrentState.idle;

                    return;
                }

                Vector2 directon = new Vector2(pm.transform.position.x - hand.transform.position.x,
                                               pm.transform.position.y - hand.transform.position.y);

                bulletSpawnArea.transform.right = directon;
                hand.transform.up = -directon;

                if(directon.x < 0) {
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                } else if(directon.x > 0 && transform.rotation.y != 0) {
                    transform.rotation = Quaternion.identity;
                }

                if(currentTimeTillNextFire >= timeTillNextFire) {
                    currentTimeTillNextFire = 0;

                    //Shooting..
                    Instantiate(bullet, bulletSpawnArea.transform.position, bulletSpawnArea.transform.rotation);
                    AudioManager.instance.Play("Enemy_Shoot");
                }

                break;
        }

        if (currentState != CurrentState.aiming) {
            currentTimeTillNextFire = 0;
        }

        #region RayCasting
        RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, transform.right);

        for (int i = 0; i < hit.Length; i++) {
            // If it hits something...
            if (hit[i].collider != null) {
                if(hit[i].collider.tag.Equals("Player")) {
                    float distance = Mathf.Abs(hit[i].point.x - transform.position.x);

                    if(distance <= 8) {
                        currentState = CurrentState.aiming;

                        pm = hit[i].collider.GetComponent<PlayerManager>();
                    }
                }
            }

            if(hit[i].collider == null) {
                if (canWalk)
                    currentState = CurrentState.walking;
                else
                    currentState = CurrentState.idle;
            } else {
                if(hit[i].collider.tag.Equals("Player")) {
                    float distance = Mathf.Abs(hit[i].point.x - transform.position.x);

                    if (distance > 8)
                        if (canWalk)
                            currentState = CurrentState.walking;
                        else
                            currentState = CurrentState.idle;
                }
            }
        }
        #endregion
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(currentState == CurrentState.walking) {
            if(collision.tag.Equals("Trigger")) {
                walkDirection = !walkDirection;
            
                if (!walkDirection)
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                else
                    transform.rotation = Quaternion.identity;
            }
        }
    }

    IEnumerator shootingTimer() {
        yield return new WaitForSeconds(0.1f);

        if(currentState == CurrentState.aiming) {
            currentTimeTillNextFire += 0.1f;
        }

        StartCoroutine(shootingTimer());
    }
}
