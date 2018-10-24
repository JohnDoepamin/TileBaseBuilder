﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{

    public GameObject circleCursorPrefab;
    public GameObject circleCursorParent;
    Vector3 currFramePosition;
    Vector3 lastFramePosition;
    Vector3 startDragPosition;
    Tile tileUnderMouse;

    List<GameObject> dragPreviewGameObjects;

    // Use this for initialization
    void Start()
    {
        dragPreviewGameObjects = new List<GameObject>();
        // Preload 100 circleCursors when loading the scene
        SimplePool.Preload(circleCursorPrefab, 100, circleCursorParent.transform);
    }

    // Update is called once per frame
    void Update()
    {
        currFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        tileUnderMouse = WorldController.Instance.GetTileAtCoordinates(currFramePosition);

        UpdateCameraMovement();
        // UpdateCursor();
        UpdateDrag();
        UpdateCameraZooming();

        lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void UpdateCameraMovement()
    {
        // Handle screen dragging (camera movement)
        if (Input.GetMouseButton(2))
        {
            var cameraMovement = lastFramePosition - currFramePosition;
            Camera.main.transform.Translate(cameraMovement);
        }
    }

    void UpdateCameraZooming()
    {
        // Camera.main.orthographicSize -= Input.mouseScrollDelta.y;
        Camera.main.orthographicSize -= Input.GetAxis("Mouse ScrollWheel");
    }

    void UpdateDrag()
    {
        // Start Handle actions (left mouse clicks)
        if (Input.GetMouseButtonDown(0))
        {
            startDragPosition = currFramePosition;
        }

        while (dragPreviewGameObjects.Count > 0)
        {
            GameObject go = dragPreviewGameObjects[0];
            dragPreviewGameObjects.RemoveAt(0);
            SimplePool.Despawn(go);
        }

        if (Input.GetMouseButton(0))
        {
            // Display a preview of the drag area
            var tilesToEdit = WorldController.Instance.GetTilesAtCoordinates(startDragPosition, currFramePosition);
            if (tilesToEdit != null)
            {
                foreach (var tile in tilesToEdit)
                {
                    GameObject go = SimplePool.Spawn(circleCursorPrefab, new Vector3(tile.X, tile.Y, 0), Quaternion.identity);
                    dragPreviewGameObjects.Add(go);
                }
            }
        }

        // End Handle actions (left mouse clicks)
        if (Input.GetMouseButtonUp(0))
        {
            var tilesToEdit = WorldController.Instance.GetTilesAtCoordinates(startDragPosition, currFramePosition);

            foreach (var tile in tilesToEdit)
            {
                if (tile.Type == Tile.TileType.Gras)
                    tile.Type = Tile.TileType.Floor;
                else
                    tile.Type = Tile.TileType.Gras;
            }
        }
    }

    // public void UpdateCursor()
    // {
    //     // Update the cursor graphic which indicates the selected Tile
    //     if (tileUnderMouse != null)
    //     {
    //         circleCursor.SetActive(true);
    //         circleCursor.transform.position = new Vector2(tileUnderMouse.X, tileUnderMouse.Y);
    //     }
    //     else
    //         circleCursor.SetActive(false);
    // }
}