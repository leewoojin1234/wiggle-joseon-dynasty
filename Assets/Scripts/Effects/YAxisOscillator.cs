using UnityEngine;

namespace Wiggle.Effects
{
    public class YAxisOscillator : MonoBehaviour
    {
        [SerializeField] private float interval = 1f; // n초
        [SerializeField] private float rotateSpeed = 5f;

        private float targetY;

        private void Start()
        {
            targetY = 180f;
        }

        private void Update()
        {
            // 전역 시간을 interval로 나누어 짝수/홀수 타이밍을 계산 (동기화 핵심)
            float phase = (Time.time / interval) % 2f;
            targetY = (phase > 1.0f) ? 0f : 180f;

            Quaternion targetRot = Quaternion.Euler(0f, targetY, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * rotateSpeed);
        }
    }
}