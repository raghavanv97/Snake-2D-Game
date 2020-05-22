using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssest : MonoBehaviour
{

    [SerializeField] public Sprite apple;
    [SerializeField] public Sprite snakeBody;

    public static GameAssest i;

    // Start is called before the first frame update
    void Start()
    {
        i = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
