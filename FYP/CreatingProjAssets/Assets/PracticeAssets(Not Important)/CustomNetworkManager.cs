using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomNetworkManager : NetworkManager
{
    GameObject panel1, panel2;

     void Start()
    {
        panel1 = GameObject.FindGameObjectWithTag("Panel1").transform.GetChild(0).gameObject;
        panel2 = GameObject.FindGameObjectWithTag("Panel2").transform.GetChild(0).gameObject;

    }

    public void startHosting() {

        panel1.SetActive(false);
        panel2.SetActive(true);
        base.StartHost();
        
    }
}
