using UnityEngine;
using System.Collections;

public class MapBuilder : MonoBehaviour
{
    public GameObject grass;
    public GameObject buildplace;
    public GameObject path;
    public GameObject end;
    public GameObject start;
    public GameObject mage;
    public GameObject sniper;
    public GameObject grenade;
    public GameObject vermin;
    public GameObject marauder;

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

        buildMap(charMap);
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void buildMap(char[,] charMap)
    {
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
                Instantiate(toBuild, new Vector3(j, 0, -i), Quaternion.identity);
            }
        }
        Instantiate(mage, new Vector3(2, 1, -2), Quaternion.identity);
        Instantiate(grenade, new Vector3(1, 1, -2), Quaternion.identity);
        Instantiate(sniper, new Vector3(2, 1, -3), Quaternion.identity);
        Instantiate(vermin, new Vector3(2, 1, -1), Quaternion.identity);
        Instantiate(marauder, new Vector3(3, 1, -1), Quaternion.identity);
    }
}