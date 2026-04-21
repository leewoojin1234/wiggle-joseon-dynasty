using UnityEngine;
using UnityEngine.Pool; // 유니티 내장 풀 사용

namespace Wiggle.Effects
{
    public class ClickEffect : MonoBehaviour
    {
        // 자기를 관리하는 풀의 참조를 가지고 있어야 합니다.
        private IObjectPool<ClickEffect> managedPool;
        
        // 파티클이나 애니메이션 재생 시간
        public float lifeTime = 0.5f; 

        public void SetPool(IObjectPool<ClickEffect> pool)
        {
            managedPool = pool;
        }

        // 풀에서 꺼내질 때마다 호출될 초기화 함수
        public void PlayEffect()
        {
            // 1. 파티클 재생이나 애니메이션 초기화 코드
            // GetComponent<ParticleSystem>().Play(); 

            // 2. 정해진 시간 뒤에 풀로 돌아감
            Invoke(nameof(ReturnToPool), lifeTime);
        }

        private void ReturnToPool()
        {
            // 파괴(Destroy)하는 대신 풀에 반납(Release)합니다.
            managedPool.Release(this);
        }
    }
}
