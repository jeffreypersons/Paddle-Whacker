using System;
using System.Collections;
using UnityEngine;

public static class CoroutineUtils
{
    // Wait at least the time delay, where the delay takes effect AFTER executing the task
    //
    // example usage:
    //    StartCoroutine(
    //       WaitAtLeast(myDelay, () => { /* do stuff */ })
    //    );
    public static IEnumerator RunAfter(float timeDelay, Action action)
    {
        yield return new WaitForSeconds(timeDelay);
        action();
    }
}
