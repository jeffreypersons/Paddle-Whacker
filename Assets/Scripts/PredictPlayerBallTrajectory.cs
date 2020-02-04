using UnityEngine;
using System.Collections;

public class PredictPlayerBallTrajectory : MonoBehaviour
{
    public float lifeTime;
    void Start()
    {
        GetComponent<AudioSource>().pitch = Random.Range(0.75f, 1.25f);
    }
}
