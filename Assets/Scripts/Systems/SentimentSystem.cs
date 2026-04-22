using UnityEngine;
using Wiggle.Data;
using Wiggle.Global;

namespace Wiggle.Systems
{
    public class SentimentSystem : MonoBehaviour
    {
        private GameStatus status;

        private void Awake() => status ??= DataHub.Status;
        
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
