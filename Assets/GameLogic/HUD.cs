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
    private Image draggedImage;

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
            }
            Transform HpStats = Stats.Find("HpText");
            if (AtqStats != null)
            {
                TextMeshProUGUI textComp = HpStats.GetComponent<TextMeshProUGUI>();
                if (textComp != null)
                {
                    textComp.text = "HP = " + player.HealthManager.Health_;
                }
            }
            Transform SpeedStats = Stats.Find("SpeedText");
            if (AtqStats != null)
            {
                TextMeshProUGUI textComp = SpeedStats.GetComponent<TextMeshProUGUI>();
                if (textComp != null)
                {
                    textComp.text = "Speed = " + player.moveSpeed;
                }
            }
            Transform KillsStats = Stats.Find("KillsText");
            if (AtqStats != null)
            {
                TextMeshProUGUI textComp = KillsStats.GetComponent<TextMeshProUGUI>();
                if (textComp != null)
                {
                    textComp.text = "Kills = " + player.kills;
                }
            }
            Transform LootStats = Stats.Find("LootsText");
            if (AtqStats != null)
            {
                TextMeshProUGUI textComp = LootStats.GetComponent<TextMeshProUGUI>();
                if (textComp != null)
                {
                    textComp.text = "Loots = " + player.lootedChests;
                }
            }
            Transform DmgTakenStats = Stats.Find("DmgTakenText");
            if (AtqStats != null)
            {
                TextMeshProUGUI textComp = DmgTakenStats.GetComponent<TextMeshProUGUI>();
                if (textComp != null)
                {
                    textComp.text = "Dmg Taken = " + player.dmgTaken;
                }
            }
            Transform DmgDoneStats = Stats.Find("DmgDoneText");
            if (AtqStats != null)
            {
                TextMeshProUGUI textComp = DmgDoneStats.GetComponent<TextMeshProUGUI>();
                if (textComp != null)
                {
                    textComp.text = "Dmg Done = " + player.dmgDone;
                }
            }
        }

        Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;
        DisplayPlayers(players);

    }

    public void DisplayPlayers(Photon.Realtime.Player[] players)
    {
        Transform Players = transform.Find("Players");

        if (Players != null)
        {
            Transform Image = Players.Find("Image");
            Transform RoomName = Image.Find("RoomName");
            if (RoomName != null)
            {
                TextMeshProUGUI textComp = RoomName.GetComponent<TextMeshProUGUI>();
                if (textComp != null)
                {
                    if (PhotonNetwork.InRoom)
                    {
                        textComp.text = "Session de: " + PhotonNetwork.CurrentRoom.Name + "\n\n";
                    }
                }
            }

            Transform playersUi = Players.Find("Players");
            if (playersUi != null)
            {
                TextMeshProUGUI textComp = playersUi.GetComponent<TextMeshProUGUI>();
                if (textComp != null)
                {
                    textComp.text = "";
                    int playerCount = 0;  // Ajout d'un compteur pour limiter à 10 joueurs
                    foreach (Photon.Realtime.Player player in players)
                    {
                        if (playerCount >= 10) break;  // Si plus de 10 joueurs, arrêter l'itération
                        textComp.text += player.NickName + ",\n\n";
                        playerCount++;  // Incrémenter le compteur après chaque joueur
                    }
                }
            }
        }
    }

    public void Clean()
    {
        Inventory inventory = transform.parent.GetComponent<Inventory>();
        if (inventory == null)
            return;

        Debug.Log("CLEAN IN HUD");
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
        Inventory inventory = GetComponentInParent<Inventory>();
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
        Image image = inventoryPanel.GetChild(e.Index).GetChild(0).GetChild(0).GetComponent<Image>();
        Button button = inventoryPanel.GetChild(e.Index).GetChild(0).GetComponent<Button>();
        image.sprite = null;
        image.enabled = false;
        button.onClick.RemoveAllListeners();
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

        if (selectedSlot != null)
        {
            // Récupérer l'image de l'item
            draggedImage = selectedSlot.GetChild(0).GetChild(0).GetComponent<Image>();

            // Définir la position initiale de l'image pour qu'elle suive la souris
            draggedImage.transform.position = eventData.position;

            // Enregistrer l'offset pour le mouvement précis
            offset = (Vector2)selectedSlot.position - eventData.position;

            startPos = selectedSlot.position;
            isDragging = true;
            selectedSlot.GetChild(0).GetComponent<Button>().interactable = false; // Désactiver le bouton
        }
    }

    private void OnDrag(PointerEventData eventData)
    {
        if (isDragging && selectedSlot != null)
        {
            // Déplacez l'image de l'item avec la souris
            draggedImage.transform.position = eventData.position; // Suivre le curseur

            // Assurez-vous que l'image est toujours devant les autres slots
            draggedImage.transform.SetAsLastSibling();
        }
    }

    private void OnEndDrag(PointerEventData eventData)
    {
        if (isDragging && selectedSlot != null)
        {
            selectedSlot.GetChild(0).GetComponent<Button>().interactable = true;
            isDragging = false;

            // Vérifiez le slot sous le curseur
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            Transform releasedSlot = FindSlotFromRaycastResults(results);

            if (releasedSlot != null && releasedSlot != selectedSlot)
            {
                // Échangez les slots
                SwapSlots(selectedSlot, releasedSlot);

                // Réattachez l'image au slot cible
                draggedImage.transform.SetParent(releasedSlot.GetChild(0), false);
                draggedImage.transform.localPosition = Vector3.zero; // Positionnez au centre du slot
                draggedImage.transform.SetAsLastSibling(); // Assurez-vous que l'image est devant
            }
            else
            {
                DropItem(selectedSlot);
                Clean();
                // Si aucun autre slot trouvé, remettre l'image à sa position d'origine
                //draggedImage.transform.SetParent(selectedSlot.GetChild(0), false);
                //draggedImage.transform.localPosition = Vector3.zero; // Réinitialisez la position
            }

            // Réactiver l'image dans le slot d'origine si elle avait été désactivée
            selectedSlot.GetChild(0).GetChild(0).GetComponent<Image>().enabled = true;

            // Réinitialisation des valeurs
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

    private bool SameInventory(Inventory player, IInventoryItem toFind)
    {
        foreach (IInventoryItem item in player.mItems)
        {
            if (toFind == item)
                return true;
        }
        return false;
    }

    private void DropItem(Transform slot1)
    {
        Inventory inventory = gameObject.GetComponentInParent<Inventory>();

        // Sauvegarde les positions initiales des slots
        Vector3 pos1 = slot1.position;

        // Récupération des images enfants dans chaque slot
        Transform image1 = slot1.Find("Border/itemImage");

        // Inverse les images en changeant leur parent

        // Ajuste les positions pour que les images correspondent bien aux nouveaux parents
        image1.localPosition = Vector3.zero; // Réinitialise la position locale dans le nouveau parent
        char id_1_c = slot1.tag[slot1.tag.Length - 1];
        int id_1 = int.Parse(id_1_c.ToString());
        inventory.DropItem(id_1);
    }

    private void SwapSlots(Transform slot1, Transform slot2)
    {
        // Récupération des composants Inventory
        Inventory inventory = gameObject.GetComponentInParent<Inventory>();

        // Sauvegarde les positions initiales des slots
        Vector3 pos1 = slot1.position;
        Vector3 pos2 = slot2.position;

        // Récupération des images enfants dans chaque slot
        Transform image1 = slot1.Find("Border/itemImage");
        Transform image2 = slot2.Find("Border/itemImage");

        // Inverse les images en changeant leur parent
        image1.SetParent(slot2.Find("Border"), false); // On assigne l'image1 au slot2
        image2.SetParent(slot1.Find("Border"), false); // On assigne l'image2 au slot1

        // Ajuste les positions pour que les images correspondent bien aux nouveaux parents
        image1.localPosition = Vector3.zero; // Réinitialise la position locale dans le nouveau parent
        image2.localPosition = Vector3.zero;

        bool sameParent = slot1.parent == slot2.parent;

        char id_1_c = slot1.tag[slot1.tag.Length - 1];
        int id_1 = int.Parse(id_1_c.ToString());
        char id_2_c = slot2.tag[slot2.tag.Length - 1];
        int id_2 = int.Parse(id_2_c.ToString());
        if (sameParent)
        {
            inventory.SwapItems(id_1, id_2);
        }
        else
        {
            LootHUD lootHUD = gameObject.GetComponentInChildren<LootHUD>();
            inventory.SwapItemsLoot(id_1, id_2, lootHUD.inventory);
            //lootHUD.Clean();
        }
        Clean();
    }

    void Update()
    {
        SetStatDmg();
    }
}