using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ENU_Coastalness : uint
{
    kCoastalness_East = 1u,
    kCoastalness_EastInland = 2u,
    kCoastalness_Inland = 4u,
    kCoastalness_WestInland = 8u,
    kCoastalness_West = 16u
}
