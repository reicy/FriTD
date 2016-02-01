using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapBuilder : MonoBehaviour
{
    public GameObject grass;
    public GameObject buildplace;
    public GameObject path;
    public GameObject end;
    public GameObject start;

    private List<GameObject> fields = new List<GameObject>(); 

    // Use this for initialization
    private void Start()
    {
        char[,] charMap = new char[,]
        {
            {'G', 'G', 'G', 'G', 'G'},
            {'G', 'E', 'P', 'P', 'G'},
            {'G', 'T', 'T', 'P', 'G'},
            {'G', 'G', 'T', 'S', 'G'},
            {'G', 'G', 'G', 'G', 'G'}
        };

        BuildMap(charMap);
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void BuildMap(char[,] charMap)
    {
        DestroyCurrentMap();
        for (int j = 0; j < charMap.GetLength(1); j++)
        {
            for (int i = 0; i < charMap.GetLength(0); i++)
            {
                GameObject toBuild;
                switch (charMap[i, j])
                {
                    case 'G':
                        toBuild = grass;
                        break;
                    case 'P':
                        toBuild = path;
                        break;
                    case 'T':
                        toBuild = buildplace;
                        break;
                    case 'E':
                        toBuild = end;
                        break;
                    case 'S':
                        toBuild = start;
                        break;
                    default:
                        toBuild = grass;
                        break;
                }
                fields.Add((GameObject) Instantiate(toBuild, new Vector3(j+0.5f, 0, -i-0.5f), Quaternion.identity));
            }
        }
    }

    private void DestroyCurrentMap()
    {
        foreach (var field in fields)
        {
            Destroy(field);
        }
    }
}