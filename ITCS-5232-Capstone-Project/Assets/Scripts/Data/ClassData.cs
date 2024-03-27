using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Class Data", menuName = "ClassData")]
public class ClassData : ScriptableObject
{
    public string className;
    public string classDescription;
    public Color classColorLight;
    public Color classColorDark;
    public Sprite classSprite;
}
