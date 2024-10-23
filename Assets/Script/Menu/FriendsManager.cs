using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;
using UnityEngine.UI;
using System.Linq;

public class FriendsManager : MonoBehaviourPunCallbacks
{
    public GameObject FriendPrefab_, Notif_, Error_, FriendRequest_;
    public Transform ContentObject_, ContentNotif_;
    public InputField Name_;

    List<Friend> FriendItemList_ = new List<Friend>();

    public List<PlayFab.ClientModels.FriendInfo> friendsList = new List<PlayFab.ClientModels.FriendInfo>();

    private float lastFindFriendsTime;

    public Text Username_;

    public GameObject Chat;
    public GameObject Friend;

    private const byte CustomEventCode = 1;

    void Start()
    {
        lastFindFriendsTime = Time.time;
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), result => {
        string username = result.AccountInfo.Username;
        Username_.text = "Your username is : " + username;
        }, error => {
            Username_.text = "Failed to fetch username";
            Debug.LogError("Failed to get account info: " + error.ErrorMessage);
        });
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (gameObject.activeSelf)
            {
                OnClickAddButton();
            }
        }
        if (Time.time - lastFindFriendsTime >= 1f)
        {

            GetFriendsList();
            lastFindFriendsTime = Time.time;
        }
    }

    public void OnClickAddButton()
    {
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest
        {
            Username = Name_.text,
        }, result =>
        {
            string playfabID = result.AccountInfo.PlayFabId;
            var addFriendRequest = new AddFriendRequest
            {
                FriendPlayFabId = playfabID
            };

            PlayFabClientAPI.AddFriend(addFriendRequest, result =>
            {
                GameObject Notifi = Instantiate(Notif_, ContentNotif_);
                Notif tmp = Notifi.GetComponent<Notif>();
                tmp.SetNotifText("Friend added successfully !");
                Name_.text = "";
            }, error =>
            {
                GameObject errorObject = Instantiate(Error_, ContentNotif_);
                Error errorScript = errorObject.GetComponent<Error>();
                errorScript.SetErrorText("Something went wrong while adding friend !");
            });
        }, error => { GameObject errorObject = Instantiate(Error_, ContentNotif_);
                Error errorScript = errorObject.GetComponent<Error>();
                errorScript.SetErrorText("Something went wrong while adding friend !"); });
    }

    public void OnClickRemoveButton(string Name)
    {
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest
        {
            Username = Name,
        }, result =>
        {
            string playfabID = result.AccountInfo.PlayFabId;

            var removeFriendRequest = new RemoveFriendRequest
            {
                FriendPlayFabId = playfabID
            };

            PlayFabClientAPI.RemoveFriend(removeFriendRequest, OnRemoveFriendSuccess, OnErrorRemoveFriend);
        }, error => { Debug.LogError("Failed to get PlayFab account info: " + error.ErrorMessage); });
    }

    private void OnRemoveFriendSuccess(RemoveFriendResult result)
    {
        GameObject Notifi = Instantiate(Notif_, ContentNotif_);
        Notif tmp = Notifi.GetComponent<Notif>();
        tmp.SetNotifText("Friend removed successfully !");
    }

    private void OnErrorRemoveFriend(PlayFabError error)
    {
        GameObject errorObject = Instantiate(Error_, ContentNotif_);
        Error errorScript = errorObject.GetComponent<Error>();
        errorScript.SetErrorText("Something went wrong while deleting friend !");
    }


    private void GetFriendsList()
    {
        var request = new GetFriendsListRequest
        {
            IncludeFacebookFriends = false,
            IncludeSteamFriends = false
        };

        PlayFabClientAPI.GetFriendsList(request, result =>
        {
            bool tmp = false;
            friendsList = result.Friends;
            Debug.Log("Friend list retrieved successfully. Number of friends: " + friendsList.Count + "   Size Friend list item : " + FriendItemList_.Count());
            if (friendsList.Count != FriendItemList_.Count())
            {
                foreach (Friend item in FriendItemList_)
                {
                    Destroy(item.gameObject);
                }
                FriendItemList_.Clear();
                tmp = true;

            }

            foreach (var friend in friendsList)
            {
                if (tmp == true)
                {
                    Friend newFriend = Instantiate(FriendPrefab_, ContentObject_).GetComponent<Friend>();
                    newFriend.SetUsername(friend.Username);
                    FriendItemList_.Add(newFriend);
                }
                GetConnectStatus(friend.FriendPlayFabId, friend.Username);
            }
        }, error =>
        {
            Debug.LogError("Error retrieving friend list: " + error.GenerateErrorReport());
        });
    }

    void GetConnectStatus(string friendPlayFabId, string username)
    {
        var request = new GetUserDataRequest
        {
            PlayFabId = friendPlayFabId
        };

        PlayFabClientAPI.GetUserData(request, result =>
        {
            if (result.Data != null && result.Data.ContainsKey("Status"))
            {
                string Status = result.Data["Status"].Value;
                foreach (Friend friend in FriendItemList_)
                {
                    if (friend.usernameText.text == username)
                    {
                        friend.SetState(Status);
                    }
                }
                Debug.Log("Friend PlayFabId: " + username + " - Status: " + Status);
            }
            else
            {
                Debug.Log("Friend PlayFabId: " + username + " has no photonId.");
            }

        }, error =>
        {
            Debug.LogError("Error retrieving Status for friend: " + error.GenerateErrorReport());
        });
    }


    public void OnClick()
    {
        if (Chat.activeSelf)
            Chat.SetActive(false);
        if (gameObject.activeSelf)
            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);
    }
}

