using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Gun : MonoBehaviour
{
    //camera
    public Camera Cam;


    //Animators
    [SerializeField]
    public Animator shootAnim;
    public Animator ADS_Anim;

    bool isFiring_;

    //***SHOOTING****\\
    //damage and range
    [Header("Damage and Range")]
    public float damage = 10f;
    public float range = 100f;
    public float baseDamage = 10f;

    [Header("Bullet")] [Space]
    public GameObject impact; //bullet hit effect
    public float force = 30f; // add force to rigidBodies
    public float firerate = 0.1f;
    public float spray = 1f;
    public float sprayz_ = 10f;

    [Header("Fire Rate and Ammo")]
    public float maxAmmo = 50f;
    public float ammo;
    private float nextFire;
    public Text ammoNum_;
    public GameObject cog;

    [Header("Reticle Control")]
    public Image redDot;
    public GameObject[] reticle;

    //Discarded bullets
    [Header("Discarded Bullets")]
    public GameObject bulletSpawn;
    public GameObject bullet;
    private GameObject spawnedBullet;
    private Rigidbody spawnedBullet_RB;
    Vector3 bulletPos;

    //Muzzle Flash
    [Header("Muzzle Flash")]
    public GameObject muzzle;
    Vector3 muz_pos;
    private GameObject muzzleFlash_FX;
    public GameObject flash;

    //SFX
    [Header("SFX")]
    public AudioSource sfx;
    public AudioClip sfxClip;

    bool isShooting_ = false;
    bool canShoot = true;

    Vector3 gunOrg; // 0.306 -0.356 0.492 rot = 9.94

    Vector3 ADSPos = new Vector3(0,-0.32f,0.09f);
    Quaternion gunRot;

    Vector3 cogPos;

    // Start is called before the first frame update
    void Awake()
    {
        cogPos = cog.transform.localPosition;
        ammo = maxAmmo;
        sfx = GetComponent<AudioSource>();
        gunOrg = this.gameObject.transform.localPosition;
        gunRot = this.gameObject.transform.localRotation;
        ADS_Anim.speed = 3f;
        Cursor.lockState = CursorLockMode.Locked;
        redDot.gameObject.SetActive(false);
        
    }

    public void PlayAudio(string name, float vol, bool loop, bool shoot)
    {
        isShooting_ = shoot;
        sfxClip = Resources.Load<AudioClip>("Audio/"+ name);
        sfx.clip = sfxClip;
        sfx.loop = loop;
        sfx.volume = vol;
        sfx.Play();
    }

    bool aiming = false;



    // Update is called once per frame
    void Update()
    {
        
        if(Input.GetButtonDown("Fire2"))
        {
            if(!aiming)
            {
                ADS_Anim.SetBool("adsStart", true);
                ADS_Anim.SetBool("adsEnd", false);

                //bullet spread over time 1


                foreach(GameObject go in reticle)
                {
                    go.SetActive(false);
                }

                redDot.gameObject.SetActive(true);
                aiming = true;
            }



        } else if(Input.GetButtonUp("Fire2"))
        {
            ADS_Anim.SetBool("adsEnd", true);
            ADS_Anim.SetBool("adsStart", false);
            aiming = false;
            redDot.gameObject.SetActive(false);
            foreach (GameObject go in reticle)
            {
                go.SetActive(true);
            }
        }
        ammoNum_.text = ammo.ToString();

        if (canShoot)
        {
            if (ammo > 0)
            {
                canShoot = true;
            }
            else
            {
                
                shootAnim.SetBool("Shooting", false);
                canShoot = false;
                return;
            }

            if (Input.GetButton("Fire1") && Time.time >= nextFire && ammo > 0)
            {

                ShootGun();



                //spawns bullet coming out of the side of the gun, making it look like it is shooting 
                float xVol = Random.Range(-.5f, -3f);
                float yVol = Random.Range(.5f, 1.2f);
                float zVol = Random.Range(-.5f, .5f);
                bulletPos = bulletSpawn.transform.position;
                spawnedBullet = Instantiate(bullet, bulletPos, Quaternion.identity);
                spawnedBullet_RB = spawnedBullet.GetComponent<Rigidbody>();
                spawnedBullet_RB.velocity = new Vector3(xVol, yVol, 0f);

                Destroy(spawnedBullet, 2f);
            }
            else
            {
                if(Input.GetButtonUp("Fire1"))
                {
                    sfx.Stop();
                    sfx.loop = false;
                    spray = .1f;
                }
                //Destroy(muzzleFlash_FX);
                if (isShooting_ && Input.GetButtonUp("Fire1") || isShooting_ && ammo <= 0)
                {

                    sfx.Stop();
                    sfx.loop = false;
                    isShooting_ = false;
                    shootAnim.SetBool("Shooting", false);
                    
                }

                
            }
        }
        if (Input.GetButton("Fire1") && ammo <= 0)
        {
            sfx.Stop();
            sfx.loop = false;
            shootAnim.SetBool("Empty", true);
            PlayAudio("click", 1f, false, false);
            shootAnim.SetBool("Shooting", false);
        }

        shootAnim.SetBool("Shooting", false);

        //Debug.Log(" No More Shooting");


        if (Input.GetButtonDown("Reload") && ammo != maxAmmo)
        {
            
            Reload();
        }
        
    }

    public void ADS(bool enabled)
    {
        
        if (enabled)
        {
            Debug.Log("ADS");
            ;
        } else if (!enabled)
        {
           // ADS_.SetBool("ADS_Bool", false);
        }
       
    }

    IEnumerator Delay(float sec, string type)
    {
        yield return new WaitForSeconds(sec);
        if(type == "stop")
        {
            sfx.Stop();
        }
        if(type == "Reload")
        {

            shootAnim.SetBool("Reload", false);
            ADS_Anim.SetBool("Reload", false);
            ADS_Anim.SetBool("adsEnd", false);
            ammo = maxAmmo;
            foreach (GameObject go in reticle)
            {
                go.SetActive(true);
            }
            reloading_ = false;
            canShoot = true;
        }
        if(type == "ADS")
        {
            ADS_Anim.SetBool("adsStart", false);
            ADS_Anim.SetBool("adsEnd", false);
        }


    }

    bool reloading_ = false;
    bool adsReload = false;

    void Reload()
    {
        StartCoroutine( Delay(1.5f, "stop"));
        if(!reloading_)
        {
            foreach (GameObject go in reticle)
            {
                go.SetActive(false);
            }
            if(!adsReload)
            {
                ADS_Anim.SetBool("adsEnd", false);
                adsReload = true;
            }

            shootAnim.SetBool("Empty", false);
            shootAnim.SetBool("Reload", true);
            ADS_Anim.SetBool("Reload", true);
            
            reloading_ = true;
        }

        PlayAudio("Reload", 1, false, false);
        spray = .1f;
        StartCoroutine(Delay(2, "Reload"));
    }



    void ShootGun()
    {
        nextFire = Time.time + firerate;

        shootAnim.SetBool("Shooting", true);
        if(!aiming)
        {
            spray = spray + 0.1f;
            if(spray >= 0.8f)
            {
                spray = .8f;
            }
        }
        else
        {
            spray = 0.1f;
        }


        //muz_pos = muzzle.transform.localPosition;
        //muzzleFlash_FX = Instantiate(flash, muz_pos, Quaternion.identity);

        if (!isShooting_)
        {
            PlayAudio("one_shot", 1f, true, true);
            isShooting_ = true;
        }

        ammo--;


        //Raycast to check what the gun is pointing and shooting at


        //  Generate a random XY point inside a circle:
        Vector3 direction = Random.insideUnitCircle * spray;
        direction.z = sprayz_; // circle is at Z units 
        direction = Cam.transform.TransformDirection(direction.normalized);
        //Raycast and debug
        Ray r = new Ray(Cam.transform.position, direction);

        RaycastHit hit;
        if (Physics.Raycast(r, out hit, range))
        {
            Debug.DrawLine(Cam.transform.position, hit.point,Color.red);
            string targetTag = hit.transform.tag.ToString();

            Debug.Log(targetTag + Time.time);

           
            Target parent_target = hit.transform.GetComponentInParent<Target>();

            if (parent_target!=null)
            {
                if(targetTag == "Limb")
                {
                    damage = baseDamage * 0.8f;
                    
                } else if(targetTag == "Body")
                {
                    damage = baseDamage;
                } else if (targetTag == "Head")
                {
                    damage = baseDamage * 1.5f;
                }


                Debug.Log(damage);
                parent_target.Damage(damage);
            }
            if(hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(hit.normal * force);
            }
            if(targetTag != "Body" && targetTag != "Head" && targetTag != "Limb")
            {
                impact = Resources.Load<GameObject>("Particles/ImpactMisc");
            } else
            {
                impact = Resources.Load<GameObject>("Particles/ImpactPlayer");
            }
            GameObject impactobject = Instantiate(impact, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactobject, 2);

        }
    }
    
}
