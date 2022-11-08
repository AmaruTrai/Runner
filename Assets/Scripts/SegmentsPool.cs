using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Game
{
    public class SegmentsPool : MonoBehaviour
    {
        /// <summary>
        /// ����������, ����� ��������� ����� ��������� �� �������.
        /// </summary>
        public event Action<CorridorSegment> OnCreat;

        [Serializable]
        private struct SegmentContainer
        {
            [Range(0, 100)]
            [Tooltip("���� ��������� ��������.")]
            public int spawn�hance;
            public CorridorSegment segment;
        }

        [SerializeField]
        [Tooltip("������ ��������� ���������.")]
        private List<SegmentContainer> segmentPrefabs;
        [SerializeField]
        [Tooltip("���� � ������ ������������ ��������, �� ���������� ��������� ��������� ��������, � �������� �������.")]
        private List<int> defaultSpawnList;

        private Dictionary<CorridorSegment, ObjectPool<CorridorSegment>> pools;
        private IEnumerator<int> defaultSpawn;
        private int max;

        private void Awake()
        {
            pools = new Dictionary<CorridorSegment, ObjectPool<CorridorSegment>>();
            foreach (var prefab in segmentPrefabs)
            {
                var pool = new ObjectPool<CorridorSegment>(
                    createFunc: () => { return Create(prefab.segment); },
                    actionOnRelease: Realease
                );
                pools.Add(prefab.segment, pool);
                max += prefab.spawn�hance;
            }

            if (defaultSpawnList != null && defaultSpawnList.Count > 0)
            {
                defaultSpawn = defaultSpawnList.GetEnumerator();
            }
        }

        private CorridorSegment Create(CorridorSegment prefab)
        {
            var segment = Instantiate<CorridorSegment>(prefab);
            segment.OnRelease += () => { pools[prefab].Release(segment); };
            OnCreat?.Invoke(segment);
            return segment;
        }

        private void Realease(CorridorSegment segment)
        {
            segment.gameObject.SetActive(false);
        }

        public bool TryGetSegment(out CorridorSegment tool)
        {
            tool = null;
            if (pools == null || pools.Count == 0)
            {
                return false;
            }

            if (defaultSpawn != null && defaultSpawn.MoveNext())
            {
                tool = pools[segmentPrefabs[defaultSpawn.Current].segment].Get();
                return true;
            }

            int chance = UnityEngine.Random.Range(0, max);
            int chanceSum = 0;
            foreach (var prefab in segmentPrefabs)
            {
                chanceSum += prefab.spawn�hance;
                if (chance <= chanceSum)
                {
                    tool = pools[prefab.segment].Get();
                    return true;
                }
            }

            return false;
        }

        public void Restart()
        {
            if (defaultSpawnList != null && defaultSpawnList.Count > 0)
            {
                defaultSpawn = defaultSpawnList.GetEnumerator();
            }
        }
    }

}
