using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingHammer : EquipScript
{
    [SerializeField] private float healAmount;

    void Update()
    {
        GeneralFunctions.Player.GetComponent<PlayerStats>().Heal(healAmount * Time.deltaTime);
    }
}
