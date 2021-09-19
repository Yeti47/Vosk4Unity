using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Yetibyte.Unity.SpeechRecognition.KeywordDetection;

namespace Yetibyte.Unity.SpeechRecognition.Examples
{

    public class VoskExampleCommandProcessor : MonoBehaviour
    {
        #region Constants

        private const float SPAWN_HEIGHT = 5;
        private const float MAX_SPAWN_OFFSET = 6;
        private const int JUMP_FORCE = 8;

        #endregion

        #region Fields

        // Simple public field for demonstration purposes. Always make sure to properly encapsulate
        // in a real life application ;)
        public GameObject _cubePrefab;

        public GameObject _floor;

        private readonly List<GameObject> _spawnedCubes = new List<GameObject>();

        #endregion

        #region Properties

        public GameObject LastCubeSpawned => _spawnedCubes.Any() ? _spawnedCubes.Last() : null;

        #endregion

        #region Voice Command Handling Methods

        public void ÓnVoiceCommand_SpawnCube(VoiceCommand voiceCommand, string detectedText)
        {
            if(_cubePrefab != null && _floor != null)
            {
                float offsetX = UnityEngine.Random.Range(-MAX_SPAWN_OFFSET, MAX_SPAWN_OFFSET);
                float offsetZ = UnityEngine.Random.Range(-MAX_SPAWN_OFFSET, MAX_SPAWN_OFFSET);
                float offsetY = SPAWN_HEIGHT;

                Vector3 spawnPosition = _floor.transform.position + new Vector3(offsetX, offsetY, offsetZ);

                GameObject spawnedCube = Object.Instantiate(_cubePrefab, spawnPosition, Quaternion.identity);
                _spawnedCubes.Add(spawnedCube);
            }
        }

        public void ÓnVoiceCommand_TintCube(VoiceCommand voiceCommand, string detectedText)
        {
            if(LastCubeSpawned != null)
            {
                Color tintColor = Color.white;

                // The recognizer might confuse "red" for "read" because they are phonetocally indentical
                if(detectedText.Contains("red") || detectedText.Contains("read"))
                {
                    tintColor = Color.red;
                }
                else if (detectedText.Contains("green"))
                {
                    tintColor = Color.green;
                }
                else if (detectedText.Contains("blue"))
                {
                    tintColor = Color.blue;
                }

                LastCubeSpawned.GetComponent<MeshRenderer>().material.color = tintColor;

            }
        }

        public void ÓnVoiceCommand_MakeCubeJump(VoiceCommand voiceCommand, string detectedText)
        {
            if(LastCubeSpawned != null)
            {
                LastCubeSpawned.GetComponent<Rigidbody>().AddForce(Vector3.up * JUMP_FORCE, ForceMode.Impulse);
            }
        }


        #endregion
    }

}


