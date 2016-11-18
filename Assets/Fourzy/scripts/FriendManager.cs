using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameSparks.Api.Requests;
using UnityEngine.UI;

namespace Fourzy
{
    public class FriendManager : MonoBehaviour 
    {

    	public GameObject friendEntryPrefab;

    	public GameObject friendsList;

    	public List<GameObject> friends = new List<GameObject>();

        void Start()
        {
            GetFriends();
        }

    	public void GetFriends()
    	{
    		//Every time we call get friends we'll refresh the list
    		for (int i = 0; i < friends.Count; i++)
    		{
    			//Destroy all friend gameObjects currently in the scene
    			Destroy(friends[i]);
    		}
    		//Clear the list of friends so we don't have null reference errors
    		friends.Clear();

    		//Send a ListGameFriendsRequest, which will get all facebook friends who have played the game
    		new ListGameFriendsRequest().Send((response) =>
    			{
    				Debug.Log("Users friends: " + response.JSONString);
    				//For ever friend stored in our collection of friends
    				foreach (var friend in response.Friends)
    				{
    					//Create a new gameObject, add friendEntryPrefab as a child of the Friend Grid GameObject
    					//GameObject go = NGUITools.AddChild(friendGrid.gameObject, friendEntryPrefab);
    					GameObject go = Instantiate(friendEntryPrefab) as GameObject;
    					go.gameObject.transform.SetParent(friendsList.transform);
    					//Update all the gameObject's variables
                        FriendEntry friendEntry = go.GetComponent<FriendEntry>();
                        friendEntry.userName = friend.DisplayName;
                        friendEntry.id = friend.Id;
                        friendEntry.isOnline = friend.Online.Value;
                        friendEntry.facebookId = friend.ExternalIds.GetString("FB");

    					//Add the gameObject to the list of friends
    					friends.Add(go);
    					//Tell the grid to reposition everything nicely
    					//friendGrid.Reposition();
    				}
    			});
    	}
    }
}