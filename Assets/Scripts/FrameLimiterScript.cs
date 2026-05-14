using System;
using UnityEngine;

public class FrameLimiterScript : MonoBehaviour
{
    private void Awake()
    {
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 60;
    }
}
