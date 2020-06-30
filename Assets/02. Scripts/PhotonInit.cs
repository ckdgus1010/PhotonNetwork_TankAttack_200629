using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PhotonInit : MonoBehaviourPunCallbacks
{
    public InputField userNameInput;
    public InputField roomNameInput;

    private readonly string gameVersion = "v1.0";
    public string userName;
    public byte maxPlayerCount = 25;
    private bool isConnected = false;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }


    void Start()
    {
        userName = PlayerPrefs.GetString("USER_NAME");
        userNameInput.text = userName;

        if (string.IsNullOrEmpty(userName))
        {
            userName = "Player_" + Random.Range(1, 999).ToString("000");
            userNameInput.text = userName;
        }

        if (PhotonNetwork.IsConnected)
        {
            print("Connected");
        }
        else
        {
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }


    public override void OnConnectedToMaster()
    {
        print("Connected to Master");

        PhotonNetwork.NickName = userName;
        PhotonNetwork.JoinLobby();
        //PhotonNetwork.JoinRandomRoom();
    }


    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print($"Error Code = {returnCode} Msg = {message}");

        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = maxPlayerCount;
        ro.IsOpen = true;
        ro.IsVisible = true;
        PhotonNetwork.CreateRoom("잡채로 이행시 해보겠습니다. " + Random.Range(0, 999).ToString(), ro);
    }

    public override void OnJoinedRoom()
    {
        print("Entered room");

        if (PhotonNetwork.IsMasterClient)
        {
            SceneManager.LoadScene("BattleField");
        }
    }

    public void OnCreateRoomClick()
    {
        string roomName ="";

        if (string.IsNullOrEmpty(roomNameInput.text))
        {
            roomName = "Room_" + Random.Range(0, 99).ToString("00");
            roomNameInput.text = roomName;
        }

        userName = userNameInput.text;

        PlayerPrefs.SetString("USER_NAME", userName);
        PhotonNetwork.NickName = userName;

        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 4;
        ro.IsOpen = true;
        ro.IsVisible = true;
        PhotonNetwork.CreateRoom(roomNameInput.text, ro);
    }

    public void OnJoinRandomRoomClick()
    {
        userName = userNameInput.text;

        PlayerPrefs.SetString("USER_NAME", userName);
        PhotonNetwork.NickName = userName;
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(RoomInfo room in roomList)
        {
            string msg = $"{room.Name} [{room.PlayerCount}/{room.MaxPlayers}]";
            print(msg);
        }
    }
}
