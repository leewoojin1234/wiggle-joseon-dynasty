using System;
using UnityEngine;

namespace Wiggle.Data
{
    public enum PetitionStoryArc
    {
        Common,
        Agriculture,
        Taxation,
        Corruption,
        People
    }

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
        [Header("Story")]
        public string petitionId;
        public PetitionStoryArc storyArc = PetitionStoryArc.Common;
        public int chapter = 1;
        public bool oneShot;

        [Header("Text")]
        public string title;
        [TextArea(2, 5)] public string description;

        [Header("Choices")]
        public PetitionOption optionA;
        public PetitionOption optionB;
    }
}
