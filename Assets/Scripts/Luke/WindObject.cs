using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindObject : MonoBehaviour
{
    [SerializeField] private GameObject windGenerator;

    private CharacterController controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Since player does not use a dynamic rigidbody, you need to use vector maths to calculate which direction to MovePosition to
    }
}
