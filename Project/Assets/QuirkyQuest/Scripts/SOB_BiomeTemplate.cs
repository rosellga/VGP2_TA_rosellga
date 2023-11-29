using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Biome", menuName = "QuirkyQuest/Biome/Template")]
public class SOB_BiomeTemplate : ScriptableObject
{
    public ENU_Coastalness coastalness_;
    public float seaLevel_;
    public float globalHeight_;
    public float terrainHeight_;
}
