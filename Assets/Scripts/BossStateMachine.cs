using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStateMachine : StateMachineBehaviour
{
    public float delay;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, Int32 layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        animator.gameObject.transform.localScale = new Vector3(10, 10, 10);
    }
}
