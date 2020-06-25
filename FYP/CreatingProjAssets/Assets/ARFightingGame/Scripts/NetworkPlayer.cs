using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class HealthBarStuff
{

    RectTransform healthRect;
    float origionalHealth = 100;
    public float updatedHealth;
    int hitRecieved = 0;
    float healthX, healthPosX;
    public float healthReductionFactor = 10f;

    public void initializeObjects(GameObject healthFillBar)
    {
        healthRect = healthFillBar.GetComponent<RectTransform>();
        updatedHealth = origionalHealth;
        healthX = healthRect.sizeDelta.x;
        healthPosX = healthRect.anchoredPosition.x;
    }

    public void updateHealthStatus()
    {
        updatedHealth -= healthReductionFactor;
        healthRect.sizeDelta = new Vector2(healthX * (updatedHealth / origionalHealth), healthRect.sizeDelta.y);
        float x = (healthX - (healthX * (updatedHealth / origionalHealth))) / 2;
        healthRect.anchoredPosition = new Vector2(healthPosX - x, healthRect.anchoredPosition.y);
    }

    public void resetHealth()
    {
        updatedHealth = origionalHealth;
        healthRect.sizeDelta = new Vector2(healthX, healthRect.sizeDelta.y);
        healthRect.anchoredPosition = new Vector2(healthPosX, healthRect.anchoredPosition.y);
    }
}


public class NetworkPlayer : CaptainsMessPlayer
{
    Animator animator;

    [SerializeField]
    float turnSpeed = 1.5f;

    [SerializeField]
    float moveSpeed = 0.2f;

    FixedJoystick joystick;

    [SerializeField]
    float actionWaitTime = 0.5f;

    UserHUDControls userHudControls;
    UIControls UIControls;

    bool isColliding = false;

    GameObject playerInfoObject;

    Text readyField;

    GameObject[] playerSpawnPos=new GameObject[2];
    
    GameObject playerHealthFillBar, enemyHealthFillBar;

    public HealthBarStuff playerHealth, enemyHealth;

    [SerializeField]
    Material indicatorMat1;

    [SerializeField]
    Material indicatorMat2;

    int animMin, animMax;

    /// <summary>
    /// new stuff
    /// </summary>

    [SyncVar]
    public bool enemyIsAttacking = false;

    public bool endReactionCompleted = false;

    public bool winner = false;
    public int score = 0;

    /// <summary>
    /// new stuff
    /// </summary>


    void Awake()
    {
        UIControls = GameObject.Find("UIControls").GetComponent<UIControls>();
    }

    // Start is called before the first frame update
    void Start()
    {                
        joystick = GameObject.Find("GameplayMenu").transform.GetChild(0).GetChild(3).GetComponent<FixedJoystick>();
        animator = GetComponent<Animator>();
        userHudControls = GameObject.Find("UserHUDControls").GetComponent<UserHUDControls>();
        

        playerHealth = new HealthBarStuff();
        enemyHealth = new HealthBarStuff();

        playerHealthFillBar = GameObject.Find("GameplayMenu").transform.GetChild(0).GetChild(4).GetChild(1).gameObject;
        playerHealth.initializeObjects(playerHealthFillBar);

        enemyHealthFillBar = GameObject.Find("GameplayMenu").transform.GetChild(0).GetChild(5).GetChild(1).gameObject;
        enemyHealth.initializeObjects(enemyHealthFillBar);


        if (isLocalPlayer)
        {            
            transform.Find("Indicator").gameObject.GetComponent<MeshRenderer>().material = indicatorMat1;
        }
        else
        {                       
            transform.Find("Indicator").gameObject.GetComponent<MeshRenderer>().material = indicatorMat2;
        }

        playerSpawnPos[0] = GameObject.FindGameObjectWithTag("SpawnPos1").gameObject;
        playerSpawnPos[1] = GameObject.FindGameObjectWithTag("SpawnPos2").gameObject;

        
    }    

    public override void OnStartLocalPlayer()
    {
        animationSelector();
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
        gameObject.GetComponent<CapsuleCollider>().enabled = false;
        //transform.SetParent(GameObject.FindGameObjectWithTag("FightGround").transform);
        //CmdSetPlayerPos();
        base.OnStartLocalPlayer();
        
    }

    public override void OnStartClient()
    {
        GameObject.Find("GameSession").GetComponent<GameSession>().addPlayerToList(this);
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
        gameObject.GetComponent<CapsuleCollider>().enabled = false;
        //transform.SetParent(GameObject.FindGameObjectWithTag("FightGround").transform);
        //CmdSetPlayerPos();
        base.OnStartClient();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }

       

        NetworkGameSession gameSession = NetworkGameSession.instance;
        if (gameSession)
        {
            if (gameSession.gameState == GameState.Lobby ||
                gameSession.gameState == GameState.Countdown)
            {
                UIControls.readybtnVisible(true);

                if (UIControls.ReadyBtnClicked)
                {

                    if (IsReady())
                    {
                        UIControls.readyBtnTextString = "Ready";
                        SendNotReadyToBeginMessage();
                        readyField.text = "Not Ready";
                        readyField.color = Color.red;
                    }
                    else
                    {
                        UIControls.readyBtnTextString = "Not Ready";
                        SendReadyToBeginMessage();
                        readyField.text = "Ready";
                        readyField.color = Color.green;
                    }

                    UIControls.ReadyBtnClicked = false;
                }

            }

            else if (gameSession.gameState == GameState.GameOver)
            {

                if (isServer)
                {
                    // CmdPlayAgain();
                }
            }
            
            else
            {

                UIControls.readybtnVisible(false);

            }
            
        }

        move();

        punch();

        block();

        kick();

        hitRecieveReaction();
        
        
        
    }
    
    void move() {
        
        Vector2 movementInput = new Vector2(joystick.Horizontal, joystick.Vertical);
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 desiredMoveDirection = (forward * movementInput.y + right * movementInput.x);
        
        animator.SetFloat("Move", desiredMoveDirection.magnitude);

        transform.position = transform.position + desiredMoveDirection * Time.deltaTime * moveSpeed;

        if (desiredMoveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), turnSpeed);
        }
        
    }
    
    void punch() {

        if (userHudControls.punched) {

            int value = Random.Range(animMin, animMax);
            animator.SetInteger("PunchValue", value);
            StartCoroutine(backToIdleAnim("PunchValue"));
            Debug.Log("Punched");
            userHudControls.punched = false;

        }
       
    }

    void kick(){

        if (userHudControls.kicked)
        {
            int value = Random.Range(animMin, animMax);
            animator.SetInteger("KickValue", value);            
            StartCoroutine(backToIdleAnim("KickValue"));
            Debug.Log("kicked");
            userHudControls.kicked = false;
        }
    }

    void block() {

        if (userHudControls.blocked)
        {
            int value = Random.Range(1, 2);
            animator.SetInteger("BlockValue", value);
            StartCoroutine(backToIdleAnim("BlockValue"));
            Debug.Log("Blocked");
            userHudControls.blocked = false;
        }
    }

    void hitRecieveReaction() {

        if ((animator.GetInteger("HitReactionValue") == 1 || animator.GetInteger("EndReactionValue") == 1) && !isColliding)
        {
            playerHealth.updateHealthStatus();
            CmdUpdateEnemyHealth();
            isColliding = true;
        }

    }

    public void EndReaction() {
        Debug.Log("Celebration started");
        animator.SetInteger("EndReactionValue", 2);
        StartCoroutine(backToIdleAnim("EndReactionValue"));
    }

    void animationSelector() {

        if (UIControls.playerClass == 0)
        {
            animMin = 1;
            animMax = 4;
        }

        else if (UIControls.playerClass == 1)
        {
            animMin = 3;
            animMax = 6;
        }

        else if (UIControls.playerClass == 2)
        {
            animMin = 5;
            animMax = 8;
        }

        else if (UIControls.playerClass == 3)
        {
            animMin = 7;
            animMax = 10;
        }


    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag.Equals("HitCollider") )
        {
            if (playerHealth.updatedHealth != playerHealth.healthReductionFactor)
            {
                animator.SetInteger("HitReactionValue", 1);
                StartCoroutine(backToIdleAnim("HitReactionValue"));
            }
            else
            {
                animator.SetInteger("EndReactionValue", 1);
            }
        }
    }

    IEnumerator backToIdleAnim(string value)
    {
        yield return new WaitForSeconds(actionWaitTime);
        animator.SetInteger(value, 0);
        isColliding = false;

        if (value.Equals("HitReactionValue"))
        {
            enemyIsAttacking = false;
        }

        if (value.Equals("EndReactionValue"))
        {
            yield return new WaitForSeconds(actionWaitTime);
            endReactionCompleted = true;
        }
    }

    [Command]
    public void CmdPlayAgain()
    {
        NetworkGameSession.instance.PlayAgain();
    }

    public override void OnClientEnterLobby()
    {
        base.OnClientEnterLobby();        
        Invoke("ShowPlayer", 0.5f);
    }

    public override void OnClientReady(bool readyState)
    {
        if (readyField != null) {
            if (readyState)
            {
                readyField.text = "Ready";
                readyField.color = Color.green;
            }
            else
            {
                readyField.text = "Not Ready";
                readyField.color = Color.red;
            }
        }
        
    }

    void ShowPlayer()
    {       
        SpawnObj();       
        OnClientReady(IsReady());
    }

    public void SpawnObj() {
        playerInfoObject = UIControls.createPlayersInfo();
        NetworkServer.Spawn(playerInfoObject);
        readyField = playerInfoObject.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>();
        playerInfoObject.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = deviceName;
    }

    [Command]
    public void CmdUpdateEnemyHealth()
    {
        RpcUpdateEnemyHealth();
    }

    [ClientRpc]
    public void RpcUpdateEnemyHealth()
    {
        if (!isLocalPlayer)
        {
            enemyHealth.updateHealthStatus();
        }
    }
    

    [ClientRpc]
    public void RpcOnStartedGame()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(2).gameObject.SetActive(true);
        GetComponent<CapsuleCollider>().enabled = true;        
    }

    /*
    [Command]
    void CmdSetPlayerPos()
    {
        playerSpawnPos1 = GameObject.Find("PlayerSpawnPos1");
        playerSpawnPos2 = GameObject.Find("PlayerSpawnPos2");

        if (isServer)
        {
            transform.position = playerSpawnPos2.transform.position;
            transform.rotation = playerSpawnPos2.transform.rotation;
        }

        else if (isClient)
        {
            transform.position = playerSpawnPos1.transform.position;
            transform.rotation = playerSpawnPos1.transform.rotation;
        }

    }

    */


    public override void OnClientExitLobby()
    {
        base.OnClientExitLobby();
        //NetworkServer.Destroy(playerInfoObject);
    }


    //// new stuff

    public bool playAgain;

    [Command]
    public void CmdSendReplayInfo(bool x)
    {

        RpcSendReplayInfo(x);
    }

    [ClientRpc]
    public void RpcSendReplayInfo(bool x)
    {
        playAgain = x;
    }

    public float myPlayerHealth;

    [Command]
    public void CmdSendHealthInfo(float x)
    {

        RpcSendHealthInfo(x);
    }

    [ClientRpc]
    public void RpcSendHealthInfo(float x)
    {
        myPlayerHealth = x;
    }

    [Command]
    public void CmdResetPlay(int spawnPosIndex)
    {
        RpcResetPlay(spawnPosIndex);
    }

    [ClientRpc]
    public void RpcResetPlay(int spawnPosIndex)
    {
        if (isLocalPlayer)
        {
            playerHealth.resetHealth();
        }
        else
        {
            enemyHealth.resetHealth();
        }

        gameObject.transform.position = playerSpawnPos[spawnPosIndex].transform.position;
        gameObject.transform.rotation = playerSpawnPos[spawnPosIndex].transform.rotation;
        animator.SetInteger("EndReactionValue", 0);
    }

    [Command]
    public void CmdShowHidePlayer(bool x) {

        RpcShowHidePlayer(x);

    }

    [ClientRpc]
    public void RpcShowHidePlayer(bool x)
    {
        transform.GetChild(0).gameObject.SetActive(x);
        transform.GetChild(2).gameObject.SetActive(x);
        gameObject.GetComponent<CapsuleCollider>().enabled = x;

    }


    ///// new stuff

}
