using System;
using UnityEngine;

namespace Wiggle.Data
{
    [Serializable]
    public class PetitionOption
    {
        public string label;
        public double incomeSeconds;
        public float minSimDelta;
        public float wiggleDelta;
    }

    [CreateAssetMenu(fileName = "PetitionEvent", menuName = "Wiggle/Petition Event")]
    public class PetitionEventData : ScriptableObject
    {
        [Header("Identity")]
        public string petitionId;

        [Header("Text")]
        public string title;
        [TextArea(2, 5)] public string description;

        [Header("Choices")]
        public PetitionOption optionA;
        public PetitionOption optionB;
    }
}
