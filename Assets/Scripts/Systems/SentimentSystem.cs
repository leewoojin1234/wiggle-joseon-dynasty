using UnityEngine;
using Wiggle.Data;

namespace Wiggle.Systems
{
    public class SentimentSystem : MonoBehaviour
    {
        public GameStatus status;

        public void AddMinSim(float amount)
        {
            if (status == null) return;
            status.AddMinSim(amount);
            
            if (status.minSim <= 20f)
            {
                Debug.LogWarning("민심 위험! 반란 조짐이 보입니다.");
            }
        }
    }
}
