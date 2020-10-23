using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinServer : MonoBehaviour
{
    [SerializeField] private Text ServerNameText;
    [SerializeField] private Text ServerMapNameText;
    [SerializeField] private Text ServerPlayersText;
    [SerializeField] private Text ServerGameTypeText;
    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(ServerNameText.text);
    }

    public void Setup(string Server)
    {
        string[] ser = Server.Split('-');
        ServerNameText.text = ser[0];
        ServerPlayersText.text = ser[1] + "/" + ser[2];
        ServerGameTypeText.text =ser[3];
        ServerMapNameText.text = ser[4];
    }
}
