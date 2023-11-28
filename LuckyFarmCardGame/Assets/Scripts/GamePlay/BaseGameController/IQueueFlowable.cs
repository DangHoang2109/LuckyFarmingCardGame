using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IQueueFlowable
{
    public bool IsReadyForNext { get; set; }
}
