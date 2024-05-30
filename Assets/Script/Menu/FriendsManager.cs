using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Runtime.InteropServices.WindowsRuntime;
using Codice.CM.Common;
using System.Linq;

public class FriendsManager : MonoBehaviourPunCallbacks
{
    public GameObject FriendPrefab_;
    public Transform ContentObject_;
    public InputField Name_;

    string Current_Player_PlayFabID_;
    string Current_Player_Name_;
    string Current_Player_PhotonID_;

    List<Friend> FriendItemList_ = new List<Friend>();

    List<string> friendUsernames = new List<string>();

    List<List<string>> Data = new List<List<string>>();
    List<string> PhotonIDS = new List<string>();
    
    private float lastFindFriendsTime;

    bool hasGottenFriendsList = false;
    
    void Start()
    {
        lastFindFriendsTime = Time.time;
    }

    void Update()
    {
        if (gameObject.activeSelf && !hasGottenFriendsList) {
            PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest(), OnGetFriendsListSuccess, OnError);
            hasGottenFriendsList = true;
        }
        if (Time.time - lastFindFriendsTime >= 10f)
        {
            //PhotonNetwork.FindFriends(friendUsernames.ToArray());
            lastFindFriendsTime = Time.time;
        }
    }

    public void OnClickAddButton()
    {
        string ID = null;
        List<string> tmp = new List<string>();

        for (int i = 0; i < Data.Count(); i++)
        {
            if (Data[i].Contains(Name_.text))
            {
                Debug.Log("Friend " + Name_.text + " already in Data list");
                return;
            }
        }
        tmp.Add(Name_.text);
        Data.Add(tmp);
        GetPlayfabIDbyUsername(ID);
        for (int i = 0; i < Data.Count(); i++)
        {
            if (Data[i].Contains(Name_.text))
            {
                GetUserData(Data[i][1]);
                break;
            }
        }

        var addFriendRequest = new AddFriendRequest
        {
            FriendPlayFabId = ID
        };
        PlayFabClientAPI.AddFriend(addFriendRequest, OnAddFriendSuccess, OnError);
    }

    public void OnClickRemoveButton(string Name)
    {
        string ID = null;
        for (int i = 0; i < Data.Count(); i++)
        {
            if (Data[i].Contains(Name)) {
                ID = Data[i][1];
                Data.RemoveAt(i);
                break;
            }
        }
        if (ID == null) {
            Debug.Log("ID OnClickRemove null");
        }
        var removeFriendRequest = new RemoveFriendRequest
        {
            FriendPlayFabId = ID
        };

        PlayFabClientAPI.RemoveFriend(removeFriendRequest, OnRemoveFriendSuccess, OnError);
    }

    //////////////////////////////////////////////////////////////////////////////////////// ADD


    private void OnAddFriendSuccess(AddFriendResult result)
    {
        List<string> PhotonID = new List<string>();
        for (int i = 0; i < Data.Count(); i++)
        {
            if (Data[i][2] != null) {
                PhotonID.Add(Data[i][2]);
            }
        }
        PhotonNetwork.FindFriends(PhotonID.ToArray());
        Debug.Log("Friend added successfully!");
        Debug.Log("Friend array : " + PhotonID.ToArray());
    }

    private void OnRemoveFriendSuccess(RemoveFriendResult result)
    {
        List<string> PhotonID = new List<string>();
        for (int i = 0; i < Data.Count(); i++)
        {
            if (Data[i][2] != null) {
                PhotonID.Add(Data[i][2]);
            }
        }
        PhotonNetwork.FindFriends(PhotonID.ToArray());
        Debug.Log("Friend removed successfully!");
        Debug.Log("Friend array : " + PhotonID.ToArray());
    }

    public void GetUserData(string playFabId)
    {
 
        Current_Player_PlayFabID_ = playFabId;
        var request = new GetUserDataRequest
        {
            PlayFabId = playFabId
        };

        PlayFabClientAPI.GetUserData(request, GetPhotonID, OnError);
    }

    private void GetPhotonID(GetUserDataResult result)
    {
        string ID = null;
        foreach (var entry in result.Data)
        {
            if (entry.Key == "PhotonID") {
                ID = entry.Value.Value;
                Debug.Log("Friend Photon ID : " + entry.Key);
                break;
            }
        }
        if (ID == null) {
            Debug.Log("Photon ID Null for player " + Current_Player_PlayFabID_);
            return;
        }
        if (Current_Player_PlayFabID_ != null) {
            for (int i = 0; i < Data.Count; i++)
            {
                if (Data[i].Contains(Current_Player_PlayFabID_))
                {
                    if (ID != null) {
                        Data[i].Add(ID);
                        break;
                    }
                }
            }
        } else {
            Debug.Log("Current_Player_PlayFabID_ null");
            return;
        }

        Current_Player_PlayFabID_ = null;
    }

    private void GetPlayfabIDbyUsername(string username)
    {
        Current_Player_Name_ = username;
        var request = new GetAccountInfoRequest
        {
            Username = username
        };

        PlayFabClientAPI.GetAccountInfo(request, GetPlayFabID, OnError);
    }

    private void GetPlayFabID(GetAccountInfoResult result)
    {
        string ID = result.AccountInfo.PlayFabId;
        if (ID == null) {
            Debug.Log("PlayFabID ID Null for player " + Current_Player_PlayFabID_);
        }

        if (Current_Player_Name_ != null) {
            for (int i = 0; i < Data.Count; i++)
            {
                if (Data[i].Contains(Current_Player_Name_))
                {
                    if (ID != null) {
                        Data[i].Add(ID);
                        break;
                    }
                }
            }
        } else {
            Debug.Log("Current_Player_Name_ null");
        }


        Current_Player_Name_ = null;

    }



    //////////////////////////////////////////////////////////////////////////////////////// REMOVE


    // public override void OnFriendListUpdate(List<Photon.Realtime.FriendInfo> friendList)
    // {
    //     // foreach (var friend in friendList)
    //     // {
    //     //     string friendUsername = friend.UserId;
    //     //     bool isOnline = friend.IsOnline;
    //     //     Debug.Log($"Friend: {friendUsername}, Status: {(isOnline ? "Online" : "Offline")}");
    //     // 

    //     foreach (Friend item in FriendItemList_)
    //     {
    //         Destroy(item.gameObject);
    //     }
    //     FriendItemList_.Clear();

    //     foreach (Photon.Realtime.FriendInfo friend in friendList)
    //     {
    //         Debug.Log("ADD Item :" + friend.UserId);
    //         GetUserData(friend.UserId);
    //         //AddFriendByUsername(friend.UserId);
    //         Friend newFriend = Instantiate(FriendPrefab_, ContentObject_).GetComponent<Friend>();
    //         newFriend.SetUsername(friend.UserId);
    //         string state = friend.IsOnline ? "online" : "disconnected";
    //         newFriend.SetState(state);
    //         FriendItemList_.Add(newFriend);
    //         Debug.Log("Friend : " + friend.UserId + " Connected state : " + friend.IsOnline);
    //     }
    //     Debug.Log("Friend array After OnFriendListUpdate : " + friendUsernames.ToArray());
    // }


    //////////////////////////////////////////////////////////////////////////////////////// GET

    private void OnGetFriendsListSuccess(GetFriendsListResult result)
    {
        Debug.Log("Friends list retrieved");
        List<string> tmp = new List<string>();

        foreach (var friend in result.Friends)
        {
            tmp.Clear();
            tmp.Add(friend.Username);
            if (tmp != null)
                Data.Add(tmp);
            else
                Debug.Log("List<strin> tmp null. friend.username = " + friend.Username);
            Debug.Log("Friend : " + friend.Username + " added in list");
            GetPlayfabIDbyUsername(friend.Username);
        }
        // for (int i = 0; i < Data.Count; i++)
        // {
        //     GetUserData(Data[i][1]);
        // }
        PrintData();
    }

    private void PrintData()
    {
        for (int i = 0; i < Data.Count; i++)
        {
            for (int x = 0; x < Data[i].Count(); x++)
                Debug.Log("Index i : " + i + " Index x : " + x + " Value : " + Data[i][x]);
        }
    }

    private void OnError(PlayFabError error)
    {
        Debug.LogError("Error: " + error.GenerateErrorReport());
    }
}

