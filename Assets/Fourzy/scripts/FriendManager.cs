using UnityEngine;
using System.Collections.Generic;
using GameSparks.Api.Requests;
using Facebook.Unity;
using UnityEngine.UI;
using Lean.Pool;

namespace Fourzy
{
    public class FriendManager : MonoBehaviour 
    {
        public static FriendManager instance;
        public GameObject friendEntryPrefab;
        public GameObject friendsList;
        public GameObject noFriendsText;
        public List<GameObject> friends = new List<GameObject>();
        public Image onlineIndicator;
        private bool isOnline;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
                Destroy(gameObject);
            }

            //Sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(gameObject);
        }

        public void GetFriends()
        {
            isOnline = false;

            if (FB.IsLoggedIn) {
                //Every time we call get friends we'll refresh the list
                for (int i = 0; i < friends.Count; i++)
                {
                    //Destroy all friend gameObjects currently in the scene
                    //Destroy(friends[i]);
                    LeanPool.Despawn(friends[i].gameObject);
                }
                //Clear the list of friends so we don't have null reference errors
                friends.Clear();

                //Send a ListGameFriendsRequest, which will get all facebook friends who have played the game
                new ListGameFriendsRequest().Send((response) =>
                    {
                        //For ever friend stored in our collection of friends
                        foreach (var friend in response.Friends)
                        {
                            //Create a new gameObject, add friendEntryPrefab as a child of the Friend List GameObject
                            //GameObject go = Instantiate(friendEntryPrefab) as GameObject;
                            GameObject go = LeanPool.Spawn(friendEntryPrefab) as GameObject;

                            go.gameObject.transform.SetParent(friendsList.transform);
                            //Update all the gameObject's variables
                            FriendEntry friendEntry = go.GetComponent<FriendEntry>();
                            friendEntry.Reset();

                            friendEntry.userName = friend.DisplayName;
                            friendEntry.id = friend.Id;
                            friendEntry.isOnline = friend.Online.Value;
                            friendEntry.facebookId = friend.ExternalIds.GetString("FB");
                            friendEntry.transform.localScale = new Vector3(1f,1f,1f);

                            if (friend.Online.Value) {
                                isOnline = true;
                            }

                            friendEntry.UpdateFriend();

                            //Add the gameObject to the list of friends
                            friends.Add(go);
                        }

                        if (friends.Count > 0) {
                            noFriendsText.SetActive(false);
                        } else {
                            noFriendsText.SetActive(true);
                        }

                        onlineIndicator.color = isOnline ? Color.green : Color.gray;
                    });
            }
        }
    }
}