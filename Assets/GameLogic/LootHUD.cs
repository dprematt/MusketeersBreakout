using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;

public class LootHUD : MonoBehaviourPunCallbacks
{
    private Transform selectedSlot;
    private Vector3 startPos;
    private Vector3 endPos;
    private bool isDragging = false;
    private Vector3 offset;
    public Inventory inventory;
    private Image draggedImage;

    void Start()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("KillHud", RpcTarget.Others);
        }
    }

    [PunRPC]
    public void KillHud()
    {
        gameObject.SetActive(false);
    }

    public void Clean()
    {
        if (inventory == null)
            return;
        for (int i = 0; i < 9; i++)
        {
            InventoryScript_ItemRemoved(this, new InventoryEventArgs(i));
        }
        for (int i = 0; i < 9; i++)
        {
            if (inventory.mItems[i] != null)
            {
                LootInventoryScript_InsertItemAt(this, new InventoryEventArgs(inventory.mItems[i], i));
            }
        }
    }

    public void init(ref Inventory LootInventory)
    {
        inventory = LootInventory;
        LootInventory.ItemAdded += LootInventoryScript_ItemAdded;
        LootInventory.ItemRemoved += InventoryScript_ItemRemoved;
        LootInventory.ItemInsertedAt += LootInventoryScript_InsertItemAt;

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

    private void LootInventoryScript_InsertItemAt(object sender, InventoryEventArgs e)
    {
        Debug.Log("event item inserted hud");
        Debug.Log(e.Item.Name);
        Transform inventoryPanel = transform.Find("Inventory");
        Image image = inventoryPanel.GetChild(e.Index).GetChild(0).GetChild(0).GetComponent<Image>();
        Button button = inventoryPanel.GetChild(e.Index).GetChild(0).GetComponent<Button>();

        image.enabled = true;
        image.sprite = e.Item.Image;
        button.onClick.AddListener(e.Item.Attack);
    }
    
    public void LootInventoryScript_ItemAdded(object sender, InventoryEventArgs e)
    {
        Transform inventoryPanel = transform.Find("Inventory");
        foreach (Transform slot in inventoryPanel)
        {
            Image image = slot.GetChild(0).GetChild(0).GetComponent<Image>();
            Button button = slot.GetChild(0).GetComponent<Button>();
            if (!image.enabled)
            {
                image.enabled = true;
                image.sprite = e.Item.Image;
                button.onClick.AddListener(e.Item.Attack);
                break;
            }
        }
    }

    private void InventoryScript_ItemRemoved(object sender, InventoryEventArgs e)
    {
        Transform inventoryPanel = transform.Find("Inventory");
        string tag = "Slot" + e.Index;
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
        selectedSlot = eventData.pointerPress.transform.parent;

        if (selectedSlot != null)
        {
            // R�cup�rer l'image de l'item
            draggedImage = selectedSlot.GetChild(0).GetChild(0).GetComponent<Image>();

            // D�finir la position initiale de l'image pour qu'elle suive la souris
            draggedImage.transform.position = eventData.position;

            // Enregistrer l'offset pour le mouvement pr�cis
            offset = (Vector2)selectedSlot.position - eventData.position;

            startPos = selectedSlot.position;
            isDragging = true;
            selectedSlot.GetChild(0).GetComponent<Button>().interactable = false; // D�sactiver le bouton
        }
    }

    private void LootOnDrag(PointerEventData eventData)
    {
        if (isDragging && selectedSlot != null)
        {
            // D�placez l'image de l'item avec la souris
            draggedImage.transform.position = eventData.position; // Suivre le curseur

            // Assurez-vous que l'image est toujours devant les autres slots
            draggedImage.transform.SetAsLastSibling();
        }
    }

    private void LootOnEndDrag(PointerEventData eventData)
    {
        if (isDragging && selectedSlot != null)
        {
            selectedSlot.GetChild(0).GetComponent<Button>().interactable = true;
            isDragging = false;

            // V�rifiez le slot sous le curseur
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            Transform releasedSlot = FindSlotFromRaycastResults(results);

            if (releasedSlot != null && releasedSlot != selectedSlot)
            {
                // �changez les slots
                SwapSlots(selectedSlot, releasedSlot);

                // R�attachez l'image au slot cible
                draggedImage.transform.SetParent(releasedSlot.GetChild(0), false);
                draggedImage.transform.localPosition = Vector3.zero; // Positionnez au centre du slot
                draggedImage.transform.SetAsLastSibling(); // Assurez-vous que l'image est devant
            }
            else
            {
                // Si aucun autre slot trouv�, remettre l'image � sa position d'origine
                draggedImage.transform.SetParent(selectedSlot.GetChild(0), false);
                draggedImage.transform.localPosition = Vector3.zero; // R�initialisez la position
            }

            // R�activer l'image dans le slot d'origine si elle avait �t� d�sactiv�e
            selectedSlot.GetChild(0).GetChild(0).GetComponent<Image>().enabled = true;

            // R�initialisation des valeurs
            draggedImage = null;
            selectedSlot = null;
            startPos.Set(0, 0, 0);
            endPos.Set(0, 0, 0);
            offset.Set(0, 0, 0);
        }
    }

    private Transform FindSlotFromRaycastResults(List<RaycastResult> results)
    {
        foreach (RaycastResult result in results)
        {
            Transform slot = result.gameObject.transform;
            // Check if the GameObject is a slot or a child of a slot
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
        // R�cup�ration des composants Inventory
        Inventory p_inventory = gameObject.GetComponentInParent<Inventory>();
        Debug.Log(p_inventory.transform.name);

        // Sauvegarde les positions initiales des slots
        Vector3 pos1 = slot1.position;
        Vector3 pos2 = slot2.position;

        // R�cup�ration des images enfants dans chaque slot
        Transform image1 = slot1.Find("Border/itemImage");
        Transform image2 = slot2.Find("Border/itemImage");

        // Inverse les images en changeant leur parent
        image1.SetParent(slot2.Find("Border"), false); // On assigne l'image1 au slot2
        image2.SetParent(slot1.Find("Border"), false); // On assigne l'image2 au slot1

        // Ajuste les positions pour que les images correspondent bien aux nouveaux parents
        image1.localPosition = Vector3.zero; // R�initialise la position locale dans le nouveau parent
        image2.localPosition = Vector3.zero;

        // Ensuite, vous pouvez appeler SwapItems ou SwapItemsLoot pour �changer les �l�ments logiquement dans l'inventaire
        bool sameParent = slot1.parent == slot2.parent;

        char id_1_c = slot1.tag[slot1.tag.Length - 1];
        int id_1 = int.Parse(id_1_c.ToString());
        char id_2_c = slot2.tag[slot2.tag.Length - 1];
        int id_2 = int.Parse(id_2_c.ToString());
        if (sameParent)
        {
            Debug.Log("swap items in LootHUD");
            inventory.SwapItems(id_1, id_2);
        }
        else
        {
            Debug.Log("swap items loot in LootHUD");
            inventory.SwapItemsLoot(id_1, id_2, p_inventory);
            HUD hud = gameObject.GetComponentInChildren<HUD>();
        }
        Clean();
    }

    void Update()
    {
    }
}