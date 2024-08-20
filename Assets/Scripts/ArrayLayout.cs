using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ArrayLayout
{
    [Serializable]
    public struct rowData
    {
        public bool[] row;
    }

    public rowData[] rows = new rowData[8];
    
}
