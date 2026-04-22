using System;
using System.Collections.Generic;

namespace Wiggle.Global
{
    [Serializable]
    public class SaveData
    {
        public string saveName;
        public string lastSaveDate;
        
        // GameStatus 정보
        public double money;
        public float minSim;

        // HelperStatus 정보 (조력자 & 투자 레벨)
        public int[] helperLevels;
        public int[] investmentLevels;

        public SaveData(string name)
        {
            saveName = name;
            lastSaveDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            money = 0;
            minSim = 50f;
            helperLevels = new int[0];
            investmentLevels = new int[0];
        }
    }
}
