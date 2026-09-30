using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropScript : MonoBehaviour
{
    public Drop drop;
    private Animator anim;
    public int amount;
    public Transform image;

    private void Start()
    {
        anim = GetComponent<Animator>();
        //image = gameObject.transform.GetChild(0);
        gameObject.GetComponentInChildren<SpriteRenderer>().sprite = drop.image;
        StartCoroutine("Timer");
    }

    public void Init()
    {
        if (drop != null)
        {
            if (anim == null)
                anim = GetComponent<Animator>();
            gameObject.GetComponentInChildren<SpriteRenderer>().sprite = drop.image;
            anim.enabled = true;
        }
        

    }
                       
    private void Update()
    {
        gameObject.transform.GetChild(0).LookAt(Camera.main.transform);

    }

    public void UseAbility()
    {
        drop.UseAbility();
    }
    public void TurnOffAnimation()
    {
        anim.enabled = false;
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(30);
        GeneralFunctions.Pooler.ReturnObjectToPool(this.transform.parent.gameObject);
    }

}
