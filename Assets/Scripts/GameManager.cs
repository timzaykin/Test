using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

//Всю основную логика находится в этом классе
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int sizeX = 30;
    public int sizeY = 30;
    public int elementsPerSecond = 2;
    public int roundTime = 60;
    public int scoreToWin = 2000;

    [SerializeField]
    private Figure sampleFigure;
    [SerializeField]
    private ElementColor sampleColor;
    [SerializeField]
    private float timeToChangeSample = 5f;
    [SerializeField]
    private int score = 0;

    public List<Vector2> emptyСells;
    public Element[,] ElementsOnScene; //Двухмерная матрица содержащая все элементы котоорые находятся на игровом поле
    public int[] CountElementsOnScene = new int[3]; // массив с количеством всех видов игровыхобъектов

    public GameObject fameField; //Родитекльский объект для поля
    public GameObject elementPrefab; //Префаб игрового элемента

    private Vector2[] directions;  

    private bool started;

    public void Awake()
    {
        Instance = this;
        ////Заранее создаем необходимое количество объектов в пуле
        if(ManagerPool.Instance.pools.Count<1) ManagerPool.Instance.AddPool(PoolType.Entitys, true).PopulateWith(GameManager.Instance.elementPrefab, GameManager.Instance.sizeX * GameManager.Instance.sizeY, 100);
    }

    public void Start()
    {

        //инициализируем все необходимые переменные
        ElementsOnScene = new Element[sizeY,sizeX];
        emptyСells = new List<Vector2>();
        directions = new Vector2[8] { new Vector2(-1, 1), Vector2.up, new Vector2(1, 1), Vector2.left, Vector2.right, new Vector2(-1, -1), Vector2.down, new Vector2(1, -1) };
        for (int j = 0; j < sizeY; j++)
        {
            for (int i = 0; i < sizeX; i++)
            {
                emptyСells.Add(new Vector2(i, j));
            }
        }

        // C помощью UniRX превращяем корутины в потоки и объединяем в один поток
        Observable.WhenAll(
            Observable.FromCoroutine(StatrTimer).SelectMany(ChangeSampleCorutine),
            Observable.FromCoroutine(StatrTimer).SelectMany(TimerCorutine).SelectMany(EndGame)
            ).Subscribe(_ => { Debug.Log("End"); }).AddTo(this);

    }
        
    public void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    //функция для заполнения всего поля в начале игры 
    private void SpawnAllElements () {
        for (int j = 0; j < sizeY; j++)
        {
            for (int i = 0; i < sizeX; i++)
            {
                Element element = ManagerPool.Instance.Spawn<Element>(PoolType.Entitys,
                                                                    elementPrefab,
                                                                    Vector3.zero,
                                                                    Quaternion.identity, 
                                                                    fameField.transform);
                SetOnRandomEmptyCell(element);
            }
        }

    }


    //Функция для получения наименее представленного типа объектов на поле
    public int GetElementType() {
        int tempCount= 0;
        int tempValue = CountElementsOnScene[0];
        for (int i = 0; i < CountElementsOnScene.Length; i++)
        {
            if (CountElementsOnScene[i] < tempValue)
            {
                tempValue = CountElementsOnScene[i];
                tempCount = i;
            }
        }
        return tempCount;
    }

    
    public void SetOnRandomEmptyCell(Element element) {
        if (element == null) {
            element = ManagerPool.Instance.Spawn<Element>(PoolType.Entitys,
                                                                    elementPrefab,
                                                                    Vector3.zero,
                                                                    Quaternion.identity,
                                                                    fameField.transform);
        }

        Vector2 randomPosition = emptyСells[Random.Range(0, emptyСells.Count)];
        element.SetPosition((int)randomPosition.x, (int)randomPosition.y);
        emptyСells.Remove(randomPosition);
    }

    //Корутина для изменения элемента - образца через заданный промежуток времени
    IEnumerator ChangeSampleCorutine() {
        ChangeSample();
        while (true) {
            yield return new WaitForSeconds(timeToChangeSample);
            if (!started) continue;
            ChangeSample();
            score -= 50;
            UIManager.Instance.SetScoreValue(score);
        }
    }

    private void ChangeSample() {
        sampleFigure = (Figure)Random.Range(0, 3);
        sampleColor = (ElementColor)Random.Range(0, 7);
        UIManager.Instance.SetSampleFigure(sampleFigure, sampleColor);
        //Debug.Log(sampleFigure.ToString() + "   " + sampleColor.ToString());
    }


    //Корутина для отсчета времени и спавна новых элементах в незанятых ячейках поля
    IEnumerator TimerCorutine() {
        SpawnAllElements();
        started = true;
        for (int i = roundTime; i > 0; i--)
        {
            UIManager.Instance.SetTimerValue(i);
            yield return new WaitForSeconds(1f);

                if (emptyСells.Count>0) {
                    for (int j = 0; j < elementsPerSecond && emptyСells.Count > 0; j++)
                    {
                        SetOnRandomEmptyCell(null);
                    }
                }
        }
        started = false;
    }


    //предигровой таймер
    IEnumerator StatrTimer() {
        UIManager.Instance.SetActiveStartTimer(true);
        for (int i = 3; i > 0; i--)
        {
            UIManager.Instance.SetStartTimerValue(i);
            yield return new WaitForSeconds(1f);
        }
        UIManager.Instance.SetActiveStartTimer(false);
    }


    IEnumerator EndGame() {
        if (score >= scoreToWin)
        {
            UIManager.Instance.ShowEndGameMessage("You win!");
        }
        else
        {
            UIManager.Instance.ShowEndGameMessage("You loose!");
        }

        if (score > Settings.Instance.hiScore) Settings.Instance.SetHiScore(score);

        for (int i = 10; i > 0; i--)
        {
            yield return new WaitForSeconds(1f);
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }


    // Функция проверки клика по фигуре
    public void CheckClickOnFigure(Element element) {
        //если цвет и тип фигуры совпадают получаем все элементы данного типа на сцене
        if (element.GetFigure() == sampleFigure && element.GetColor() == sampleColor && started) {
            List<Element> elementsToDespawn = new List<Element>();
            for (int j = 0; j < sizeY; j++)
            {
                for (int i = 0; i < sizeX; i++)
                {
                    if (ElementsOnScene[i, j] != null && ElementsOnScene[i,j].GetFigure() == sampleFigure && ElementsOnScene[i, j].GetColor() == sampleColor)
                    {
                        elementsToDespawn.Add(ElementsOnScene[i, j]);
                    }
                }
            }

            //удаляем их
            foreach (var item in elementsToDespawn)
            {
                ManagerPool.Instance.Despawn(PoolType.Entitys, item.gameObject);
            }
            //и добавляем игроку очки
            score += 200;
            UIManager.Instance.SetScoreValue(score);
        }
        else
        {
            // в противном случае просто перекрашиваем все элементы этого типа и цвета
            for (int j = 0; j < sizeY; j++)
            {
                for (int i = 0; i < sizeX; i++)
                {
                    if (ElementsOnScene[i, j] != null && ElementsOnScene[i, j].GetFigure() == element.GetFigure() && ElementsOnScene[i, j].GetColor() == element.GetColor())
                    {
                        ElementsOnScene[i, j].SetRandomColor();
                    }
                }
            }
        }
    }

    //Функция для иземения цвета соседних элементов
    public void EditNeighbors(Element element) {

        foreach (var direction in directions)
        {
            Vector2 tempVector = element.position + direction;
            if (tempVector.x >= 0 && 
                tempVector.y >= 0 && 
                tempVector.x < sizeX && 
                tempVector.y < sizeY &&
                ElementsOnScene[(int)tempVector.x, (int)tempVector.y] != null)
            { 
                ElementsOnScene[(int)tempVector.x, (int)tempVector.y].ChangeColor(element.GetColor());
            }

        }
    }

}
