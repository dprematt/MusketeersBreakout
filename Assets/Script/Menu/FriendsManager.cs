using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Runtime.InteropServices.WindowsRuntime;
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

    private Queue<string> friendQueuePlayfab = new Queue<string>();
    private Queue<string> friendQueuePhoton = new Queue<string>();
    
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
            List<string> tmp = new List<string>();
            for (int i = 0; i < Data.Count(); i++) {
                if (Data[i][2] != null)
                    tmp.Add(Data[i][2]);
            }
            PhotonNetwork.FindFriends(tmp.ToArray());
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
        GetPlayfabIDbyUsername(ID, false);
        for (int i = 0; i < Data.Count(); i++)
        {
            if (Data[i].Contains(Name_.text))
            {
                GetUserData(Data[i][1], false);
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
        List<string> tmp = new List<string>();
            for (int i = 0; i < Data.Count(); i++) {
                if (Data[i][2] != null)
                    tmp.Add(Data[i][2]);
            }
        PhotonNetwork.FindFriends(tmp.ToArray());
        Debug.Log("Friend added successfully!");
        Debug.Log("Friend array : " + tmp.ToArray());
    }

    private void OnRemoveFriendSuccess(RemoveFriendResult result)
    {
        List<string> tmp = new List<string>();
            for (int i = 0; i < Data.Count(); i++) {
                if (Data[i][2] != null)
                    tmp.Add(Data[i][2]);
            }
        PhotonNetwork.FindFriends(tmp.ToArray());
        Debug.Log("Friend removed successfully!");
        Debug.Log("Friend array : " + tmp.ToArray());
    }

    public void GetUserData(string playFabId, bool SourceGet)
    {
 
        Current_Player_PlayFabID_ = playFabId;
        var request = new GetUserDataRequest
        {
            PlayFabId = playFabId
        };
        Debug.Log("CurrentPlayerPlayfabID : " + Current_Player_PlayFabID_);
        if(SourceGet == true)
            PlayFabClientAPI.GetUserData(request, GetPhotonID, OnErrorGetPhoton);
        else
            PlayFabClientAPI.GetUserData(request, GetPhotonID, OnError);
    }

    private void GetPhotonID(GetUserDataResult result)
    {

        if (result.Data == null)
    {
        Debug.Log("result.Data is null");
        return;
    }

    if (result.Data.Count == 0)
    {
        Debug.Log("result.Data is empty");
        return;
    }

    Debug.Log("result.Data count: " + result.Data.Count);
        string ID = null;
        foreach (var entry in result.Data)
        {
            Debug.Log("Entry key :" + entry.Value.Value);
            if (entry.Key == "PhotonID") {
                ID = entry.Value.Value;
                Debug.Log("Friend Photon ID : " + entry.Key);
                break;
            }
        }
        Debug.Log("GETPHOTONID");
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

        if (friendQueuePhoton.Count > 0)
        {
            string nextFriend = friendQueuePhoton.Dequeue();
            StartCoroutine(ProcessFriendPhoton(nextFriend));
        }
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
            Friend newFriend = Instantiate(FriendPrefab_, ContentObject_).GetComponent<Friend>();
            for (int i = 0; i < Data.Count(); i++) {
                if (Data[i].Contains(friend.UserId)) {
                    newFriend.SetUsername(Data[i][0]);
                    string state = friend.IsOnline ? "online" : "disconnected";
                    newFriend.SetState(state);
                    break;
                }
            }
            FriendItemList_.Add(newFriend);
            Debug.Log("Friend : " + friend.UserId + " Connected state : " + friend.IsOnline);
        }
        Debug.Log("Friend array After OnFriendListUpdate : " + friendUsernames.ToArray());
    }

    private void GetPlayfabIDbyUsername(string username, bool SourceGet)
    {
        Current_Player_Name_ = username;
        var request = new GetAccountInfoRequest
        {
            Username = username
        };
        if (SourceGet == true)
            PlayFabClientAPI.GetAccountInfo(request, GetPlayFabID, OnErrorGetPlayfab);
        else
            PlayFabClientAPI.GetAccountInfo(request, GetPlayFabID, OnError);
    }

    private void GetPlayFabID(GetAccountInfoResult result)
    {
        string ID = result.AccountInfo.PlayFabId;
        
        if (ID == null)
        {
            Debug.Log("PlayFabID ID Null for player " + Current_Player_Name_);
        }

        if (Current_Player_Name_ != null)
        {
            //Debug.Log("ID GetPlayFabID: " + ID + " Username : " + Current_Player_Name_);
            for (int i = 0; i < Data.Count; i++)
            {
                if (Data[i].Contains(Current_Player_Name_))
                {
                    if (ID != null)
                    {
                        Data[i].Add(ID);
                        break;
                    }
                }
            }
        }
        else
        {
            Debug.Log("Current_Player_Name_ null");
        }
        

        Current_Player_Name_ = null;

        // Continue processing the next friend in the queue
        if (friendQueuePlayfab.Count > 0)
        {
            string nextFriend = friendQueuePlayfab.Dequeue();
            StartCoroutine(ProcessFriendPlayfab(nextFriend));
        }
    }

    private void OnErrorGetPlayfab(PlayFabError error)
    {
        Debug.LogError("Error: " + error.GenerateErrorReport());
        
        // Continue processing the next friend in the queue even if there's an error
        if (friendQueuePlayfab.Count > 0)
        {
            string nextFriend = friendQueuePlayfab.Dequeue();
            StartCoroutine(ProcessFriendPlayfab(nextFriend));
        }
    }

    private void OnErrorGetPhoton(PlayFabError error)
    {
        Debug.LogError("Error: " + error.GenerateErrorReport());
        
        // Continue processing the next friend in the queue even if there's an error
        if (friendQueuePhoton.Count > 0)
        {
            string nextFriend = friendQueuePhoton.Dequeue();
            StartCoroutine(ProcessFriendPhoton(nextFriend));
        }
    }

    private IEnumerator ProcessFriendPlayfab(string username)
    {
        GetPlayfabIDbyUsername(username, true);
        yield return new WaitUntil(() => Current_Player_Name_ == null);
    }

    private IEnumerator ProcessFriendPhoton(string username)
    {
        GetUserData(username, true);
        yield return new WaitUntil(() => Current_Player_PlayFabID_ == null);
    }

    private void OnGetFriendsListSuccess(GetFriendsListResult result)
    {
        foreach (var friend in result.Friends)
        {
            List<string> tmp = new List<string>();
            tmp.Add(friend.Username);
            if (tmp != null)
                Data.Add(tmp);
            else
                Debug.Log("List<strin> tmp null. friend.username = " + friend.Username);
            friendQueuePlayfab.Enqueue(friend.Username);
        }

        if (friendQueuePlayfab.Count > 0)
        {
            string firstFriend = friendQueuePlayfab.Dequeue();
            StartCoroutine(ProcessFriendPlayfab(firstFriend));
        }
        

    }

    public void PrintData()
    {
        for (int i = 0; i < Data.Count(); i++)
        {
            if (Data[i][1] != null) {
                friendQueuePhoton.Enqueue(Data[i][1]);
                Debug.Log("ADD PLAYFAB ID : " + Data[i][1]);
            }
        }

        if (friendQueuePhoton.Count > 0)
        {
            string firstFriend = friendQueuePhoton.Dequeue();
            StartCoroutine(ProcessFriendPhoton(firstFriend));
        }
        Debug.Log("PrintData");
        for (int i = 0; i < Data.Count; i++)
        {
            for (int x = 0; x < Data[i].Count(); x++) {
                Debug.Log("Index i : " + i + " Index x : " + x + " Value : " + Data[i][x]);
            }

            
        }
        // Debug.Log(Data.Count);
        // Debug.Log(Data[0][0]);
        // Debug.Log(Data[1][0]);
        // Debug.Log(Data[2][1]);
    }

    private void OnError(PlayFabError error)
    {
        Debug.LogError("Error: " + error.GenerateErrorReport());
    }
}

