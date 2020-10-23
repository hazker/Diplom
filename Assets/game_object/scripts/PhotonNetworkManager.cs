using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhotonNetworkManager : MonoBehaviourPunCallbacks
{
    [HideInInspector] public static PhotonNetworkManager instance;
    [Header("Photon Networking Settings")]
    public string GameVersion = "";

    [HideInInspector] public string[] Servers;
    void Awake()
    {

        if (instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        MainMenuUI.instance.SetServerStatus("Connected");
    }

    override public void OnJoinedLobby() //When we have connected to the photonservices
    {
        //Debug.Log("Join lobby " + PhotonNetwork.CurrentLobby.Name);

        PhotonNetwork.AutomaticallySyncScene = true;        //Automatically syncs the scene only when entering a new room
    }

    override public void OnJoinedRoom()  //Called on ourself when we join a room
    {

        //Debug.Log("JoinRoom "+PhotonNetwork.CurrentRoom);
        //PhotonNetwork.CurrentRoom.EmptyRoomTtl = 10;
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerListOthers)
        {
            if (player.NickName == PhotonNetwork.LocalPlayer.NickName)
            {       //If there is a player already with the same name as our player
                PhotonNetwork.LocalPlayer.NickName = PhotonNetwork.LocalPlayer.NickName + UnityEngine.Random.Range(0, 9999).ToString(); //Change the name of our player to something unique
            }
        }
        StartCoroutine(WaitTillGameTypeDeclared());
    }

    IEnumerator WaitTillGameTypeDeclared()
    {
        while (PhotonNetwork.CurrentRoom.CustomProperties["gm"].ToString() == null)
        {
            yield return new WaitForSeconds(0.1f);
        }
        GameManager.instance.GetCurrentGameType();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        //Debug.Log("OnRoomListUpdate");
        Array.Clear(Servers, 0, Servers.Length);
        Servers = new string[roomList.Count];
        for (int i = 0; i < roomList.Count; i++)
        {
            Servers[i] = roomList[i].Name + "-" + roomList[i].PlayerCount + "-" + roomList[i].MaxPlayers+"-" +roomList[i].CustomProperties["gm"]+"-"+roomList[i].CustomProperties["map"];
            //Debug.Log(roomList[i].Name + "-" + roomList[i].PlayerCount + "-" + roomList[i].MaxPlayers + roomList[i].CustomProperties["gm"] +"-"+ roomList[i].CustomProperties["map"]);
        }

    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
    }


    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRandomFailed");
        MainMenuUI.instance.CreateServer();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //if (PhotonNetwork.InRoom)
            //Debug.Log(PhotonNetwork.CurrentRoom);
    }
}
