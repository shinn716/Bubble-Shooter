using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shinn
{
    interface IElastic
    {
        void OnElastic(Collision hit);
        void OnElastic(RaycastHit hit);
    }

    [RequireComponent(typeof(MeshRenderer))]
    public class ElasticObject : MonoBehaviour, IElastic
    {
        private static int s_pos, s_nor, s_time;

        public enum CollisionState
        {
            Collision,
            MouseRaycasting
        }
        public CollisionState myState;

        [Header("Collision State")]
        public string CollisionTag = "ball";
        [Range(0, 5)]
        public float magnitude = 1;

        static ElasticObject()
        {
            s_pos = Shader.PropertyToID("_Position");
            s_nor = Shader.PropertyToID("_Normal");
            s_time = Shader.PropertyToID("_PointTime");
        }
        private MeshRenderer mesh;


        private void Awake()
        {
            mesh = GetComponent<MeshRenderer>();
        }
        private void Update()
        {

            if (myState == CollisionState.MouseRaycasting)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hit, 100.0f))
                    {
                        Debug.DrawRay(ray.direction, hit.point, Color.red);
                        OnElastic(hit);
                    }
                }
            }
        }
        void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.tag == CollisionTag && myState == CollisionState.Collision)
                OnElastic(collision);
        }

        public void OnElastic(Collision hit)
        {
            //當力大於一定範圍
            if (hit.relativeVelocity.magnitude > magnitude)
            {

                //反作用力
                Rigidbody rigidbody;
                rigidbody = hit.gameObject.GetComponent<Rigidbody>();
                rigidbody.AddForce(-hit.relativeVelocity);


                //反弹的坐标
                Vector4 v = transform.InverseTransformPoint(hit.contacts[0].point);

                //受影响顶点范围的半径
                v.w = Random.Range(.1f, 1);

                if (s_pos != 0)
                    mesh.material.SetVector(s_pos, v);

                //法线方向，该值为顶点偏移方向，可自己根据需求传。
                v = transform.InverseTransformDirection(hit.contacts[0].point.normalized);
                //反弹力度
                v.w = Random.Range(.1f, 1);
                mesh.material.SetVector(s_nor, v);
                //重置时间
                mesh.material.SetFloat(s_time, Time.time);
            }
        }
        public void OnElastic(RaycastHit hit)
        {
            //反弹的坐标
            Vector4 v = transform.InverseTransformPoint(hit.point);

            //受影响顶点范围的半径
            v.w = Random.Range(.1f, 1);

            if (s_pos != 0)
                mesh.material.SetVector(s_pos, v);

            //法线方向，该值为顶点偏移方向，可自己根据需求传。
            v = transform.InverseTransformDirection(hit.point.normalized);
            //反弹力度
            v.w = Random.Range(.1f, 1);
            mesh.material.SetVector(s_nor, v);
            //重置时间
            mesh.material.SetFloat(s_time, Time.time);

        }
    }
}
