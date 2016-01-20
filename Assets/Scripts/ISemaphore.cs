using UnityEngine;
using System.Collections;

/// <summary>
/// This interface is used to unite 2 semaphores which are used for cars so that
/// RedLightCollider can used them indescriminatevely.
/// </summary>
public interface ISemaphore
{
    bool isCarGreen();
    bool isCarYellow();
    bool isCarRed();
}
