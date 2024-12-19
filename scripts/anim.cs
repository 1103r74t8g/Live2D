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

    private bool isAnimating = false;  // 用來標記動畫是否播放中
    private float animationCooldownTime = 5f;  // 動畫播放後的冷卻時間 (5秒)
    private float cooldownTimer = 0f;  // 計時器，用來跟蹤冷卻時間
    private float expressionChangeTimer = 60f;  // 隔60秒
    private float expressionDuration = 20f;    // 笑20秒
    private float expressionDurationTimer = 0f;
    private int currentExpression = 0;  // 目前表情的索引
    private bool isExpressionActive = false;

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
        checkMouseShyLine();
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

    private int crossCount = 0;           // 穿越次數
    private float timer = 0f;            // 計時器
    private bool isMouseOverLine = false; // 滑鼠上一幀是否在範圍內
    private int madToSmile = 3;
    void checkHeadTouch()
    {
        // 按下左鍵時，重置計時器與穿越次數
        if (Input.GetMouseButtonDown(0)) // 左鍵按下
        {
            crossCount = 0;   // 重置穿越計數
            isMouseOverLine = false;
            timer = 0f;       // 計時重置
        }

        // 如果左鍵持續被按住
        if (Input.GetMouseButton(0)) // 左鍵持續按下
        {
            // 獲取滑鼠當前位置
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;  // 設定滑鼠 Z 軸為 0

            // 更新計時
            timer += Time.deltaTime;

            // 判斷滑鼠是否在設定的「範圍線」內
            if (IsMouseInHeadLine(mousePosition))
            {
                if (!isMouseOverLine) // 滑鼠從外部進入範圍
                {
                    crossCount++; // 穿越次數 +1
                    Debug.Log("Count: " + crossCount);
                }
                isMouseOverLine = true;
            }
            else
            {
                isMouseOverLine = false; // 滑鼠不在範圍內
            }

            // 如果滑鼠在5秒內穿越範圍 >= 10次，觸發動畫
            if (crossCount >= 10 && timer <= 5f && !isAnimating && cooldownTimer <= 0f)
            {
                // 開始動畫並設定表情
                if (currentExpression == 7)
                {
                    madToSmile--;
                    if (madToSmile == 0)
                    {
                        expressionController.CurrentExpressionIndex = 1;  // 變回開心
                        currentExpression = 1;
                        madToSmile = 3;
                    }
                }
                int ramdomTrigger = Random.Range(0, 2); // 身體搖晃動作
                if (ramdomTrigger == 0)
                {
                    charAnim.SetTrigger("bodyWavingTrigger");
                }
                else
                {
                    charAnim.SetTrigger("armWavingTrigger");
                }
                isAnimating = true;  // 開始動畫
                cooldownTimer = animationCooldownTime;  // 設定冷卻時間為5秒

                // 重置計時器與穿越次數
                crossCount = 0;
                timer = 0f;
                isMouseOverLine = false;
            }

            // 如果超過5秒未觸發，重置計數器
            if (timer > 5f)
            {
                Debug.Log("Resetting counter.");
                crossCount = 0;
                timer = 0f;
                isMouseOverLine = false;
            }
        }
    }

    // 判斷滑鼠是否在設定的範圍線內
    private bool IsMouseInHeadLine(Vector3 mousePosition)
    {
        float mouseX = mousePosition.x;
        float mouseY = mousePosition.y;

        // 滑鼠是否在範圍線內 (x接近0，且 y 在2.5-4之間)
        return Mathf.Abs(mouseX) <= 0.1f && (mouseY > 2.5f && mouseY < 4f);
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
            expressionController.CurrentExpressionIndex = currentExpression;  //變回目前表情
            isAnimating = false;  // 動畫完成，停止標記
        }
    }

    void HandleExpressionChange()
    {
        if (!isExpressionActive)
        {
            // 倒數 50s 後笑一下
            expressionChangeTimer -= Time.deltaTime;

            if (expressionChangeTimer <= 0f)
            {
                expressionController.CurrentExpressionIndex = 1;
                currentExpression = 1;
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
                currentExpression = 0;
                Debug.Log("change back");
                isExpressionActive = false;
            }
        }
    }
    private int crossCountShyLine = 0;         // 穿越次數 (新範圍)
    private float timerShy = 0f;           // 計時器 (新範圍)
    private bool isMouseOverShyLine = false; // 滑鼠上一幀是否在新範圍內
    private int crossCountShock = 0;          // 穿越次數 (shock 範圍)
    private float timerShock = 0f;            // 計時器 (shock 範圍)
    private bool isMouseOverShockLine = false; // 滑鼠上一幀是否在shock範圍內
    private bool isShockActive = false;       // 判斷是否處於震驚狀態
    void checkMouseShyLine()
    {
        // 按下左鍵時，重置計時器與穿越次數
        if (Input.GetMouseButtonDown(0)) // 左鍵按下
        {
            crossCountShyLine = 0;   // 重置穿越計數
            isMouseOverShyLine = false;
            timerShy = 0f;       // 計時重置
        }

        // 如果左鍵持續被按住
        if (Input.GetMouseButton(0)) // 左鍵持續按下
        {
            // 獲取滑鼠當前位置
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;  // 設定滑鼠 Z 軸為 0

            // 更新計時
            timerShy += Time.deltaTime;

            // 判斷滑鼠是否在設定的新範圍內
            if (IsMouseInShyLine(mousePosition))
            {
                if (!isMouseOverShyLine) // 滑鼠從外部進入範圍
                {
                    crossCountShyLine++; // 穿越次數 +1
                    Debug.Log("Shock Count: " + crossCountShyLine);
                }
                isMouseOverShyLine = true;
            }
            else
            {
                isMouseOverShyLine = false; // 滑鼠不在範圍內
            }

            // 如果滑鼠在5秒內穿越範圍 >= 10次，觸發表情變更為 shock
            if (crossCountShyLine >= 10 && timerShy <= 5f && !isShockActive)
            {
                expressionController.CurrentExpressionIndex = 6;  // 震驚表情 (shock)
                expressionChangeTimer = 50f;// 重新計時50秒，不然會突然笑一下
                Debug.Log("Shock Expression Activated!");
                isShockActive = true;

                // 重置計時器與穿越次數
                crossCountShyLine = 0;
                timerShy = 0f;
                isMouseOverShyLine = false;
            }

            // 如果超過5秒未觸發，重置計數器
            if (timerShy > 5f)
            {
                Debug.Log("New Resetting counter.");
                crossCountShyLine = 0;
                timerShy = 0f;
                isMouseOverShyLine = false;
                isShockActive = false;
            }

            // Shock後進行來回穿越判斷
            if (isShockActive)
            {
                timerShy = 0f;
                crossCountShyLine = 0;
                timerShock += Time.deltaTime; // 計時

                // 判斷滑鼠是否在shock範圍內
                if (IsMouseInShyLine(mousePosition))
                {
                    if (!isMouseOverShockLine)
                    {
                        crossCountShock++;  // 穿越次數 +1
                        Debug.Log("Shy Count: " + crossCountShock);
                    }
                    isMouseOverShockLine = true;
                }
                else
                {
                    isMouseOverShockLine = false; // 滑鼠不在範圍內
                }

                // 判斷滑鼠是否來回穿越 20 次
                if (crossCountShock >= 20 && timerShock <= 10f)
                {
                    // 隨機選擇生氣或害羞的表情
                    int randomExpression = (Random.Range(0, 2) == 0) ? 5 : 7;  // 50%機率選擇5或7                                                                               // 7: mad, 5: shy
                    expressionController.CurrentExpressionIndex = randomExpression;
                    currentExpression = randomExpression;
                    charAnim.SetTrigger("grabHatTrigger");
                    isAnimating = true;  // 開始動畫
                    cooldownTimer = animationCooldownTime;  // 設定冷卻時間為5秒
                    expressionChangeTimer = 50f;// 重新計時50秒，不然會突然笑一下
                    Debug.Log("mad or shy: " + randomExpression);

                    // 重置計時器和穿越次數
                    crossCountShock = 0;
                    timerShock = 0f;
                    isShockActive = false;
                }
                else if (timerShock > 10f)
                {
                    // 如果超過 10 秒，回到原來的表情
                    expressionController.CurrentExpressionIndex = 0;  // 恢復 normal
                    Debug.Log("Back to normal expression.");
                    isShockActive = false;
                    timerShock = 0f;
                }
            }


        }
    }

    // 判斷滑鼠是否在設定的shock範圍內
    private bool IsMouseInShyLine(Vector3 mousePosition)
    {
        float mouseX = mousePosition.x;
        float mouseY = mousePosition.y;

        // 新範圍：x 接近 0 且 y 在 0.6 - 1.5 之間
        if (Mathf.Abs(mouseX) <= 0.1f && mouseY > 0.6f && mouseY < 1.5f)
        {
            return true;
        }

        // 或是範圍：x 在 -0.8 ~ 0.8 之間且 y 接近 -1.4
        if (mouseX > -0.8f && mouseX < 0.8f && Mathf.Abs(mouseY + 1.4f) <= 0.1f)
        {
            return true;
        }

        return false;
    }
}
