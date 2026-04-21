using UnityEngine;
using UnityEngine.Pool;
using Wiggle.Effects;

namespace Wiggle.Global
{
    public class EffectPoolManager : MonoBehaviour
    {
        public static EffectPoolManager Instance;

        [Header("Pool Settings")]
        public ClickEffect effectPrefab;
        public int defaultCapacity = 20;
        public int maxSize = 100;

        // 추가된 부분: 이펙트들을 담아둘 캔버스 안의 부모 오브젝트
        [Tooltip("Canvas 안의 빈 오브젝트나 Canvas 자체를 끌어다 넣으세요")]
        public Transform effectContainer; 

        public IObjectPool<ClickEffect> Pool { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            Pool = new ObjectPool<ClickEffect>(
                createFunc: CreateEffect,
                actionOnGet: OnTakeEffectFromPool,
                actionOnRelease: OnReturnEffectToPool,
                actionOnDestroy: OnDestroyEffect,
                collectionCheck: false,
                defaultCapacity: defaultCapacity,
                maxSize: maxSize
            );
        }

        private ClickEffect CreateEffect()
        {
            // 수정된 부분: Instantiate할 때 effectContainer를 부모로 지정합니다.
            ClickEffect effect = Instantiate(effectPrefab, effectContainer);
            effect.SetPool(Pool);
            return effect;
        }

        private void OnTakeEffectFromPool(ClickEffect effect) => effect.gameObject.SetActive(true);
        private void OnReturnEffectToPool(ClickEffect effect) => effect.gameObject.SetActive(false);
        private void OnDestroyEffect(ClickEffect effect) => Destroy(effect.gameObject);
    }
}
