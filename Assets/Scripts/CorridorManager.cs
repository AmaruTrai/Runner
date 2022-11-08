using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(SegmentsPool))]
    public class CorridorManager : MonoBehaviour
    {
        [Header("Logic")]
        [SerializeField]
        [Tooltip("Количество заранее создаваемых сегментов.")]
        private int lenght = 3;
        [SerializeField]
        [Tooltip("Если сегментов больше, чем lenght  + additionalLenght, самый старый сегмент будет убран.")]
        private int additionalLenght = 2;
        [SerializeField]
        [Tooltip("Скорость движения коридора.")]
        private float speed;
        [SerializeField]
        [Tooltip("Место откуда начинается создание коридора.")]
        private Transform defaultSpawnPoint;

        [Header("Components")]
        [SerializeField]
        private SegmentsPool pool;

        private Queue<CorridorSegment> segments;
        private CorridorSegment lastSegment;
        private float currentSpeed;

        private void Awake()
        {
            currentSpeed = speed;
            segments = new Queue<CorridorSegment>();
            pool.OnCreat += OnSegmentCreated;
        }

        private void OnSegmentCreated(CorridorSegment segment)
        {
            segment.OnPlayerEnter += SetNextSegment;
            segment.OnPlayerExit += OnPlayerExitSegment;
        }

        private void OnPlayerExitSegment(CorridorSegment segment)
        {
            if (segments.Count > additionalLenght + lenght)
            {
                segments.Dequeue().Release();
            }
        }

        private void SetNextSegment(CorridorSegment segment)
        {
            if (pool.TryGetSegment(out var newSegment))
            {
                newSegment.transform.position = lastSegment.transform.position + new Vector3(0, 0, lastSegment.Size.x);
                newSegment.Speed = currentSpeed;
                newSegment.gameObject.SetActive(true);
                segments.Enqueue(newSegment);
                lastSegment = newSegment;
            }
        }

        public void SetSpeed(float speed)
        {
            this.speed = speed;
            currentSpeed = speed;
        }

        public void OnGameStart()
        {
            if (pool.TryGetSegment(out var segment))
            {
                segment.transform.position = defaultSpawnPoint.position;
                segment.Speed = currentSpeed;
                segment.gameObject.SetActive(true);
                segments.Enqueue(segment);
                lastSegment = segment;
            }

            for (int i = 1; i < lenght; i++)
            {
                SetNextSegment(null);
            }
        }

        public void OnGameEnd()
        {
            foreach(var segment in segments)
            {
                segment.Release();
            }
            segments.Clear();
            pool.Restart();
        }
    }
}