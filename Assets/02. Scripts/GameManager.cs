using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance = null;

    public Text msgText;
    public InputField msgInput;
    public PhotonView pv;

    //SingleTon Design Pattern
    private void Awake()
    {
        //if (instance == null)
        //{
        //    instance = this;
        //    DontDestroyOnLoad(this.gameObject);
        //}
        //else if (instance != this)
        //{
        //    Destroy(this.gameObject);
        //}
    }

    void Start()
    {
        pv = this.gameObject.GetComponent<PhotonView>();

        CreateTank();
    }

    void CreateTank()
    {
        Vector3 pos = new Vector3(Random.Range(-100, 100), 10.0f, Random.Range(-100, 100));
        PhotonNetwork.Instantiate("Tank", pos, Quaternion.identity, 0);
    }

    public void Message()
    {
        string msg = $"<color=#00ff00>[{PhotonNetwork.NickName}]</color> {msgInput.text}";
        pv.RPC("SendMsg", RpcTarget.AllBufferedViaServer, msg);
    }

    [PunRPC]
    public void SendMsg(string msg)
    {
        msgText.text += $"\n{msg}";
    }

    public void OnExitRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Lobby");
    }
}
