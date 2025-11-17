using UnityEngine;

public abstract class State
{
    abstract public void AI();
    abstract public void FixedAI();

    abstract public State ChangeState();
}
