using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagicLeapTools;

public class BombExplode : MonoBehaviour {
    public GameObject explosionEffect;
    public Vector3 end; // ball.transform.position not accurate, used for end of path collision
    private float time = 0f;
    private bool collided = false;

    // Update is called once per frame
    void Update() {
        time += Time.deltaTime;
    }

    private void OnCollisionEnter(Collision coll) {
        // grabbed by controller
        if (coll.collider.tag == "GameController" && gameObject.activeSelf) {
            gameObject.SetActive(false);
            collided = true;
            Explode(coll.contacts[0].point);
            Destroy(gameObject);
        }
        // collide on furniture/wall
        if (!OnPlaySpaceEdge() && gameObject.activeSelf) {
            gameObject.SetActive(false);
            collided = true;
            Destroy(gameObject);
        } 
    }

    private bool OnPlaySpaceEdge() {
        if (time < 2f) return true;
        return false;
    }

    public void Explode(Vector3 location) {
        Instantiate(GameObject.Find("AudioManager").GetComponent<AudioManager>().grab,
                location,Quaternion.identity);
        GameObject explosion = Instantiate(explosionEffect, transform.position , transform.rotation);
        explosion.GetComponent<ParticleSystem>().Play();
        
    }
    
}