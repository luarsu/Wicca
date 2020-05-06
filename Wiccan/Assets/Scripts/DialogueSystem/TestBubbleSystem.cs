using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBubbleSystem : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform target1;
    public Transform target2;
    public Transform currentTarget;
    public RectTransform BubbleTransform;
    public RectTransform TriangleTransform;
    public float yOffset = 15f;
    public float maxDistanceBubbleToTriangle = 90;

    private float offsetBubbleToTriangle = 72;
    
    void Start()
    {
        currentTarget = target1;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("1"))
        {
            currentTarget = target1;
            setBubble();
        }
        if(Input.GetKeyDown("2"))
        {
            currentTarget = target2;
            setBubble();
        }

        updateBubblePosition();

        
    }

    public void setBubble()
    {
        Vector2 BubblePosition = RectTransformUtility.WorldToScreenPoint(Camera.main, currentTarget.transform.position);

        //Add the offset from the target object to look good and set the position for the triangle
        BubblePosition.y += yOffset;
        TriangleTransform.position = BubblePosition;
        //Add the offset from the triangle to the bubble to look right and set the position of the bubble
        BubblePosition.y += offsetBubbleToTriangle;
        BubbleTransform.position = BubblePosition;
    }

    public void updateBubblePosition()
    {
        //here we just update the x position of the triangle, and we update the position of the bubble when is further than it should
        Vector2 TrianglePosition = RectTransformUtility.WorldToScreenPoint(Camera.main, currentTarget.transform.position);

        //update the triangle positiom
        TrianglePosition.y += yOffset;
        TriangleTransform.position = TrianglePosition;

        //the bubble position only updates the Y unless it's too far from the triangle in the x axis
        Vector2 BubblePosition = new Vector2(BubbleTransform.position.x, TrianglePosition.y + offsetBubbleToTriangle);
        float distanceTriangleToBubble = Mathf.Abs(TrianglePosition.x - BubblePosition.x);

        //If the bubble is too far from the triangle, update the x of the bubble position
        if (distanceTriangleToBubble  > maxDistanceBubbleToTriangle)
        {
            if(BubblePosition.x > TrianglePosition.x)
            {
                BubblePosition.x -= Mathf.Abs(distanceTriangleToBubble - maxDistanceBubbleToTriangle);
            }
            else
            {
                BubblePosition.x += Mathf.Abs(distanceTriangleToBubble - maxDistanceBubbleToTriangle);
            }
        }
        //update the bubble position
        BubbleTransform.position = BubblePosition;
    }
}
