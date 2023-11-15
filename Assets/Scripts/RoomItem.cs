using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    //[SerializeField]
    //public Text roomName;

    public Text roomName;

    LobbyManager manager;

    private void Start()
    {
        //roomName = roomButtonObject.GetComponent<Text>();
        manager = FindObjectOfType<LobbyManager>();
    }

    public void SetRoomName(string _roomName)
    {
        Debug.Log("inside SetRoomname roomitem roomname is =");
        Debug.Log(_roomName);
        roomName.text = _roomName;
    }

    public void OnClickItem()
    {
        manager.JoinRoom(roomName.text);
    }
}
