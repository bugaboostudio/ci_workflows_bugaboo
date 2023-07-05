using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion.XR.Tools
{
    public class BoardManager : MonoBehaviour
    {
        float recordEndTime = -1;
        float recordDuration = 1;

        private void Start()
        {
            recordEndTime = Time.time + recordDuration;
        }

        public void ActivateBoards()
        {
            recordEndTime = recordDuration + Time.time;
        }

        private void Update()
        {
            _shouldBoardsRecord = Time.time < recordEndTime;
        }

        bool _shouldBoardsRecord;
        public bool ShouldBoardsRecord => _shouldBoardsRecord;
    }
}
