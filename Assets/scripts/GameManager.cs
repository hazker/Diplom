using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public List<GameObject> playerpref = new List<GameObject>();


    [HideInInspector] public static GameManager instance;
    public PhotonView GameManagerPhotonView;
    [Header("Game Rules")]
    public string ServerName = "";
    public byte MaxPlayers = 20;
    public MapSettings CurrentMap;
    public GameType CurrentGameType;
    public int EndGameTime = 5;
    /// <summary>
    /// GAME MODE/// TIME LIMIT/// KILL LIMIT//Max respawns???????
    /// </summary>

    [Header("Setup")]
    public MapSettings[] MapList;
    public GameType[] GameTypeList;
    public GameObject LoadingPanel;

    [Header("InGame References")]
    public bool HasTeam = false;
    public bool CanSpawn = false;
    public bool InGame = false;
    public bool IsAlive = false;
    public float RespawnTime;
    public bool MatchActive = false;

    [Header("InGameUI")]
    public GameObject GameUI;
    public bool menuOn;

    static string map;
    [HideInInspector] public GameObject[] DeathMatchSpawns;
    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        if (instance == null)
        {
            instance = this;
            SceneManager.sceneLoaded += OnLevelWasFinishedLoading;
        }
        else if (instance != null)
        {
            DestroyImmediate(this.gameObject);
        }
    }

    public void GetRespawns()
    {
        DeathMatchSpawns = GameObject.FindGameObjectsWithTag("DeathMatchSpawn");
        //Debug.Log(DeathMatchSpawns.Length);
    }

    void OnLevelWasFinishedLoading(Scene LoadedScene, LoadSceneMode SceneLoadMode)
    {
        if (LoadedScene.name == "MainMenu")
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = PhotonNetworkManager.instance.GameVersion;
            //Debug.Log("Connected " + PhotonNetwork.IsConnected);
            SetGameType(0);
            SetMap(0);
            InGame = false;
            GameUIActive();
            LockCursor(true);
        }


        if (LoadedScene.name != "MainMenu")
        {
            InGame = true;
            DeathMatchSpawns = GameObject.FindGameObjectsWithTag("DeathMatchSpawn");
            LockCursor(false);
            //map = CurrentMap.MapName;
            if (PhotonNetwork.InRoom)
            {
                InitGameType();
            }
            GameUIActive();
        }
    }

    void GameUIActive()
    {
        //Debug.Log("InGame "+ InGame);
        GameUI.SetActive(InGame);
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(map);
    }

    public void LockCursor(bool cursor)
    {
        Cursor.visible = cursor;
        if (cursor == false)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;
    }

    public void SetMap(int index)
    {
        CurrentMap = MapList[index];
    }

    public void SetGameType(int index)
    {
        CurrentGameType = GameTypeList[index];
    }

    public MapSettings[] GetAllMap()
    {
        MapSettings[] maps = new MapSettings[MapList.Length];
        for (int i = 0; i < MapList.Length; i++)
        {
            maps[i] = MapList[i];
            //Debug.Log(MapList[i].MapName+" "+ MapList.Length+" "+i);
        }
        return maps;
    }

    [PunRPC]
    public void LoadNextMap(int MapToLoad)
    {
        //Debug.Log(MapToLoad);
        //MapChanged = true;
        GameManager.instance.ResetPlayerStats();
        EventManager.TriggerEvent("DisableGameType"); //Tell the gametype to destroy itself
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(MapToLoad);
        }
        InGameUI.instance.gameEndScreen.SetActive(false);
        GameManager.instance.IsAlive = true;

    }

    public void GetCurrentGameType()
    {

        for (int i = 0; i < GameTypeList.Length; i++)
        {
            if (GameTypeList[i].GameTypeLoadName == PhotonNetwork.CurrentRoom.CustomProperties["gm"].ToString())
            {
                CurrentGameType = GameTypeList[i];
            }
        }

        if (CurrentGameType != null)
        {
            InitGameType();
            LoadingPanel.SetActive(false);
        }
    }

    public int GetMap()
    {
        int ind = 0;
        for (int i = 0; i < MapList.Length; i++)
        {
            if (SceneManager.GetActiveScene().name == MapList[i].MapName)
            {
                ind = i;
                break;
            }
        }
        ind++;
        if (ind > MapList.Length - 1)
        {
            ind = 0;
        }
        ind++;
        return ind;
    }

    void InitGameType()
    {
        if (CurrentGameType.GameTypeLoadName == "dm")
        {
            GameTypeList[0].GameTypeBehaviour.enabled = true;
        }
    }

    public void ToggleLoadScreen(bool Toggle, MapSettings MapToLoad)
    {
        //Debug.Log(MapToLoad.MapName);
        map = MapToLoad.MapName;
        LoadingPanel.SetActive(Toggle);
    }

    public void SetCursorLock(bool Toggle)
    {
        if (Toggle == false)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void ClearMatchSettings()
    {
        EventManager.TriggerEvent("DisableGameType");
        ResetPlayerStats();
        ServerName = "";
        CanSpawn = false;
        CurrentGameType = GameTypeList[0];
    }

    public void ResetPlayerStats()
    {
        PhotonNetwork.LocalPlayer.SetKills(0);
        PhotonNetwork.LocalPlayer.SetDeaths(0);
    }

}

[System.Serializable]
public class MapSettings
{
    public string MapName;
    public Sprite MapLoadImage;
}

[System.Serializable]
public class GameType
{
    public string GameTypeFullName;
    public string GameTypeLoadName;
    public Behaviour GameTypeBehaviour;
}