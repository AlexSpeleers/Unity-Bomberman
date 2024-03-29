﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public BlockManager blockManager;
    private void Awake()
    {
        blockManager.SetActiveBlock(this, transform.position);
    }
    private void OnDestroy()
    {
        blockManager.RemoveBlock(this, transform.position);
    }

}
