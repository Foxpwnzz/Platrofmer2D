using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class Hero : MonoBehaviour
{
    [SerializeField] private float _speed;
    private Vector2 _direction;

    public void SetDirection(Vector2 direction)
    {
        _direction = direction;
    }

    private void Update()
    {
        if (_direction != Vector2.zero)
        {
            var delta = _direction * _speed * Time.deltaTime;
            var newPosition = new Vector3(transform.position.x + delta.x, transform.position.y + delta.y, transform.position.z);
            transform.position = newPosition;
        }
    }

    public void SaySomething()
    {
        Debug.Log("Something!!!");
    }
}