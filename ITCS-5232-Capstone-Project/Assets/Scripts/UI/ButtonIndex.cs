using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonIndex : MonoBehaviour
{
    public int index = -1;

    public void SetIndex(int newIndex)
    {
        index = newIndex;
    }

    public int GetIndex()
    {
        return index;
    }
}
