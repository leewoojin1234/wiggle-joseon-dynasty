using System.Collections;
using UnityEngine;
using UnityEngine.Pool; // 유니티 내장 풀 사용
using UnityEngine.UI;

namespace Wiggle.Effects
{
    public class ClickEffect : MonoBehaviour
    {
        // 자기를 관리하는 풀의 참조를 가지고 있어야 합니다.
        private IObjectPool<ClickEffect> managedPool;
        
        // 파티클이나 애니메이션 재생 시간
        public float lifeTime = 0.5f; 
        public Image effectImage;
        public float rotationStepDegrees = 60f;
        public int maxRotationSteps = 1;
        public float riseDistance = 90f;
        public float startScale = 1f;
        public float endScale = 1.15f;

        private Coroutine effectRoutine;
        private Vector3 startPosition;

        public void SetPool(IObjectPool<ClickEffect> pool)
        {
            managedPool = pool;
        }

        public void SetSprite(Sprite sprite)
        {
            if (effectImage == null)
                effectImage = GetComponent<Image>();

            if (effectImage != null)
                effectImage.sprite = sprite;
        }

        // 풀에서 꺼내질 때마다 호출될 초기화 함수
        public void PlayEffect()
        {
            CancelInvoke(nameof(ReturnToPool));
            ApplyRandomRotation();
            startPosition = transform.localPosition;

            if (effectRoutine != null)
                StopCoroutine(effectRoutine);

            effectRoutine = StartCoroutine(PlayEffectRoutine());

            // 1. 파티클 재생이나 애니메이션 초기화 코드
            // GetComponent<ParticleSystem>().Play(); 
        }

        private IEnumerator PlayEffectRoutine()
        {
            if (effectImage != null)
            {
                Color color = effectImage.color;
                color.a = 1f;
                effectImage.color = color;
            }

            transform.localPosition = startPosition;
            transform.localScale = Vector3.one * startScale;

            float elapsed = 0f;
            while (elapsed < lifeTime)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / lifeTime);
                float easedProgress = 1f - Mathf.Pow(1f - progress, 2f);

                transform.localPosition = startPosition + Vector3.up * (riseDistance * easedProgress);
                transform.localScale = Vector3.one * Mathf.Lerp(startScale, endScale, easedProgress);

                if (effectImage != null)
                {
                    Color color = effectImage.color;
                    color.a = 1f - progress;
                    effectImage.color = color;
                }

                yield return null;
            }

            effectRoutine = null;
            ReturnToPool();
        }

        private void ApplyRandomRotation()
        {
            int step = Random.Range(-maxRotationSteps, maxRotationSteps + 1);
            transform.localRotation = Quaternion.Euler(0f, 0f, step * rotationStepDegrees);
        }

        private void ReturnToPool()
        {
            // 파괴(Destroy)하는 대신 풀에 반납(Release)합니다.
            managedPool.Release(this);
        }
    }
}
