using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorPlayer_Control : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private Player player;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        
    }

    void Update()
    {
        // 获取移动状态
        bool isMoving = player.GetIsMoving();   
        bool isJumping = player.GetIsJumping();

        // 设置Walking动画参数
        animator.SetBool("IsWalking", isMoving);
        animator.SetBool("IsJumping", isJumping);
       
    }
}
