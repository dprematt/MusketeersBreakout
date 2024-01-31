using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class PlayFabInventory : MonoBehaviour
{
    string PlayFabID_; 

    private Inventory Inventory_;
    //public PlayerMove Player_;
    public GameObject Player_;

    private void Start()
    {
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
        //Inventory_ = Player_.GetComponent<Inventory>();
        //PlayFabSettings.TitleId = PlayFabID_;
        PlayFabID_ = PlayerPrefs.GetString("playfabID");
        
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
        foreach (IInventoryItem tmp in Inventory_.GetInventory())
        {
            SaveInventory(tmp.Name, 1);
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
}
