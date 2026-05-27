using UnityEngine;
using UnityEngine.UI;
using Wiggle.Data;
using Wiggle.Global;

namespace Wiggle.Controllers
{
    public class KingAnimationController : MonoBehaviour
    {
        [Header("Data Reference")]
        private GameStatus status;
        private GameSettings settings;

        [Header("UI Reference")]
        [Tooltip("왕을 표시하는 UI Image 컴포넌트")]
        public Image kingImage; 
        private RectTransform rectTransform;

        [Header("Sprites")]
        [Tooltip("보너스 없을 때 (가만히)")]
        public Sprite idleSprite;       
        [Tooltip("보너스 상태 (춤추는 이미지 1장)")]
        public Sprite danceSprite;      

        [Header("Animation Settings")]
        [Tooltip("기본 실룩 파워일 때 좌우 반전이 교체되는 속도")]
        public float wiggleSpeed = 5f; 
        private float wigglePhase;

        void Awake()
        {
            status ??= DataHub.Status;
            settings ??= DataHub.Settings;
            
            // Image 컴포넌트가 연결되어 있다면 RectTransform을 미리 가져옵니다.
            if (kingImage != null)
            {
                rectTransform = kingImage.GetComponent<RectTransform>();
            }
        }

        void Update()
        {
            if (status == null || settings == null || rectTransform == null) return;

            bool isBonusActive = status.wigglePower > (settings.wiggleBase + 0.01f);

            if (isBonusActive)
            {
                // 1. 춤추는 이미지로 변경
                kingImage.sprite = danceSprite;

                // 2. 현재 실룩 파워에 비례해서 Y축 회전을 0도 <-> 180도로 전환
                float basePower = Mathf.Max(0.01f, settings.wiggleBase);
                float speedMultiplier = status.wigglePower / basePower;
                wigglePhase += Time.deltaTime * wiggleSpeed * speedMultiplier;
                float pingPong = Mathf.PingPong(wigglePhase, 1f);
                
                if (pingPong > 0.5f)
                {
                    // Y축 180도 회전 (좌우 반전)
                    rectTransform.localRotation = Quaternion.Euler(0f, 180f, 0f);
                }
                else
                {
                    // 원본 방향 (0도)
                    rectTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                }
            }
            else
            {
                // 1. 가만히 있는 이미지로 변경
                kingImage.sprite = idleSprite;
                
                // 2. 회전값 원상 복구 (이거 안 하면 가만히 있을 때도 뒤집혀 있을 수 있음)
                wigglePhase = 0f;
                rectTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }
    }
}
