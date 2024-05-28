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
    
    void Start()
    {
        lastFindFriendsTime = Time.time; 
        GetFriendsList();
    }

    void Update()
    {
        if (Time.time - lastFindFriendsTime >= 10f)
        {
            PhotonNetwork.FindFriends(friendUsernames.ToArray());
            lastFindFriendsTime = Time.time; // Met à jour le temps du dernier appel
        }
    }

    public void OnClickAddButton()
    {
        AddFriendByUsername(Name_.text, false);
    }

    public void OnClickRemoveButton(string Name)
    {
        RemoveFriendByUsername(Name);
    }

    //////////////////////////////////////////////////////////////////////////////////////// ADD
    public Friend AddFriendByUsername(string friendUsername, bool GetFunc)
    {
        if (!friendUsernames.Contains(friendUsername))
        {
            friendUsernames.Add(friendUsername);
        }
        Friend newfriend = Instantiate(FriendPrefab_, ContentObject_).GetComponent<Friend>();
        if (GetFunc == false)
        {
            var request = new GetAccountInfoRequest
            {
                Username = friendUsername
            };
            PhotonNetwork.FindFriends(friendUsernames.ToArray());

            PlayFabClientAPI.GetAccountInfo(request, OnGetAccountInfoForAddSuccess, OnGetAccountInfoError);
        }
        return newfriend;
    }

    private void OnGetAccountInfoForAddSuccess(GetAccountInfoResult result)
    {
        var friendPlayFabId = result.AccountInfo.PlayFabId;
        var addFriendRequest = new AddFriendRequest
        {
            FriendPlayFabId = friendPlayFabId
        };


        PlayFabClientAPI.AddFriend(addFriendRequest, OnAddFriendSuccess, OnAddFriendError);
    }

    private void OnGetAccountInfoError(PlayFabError error)
    {
        Debug.LogError("Error retrieving user info: " + error.GenerateErrorReport());
    }

    private void OnAddFriendSuccess(AddFriendResult result)
    {
        Debug.Log("Friend added successfully!");
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

        if (friendUsernames.Contains(friendUsername))
        {
            friendUsernames.Remove(friendUsername);
        }

        foreach (Friend item in FriendItemList_)
        {
            if (item.usernameText.text == friendUsername) {
                Destroy(item.gameObject);
                break;
            }
        }

        PhotonNetwork.FindFriends(friendUsernames.ToArray());

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
            Friend newFriend = AddFriendByUsername(friend.UserId, true);
            newFriend.SetUsername(friend.UserId);
            string state = friend.IsOnline ? "online" : "disconnected";
            newFriend.SetState(state);
            FriendItemList_.Add(newFriend);
        }
    }

    private void OnGetAccountInfoForRemoveSuccess(GetAccountInfoResult result)
    {
        var friendPlayFabId = result.AccountInfo.PlayFabId;
        var removeFriendRequest = new RemoveFriendRequest
        {
            FriendPlayFabId = friendPlayFabId
        };

        PlayFabClientAPI.RemoveFriend(removeFriendRequest, OnRemoveFriendSuccess, OnRemoveFriendError);
    }

    private void OnRemoveFriendSuccess(RemoveFriendResult result)
    {
        Debug.Log("Friend removed successfully!");
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

    private void OnGetFriendsListSuccess(GetFriendsListResult result)
    {
        Debug.Log("Friends list retrieved");
        foreach (var friend in result.Friends)
        {
            AddFriendByUsername(friend.Username, true);
        }
    }

    private void OnErrorGet(PlayFabError error)
    {
        Debug.Log("Error GET List: " + error.ErrorMessage);
    }
}
