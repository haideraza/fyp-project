using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkControls : MonoBehaviour
{
    private CaptainsMess mess;
    private CaptainsMessNetworkManager networkManager;    
    private UIControls UIControls;
    public string button3Text;
    GameSession gameSess;

    public void Awake()
    {
        mess = FindObjectOfType(typeof(CaptainsMess)) as CaptainsMess;
        networkManager = GetComponent<CaptainsMessNetworkManager>();
        UIControls = GameObject.Find("UIControls").GetComponent<UIControls>(); 
    }

    void Update() {

        if (GameObject.Find("GameSession") != null)
        {
            gameSess = GameObject.Find("GameSession").GetComponent<GameSession>();
        }

        NetworkDebugString();
        peformNetworkOperation_Btn3();
        startHost_btn1();
        Join_btn2();
        
     
    }
    
    public string NetworkDebugString()
    {
        string serverString = "[SERVER] | ";
        if (NetworkServer.active && networkManager.numPlayers > 0)
        {
            //serverString += "Hosting at " +  + "\n";
            serverString += string.Format("Players Ready = {0}/{1}", networkManager.NumReadyPlayers(), networkManager.NumPlayers()) + " | ";
        }
        if (networkManager.discoveryServer.running && networkManager.discoveryServer.isServer)
        {
            serverString += "Broadcasting: " + networkManager.discoveryServer.broadcastData + " | ";
        }

        string clientString = "[CLIENT] | ";
        if (networkManager.IsClientConnected())
        {
            clientString += "Connected to server: " + networkManager.client.connection.address + " | ";
        }
        if (networkManager.discoveryClient.running && networkManager.discoveryClient.isClient)
        {
            // Discovered servers
            clientString += "Discovered servers =";
            foreach (DiscoveredServer server in networkManager.discoveryClient.discoveredServers.Values)
            {
                bool isMe = (server.peerId == networkManager.peerId);
                clientString += " | - ";
                if (isMe)
                {
                    clientString += "(me) ";
                }
                clientString += server.ipAddress + " : " + server.rawData;
            }
            clientString += " | ";

            // Received broadcasts
            clientString += "Received broadcasts =";
            foreach (var entry in networkManager.discoveryClient.receivedBroadcastLog)
            {
                clientString += " | " + entry;
            }
            clientString += " | ";
        }

        return serverString + " | " + clientString;
    }


    void peformNetworkOperation_Btn3()
    {
        if (UIControls.Btn3Clicked)
        {

            if (networkManager.IsConnected())
            {
                button3Text = "Disconnect";
                gameSess.players.Clear();
                mess.Cancel();
                UIControls.ResetMainMenu();
                networkManager.StopHost();                            }

            else if (networkManager.IsBroadcasting() || networkManager.IsJoining())
            {
                button3Text = "Cancel";
                gameSess.players.Clear();
                UIControls.ResetMainMenu();
                mess.Cancel();                
               
            }
            else
            {
                button3Text = "Auto Connect";
                mess.AutoConnect();

            }

            UIControls.Btn3Clicked = false;
        }

        else {

            if (networkManager.IsConnected())
            {
                button3Text = "Disconnect";
            }

            else if (networkManager.IsBroadcasting() || networkManager.IsJoining())
            {
                button3Text = "Cancel";                
            }
            else
            {
                button3Text = "Auto Connect";
            }
        }
    }

    void startHost_btn1() {

        if (UIControls.Btn1Clicked)
        {
            mess.StartHosting();
            UIControls.Btn1Clicked = false;
        }
    }

    void Join_btn2()
    {
        if (UIControls.Btn2Clicked)
        {
            mess.StartJoining();
            UIControls.Btn2Clicked = false;
        }          
    }

}
