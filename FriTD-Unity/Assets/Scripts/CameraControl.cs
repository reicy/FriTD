using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// This class adds control to camera
    /// </summary>
    public class CameraControl : MonoBehaviour
    {

        /// <summary>
        /// Speed of camera
        /// </summary>
        public int speed = 10;

        /// <summary>
        /// Update is called once per frame.
        /// It moves this camera
        /// </summary>
        private void Update()
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Translate(new Vector3(speed*Time.deltaTime, 0, 0));
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Translate(new Vector3(-speed*Time.deltaTime, 0, 0));
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                transform.Translate(new Vector3(0, -speed*Time.deltaTime, 0));
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                transform.Translate(new Vector3(0, speed*Time.deltaTime, 0));
            }
            if (Input.GetKey(KeyCode.PageUp))
            {
                transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));
            }
            if (Input.GetKey(KeyCode.PageDown))
            {
                transform.Translate(new Vector3(0, 0, -speed * Time.deltaTime));
            }
        }
    }
}