using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChipArea : MonoBehaviour {
    [Header("Prefabs")]
    public Image fullChip;
    
    [Header("Lists")]
    public List<Chip> chips = new List<Chip>();
    public List<Image> chipsImages = new List<Image>();

    //Kinda expensive because I'm lazy
    public void updateChips() {
        if(chips.Count >= 9) {
            fullChip.gameObject.SetActive(true);
            
            return;
        }
        
        for(int j = 0; j < chipsImages.Count; j++) {
            chipsImages[j].gameObject.SetActive(false);
        }
        
        for(int i = 0; i < chips.Count; i++) {
            for(int j = 0; j < chipsImages.Count; j++) {
                int index = int.Parse(chipsImages[j].name.Split(' ')[1]);

                if (chips[i].number == index) {
                    chipsImages[j].gameObject.SetActive(true);
                }
            }
        }
    }
}
