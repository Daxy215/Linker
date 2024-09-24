using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {
    [Header("Door Settings")]
    public string code;

    [Header("Door Status")]
    public bool isOpened;

    [Header("Sprites")]
    public Sprite doorOpened;
    public Sprite doorClosed;

    private SpriteRenderer sr;
    private BoxCollider2D bc;

    private void Start() {
        sr = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider2D>();

        updateDoorSprite();
    }

    void Update() {
        if (isOpened)
            bc.isTrigger = true;
        else
            bc.isTrigger = false;
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if(collision.transform.tag.Equals("Player")) {
            PlayerManager pm = collision.gameObject.GetComponent<PlayerManager>();

            if(pm.hasKey) {
                if(pm.items[0].tag.Equals("Key")) {
                    useKey(pm, 0);
                } else if(pm.isFull && pm.items[1].tag.Equals("Key")) {
                    useKey(pm, 1);
                } else {
                    Debug.Log("No key..");
                }
            }
        }    
    }

    public void useKey(PlayerManager pm, int index) {
        Key key = pm.items[index].GetComponent<Key>();

        if (key.code.Equals(code)) {
            isOpened = true;
            pm.removeItem(index);
            pm.hasKey = false;

            updateDoorSprite();
        } else {
            Debug.Log("Different codes...");
        }
    }

    public void updateDoorSprite() {
        if (isOpened)
            sr.sprite = doorOpened;
        else
            sr.sprite = doorClosed;
    }
}
