using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagicLeapTools;

public class Explosion : MonoBehaviour {

    public float cubeSize = 0.2f;
    public int cubesInRow = 5;

    float cubesPivotDistance;
    Vector3 cubesPivot;

    public float explosionForce = 50f;
    public float explosionRadius = 4f;
    public float explosionUpward = 0.4f;
    public GameObject grabExplosionEffect;
    public GameObject otherExplosionEffect;
    public Vector3 end;
    private float time = 0f;
    private bool collided = false;

    // Use this for initialization
    void Start() {
        //calculate pivot distance
        cubesPivotDistance = cubeSize * cubesInRow / 2;
        //use this value to create pivot vector)
        cubesPivot = new Vector3(cubesPivotDistance, cubesPivotDistance, cubesPivotDistance);

    }

    // Update is called once per frame
    void Update() {
        time += Time.deltaTime;
    }

    void OnDestroy() {
        // disappear at the end of trail if did not hit mesh
        // Destroy() would disable the gameObject directly
        // so we nned the collided value to record if its previous collision
        if (!collided) OtherExplode(end);
    }

    private void OnCollisionEnter(Collision coll) {
        // grabbed by controller
        if (coll.collider.tag == "Collision" && gameObject.activeSelf) {
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
    public void explode2() {
        //make object disappear
        gameObject.SetActive(false);

        //loop 3 times to create 5x5x5 pieces in x,y,z coordinates
        for (int x = 0; x < cubesInRow; x++) {
            for (int y = 0; y < cubesInRow; y++) {
                for (int z = 0; z < cubesInRow; z++) {
                    createPiece(x, y, z);
                }
            }
        }

        //get explosion position
        Vector3 explosionPos = transform.position;
        //get colliders in that position and radius
        Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
        //add explosion force to all colliders in that overlap sphere
        foreach (Collider hit in colliders) {
            //get rigidbody from collider object
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null) {
                //add explosion force to this body with given parameters
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, explosionUpward);
            }
        }

    }

    void createPiece(int x, int y, int z) {

        //create piece
        GameObject piece;
        piece = GameObject.CreatePrimitive(PrimitiveType.Cube);

        //set piece position and scale
        piece.transform.position = transform.position + new Vector3(cubeSize * x, cubeSize * y, cubeSize * z) - cubesPivot;
        piece.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);

        //add rigidbody and set mass
        piece.AddComponent<Rigidbody>();
        piece.GetComponent<Rigidbody>().mass = cubeSize;
        Destroy(piece, 1.5f);
    }

}