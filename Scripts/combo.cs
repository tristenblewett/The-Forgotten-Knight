using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class combo : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private playercombat playerCombat;
    [SerializeField] private int numberofClicks = 0;
    private float lastClickTime = 0;
    [SerializeField] private float maxComboDelay = 1.2f;

    public void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        playerCombat = FindObjectOfType<playercombat>();
    }

    private void Update()
    {
        if (Time.time - lastClickTime > maxComboDelay)
        {
            numberofClicks = 0;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            lastClickTime = Time.time;
            numberofClicks++;

            if (numberofClicks == 1)
            {
                animator.SetBool("Attack1", true);
                playerCombat.Attack();
            }

            numberofClicks = Mathf.Clamp(numberofClicks, 0, 3);
        }

    }

    public void return1()
    {
        
        if (numberofClicks >= 2)
        {
            animator.SetBool("Attack2", true);
            playerCombat.Attack();
        }
        else
        {
            animator.SetBool("Attack1", false);
            numberofClicks = 0;
        }
    }

    public void return2()
    {
        
        if (numberofClicks >= 3)
        {
            animator.SetBool("Attack3", true);
            playerCombat.Attack();
        }
        else
        {
            animator.SetBool("Attack2", false);
            numberofClicks = 0;
        }
    }

    public void return3()
    {

        animator.SetBool("Attack1", false);
        animator.SetBool("Attack2", false);
        animator.SetBool("Attack3", false);
        numberofClicks = 0;
    }
}
