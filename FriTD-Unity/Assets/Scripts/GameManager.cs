using System.Collections.Generic;
using System.IO;
using System.Text;
using Assets.Scripts;
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

        private bool _gameStarted = false;
        private bool _simplePlayer = false;
        private string _command = "";
        private string _aiLevel = "";

        private void Start()
        {
            _mapBuilder = GameObject.Find("MapBuilder").GetComponent<MapBuilder>();
            _infoPanel = GameObject.Find("InfoPanel").GetComponent<InfoPanel>();
            InvokeRepeating("Tic", interval, interval);
        }

        private void Update()
        {
            if (_gameStarted)
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
                    GameObject go = gameObjects[entity.SeqId];
                    go.transform.position = new Vector3(entity.X*1.0f/100, 1, -entity.Y*1.0f/100);
                    go.GetComponentInChildren<Health>().SetPerctHp((float) entity.Perc);
                    go.transform.LookAt(new Vector3(entity.WX*1.0f/100, 1, -entity.WY*1.0f/100));
                    //go.transform.Rotate(new Vector3(0, -90, 0));
                }
                served.Add(entity.SeqId);
            }
        }

        private void OnGUI()
        {
            if (_gameStarted)
            {
                int y = 10;
                var newInterval = GUI.HorizontalSlider(new Rect(Screen.width - 205, y, 200, 25), interval, 0.001f, 0.1f);
                if (newInterval != interval)
                {
                    interval = newInterval;
                    CancelInvoke("Tic");
                    InvokeRepeating("Tic", interval, interval);
                }
                y += 35;
                if (GUI.Button(new Rect(Screen.width - 205, y, 200, 50), "Stop game"))
                {
                    _gameStarted = false;
                    _mapBuilder.DestroyCurrentMap();
                    DestroyAllEntities();
                    _infoPanel.SetText("");
                }
                y += 60;
                if (_manager.GetGameState() == GameState.Waiting)
                {
                    if (GUI.Button(new Rect(Screen.width - 205, y, 200, 50), "Start next level"))
                    {
                        _manager.UnityStartLevel();
                        GameVisualImage image = _dataStore.ExchangeData(null);
                        _mapBuilder.BuildMap(image.Map);
                        //_manager.ExecuteCmd("b_3_1");
                    }
                    y += 60;
                    if (_simplePlayer)
                    {
                        _command = GUI.TextField(new Rect(Screen.width - 205, y, 200, 25), _command);
                        y += 35;
                        if (GUI.Button(new Rect(Screen.width - 205, y, 200, 50), "Execute command"))
                        {
                            _manager.ExecuteCmd(_command);
                        }
                    }
                }
                else if (_manager.GetGameState() == GameState.Lost || _manager.GetGameState() == GameState.Won)
                {
                    _infoPanel.SetText("You " + _manager.GetGameState());
                }
            }
            else
            {
                if (GUI.Button(new Rect(Screen.width - 205, 10, 200, 50), "Start simple game"))
                {
                    _manager = ManagerBuilder.BuildSimplePlayerManager();
                    _manager.PrepareGame();
                    _dataStore = _manager._store;
                    _simplePlayer = true;
                    _gameStarted = true;
                }

                if (GUI.Button(new Rect(Screen.width - 205, 70, 200, 50), "Start AI game"))
                {
                    StreamReader streamReader = new StreamReader("qvalues/" + _aiLevel, Encoding.Default);
                    _manager = ManagerBuilder.BuildObservableAiLearningManager(streamReader);
                    _manager.PrepareGame();
                    _dataStore = _manager._store;
                    _simplePlayer = false;
                    _gameStarted = true;
                }
                _aiLevel = GUI.TextField(new Rect(Screen.width - 205, 125, 200, 25), _aiLevel);
            }
        }

        private void DestroyAllEntities()
        {
            _servedBullets.Clear();
            _servedEnemies.Clear();
            _servedTowers.Clear();
            DestroyNotExistingGameObjects(_bullets, _servedBullets);
            DestroyNotExistingGameObjects(_towers, _servedTowers);
            DestroyNotExistingGameObjects(_enemies, _servedEnemies);
        }

        private void Tic()
        {
            if (_gameStarted)
            {
                _manager.UnityTic();
            }
        }
    }
}