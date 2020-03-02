using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBubbleSystem : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform target1;
    public Transform target2;
    public float xOffset = 50f;
    public float yOffset = 85f;
    bool whichTarget;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("1"))
        {
            whichTarget = true;
        }
        if(Input.GetKeyDown("2"))
        {
            whichTarget = false;
        }

        Vector2 BubblePosition;
        if (whichTarget)
        {
            BubblePosition = RectTransformUtility.WorldToScreenPoint(Camera.main, target1.transform.position);
        }
        else
        {
            BubblePosition = RectTransformUtility.WorldToScreenPoint(Camera.main, target2.transform.position);
        }
        BubblePosition.y += yOffset;
        BubblePosition.x += xOffset;
        transform.position = BubblePosition;
    }
}
