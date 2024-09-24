using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Chip : MonoBehaviour {
    [Header("Chip Settings")]
    public int number;
    public float memory;
    public float addedMemory;

    void Start() {
        PlayerManager pm = FindObjectOfType<PlayerManager>();

        for(int i = 0; i < pm.chips.Count; i++) { 
            if(pm.chips[i].number == number) {
                Destroy(gameObject);
            }
        }
    }
}
