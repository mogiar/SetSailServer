using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Golem : Enemy
{
    private Transform Rotater;
    private float attackTimer;
    private Animator anim;
    [SerializeField] private float attackQueue;
    private Transform Head;
    

    // Start is called before the first frame update
    void Start()
    {
        Rotater = transform.Find("Rig 1").Find("Rotation");
        anim = transform.GetComponent<Animator>();
        AddEvent(2, 0f, "AttackEnd", 0f);
        AddEvent(0, 0f, "AttackEnd", 0f);
        //Debug.Log(anim.runtimeAnimatorController.animationClips[2].name);
        Head = transform.Find("EarthElementalTop");
        PossibleDrops = new List<string>();
        PossibleDrops.Add("GoldBar,5");
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, GeneralFunctions.Player.transform.position) < 50f)
        {
            //Rotate to target
            Rotater.LookAt(GeneralFunctions.Player.transform);
            Rotater.GetComponent<MultiRotationConstraint>().weight = Mathf.Lerp(Rotater.GetComponent<MultiRotationConstraint>().weight, 1, Time.deltaTime);

            if(attackTimer <= attackQueue)
                attackTimer += Time.deltaTime;

            if (attackTimer > attackQueue)
            {
                //attack
                if (Vector3.Distance(transform.position, GeneralFunctions.Player.transform.position) < 10f)
                {
                    anim.SetBool("MeleeAttack", true);
                }
                else
                {
                    anim.SetBool("ThrowAttack", true);
                }                
            }
        }
        else
        {
            if (attackTimer > 0)
            {
                attackTimer -= Time.deltaTime;
            }
            AttackEnd();
            Rotater.GetComponent<MultiRotationConstraint>().weight = Mathf.Lerp(Rotater.GetComponent<MultiRotationConstraint>().weight, 0, Time.deltaTime);
        }
        
    }

    void AddEvent(int Clip, float time, string functionName, float floatParameter)
    {
        AnimationEvent animationEvent = new AnimationEvent();
        animationEvent.functionName = functionName;
        animationEvent.floatParameter = floatParameter;
        animationEvent.time = time;
        AnimationClip clip = anim.runtimeAnimatorController.animationClips[Clip];
        clip.AddEvent(animationEvent);
    }

    private void AttackEnd()
    {
        anim.SetBool("MeleeAttack", false);
        anim.SetBool("ThrowAttack", false);
    }
    public void ShootRock()
    {
        GameObject ammo = GeneralFunctions.Pooler.SpawnFromPool("golemrock", new Vector2(Rotater.position.x, Rotater.position.z + 7), new Quaternion(Rotater.eulerAngles.x, Rotater.eulerAngles.y, Rotater.eulerAngles.z, 0));

    }
}
