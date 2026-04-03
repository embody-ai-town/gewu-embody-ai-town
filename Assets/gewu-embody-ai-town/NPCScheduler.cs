using UnityEngine;
public class NPCScheduler : MonoBehaviour
{
    enum DailyState { Sleeping, GoingOut, Working, Socializing, Returning, Charging }
    DailyState currentState;

    void Update()
    {
        float hour = TimeManager.Instance.GetGameHour();
        
        DailyState newState = hour switch
        {
            < 6f  => DailyState.Sleeping,    // 子时~卯时：充电休眠
            < 8f  => DailyState.GoingOut,    // 卯时：陆续出门
            < 18f => DailyState.Working,     // 辰时~酉时：劳作/社交
            < 20f => DailyState.Returning,   // 酉时：返家
            _     => DailyState.Charging     // 戌时以后：充电
        };

        if (newState != currentState)
        {
            currentState = newState;
            OnStateChanged(newState);
        }
    }

    void OnStateChanged(DailyState state)
    {
        switch (state)
        {
            case DailyState.GoingOut:
                // NavMesh 导航到职业对应地点
                break;
            case DailyState.Charging:
                // 导航回 homePosition，播放充电动画
                break;
        }
    }
}