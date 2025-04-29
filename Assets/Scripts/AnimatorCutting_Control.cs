using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorCutting_Control : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Player player;
    
    // 动画参数名称
    private const string CUT = "Cut";
    
    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }
    
    private void Start()
    {
        if (player != null)
        {
            // 订阅玩家切菜事件
            player.GetCuttingprocess += StartCutting;
        }
        else
        {
            Debug.LogError("AnimatorCutting_Control: Missing Player reference!");
        }
    }
    
    private void OnDestroy()
    {
        // 解除订阅以防止内存泄漏
        if (player != null)
        {
            player.GetCuttingprocess -= StartCutting;
        }
    }
    
    // 启动切菜动画
    private void StartCutting()
    {
        Debug.Log("AnimatorCutting_Control: Triggering cut animation");
        animator.SetBool(CUT, true);
        
        // 使用协程在动画播放一段时间后重置参数
        StartCoroutine(ResetCutAnimation());
    }
    
    // 重置切菜动画参数的协程
    private IEnumerator ResetCutAnimation()
    {
        // 等待一小段时间，让动画有时间播放
        yield return new WaitForSeconds(0.5f);
        
        // 重置Cut参数
        animator.SetBool(CUT, false);
        Debug.Log("AnimatorCutting_Control: Cut animation parameter reset");
    }
}
