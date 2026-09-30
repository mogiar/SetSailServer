using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : Unit
{
    //Exp
    private int Level;
    private float currentEXP;
    private float expNeeded;


    //Sound
    private AudioSource _as;
    [SerializeField] private AudioClip levelUpAudio;
    [SerializeField] private AudioClip[] hitRecieved;


    public override void Start()
    {
        //maxHP = 100;
        //armorValue = 0;
        //attackValue = 1; // test
        base.Start();
        Level = 1;
        expNeeded = 3f;
        _as = GetComponent<AudioSource>();


    }
    public void AddExp(float amount)
    {
        currentEXP += amount;
        GeneralFunctions.GameManager.FloatingText("+" + amount.ToString(), Color.green, GeneralFunctions.Player.transform.position);

        //check if level up
        if (currentEXP >= expNeeded)
        {
            LevelUp();
        }
    }

    public void RemoveExp(float amount)
    {
        currentEXP -= amount;
        GeneralFunctions.GameManager.FloatingText("-" + amount.ToString(), Color.green, GeneralFunctions.Player.transform.position);
        //add if below 0, update ui exp to 0
    }

    private void LevelUp()
    {
        GeneralFunctions.GameManager.FloatingText("LEVEL UP!", Color.yellow, GeneralFunctions.Player.transform.position);
        currentEXP = currentEXP - expNeeded;
        expNeeded = expNeeded * 2;
        attackValue += 10f;
        Level += 1;
        //_as.clip = levelUpAudio;
        _as.PlayOneShot(levelUpAudio);
        Debug.Log("Level " + Level + " Next level exp needed " + expNeeded);
    }

    public int GetPlayerLevel()
    {
        return Level;
    }

    public void StartingState()
    {
        Level = 1;
        currentEXP = 0f;
        expNeeded = 100f;
    }

    public override void Encounter(Visitor attacker)
    {
        float damage = attacker.Attack(this);
        currentHP += Mathf.Min(0, armorValue - damage);
        healthBarTransform.Find("Bar").localScale = new Vector3((currentHP / maxHP), 1);
        _as.PlayOneShot(hitRecieved[Random.Range(0, hitRecieved.Length)]);
    }
    public override float Attack(FishScript fish)
    {// player tell fish how much damage he will do to him
        return 0;
    }
    public override float Attack(SharkScript shark)
    {// player tell shark how much damage he will do to him
        return 0;
    }

    public float GetPlayerDamage()
    {
        return attackValue;
    }
    public override void Death()
    {
        //base.Death();
        Debug.Log("dead");
        GetComponent<PlayerControls>().OpenEndGameUI();
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        //Debug.Log("OnCollisionEnter");
        if (collision.gameObject.layer == 13 || collision.gameObject.layer == 7)
        {
            //Debug.Log(collision.gameObject.name);
            float damage = Mathf.Min(GetComponent<Rigidbody>().velocity.magnitude * 30f, 20f);
            currentHP -= damage;
            healthBarTransform.Find("Bar").localScale = new Vector3((currentHP / maxHP), 1);
            _as.PlayOneShot(hitRecieved[Random.Range(0, hitRecieved.Length)]);
            GeneralFunctions.GameManager.FloatingText("-" + (int)damage, Color.red, transform.position);
        }
    }

    public void Heal(float heal)
    {
        if (currentHP < maxHP)
        {
            currentHP += heal;
        }
        else
        {
            return;
        }

        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }

        healthBarTransform.Find("Bar").localScale = new Vector3((currentHP / maxHP), 1);

    }


}
