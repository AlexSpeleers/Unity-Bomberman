using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public ParticleSystem mainParticle;
    public PlayerBrain ownerBrain;

    public GameManager gameManager;
    public StateManager stateManager;
    
    public FloatVariable bombTimerLegth;
    public float explosionTick;
    bool exploded = true;
    public AudioSource audioSource;
    public AudioClip beepingSound;
    public AudioClip explosionSound;

    private void OnEnable()
    {
        //todo particles
        gameManager = stateManager.gameManager;
        explosionTick = Time.time + bombTimerLegth.value;

        var particle = mainParticle.main;
        var particles = mainParticle.subEmitters;
        particle.startLifetime = bombTimerLegth.value;

        for (int i = 0; i < particles.subEmittersCount; i++)
        {
            var emitter = particles.GetSubEmitterSystem(i).main;
            emitter.startDelay = bombTimerLegth.value;
        }
        mainParticle.Play();
        exploded = false;
        if (ownerBrain)
        {
            ownerBrain.AddActiveBomb(this);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!exploded && Time.time > explosionTick)
        {
            exploded = true;
            HandleExplosion();
        }
    }
    
    void HandleExplosion()
    {
        print("exploding");

        audioSource.PlayOneShot(explosionSound);

        RaycastHit hit;
        if (Utils.RoundedVector3(gameManager.player1.playerGO.transform.position) == transform.position)
        {
            gameManager.OnPlayerKilled(gameManager.player1);
        }
        if (gameManager.player2.playerGO && Utils.RoundedVector3(gameManager.player2.playerGO.transform.position) == transform.position)
        {
            gameManager.OnPlayerKilled(gameManager.player2);
        }
        for (int i = 0; i < gameManager.enemyManager.spawnedEnemies.Count; i++)
        {
            if (Utils.RoundedVector3(gameManager.enemyManager.spawnedEnemies[i].transform.position)==transform.position)
            {
                Destroy(gameManager.enemyManager.spawnedEnemies[i].gameObject);
            }
        }

        for (int i = 0; i < 4; i++)
        {
            transform.rotation = Quaternion.Euler(0, 90 * i, 0);
            Debug.DrawRay(transform.position, transform.forward * 2f, Color.green, 60f);

            if (Physics.Raycast(transform.position, transform.forward, out hit, 2f))
            {
                print("I hit " + hit.transform.name);
                if (hit.transform.tag == "Player")
                {
                    gameManager.OnPlayerKilled(hit.transform.GetComponent<Player>().playerBrain);
                }
                else if (hit.transform.tag == "BreakableBlock")
                {
                    Destroy(hit.transform.gameObject);
                }
                else if (hit.transform.tag == "Enemy")
                {
                    Destroy(hit.transform.gameObject);
                }
            }
        }
        Destroy(gameObject, 0.6f);
    }

    private void OnDestroy()
    {
        if (ownerBrain)
        {
            ownerBrain.RemoveBomb(this);
        }
    }
}
