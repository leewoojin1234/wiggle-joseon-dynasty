using UnityEngine;
using System.Collections.Generic;
using Wiggle.Systems;

namespace Wiggle.UI
{
    public class InvestmentListUI : MonoBehaviour
    {
        [Header("System Reference")]
        public InvestmentSystem investmentSystem;

        [Header("UI Prefab")]
        public InvestmentItemUI itemPrefab;
        public Transform contentParent;

        private List<InvestmentItemUI> _items = new List<InvestmentItemUI>();

        void Start()
        {
            if (investmentSystem == null) investmentSystem = InvestmentSystem.Instance;
            if (investmentSystem == null || itemPrefab == null || contentParent == null) return;

            InitializeList();
        }

        void OnEnable()
        {
            var iSystem = investmentSystem != null ? investmentSystem : InvestmentSystem.Instance;
            if (iSystem != null && iSystem.investmentStatus != null)
            {
                // 레벨업 시 텍스트 갱신을 위해 구독
                iSystem.investmentStatus.OnHelperUpdated += RefreshAllStaticUI;
            }
        }

        void OnDisable()
        {
            var iSystem = investmentSystem != null ? investmentSystem : InvestmentSystem.Instance;
            if (iSystem != null && iSystem.investmentStatus != null)
            {
                iSystem.investmentStatus.OnHelperUpdated -= RefreshAllStaticUI;
            }
        }

        private void InitializeList()
        {
            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }
            _items.Clear();

            for (int i = 0; i < investmentSystem.allInvestments.Length; i++)
            {
                InvestmentItemUI newItem = Instantiate(itemPrefab, contentParent);
                newItem.Setup(investmentSystem, investmentSystem.allInvestments[i], i);
                _items.Add(newItem);
            }
        }

        private void RefreshAllStaticUI()
        {
            foreach (var item in _items)
            {
                item.UpdateStaticUI();
            }
        }
    }
}
