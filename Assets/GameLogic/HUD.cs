using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    private Transform selectedSlot;
    void Start()
    {
        //gameObject.SetActive(false);
    }

    public void init()
    {
        Debug.Log("player event handler added");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Debug.Log(player);
        Inventory inventory = player.GetComponent<Inventory>();
        Debug.Log("start hud");
        Debug.Log(inventory);
        Debug.Log("start 2 hud");
        inventory.ItemAdded += InventoryScript_ItemAdded;
        Transform inventoryPanel = transform.Find("Inventory");
        foreach (Transform slot in inventoryPanel)
        {
            Image image = slot.GetChild(0).GetChild(0).GetComponent<Image>();
            image.enabled = false;
        }
    }

    private void InventoryScript_ItemAdded(object sender, InventoryEventArgs e)
    {
        Debug.Log("event item added hud");
        Transform inventoryPanel = transform.Find("Inventory");
        foreach(Transform slot in inventoryPanel)
        {
            Debug.Log("event item loop");
            Image image = slot.GetChild(0).GetChild(0).GetComponent<Image>();
            Button button = slot.GetChild(0).GetComponent<Button>();
            Debug.Log(e.Item.Image);
            Debug.Log(image.enabled);

            if (!image.enabled)
            {
                Debug.Log("event item to enable");
                image.enabled = true;
                image.sprite = e.Item.Image;
                Debug.Log("event item enabled");
                button.onClick.AddListener(e.Item.Attack);
                Debug.Log("onclick button event listener item enabled");
                break;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            // Set the first slot as selected
            SelectSlot(transform.Find("Inventory").GetChild(0));
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            SelectSlot(transform.Find("Inventory").GetChild(1));
        }
    }

    void SelectSlot(Transform slot)
    {
        // Deselect the previously selected slot
        if (selectedSlot != null)
        {
            DeselectSlot(selectedSlot);
        }

        // Select the new slot
        selectedSlot = slot;
        SelectBorder(selectedSlot.Find("Border"));
        selectedSlot.GetChild(0).GetComponent<Button>().interactable = false;
    }

    void DeselectSlot(Transform slot)
    {
        // Deselect the slot
        DeselectBorder(slot.Find("Border"));
        slot.GetChild(0).GetComponent<Button>().interactable = true;
    }

    void SelectBorder(Transform border)
    {
        // Toggle the selected state of the Border
        border.GetComponent<Image>().enabled = true;

        // Manually invoke the onClick event of the button in the Border
        Button button = border.GetComponent<Button>();
        Debug.Log("button found");
        Debug.Log(border);
        Debug.Log(button);
        button.onClick.Invoke();
        // Add other selected visual changes as needed
    }

    void DeselectBorder(Transform border)
    {
        // Deselect the Border
        border.GetComponent<Image>().enabled = false;

        // Add other deselected visual changes as needed
    }
}