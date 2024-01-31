using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class HUD : MonoBehaviour
{
    private Transform selectedSlot;
    private Vector3 startPos;
    private Vector3 endPos;
    private bool isDragging = false;
    private Vector3 offset;

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
        foreach (Transform slot in inventoryPanel)
        {
            EventTrigger trigger = slot.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) => { OnPointerClick((PointerEventData)data); });
            trigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.BeginDrag;
            entry.callback.AddListener((data) => { OnBeginDrag((PointerEventData)data); });
            trigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Drag;
            entry.callback.AddListener((data) => { OnDrag((PointerEventData)data); });
            trigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.EndDrag;
            entry.callback.AddListener((data) => { OnEndDrag((PointerEventData)data); });
            trigger.triggers.Add(entry);
        }
    }

    private void InventoryScript_ItemAdded(object sender, InventoryEventArgs e)
    {
        Debug.Log("event item added hud");
        Transform inventoryPanel = transform.Find("Inventory");
        foreach (Transform slot in inventoryPanel)
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
    private void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("Store the selected slot when clicked");
        //selectedSlot = eventData.pointerPress.transform;

        // Calculate the offset between the mouse position and the slot position
        //offset = (Vector2)selectedSlot.position - eventData.position;
    }

    private void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("ON BEGIN DRAG !");
        selectedSlot = eventData.pointerPress.transform.parent;
        offset = (Vector2)selectedSlot.position - eventData.position;
        if (selectedSlot != null)
        {
            startPos = selectedSlot.position;
            Debug.Log(startPos);
            isDragging = true;
            Debug.Log("Disable button interaction during drag");
            Debug.Log(selectedSlot);
            Debug.Log(selectedSlot.GetChild(0));
            Debug.Log(selectedSlot.GetChild(0).GetComponent<Button>());
            selectedSlot.GetChild(0).GetComponent<Button>().interactable = false;
        }
    }

    private void OnDrag(PointerEventData eventData)
    {
        if (isDragging && selectedSlot != null)
        {
            // Move the slot with the mouse position, considering the offset
            selectedSlot.position = new Vector3(eventData.position.x + offset.x, eventData.position.y + offset.y, selectedSlot.position.z);

        }
    }

    private void OnEndDrag(PointerEventData eventData)
    {
        if (isDragging && selectedSlot != null)
        {
            Debug.Log("Enable button interaction after drag");
            selectedSlot.GetChild(0).GetComponent<Button>().interactable = true;
            isDragging = false;

            // Check if the release point is over another slot
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            Debug.Log("raycast results");
            Debug.Log(results);

            Transform releasedSlot = FindSlotFromRaycastResults(results);

            // If a slot is found, swap positions
            if (releasedSlot == null)
            {
                Debug.Log("released slot is null!");
                Debug.Log(startPos);
                selectedSlot.position = startPos;
                return;
            }
            if (releasedSlot == selectedSlot)
            {
                Debug.Log("released slot not selected slot!");
                Debug.Log(startPos);
                selectedSlot.position = startPos;
                return;
            }
            if (releasedSlot != null && releasedSlot != selectedSlot)
            {
                SwapSlots(selectedSlot, releasedSlot);
                return;
            }
        }
    }

    private Transform FindSlotFromRaycastResults(List<RaycastResult> results)
    {
        foreach (RaycastResult result in results)
        {
            Transform slot = result.gameObject.transform;
            // Check if the GameObject is a slot or a child of a slot
            Debug.Log(slot);
            if (slot.CompareTag("Slot") && slot != null)
            {
                return slot;
            }
        }
        return null;
    }

    private void SwapSlots(Transform slot1, Transform slot2)
    {
        Debug.Log("Swap the positions of the two slots");
        //Vector3 tempPosition = slot1.position;
        endPos = slot2.position;
        slot1.position = endPos;
        slot2.position = startPos;
    }

    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.L))
        {
            // Set the first slot as selected
            SelectSlot(transform.Find("Inventory").GetChild(0));
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            SelectSlot(transform.Find("Inventory").GetChild(1));
        }*/
    }

    /*void SelectSlot(Transform slot)
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
    }*/

    /*void DeselectSlot(Transform slot)
    {
        // Deselect the slot
        DeselectBorder(slot.Find("Border"));
        slot.GetChild(0).GetComponent<Button>().interactable = true;
    }*/

    /*void SelectBorder(Transform border)
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
    }*/

    /*void DeselectBorder(Transform border)
    {
        // Deselect the Border
        border.GetComponent<Image>().enabled = false;

        // Add other deselected visual changes as needed
    }*/
}