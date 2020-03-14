using System;
using System.Collections;
using UnityEngine;


public static class MathUtils
{
    // assuming startA <= endA and startB <= endB, two ranges overlap in the case that
    // there exists a value C such that: startA <= C <= endA and startB <= C <= endB
    // see: https://stackoverflow.com/questions/3269434/whats-the-most-efficient-way-to-test-two-integer-ranges-for-overlap
    public static bool IsOverlappingRange(float startA, float endA, float startB, float endB)
    {
        return startA <= endB && startB <= endA;
    }
}
