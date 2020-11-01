using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [HideInInspector] public static InGameUI instance;

    [Header("Stuff")]
    public Text KillFeed;
    public Text TopFragger;
    public Text Ammo;
    

    [Header("PlayerStat")]
    public Text Hp;
    public Text Kills;
    public bool KillPanelShown = false;
    public GameObject KillPanel;

    [Header("Hitmarker")]
    public GameObject Hitmarker;
    public float TimeHitmarker;

    [Header("InGameMenu")]
    public GameObject InGameMenu;
    public bool InGameMenuShown = false;

    [Header("GameEnd")]
    public GameObject gameEndScreen;
    public Text yourResult;
    public Text nextMap;
    public Text winner;

    [Header("DeathScreen")]
    public GameObject DeathPanel;
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            DestroyImmediate(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && GameManager.instance.CurrentGameType.GameTypeLoadName == "dm" && !InGameMenuShown)
        {
            GameManager.instance.menuOn = true;
            Kills.text = "";
            TopFragger.enabled = false;
            foreach (var item in PhotonNetwork.PlayerList)
            {
                Kills.text += item.NickName + " has " + item.GetKills() + " kill(s)\n";
            }
            KillPanel.SetActive(KillPanelShown = !KillPanelShown);
            KillFeed.enabled = false;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            InGameMenu.SetActive(InGameMenuShown = !InGameMenuShown);
            TopFragger.enabled = false;
            GameManager.instance.LockCursor(InGameMenuShown);
            KillPanel.SetActive(false);
            GameManager.instance.menuOn = true;
            KillFeed.enabled = false;
        }
        if (!KillPanelShown && !InGameMenuShown)
        {
            TopFragger.enabled = true;
            GameManager.instance.menuOn = false;
            KillFeed.enabled = true;
        }

    }

    public void SetHp(float hp)
    {
        Hp.text = hp.ToString();
    }

    public void SetAmmo(int ammo)
    {
        Ammo.text = ammo.ToString();
    }

    public void SetDeathpanel(bool toggle)
    {
        DeathPanel.SetActive(toggle);
    }

    public void DoHitMarker()
    {
        Hitmarker.SetActive(true);
        StartCoroutine(HitmarkerDont(TimeHitmarker));

    }

    public void Quit()
    {
        PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer.ActorNumber);
        PhotonNetwork.Disconnect();
        InGameMenu.SetActive(false);
        Application.Quit();
    }

    public void MainMenuLoad()
    {
        GameManager.instance.InGame = false;
        GameManager.instance.ClearMatchSettings();
        PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer.ActorNumber);
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(0);
    }


    public static InGameUI GetInstanse()
    {
        if (instance == null)
        {
            instance = new InGameUI();
        }
        return instance;
    }

    IEnumerator HitmarkerDont(float time)
    {
        yield return new WaitForSeconds(time);
        Hitmarker.SetActive(false);
    }
}
