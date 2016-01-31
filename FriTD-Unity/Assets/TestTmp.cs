using System.Collections.Generic;
using UnityEngine;
using Manager.Core;
using TD.Core;
using TD.Enums;

public class TestTmp : MonoBehaviour {

    Manager.Core.Manager manager;
    DataStore dataStore;

    public GameObject marauderPrefab;

    private Dictionary<int, GameObject> enemies = new Dictionary<int, GameObject>(); 


    // Use this for initialization
    void Start ()
	{
        manager = ManagerBuilder.BuildSimplePlayerManager();
        manager.PrepareGame();
	    dataStore = manager._store;
        float interval = 0.1f;
        InvokeRepeating("Tic", interval, interval);
	}
	
	// Update is called once per frame
	void Update ()
	{
	    GameVisualImage image = dataStore.ExchangeData(null);
	    if (image != null)
	    {
            Debug.Log("------- update --------");
            foreach (var enemy in image.Enemies)
	        {
	            Debug.Log("ID:" + enemy.Id + " SeqID:" + enemy.SeqId + " HP:" + enemy.Perc + " X:" + enemy.X + " Y:" + enemy.Y);
	            if (!enemies.ContainsKey(enemy.SeqId))
	            {
	                enemies.Add(enemy.SeqId,
	                    (GameObject) Instantiate(marauderPrefab, new Vector3(enemy.X * 1.0f / 100, 1, -enemy.Y * 1.0f / 100), Quaternion.identity));
	            }
	            else
	            {
	                enemies[enemy.SeqId].transform.position = new Vector3(enemy.X*1.0f/100, 1, -enemy.Y*1.0f/100);
	            }
	        }
	    }
	}

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 200, 50), "Start level"))
        {
            manager.UnityStartLevel();
        }
    }

    private void Tic()
    {
        if (manager.GetGameState() == GameState.InProgress)
        {
            manager.UnityTic();
        }
    }
}
