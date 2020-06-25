using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameSession : NetworkBehaviour
{

    public List<NetworkPlayer> players = new List<NetworkPlayer>();
    bool roundFinished = false;
    public bool gameFinished = false;
    bool infoUpdated = false;
    bool playResetted = false;
    int roundNo = 1;

    Text roundNoText;
    Text playerScoreText;
    Text enemyScoreText;

    GameObject roundPanel;
    GameObject playAgainPanel;

    Text roundPanelMsg;
    Text playAgainPanelMsg;

    Button playAgainYesBtn;
    GameObject playMenu;

    void Start()
    {

        playMenu = GameObject.Find("GameplayMenu").transform.GetChild(0).gameObject;

        roundNoText = playMenu.transform.GetChild(10).GetChild(0).GetComponent<Text>();
        playerScoreText = playMenu.transform.GetChild(10).GetChild(1).GetComponent<Text>();
        enemyScoreText = playMenu.transform.GetChild(10).GetChild(2).GetComponent<Text>();
        roundPanel = playMenu.transform.GetChild(11).gameObject;
        playAgainPanel = playMenu.transform.GetChild(8).gameObject;
        roundPanelMsg = roundPanel.transform.GetChild(0).GetChild(0).GetComponent<Text>();
        playAgainPanelMsg = playAgainPanel.transform.GetChild(0).GetChild(0).GetComponent<Text>();
        playAgainYesBtn = playAgainPanel.transform.GetChild(0).GetChild(2).GetComponent<Button>();
        roundPanel.SetActive(false);
        playAgainPanel.SetActive(false);
    }

    void Update()
    {
        
        //GameObject.Find("TTT").GetComponent<Text>().text = "No of players connected: " + players.Count;
                                                           
        if (players.Count == 2)
        {



            if (roundNo <= 3)
            {


                //if (players.Count == 2)
                //{

                startGame();

                if (roundFinished)
                {
                    StartCoroutine(showRoundEndPanel());
                }
                else
                {

                    roundPanel.SetActive(false);

                }

                if (players[0].myPlayerHealth > 0 && players[1].myPlayerHealth > 0)
                {

                    if (infoUpdated)
                    {

                        infoUpdated = false;
                        roundFinished = false;

                    }

                }


                /// }

            }


            else
            {

                playAgainPanel.SetActive(true);
                gameFinished = true;

                if (players[0].score > players[1].score)
                {

                    if (players[0].isLocalPlayer)
                    {
                        playAgainPanelMsg.text = "You Win";
                    }
                    if (players[1].isLocalPlayer)
                    {
                        playAgainPanelMsg.text = "You Lose";
                    }

                }

                if (players[1].score > players[0].score)
                {

                    if (players[1].isLocalPlayer)
                    {
                        playAgainPanelMsg.text = "You Win";
                    }
                    if (players[0].isLocalPlayer)
                    {
                        playAgainPanelMsg.text = "You Lose";
                    }

                }

            }

            if (players[0].playAgain && players[1].playAgain)
            {

                resetGame();

            }

            
            updateUI_Info();

            if (playResetted) {
                StartCoroutine(waitForPlayerToReturn());
            }
              
        }
    }

    void startGame()
    {

        string str1 = "";

        foreach (NetworkPlayer p in players)
        {
            p.CmdSendHealthInfo(p.playerHealth.updatedHealth);
            str1 += " \n My Player " + (players.IndexOf(p) + 1) + " health : " + p.myPlayerHealth;
        }

        StartCoroutine(startFightRound());

    }

    IEnumerator startFightRound()
    {

        yield return new WaitForSeconds(0.1f);

        if (players[0].myPlayerHealth <= 0)
        {
            players[0].winner = false;
            players[1].winner = true;
            players[1].EndReaction();

            if (players[1].endReactionCompleted)
            {
                roundFinished = true;
            }
        }

        else if (players[1].myPlayerHealth <= 0)
        {

            players[0].winner = true;
            players[1].winner = false;
            players[0].EndReaction();

            if (players[0].endReactionCompleted)
            {
                roundFinished = true;
            }

        }

    }

    IEnumerator showRoundEndPanel()
    {

        if (!roundPanel.activeInHierarchy)
        {

            roundPanel.SetActive(true);

            if (players[0].winner)
            {

                if (players[0].isLocalPlayer)
                {
                    roundPanelMsg.text = "Yeah beat'em up";
                }
                if (players[1].isLocalPlayer)
                {
                    roundPanelMsg.text = "You lost this round";
                }

            }

            if (players[1].winner)
            {

                if (players[1].isLocalPlayer)
                {
                    roundPanelMsg.text = "Yeah beat'em up";
                }
                if (players[0].isLocalPlayer)
                {
                    roundPanelMsg.text = "You lost this round";
                }

            }

        }

        yield return new WaitForSeconds(6f);

        if (!infoUpdated && roundFinished)
        {

            if (roundNo <= 3)
            {

                if (players[0].winner)
                {
                    players[0].score = players[0].score + 1;
                }

                else if (players[1].winner)
                {
                    players[1].score = players[1].score + 1;
                }


                roundNo++;


                infoUpdated = true;
                resetPlay();
                

            }

        }

    }


    void resetPlay()
    {
        foreach (NetworkPlayer p in players)
        {
            p.CmdShowHidePlayer(false);
            p.CmdResetPlay(players.IndexOf(p));
        }
        
        playResetted = true;
        
    }

    IEnumerator waitForPlayerToReturn() {

        yield return new WaitForSeconds(2);
        foreach (NetworkPlayer p in players)
        {
            p.CmdShowHidePlayer(true);
        }
        playResetted = false;
        roundPanel.SetActive(false);
    }


    public void resetGame()
    {

        foreach (NetworkPlayer p in players)
        {
            p.CmdResetPlay(players.IndexOf(p));
            p.winner = false;
            p.score = 0;
        }

        roundNo = 1;

        roundPanel.SetActive(false);
        playAgainYesBtn.interactable = true;
        playAgainPanel.SetActive(false);
        infoUpdated = false;
        roundFinished = false;        
        players[0].CmdSendReplayInfo(false);
        players[1].CmdSendReplayInfo(false);
        
    }

    void updateUI_Info()
    {

        if (roundNo <= 3)
        {

            roundNoText.text = "Round No : " + roundNo.ToString();
        }


        if (players[0].isLocalPlayer)
        {

            playerScoreText.text = players[0].score.ToString();
            enemyScoreText.text = players[1].score.ToString();
        }
        else if (players[1].isLocalPlayer)
        {
            playerScoreText.text = players[1].score.ToString();
            enemyScoreText.text = players[0].score.ToString();
        }

        /*
        GameObject.Find("TTT").GetComponent<Text>().text = " p1 playagain  : " + players[0].playAgain +
                                                           "\n p2 playagain  : " + players[1].playAgain +
                                                           " info updated : " + infoUpdated + 
                                                           ", P1 health:" + players[0].myPlayerHealth;
                          // "\n game finished : " + gameFinished + ", P2 health:" + players[1].myPlayerHealth;

       */
       /*
        GameObject.Find("TTT").GetComponent<Text>().text = "p1 pos:" + players[0].transform.position +
                                                               "\np2 pos:" + players[1].transform.position +
                                                               "\np1 local pos:" + players[0].transform.localPosition +
                                                               "\np2 local pos:" + players[1].transform.localPosition +
                                                               "\np1 parent:" + players[0].transform.parent +
                                                               "\np2 parent:" + players[1].transform.parent;
                                                               
    */


    }

    public void addPlayerToList(NetworkPlayer p)
    {

        players.Add(p);

    }

    public void playAgainYes()
    {


        if (players[0].isLocalPlayer)
        {


            players[0].CmdSendReplayInfo(true);
            playAgainYesBtn.interactable = false;

        }

        else if (players[1].isLocalPlayer)
        {

            players[1].CmdSendReplayInfo(true);
            playAgainYesBtn.interactable = false;

        }


    }

    public void playAgainNo()
    {


    }



}

