using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagicLeapTools;

public class Explosion : MonoBehaviour {
    public GameObject grabExplosionEffect;
    public GameObject otherExplosionEffect;
    public Vector3 end; // ball.transform.position not accurate, used for end of path collision
    private float time = 0f;
    private bool collided = false;

    // Update is called once per frame
    void Update() {
        time += Time.deltaTime;
    }

    void OnDestroy() {
        // disappear at the end of trail if did not hit mesh
        // Destroy() would disable the gameObject sutomatically
        // so we nned the collided value to record if its previous collision
        if (!collided) OtherExplode(end);
    }

    private void OnCollisionEnter(Collision coll) {
        // grabbed by controller
        if (coll.collider.tag == "GameController" && gameObject.activeSelf) {
            gameObject.SetActive(false);
            collided = true;
            GrabExplode(coll.contacts[0].point);
            Destroy(gameObject);
        }
        // collide on furniture/wall
        if (!OnPlaySpaceEdge() && gameObject.activeSelf) {
            gameObject.SetActive(false);
            collided = true;
            OtherExplode(coll.contacts[0].point);
            Destroy(gameObject);
        } 
    }

    private bool OnPlaySpaceEdge() {
        if (time < 2f) return true;
        return false;
    }

    public void GrabExplode(Vector3 location) {
        Instantiate(GameObject.Find("AudioManager").GetComponent<AudioManager>().grab,
                location,Quaternion.identity);
        GameObject explosion = Instantiate(grabExplosionEffect, location, Quaternion.identity);
        explosion.GetComponent<ParticleSystem>().Play();
        
    }

    public void OtherExplode(Vector3 location) {
        Instantiate(GameObject.Find("AudioManager").GetComponent<AudioManager>().crash,
                location,Quaternion.identity);        
        GameObject explosion = Instantiate(otherExplosionEffect, location, Quaternion.identity);
        explosion.GetComponent<ParticleSystem>().Play();
    }
    
}