using Photon.Pun;
using System.Collections;
using UnityEngine;

public class dm : MonoBehaviour
{
    public static dm instance;

    [Header("Game Rules")]
    public int KillsToWin = 30;

    Photon.Realtime.Player max;
    InGameUI ingameui;
    // Start is called before the first frame update
    void OnEnable()
    {
        EventManager.StartListening("OnPlayerRespawn", OnPlayerRespawn);
        EventManager.StartListening("DisableGameType", DisableGameType);
        EventManager.StartListening("GameEnd", GameEnd);
        GameManager.instance.MatchActive = true;
        //Debug.Log("Game in mode " + GameManager.instance.CurrentGameType.GameTypeFullName + " started");
        StartGameLogic();
        ingameui = GameObject.FindGameObjectWithTag("InGameUI").GetComponent<InGameUI>();
        ingameui.InGameMenu.SetActive(false);
    }

    void DisableGameType()
    {

        EventManager.StopListening("OnPlayerRespawn", OnPlayerRespawn);
        EventManager.StopListening("GameEnd", GameEnd);
        EventManager.StopListening("DisableGameType", DisableGameType);
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.MatchActive == true)
            TopKills();
    }

    void TopKills()
    {
        max = PhotonNetwork.LocalPlayer;
        foreach (var item in PhotonNetwork.PlayerList)
        {
            if (item.GetKills() > max.GetKills())
            {
                max = item;
            }
        }
        ingameui.TopFragger.text = "Killleader " + max.NickName + " with " + max.GetKills() + " kill(s)";
        if (max.GetKills() == KillsToWin || max.GetKills() > KillsToWin)
        {
            EventManager.TriggerEvent("GameEnd");
        }
    }

    void GameEnd()
    {
        Ending(false);
        ingameui.gameEndScreen.SetActive(true);
        ingameui.winner.text = "Winner " + max.NickName;
        ingameui.yourResult.text = "You " + PhotonNetwork.LocalPlayer.NickName + " has " + PhotonNetwork.LocalPlayer.GetKills() + " points";
        //string nextMap= GameManager.instance.MapList[Random.Range(0, GameManager.instance.MapList.Length)].MapName;

        int nextMap = GameManager.instance.GetMap();

        ingameui.nextMap.text = "Next Map " + GameManager.instance.MapList[GameManager.instance.GetMap()-1].MapName;
        
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(WaitToLoadNextMap(nextMap));
        }

    }

    void Ending(bool toggle)
    {
        ingameui.TopFragger.enabled = toggle;
        ingameui.DeathPanel.SetActive(toggle);
        //ingameui.Kills.enabled = toggle;
        GameManager.instance.IsAlive = toggle;
        GameManager.instance.MatchActive = toggle;
    }
    IEnumerator WaitToLoadMenu()
    {
        yield return new WaitForSeconds(GameManager.instance.EndGameTime);
        PhotonNetwork.LoadLevel(0);
    }
    IEnumerator WaitToLoadNextMap(int mapname)
    {
        
        yield return new WaitForSeconds(GameManager.instance.EndGameTime);
        PhotonNetwork.RemoveRPCs(GameManager.instance.GameManagerPhotonView);
        GameManager.instance.GameManagerPhotonView.RPC("LoadNextMap", RpcTarget.All, mapname);
        //ingameui.gameEndScreen.SetActive(false);
        //Ending(true);
    }

    void StartGameLogic()
    {
        GameManager.instance.GetRespawns();
        GameManager.instance.HasTeam = false;
        //GameManager.instance.SetCursorLock(true);
        GameManager.instance.CanSpawn = true;
        int r = Random.Range(0, GameManager.instance.DeathMatchSpawns.Length);
        PhotonNetwork.Instantiate(GameManager.instance.playerpref[0].name, GameManager.instance.DeathMatchSpawns[r].transform.position, GameManager.instance.DeathMatchSpawns[r].transform.rotation);
    }

    public void OnPlayerRespawn()
    {
        Debug.Log("Respawning");
        GameManager.instance.IsAlive = true;
        //player_stat.instance.isAlive = true;
        int r = Random.Range(0, GameManager.instance.DeathMatchSpawns.Length);
        PhotonNetwork.Instantiate(GameManager.instance.playerpref[0].name, GameManager.instance.DeathMatchSpawns[r].transform.position, GameManager.instance.DeathMatchSpawns[r].transform.rotation);
    }


}