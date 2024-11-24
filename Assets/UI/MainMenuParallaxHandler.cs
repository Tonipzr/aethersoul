using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuParallaxHandler : MonoBehaviour
{
    [SerializeField]
    private float speed = 25f;
    [SerializeField]
    private Image clouds;
    [SerializeField]
    private Image clouds2;
    [SerializeField]
    private Image clouds3;

    private float leftBounds = -2472.799f;
    private float rightBounds = 2472.799f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        clouds.transform.Translate(Vector3.right * speed * Time.deltaTime);
        clouds2.transform.Translate(Vector3.right * speed * Time.deltaTime);
        clouds3.transform.Translate(Vector3.right * speed * Time.deltaTime);

        if (clouds.transform.position.x > rightBounds)
        {
            clouds.transform.position = new Vector3(leftBounds, clouds.transform.position.y, clouds.transform.position.z);
        }

        if (clouds2.transform.position.x > rightBounds)
        {
            clouds2.transform.position = new Vector3(leftBounds, clouds2.transform.position.y, clouds2.transform.position.z);
        }

        if (clouds3.transform.position.x > rightBounds)
        {
            clouds3.transform.position = new Vector3(leftBounds, clouds3.transform.position.y, clouds3.transform.position.z);
        }
    }
}
