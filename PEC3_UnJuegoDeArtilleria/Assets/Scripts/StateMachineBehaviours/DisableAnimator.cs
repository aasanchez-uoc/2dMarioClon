using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAnimator : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.enabled = false;
        animator.gameObject.GetComponent<ItemMovement>().enabled = true;
    }
}
