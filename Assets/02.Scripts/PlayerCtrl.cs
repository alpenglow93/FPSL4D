using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    private float h = 0.0f; //horizontal
    private float v = 0.0f; //vertical
    private float r = 0.0f; //rotate

    private Transform tr;
    private Rigidbody rb;

    bool isJumping = false;

    public float moveSpeed = 10.0f;
    public float rotSpeed = 80.0f;
    public float jumpHeight = 4.0f;

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Jump();
    }

    private void Move()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        r = Input.GetAxisRaw("Mouse X");

        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);


        tr.Translate(moveDir.normalized * moveSpeed * Time.deltaTime, Space.Self);
        tr.Rotate(Vector3.up * rotSpeed * Time.deltaTime * r);
    }

    private void Jump()
    {
        //�������϶� ����
        if (isJumping)
            return;

        if (Input.GetButtonDown("Jump"))
        {
            isJumping = true;
            
            rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //���� ����� ��� ���ǹ� �κ� �����ؾ��Ұ���
        if(collision.gameObject.CompareTag("Floor"))
        {
            isJumping = false;
        }
    }
}
