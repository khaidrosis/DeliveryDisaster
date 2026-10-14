using UnityEngine;

using UnityEngine.InputSystem;



public class SpawnManager : MonoBehaviour

{

    public GameObject objectPrefab;

    public Transform[] spawnPoints;



    void Update()

    {

        // Testing only 

        if (Keyboard.current.pKey.wasPressedThisFrame)

        {

            SpawnObject();

        }

    }



    public void SpawnObject()

    {

        if (objectPrefab == null)

        {

            Debug.Log("No prefab assigned.");

            return;

        }



        if (spawnPoints.Length == 0)

        {

            Debug.Log("No spawn points assigned.");

            return;

        }



        int randomIndex =

            Random.Range(0, spawnPoints.Length);



        Transform selectedSpawnPoint =

            spawnPoints[randomIndex];



        Instantiate(

            objectPrefab,

            selectedSpawnPoint.position,

            selectedSpawnPoint.rotation

        );



        Debug.Log("Object Spawned!");

    }

}

