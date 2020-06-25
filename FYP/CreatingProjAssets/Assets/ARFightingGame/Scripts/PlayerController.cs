using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    [SerializeField]
    float rotSpeed;

    [SerializeField]
    float gravity;

    [SerializeField]
    float actionWaitTime = 1f;

    [SerializeField]
    Joystick joystick1;

    [SerializeField]
    Joystick joystick2;

    Animator anim;
    //CharacterController controller;
    float yComponent = 0f;
    List<GameObject> colliders;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        anim =gameObject.GetComponent<Animator>();
        //controller = gameObject.GetComponent<CharacterController>();
        colliders = new List<GameObject>();
        colliders.AddRange(GameObject.FindGameObjectsWithTag("HitCollider"));
        toggleOnOffColliders(false);       
    }

    // Update is called once per frame
    void Update()
    {        
        movement();       
    }

    void movement()
    {
        if (joystick1.Vertical>0)
        {
            anim.SetInteger("MovementValue", 1);
        }

        else if (joystick1.Vertical == 0)
        {
            anim.SetInteger("MovementValue", 0);
        }

        if (joystick1.Vertical < 0)
        {
            anim.SetInteger("MovementValue", 2);
        }

        else if (joystick1.Vertical == 0)
        {
            anim.SetInteger("MovementValue", 0);
        }

        if (joystick2.Horizontal < 0)
        {
            transform.Rotate(0, -Time.deltaTime * rotSpeed, 0);
        }

        if (joystick2.Horizontal > 0)
        {
            transform.Rotate(0, Time.deltaTime * rotSpeed, 0);
        }

        yComponent = transform.position.y;

        //if (!controller.isGrounded)
        //{
           // yComponent -= gravity * Time.deltaTime;
            //transform.Translate(0, yComponent, 0);
        //}
    }
    
    public void Punch() {

        int value = Random.Range(1, 4);
        anim.SetInteger("PunchValue", value);
        toggleOnOffColliders(true);
        StartCoroutine(backToIdleAnim("PunchValue"));
        Debug.Log("Punched");
    }

    public void Kick()
    {
        
        int value = Random.Range(1, 3);
        anim.SetInteger("KickValue", value);
        toggleOnOffColliders(true);
        StartCoroutine(backToIdleAnim("KickValue"));
        Debug.Log("kicked");
    }

    IEnumerator backToIdleAnim(string value) {

        yield return new WaitForSeconds(actionWaitTime);
        anim.SetInteger(value, 0);
        toggleOnOffColliders(false);
    } 
  
    void toggleOnOffColliders(bool value) {

        for (int i = 0; i < colliders.Count; i++)
        {
            colliders[i].GetComponent<Collider>().enabled = value;
        }

    }
    
}
