/*
    File name: WindObject.cs
    Author:    Luke Lazzaro
    Summary: An object that can be moved by a wind generator
    Creation Date: 29/07/2020
    Last Modified: 3/08/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindObject : MonoBehaviour
{
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Moves the object based on wind generator direction
    public void Move(Vector3 direction, float speed)
    {
        // Since player does not use a dynamic rigidbody, you need to use vector maths to calculate which direction to MovePosition to
        controller.Move(direction * speed * Time.deltaTime);
    }
}
