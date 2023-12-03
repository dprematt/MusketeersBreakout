using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
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
        /*Transform inventoryPanel = transform.Find("Inventory");
        foreach (Transform slot in inventoryPanel)
        {
            Debug.Log("update item loop");
            Button button = slot.GetChild(0).GetComponent<Button>();
            button.onClick.AddListener()
        */}
}
