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
        yield return null;
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

    // Wait at least time delay (in seconds), and then if predicate is false keep repeating action every interval
    // until condition is met
    //
    // example usage:
    //    StartCoroutine(
    //       RunRepeatedlyUntil(myInitialDelay, myInterval, () => { /* do stuff */ })
    //    );
    public static IEnumerator RunRepeatedlyUntil(float timeDelay, float timeInterval, Action action, Func<bool> predicate)
    {
        yield return new WaitForSeconds(timeDelay);
        if (predicate()) {
            yield return null;
        }

        action();
        while (!predicate())
        {
            yield return new WaitForSeconds(timeInterval);
            action();
        }
    }
}
