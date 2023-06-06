using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyCommon : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
