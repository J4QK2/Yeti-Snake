using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] private Snake _snake; 
    [SerializeField] private float _timeRemaining;
    [SerializeField] private Slider _timerSlider;

    private void Start()
    {
        _timerSlider.maxValue = _timeRemaining;
    }
    // Update is called once per frame
    void Update()
    {
        if(_timeRemaining > 0)
        {
            _timeRemaining -= Time.deltaTime;
        }else if (_timeRemaining < 0)
        {
            _timeRemaining = 0;
            _snake.GameOver();
        }
        _timerSlider.value = _timeRemaining;
    }
}
