using UnityEngine;
using Live2D.Cubism.Framework.Expression;
using System.Collections.Generic;

public class anim : MonoBehaviour
{
    private Animator charAnim;
    private CubismExpressionController expressionController;

    private Dictionary<KeyCode, int> expressionMapping = new Dictionary<KeyCode, int>()
    {
        { KeyCode.Alpha0, 0 }, // normal
        { KeyCode.Alpha1, 1 }, // smile
        { KeyCode.Alpha2, 2 }, // proud
        { KeyCode.Alpha3, 3 }, // shining
        { KeyCode.Alpha4, 4 }, // sad
        { KeyCode.Alpha5, 5 }, // shy
        { KeyCode.Alpha6, 6 }, // shock
        { KeyCode.Alpha7, 7 }  // mad
    };

    void Start()
    {
        charAnim = GetComponent<Animator>();
        expressionController = GetComponent<CubismExpressionController>();
    }

    void Update()
    {
        characterMotion();
        facialExpress();
    }

    void characterMotion()
    {
        if (Input.GetKeyDown(KeyCode.Q)) charAnim.SetTrigger("armWavingTrigger");
        else if (Input.GetKeyDown(KeyCode.W)) charAnim.SetTrigger("bodyWavingTrigger");
        else if (Input.GetKeyDown(KeyCode.E)) charAnim.SetTrigger("grabHatTrigger");
    }

    void facialExpress()
    {
        foreach (var key in expressionMapping.Keys)
        {
            if (Input.GetKeyDown(key))
            {
                expressionController.CurrentExpressionIndex = expressionMapping[key];
            }
        }
    }
}
