using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CreationAnchor : MonoBehaviour
{
    private static List<CreationAnchor> active = new List<CreationAnchor>();
    public CreationAnchor paired;
    private static int POSITION_ID = Shader.PropertyToID("POSITIONS");
    private static Vector4[] POSITIONS_ARR = new Vector4[8];
    [NonSerialized] public CreationBehaviour createdObject;

    private void OnEnable()
    {
        active.Add(this);
    }

    private void OnDisable()
    {
        active.Remove(this);
    }

    void Update()
    {
        if (active[0] != this) return;
        for (int i = 0; i < POSITIONS_ARR.Length; i++)
        {
            if (i >= active.Count) POSITIONS_ARR[i] = new Vector4();
            else
            {
                POSITIONS_ARR[i] = active[i].transform.position;
                POSITIONS_ARR[i].w = 1;
            }
        }
        Shader.SetGlobalVectorArray(POSITION_ID, POSITIONS_ARR);
    }
}
