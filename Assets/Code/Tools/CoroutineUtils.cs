using System;
using System.Diagnostics;
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
    public static IEnumerator WaitAtLeast(double timeDelay, Action action)
    {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();
        action();
        stopWatch.Stop();

        double elapsedTime = stopWatch.Elapsed.TotalSeconds;
        if (elapsedTime < timeDelay)
        {
            yield return new WaitForSeconds((float)(timeDelay - elapsedTime));
        }
    }
}
