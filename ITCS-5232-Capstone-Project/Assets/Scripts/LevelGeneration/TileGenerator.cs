using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TileGenerator : MonoBehaviour
{
    public abstract GameObject GenerateTile((int, int) position);
}
