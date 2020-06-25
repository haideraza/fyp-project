using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBehaviour : MonoBehaviour
{
    /*
    [SerializeField]
    GameObject healthFillBar;

    RectTransform healthRect;

    float yComponent = 0f;
    float origionalHealth = 100;
    float updatedHealth;
    int hitRecieved = 0;

    float healthX, healthPosX;

    */

    Animator anim;
    bool isColliding = false;

    
    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();       

        /*
        healthRect = healthFillBar.GetComponent<RectTransform>();
        updatedHealth = origionalHealth;
        healthX = healthRect.sizeDelta.x;
        healthPosX = healthRect.anchoredPosition.x;      
        */
    }

    // Update is called once per frame
    void Update()
    {
      
        if (anim.GetInteger("HitReactionValue") == 1 && !isColliding) {

            /*
            Debug.Log("HitCollider");
            updatedHealth -= 10f;
            healthRect.sizeDelta = new Vector2(healthX * (updatedHealth / origionalHealth), healthRect.sizeDelta.y);
            float x = (healthX -(healthX * (updatedHealth / origionalHealth))) / 2;
            healthRect.anchoredPosition= new Vector2(healthPosX-x, healthRect.anchoredPosition.y);
            */
            isColliding = true;
        }

    }

    void OnTriggerEnter(Collider collider)
    {     
        if (collider.gameObject.tag.Equals("HitCollider"))
        {            
            anim.SetInteger("HitReactionValue", 1);                        
            StartCoroutine(backToIdleAnim());            
        }        
    }

    //private void OnTriggerExit(Collider collider){}

    IEnumerator backToIdleAnim() {

        yield return new WaitForSeconds(1);
        anim.SetInteger("HitReactionValue", 0);
        isColliding = false;

    }
    
}
