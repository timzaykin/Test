using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public float minX, maxX, minY, maxY;
    public float speed = 100f;
    public Vector3 mousePos;

    public void Start()
    {
        maxX = GameManager.Instance.sizeX;
        maxY = GameManager.Instance.sizeX;
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = Input.mousePosition;
        if (mousePos.x > Screen.width - 50 && transform.position.x<maxX) MoveRight();
        if (mousePos.x < 50 && transform.position.x > minX) MoveLeft();
        if (mousePos.y > Screen.height - 50 && transform.position.y < maxY) MoveUp();
        if (mousePos.y < 50 && transform.position.y > minY) MoveDown();
    }

    public void MoveRight() {
        transform.position += Vector3.right * Time.deltaTime *speed;
    }

    public void MoveLeft() {
        transform.position += Vector3.left * Time.deltaTime * speed;
    }

    public void MoveUp() {
        transform.position += Vector3.up * Time.deltaTime * speed;
    }

    public void MoveDown() {
        transform.position += Vector3.down * Time.deltaTime * speed;
    }
}
