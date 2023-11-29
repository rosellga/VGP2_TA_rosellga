using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CLA_Texture2D
{
    public Vector2Int origin_;
    public Vector2Int offset_;
    public uint size_;
    public float scale_;
    public Texture2D texture_;
    public string seed_;
}