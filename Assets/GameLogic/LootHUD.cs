using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class LootHUD : MonoBehaviour
{
    private Transform selectedSlot;
    private Vector3 startPos;
    private Vector3 endPos;
    private bool isDragging = false;
    private Vector3 offset;
    public Inventory inventory;

    void Start()
    {
        //gameObject.SetActive(false);
    }

    public void Clean()
    {
        Debug.Log("CLEAN HUD");
        GameObject loot = GameObject.FindGameObjectWithTag("LootHUD");
        LootHUD lootHUD = loot.GetComponent<LootHUD>();
        Inventory inventory = lootHUD.inventory;
        for (int i = 0; i < 9; i++)
        {
            InventoryScript_ItemRemoved(this, new InventoryEventArgs(i));
        }
        foreach (IInventoryItem lootItem in inventory.mItems)
        {
            if (lootItem != null)
            {
                Debug.Log(lootItem.Name);
                LootInventoryScript_ItemAdded(this, new InventoryEventArgs(lootItem));
            }
        }
    }
    public void init(ref Inventory LootInventory)
    {
        inventory = LootInventory;
        Debug.Log("player event handler added in loothud");
        //Debug.Log("start hud");
        //Debug.Log(inventory);
        //Debug.Log("start 2 hud");
        //inventory.ItemAdded += LootInventoryScript_ItemAdded;
        LootInventory.ItemAdded += LootInventoryScript_ItemAdded;
        LootInventory.ItemRemoved += InventoryScript_ItemRemoved;
        Transform inventoryPanel = transform.Find("Inventory");
        for (int i = 0; i < 9; i++)
        {
            InventoryScript_ItemRemoved(this, new InventoryEventArgs(i));
        }
        foreach (Transform slot in inventoryPanel)
        {
            Image image = slot.GetChild(0).GetChild(0).GetComponent<Image>();
            image.enabled = false;
        }
        foreach (Transform slot in inventoryPanel)
        {
            slot.GetChild(0).GetChild(0).GetComponent<Image>().enabled = false;
            EventTrigger trigger = slot.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) => { LootOnPointerClick((PointerEventData)data); });
            trigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.BeginDrag;
            entry.callback.AddListener((data) => { LootOnBeginDrag((PointerEventData)data); });
            trigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Drag;
            entry.callback.AddListener((data) => { LootOnDrag((PointerEventData)data); });
            trigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.EndDrag;
            entry.callback.AddListener((data) => { LootOnEndDrag((PointerEventData)data); });
            trigger.triggers.Add(entry);
        }
    }

    private void LootInventory_ItemAdded(object sender, InventoryEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    public void LootInventoryScript_ItemAdded(object sender, InventoryEventArgs e)
    {
        // Debug.Log("LootHUD event item added hud");
        Transform inventoryPanel = transform.Find("Inventory");
        foreach (Transform slot in inventoryPanel)
        {
            // Debug.Log("event item loop");
            Image image = slot.GetChild(0).GetChild(0).GetComponent<Image>();
            Button button = slot.GetChild(0).GetComponent<Button>();
            // Debug.Log(e.Item.Image);
            // Debug.Log(image.enabled);

            if (!image.enabled)
            {
                //Debug.Log("event item to enable");
                image.enabled = true;
                image.sprite = e.Item.Image;
                //Debug.Log("event item enabled");
                button.onClick.AddListener(e.Item.Attack);
                //  Debug.Log("onclick button event listener item enabled");
                break;
            }
            /*foreach (Transform s in inventoryPanel)
                {
                    if (s.GetChild(0).GetChild(0).GetComponent<Image>().sprite == e.Item.Image)
                    {
                        //      Debug.Log("ntm de la fdp");
                        return;
                    }*/
        }
    }

    private void InventoryScript_ItemRemoved(object sender, InventoryEventArgs e)
    {
        Transform inventoryPanel = transform.Find("Inventory");
        string tag = "Slot" + e.Index;
        //Debug.Log(tag);
        foreach (Transform slot in inventoryPanel)
        {
            if (slot.CompareTag(tag))
            {
                Image image = slot.GetChild(0).GetChild(0).GetComponent<Image>();
                Button button = slot.GetChild(0).GetComponent<Button>();
                image.sprite = null;
                image.enabled = false;
                button.onClick.RemoveAllListeners();
            }
        }
    }
    // Update is called once per frame
    private void LootOnPointerClick(PointerEventData eventData)
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
            private void LootOnBeginDrag(PointerEventData eventData)
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

    private void LootOnDrag(PointerEventData eventData)
    {
        if (isDragging && selectedSlot != null)
        {
            // Move the slot with the mouse position, considering the offset
            selectedSlot.position = new Vector3(eventData.position.x + offset.x, eventData.position.y + offset.y, selectedSlot.position.z);

        }
    }

    private void LootOnEndDrag(PointerEventData eventData)
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
                inventory.DropItem(id_1);
                selectedSlot.position = startPos;
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

    private void SwapSlots(Transform slot1, Transform slot2)
    {
        //Debug.Log("Swap the positions of the two slots");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Inventory p_inventory = player.GetComponent<Inventory>();
        //Vector3 tempPosition = slot1.position;
        //Debug.Log("tag1");
        //Debug.Log(slot1.tag);
        //Debug.Log("tag2");
        //Debug.Log(slot2.tag);
        endPos = slot2.position;
        //slot1.position = endPos;
        //slot2.position = startPos;
        char id_1_c = slot1.tag[slot1.tag.Length - 1];
        int id_1 = int.Parse(id_1_c.ToString());
        char id_2_c = slot2.tag[slot2.tag.Length - 1];
        int id_2 = int.Parse(id_2_c.ToString());
        Debug.Log(id_1);
        Debug.Log(id_2);
        if ((slot1.position.y <= slot2.position.y + 10) && ((slot1.position.y >= slot2.position.y - 10)))
        {
            inventory.SwapItems(id_1, id_2);
        }
        else
        {
            inventory.SwapItemsLoot(id_1, id_2, p_inventory);

            //slot1.position = endPos;
            //slot2.position = startPos;
        }
        Clean();
    }

    void Update()
    {
    }
}