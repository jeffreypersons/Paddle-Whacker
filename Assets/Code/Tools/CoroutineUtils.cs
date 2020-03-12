using System;
using System.Collections;
using UnityEngine;


public static class CoroutineUtils
{
    // Wait at least the time delay (in seconds), where the delay takes effect AFTER executing the task
    //
    // example usage:
    //    StartCoroutine(
    //       RunAfter(myDelay, () => { /* do stuff */ })
    //    );
    public static IEnumerator RunNow(Action action)
    {
        yield return new WaitForSeconds(0);
        action();
    }

    // Wait at least the time delay (in seconds), where the delay takes effect AFTER executing the task
    //
    // example usage:
    //    StartCoroutine(
    //       RunAfter(myDelay, () => { /* do stuff */ })
    //    );
    public static IEnumerator RunAfter(float timeDelay, Action action)
    {
        yield return new WaitForSeconds(timeDelay);
        action();
    }

    public static IEnumerator RunRepeatedly(float timeInterval, Action action)
    {
        WaitForSeconds cachedWaitTime = new WaitForSeconds(timeInterval);
        action();
        while (true)
        {
            yield return cachedWaitTime;
            action();
        }
    }
}
