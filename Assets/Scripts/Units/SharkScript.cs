using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SharkScript : Enemy
{
    public override void Start()
    {
        //this.maxHP = 20;
        //this.regularSpeed = 3;
        //this.chaseSpeed = 7;
        //this.visionRange = 60;
        //this.attackRange = 10;
        //armorValue = 0;
        //attackValue = 2;
        base.Start();
        PossibleDrops.Add("SharkTeeths,2");
        PossibleDrops.Add("SharkTeeths,2");
        PossibleDrops.Add("SharkTeeths,2");
        PossibleDrops.Add("SharkTeeths,2");
        PossibleDrops.Add("Swordfish,5");
    }
    public override void Encounter(Visitor attacker)
    { // unit is the attacker 
        float damage = attacker.Attack(this);
        currentHP += Mathf.Min(0, armorValue - damage);
        healthBarTransform.Find("Bar").localScale = new Vector3((currentHP / maxHP), 1);
    }
    public override float Attack(SharkScript shark)
    {
        return 0;
    }
    override public void ReciveDamage(Projectile item)
    {
        currentHP += Mathf.Min(0, armorValue - item.DoDamage(this));
        healthBarTransform.Find("Bar").localScale = new Vector3((currentHP / maxHP), 1);
    }

}

