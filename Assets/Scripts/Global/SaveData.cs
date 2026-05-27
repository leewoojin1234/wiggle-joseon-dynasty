using System;
using System.Collections.Generic;

namespace Wiggle.Global
{
    [Serializable]
    public class SaveData
    {
        public string saveName;
        public string lastSaveDate;
        public long lastSaveUtcTicks;
        
        // GameStatus 정보
        public double money;
        public float minSim;
        public float lastWigglePower; 
        public float rulerAgeSeconds;
        public float rulerLifeSpanSeconds;
        public bool generationEndingPending;

        // HelperStatus 정보 (조력자 & 투자 레벨)
        public int[] helperLevels;
        public int[] investmentLevels;

        // PermanentStatus 정보 (환생 데이터)
        public long kingshipPoints;
        public int reincarnationCount;
        public int incomeBuffLevel;
        public int wiggleBuffLevel;
        public int sentimentBuffLevel;

        public SaveData(string name)
        {
            saveName = name;
            lastSaveDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            lastSaveUtcTicks = DateTime.UtcNow.Ticks;
            money = 0;
            minSim = 50f;
            lastWigglePower = 1.0f;
            rulerAgeSeconds = 0f;
            rulerLifeSpanSeconds = 600f;
            generationEndingPending = false;
            helperLevels = new int[0];
            investmentLevels = new int[0];

            // 영구 데이터 초기화
            kingshipPoints = 0;
            reincarnationCount = 1;
            incomeBuffLevel = 0;
            wiggleBuffLevel = 0;
            sentimentBuffLevel = 0;
        }
    }
}
