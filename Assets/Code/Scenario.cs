﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class Scenario
{
    System.Random random = new System.Random(DateTime.Now.Millisecond);

    public GameObject leaderPrefab = SteeringManager.Instance().leaderPrefab;
    public GameObject boidPrefab = SteeringManager.Instance().boidPrefab;
    
    public abstract string Description();
    public abstract void SetUp();

    protected GameObject leader;

    public virtual void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject camera = (GameObject) GameObject.FindGameObjectWithTag("MainCamera");
            Vector3 newTargetPos = camera.transform.position + camera.transform.forward * 200.0f;
            leader.GetComponent<SteeringBehaviours>().seekTargetPos = newTargetPos;
        }

        if (Input.GetMouseButtonDown(1))
        {
            GameObject camera = (GameObject)GameObject.FindGameObjectWithTag("MainCamera");
            Vector3 newTargetPos = camera.transform.position;
            leader.GetComponent<SteeringBehaviours>().seekTargetPos = newTargetPos;
        }        
    }

    public void DestroyObjectsWithTag(string tag)
    {
        GameObject[] o = GameObject.FindGameObjectsWithTag(tag);
        Debug.Log(o.Length + "objects found");
        for (int i = 0; i < o.Length; i ++)
        {
            GameObject.Destroy(o[i]);
        }
    }

    public GameObject CreateObstacle(Vector3 position, float radius)
    {
        GameObject o;

        o = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        o.tag = "obstacle";
        o.renderer.material.color = new Color((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
        o.transform.localScale = new Vector3(radius * 2, radius * 2, radius * 2);
        o.transform.position = position;
        return o;
    }

    public GameObject CreateBoid(Vector3 position, GameObject prefab)
    {
        GameObject boid;

        boid = (GameObject)SteeringManager.Instantiate(prefab);
        boid.tag = "boid";
        boid.AddComponent<SteeringBehaviours>();
        boid.transform.position = position;

        return boid;
    }

    public GameObject CreateCamFollower(GameObject leader, Vector3 offset)
    {
        GameObject camFollower = new GameObject();
        camFollower.tag = "camFollower";
        camFollower.AddComponent<SteeringBehaviours>();
        camFollower.GetComponent<SteeringBehaviours>().leader = leader;
        camFollower.GetComponent<SteeringBehaviours>().offset = offset;
        camFollower.transform.position = leader.transform.TransformPoint(offset);
        camFollower.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.offset_pursuit);
        //camFighter.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.wall_avoidance);
        camFollower.GetComponent<SteeringBehaviours>().turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
        SteeringManager.Instance().camFighter = camFollower;
        GameObject.FindGameObjectWithTag("MainCamera").transform.position = camFollower.transform.position;

        return camFollower;
    }

    public virtual void TearDown()
    {
        DestroyObjectsWithTag("boid");
        DestroyObjectsWithTag("obstacle");
        DestroyObjectsWithTag("camFollower");
    }
}