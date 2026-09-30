using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class floatingText : MonoBehaviour
{
    [SerializeField] private float fadingTime;
    [SerializeField] private float height;
    private float TargetPosY;
    // Update is called once per frame

    private void Start()
    {
        TargetPosY = gameObject.transform.position.y + height;
    }

    void Update()
    {
        Color textColor = gameObject.GetComponent<TextMeshPro>().color;
        if (textColor.a > 0.05)
        {
            gameObject.GetComponent<TextMeshPro>().color = Color.Lerp(textColor,
                                                 new Color(textColor.r, textColor.g, textColor.b, 0),
                                                 Time.deltaTime * fadingTime);
        }
        else
        {
            gameObject.GetComponent<TextMeshPro>().color = new Color(textColor.r, textColor.g, textColor.b, 0);
        }
            

        /*gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position,
                                                            new Vector3(gameObject.transform.position.x,
                                                                        height,
                                                                        gameObject.transform.position.z),
                                                            fadingTime * Time.deltaTime);*/

        gameObject.GetComponent<RectTransform>().position = Vector3.MoveTowards(gameObject.GetComponent<RectTransform>().position,
                                                            new Vector3(gameObject.GetComponent<RectTransform>().position.x,
                                                                        height,
                                                                        gameObject.GetComponent<RectTransform>().position.z),
                                                            fadingTime * Time.deltaTime);

    }
}
