using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface  IAttackable <t> where t : Unit
{
    void Attack(t unit);
}