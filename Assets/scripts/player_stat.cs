using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class player_stat : MonoBehaviour, IPunObservable
{
    [HideInInspector] public static player_stat instance;

    [Header("Player Attributes")]
    public int PlayerHealth = 100;
    public bool isAlive = true;
    public PunTeams.Team PlayerTeam;
    public Animator ragdol;

    [HideInInspector]public PhotonView iam;

    public List<Animator> hands = new List<Animator>();

    //public GameObject body_obj;
    [HideInInspector] public InGameUI ingameui;
    public List<GameObject> hands_obj = new List<GameObject>();
    public Camera cam;
    public float count_live = 100;
    // Start is called before the first frame update
    void Start()
    {
        iam = this.GetComponent<PhotonView>();
        ingameui = GameObject.FindGameObjectWithTag("InGameUI").GetComponent<InGameUI>();
        if(iam.IsMine)
            ingameui.SetHp(count_live);

    }

    // Update is called once per frame
    void Update()
    {
    }

    [PunRPC]
    public void ApplyPlayerDamage(float dmg, Photon.Realtime.Player attacker, string source)
    {
        
        //if (attacker.GetTeam() == PunTeams.Team.none && attacker != PhotonNetwork.LocalPlayer)
        {
            //Debug.Log(attacker.NickName + " dealt " + dmg + " damage" + " with ");
            if (count_live > 0)
            {
                count_live -= dmg;
                if (iam.IsMine)
                {
                    if (count_live <= 0)
                    {
                        //Debug.Log("tur");
                        
                        count_live = 0;
                        iam.RPC("OnPlayerKilled", RpcTarget.All, attacker, PhotonNetwork.LocalPlayer.NickName,source);
                    }
                    ingameui.SetHp(count_live);
                }

            }
        }
    }

    [PunRPC]
    public void OnPlayerKilled(Photon.Realtime.Player attacker, string dead,string source)
    {
        //Debug.Log("Die from " + source);
        this.isAlive = false;
        ingameui.KillFeed.text = attacker.NickName + " kill " + dead+" with "+source;
        if (iam.IsMine && GameManager.instance.IsAlive)
        {
            if (attacker.NickName != PhotonNetwork.LocalPlayer.NickName && source!= "Fall out of map")
            {
                attacker.AddKill(1);
                //Debug.Log(attacker.NickName + " Has " + attacker.GetKills() + " Kills");
            }
            PhotonNetwork.LocalPlayer.AddDeath(1);
            GameManager.instance.IsAlive = false;
            InGameUI.instance.SetDeathpanel(true);
            //ragdol.gameObject.SetActive(false);
            

            Invoke("PlayerRespawn", GameManager.instance.RespawnTime);

        }
        Dead();
    }

    void PlayerRespawn()
    {
        if (GameManager.instance.MatchActive)
        {
            EventManager.TriggerEvent("OnPlayerRespawn");
            PhotonNetwork.Destroy(this.gameObject);
            InGameUI.instance.SetDeathpanel(false);
        }
    }

    private void Dead()
    {
            ragdol.enabled = false;
            foreach (Animator a in hands)
                a.enabled = false;

            //body_obj.SetActive(true);
            foreach (GameObject g in hands_obj)
                g.SetActive(false);

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        /* if(stream.IsWriting){
             stream.SendNext(live);
         }else{
             live = (bool)stream.ReceiveNext();
         }*/
    }
}
