using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEditor.VersionControl;

public class FriendsManager : MonoBehaviourPunCallbacks
{
    public GameObject FriendPrefab_;
    public Transform ContentObject_;
    public InputField Name_;

    List<Friend> FriendItemList_ = new List<Friend>();

    public List<string> friendUsernames = new List<string>();
    
    private float lastFindFriendsTime;

    bool hasGottenFriendsList = false;
    
    void Start()
    {
        lastFindFriendsTime = Time.time; 
    }

    void Update()
    {
        if (gameObject.activeSelf && !hasGottenFriendsList) {
            GetFriendsList();
            hasGottenFriendsList = true;
        }
        if (Time.time - lastFindFriendsTime >= 10f)
        {
            PhotonNetwork.FindFriends(friendUsernames.ToArray());
            lastFindFriendsTime = Time.time;
        }
    }

    public void OnClickAddButton()
    {
        AddFriendByUsername(Name_.text);
    }

    public void OnClickRemoveButton(string Name)
    {
        RemoveFriendByUsername(Name);
    }

    //////////////////////////////////////////////////////////////////////////////////////// ADD
    public void AddFriendByUsername(string friendUsername)
    {
        var request = new GetAccountInfoRequest
        {
            Username = friendUsername
        };

        PlayFabClientAPI.GetAccountInfo(request, OnGetAccountInfoForAddSuccess, OnGetAccountInfoError);

    }

    private void OnGetAccountInfoForAddSuccess(GetAccountInfoResult result)
    {
        Debug.Log("ADD Friend");
        var friendPlayFabId = result.AccountInfo.PlayFabId;
        var addFriendRequest = new AddFriendRequest
        {
            FriendPlayFabId = friendPlayFabId
        };
        if (!friendUsernames.Contains(result.AccountInfo.Username))
        {
            friendUsernames.Add(result.AccountInfo.Username);
        }


        PlayFabClientAPI.AddFriend(addFriendRequest, OnAddFriendSuccess, OnAddFriendError);
    }

    private void OnGetAccountInfoError(PlayFabError error)
    {
        Debug.LogError("Error retrieving user info: " + error.GenerateErrorReport());
    }

    private void OnAddFriendSuccess(AddFriendResult result)
    {
        Debug.Log("Friend added successfully!");
        Debug.Log("Friend array : " + friendUsernames.ToArray());
        PhotonNetwork.FindFriends(friendUsernames.ToArray());
    }

    private void OnAddFriendError(PlayFabError error)
    {
        Debug.LogError("Error adding friend: " + error.GenerateErrorReport());
    }

    //////////////////////////////////////////////////////////////////////////////////////// REMOVE
    public void RemoveFriendByUsername(string friendUsername)
    {
        var request = new GetAccountInfoRequest
        {
            Username = friendUsername
        };

        // foreach (Friend item in FriendItemList_)
        // {
        //     if (item.usernameText.text == friendUsername) {
        //         Destroy(item.gameObject);
        //         break;
        //     }
        // }

        PlayFabClientAPI.GetAccountInfo(request, OnGetAccountInfoForRemoveSuccess, OnGetAccountInfoError);
    }

    public override void OnFriendListUpdate(List<Photon.Realtime.FriendInfo> friendList)
    {
        // foreach (var friend in friendList)
        // {
        //     string friendUsername = friend.UserId;
        //     bool isOnline = friend.IsOnline;
        //     Debug.Log($"Friend: {friendUsername}, Status: {(isOnline ? "Online" : "Offline")}");
        // 

        foreach (Friend item in FriendItemList_)
        {
            Destroy(item.gameObject);
        }
        FriendItemList_.Clear();

        foreach (Photon.Realtime.FriendInfo friend in friendList)
        {
            Debug.Log("ADD Item :" + friend.UserId);
            //AddFriendByUsername(friend.UserId);
            Friend newFriend = Instantiate(FriendPrefab_, ContentObject_).GetComponent<Friend>();
            newFriend.SetUsername(friend.UserId);
            string state = friend.IsOnline ? "online" : "disconnected";
            newFriend.SetState(state);
            FriendItemList_.Add(newFriend);
        }
        Debug.Log("Friend array After OnFriendListUpdate : " + friendUsernames.ToArray());
    }

    private void OnGetAccountInfoForRemoveSuccess(GetAccountInfoResult result)
    {
        Debug.Log("RM Friend");
        var friendPlayFabId = result.AccountInfo.PlayFabId;
        var removeFriendRequest = new RemoveFriendRequest
        {
            FriendPlayFabId = friendPlayFabId
        };
        if (friendUsernames.Contains(result.AccountInfo.Username))
        {
            friendUsernames.Remove(result.AccountInfo.Username);
        }

        PlayFabClientAPI.RemoveFriend(removeFriendRequest, OnRemoveFriendSuccess, OnRemoveFriendError);
    }

    private void OnRemoveFriendSuccess(RemoveFriendResult result)
    {
        PhotonNetwork.FindFriends(friendUsernames.ToArray());
        Debug.Log("Friend removed successfully!");
        Debug.Log("Friend array : " + friendUsernames.ToArray());
    }

    private void OnRemoveFriendError(PlayFabError error)
    {
        Debug.LogError("Error removing friend: " + error.GenerateErrorReport());
    }

    //////////////////////////////////////////////////////////////////////////////////////// GET
    public void GetFriendsList()
    {
        PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest(), OnGetFriendsListSuccess, OnErrorGet);
    }

    private void FillFriendList(string name)
    {
        if (!friendUsernames.Contains(name))
        {
            friendUsernames.Add(name);
        }
    }

    private void OnGetFriendsListSuccess(GetFriendsListResult result)
    {
        Debug.Log("Friends list retrieved");
        foreach (var friend in result.Friends)
        {
            FillFriendList(friend.Username);
        }
    }

    private void OnErrorGet(PlayFabError error)
    {
        Debug.Log("Error GET List: " + error.ErrorMessage);
    }
}
