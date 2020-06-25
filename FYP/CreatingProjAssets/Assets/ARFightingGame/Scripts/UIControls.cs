using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIControls : MonoBehaviour
{

    public bool Btn1Clicked = false;
    public bool Btn2Clicked = false;
    public bool Btn3Clicked = false;
    public bool ReadyBtnClicked = false;

    NetworkControls netCtrl;
    NetworkPlayer netPlayer; 

    [SerializeField]
    GameObject netDebugString;
    Text strVal;

    [SerializeField]
    Button btn1;

    [SerializeField]
    Button btn2;

    [SerializeField]
    Button btn3;

    [SerializeField]
    Button backBtn;

    [SerializeField]
    Button readyBtn;

    [SerializeField]
    Button[] charBtn = new Button[4];
    
    Text readyBtnText;
    public string readyBtnTextString;

    [SerializeField]
    public GameObject playerMatchPrefab;
    
    public Transform prefabContainer;

    [SerializeField]
    GameObject TitleMenu;

    [SerializeField]
    GameObject ARCoreMenu;

    [SerializeField]
    GameObject startMenu;

    [SerializeField]
    GameObject mainMenu;

    public int playerClass = 0;

    CaptainsMessNetworkManager mgr;

    GameSession gameSess;

    void Start()
    {
        TitleMenu.SetActive(true);
        ARCoreMenu.SetActive(false);
        startMenu.SetActive(false);
        mainMenu.SetActive(false);
        mgr = GameObject.Find("CaptainsMessNetworkManager").GetComponent<CaptainsMessNetworkManager>();
        netCtrl = GameObject.FindGameObjectWithTag("NetMgr").GetComponent<NetworkControls>();        
        strVal = netDebugString.GetComponent<Text>();
        readyBtnText = readyBtn.transform.GetChild(0).GetComponent<Text>();
        readyBtn.gameObject.SetActive(false);
        readyBtnTextString = "Ready";
        prefabContainer = GameObject.Find("MainMenu").transform.GetChild(0).GetChild(2).transform;
        toggleOnOffButtons(0);        

    }

    void Update()
    {

        if (GameObject.Find("GameSession") != null) {
            gameSess = GameObject.Find("GameSession").GetComponent<GameSession>();
        }        

        strVal.text = netCtrl.NetworkDebugString();
        btn3.transform.GetChild(0).GetComponent<Text>().text = netCtrl.button3Text;
        readyBtnText.text = readyBtnTextString;

        if (GameObject.FindObjectOfType<NetworkGameSession>()==null)
        {
            Transform gameplayPanel = GameObject.Find("GameplayMenu").transform.GetChild(0);
            
            if (gameplayPanel.gameObject.activeInHierarchy && !gameSess.gameFinished)
            {
                gameplayPanel.GetChild(9).gameObject.SetActive(true);
            }
            
        }

    }

    public GameObject createPlayersInfo() {
        
        return Instantiate(playerMatchPrefab, prefabContainer);
        
    }


    public void ResetMainMenu() {

        mainMenu.SetActive(true);
        btn1.interactable = true;
        btn2.interactable = true;
        btn3.interactable = true;
        backBtn.interactable = true;
        readyBtn.gameObject.SetActive(false);
        readyBtnTextString = "Ready";
        for (int i = 0; i < prefabContainer.childCount; i++) {
            Destroy(prefabContainer.GetChild(i).gameObject);
        }
            
    }

    public void btn1Action()
    {
        Btn1Clicked = true;
        btn1.interactable = false;
        btn2.interactable = false;
        backBtn.interactable = false;
    }

    public void btn2Action()
    {
        Btn2Clicked = true;        
        btn1.interactable = false;
        btn2.interactable = false;
        backBtn.interactable = false;
    }

    public void btn3Action()
    {
        if (netCtrl.button3Text.Equals("Auto Connect"))
        {
            btn1.interactable = false;
            btn2.interactable = false;
            backBtn.interactable = false;
        }

        else {

            btn1.interactable = true;
            btn2.interactable = true;
            backBtn.interactable = true;
            mgr.spawnLocationCounter = 0;
        }

        Btn3Clicked = true;
        
    }

    void toggleOnOffButtons(int index) {

        for (int i = 0; i < charBtn.Length; i++)
        {
            if (i == index)
            {
                charBtn[i].interactable = false;
            }
            else
            {
                charBtn[i].interactable = true;
            }
        }

    }    

    public void selectCharacter1()
    {
        toggleOnOffButtons(0);
        playerClass = 0;
    }

    public void selectCharacter2()
    {
        toggleOnOffButtons(1);
        playerClass = 1;
    }

    public void selectCharacter3()
    {
        toggleOnOffButtons(2);
        playerClass = 2;
    }

    public void selectCharacter4()
    {
        toggleOnOffButtons(3);
        playerClass = 3;
    }

    public void TitleMenuStartBtnAction()
    {
        TitleMenu.SetActive(false);
        ARCoreMenu.SetActive(true);
        startMenu.SetActive(false);
        mainMenu.SetActive(false);
    }


    public void ARCoreMenuProceedBtnAction()
    {
        TitleMenu.SetActive(false);
        ARCoreMenu.SetActive(false);
        startMenu.SetActive(true);
        mainMenu.SetActive(false);
    }


    public void proceedBtnAction() {

        TitleMenu.SetActive(false);
        ARCoreMenu.SetActive(false);
        startMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void backBtnAction()
    {
        TitleMenu.SetActive(false);
        ARCoreMenu.SetActive(false);
        startMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void backToARCoreMenuBtnAction()
    {
        TitleMenu.SetActive(false);
        ARCoreMenu.SetActive(true);
        startMenu.SetActive(false);
        mainMenu.SetActive(false);
    }

    public void backToTitleMenu()
    {
        TitleMenu.SetActive(true);
        ARCoreMenu.SetActive(false);
        startMenu.SetActive(false);
        mainMenu.SetActive(false);
    }


    public void readybtnAction()
    {
        ReadyBtnClicked = true;
    }

    public void readybtnVisible(bool value)
    {
        readyBtn.gameObject.SetActive(value);
    }

    public void Quit()
    {
        Application.Quit();
    }

}