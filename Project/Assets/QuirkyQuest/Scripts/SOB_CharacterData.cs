using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "QuirkyQuest/Character/Data")]
public class SOB_CharacterData : ScriptableObject
{
    [Header("Action Properties")]
    public float swimmingSpeed_;
    public float runningSpeed_;
    public float walkingSpeed_;
    public float rollingSpeed_;
    public float flyingSpeed_;

    [Header("Layers and Collision Detection")]
    public LayerMask groundLayers_;
    public LayerMask waterLayers_;

    [Header("Miscellaneous")]
    public float jumpStrength_;
    public float gravityPull_;
}
