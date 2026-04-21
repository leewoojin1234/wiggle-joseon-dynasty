using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using Wiggle.Global;
using Wiggle.Effects;
using Wiggle.Systems;

namespace Wiggle.Core
{
    public class WiggleClickHandler : MonoBehaviour, IPointerDownHandler
    {
        [Header("Systems")]
        public WiggleSystem wiggleSystem;

        [Header("UX Settings")]
        [Tooltip("클릭 시 흔들릴 렌더러 오브젝트 (왕 이미지)")]
        public Transform kingVisual;
        
        [Tooltip("클릭 시 소리 재생 (AudioSource)")]
        public AudioSource clickSound;

        // 모바일 터치 / 마우스 클릭 즉시 호출됨
        public void OnPointerDown(PointerEventData eventData)
        {
            // 1. 코어 로직: 실룩 지수 증가
            if (wiggleSystem != null)
            {
                wiggleSystem.BoostWiggle();
            }

            // 2. UX 피드백 실행
            PlayClickFeedback(eventData);
        }

        // 타격감을 통합 처리하는 메서드
        private void PlayClickFeedback(PointerEventData eventData)
        {
            // A. 사운드 재생
            if (clickSound != null) clickSound.Play();

            // B. 풀링된 이펙트 생성
            SpawnClickEffect(eventData);

            // C. 왕 실룩 애니메이션 (코루틴)
            if (kingVisual != null)
            {
                // 연타 시 코루틴이 꼬이지 않도록 기존 진행 중인 애니메이션 정지
                StopAllCoroutines();
                StartCoroutine(PunchScaleRoutine());
            }

            // D. 모바일 햅틱 피드백 (기본 진동)
            #if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate(); 
            #endif
        }

        // 터치한 위치에 오브젝트 풀에서 이펙트를 꺼내 배치하는 로직
        private void SpawnClickEffect(PointerEventData eventData)
        {
            if (EffectPoolManager.Instance == null) return;

            // 풀에서 이펙트 가져오기
            ClickEffect effect = EffectPoolManager.Instance.Pool.Get();
            
            // 수정된 부분: 캔버스(Overlay) 환경에서는 eventData.position을 바로 쓰면 됩니다.
            effect.transform.position = eventData.position;
            
            effect.PlayEffect();
        }

        // DOTween 없이 구현한 간단한 찌그러짐 애니메이션
        private IEnumerator PunchScaleRoutine()
        {
            Vector3 originalScale = Vector3.one;
            Vector3 punchScale = new Vector3(1.1f, 0.9f, 1f); // 좌우로 퍼지고 위아래로 눌림

            kingVisual.localScale = punchScale;
            yield return new WaitForSeconds(0.05f); // 0.05초 대기
            kingVisual.localScale = originalScale;
        }
    }
}
