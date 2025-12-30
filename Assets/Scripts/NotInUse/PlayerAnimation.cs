using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[DisallowMultipleComponent]
public class PlayerAnimations : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    /// <summary>
    /// Mirrors what your old script set on the Animator.
    /// </summary>
    public void UpdateLocomotion(float velX, bool movementBool, bool downArrow, bool grounded)
    {
        anim.SetFloat("DirX", velX);
        anim.SetBool("MovementBool", movementBool);
        anim.SetBool("DownArrow", downArrow);
        anim.SetBool("IsGrounded", grounded);
    }

    public void TriggerHit() => anim.SetTrigger("Hit");
    public void TriggerPickup() => anim.SetTrigger("Pickup");
}
