using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taginfo 
{
    public string tagCode { get; set; }
    public bool isRecongnized { get; set; }


    public override string ToString()
    {
        return "±Í«©–≈œ¢£∫"+tagCode+isRecongnized;
    }
}
