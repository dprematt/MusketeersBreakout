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
using UnityEngine.TextCore.Text;

public class FriendsManager : MonoBehaviourPunCallbacks
{
    public GameObject FriendPrefab_;
    public Transform ContentObject_;
    public InputField Name_;

    List<Friend> FriendItemList_ = new List<Friend>();

    public List<PlayFab.ClientModels.FriendInfo> friendsList = new List<PlayFab.ClientModels.FriendInfo>();

    List<List<string>> Data = new List<List<string>>();
    List<string> PhotonIDS = new List<string>();

    private Queue<string> friendQueuePlayfab = new Queue<string>();
    private Queue<string> friendQueuePhoton = new Queue<string>();

    private float lastFindFriendsTime;

    bool hasGottenFriendsList = false;

    public GameObject Chat;

    void Start()
    {
        lastFindFriendsTime = Time.time;
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

        //Dans cette fonction il faut d'abord aller rechercher tous les Photon ID grâce à la liste des usernames en utilisant ProcessFriendPlayfab
        if (Time.time - lastFindFriendsTime >= 2f)
        {

            GetFriendsList();
            lastFindFriendsTime = Time.time;
        }
    }

    public void OnClickAddButton()
    {
        //PopUp pour confirmer l'envoie de la demande d'amis
        //Mise à jour de la liste de demande d'amis de la personne demandée en amis
    }

    private void OnAddFriendSuccess(AddFriendResult result)
    {
    }

    private void OnErrorAddFriend(PlayFabError error)
    {
    }

    public void OnClickRemoveButton(string Name)
    {
        //Mise à jour de la liste d'amis
        //Mise à jour de la liste d'amis de la personne concernée

        var removeFriendRequest = new RemoveFriendRequest
        {
            //FriendPlayFabId = ID
        };

        PlayFabClientAPI.RemoveFriend(removeFriendRequest, OnRemoveFriendSuccess, OnErrorRemoveFriend);
    }

    private void OnRemoveFriendSuccess(RemoveFriendResult result)
    {
    }

    private void OnErrorRemoveFriend(PlayFabError error)
    {
    }

    public void OnAcceptFriendRequest()
    {
        //Ajout de l'amis dans la liste d'amis
        //Ajout de moi dans la liste d'amis de la personne faisant la requete
        //Suppression de la personne faisant la requete dans la liste de string FriendRequest
    }

    public void OnDeclineFriendRequest()
    {
        //Suppression de la personne faisant la requete dans la liste de string FriendRequest
    }



    public void DisplayFriendRequest()
    {
        //Affichage de toutes les personnes présentes dans le champ "FriendRequest"
    }


    public void GetUserData(string playFabId)
    {
        var request = new GetUserDataRequest
        {
            PlayFabId = playFabId
        };
        PlayFabClientAPI.GetUserData(request, OnSuccessGetData, OnErrorGetData);
    }

    private void OnSuccessGetData(GetUserDataResult result)
    {
    }

    private void OnErrorGetData(PlayFabError error)
    {
    }

    private void GetPlayfabIDbyUsername(string username)
    {
        var request = new GetAccountInfoRequest
        {
            Username = username
        };
        PlayFabClientAPI.GetAccountInfo(request, OnSuccessGetPlayfab, OnErrorGetPlayfab);
    }

    private void OnSuccessGetPlayfab(GetAccountInfoResult result)
    {
    }

    private void OnErrorGetPlayfab(PlayFabError error)
    {
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
                if (tmp == true) {
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
                        friend.SetState(Status); // Varaible d'état de co
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

