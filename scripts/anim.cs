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

    public GameObject headTouchArea;  // 頭部觸摸區域的 GameObject
    public float headTouchRadius = 0.8f;  // 頭部範圍半徑
    private float timeInside = 0f;  // 滑鼠在範圍內的停留時間
    private bool isMouseOverHead = false;  // 標記滑鼠是否在範圍內
    private bool isAnimating = false;  // 用來標記動畫是否播放中
    private float animationCooldownTime = 5f;  // 動畫播放後的冷卻時間 (5秒)
    private float cooldownTimer = 0f;  // 計時器，用來跟蹤冷卻時間
    private float expressionChangeTimer = 90f;  // 1分半的倒計時
    private float expressionDuration = 30f;    // 表情持續時間
    private float expressionDurationTimer = 0f;
    private int currentExpression = 0;  // 目前表情的索引
    private bool isExpressionActive = false;
    private int[] happyOrShiningExpressions = { 1, 3 };  // smile 和 shining 的表情

    void Start()
    {
        charAnim = GetComponent<Animator>();
        expressionController = GetComponent<CubismExpressionController>();

    }

    void Update()
    {
        characterMotion();
        facialExpress();
        checkHeadTouch();
        handleAnimationCooldown();
        HandleExpressionChange();
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

    void checkHeadTouch()
    {
        if (Input.GetMouseButtonDown(0)) // 左鍵按下
        {
            // 重置計時器
            timeInside = 0f;
            isMouseOverHead = false;
        }

        // 如果左鍵被按下並且滑鼠位置在範圍內
        if (Input.GetMouseButton(0)) // 左鍵持續按下
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;  // 設定滑鼠 Z 軸為 0

            float distanceToHead = Vector3.Distance(mousePosition, headTouchArea.transform.position);

            if (distanceToHead <= headTouchRadius)  // 滑鼠接近頭部範圍
            {
                isMouseOverHead = true;
                timeInside += Time.deltaTime;  // 累計滑鼠在範圍內的時間
            }
            else
            {
                isMouseOverHead = false;
                timeInside = 0f;  // 滑鼠離開範圍時，重置計時器
            }

            // 如果滑鼠停留在範圍內超過 1 秒，並且沒有正在播放動畫，且沒有進入冷卻時間
            if (isMouseOverHead && timeInside >= 0.6f && !isAnimating && cooldownTimer <= 0f)
            {
                // 開始動畫並設定表情
                expressionController.CurrentExpressionIndex = 1;  // 開心表情
                charAnim.SetTrigger("bodyWavingTrigger");  // 身體搖晃動作
                isAnimating = true;  // 開始動畫
                cooldownTimer = animationCooldownTime;  // 設定冷卻時間為5秒
                isMouseOverHead = false;  // 禁止再次觸發摸頭
                timeInside = 0f;  // 觸發後重置計時器，避免重複觸發
            }
        }
    }

    void handleAnimationCooldown()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;  // 減少冷卻時間
        }

        // 檢查是否動畫播放結束
        if (isAnimating)
        {
            // 檢查身體搖晃動畫是否完成，可以根據動畫名稱或狀態機條件來判斷
            if (!charAnim.GetCurrentAnimatorStateInfo(0).IsName("bodyWaving") || charAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                expressionController.CurrentExpressionIndex = currentExpression;  //變回目前表情
                isAnimating = false;  // 動畫完成，停止標記
            }
        }
    }

    void HandleExpressionChange()
    {
        if (!isExpressionActive)
        {
            // 倒數 90s 後切換表情
            expressionChangeTimer -= Time.deltaTime;

            if (expressionChangeTimer <= 0f)
            {
                // 隨機選擇一個表情
                int randomIndex = Random.Range(0, happyOrShiningExpressions.Length);
                expressionController.CurrentExpressionIndex = happyOrShiningExpressions[randomIndex];
                currentExpression = happyOrShiningExpressions[randomIndex];
                Debug.Log("change");
                // 開始計時表情持續時間
                expressionDurationTimer = expressionDuration;
                isExpressionActive = true;

                // 重置90s計時器
                expressionChangeTimer = 90f;
            }
        }
        else
        {
            // 當表情啟動後，倒數 30 秒
            expressionDurationTimer -= Time.deltaTime;

            if (expressionDurationTimer <= 0f)
            {
                // 回到 normal 表情
                expressionController.CurrentExpressionIndex = 0;
                Debug.Log("change back");
                isExpressionActive = false;
            }
        }
    }
}
