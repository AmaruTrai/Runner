using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        [Header("Logic")]
        [SerializeField]
        private PlayerController playerPrefab;
        [SerializeField]
        private Transform spawnPoint;
        [SerializeField]
        private float speedMultiplyTimeOut;
        [SerializeField]
        private float speedMultiplier;

        [Header("UI")]
        [SerializeField]
        private Text timeText;
        [SerializeField]
        private Text timeResultText;
        [SerializeField]
        private Text countText;

        [Header("Events")]
        public UnityEvent OnStart;
        public UnityEvent OnPause;
        public UnityEvent OnContinue;
        public UnityEvent OnEnd;
        public UnityEvent OnRestart;

        private PlayerController player;
        private float playTime;
        private float savedTime;
        private float multiplyTime;
        private bool isGameRun;
        private string currentDifficult;

        private void Awake()
        {
            player = Instantiate<PlayerController>(playerPrefab);
            player.gameObject.SetActive(false);
            player.OnBlocked += GameEnd;

            playTime = 0;
            savedTime = 0;
            multiplyTime = speedMultiplyTimeOut;
            isGameRun = false;
            currentDifficult = "Easy";
        }

        private void Update()
        {
            if (!isGameRun)
            {
                return;
            }

            playTime += Time.deltaTime;

            if (playTime - savedTime >= 1)
            {
                UpdateTime();
            }

            if (playTime - multiplyTime >= 0)
            {
                multiplyTime = playTime + speedMultiplyTimeOut;
                float newSpeed = Math.Abs(player.Speed) * speedMultiplier;
                player.Speed = newSpeed;
            }
        }

        private void UpdateTime()
        {
            var format = TimeSpan.FromSeconds(playTime);
            timeText.text = $"Time: {format.ToString(@"mm\:ss")}";
            savedTime = playTime;
        }

        public void GameStart()
        {
            var count = PlayerPrefs.GetInt(currentDifficult, 0);
            count ++;
            PlayerPrefs.SetInt(currentDifficult, count);

            playTime = 0;
            savedTime = 0;
            Time.timeScale = 1;
            UpdateTime();
            player.transform.position = spawnPoint.position;
            player.gameObject.SetActive(true);
            isGameRun = true;
            OnStart?.Invoke();
        }

        public void GamePause()
        {
            isGameRun = false;
            Time.timeScale = 0;
            OnPause?.Invoke();
        }

        public void GameContinue()
        {
            isGameRun = true;
            Time.timeScale = 1;
            OnContinue?.Invoke();
        }

        public void GameEnd()
        {
            var format = TimeSpan.FromSeconds(playTime);
            timeResultText.text = format.ToString(@"mm\:ss");

            var count = PlayerPrefs.GetInt(currentDifficult, 0);
            countText.text = count.ToString();

            playTime = 0;
            savedTime = 0;
            multiplyTime = speedMultiplyTimeOut;
            isGameRun = false;
            Time.timeScale = 0;
            player.ResetSpeed();
            player.gameObject.SetActive(false);
            OnEnd?.Invoke();
        }

        public void Restart()
        {
            isGameRun = false;
            OnRestart?.Invoke();
        }

        public void SetDifficult(string difficult)
        {
            currentDifficult = difficult;
        }
    }

}