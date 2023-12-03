using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LootHUD : MonoBehaviour
{
    void Start()
    {
        Transform inventoryPanel = transform.Find("Inventory");
        foreach (Transform slot in inventoryPanel)
        {
            Image image = slot.GetChild(0).GetChild(0).GetComponent<Image>();
            image.enabled = false;
        }
        //gameObject.SetActive(false);
    }

    public void InventoryFill(List<IInventoryItem> items)
    {
        Transform inventoryPanel = transform.Find("Inventory");
        int counter = 0;
        foreach (Transform slot in inventoryPanel)
        {
            Image image = slot.GetChild(0).GetChild(0).GetComponent<Image>();
            if (!image.enabled)
            {
                image.enabled = true;
                image.sprite = items[counter].Image;
            }
            counter++;
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
    void Update()
    {
        /*Transform inventoryPanel = transform.Find("Inventory");
        foreach (Transform slot in inventoryPanel)
        {
            Debug.Log("update item loop");
            Button button = slot.GetChild(0).GetComponent<Button>();
            button.onClick.AddListener()
        */
    }
}
