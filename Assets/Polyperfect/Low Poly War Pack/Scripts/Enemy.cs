using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

namespace PolyPerfect
{
    namespace War
    {
        [SerializeField]
        public enum AIState
        {
            Idle,
            WalkingOnPath,
            Shooting,
            FindingPlayer,
            FindingAmmo,
            FindingWeapon,
            FindingMedkit,
            RunningToShootPosition
        }
        [RequireComponent(typeof(NavMeshAgent))]
        public class Enemy : MonoBehaviour, IPickable, IDamageable<float>
        {
            [SerializeField]
            private float _health;
            public float health
            {
                get
                {
                    return _health;
                }
                set
                {
                    _health = value;
                }
            }
            [SerializeField]
            private float _currentHealth;
            public float currentHealth
            {
                get
                {
                    return _currentHealth;
                }
                set
                {
                    _currentHealth = value;
                }
            }
            public Canvas canvas;
            public Slider healthSlider;
            public NavPath navPath;
            public Transform eyesPos;

            [HideInInspector]
            public GameObject hand;
            [HideInInspector]
            public List<GameObject> weapons = new List<GameObject>();
            //public List<WeaponController> weapons = new List<WeaponController>();
            [HideInInspector]
            public List<StartingAmmo> startingAmmo = new List<StartingAmmo>();
            public Dictionary<int, int> inventoryAmmos = new Dictionary<int, int>();
            public Dictionary<int, Weapon> inventoryWeapons = new Dictionary<int, Weapon>();
            [HideInInspector]
            public AIState state = AIState.Idle;

            [Range(0f, 5f)]
            public float speed;
            [Range(0f, 10f)]
            public float runningSpeed;
            [Range(0f, 200f)]
            public float viewRange;
            [Range(0f, 200f)]
            public float maxShootRange;
            [Range(0f, 20f)]
            public float timeToLoseAggro;
            [Range(0f, 5f)]
            public float difficulty;
            [Range(0f, 10f)]
            public float scatter;
            [HideInInspector]
            public int currentCheckpoint;

            private float distanceToPlayer = float.PositiveInfinity;
            private float distanceToAmmo = float.PositiveInfinity;
            private float distanceToWeapon = float.PositiveInfinity;
            private float distanceToMedkit = float.PositiveInfinity;
            private int currWeapon = 0;
            private float timeSinceLastShoot = 0;

            private Vector3 IK;
            private Transform leftHandTarget;
            private bool reloading = false;
            private PlayerController player;
            private Animator animator;
            private NavMeshAgent navAgent;
            // Start is called before the first frame update
            void Start()
            {

                animator = GetComponent<Animator>();
                player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerController>();
                distanceToPlayer = Vector3.Distance(player.transform.position,this.transform.position);
                SetUpAmmo();
                SetUpWeapons();
                if (inventoryAmmos[inventoryWeapons[currWeapon].ammoType] > 0)
                    inventoryWeapons[currWeapon].currentMagazine = inventoryWeapons[currWeapon].magazine;
                else
                    inventoryWeapons[currWeapon].currentMagazine = 0;
                animator.SetBool(inventoryWeapons[0].weaponName, true);
                leftHandTarget = inventoryWeapons[0].leftHandPosition;
                currentHealth = health;
                healthSlider.maxValue = health;
                healthSlider.value = health;
                navAgent = GetComponent<NavMeshAgent>();
                navAgent.speed = speed;
                currentCheckpoint = 0;
                if (navPath != null)
                {
                    navAgent.SetDestination(navPath.GetCurrentTarget(currentCheckpoint));
                    state = AIState.WalkingOnPath;
                }
            }
            void SetUpWeapons()
            {
                int i = 0;
                foreach (GameObject weapon in weapons)
                {
                    var weaponObj = Instantiate<GameObject>(weapon, hand.transform);
                    if (i != 0)
                    {
                        weaponObj.SetActive(false);
                    }
                    inventoryWeapons.Add(i, weaponObj.GetComponent<Weapon>());

                    i++;
                }
            }
            void SetUpAmmo()
            {
                foreach (StartingAmmo ammo in startingAmmo)
                {
                    inventoryAmmos.Add(ammo.ammoType, ammo.count);
                }
            }
            private Medkit FindClosestMedkit()
            {
                distanceToMedkit = float.PositiveInfinity;
                Medkit closestMedkit = null;
                foreach(Medkit medkit in Medkit.medkits)
                {
                    if (medkit.enabled)
                    {
                        if (Vector3.Distance(medkit.transform.position, this.transform.position) < distanceToMedkit)
                        {
                            distanceToMedkit = Vector3.Distance(medkit.transform.position, this.transform.position);
                            closestMedkit = medkit;
                        }
                    }
                }
                return closestMedkit;
            }
            private AmmoBox FindClosestAmmo(int ammoType)
            {
                distanceToAmmo = float.PositiveInfinity;
                AmmoBox closestBox = null;
                foreach (AmmoBox ammoBox in AmmoBox.ammoBoxes)
                {
                    if (ammoBox.enabled && ammoBox.ammoType == ammoType)
                    {
                        if (Vector3.Distance(ammoBox.transform.position, this.transform.position) < distanceToAmmo)
                        {
                            distanceToAmmo = Vector3.Distance(ammoBox.transform.position, this.transform.position);
                            closestBox = ammoBox;
                        }
                    }
                }
                return closestBox;
            }
            private void FindClosestWeapon()
            {
                distanceToAmmo = float.PositiveInfinity;
                foreach (AmmoBox ammoBox in AmmoBox.ammoBoxes)
                {
                    if (Vector3.Distance(ammoBox.transform.position, this.transform.position) < distanceToMedkit)
                    {
                        distanceToAmmo = Vector3.Distance(ammoBox.transform.position, this.transform.position);
                    }
                }
            }
            // Update is called once per frame
            void Update()
            {
                timeSinceLastShoot += Time.deltaTime;
                canvas.transform.rotation = Quaternion.LookRotation(canvas.transform.position - Camera.main.transform.position);
                distanceToPlayer = Vector3.Distance(player.transform.position, this.transform.position);
                Medkit medkit = FindClosestMedkit();
                if (currentHealth <= health * 0.4f && distanceToMedkit < distanceToPlayer)
                {
                    navAgent.speed = runningSpeed;
                    navAgent.SetDestination(medkit.transform.position);
                    state = AIState.FindingMedkit;
                }
                else
                {
                    if (weapons.Count > 0)
                    {
                        bool noAmmo = true;
                        if ( (inventoryAmmos[inventoryWeapons[currWeapon].ammoType] + inventoryWeapons[currWeapon].currentMagazine) > 0)
                        {
                            noAmmo = false;
                        }
                        if (noAmmo)
                        {
                            AmmoBox closestBox = FindClosestAmmo(inventoryWeapons[currWeapon].ammoType);
                            if (closestBox != null)
                            {
                                    navAgent.speed = runningSpeed;
                                    navAgent.isStopped = false;
                                    navAgent.SetDestination(closestBox.transform.position);
                                    state = AIState.FindingAmmo;
                            }
                            else
                            {
                                navAgent.isStopped = false;
                                state = AIState.WalkingOnPath;
                            }
                        }
                        else
                        {
                            if (Physics.SphereCast(eyesPos.position, 0.5f, player.hand.transform.position - eyesPos.position, out RaycastHit hit, viewRange))
                            {
                                if (hit.transform.CompareTag("Player") || hit.transform.CompareTag("PlayerPart"))
                                {
                                    if (hit.distance < maxShootRange)
                                    {
                                        state = AIState.Shooting;
                                        animator.SetBool("Aiming", true);
                                        navAgent.isStopped = true;
                                    }
                                    else
                                    {
                                        navAgent.isStopped = false;
                                        state = AIState.RunningToShootPosition;
                                    }
                                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(player.hand.transform.position - eyesPos.position), 2);
                                    StopCoroutine("WaitToLostAggro");
                                    StartCoroutine("WaitToLostAggro");
                                }
                                else
                                {
                                    if (state == AIState.Shooting)
                                    {
                                        state = AIState.RunningToShootPosition;
                                    }
                                    animator.SetBool("Aiming", false);
                                    navAgent.isStopped = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        navAgent.speed = runningSpeed;
                        state = AIState.FindingWeapon;
                    }
                }
                switch (state)
                {
                    case AIState.Idle:
                        break;
                    case AIState.WalkingOnPath:
                        if (navPath != null)
                        {
                            DoPath();
                            navAgent.speed = speed;
                        }
                        break;
                    case AIState.Shooting:
                        if (player != null)
                        {
                            if (navAgent.velocity.magnitude < 0.1f)
                            {
                                IK = player.gameObject.transform.position + Vector3.up;
                            }
                            if (inventoryWeapons[currWeapon].currentMagazine > 0)
                            {
                                if (timeSinceLastShoot >= 60 / (inventoryWeapons[currWeapon].fireRate * 60) && state == AIState.Shooting && Vector3.Distance(transform.position, player.transform.position) <= (maxShootRange + 1))
                                {
                                    animator.SetBool("Aiming", true);
                                    Fire();
                                    timeSinceLastShoot = 0;
                                }
                            }
                            else if (!reloading)
                            {
                                if (inventoryAmmos.ContainsKey(inventoryWeapons[currWeapon].ammoType))
                                {
                                    if (inventoryAmmos[inventoryWeapons[currWeapon].ammoType] > 0)
                                    {
                                        animator.SetTrigger("Reload");
                                        reloading = true;
                                        StopCoroutine("WaitForReload");
                                        StartCoroutine("WaitForReload");
                                    }
                                }
                            }
                        }
                        break;
                    case AIState.FindingPlayer:
                        navAgent.speed = runningSpeed;
                        animator.SetBool("Aiming", false);
                        break;
                    case AIState.FindingAmmo:
                        animator.SetBool("Aiming", false);
                        break;
                    case AIState.FindingWeapon:
                        break;
                    case AIState.FindingMedkit:
                        break;
                    case AIState.RunningToShootPosition:
                        navAgent.SetDestination(player.transform.position);
                        navAgent.speed = runningSpeed;
                        break;
                    default:
                        break;
                }
              
                if (navAgent.velocity.magnitude > 0.1f)
                {
                    animator.SetFloat("Horizontal", 0f);
                    if (navAgent.velocity.magnitude > runningSpeed * 0.9)
                    {
                        animator.SetBool("isRunning", true);
                        animator.SetFloat("Vertical", navAgent.velocity.normalized.magnitude*0.9f);
                    }
                    else
                    {
                        animator.SetBool("isRunning", false);
                        animator.SetFloat("Vertical", navAgent.velocity.normalized.magnitude * 0.9f);
                    }
                }
                else
                {
                    animator.SetFloat("Vertical", 0);
                }
            }

            void Fire()
            {
                float range;
                if (navAgent.velocity.magnitude > 0.1f)
                {
                    range = Vector3.Distance(transform.position, player.transform.position) / 25 * scatter;
                }
                else
                    range = Vector3.Distance(transform.position, player.transform.position) / 50 * scatter;
                Vector3 _pos = player.gameObject.transform.position + Vector3.up;
                    
                    IK = new Vector3(_pos.x + Random.Range(-range, range), _pos.y + Random.Range(-range, range), _pos.z + Random.Range(-range, range));


                inventoryWeapons[currWeapon].Shoot(difficulty);
            }

            public void OnCollisionEnter(Collision collision)
            {
                Projectile projectile = collision.collider.GetComponent<Projectile>();
                if (projectile != null && !projectile.areaDamage)
                {
                    float damageMultiplayer = 1f;
                    switch (collision.GetContact(0).thisCollider.name)
                    {
                        case "Neck_M":
                        case "Head_M":
                            damageMultiplayer = 2;
                            break;
                        case "Root_M":
                        case "Spine1_M":
                        case "Hip_L":
                        case "Hip_R":
                            damageMultiplayer = 1;
                            break;
                        case "Ankle_R":
                        case "Ankle_L":
                        case "Shoulder_L":
                        case "Shoulder_R":
                        case "Scapula_L":
                        case "Scapula_R":
                        case "Elbow_L":
                        case "Elbow_R":
                            damageMultiplayer = 0.7f;
                            break;
                    }
                    TakeDamage(projectile.weaponDamage * damageMultiplayer);
                   

                }
                var playerControler = collision.collider.GetComponent<PlayerController>();
                if (playerControler != null)
                {
                    Debug.Log("Player enter");
                }
            }
            void OnAnimatorIK(int layerIndex)
            {
                if (IK != null && animator.GetBool("Aiming"))
                {

                    //We add an offset to the ik so that we can make the gun aim in the correct location
                    animator.SetLookAtPosition(IK);

                    animator.SetLookAtWeight(1, 1, 1, .2f);

                    if (inventoryWeapons.Count > 0)
                    {
                        if (leftHandTarget != null)
                        {
                            inventoryWeapons[currWeapon].transform.LookAt(IK);
                            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
                            animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);
                            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                        }

                        else
                        {
                            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
                        }
                    }
                }
                else
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                }
            }
            public void TakeDamage(float damage)
            {
                canvas.gameObject.SetActive(true);
                currentHealth -= damage;
                healthSlider.value = currentHealth;
                if (navAgent.enabled)
                {
                    state = AIState.FindingPlayer;
                    navAgent.SetDestination(player.transform.position);
                }
                StopCoroutine("WaitToHideCanvas");
                if (currentHealth <= 0)
                {
                    currentHealth = 0;
                    Death();
                }
                else
                {
                    StartCoroutine("WaitToHideCanvas");
                }
            }
            void Death()
            {
                animator.SetBool("Aiming", false);
                animator.SetTrigger("Death");
                canvas.gameObject.SetActive(false);
                navAgent.enabled = false;
                enabled = false;
                StopCoroutine("WaitToAnim");
                StartCoroutine("WaitToAnim");
            }
            public void PickAmmo(AmmoBox ammo)
            {
                if (inventoryAmmos.ContainsKey(ammo.ammoType))
                    inventoryAmmos[ammo.ammoType] += ammo.count;
                else
                    inventoryAmmos.Add(ammo.ammoType, ammo.count);
                state = AIState.WalkingOnPath;
            }
            public void PickUpMedkit(Medkit medkit)
            {
                if (medkit.instantHeal || medkit.healDuration == 0)
                {
                    if (currentHealth + medkit.healAmount <= health)
                    {
                        currentHealth += medkit.healAmount;
                    }
                    else
                        currentHealth = health;
                    healthSlider.value = currentHealth;
                }
                else
                {
                    StartCoroutine(Healing(medkit.healDuration, medkit.healAmount));
                }
                state = AIState.WalkingOnPath;
            }
            IEnumerator Healing(float time, float heal)
            {
                float deltaTime = 0f;
                while (deltaTime < time)
                {
                    currentHealth += heal * Time.deltaTime / time;
                    deltaTime += Time.deltaTime;
                    if (currentHealth > health)
                    {
                        currentHealth = health;
                    }
                    healthSlider.value = currentHealth;
                    yield return null;
                }
            }
            void DoPath()
            {
                if (navAgent.remainingDistance < 0.25f)
                    navAgent.SetDestination(navPath.GetNextTarget(ref currentCheckpoint));

            }
            private void OnTriggerEnter(Collider other)
            {
                var isPlayer = other.GetComponent<PlayerController>();
                if (isPlayer != null && navAgent.enabled && state != AIState.FindingAmmo && state != AIState.FindingMedkit && state != AIState.FindingWeapon)
                {
                    player = isPlayer;
                    state = AIState.RunningToShootPosition;
                    animator.SetBool("Aiming", true);
                    navAgent.speed = runningSpeed;
                    navAgent.SetDestination(Vector3.MoveTowards(transform.position, player.transform.position, 1));
                }
            }

            private void OnTriggerExit(Collider other)
            {
                var isPlayer = other.GetComponent<PlayerController>();
                if (isPlayer != null && navAgent.enabled && state != AIState.FindingAmmo && state != AIState.FindingMedkit && state != AIState.FindingWeapon)
                {
                    StopCoroutine("WaitToLostAggro");
                    StartCoroutine("WaitToLostAggro");
                }
            }
            IEnumerator WaitForReload()
            {
                yield return new WaitForSeconds(inventoryWeapons[currWeapon].reloadTime);
                if (inventoryAmmos.ContainsKey(inventoryWeapons[currWeapon].ammoType))
                {
                    inventoryAmmos[inventoryWeapons[currWeapon].ammoType] -= inventoryWeapons[currWeapon].Reload(inventoryAmmos[inventoryWeapons[currWeapon].ammoType]);
                }
                reloading = false;
            }
            IEnumerator WaitToLostAggro()
            {
                yield return new WaitForSeconds(timeToLoseAggro);
                
                animator.SetBool("Aiming", false);
                navAgent.stoppingDistance = 0f;
                navAgent.speed = speed;
                if (navPath.enabled && enabled)
                {
                    navAgent.SetDestination(navPath.GetCurrentTarget(currentCheckpoint));
                    state = AIState.WalkingOnPath;
                }
                else
                    state = AIState.Idle;
            }
            IEnumerator WaitToAnim()
            {
                yield return new WaitForSeconds(3);
                gameObject.SetActive(false);
            }
            IEnumerator WaitToHideCanvas()
            {
                yield return new WaitForSeconds(5);
                canvas.gameObject.SetActive(false);
            }
        }
    }
}
