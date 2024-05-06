using UnityEngine;
using PlayFab;
using System.Threading.Tasks;
using PlayFab.ClientModels;
using System.Collections.Generic;

public class PlayFabInventory : MonoBehaviour
{
    string PlayFabID_; 

    private Inventory Inventory_;
    //public PlayerMove Player_;
    public GameObject Player_;

    private void Start()
    {
        Task.Delay(4000).Wait();
        Player_ = GameObject.FindGameObjectWithTag("Player");
        if (Player_ != null)
        {
            Inventory_ = Player_.GetComponent<Inventory>();
            PlayFabID_ = PlayerPrefs.GetString("playfabID");
        }
        else
        {
            Debug.LogError("PlayerMove not found on the same GameObject as PlayFabInventory.");
        }
        PlayFabID_ = PlayerPrefs.GetString("playfabID");
        FetchAllPlayerData();

    }

    public void PlayerLose()
    {
        foreach (IInventoryItem tmp in Inventory_.GetInventory())
        {
            SaveInventory(tmp.Name, 0);
        }

    }

    public void PlayerWin()
    {
        List <IInventoryItem> ItemList = Inventory_.GetInventory();
        foreach (IInventoryItem tmp in ItemList)
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
            //PlayFabId = PlayFabID_,
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
