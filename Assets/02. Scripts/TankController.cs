using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cinemachine;

using Photon.Pun;
using Photon.Realtime;

public class TankController : MonoBehaviourPunCallbacks
{
    private Transform tr;
    private Rigidbody rb;
    private CinemachineVirtualCamera cvc;

    //이동 속도
    public float moveSpeed = 10.0f;
    //회전 속도
    public float turnSpeed = 100.0f;

    //포탄 prefab
    public GameObject cannonObj;
    public Transform firePos;
    public float cannonSpeed = 100.0f;

    private PhotonView pv;

    //Health
    private float currHp = 100.0f;
    private float initHp = 100.0f;

    void Start()
    {
        //if (!pv.IsMine)
        //{
        //    this.enabled = false;
        //}
        //this.enabled = pv.IsMine;

        tr = this.gameObject.GetComponent<Transform>();
        rb = this.gameObject.GetComponent<Rigidbody>();
        pv = this.gameObject.GetComponent<PhotonView>();

        if (pv.IsMine)
        {
            GameObject _cvc = GameObject.FindGameObjectWithTag("V_CAM");
            cvc = _cvc.GetComponent<CinemachineVirtualCamera>();
            cvc.Follow = tr;
            cvc.LookAt = tr;
        }

        rb.centerOfMass = new Vector3(0, -5.0f, 0);
    }

    
    void Update()
    {
        if (pv.IsMine)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            tr.Translate(Vector3.forward * Time.deltaTime * v * moveSpeed);
            tr.Rotate(Vector3.up * Time.deltaTime * h * turnSpeed);

            if (Input.GetMouseButtonDown(0))
            {
                int actNumber = pv.Owner.ActorNumber;
                pv.RPC("Fire", RpcTarget.AllViaServer, actNumber);
            }
        }
    }


    [PunRPC]
    void Fire(int _actNumber)
    {
        GameObject cannon = Instantiate(cannonObj, firePos.position, firePos.rotation);
        cannon.GetComponent<Cannon>().actNumber = _actNumber;
        cannon.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * cannonSpeed);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("CANNON"))
        {
            currHp -= 10.0f;

            if (currHp < 0)
            {
                DisplayHitInfo(collision.collider.GetComponent<Cannon>().actNumber);
                YouDie();
            }
        }
    }

    void DisplayHitInfo(int _actNumber)
    {
        string userName = "";

        foreach (Player player in PhotonNetwork.PlayerListOthers)
        {
            if (player.ActorNumber == _actNumber)
            {
                userName = player.NickName;
            }
        }

        string msg = $"[{PhotonNetwork.NickName}] is killed by [{userName}].";
        GameManager.instance.pv.RPC("SendMsg", RpcTarget.AllBufferedViaServer, msg);
    }

    void YouDie()
    {
        print("유다희");
        SetVisible(false);

        Invoke("RespawnTank", 3.0f);
    }


    void RespawnTank()
    {
        //HP 초기화
        currHp = initHp;

        Vector3 pos = new Vector3(Random.Range(-100, 100), 10.0f, Random.Range(-100, 100));
        tr.position = pos;
        SetVisible(true);
    }


    void SetVisible(bool isVisible)
    {
        foreach (Renderer _render in GetComponentsInChildren<MeshRenderer>())
        {
            _render.enabled = isVisible;
        }
    }
}
