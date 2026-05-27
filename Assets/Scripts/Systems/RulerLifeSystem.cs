using UnityEngine;
using Wiggle.Data;
using Wiggle.Global;

namespace Wiggle.Systems
{
    public class RulerLifeSystem : MonoBehaviour
    {
        public GameStatus status => DataHub.Status;
        public GameSettings settings => DataHub.Settings;

        private void Start()
        {
            if (status == null || settings == null) return;

            status.SetRulerLife(0f, settings.rulerLifeSpanSeconds, false);
        }

        private void Update()
        {
        }
    }
}
