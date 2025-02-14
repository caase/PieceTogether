﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    private bool draggingItem = false;
    private GameObject draggedObject;
    private Vector2 touchOffset;

    private Vector2 firstPosition;
    public bool firstSwipeHasBeenDone = false;
    public bool movingEnabled = false;

    public ScrollRect scrollRect;

    private Nav nav;

    public bool interactable = true;

    private void Start()
    {
        nav = GameObject.Find("/UI/SafeArea/Nav/Content").GetComponent<Nav>();
    }

    void Update()
    {
        if (interactable)
        {
            if (HasInput)
            {
                DragOrPickUp();
            }
            else
            {
                if (draggingItem)
                {
                    DropItem();
                }
            }
        }
    }

    Vector2 CurrentTouchPosition
    {
        get
        {
            Vector2 inputPos;
            inputPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return inputPos;
        }
    }

    private void DragOrPickUp()
    {
        var inputPosition = CurrentTouchPosition;

        if (draggingItem)
        {
            if (firstSwipeHasBeenDone == false)
            {
                float xDif = Mathf.Abs(firstPosition.x - inputPosition.x);
                float yDif = Mathf.Abs(firstPosition.y - inputPosition.y);

                if (draggedObject.GetComponent<ShapeScript>().isInNav)
                {
                    if (xDif > 0.1f || yDif > 0.1f)
                    {
                        if (draggedObject.GetComponent<ShapeScript>().isInNav)
                        {
                            if (nav.objectsInNav.Count >= Mathf.FloorToInt(Camera.main.orthographicSize * 2 * Screen.width / Screen.height)) 
                            {
                                if (xDif < 3f * yDif)
                                {
                                    movingEnabled = true;
                                    draggedObject.GetComponent<ShapeScript>().DraggingItem();
                                    scrollRect.enabled = false;
                                }
                                else
                                {
                                    movingEnabled = false;
                                }
                            }
                            else
                            {
                                movingEnabled = true;
                                draggedObject.GetComponent<ShapeScript>().DraggingItem();
                                scrollRect.enabled = false;
                            }
                        }

                        firstSwipeHasBeenDone = true;
                    }

                    else 
                    {
                        movingEnabled = false;
                    }
                }

                else {
                    movingEnabled = true;
                    draggedObject.GetComponent<ShapeScript>().DraggingItem();
                    scrollRect.enabled = false;
                }
            }
           
            if (movingEnabled)
            {
                draggedObject.transform.position = inputPosition + touchOffset;
            }
        }

        else
        {
            RaycastHit2D[] touches = Physics2D.RaycastAll(inputPosition, inputPosition, 0.5f);
            if (touches.Length > 0)
            {
                var hit = touches[0];
                if (hit.transform != null)
                {
                    draggingItem = true;
                    draggedObject = hit.transform.gameObject;
                    touchOffset = (Vector2)hit.transform.position - inputPosition;
                    firstPosition = CurrentTouchPosition;
                }
            }
        }
    }

    private bool HasInput
    {
        get
        {
            // returns true if either the mouse button is down or at least one touch is felt on the screen
            return Input.GetMouseButton(0);
        }
    }

    void DropItem()
    {
        if ((Mathf.Abs(CurrentTouchPosition.x - firstPosition.x) < 0.1f) && (Mathf.Abs(CurrentTouchPosition.y - firstPosition.y) < 0.1f))
        {
            draggedObject.GetComponent<ShapeScript>().Rotate();
        }

        draggingItem = false;
        firstSwipeHasBeenDone = false;

        if (nav.objectsInNav.Count > Mathf.FloorToInt(Camera.main.orthographicSize * 2 * Screen.width / Screen.height))
        {
            scrollRect.enabled = true;
        }

        if (movingEnabled) {
            draggedObject.GetComponent<ShapeScript>().DropItem();
        }
    }
}