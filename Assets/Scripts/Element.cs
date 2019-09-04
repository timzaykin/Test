using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element : MonoBehaviour, IPoolable
{
    public Vector2 position;

    [SerializeField]
    private Figure currentFigure;
    [SerializeField]
    private ElementColor currentColor;

    [SerializeField]
    private Sprite[] sprites;

    private SpriteRenderer spriteRenderer;

    public static Color GetColor(ElementColor _color)
    {
        switch (_color)
        {
            case ElementColor.red:
                return Color.red;
            case ElementColor.orange:
                return new Color(1, 0.35f, 0);
            case ElementColor.yellow:
                return Color.yellow;
            case ElementColor.green:
                return Color.green;
            case ElementColor.lightBlue:
                return new Color(0, 1, 1);
            case ElementColor.blue:
                return Color.blue;
            case ElementColor.violet:
                return Color.magenta;
            default:
                return Color.white;
        }
    }

    public void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ChangeColor(ElementColor _color) {
        currentColor = _color;
        spriteRenderer.color = GetColor(currentColor);
    }

    public void SetRandomColor() {
        currentColor = (ElementColor)Random.Range(0, 7);
        spriteRenderer.color = GetColor(currentColor);
    }

    public void SetPosition(int x, int y) {
        position = new Vector2(x, y);
        transform.position = new Vector3(x, y, 0);
        GameManager.Instance.ElementsOnScene[(int)position.x, (int)position.y] = this;
    }

    public Figure GetFigure()
    {
        return currentFigure;
    }

    public ElementColor GetColor()
    {
        return currentColor;
    }

    private void OnMouseDown()
    {
        GameManager.Instance.CheckClickOnFigure(this);
    }

    public void OnSpawn()
    {
        // при спавне объекта задаем получаем тип текущего объекта, и рандомный цвет
        int elemetnType = GameManager.Instance.GetElementType();
        GameManager.Instance.CountElementsOnScene[elemetnType]++;
        currentFigure = (Figure)elemetnType;
        spriteRenderer.sprite = sprites[(int)currentFigure];
        SetRandomColor();


    }

    public void OnDespawn()
    {
        //при деспавне меняем цвет соседей, и очищаем ячейку в матрице элементов
        GameManager.Instance.EditNeighbors(this);
        GameManager.Instance.ElementsOnScene[(int)position.x, (int)position.y] = null;
        GameManager.Instance.CountElementsOnScene[(int)currentFigure]--;
        GameManager.Instance.emptyСells.Add(position);
    }
}



