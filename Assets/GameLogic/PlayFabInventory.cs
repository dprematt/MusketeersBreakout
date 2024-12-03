using UnityEngine;
using PlayFab;
using System.Threading.Tasks;
using PlayFab.ClientModels;
using System.Collections.Generic;

public class PlayFabInventory : MonoBehaviour
{
    string PlayFabID_; 

    private Inventory Inventory_;

    private void Start()
    {
        Player player = gameObject.GetComponentInParent<Player>();
        Inventory_ = player.GetComponent<Inventory>();
        //Inventory_ = GetComponent<Inventory>();
        PlayFabID_ = PlayerPrefs.GetString("playfabID");
        FetchAllPlayerData();

    }

    public void PlayerLose()
    {
        foreach (IInventoryItem tmp in Inventory_.GetInventory())
        {
            if (tmp != null && tmp.Name != null && tmp.Name != "null") {
            SaveInventory(tmp.Name, 0);
            }
        }

    }

    void OnApplicationQuit()
    {

        var request = new UpdateUserDataRequest
        {
            Data = new System.Collections.Generic.Dictionary<string, string>
            {
                { "Status", "offline" },

            },
            Permission = UserDataPermission.Public
        };

        PlayFabClientAPI.UpdateUserData(request, OnVariableEnregistree, OnPlayFabError);
        
    }

    public void PlayerWin()
    {
        //IInventoryItem[] Item = Inventory_.GetInventory();
        IInventoryItem[] Item = Inventory_.GetInventory();
        if (Item == null)
            Debug.Log("IInventory null");
        foreach (IInventoryItem tmp in Item)
        {
            if (tmp != null && tmp.Name != null && tmp.Name != "null") {
                Debug.Log("Arme :" + tmp.Name);
                SaveInventory(tmp.Name, 1);
            }
        }
    }

    private void SaveInventory(string variableKey, int flag)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new System.Collections.Generic.Dictionary<string, string>
            {
                { variableKey, flag.ToString() }
            }
        };

        PlayFabClientAPI.UpdateUserData(request, OnVariableEnregistree, OnPlayFabError);
    }

    private void OnVariableEnregistree(UpdateUserDataResult result)
    {
    }

    private void OnPlayFabError(PlayFabError error)
    {
    }

    public void FetchAllPlayerData()
    {
        GetUserDataRequest request = new GetUserDataRequest();
        PlayFabClientAPI.GetUserData(request, OnUserDataSuccess, OnUserDataError);
    }

    private void OnUserDataSuccess(GetUserDataResult result)
    {
        foreach (var kvp in result.Data)
        {
            Debug.Log("Key: " + kvp.Key + ", Value: " + kvp.Value.Value);
            if (kvp.Value.Value == "1")
            {
                Inventory_.AddWeapon(kvp.Key);
            }
        }
    }

    private void OnUserDataError(PlayFabError error)
    {
        Debug.LogError("Failed to get user data: " + error.GenerateErrorReport());
    }
}
