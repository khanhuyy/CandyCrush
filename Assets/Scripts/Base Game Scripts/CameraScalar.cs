using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScalar : MonoBehaviour
{
    private Board board;
    [SerializeField] private float cameraOffset;
    [SerializeField] private float aspectRatio;
    [SerializeField] private float padding;
    [SerializeField] private float yOffset;
    
    void Start()
    {
        board = FindObjectOfType<Board>();
        if(board != null) {
            RepositionCamera(board.width - 1, board.height - 1);
        }    
    }

    void RepositionCamera(float x, float y)
    {
        Vector3 tempPosition = new Vector3(x / 2, y / 2 + yOffset, cameraOffset);
        transform.position = tempPosition;
        if (Camera.main != null)
        {
            if (board.width >= board.height)
            {
                Camera.main.orthographicSize = (board.width / 2 + padding) / aspectRatio;
            }
            else
            {
                Camera.main.orthographicSize = board.height / 2 + padding;
            }
        }
    }
}
