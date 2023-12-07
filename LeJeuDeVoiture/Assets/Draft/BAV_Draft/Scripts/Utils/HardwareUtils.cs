using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HardwareUtils
{
    public static void DisplayVRAMInfo(Text vramText)
    {
        int vramSizeMB = SystemInfo.graphicsMemorySize;
        vramText.text = "VRAM: " + vramSizeMB + " MB";
    }
    
    public static void DisplayVRAMInfo(TMP_Text vramText)
    {
        int vramSizeMB = SystemInfo.graphicsMemorySize;
        vramText.text = "VRAM: " + vramSizeMB + " MB";
    }
}
