using UnityEngine;
using System.Collections;

public class RandomSpawner : MonoBehaviour
{
    public GameObject objectToSpawn;
    public float minDelay = 5f;
    public float maxDelay = 10f;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            float randomTime = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(randomTime);

            if (objectToSpawn != null)
            {
                Instantiate(objectToSpawn, transform.position, transform.rotation);
            }
        }
    }
}