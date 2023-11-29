using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITF_CharacterActions
{
    public bool Swim();

    public bool Run();

    public bool Walk();

    public bool Roll();

    public bool Fly();

    public void Eat();

    public void Die();

    public void Attack();

    public void Click();

    public void Jump();
}
