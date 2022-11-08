using System;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(CharacterController))]
    public class CorridorSegment : MonoBehaviour
    {
        /// <summary>
        /// Вызывается в момент входа игрока внутрь сегмента.
        /// </summary>
        public event Action<CorridorSegment> OnPlayerEnter;
        /// <summary>
        /// Вызывается в момент выхода игрока из сегмента.
        /// </summary>
        public event Action<CorridorSegment> OnPlayerExit;
        /// <summary>
        /// Вызывается в момент когда сегмент возвращается в пул.
        /// </summary>
        public event Action OnRelease;

        public Vector3 Size => box.size;

        public float Speed
        {
            get => speed;
            set => speed = value;
        }

        [Header("Logic")]
        [SerializeField]
        [Tooltip("Слой на котором находится игрок.")]
        private int playerLayer;

        [Header("Components")]
        [SerializeField]
        private BoxCollider box;
        [SerializeField]
        private CharacterController controller;

        private float speed;

        private void FixedUpdate()
        {
            if (speed != 0f)
            {
                var move = new Vector3(0, 0, Time.deltaTime * speed);
                controller.Move(transform.TransformDirection(move));
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == playerLayer)
            {
                OnPlayerEnter?.Invoke(this);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == playerLayer)
            {
                OnPlayerExit?.Invoke(this);
            }
        }

        public void Release()
        {
            OnRelease?.Invoke();
        }
    }
}