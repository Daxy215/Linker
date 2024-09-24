using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour {
    [Header("Slot Settings")]
    public int number;
    
    public Chip chip;

    [Header("Slot Status")]
    public bool hasItem;

    public void addChip(Chip chip) {
        if(hasItem)
            return;

        this.chip = chip;
        this.hasItem = chip != null;

        transform.GetChild(0).gameObject.SetActive(hasItem);
    }

    public void removeChip() {
        this.chip = null;
        this.hasItem = false;

        transform.GetChild(0).gameObject.SetActive(hasItem);
    }
}
