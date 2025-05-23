using UnityEngine;

public class BT_Leaf : BT_Node
{
    private System.Func<BT_NodeStatus> action;

    public BT_Leaf(System.Func<BT_NodeStatus> action)
    {
        this.action = action;
    }
    public override BT_NodeStatus Evaluate()
    {
        return action();
    }

}
