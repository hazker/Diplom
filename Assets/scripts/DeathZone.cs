using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag+" triggerenter");
        if (other.tag == "Player")
        {
            
            other.transform.root.GetComponent<PhotonView>().RPC("ApplyPlayerDamage", RpcTarget.All, 100f, PhotonNetwork.LocalPlayer,"Fall out of map");
        }
    }
}
