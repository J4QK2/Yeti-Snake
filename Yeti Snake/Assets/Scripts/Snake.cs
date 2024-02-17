using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Snake : MonoBehaviour
{
    [SerializeField]private int _xSize, _ySize; 
    [SerializeField]private GameObject _block; 
    [SerializeField]private GameObject _food;
    [SerializeField]private TextMeshProUGUI _points;
    [SerializeField]private GameObject _gameOverUI;
    [SerializeField]private GameObject _pauseUI;
    [SerializeField]private GameObject _pauseButton;
    GameObject _head; 
    List<GameObject> _tail; 
    Vector2 _dir;


    // Start is called before the first frame update
    void Start()
    {
        timeBetweenMovements = 0.5f;
        _dir = Vector2.right;
        createGrid();
        createPlayer(); 
        spawnFood(); 
        //_block.SetActive(false);
        isAlive = true;
    }

    private Vector2 getRandomPos(){
        return new Vector2(Random.Range(-_xSize/2+1, _xSize/2), Random.Range(-_ySize/2+1, _ySize/2)); 
    }

    private bool containedInSnake(Vector2 spawnPos){
        bool isInHead = spawnPos.x == _head.transform.position.x && spawnPos.y == _head.transform.position.y;
        bool isInTail = false; 
        foreach (var item in _tail)
        {
            if(item.transform.position.x == spawnPos.x && item.transform.position.y == spawnPos.y){
                isInTail = true; 
            }
        }
        return isInHead || isInTail;
    }
    GameObject food;
    bool isAlive;

    private void spawnFood(){
        Vector2 spawnPos = getRandomPos();
        while(containedInSnake(spawnPos)){
            spawnPos = getRandomPos();
        }
        food = Instantiate(_food);
        food.transform.position = new Vector3(spawnPos.x, spawnPos.y, 0);
        food.SetActive(true);
    }

    private void createPlayer(){
        _head = Instantiate(_block) as GameObject; 
        _head.GetComponent<SpriteRenderer>().color = Color.green;
        _tail = new List<GameObject>(); 
    }

    private void createGrid(){
        for(int x = 0; x <= _xSize; x++){
            GameObject borderBottom = Instantiate(_block) as GameObject; 
            borderBottom.GetComponent<Transform>().position = new Vector3(x-_xSize/2, -_ySize/2, 0);

            GameObject borderTop = Instantiate(_block) as GameObject; 
            borderTop.GetComponent<Transform>().position = new Vector3(x-_xSize/2, _ySize-_ySize/2, 0);
        }

        for(int y = 0; y <= _ySize; y++){
            GameObject borderRight = Instantiate(_block) as GameObject;
            borderRight.GetComponent<Transform>().position = new Vector3(-_xSize/2, y-(_ySize/2), 0); 

            GameObject borderLeft = Instantiate(_block) as GameObject;
            borderLeft.GetComponent<Transform>().position = new Vector3(_xSize-(_xSize/2), y-(_ySize/2), 0); 
        }

    }

    float passedTime, timeBetweenMovements;


    public void GameOver(){
        isAlive = false; 
        _gameOverUI.SetActive(true); 
        var currentScore = _gameOverUI.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        var recordScore = _gameOverUI.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        currentScore.text = "Score: " + _tail.Count.ToString();
        recordScore.text = "Best score: " + PlayerPrefs.GetInt("highscore", 0).ToString();
        
        if(SceneManager.GetActiveScene().buildIndex == 5)
        {
            recordScore.text = "Best score: " + PlayerPrefs.GetInt("challenge_highscore", 0).ToString();
        }
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        _pauseUI.SetActive(true);
    }

    public void Resume(){
        Time.timeScale = 1f;
        _pauseUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Y)){
            PlayerPrefs.DeleteAll();
        }

        if(Input.GetKey(KeyCode.DownArrow)){
            _dir = Vector2.down;
        } else if(Input.GetKey(KeyCode.UpArrow)){
            _dir = Vector2.up; 
        } else if(Input.GetKey(KeyCode.RightArrow)){
            _dir = Vector2.right;
        } else if(Input.GetKey(KeyCode.LeftArrow)){
            _dir = Vector2.left;
        }

        passedTime += Time.deltaTime;
        if(timeBetweenMovements < passedTime && isAlive){
            passedTime = 0;
            // Move
            Vector3 newPosition = _head.GetComponent<Transform>().position + new Vector3(_dir.x, _dir.y, 0);

            // Check if collides with border
            if(newPosition.x >= _xSize/2
            || newPosition.x <= -_xSize/2
            || newPosition.y >= _ySize/2
            || newPosition.y <= -_ySize/2){
                GameOver();
            }

            // check if collides with any _tail tile
            foreach (var item in _tail)
            {
                if(item.transform.position == newPosition){
                    GameOver();
                }
            }
            if(newPosition.x == food.transform.position.x && newPosition.y == food.transform.position.y){
                GameObject newTile = Instantiate(_block);
                newTile.SetActive(true);
                newTile.transform.position = food.transform.position;
                DestroyImmediate(food);
                _head.GetComponent<SpriteRenderer>().color = Color.yellow;
                _tail.Add(_head); 
                _head = newTile;
                _head.GetComponent<SpriteRenderer>().color = Color.green;
                spawnFood();
                _points.text = _tail.Count.ToString();

                //Highscore check
                if(SceneManager.GetActiveScene().buildIndex == 4)
                {
                    if(_tail.Count > PlayerPrefs.GetInt("highscore", 0))
                    {
                        PlayerPrefs.SetInt("highscore", _tail.Count);
                        
                    }
                }

                if(SceneManager.GetActiveScene().buildIndex == 5)
                    if(_tail.Count > PlayerPrefs.GetInt("challenge_highscore", 0)){
                        PlayerPrefs.SetInt("challenge_highscore", _tail.Count);
                    }
            } else {
                if(_tail.Count == 0){
                    _head.transform.position = newPosition;
                } else {
                    _head.GetComponent<SpriteRenderer>().color = Color.yellow;
                    _tail.Add(_head); 
                    _head = _tail[0];
                    _head.GetComponent<SpriteRenderer>().color = Color.green;
                    _tail.RemoveAt(0);
                    _head.transform.position = newPosition;
                }
            }

        }
        
    }
}
