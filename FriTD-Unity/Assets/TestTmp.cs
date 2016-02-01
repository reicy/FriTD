using System.Collections.Generic;
using Manager.Core;
using TD.Core;
using TD.Enums;
using UnityEngine;

namespace Assets
{
    public class TestTmp : MonoBehaviour
    {
        public GameObject marauderPrefab;
        public float interval = 0.05f;

        private Manager.Core.Manager _manager;
        private DataStore _dataStore;
        private Dictionary<int, GameObject> _enemies = new Dictionary<int, GameObject>();
        private MapBuilder _mapBuilder;

        private void Start()
        {
            _mapBuilder = GameObject.Find("MapBuilder").GetComponent<MapBuilder>();
            _manager = ManagerBuilder.BuildSimplePlayerManager();
            _manager.PrepareGame();
            _dataStore = _manager._store;
            InvokeRepeating("Tic", interval, interval);
        }

        private void Update()
        {
            GameVisualImage image = _dataStore.ExchangeData(null);
            if (image != null)
            {
                Debug.Log("------- update --------");
                foreach (var enemy in image.Enemies)
                {
                    Debug.Log("ID:" + enemy.Id + " SeqID:" + enemy.SeqId + " HP:" + enemy.Perc + " X:" + enemy.X + " Y:" +
                              enemy.Y);
                    if (!_enemies.ContainsKey(enemy.SeqId))
                    {
                        _enemies.Add(enemy.SeqId,
                            (GameObject)
                                Instantiate(marauderPrefab, new Vector3(enemy.X*1.0f/100, 1, -enemy.Y*1.0f/100),
                                    Quaternion.identity));
                    }
                    else
                    {
                        _enemies[enemy.SeqId].transform.position = new Vector3(enemy.X*1.0f/100, 1, -enemy.Y*1.0f/100);
                    }
                }
            }
        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(10, 10, 200, 50), "Start level"))
            {
                _manager.UnityStartLevel();
                GameVisualImage image = _dataStore.ExchangeData(null);
                _mapBuilder.BuildMap(image.Map);
            }
        }

        private void Tic()
        {
            _manager.UnityTic();
        }
    }
}
