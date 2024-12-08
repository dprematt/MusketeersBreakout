using System.Collections.Generic;
using UnityEngine;
using System;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;
using PlayFab.CloudScriptModels;

public class Inventory : MonoBehaviourPunCallbacks
{
    public int SLOTS;
    PhotonView view;
    public IInventoryItem[] mItems;
    public bool loot = false;
    public bool test = false;
    public string LastItemName;

    public event EventHandler<InventoryEventArgs> ItemAdded;
    public event EventHandler<InventoryEventArgs> ItemInsertedAt;
    public event EventHandler<InventoryEventArgs> ItemRemoved;

    public void AddEnemyWeapon(Weapon weapon)
    {
        if (mItems == null)
            mItems = new IInventoryItem[9];
        Add(weapon);
        if (ItemAdded != null)
        {
            ItemAdded(this, new InventoryEventArgs(weapon));
        }
    }

    public void AddWeapon(string weaponName)
    {
        Debug.Log("Add Weapon : " + weaponName);
        DropWeapons(weaponName);
        loot = false;
    }

    public void EquipMainWeapon(string weaponName, int index)
    {
        if (weaponName == null)
            return;

        GameObject weaponPrefab = Resources.Load<GameObject>(weaponName);
        if (weaponPrefab == null)
        {
            return;
        }

        Vector3 pos;
        pos.z = 0;
        pos.y = 0;
        pos.x = 0;

        Player playerScript = transform.GetComponentInParent<Player>();
        GameObject weaponObject = PhotonNetwork.Instantiate(weaponPrefab.name, pos, Quaternion.identity);
        Weapon weaponItem = weaponObject.GetComponent<Weapon>();

        //Destroy(weaponObject);

        if (weaponItem == null)
        {
            Destroy(weaponObject);
            return;
        }

        if (index == 0)
        {
            playerScript.SetWeaponEvents(weaponItem);
            weaponObject.SetActive(true);
        }
        else
        {
            weaponObject.SetActive(false);
        }
        weaponItem.whenPickUp(playerScript.gameObject);
    }

    public int Count()
    {
        int res = 0;

        if (mItems == null)
            return 0;
        for (int i = 0; i < mItems.Length; i++)
        {
            if (mItems[i] != null)
                res += 1;
        }
        return res;
    }

    public bool CheckOldWeapon(string name)
    {
        Player player = gameObject.GetComponent<Player>();

        Transform weapon = player.FindDeepChild(gameObject.transform, name);
        if (weapon == null)
        {
            return false;
        }
        weapon.gameObject.SetActive(true);
        return true;
    }

    public void RemoveAnimR()
    {
        Player player = gameObject.GetComponent<Player>();

        Transform weapons = player.FindDeepChild(gameObject.transform, "jointItemR");
        if (weapons == null)
        {
            return;
        }
        foreach (Transform child in weapons.transform)
        {
            //gameObject.GetComponent<Weapon>().resetAnim();
            child.gameObject.SetActive(false);
        }
    }

    public void RemoveAnimL()
    {
        Player player = gameObject.GetComponent<Player>();

        Transform weapons = player.FindDeepChild(gameObject.transform, "jointItemL");
        if (weapons == null)
        {
            return;
        }
        foreach (Transform child in weapons.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void ReleaseLeftArm()
    {
        Player player = gameObject.GetComponent<Player>();

        if (player != null)
        {
            Weapon mainWeapon = player.RetrieveEquippedWeapon("jointItemL");
            if (mainWeapon != null)
            {
                mainWeapon.resetAnim(player.gameObject);
            }
        }
    }

    public void ResetWeapon(IInventoryItem item)
    {
        if (item != null)
        {
            Weapon weaponItem = item.GameObject.GetComponent<Weapon>();
            if (weaponItem != null)
            {
                Player playerScript = transform.GetComponentInParent<Player>();
                playerScript.SetWeaponEvents(weaponItem);
            }
        }
        else
        {
            Debug.Log("item == null in resetweapon");
            //RemoveAnimR();
        }
    }
    public void InsertAt(IInventoryItem item, int id)
    {
        mItems[id] = item;
        if (!loot)
        {
            if (gameObject.GetComponent<Player>() != null && item != null)
            {
                Player playerScript = transform.GetComponentInParent<Player>();

                Shield shieldComp = item.GameObject.GetComponent<Shield>();
                if (shieldComp != null)
                {
                    GameObject shield = item.GameObject;

                    if (id == 0)
                    {
                        playerScript.anim.SetLayerWeight(1, 0f);
                        playerScript.anim.SetLayerWeight(4, 1f);
                        playerScript.hasShield = true;
                        playerScript.shieldComp = shieldComp;
                        UpdateActiveWeapon(shield, true);
                    }
                    else
                    {
                        playerScript.shieldComp = null;
                        UpdateActiveWeapon(shield, false);
                    }
                    shieldComp.whenPickUp(gameObject);
                }

                Weapon weaponItem = item.GameObject.GetComponent<Weapon>();
                if (weaponItem != null)
                {
                    GameObject weaponObject = item.GameObject;

                    if (id == 0)
                    {
                        playerScript.SetWeaponEvents(weaponItem);
                        UpdateActiveWeapon(weaponObject, true);
                    }
                    else
                    {
                        UpdateActiveWeapon(weaponObject, false);
                    }
                    weaponItem.whenPickUp(playerScript.gameObject);
                }
            }
        }
    }

    public int Add(IInventoryItem item)
    {
        for (int i = 0; i < mItems.Length; i++)
        {
            if (mItems[i] == null)
            {
                //mItems[i] = item;
                InsertAt(item, i);
                if (i == 0)
                    UpdateActiveWeapon(item.GameObject, true);
                return 0;
            }
        }
        return -1;
    }

    public int PocketCount()
    {
        int res = 0;

        if (mItems[0] != null)
            res += 1;
        if (mItems[1] != null)
            res += 1;
        return res;
    }

    public void RemoveAt(int index)
    {
        mItems[index] = null;
    }

    public void Initialize(int slots, IInventoryItem[] items, bool isLoot)
    {
        if (items == null)
            Debug.Log("in init, items == null");
        SLOTS = slots;
        mItems = items;
        loot = isLoot;
        DisplayCurrentLoot();
    }

    public Inventory(int slots, IInventoryItem[] items, bool isLoot)
    {
        SLOTS = slots;
        mItems = items;
        loot = isLoot;
    }
    public void Start()
    {
        SLOTS = 9;
        view = GetComponent<PhotonView>();
        if (mItems == null)
        {
            mItems = new IInventoryItem[9];
        }
    }

    public IInventoryItem[] GetInventory()
    {
        return mItems;
    }

    public void SwapItems(int index1, int index2)
    {
        Shield shieldComp;

        if (index1 == 0 || index2 == 0)
        {
            IInventoryItem item1 = mItems[index1];
            IInventoryItem item2 = mItems[index2];

            //index2 position d'arriv�e
            if (index2 == 0)
            {
                Debug.Log("In index2 == 0");
                if (item1 != null)
                {
                    Debug.Log("In item1 != null");
                    shieldComp = item1.GameObject.GetComponent<Shield>();
                    if (shieldComp != null)
                    {
                        Debug.Log("In Shield comp != null");
                        Player playerComp = gameObject.GetComponent<Player>();
                        playerComp.shieldComp = shieldComp;
                        playerComp.hasShield = true;
                        playerComp.anim.SetLayerWeight(1, 0f);
                        playerComp.anim.SetLayerWeight(4, 1f);
                    }
                    UpdateActiveWeapon(item1.GameObject, true);
                }
                if (item2 != null)
                {
                    Debug.Log("In item2 != null");
                    shieldComp = item2.GameObject.GetComponent<Shield>();
                    if (shieldComp != null)
                    {
                        Player playerComp = gameObject.GetComponent<Player>();
                        playerComp.shieldComp = null;
                        gameObject.GetComponent<Player>().hasShield = false;
                    }
                    UpdateActiveWeapon(item2.GameObject, false);
                    //item2.GameObject.GetComponent<Weapon>().resetAnim(GameObject.FindGameObjectWithTag("Player"));
                }
                ResetWeapon(mItems[0]);
            }

            //index1 position d'arriv�e
            if (index1 == 0)
            {
                Debug.Log("In index1 == 0");
                if (item1 != null)
                {
                    Debug.Log("In item1 != null");
                    shieldComp = item1.GameObject.GetComponent<Shield>();
                    if (shieldComp != null)
                    {
                        Debug.Log("In Shield comp != null");
                        Player playerComp = gameObject.GetComponent<Player>();
                        playerComp.shieldComp = null;
                        gameObject.GetComponent<Player>().hasShield = false;
                    }
                    UpdateActiveWeapon(item1.GameObject, false);
                    //item1.GameObject.GetComponent<Weapon>().resetAnim(GameObject.FindGameObjectWithTag("Player"));
                }
                if (item2 != null)
                {
                    Debug.Log("In item2 != null");
                    shieldComp = item2.GameObject.GetComponent<Shield>();
                    if (shieldComp != null)
                    {
                        Debug.Log("In Shield comp != null");
                        Player playerComp = gameObject.GetComponent<Player>();
                        playerComp.shieldComp = shieldComp;
                        gameObject.GetComponent<Player>().hasShield = true;
                        playerComp.anim.SetLayerWeight(1, 0f);
                        playerComp.anim.SetLayerWeight(4, 1f);
                    }
                    UpdateActiveWeapon(item2.GameObject, true);
                }
                ResetWeapon(mItems[0]);
            }
        }

        IInventoryItem temp = mItems[index1];
        mItems[index1] = mItems[index2];
        mItems[index2] = temp;
    }

    public void SwapItemsLoot(int index1, int index2, Inventory lootInventory)
    {
        IInventoryItem temp = mItems[index1];
        InsertAt(lootInventory.mItems[index2], index1);
        lootInventory.InsertAt(temp, index2);
    }

    public void Print_Inventory()
    {
        /*IInventoryItem[] Item = GetInventory();

        for (int i = 0; i < mItems.Length; i++)
        {
            if (mItems[i] != null)
            else
            {
            }
        }*/
    }

    public void InsertItem(IInventoryItem item, int index)
    {
        if (Count() == 0)
            mItems = new IInventoryItem[9];
        InsertAt(item, index);
        Print_Inventory();
        if (ItemAdded != null)
        {
            ItemInsertedAt(this, new InventoryEventArgs(item, index));
        }
        if (!loot && index == 0)
        {
            EquipMainWeapon(item.Name, index);
        }
    }

    public void AddItem(IInventoryItem item)
    {
        if (loot == true)
        {
            GameObject newItem = PhotonNetwork.Instantiate(item.Name, gameObject.transform.position, gameObject.transform.rotation);
            if (mItems == null)
                mItems = new IInventoryItem[9];
            Weapon weapon = newItem.GetComponent<Weapon>();
            Add(weapon);
            if (ItemAdded != null)
            {
                ItemAdded(this, new InventoryEventArgs(weapon));
            }
            Destroy(newItem);
            return;
        }

        if (Count() == 0)
        {
            mItems = new IInventoryItem[9];
            Add(item);
            Print_Inventory();
            //item.OnPickup();
            if (ItemAdded != null)
            {
                ItemAdded(this, new InventoryEventArgs(item));
            }
            if (!loot)
            {
                EquipMainWeapon(item.Name, 0);
            }
            return;
        }

        if (Count() < SLOTS)
        {
            Add(item);
            //item.OnPickup();
            if (ItemAdded != null)
            {
                ItemAdded(this, new InventoryEventArgs(item));
            }
        }
    }

    public void Update()
    {
        if (Count() == 0 && loot == true)
        {
            view = gameObject.GetComponent<PhotonView>();
            if (view != null)
            {
                if (view.ViewID == 0)
                {
                    return;
                }
                view.RPC("DestroyObject", RpcTarget.All);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public void DisplayCurrentLoot()
    {
        if (mItems == null)
        {
            return;
        }
        for (int i = 0; i < 9; i++)
        {
            if (mItems[i] != null)
            {
                if (loot == true)
                {
                    Debug.Log("ITEM IN LOOT id = " + i + " name = " + mItems[i].Name);
                }
            }
        }
    }

    public void DisplayLoot(Inventory playerInventory)
    {
        for (int i = 0; i < 9; i++)
        {
            if (mItems[i] != null)
            {
                ItemInsertedAt(this, new InventoryEventArgs(mItems[i], i));
                GameObject playerGo = gameObject;

                if (playerGo == null)
                {
                    return;
                }

                Player player = playerGo.GetComponent<Player>();
                player.lootedChests += 1;
            }
        }
    }
    public void OnCollisionEnter(Collision collision)
    {
        /*if (loot == true)
        {
            Inventory playerInventory = collision.collider.GetComponent<Inventory>();
            if (playerInventory != null)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    DisplayLoot(playerInventory);
                }
            }
        }*/
    }

    public void UpdateActiveWeapon(GameObject item, bool state)
    {
        PhotonView view = item.GetComponent<PhotonView>();
        photonView.RPC("SetActiveWeapon", RpcTarget.All, view.ViewID, state);
    }

    [PunRPC]
    public void SetActiveWeapon(int viewID, bool state)
    {
        PhotonView view = PhotonView.Find(viewID);
        view.gameObject.SetActive(state);
    }

    [PunRPC]
    public void UpdateItems(string name)
    {

        GameObject weaponPrefab = GameObject.FindGameObjectWithTag("TempObjTag");
        if (weaponPrefab == null)
        {
            return;
        }
        //Destroy(weaponPrefab);
        if (weaponPrefab.TryGetComponent(out Weapon weapon))
        {
            if (mItems == null)
                mItems = new IInventoryItem[9];
            Add(weapon);
            ItemAdded?.Invoke(this, new InventoryEventArgs(weapon));
            weaponPrefab.SetActive(false);
            //Destroy(weaponPrefab);
            //weapon.DeactivateAllObjects();
        }

        if (weaponPrefab.TryGetComponent(out Shield shield))
        {
            if (mItems == null)
                mItems = new IInventoryItem[9];
            Add(shield);
            ItemAdded?.Invoke(this, new InventoryEventArgs(shield));
            weaponPrefab.SetActive(false);
            //Destroy(weaponPrefab);
            //weapon.DeactivateAllObjects();
        }
        //PhotonNetwork.Destroy(newItem);
        int viewID = weaponPrefab.GetComponent<PhotonView>().ViewID;
        photonView.RPC("ResetTag", RpcTarget.AllBuffered, viewID, "Untagged");

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(LastItemName);
        }
        else
        {
            LastItemName = (string)stream.ReceiveNext();
        }
    }

    public void ApplyNetworkUpdate(string name)
    {
        view = gameObject.GetComponent<PhotonView>();
        if (view.IsMine)
        {
            view.RPC("UpdateItems", RpcTarget.All, name);
        }
    }

    public void DropItem(int id)
    {
        Vector3 newPos = gameObject.transform.position;
        newPos.x += 2;
        GameObject loot = PhotonNetwork.Instantiate("Prefabs/Loot", newPos, gameObject.transform.rotation);
        loot.GetComponentInChildren<Inventory>().loot = true;
        Inventory lootInventory = loot.GetComponentInChildren<Inventory>();
        LastItemName = mItems[id].Name;
        GameObject newItem = PhotonNetwork.Instantiate(mItems[id].Name, transform.position, transform.rotation);

        if (newItem.GetComponent<Weapon>() != null)
        {
            newItem.GetComponent<Weapon>().RequestTagChange("TempObjTag");
        }
        else if (newItem.GetComponent<Shield>() != null)
        {
            newItem.GetComponent<Shield>().RequestTagChange("TempObjTag");
        }
        lootInventory.ApplyNetworkUpdate("TempObjTag");
        RemoveAt(id);

        if (id == 0)
            RemoveAnimR();
    }

[PunRPC]
public void UpdateTag(int viewID, string newTag)
{
    PhotonView view = PhotonView.Find(viewID);
    if (view != null)
    {
        view.gameObject.tag = newTag;
    }
}
[PunRPC]
public void ResetTag(int viewID, string defaultTag)
{
    PhotonView view = PhotonView.Find(viewID);
    if (view != null)
    {
        view.gameObject.tag = defaultTag;
    }
}

public void DropWeapons(string name)
{
    GameObject newItem = PhotonNetwork.Instantiate(name, transform.position, transform.rotation);
    Debug.Log("DropWeapons : " + name);

    string newTag = "TempObjTag";
    if (name == "Shield")
    {
        newItem.GetComponent<Shield>().RequestTagChange(newTag);
    }
    else
    {
        newItem.GetComponent<Weapon>().RequestTagChange(newTag);
    }

    // Synchroniser le tag avec tous les joueurs
    int viewID = newItem.GetComponent<PhotonView>().ViewID;
    photonView.RPC("UpdateTag", RpcTarget.AllBuffered, viewID, newTag);

    ApplyNetworkUpdate(newTag);
}


    [PunRPC]
    public void DestroyObject()
    {
        PhotonNetwork.Destroy(this.gameObject);
    }
}