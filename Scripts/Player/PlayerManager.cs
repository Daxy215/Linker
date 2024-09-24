using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//TODO: SLOW DOWN TIME. CHIP 7
public class PlayerManager : MonoBehaviour {
    [Header("Player Setttings")]
    public float movementSpeed = 0.05f;
    public float jumpPower = 0.5f;
    public float flyingSpeed = 0.05f;
    public float maxFlyingTime;
    public float maxSlowTime;

    [Header("Player Status")]
    public float health = 3.0f;

    public float flyingTime;
    public float currentSlowTime;

    public bool isGrounded;
    public bool canDoubleJump;
    public bool isCrouching;
    public bool isLayingDown;
    public bool isFlying;
    public bool isTimeSlowed;

    [Header("Memory Settings")]
    public float maxMemory = 2000;

    [Header("Memory Status")]
    public float currentMemory;

    [Header("Items Status")]
    public bool isFull;

    public GameObject pipe;

    [Header("Key Status")]
    public bool hasKey;

    [Header("Particles")]
    public GameObject fireParticle;

    [Header("Scripts")]
    public Inventory inventory;
    public Animator animator;

    [Header("Arrays")]
    public GameObject[] items = new GameObject[2];

    [Header("Lists")]
    public List<Chip> chips = new List<Chip>();

    private Rigidbody2D rb;
    private BoxCollider2D bc;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();

        if (inventory == null)
            Debug.LogError("Error: Couldn't find {Inventory} script on the {PlayerManager}");

        if (fireParticle == null)
            Debug.LogError("Error: Couldn't find {FireParticle} inside the {PlayerManager}");

        StartCoroutine(timers());
    }

    void LateUpdate() {
        isFull = items.Length == 1;
        pipe.SetActive(PlayerPrefs.GetInt("hasPipe") == 1);

        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, 0) * movementSpeed;

        isGrounded = Physics2D.OverlapArea(new Vector2(transform.position.x, transform.position.y - (0.3f + bc.bounds.extents.y)), new Vector2(transform.position.x, transform.position.y - (0.31f + bc.bounds.extents.y)));

        /*if(Input.GetKeyDown(KeyCode.R)) {
            PlayerPrefs.DeleteAll();
        }*/

        //Resting variables
        if(isGrounded) {
            flyingTime = 0;
        }

        if (!canDoubleJump) {
            canDoubleJump = isGrounded;
        }

        animator.SetFloat("Speed", Mathf.Abs(movement.x));
        animator.SetBool("IsJumping", !isGrounded);
        animator.SetBool("IsCrouching", isCrouching);
        animator.SetBool("IsLaying", isLayingDown);
        animator.SetBool("IsFlying", isFlying);

        //Movement
        if(movement != Vector3.zero && hasChip(0)) {
            transform.rotation = movement.x > 0 ? Quaternion.identity : Quaternion.Euler(0, 180, 0); 
            
            transform.position += movement * Time.deltaTime;
        }

        #region Player Mechanics
        
        //Crouching
        if(hasChip(5)) {
            if (Input.GetKeyDown(ControllerManager.chrouch) && !isLayingDown && isGrounded) {
                isCrouching = !isCrouching;

                if(isCrouching) {
                    movementSpeed /= 2;
                } else if(!isCrouching) {
                    movementSpeed *= 2;
                }
            }
        }

        //Laying down
        if(hasChip(2)) {
            if(Input.GetKeyDown(ControllerManager.layDown) && !isCrouching && isGrounded) {
                isLayingDown = !isLayingDown;

                if(isLayingDown) {
                    movementSpeed /= 3;
                } else if(!isLayingDown) {
                    movementSpeed *= 3;
                }
            }
        }

        //Jumping and double jumping
        if(Input.GetKeyDown(ControllerManager.jump) && !isCrouching && !isLayingDown) {
            if(isGrounded && hasChip(1)) {
                rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);

                AudioManager.instance.Play("Player_Jump");
            } else if(canDoubleJump && hasChip(6)) {
                canDoubleJump = false;

                rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);

                AudioManager.instance.Play("Player_Jump");
            }
        }

        //Flying
        if(hasChip(4)) {
            if(Input.GetKeyDown(ControllerManager.fly)) {
                isFlying = !isFlying;
            }
        }

        if(hasChip(3) && PlayerPrefs.GetInt("hasPipe") == 1 && Input.GetMouseButtonDown(0)) {
            attack();
        }
        
        if(PlayerPrefs.GetInt("hasPipe") == 1 && hasChip(3) && Input.GetMouseButtonUp(0)) {
            pipe.GetComponent<Animator>().SetBool("IsAttacking", false);
        }

        if(Input.GetKeyDown(ControllerManager.slowTime) && hasChip(7)) {
            isTimeSlowed = !isTimeSlowed;
        }
       
        #endregion

        fireParticle.SetActive(isFlying);

        if (isFlying) {
            if(flyingTime < maxFlyingTime) {
                transform.position += new Vector3(0, flyingSpeed, 0);
            } else {
                isFlying = false;
            }
        }

        if(isTimeSlowed) {
            if(currentSlowTime >= maxSlowTime) {
                currentSlowTime = 0;
                isTimeSlowed = false;

                Time.timeScale = 1;
            } else {
                Time.timeScale = 0.05f;
            }
        } else if(!isTimeSlowed && currentSlowTime != 0) {
            currentSlowTime = 0;
         }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if(!collision.enabled)
            return;

        if(collision.transform.tag.Equals("Enemy")) {
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }

        if(collision.transform.tag.Equals("Chip")) {
            Chip chip = collision.transform.GetComponent<Chip>();
            inventory.addItem(chip);

            collision.gameObject.SetActive(false);

            maxMemory += chip.addedMemory;

            AudioManager.instance.Play("Player_PickUpChip");
        }

        if(collision.transform.tag.Equals("Pipe")) {
            PlayerPrefs.SetInt("hasPipe", 1);

            Destroy(collision.gameObject);
        }

        if(collision.transform.tag.Equals("Key")) {
            addItem(collision.gameObject);
            collision.gameObject.SetActive(false);

            hasKey = true;
        }

        if(collision.transform.tag.Equals("NextLevel")) {
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());

            PlayerPrefs.SetInt("CurrentLevelIndex", PlayerPrefs.GetInt("CurrentLevelIndex") + 1);

            if (PlayerPrefs.GetInt("CurrentLevelIndex") == 0)
                PlayerPrefs.SetInt("CurrentLevelIndex", 1);

            if (PlayerPrefs.GetInt("CurrentLevelIndex") >= 10)
                PlayerPrefs.SetInt("CurrentLevelIndex", 1);

            SceneManager.LoadScene("Level" + PlayerPrefs.GetInt("CurrentLevelIndex"));
        }

        if(collision.transform.tag.Equals("Back")) {
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());

            PlayerPrefs.SetInt("CurrentLevelIndex", PlayerPrefs.GetInt("CurrentLevelIndex") - 1);

            if (PlayerPrefs.GetInt("CurrentLevelIndex") <= 0)
                PlayerPrefs.SetInt("CurrentLevelIndex", 9);

            SceneManager.LoadScene("Level" + PlayerPrefs.GetInt("CurrentLevelIndex"));
        }
    }

    public void attack() {
        pipe.GetComponent<Animator>().SetBool("IsAttacking", true);
    }

    public void damage() {
        health--;

        AudioManager.instance.Play("Player_Damaged");

        if(health <= 0) {
            die();
        }
    }

    public void die() {
        AudioManager.instance.Play("Player_Dead");

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public bool hasChip(int number) { 
        for(int i = 0; i < chips.Count; i++) { 
            if(chips[i].number == number) {
                return true;
            }
        }

        return false;
    }

    public bool addChipToMemory(Chip chip) {
        if (hasChip(chip.number))
            return false;

        if (currentMemory + chip.memory <= maxMemory) {
            currentMemory += chip.memory;

            chips.Add(chip);

            PlayerPrefs.SetString("CollectedChips:", PlayerPrefs.GetString("CollectedChips:") + " " + chip.number);
            
            if(chip.number == 8) {
                maxMemory += chip.addedMemory;
            }

            return true;
        } else {
            return false;
        }
    }

    public void addItem(GameObject go) {
        if (items[0] == null)
            items[0] = go;
        else
            items[1] = go;
    }

    public void removeItem(int i) {
        int index = Mathf.Clamp(i, 0, 1);
        
        if(index == 0) {
            items[0] = items[1];
            items[1] = null;
        } else {
            items[1] = items[0];
            items[0] = null;
        }
    }

    IEnumerator timers() {
        yield return new WaitForSeconds(0.1f);

        if(isFlying)
            flyingTime += 0.1f;

        if (isTimeSlowed)
            currentSlowTime += 0.1f;

        StartCoroutine(timers());
    }
}
