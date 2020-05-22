using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Snake : MonoBehaviour
{

    enum SnakeDirection
    {
        left,
        right,
        up,
        down
    }

    private Vector2Int snakeHeadPosition;
    private float snakeMoveTimer;
    private float snakeMoveTimerMax;
    private SnakeDirection snakeMovePosition;
    private Vector2Int currentSnakeMovePosition;
    private int collisionOfBody = 0;

    private int snakeBodySize = 0;
    private List<SnakePosition> snakeBodyPositionList;
    private List<Transform>  snakeBodyTransformPosition;

    private int previousSnakeBodySize = 0;

    private GameObject apple;

    private int W, H;
    

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        snakeHeadPosition = new Vector2Int(10, 10);
        snakeMoveTimerMax = .1f;
        snakeMoveTimer = snakeMoveTimerMax;

        snakeBodyPositionList = new List<SnakePosition>();
        snakeBodyTransformPosition = new List<Transform>();


        W = Screen.width;
        H = Screen.height;
        Debug.Log("Width " + W + " height " + H);


        generateAppleIcon();


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if(snakeMovePosition != SnakeDirection.down)
            {
                snakeMovePosition = SnakeDirection.up;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (snakeMovePosition != SnakeDirection.up)
            {
                snakeMovePosition = SnakeDirection.down;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (snakeMovePosition != SnakeDirection.right)
            {
                snakeMovePosition = SnakeDirection.left;
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (snakeMovePosition != SnakeDirection.left)
            {
                snakeMovePosition = SnakeDirection.right;
            }
        }


        snakeMoveTimer += Time.deltaTime;
        if (snakeMoveTimer >= snakeMoveTimerMax)
        {
            if(snakeBodySize != previousSnakeBodySize)
            {
                previousSnakeBodySize = snakeBodySize;
                generateSnakeBody();
                generateAppleIcon();
            }

            SnakePosition previousSnakePosition = new SnakePosition(snakeHeadPosition, snakeMovePosition);
            snakeBodyPositionList.Insert(0, previousSnakePosition);
            if (snakeBodyPositionList.Count > snakeBodySize)
            {
                snakeBodyPositionList.RemoveAt(snakeBodyPositionList.Count - 1);
            }

            
            switch (snakeMovePosition)
            {
                case SnakeDirection.up:
                    currentSnakeMovePosition = new Vector2Int(0, 1);
                    break;

                case SnakeDirection.down:
                    currentSnakeMovePosition = new Vector2Int(0, -1);
                    break;

                case SnakeDirection.left:
                    currentSnakeMovePosition = new Vector2Int(-1, 0);
                    break;

                case SnakeDirection.right:
                    currentSnakeMovePosition = new Vector2Int(1, 0);
                    break;


                default:
                    currentSnakeMovePosition = new Vector2Int(0, 0);
                    break;
                       
            }
            snakeMoveTimer -= snakeMoveTimerMax;
            snakeHeadPosition += currentSnakeMovePosition;

            updateSnakePosition();

        }

    }

    private void updateSnakePosition()
    {
        transform.position = new Vector2(snakeHeadPosition.x, snakeHeadPosition.y);
        transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(currentSnakeMovePosition) + 90);

        for (int i = 0; i < snakeBodyPositionList.Count; i++)
        {
            snakeBodyTransformPosition[i].position = snakeBodyPositionList[i].getSnakePosition();
            float angle = 0;
            switch (snakeBodyPositionList[i].getDirection())
            {
                case SnakeDirection.up:
                    angle = 0;
                    break;

                case SnakeDirection.down:
                    angle = 180;
                    break;

                case SnakeDirection.left:
                    angle = -90;
                    break;

                case SnakeDirection.right:
                    angle = 90;
                    break;

            }
            snakeBodyTransformPosition[i].eulerAngles = new Vector3(0, 0, angle);
        }
    }

    private float GetAngleFromVector(Vector2Int dir)
    {
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if(n< 0)
        {
            n += 360;
        }
        return n;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("collided");
        Debug.Log(collision.gameObject.name);

        if (collision.gameObject.name.Equals("food"))
        {
            Destroy(apple);
            snakeBodySize++;
        }
        else if (collision.gameObject.name.Equals("body"))
        {
            if (collisionOfBody != 1)
            {
                collisionOfBody=1;
            }
            else {
                Time.timeScale = 0f;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
        else
        {
            Time.timeScale = 0f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        
    }

   

    private void generateAppleIcon()
    {
        apple = new GameObject("food");
        //Debug.Log(apple.AddComponent<RectTransform>().sizeDelta.x);
        //apple.transform.localScale = new Vector3(5f, 5f, 1f);
        apple.AddComponent<SpriteRenderer>().sprite = GameAssest.i.apple;

        do
        {
            apple.transform.position = new Vector2(Random.Range(1, 20), Random.Range(1, 20));
        } while (checkGeneratedAppleSpawnOnBody() && apple.transform.position == transform.position);

        apple.AddComponent<PolygonCollider2D>();
    }

    private bool checkGeneratedAppleSpawnOnBody()
    {
        foreach(SnakePosition snakePosition in snakeBodyPositionList)
        {
            if (snakePosition.getSnakePosition() == apple.transform.position)
                return false;
        }
        return true;
    }

    private void generateSnakeBody()
    {
        GameObject snakeBody = new GameObject("body");
        snakeBody.AddComponent<SpriteRenderer>().sprite = GameAssest.i.snakeBody;

        snakeBody.AddComponent<PolygonCollider2D>();
        //snakeBody.GetComponent<PolygonCollider2D>().isTrigger = true;
        snakeBodyTransformPosition.Add(snakeBody.transform);
        snakeBody.GetComponent<SpriteRenderer>().sortingOrder = -snakeBodyTransformPosition.Count;
        updateSnakePosition();
    }


     private class SnakePosition
    {
        private Vector3 position;
        private SnakeDirection direction;

        public SnakePosition(Vector2 position, SnakeDirection direction)
        {
            this.position = position;
            this.direction = direction;
        }

        public Vector3 getSnakePosition()
        {
            return position;
        }

        public SnakeDirection getDirection()
        {
            return direction;
        }
    }
}
