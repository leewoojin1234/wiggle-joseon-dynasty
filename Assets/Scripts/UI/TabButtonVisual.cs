using UnityEngine;

namespace Wiggle.UI
{
    public class TabButtonVisual : MonoBehaviour
    {
        [SerializeField] private GameObject normalVisual;
        [SerializeField] private GameObject selectedVisual;

        public void SetSelected(bool selected)
        {
            if (normalVisual != null)
                normalVisual.SetActive(!selected);

            if (selectedVisual != null)
                selectedVisual.SetActive(selected);
        }
    }
}