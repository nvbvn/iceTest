﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCreation : MonoBehaviour
{
    public GameObject targetSurface;
    public PreDataSO preData;
    public SpawnSO spawnData;

    public List<List<int>> spawnRaws = new List<List<int>>();
    public int selectedSpawnIndex;
}
