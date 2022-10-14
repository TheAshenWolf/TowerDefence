using System;
using System.Collections.Generic;
using UnityEngine;

namespace Types
{
    [Serializable]
    public struct Wave
    {
        public List<SubWave> subWaves;
    }

    [Serializable]
    public struct SubWave
    {
        public GameObject enemy;
        public int count;
    }
}