using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [HideInInspector] public static MainMenuUI instance;
    [Header("Main Menu")]
    public Text ConnectionStatusText;

    [Header("Create Server Menu")]
    public InputField GameNameInputField;
    public Text SelectedMapText;
    public Text SelectedGameTypeText;
    public Text SelectedMaxPlayersText;
    public Image SelectedMapPreview;
    public Dropdown DropdownMaps;


    [Header("Settings Menu")]
    public InputField PlayerNameInputfield;
    public Dropdown ResolutionDropDown;
    private Resolution[] Resolutions;

    [Header("Servers")]
    public Transform ServerLists;
    public GameObject ServerListEntryPrefab;
    private List<GameObject> ServerListEntries = new List<GameObject>();

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        OnMenuStart();
        IndexResolution();
        SetFullscreen(true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangePictures()
    {
        SelectedMapPreview.sprite = GameManager.instance.MapList[DropdownMaps.value].MapLoadImage;
        GameManager.instance.CurrentMap = GameManager.instance.MapList[DropdownMaps.value];

    }

    void IndexResolution()
    {
        Resolutions = Screen.resolutions.Where(resolution => resolution.refreshRate == 60).ToArray();
        ResolutionDropDown.ClearOptions();
        List<string> ResolutionOptions = new List<string>();
        int CurrentResolutionIndex = 0;
        Resolutions=Resolutions.Distinct().ToArray();
        for (int i = 0; i < Resolutions.Length; i++)
        {
            string resolutionoption = Resolutions[i].width + " x " + Resolutions[i].height;
            ResolutionOptions.Add(resolutionoption);
            if (Resolutions[i].width == Screen.currentResolution.width && Resolutions[i].height == Screen.currentResolution.height)
            {
                CurrentResolutionIndex = i;
            }
        }

        //ResolutionDropDown.AddOptions(ResolutionOptions.Distinct().ToList<string>());
        ResolutionDropDown.AddOptions(ResolutionOptions);
        ResolutionDropDown.value = CurrentResolutionIndex;
        ResolutionDropDown.RefreshShownValue();
    }

    public void SetResolution(int ind)
    {
        Resolution resolution = Resolutions[ind];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetFullscreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    public void SetPlayername(string nick)
    {
        PhotonNetwork.NickName = nick;
        PlayerPrefs.SetString("NickName", nick);
    }

    public void SetQuality(int QualityIndex)
    {
        QualitySettings.SetQualityLevel(QualityIndex);
    }

    void OnMenuStart()
    {
        GameNameInputField.text = "Room:" + Random.Range(0, 9999);
        PhotonNetwork.AutomaticallySyncScene = true;
        Dropdown.OptionData[] m_NewData;
        List<Dropdown.OptionData> m_Messages = new List<Dropdown.OptionData>();
        DropdownMaps.ClearOptions();
        PlayerNameInputfield.text = "Player:" + Random.Range(0, 9999);
        SetPlayername(PlayerNameInputfield.text);
        //Debug.Log(GameManager.instance.CurrentMap.MapName);
        MapSettings[] mapnames = new MapSettings[GameManager.instance.GetAllMap().Length];
        mapnames = GameManager.instance.GetAllMap();
        m_NewData = new Dropdown.OptionData[mapnames.Length];

        for (int i = 0; i < mapnames.Length; i++)
        {
            m_NewData[i] = new Dropdown.OptionData();
            m_NewData[i].text = mapnames[i].MapName;
            m_Messages.Add(m_NewData[i]);
        }
        DropdownMaps.AddOptions(m_Messages);
        SelectedMapPreview.sprite = GameManager.instance.MapList[0].MapLoadImage;
        SelectedGameTypeText.text = GameManager.instance.CurrentGameType.GameTypeLoadName;
    }

    public void Quit()
    {
        Application.Quit();

    }

    public void SetServerStatus(string text)
    {
        ConnectionStatusText.text = text;
    }

    public void CreateServer()
    {
        GameManager.instance.ServerName = GameNameInputField.text;
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
        roomOptions.CustomRoomProperties.Add("map", GameManager.instance.CurrentMap.MapName);
        roomOptions.CustomRoomProperties.Add("gm", GameManager.instance.CurrentGameType.GameTypeLoadName);
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "map", "gm" };
        roomOptions.MaxPlayers = GameManager.instance.MaxPlayers;
        PhotonNetwork.CreateRoom(GameManager.instance.ServerName, roomOptions, null);

        //Debug.Log("Room created " + GameManager.instance.ServerName);
        GameManager.instance.ToggleLoadScreen(true, GameManager.instance.CurrentMap);
        PhotonNetwork.LoadLevel(GameManager.instance.CurrentMap.MapName);
    }

    public void Servers()
    {
        ClearServerList();
        //ServerLists.text = "";
        foreach (var item in PhotonNetworkManager.instance.Servers)
        {
            GameObject ServerListEntry = Instantiate(ServerListEntryPrefab, ServerLists);
            ServerListEntries.Add(ServerListEntry);
            ServerListEntry.GetComponent<JoinServer>().Setup(item);
        }
    }

    public void JoinRandomServer()
    {
        if (PhotonNetwork.IsConnectedAndReady)
            PhotonNetwork.JoinRandomRoom(); //Joins a random room

        //GameManager.instance.ToggleLoadScreen(true, null);
    }

    void ClearServerList()
    {
        foreach (GameObject ServerListEntry in ServerListEntries)
        {
            Destroy(ServerListEntry);
        }
        ServerListEntries.Clear();
    }


}
