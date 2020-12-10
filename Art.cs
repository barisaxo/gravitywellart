using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM : MonoBehaviour
{
    public Camera cam;

    public List<SpriteRenderer> tileColor = new List<SpriteRenderer>();
    public List<Vector2> anchorPoint = new List<Vector2>();
    public List<float> rotSpeed = new List<float>();

    public float redSpeed, redRange, redAddition,
                 greenSpeed, greenRange, greenAddition,
                 blueSpeed, blueRange, blueAddition;

    void Start()
    {
        //Import the sprite asset
        Sprite sprite = Resources.Load<Sprite>("ART/atom");

        //Set up the camera
        cam = gameObject.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 10;
        cam.backgroundColor = Color.black;

        float vertical = cam.orthographicSize;
        float horizontal = vertical * ((float)Screen.width / Screen.height);

        //Columns and rows for the grid
        int columns = (int)((horizontal + 2) * 2);
        int rows = (int)((vertical + 2) * 2);

        //Place the camera in the center of our grid
        transform.position = new Vector3((int)horizontal + 1, (rows * .5f) - .5f, -10);

        //Grab some random Colors and ranges for the sinewave
        //How frequently the color value changes
        redSpeed = Random.Range(.20f, .80f);
        greenSpeed = Random.Range(.20f, .80f);
        blueSpeed = Random.Range(.20f, .80f);
        //How far the color value can change
        redRange = Random.Range(.2f, .35f);
        greenRange = Random.Range(.2f, .35f);
        blueRange = Random.Range(.2f, .35f);
        //Add some color value on top
        redAddition = Random.Range(.35f, .5f);
        greenAddition = Random.Range(.35f, .5f);
        blueAddition = Random.Range(.35f, .5f);

        //Create a parent object to keep our heirchy clean
        Transform board = new GameObject("Board").transform;

        //Create a grid of tiles to color and gravitate towards the mousePos
        for (int c = -1; c < columns; c++)
        {
            for (int r = -1; r < rows; r++)
            {
                GameObject gO = new GameObject("Tile: " + c + ", " + r);
                gO.transform.position = new Vector2(c, r);
                gO.transform.parent = board;
                anchorPoint.Add(gO.transform.position);

                SpriteRenderer sr = gO.AddComponent<SpriteRenderer>();
                sr.sprite = sprite;
                tileColor.Add(sr);

                //Attempts to give a rotation speed of at least +/- 50
                for (int i = 0; i < 4; i++)
                {
                    float n = Random.Range(-150f, 150f);

                    if (n > 50 || n < -50)
                    { rotSpeed.Add(n); break; }

                    if (i == 3)
                    { rotSpeed.Add(n); break; }
                }
            }
        }
    }

    void Update()
    {
        //Grab the mousePos
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        //Update the color
        float redColor = (Mathf.Sin(Time.time * redSpeed) * redRange) + redAddition;
        float greenColor = (Mathf.Sin(Time.time * greenSpeed) * greenRange) + greenAddition;
        float blueColor = (Mathf.Sin(Time.time * blueSpeed) * blueRange) + blueAddition;
        Color newColor = new Color(redColor, greenColor, blueColor);

        //Cycle through the tiles
        for (int i = 0; i < tileColor.Count; i++)
        {
            //Grab the tiles components
            SpriteRenderer sr = tileColor[i];
            Vector2 ap = anchorPoint[i];

            //Find distances
            float distFromMouse = Vector2.Distance(mousePos, sr.transform.position);
            float distFromAnchor = Vector2.Distance(sr.transform.position, ap);

            //A little tug of war between the mousePos and the anchorPoint
            sr.transform.position = Vector2.MoveTowards(sr.transform.position,
                mousePos, Time.deltaTime);
            sr.transform.position = Vector2.MoveTowards(sr.transform.position,
                ap, Time.deltaTime * distFromAnchor);

            //Rotate the tile
            sr.gameObject.transform.Rotate(new Vector3(0, 0, Time.deltaTime * rotSpeed[i]));

            //Color the tile
            StartCoroutine(RefreshColor(sr, newColor, distFromMouse));
        }
    }

    //Colors a tile with a time delay related to the distance from the mouse
    IEnumerator RefreshColor(SpriteRenderer sr, Color newColor, float dist)
    {
        yield return new WaitForSeconds(dist * .15f);
        sr.color = newColor;
    }
}