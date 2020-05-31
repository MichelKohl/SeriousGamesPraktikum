using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotManager : MonoBehaviour
{
    const string FilePath = @"D:\Data\Uni\SG-P10\Judas.png";

    public void CaptureScreenShot()
    {
        ScreenCapture.CaptureScreenshot(FilePath);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            CaptureScreenShot();
    }
}
