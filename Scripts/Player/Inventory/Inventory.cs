using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {
    [Header("Inventory Settings")]
    public int sizeX;
    public int sizeY;
    public float spacingX;
    public float spacingY;

    public float startPosX;
    public float startPosY;

    [Header("Inventory Status")]
    public bool isEnabled;

    [Header("Prefabs")]
    public GameObject slot;
    public Image draggedImage;

    [Header("Lists")]
    public List<Slot> slots = new List<Slot>();

    private Chip draggingChip = null;
    private Slot lastDraggingSlot = null;

    #region Unity Functions

    private void Awake() {
        draggingChip = null;

        int index = 0;

        for(int j = sizeY; j > 0; j--) {
            for(int i = 0; i < sizeX; i++) {
                GameObject go = Instantiate(slot, new Vector3(startPosX + i * spacingX, startPosY + j * spacingY, 0), Quaternion.identity, transform.GetChild(0));
                go.name = "Slot" + index;

                slots.Add(go.GetComponent<Slot>());
                slots[index].number = index;

                index++;
            }
        }
    }

    private void Update() {
        if(Input.GetKeyDown(ControllerManager.toggleInventory)) {
            isEnabled = !isEnabled;

            if (isEnabled)
                Time.timeScale = 0;
            else
                Time.timeScale = 1;

            updateSlots();
        }

        draggedImage.gameObject.SetActive(draggingChip != null);

        if(draggingChip != null) {
            draggedImage.transform.position = Input.mousePosition;
        }
    }

    #endregion

    #region Slot Functions
    private void updateSlots() {
        transform.GetChild(0).gameObject.SetActive(isEnabled);

        for (int i = 0; i < slots.Count; i++) {
            slots[i].gameObject.SetActive(isEnabled);
        }
    }
    
    public void addItem(Chip chip) { 
        for(int i = 0; i < slots.Count; i++) { 
            if(!slots[i].hasItem) {
                slots[i].addChip(chip);

                return;
            }
        }
    }

    public void addItemTo(int index, Chip chip) {
        if(!slots[index].hasItem)
            slots[index].addChip(chip);
    }

    #endregion

    public void onSlotClick(Slot slot) {
        if(slot.hasItem && draggingChip == null) {
            draggingChip = slot.transform.GetComponent<Slot>().chip;
            slot.removeChip();

            lastDraggingSlot = slot;
        } else if(!slot.hasItem && draggingChip != null) {
            slot.addChip(draggingChip);

            draggingChip = null;
            lastDraggingSlot = slot;
        }
    }

    public void onChipAreaClick(ChipArea chipArea) {
        if (draggingChip == null)
            return;

        if(FindObjectOfType<PlayerManager>().addChipToMemory(draggingChip)) {
            chipArea.chips.Add(draggingChip);
        
            draggingChip = null;

            chipArea.updateChips();
        } else {
            lastDraggingSlot.addChip(draggingChip);
            draggingChip = null;
        }
    }

    public void onRestClick() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
