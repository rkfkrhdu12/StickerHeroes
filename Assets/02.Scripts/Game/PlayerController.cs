﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variable
    private float _spawnRange = 7.0f;

    [SerializeField]
    private SpawnManager _spawnManager;

    KeyCode[] _spawnKeyCode = new KeyCode[5];

    eMouseState _curMouseState;
    private enum eMouseState
    {
        None,
        Camera,
        Spawn,
        Spawning,
    }
    #endregion

    #region Monobehaviour Function

    private void Awake()
    {
        _curMouseState = eMouseState.Spawn;

        int i = 0;
        _spawnKeyCode[i++] = KeyCode.Alpha1;
        _spawnKeyCode[i++] = KeyCode.Alpha2;
        _spawnKeyCode[i++] = KeyCode.Alpha3;
        _spawnKeyCode[i++] = KeyCode.Alpha4;
        _spawnKeyCode[i++] = KeyCode.Alpha5;

    }

    void Update()
    {
        if (Input.GetMouseButton(0) && _curMouseState != eMouseState.Spawning)  { _curMouseState = eMouseState.Camera; }
        else if (_curMouseState == eMouseState.Camera) { _curMouseState = eMouseState.Spawn; }

        UpdateMouseState();
    }

    #endregion

    #region Private Function

    void UpdateMouseState()
    {
        switch (_curMouseState)
        {
            case eMouseState.None:                              break;
            case eMouseState.Camera:                            break;
            case eMouseState.Spawn:    UpdateSpawnInput();      break;
            case eMouseState.Spawning: UpdateSpawnPoint();      break;
        }
    }

    void Spawn(int index)
    {
        _curMouseState = eMouseState.Spawning;

        _spawnManager.SetSpawnIndex(index);
    }

    void UpdateSpawnInput()
    {
        for (int i = 0; i < _spawnKeyCode.Length; ++i) 
        {
            if (Input.GetKeyDown(_spawnKeyCode[i]))
                Spawn(i);
        }
    }


    public int Range = 200;
    void UpdateSpawnPoint()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, Range))
            {
                Vector3 dist = hitInfo.point - transform.position;

                if (dist.sqrMagnitude < _spawnRange * _spawnRange && dist.sqrMagnitude > _spawnRange + _spawnRange)
                { // Spawn
                    _spawnManager.SetSpawnPoint(hitInfo.point);

                    _spawnManager.Spawn();

                    _curMouseState = eMouseState.Spawn;
                }
            }
        }
        else if(Input.GetMouseButtonDown(0) && _curMouseState != eMouseState.Spawning)
        {
            _curMouseState = eMouseState.Camera;
        }
    }
    #endregion
}
