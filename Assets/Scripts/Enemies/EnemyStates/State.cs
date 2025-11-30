using UnityEngine;

public abstract class State
{
    protected Enemy enemy;

    abstract public void AI();
    abstract public void FixedAI();

    abstract public State ChangeState();
}
