using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class UserHUDControls : MonoBehaviour
{

    public bool punched = false;
    public bool kicked = false;
    public bool blocked = false;

    [SerializeField]
    GameObject gamePlayPanel1;

    [SerializeField]
    GameObject gamePlayPanel2;

    [SerializeField]
    GameObject gamePlayPanel3;

    [SerializeField]
    GameObject gamePlayMenu; 

    CaptainsMessNetworkManager mgr;
    UIControls uicontrols;
    GameSession gameSess;

    void Start()
    {
        mgr = GameObject.Find("CaptainsMessNetworkManager").GetComponent<CaptainsMessNetworkManager>();
        uicontrols = GameObject.Find("UIControls").GetComponent<UIControls>();        
        gamePlayPanel1.SetActive(false);
        gamePlayPanel2.SetActive(false);
        gamePlayPanel3.SetActive(false);

    }

    void Update()
    {
        if (GameObject.Find("GameSession") != null)
        {
            gameSess = GameObject.Find("GameSession").GetComponent<GameSession>();
           
        }
        
    }


    public void punchButton() {
        punched = true;
    }

    public void kickButton()
    {
        kicked = true;
    }

    public void blockButton()
    {
        blocked = true;
    }

    public void GamePlayQuit() {
        gamePlayPanel1.SetActive(true);
        gamePlayPanel2.SetActive(false);
        gamePlayPanel3.SetActive(false);
    }


    public void QuitYes()
    {
        
        gameSess.resetGame();        
        gameSess.players.Clear();
        gamePlayPanel1.SetActive(false);
        gamePlayPanel2.SetActive(false);
        gamePlayPanel3.SetActive(false);
        gamePlayMenu.SetActive(false);
        mgr.Cancel();        
        uicontrols.ResetMainMenu();
        mgr.spawnLocationCounter = 0;
    }

    public void QuitNo()
    {
        gamePlayPanel1.SetActive(false);
        gamePlayPanel2.SetActive(false);
        gamePlayPanel3.SetActive(false);
    }

    public void Leave() {
        
        gameSess.resetGame();        
        gameSess.players.Clear();
        gamePlayPanel1.SetActive(false);
        gamePlayPanel2.SetActive(false);
        gamePlayPanel3.SetActive(false);
        gamePlayMenu.SetActive(false);        
        mgr.Cancel();        
        uicontrols.ResetMainMenu();
        mgr.spawnLocationCounter = 0;

    }

    public void PlayAgain() {

        gamePlayPanel1.SetActive(false);
        gamePlayPanel2.SetActive(false);
        gamePlayPanel3.SetActive(false);

    }

    IEnumerator checkForRemainingPlayerObjects() {

        yield return new WaitForSeconds(2);
        List<NetworkPlayer> players = new List<NetworkPlayer>();
        players.AddRange(GameObject.FindObjectsOfType<NetworkPlayer>());
        if (players[0].gameObject != null)
        {
            NetworkServer.Destroy(players[0].gameObject);
        }

        if (players[1].gameObject != null)
        {
            NetworkServer.Destroy(players[1].gameObject);
        }

        players.Clear();
    }


}
