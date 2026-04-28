using UnityEngine;

namespace Wiggle.Effects
{
    public class YAxisOscillator : MonoBehaviour
    {
        [SerializeField] private float interval = 1f; // n초
        [SerializeField] private float rotateSpeed = 5f;

        private float timer;
        private float targetY;

        private void Start()
        {
            targetY = 180f;
        }

        private void Update()
        {
            timer += Time.deltaTime;

            if (timer >= interval)
            {
                timer = 0f;
                targetY = targetY == 0f ? 180f : 0f;
            }

            Quaternion targetRot = Quaternion.Euler(0f, targetY, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * rotateSpeed);
        }
    }
}