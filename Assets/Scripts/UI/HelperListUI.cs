using UnityEngine;
using System.Collections.Generic;
using Wiggle.Systems;

namespace Wiggle.UI
{
    public class HelperListUI : MonoBehaviour
    {
        [Header("System Reference")]
        public HelperSystem helperSystem;

        [Header("UI Prefab")]
        public HelperItemUI itemPrefab;
        public Transform contentParent;

        private List<HelperItemUI> _items = new List<HelperItemUI>();

        void Start()
        {
            if (helperSystem == null) helperSystem = HelperSystem.Instance;
            if (helperSystem == null || itemPrefab == null || contentParent == null) return;

            InitializeList();
        }

        void OnEnable()
        {
            var hSystem = helperSystem != null ? helperSystem : HelperSystem.Instance;
            if (hSystem != null && hSystem.gameStatus != null)
            {
                hSystem.gameStatus.OnMoneyChanged += RefreshAllItems;
            }
            if (hSystem != null && hSystem.helperStatus != null)
            {
                hSystem.helperStatus.OnHelperUpdated += RefreshAllItems;
            }
        }

        void OnDisable()
        {
            var hSystem = helperSystem != null ? helperSystem : HelperSystem.Instance;
            if (hSystem != null && hSystem.gameStatus != null)
            {
                hSystem.gameStatus.OnMoneyChanged -= RefreshAllItems;
            }
            if (hSystem != null && hSystem.helperStatus != null)
            {
                hSystem.helperStatus.OnHelperUpdated -= RefreshAllItems;
            }
        }
        
        private void InitializeList()
        {
            // 기존 아이템 삭제
            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }
            _items.Clear();

            // 시스템에 등록된 조력자 데이터만큼 아이템 생성
            for (int i = 0; i < helperSystem.allHelpers.Length; i++)
            {
                HelperItemUI newItem = Instantiate(itemPrefab, contentParent);
                newItem.Setup(helperSystem, helperSystem.allHelpers[i], i);
                _items.Add(newItem);
            }
        }

        private void RefreshAllItems()
        {
            foreach (var item in _items)
            {
                item.UpdateUI();
            }
        }
    }
}
