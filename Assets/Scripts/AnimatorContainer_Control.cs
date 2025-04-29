using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorContainer_Control : MonoBehaviour
{   
    private Animator animator;
    [SerializeField] private Player player;
    private bool openclose = true; 
    // Start is called before the first frame update
    void Start()
    {
        //监听玩家的事件
        player.GetContainerOpenClose += OnGetOpenClose;
        animator = GetComponent<Animator>();
    }
    void OnGetOpenClose(GameObject targetobject)
    {
         if (targetobject != null && targetobject.name == gameObject.name)
        {
            animator.SetBool("OpenClose", openclose);
        }
       
    }

}
