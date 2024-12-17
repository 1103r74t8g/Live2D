using UnityEngine;

public class anim : MonoBehaviour
{
    private Animator charAnim;
    private Live2D.Cubism.Framework.Expression.CubismExpressionController expressionController;
    void Start()
    {
        charAnim = GetComponent<Animator>();
        expressionController = GetComponent<Live2D.Cubism.Framework.Expression.CubismExpressionController>();
    }


    void Update()
    {
        characterMotion();
        facialExpress();
    }

    void characterMotion()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            charAnim.SetTrigger("armWavingTrigger");
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            charAnim.SetTrigger("bodyWavingTrigger");
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            charAnim.SetTrigger("grabHatTrigger");
        }
    }

    void facialExpress()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            //normal
            expressionController.CurrentExpressionIndex = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //smile
            expressionController.CurrentExpressionIndex = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            //proud
            expressionController.CurrentExpressionIndex = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            //shining
            expressionController.CurrentExpressionIndex = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            //sad
            expressionController.CurrentExpressionIndex = 4;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            //shy
            expressionController.CurrentExpressionIndex = 5;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            //shock
            expressionController.CurrentExpressionIndex = 6;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            //mad
            expressionController.CurrentExpressionIndex = 7;
        }
    }
}
