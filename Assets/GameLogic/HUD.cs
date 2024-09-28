using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;
using TMPro;

public class HUD : MonoBehaviourPunCallbacks
{
    [Header("UI")]
    RoomData roomData;
    private Transform selectedSlot;
    private Vector3 startPos;
    private Vector3 endPos;
    private bool isDragging = false;
    private Vector3 offset;

    void Start()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("KillHud", RpcTarget.Others);
        }
        //gameObject.SetActive(false);
    }

    [PunRPC]
    public void KillHud()
    {
        gameObject.SetActive(false);
    }

    public void SetStatDmg()
    {
        Transform Stats = transform.Find("Stats");

        Player player = gameObject.GetComponentInParent<Player>();

        if (player == null)
        {
            Debug.Log("player == null");
            return;
        }
        
        if (Stats != null)
        {
            Transform AtqStats = Stats.Find("AtqText");
            if (AtqStats != null)
            {
                TextMeshProUGUI textComp = AtqStats.GetComponent<TextMeshProUGUI>();
                if (textComp != null)
                {
                    if (player.EquippedWeapon != null)
                        textComp.text = "dmg = " + player.EquippedWeapon.damages;
                }
                else
                {
                    Debug.Log("HUD: textComp == null");
                }
            }
            else
            {
                Debug.Log("AtqText == Null");
            }
            Transform HpStats = Stats.Find("HpText");
            if (AtqStats != null)
            {
                TextMeshProUGUI textComp = HpStats.GetComponent<TextMeshProUGUI>();
                if (textComp != null)
                {
                    textComp.text = "HP = " + player.HealthManager.Health_;
                }
                else
                {
                    Debug.Log("HUD: textComp == null");
                }
            }
            else
            {
                Debug.Log("hpText == Null");
            }
            Transform SpeedStats = Stats.Find("SpeedText");
            if (AtqStats != null)
            {
                TextMeshProUGUI textComp = SpeedStats.GetComponent<TextMeshProUGUI>();
                if (textComp != null)
                {
                    textComp.text = "Speed = " + player.moveSpeed;
                }
                else
                {
                    Debug.Log("HUD: textComp == null");
                }
            }
            else
            {
                Debug.Log("speedText == Null");
            }
            Transform KillsStats = Stats.Find("KillsText");
            if (AtqStats != null)
            {
                TextMeshProUGUI textComp = KillsStats.GetComponent<TextMeshProUGUI>();
                if (textComp != null)
                {
                    textComp.text = "Kills = " + player.kills;
                }
                else
                {
                    Debug.Log("HUD: textComp == null");
                }
            }
            else
            {
                Debug.Log("KillsText == Null");
            }
            Transform LootStats = Stats.Find("LootsText");
            if (AtqStats != null)
            {
                TextMeshProUGUI textComp = LootStats.GetComponent<TextMeshProUGUI>();
                if (textComp != null)
                {
                    textComp.text = "Loots = " + player.lootedChests;
                }
                else
                {
                    Debug.Log("HUD: textComp == null");
                }
            }
            else
            {
                Debug.Log("LootsText == Null");
            }
            Transform DmgTakenStats = Stats.Find("DmgTakenText");
            if (AtqStats != null)
            {
                TextMeshProUGUI textComp = DmgTakenStats.GetComponent<TextMeshProUGUI>();
                if (textComp != null)
                {
                    textComp.text = "Dmg Taken = " + player.dmgTaken;
                }
                else
                {
                    Debug.Log("HUD: textComp == null");
                }
            }
            else
            {
                Debug.Log("DmgTakenText == Null");
            }
            Transform DmgDoneStats = Stats.Find("DmgDoneText");
            if (AtqStats != null)
            {
                TextMeshProUGUI textComp = DmgDoneStats.GetComponent<TextMeshProUGUI>();
                if (textComp != null)
                {
                    textComp.text = "Dmg Done = " + player.dmgDone;
                }
                else
                {
                    Debug.Log("HUD: textComp == null");
                }
            }
            else
            {
                Debug.Log("DmgDoneText == Null");
            }
        }
        else
        {
            Debug.Log("HUD: stats == null");
        }

        Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;
        DisplayPlayers(players);

    }

    public void DisplayPlayers(Photon.Realtime.Player[] players)
    {
        //Debug.Log("Display Players");
        Transform Players = transform.Find("Players");

        if (Players != null)
        {
            Debug.Log("Display Players Players found");
            Transform Image = Players.Find("Image");
            Transform RoomName = Image.Find("RoomName");
            if (RoomName != null)
            {
                //Debug.Log("Display Players Room name != null");
                TextMeshProUGUI textComp = RoomName.GetComponent<TextMeshProUGUI>();
                if (textComp != null)
                {
                    if (PhotonNetwork.InRoom)
                    {
                        //Debug.Log("room name set");
                        textComp.text = "Session de: " + PhotonNetwork.CurrentRoom.Name + "\n\n";                    }
                }
                else
                {
                    Debug.Log("HUD: textComp == null");
                }
            }

            Transform playersUi = Players.Find("Players");
            if (playersUi != null)
            {
                //Debug.Log("Display Players playersui != null");
                TextMeshProUGUI textComp = playersUi.GetComponent<TextMeshProUGUI>();
                if (textComp != null)
                {
                    //Debug.Log("Display Players playersui textcomp != null");
                    textComp.text = "";
                    foreach (Photon.Realtime.Player player in players)
                    {
                        Debug.Log("Display Players nickname = " + player.NickName);
                        textComp.text += player.NickName + ",\n\n" ;
                    }
                }
                else
                {
                    Debug.Log("HUD: textComp == null");
                }
            }
        }
    }

    public void Clean()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Inventory inventory = player.GetComponent<Inventory>();
        for (int i = 0; i < 9; i++)
        {
            InventoryScript_ItemRemoved(this, new InventoryEventArgs(i));
        }
        for (int i = 0; i < 9; i++)
        {
            if (inventory.mItems[i] != null)
            {
                InventoryScript_InsertItemAt(this, new InventoryEventArgs(inventory.mItems[i], i));
            }
        }
    }
    public void init()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Inventory inventory = player.GetComponent<Inventory>();
        inventory.ItemAdded += InventoryScript_ItemAdded;
        inventory.ItemRemoved += InventoryScript_ItemRemoved;
        inventory.ItemInsertedAt += InventoryScript_InsertItemAt;
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

    private void InventoryScript_InsertItemAt(object sender, InventoryEventArgs e)
    {
        Transform inventoryPanel = transform.Find("Inventory");
        Image image = inventoryPanel.GetChild(e.Index).GetChild(0).GetChild(0).GetComponent<Image>();
        Button button = inventoryPanel.GetChild(e.Index).GetChild(0).GetComponent<Button>();

        image.enabled = true;
        image.sprite = e.Item.Image;
        button.onClick.AddListener(e.Item.Attack);
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
    }
    private void InventoryScript_ItemRemoved(object sender, InventoryEventArgs e)
    {
        Transform inventoryPanel = transform.Find("Inventory");
        //string tag = "Slot" + e.Index;
        //foreach (Transform slot in inventoryPanel)
        // {
        //   if (slot.CompareTag(tag))
        //{
        Image image = inventoryPanel.GetChild(e.Index).GetChild(0).GetChild(0).GetComponent<Image>();
        Button button = inventoryPanel.GetChild(e.Index).GetChild(0).GetComponent<Button>();
        image.sprite = null;
        image.enabled = false;
        button.onClick.RemoveAllListeners();
        //  }
        //}
    }
    // Update is called once per frame
    private void OnPointerClick(PointerEventData eventData)
    {
        //selectedSlot = eventData.pointerPress.transform;

        // Calculate the offset between the mouse position and the slot position
        //offset = (Vector2)selectedSlot.position - eventData.position;
    }

    private void OnBeginDrag(PointerEventData eventData)
    {
        selectedSlot = eventData.pointerPress.transform.parent;
        offset = (Vector2)selectedSlot.position - eventData.position;
        if (selectedSlot != null)
        {
            startPos = selectedSlot.position;
            isDragging = true;
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
            selectedSlot.GetChild(0).GetComponent<Button>().interactable = true;
            isDragging = false;

            // Check if the release point is over another slot
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            Transform releasedSlot = FindSlotFromRaycastResults(results);

            // If a slot is found, swap positions
            if (releasedSlot == null)
            {
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
        foreach (IInventoryItem item in player.mItems)
        {
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
        if (slot2.position.y >= -30)
        {
            inventory.SwapItems(id_1, id_2);
            //slot1.position = endPos;
            //slot2.position = startPos;
        }
        else
        {
            if (loot == null)
            {
                return;
            }
            LootHUD lootHUD = loot.GetComponent<LootHUD>();
            inventory.SwapItemsLoot(id_1, id_2, lootHUD.inventory);
            //inventory.Print_Inventory();
            //lootHUD.inventory.Print_Inventory();
        }
        Clean();
    }

    void Update()
    {
        SetStatDmg();
    }
}