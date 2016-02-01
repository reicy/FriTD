using System.Collections.Generic;
using Manager.Core;
using TD.Core;
using TD.Enums;
using UnityEngine;

namespace Assets
{
    internal enum EntityType
    {
        Tower,
        Bullet,
        Enemy
    };

    public class GameManager : MonoBehaviour
    {
        public GameObject mage;
        public GameObject sniper;
        public GameObject grenade;
        public GameObject mageBullet;
        public GameObject sniperBullet;
        public GameObject grenadeBullet;
        public GameObject vermin;
        public GameObject marauder;
        public float interval = 0.05f;

        private Manager.Core.Manager _manager;
        private DataStore _dataStore;
        private Dictionary<int, GameObject> _enemies = new Dictionary<int, GameObject>();
        private Dictionary<int, GameObject> _towers = new Dictionary<int, GameObject>();
        private Dictionary<int, GameObject> _bullets = new Dictionary<int, GameObject>();
        private HashSet<int> _servedEnemies = new HashSet<int>();
        private HashSet<int> _servedTowers = new HashSet<int>();
        private HashSet<int> _servedBullets = new HashSet<int>();
        private MapBuilder _mapBuilder;
        private InfoPanel _infoPanel;

        private void Start()
        {
            _mapBuilder = GameObject.Find("MapBuilder").GetComponent<MapBuilder>();
            _infoPanel = GameObject.Find("InfoPanel").GetComponent<InfoPanel>();
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
                _servedEnemies.Clear();
                _servedTowers.Clear();
                _servedBullets.Clear();
                UpdateGameObjects(image.Enemies, _enemies, EntityType.Enemy, _servedEnemies);
                UpdateGameObjects(image.Projectiles, _bullets, EntityType.Bullet, _servedBullets);
                UpdateGameObjects(image.Towers, _towers, EntityType.Tower, _servedTowers);

                DestroyNotExistingGameObjects(_enemies, _servedEnemies);
                DestroyNotExistingGameObjects(_bullets, _servedBullets);
                DestroyNotExistingGameObjects(_towers, _servedTowers);

                _infoPanel.updateInfo(image.Hp, image.Gold);
            }
        }

        private void DestroyNotExistingGameObjects(Dictionary<int, GameObject> gameObjects, HashSet<int> served)
        {
            List<int> toDelete = new List<int>();
            foreach (var obj in gameObjects)
            {
                if (!served.Contains(obj.Key))
                {
                    toDelete.Add(obj.Key);
                }
            }

            foreach (var i in toDelete)
            {
                GameObject toDelGameObject = gameObjects[i];
                gameObjects.Remove(i);
                Destroy(toDelGameObject);
            }
        }

        private void UpdateGameObjects(List<IDisplayableObject> entities, Dictionary<int, GameObject> gameObjects,
            EntityType entityType, HashSet<int> served)
        {
            foreach (var entity in entities)
            {
                if (!gameObjects.ContainsKey(entity.SeqId))
                {
                    GameObject toBuild = null;
                    switch (entityType)
                    {
                        case EntityType.Enemy:
                            switch (entity.Id)
                            {
                                case 0:
                                    toBuild = marauder;
                                    break;
                                case 1:
                                    toBuild = vermin;
                                    break;
                            }
                            break;
                        case EntityType.Tower:
                            switch (entity.Id)
                            {
                                case 0:
                                    toBuild = sniper;
                                    break;
                                case 1:
                                    toBuild = grenade;
                                    break;
                                case 2:
                                    toBuild = mage;
                                    break;
                            }
                            break;
                        case EntityType.Bullet:
                            switch (entity.Id)
                            {
                                case 0:
                                    toBuild = sniperBullet;
                                    break;
                                case 1:
                                    toBuild = grenadeBullet;
                                    break;
                                case 2:
                                    toBuild = mageBullet;
                                    break;
                            }
                            break;
                    }
                    gameObjects.Add(entity.SeqId,
                        (GameObject)
                            Instantiate(toBuild, new Vector3(entity.X*1.0f/100, 1, -entity.Y*1.0f/100),
                                Quaternion.identity));
                }
                else
                {
                    gameObjects[entity.SeqId].transform.position = new Vector3(entity.X*1.0f/100, 1, -entity.Y*1.0f/100);
                }
                served.Add(entity.SeqId);
            }
        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(Screen.width - 205, 10, 200, 50), "Start level"))
            {
                _manager.UnityStartLevel();
                GameVisualImage image = _dataStore.ExchangeData(null);
                _mapBuilder.BuildMap(image.Map);
                //_manager.ExecuteCmd("b_3_0");
            }
        }

        private void Tic()
        {
            _manager.UnityTic();
        }
    }
}