using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class HUDFixe : MonoBehaviour
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

    public void Clean()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Inventory inventory = player.GetComponent<Inventory>();
        Debug.Log("CLEAN IN HUD FIX");
        for (int i = 0; i < 2; i++)
        {
            InventoryScript_ItemRemoved(this, new InventoryEventArgs(i));
        }
        for (int i = 0; i < 2; i++)
        {
            if (inventory.mItems[i] != null)
            {
                InventoryScript_InsertItemAt(this, new InventoryEventArgs(inventory.mItems[i], i));
            }
        }
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }
    public void init()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Inventory inventory = player.GetComponent<Inventory>();
        inventory.ItemAdded += InventoryScript_ItemAdded;
        inventory.ItemRemoved += InventoryScript_ItemRemoved;
        inventory.ItemInsertedAt += InventoryScript_InsertItemAt;
        Transform inventoryPanel = transform.Find("Inventory");
        for (int i = 0; i < 2; i++)
        {
            InventoryScript_ItemRemoved(this, new InventoryEventArgs(i));
        }
        foreach (Transform slot in inventoryPanel)
        {
            Image image = slot.GetChild(0).GetChild(0).GetComponent<Image>();
            image.enabled = false;
        }
        gameObject.SetActive(false);
        gameObject.SetActive(true);
        /*foreach (Transform slot in inventoryPanel)
        {
            slot.GetChild(0).GetChild(0).GetComponent<Image>().enabled = false;
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
        }*/
    }

    private void InventoryScript_InsertItemAt(object sender, InventoryEventArgs e)
    {
        if (e.Index > 1)
        {
            return;
        }
        Transform inventoryPanel = transform.Find("Inventory");
        Image image = inventoryPanel.GetChild(e.Index).GetChild(0).GetChild(0).GetComponent<Image>();
        Button button = inventoryPanel.GetChild(e.Index).GetChild(0).GetComponent<Button>();

        image.enabled = true;
        image.sprite = e.Item.Image;
        button.onClick.AddListener(e.Item.Attack);
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    private void InventoryScript_ItemAdded(object sender, InventoryEventArgs e)
    {
        Transform inventoryPanel = transform.Find("Inventory");
        foreach (Transform slot in inventoryPanel)
        {
            Image image = inventoryPanel.GetChild(e.Index).GetChild(0).GetChild(0).GetComponent<Image>();
            Button button = inventoryPanel.GetChild(e.Index).GetChild(0).GetComponent<Button>();
            if (!image.enabled)
            {
                image.enabled = true;
                image.sprite = e.Item.Image;
                button.onClick.AddListener(e.Item.Attack);
                break;
            }
        }
        Clean();
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }
    private void InventoryScript_ItemRemoved(object sender, InventoryEventArgs e)
    {
        if (e.Index > 1)
        {
            return;
        }
        Transform inventoryPanel = transform.Find("Inventory");
        //string tag = "Slot" + e.Index;
        //Debug.Log(tag);
        //foreach (Transform slot in inventoryPanel)
       // {
         //   if (slot.CompareTag(tag))
        //{
        Image image = inventoryPanel.GetChild(e.Index).GetChild(0).GetChild(0).GetComponent<Image>();
        Button button = inventoryPanel.GetChild(e.Index).GetChild(0).GetComponent<Button>();
        image.sprite = null;
        image.enabled = false;
        button.onClick.RemoveAllListeners();
        gameObject.SetActive(false);
        gameObject.SetActive(true);
          //  }
        //}
    }
    // Update is called once per frame
    private void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("Store the selected slot when clicked");
        //selectedSlot = eventData.pointerPress.transform;

        // Calculate the offset between the mouse position and the slot position
        //offset = (Vector2)selectedSlot.position - eventData.position;
    }

    /* private int FindSlotId(Transform slot)
     {
         int counter = 0;
         Transform inventoryPanel = transform.Find("Inventory");
         foreach (Transform s in inventoryPanel)
         {
             if (slot == s)
             {
                 return counter;
             }
             counter++;
         }
         return -1;
     }*/
    /*private void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("ON BEGIN DRAG !");
        selectedSlot = eventData.pointerPress.transform.parent;
        offset = (Vector2)selectedSlot.position - eventData.position;
        if (selectedSlot != null)
        {
            startPos = selectedSlot.position;
            //Debug.Log(startPos);
            isDragging = true;
            //Debug.Log("Disable button interaction during drag");
            //Debug.Log(selectedSlot);
            //Debug.Log(selectedSlot.GetChild(0));
            //Debug.Log(selectedSlot.GetChild(0).GetComponent<Button>());
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
            //Debug.Log("Enable button interaction after drag");
            selectedSlot.GetChild(0).GetComponent<Button>().interactable = true;
            isDragging = false;

            // Check if the release point is over another slot
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            //Debug.Log("raycast results");
            //Debug.Log(results);

            Transform releasedSlot = FindSlotFromRaycastResults(results);

            // If a slot is found, swap positions
            if (releasedSlot == null)
            {
                //Debug.Log("released slot is null!");
                //Debug.Log(startPos);
                selectedSlot.position = startPos;
                char id_1_c = selectedSlot.tag[selectedSlot.tag.Length - 1];
                int id_1 = int.Parse(id_1_c.ToString());
                Inventory inventory;
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                inventory = player.GetComponent<Inventory>();
                inventory.DropItem(id_1);
                Clean();
                return;
            }
            if (releasedSlot == selectedSlot)
            {
                //Debug.Log("released slot not selected slot!");
                //Debug.Log(startPos);
                selectedSlot.position = startPos;
                return;
            }
            if (releasedSlot != null && releasedSlot != selectedSlot)
            {
                SwapSlots(selectedSlot, releasedSlot);
                selectedSlot.position = startPos;
                return;
            }
        }
        selectedSlot = null;
        startPos.Set(0, 0, 0);
        endPos.Set(0, 0, 0);
        offset.Set(0, 0, 0);
    }

    private Transform FindSlotFromRaycastResults(List<RaycastResult> results)
    {
        foreach (RaycastResult result in results)
        {
            Transform slot = result.gameObject.transform;
            // Check if the GameObject is a slot or a child of a slot
            //Debug.Log(slot);
            string tag = slot.tag;
            if (tag.StartsWith("Slot") && slot != null)
            {
                return slot;
            }
        }
        return null;
    }

    private bool SameInventory(Inventory player, IInventoryItem toFind)
    {
        Debug.Log(toFind.Name);
        foreach (IInventoryItem item in player.mItems)
        {
            Debug.Log(item.Name);
            if (toFind == item)
                return true;
        }
        return false;
    }

    private void DropItem()
    {

    }

    private void SwapSlots(Transform slot1, Transform slot2)
    {
        Debug.Log("SwapSlots");
        endPos = slot2.position;
        Inventory inventory;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        inventory = player.GetComponent<Inventory>();
        //
        GameObject loot = GameObject.FindGameObjectWithTag("LootHUD");
      
        //
        char id_1_c = slot1.tag[slot1.tag.Length - 1];
        int id_1 = int.Parse(id_1_c.ToString());
        char id_2_c = slot2.tag[slot2.tag.Length - 1];
        int id_2 = int.Parse(id_2_c.ToString());
        Debug.Log(slot1.position);
        Debug.Log(slot2.position);
        Debug.Log(id_1);
        Debug.Log(id_2);
        Debug.Log(slot1.tag);
        Debug.Log(slot2.tag);
        if ((slot1.position.y <= slot2.position.y + 10) && ((slot1.position.y >= slot2.position.y - 10)))
        {
            Debug.Log("call swap items");
            inventory.SwapItems(id_1, id_2);
            //slot1.position = endPos;
            //slot2.position = startPos;
        }
        else
        {
            //Debug.Log("y different");
            if (loot == null)
            {
                Debug.Log("HUD NOT FOUND");
                return;
            }
            LootHUD lootHUD = loot.GetComponent<LootHUD>();
            inventory.SwapItemsLoot(id_1, id_2, lootHUD.inventory);
            //Debug.Log("print player inventory after");
            //inventory.Print_Inventory();
            //Debug.Log("print loot inventory after");
            //lootHUD.inventory.Print_Inventory();
        }
        Clean();
    }

    void Update()
    {
    }*/
}