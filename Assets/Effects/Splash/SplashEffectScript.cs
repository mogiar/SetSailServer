using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashEffectScript : MonoBehaviour
{
    void OnParticleSystemStopped()
    {
        //Debug.Log("System has stopped!");
        GeneralFunctions.Pooler.ReturnObjectToPool(this.gameObject);
    }
    //private ParticleSystem SplashEffect;


    // Start is called before the first frame update
    //void Start()
    //{
    //    //SplashEffect = GetComponentInChildren<ParticleSystem>();
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    DoSplashEffect(new Vector3(Random.Range(-10,10),0, Random.Range(-10, 10)));
    //}

    //public void DoSplashEffect(Vector3 position)
    //{
    //    //ParticleSystem ps = Instantiate(SplashEffect, position, gameObject.transform.rotation);
    //    //ps.Play();
    //    //StartCoroutine(DestoryEffect(ps));
    //}

    //private IEnumerator DestoryEffect(ParticleSystem a)
    //{
    //    //yield return new WaitForSeconds(5f);
    //    //Destroy(a.gameObject);
    //}
}
