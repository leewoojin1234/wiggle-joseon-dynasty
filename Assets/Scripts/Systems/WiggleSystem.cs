using System;
using UnityEngine;
using Wiggle.Data;
using Wiggle.Global;

namespace Wiggle.Systems
{
    public class WiggleSystem : MonoBehaviour
    {
        private GameStatus status;
        private GameSettings settings;

        private void Awake()
        {
            status ??= DataHub.Status;
            settings ??= DataHub.Settings;
        }

        void Start()
        {
            if (status != null && settings != null)
            {
                status.Initialize(settings.wiggleBase);
            }
        }

        void Update()
        {
            if (status == null || settings == null) return;

            // 실룩 지수 자연 감소 (감쇠)
            if (status.wigglePower > settings.wiggleBase)
            {
                float newPower = status.wigglePower - (settings.wiggleDecayRate * Time.deltaTime);
                status.SetWiggle(Mathf.Max(newPower, settings.wiggleBase));
            }
        }

        public void BoostWiggle()
        {
            if (status == null || settings == null) return;
            status.AddWiggle(settings.wiggleBoostPerClick, settings.wiggleMax, settings.wiggleBase);
        }
    }
}
