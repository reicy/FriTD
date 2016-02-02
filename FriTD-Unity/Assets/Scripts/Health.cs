using UnityEngine;

namespace Assets.Scripts
{
    public class Health : MonoBehaviour
    {
        public float x = 0.2f;
        public float y = 0.2f;
        public float z = 0f;

        // Use this for initialization
        void Start () {
        }
	
        // Update is called once per frame
        void Update () {
        }

        // perct in <0, 1>
        public void SetPerctHp(float perct)
        {
            gameObject.transform.localScale = new Vector3(x, y, perct);
        }
    }
}
