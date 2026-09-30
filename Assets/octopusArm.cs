using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class octopusArm : MonoBehaviour
{
    [SerializeField] private Transform rootBone;
    private Vector3 targetRotation;
    private float speed;
    [SerializeField] private float startingSpeed;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float attackChance;
    [SerializeField] private ParticleSystem effect;

    private bool isAttacking;
    // Start is called before the first frame update
    void Start()
    {
        targetRotation = new Vector3(0, 0, 0);
        speed = 0.25f;
    }

    // Update is called once per frame
    void Update()
    {
        rootBone.localRotation = Quaternion.RotateTowards(rootBone.localRotation, Quaternion.Euler(targetRotation), speed);
        if (rootBone.transform.localRotation == Quaternion.Euler(targetRotation))
        {
            if (!isAttacking)
            {
                Debug.Log("newTarget");
                GetNewTarget();
            }
            else
            {
                Debug.Log("ATTACK");
                Attack();
            }
        }
    }

    private void GetNewTarget()
    {
        float a = Random.Range(0, attackChance);
        if (a > 2)
        {
            speed = startingSpeed;
            targetRotation = new Vector3(Random.Range(-20, 20), Random.Range(-20, 20), Random.Range(-20, 20));
        }
        else
        {
            ChargeAttack();
        }
        
    }

    private void Attack()
    {
        speed = attackSpeed;
        targetRotation = new Vector3(150, 0, 0);
        isAttacking = false;
        effect.Play();
    }
    private void ChargeAttack()
    {
        targetRotation = new Vector3(-30, 0, 0);
        isAttacking = true;
        speed = startingSpeed;
    }
}
