using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [Header("Scripts")]
    public ChipArea chipArea;

    [Header("Arrays")]
    public Chip[] allChips;

    public static GameManager instance;

    void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        PlayerManager pm = FindObjectOfType<PlayerManager>();

        if(PlayerPrefs.GetString("CollectedChips:").Equals("")) {
            Debug.Log("New save..");
        } else {
            string[] chipsNumbers = PlayerPrefs.GetString("CollectedChips:").Split(' ');

            for(int i = 1; i < chipsNumbers.Length; i++) {
                int chipNum = int.Parse(chipsNumbers[i]);

                for(int j = 0; j < allChips.Length; j++) { 
                    if(allChips[j].number == chipNum) {
                        pm.addChipToMemory(allChips[j]);
                        chipArea.chips.Add(allChips[j]);
                    }
                }
            }

            chipArea.updateChips();
        }
    }
}
