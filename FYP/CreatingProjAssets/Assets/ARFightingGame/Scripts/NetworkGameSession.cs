using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Linq;

public enum GameState
{
    Offline,
    Connecting,
    Lobby,
    Countdown,
    MatchStarted,
    GameOver
}

public class NetworkGameSession : NetworkBehaviour
{

    GameObject mainMenu;
    GameObject playMenu;
    Text gameStateField;

    public static NetworkGameSession instance;

    NetworkListener networkListener;
    List<NetworkPlayer> players;
    string specialMessage = "";

    [SyncVar]
    public GameState gameState;

    [SyncVar]
    public string message = "";

    public GameObject playerLeftPanel;    

    Text msg;

    GameSession gameSess;

    void Start()
    {
        players = new List<NetworkPlayer>();        
        gameStateField =GameObject.Find("NetworkStateField").GetComponent<Text>();        
        mainMenu = GameObject.Find("MainMenu").transform.GetChild(0).gameObject;
        playMenu = GameObject.Find("GameplayMenu").transform.GetChild(0).gameObject;
        playerLeftPanel = GameObject.Find("GameplayMenu").transform.GetChild(0).GetChild(9).gameObject;
        msg = GameObject.Find("MSG").GetComponent<Text>();
        gameSess = GameObject.Find("GameSession").GetComponent<GameSession>();
        mainMenu.SetActive(true);
        playMenu.SetActive(false);
        gameSess.gameFinished = false;

    }


    void Update()
    {
        if (isServer)
        {
            if (gameState == GameState.Countdown)
            {
                message = "Game Starting in " + Mathf.Ceil(networkListener.mess.CountdownTimer()) + "...";
            }
            else if (specialMessage != "")
            {
                message = specialMessage;
            }
            else
            {
                message = gameState.ToString();
            }
            
        }

        gameStateField.text = message;

        
    }

    public void OnDestroy()
    {
        ////
    }

    [Server]
    public override void OnStartServer()
    {
        networkListener = FindObjectOfType<NetworkListener>();
        gameState = GameState.Connecting;
    }

    
    [Server]
    public void OnStartGame(List<CaptainsMessPlayer> aStartingPlayers)
    {
        players = aStartingPlayers.Select(p => p as NetworkPlayer).ToList();
        
        RpcOnStartedGame();
        foreach (NetworkPlayer p in players)
        {
            p.RpcOnStartedGame();
        }

        StartCoroutine(RunGame());
        
    }    

    [Server]
    public void OnAbortGame()
    {
        //msg.text+= "OnAbortGame , ";
        if (!gameSess.gameFinished) {
            RpcOnAbortedGame();
        }
                    
    }

    [Client]
    public void OnClientAbortGame()
    {
        //msg.text+= "OnClientAbortGame , ";
        //CmdOnAbortedGame();        
    }

    [Client]
    public override void OnStartClient()
    {
        if (instance)
        {
            Debug.LogError("ERROR: Another GameSession!");
        }
        instance = this;

        networkListener = FindObjectOfType<NetworkListener>();
        networkListener.gameSession = this;
        
        if (gameState != GameState.Lobby)
        {
            gameState = GameState.Lobby;
        }
    }

    public void OnJoinedLobby()
    {
        gameState = GameState.Lobby;
    }
    
    public void OnLeftLobby()
    {
        gameState = GameState.Offline;        
    }

    public void OnCountdownStarted()
    {
        gameState = GameState.Countdown;
    }

    public void OnCountdownCancelled()
    {
        gameState = GameState.Lobby;
    }
    
    [Server]
    IEnumerator RunGame()
    {
        gameState = GameState.MatchStarted;

        foreach (NetworkPlayer p in players)
        {            
            //p.RpcResetHealth();
        }
        
        yield return 0; 
    }

    [Server]
    public void PlayAgain()
    {
        StartCoroutine(RunGame());
    }
        
    [ClientRpc]
    public void RpcOnStartedGame()
    {
        mainMenu.SetActive(false);
        playMenu.SetActive(true);
        playerLeftPanel.SetActive(false);
    }

    [ClientRpc]
    public void RpcOnAbortedGame()
    {
        //msg.text += "RpcOnAbortGame , ";
        
        playerLeftPanel.SetActive(true);         
    }

    /*
    [Command]
    public void CmdOnAbortedGame()
    {
        //msg.text += "CmdOnAbortGame , ";
        RpcOnAbortedGame();
    }
    */
    
}

