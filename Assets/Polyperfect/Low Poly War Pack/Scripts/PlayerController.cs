using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PolyPerfect
{
    namespace War
    {
        public class PlayerController : MonoBehaviour, IDamageable<float>, IPickable
        {
            // Controls speed of walking animation
            [Range(0.1f, 5f)]
            public float speed = 1;
            // Controls speed of running animation
            [Range(1f, 5f)]
            public float runningSpeed;
            // Controls how smooth is transition between idle/walking , walking/running 
            [Range(0f, 1f)]
            public float moveSmoothing;
            // Controls how smooth is changing running/walking direction 
            [Range(0f, 1f)]
            public float turnSmoothing;
            // Transform of player right hand
            public GameObject hand;
            // Player max health
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
            // Player current health
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
            // Id of currently armed weapon
            public int currentWeapon;

            public Transform spawnPoint;
            

            // Auxiliary variables
            float turnSmoothingTime;
            float hsmoothingTime;
            float vsmoothingTime;

            // Boolean that determines if player can enter interactable object
            public bool canInteract = false;
            // Boolean that determines if player is interacting whith object
            public bool interacting = false;
            // True if player realoding his weapon
            public bool reloading = false;

            // Gameobject that player can/is interacting whith
            public GameObject interactingObject;
            // Key whith wich player can enter interactable object
            public KeyCode interactionKeyBind;
            public CharacterController characterController;
            public Transform IK;

            private Transform leftHandTarget;
            private Animator animator;
            private Transform cam;

            public Transform grenadePosition;
            public GameObject grenade;

            public Canvas canvas;
            public List<Image> weaponsUI = new List<Image>();
            public List<Sprite> healthUis = new List<Sprite>();
            public Text healthText;
            public Image healthImage;
            public Text maxAmmo;
            public Text currentAmmo;
            public Image interactImage;
            public Image crosshair;
            public Image deathScreen;

            public List<int> weapons = new List<int>();
            //public List<GameObject> weapons = new List<GameObject>();
            public List<StartingAmmo> startingAmmo = new List<StartingAmmo>();
            public Dictionary<int, int> inventoryAmmos = new Dictionary<int, int>();
            public Dictionary<int, Weapon> inventoryWeapons = new Dictionary<int, Weapon>();

            private SphereCollider soundTrigger;
            Rigidbody rb;
            Vector3 lastPos;
            bool hideCursor = true, swappedWeapon = true;

            float ikBlend = 0;

            // Start is called before the first frame update
            private void Awake()
            {
                SetUpAmmo();
                SetUpWeapons();
                soundTrigger = GetComponent<SphereCollider>();
                rb = GetComponent<Rigidbody>();
            }
            void Start()
            {
                HideCursor();
                lastPos = transform.position;
                animator = GetComponent<Animator>();
                characterController = GetComponent<CharacterController>();
                cam = Camera.main.transform;
                currentHealth = health;
                healthText.text = currentHealth.ToString();
                SetUpStartWeapon();

            }


            void SetUpWeapons()
            {
                int i = 0;
                foreach (int weapon in weapons)
                {
                    if (weapon != 0)
                    {
                        var weaponObj = Instantiate<GameObject>(GameDatabase.Instance.weaponDatabase[weapon - 1], hand.transform);
                        weaponObj.SetActive(false);
                        weaponsUI[i].gameObject.SetActive(true);
                        inventoryWeapons.Add(i, weaponObj.GetComponent<Weapon>());
                        weaponsUI[i].sprite = inventoryWeapons[i].weaponUISpriteDisable;
                        if (inventoryAmmos.ContainsKey(inventoryWeapons[i].ammoType))
                        {
                            int allAmmo = inventoryAmmos[inventoryWeapons[i].ammoType];
                            maxAmmo.text = allAmmo.ToString();
                            if (allAmmo > inventoryWeapons[i].magazine)
                                inventoryWeapons[i].currentMagazine = inventoryWeapons[i].magazine;
                            else
                                inventoryWeapons[i].currentMagazine = allAmmo;
                        }
                        else
                        {
                            inventoryWeapons[i].currentMagazine = 0;
                            maxAmmo.text = "0";
                        }
                        currentAmmo.text = inventoryWeapons[i].currentMagazine.ToString();
                    }
                    i++;
                }
                for (int j = i; j < 7; j++)
                {
                    weaponsUI[j].gameObject.SetActive(false);
                }
            }
            void SetUpAmmo()
            {
                foreach (StartingAmmo ammo in startingAmmo)
                {
                    inventoryAmmos.Add(ammo.ammoType, ammo.count);
                }
            }
            void SetUpStartWeapon()
            {
                if (inventoryWeapons.ContainsKey(currentWeapon))
                {
                    inventoryWeapons[currentWeapon].gameObject.SetActive(true);
                    leftHandTarget = inventoryWeapons[currentWeapon].leftHandPosition;
                    animator.SetBool(inventoryWeapons[currentWeapon].weaponName, true);
                    weaponsUI[currentWeapon].sprite = inventoryWeapons[currentWeapon].weaponUISprite;
                    crosshair.sprite = inventoryWeapons[currentWeapon].crosshair;
                    UpdateAmmoUI();
                }
            }
            // Update is called once per frame
            void Update()
            {
                if (!interacting && currentHealth > 0)
                {

                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        animator.SetTrigger("Jump");
                        rb.AddForce(Vector3.up * rb.mass * 3, ForceMode.Impulse);
                    }

                    WeaponSwitch();
                    interactImage.gameObject.SetActive(false);
                    soundTrigger.radius = (lastPos - transform.position).magnitude / Time.deltaTime;
                    lastPos = transform.position;
                    float animHorizontal;
                    float animVertical;
                    float horizontalRaw = Input.GetAxisRaw("Horizontal");
                    float verticalRaw = Input.GetAxisRaw("Vertical");
                    Vector3 direction = new Vector3(horizontalRaw, 0f, verticalRaw).normalized;

                    if (Input.GetMouseButton(1) && swappedWeapon)
                    {
                        int layerMask = ~(1 << 10);
                        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, 5000f, layerMask))
                        {
                            IK.position = hit.point;
                        }
                        else
                            IK.position = cam.position + cam.forward * 5000;


                        grenadePosition.rotation = Quaternion.LookRotation(IK.position - grenadePosition.position);
                        animator.SetBool("Aiming", true);
                        animator.SetBool(inventoryWeapons[currentWeapon].weaponName, true);
                        animHorizontal = Mathf.SmoothDampAngle(animator.GetFloat("Horizontal"), direction.x * 0.9f * speed, ref hsmoothingTime, moveSmoothing);
                        animVertical = Mathf.SmoothDampAngle(animator.GetFloat("Vertical"), direction.z * 0.9f * speed, ref vsmoothingTime, moveSmoothing);
                        // float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, cam.eulerAngles.y, ref turnSmoothingTime, turnSmoothing);
                        transform.rotation = Quaternion.Euler(0f, cam.eulerAngles.y, 0f);
                        animator.SetBool("isRunning", false);
                        animator.SetFloat("Vertical", animVertical);
                        animator.SetFloat("Horizontal", animHorizontal);
                        if (Input.GetKey(KeyCode.LeftShift))
                        {
                            animator.SetBool("isRunning", true);
                            animator.SetFloat("Vertical", animVertical);
                            animator.SetFloat("Horizontal", animHorizontal);
                        }

                        if (Input.GetMouseButton(0) && !reloading)
                        {
                            if (inventoryWeapons[currentWeapon].currentMagazine > 0)
                            {
                                    Fire();
                                    currentAmmo.text = inventoryWeapons[currentWeapon].currentMagazine.ToString();
                                    soundTrigger.radius += inventoryWeapons[currentWeapon].weaponDamage;
                            }
                        }
                    }
                    else
                    {
                        IK.position = transform.position + transform.forward * 5000;

                        animator.SetBool("Aiming", false);
                        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                        animVertical = Mathf.SmoothDampAngle(animator.GetFloat("Vertical"), direction.normalized.magnitude * speed, ref vsmoothingTime, moveSmoothing);
                        if (direction.magnitude >= 0.1f)
                        {
                            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothingTime, turnSmoothing);
                            transform.rotation = Quaternion.Euler(0f, angle, 0f);
                        }
                        animator.SetFloat("Horizontal", 0f);
                        animator.SetBool("isRunning", false);
                        animator.SetFloat("Vertical", animVertical);

                        if (Input.GetKey(KeyCode.LeftShift))
                        {
                            animator.SetBool("isRunning", true);
                            animator.SetFloat("Vertical", animVertical);
                        }
                    }

                    if (Input.GetKeyDown(KeyCode.R) && !reloading && !inventoryWeapons[currentWeapon].IsMagazineFull())
                    {
                        reloading = true;
                        animator.SetBool("Reloading",true);
                        animator.SetTrigger("Reload");
                        StartCoroutine(WaitForReload());
                    }
                    if (Input.GetKeyDown(KeyCode.G))
                    {
                        PoolSystem.Instance.Spawn(grenade, grenadePosition.position, grenadePosition.rotation);
                    }

                    if (canInteract && interactingObject != null)
                    {
                        interactImage.gameObject.SetActive(true);
                        if (Input.GetKeyDown(interactionKeyBind))
                        {
                            InteractableObject interactable = interactingObject.GetComponent<InteractableObject>();
                            if (interactable != null)
                            {
                                canvas.gameObject.SetActive(false);
                                interacting = true;
                                interactable.OnInteract(gameObject);
                                //StartCoroutine(WaitForAnim(interactable));
                            }
                        }
                    }

                    animator.SetFloat("RunningSpeed", melee ? runningSpeed : runningSpeed * 0.75f);
                    animator.SetFloat("Speed", melee ? speed : speed * 0.75f);
                }
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
                        case "Knee_R":
                        case "Knee_L":
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
            }
            void Fire()
            {
                inventoryWeapons[currentWeapon].Shoot(rb.velocity);
            }
            float layerBlend = 1;
            bool melee;

            void WeaponSwitch()
            {
                //Melee
                if (Input.GetKeyDown(KeyCode.H))
                {
                    melee = !melee;
                }

                inventoryWeapons[currentWeapon].gameObject.SetActive(!melee);

                var speed = (Time.deltaTime * 4f);
                if (melee || animator.GetBool("isRunning"))
                    layerBlend = Mathf.Clamp(layerBlend - speed, 0, 1);
                else
                    layerBlend = Mathf.Clamp(layerBlend + speed, 0, 1);

                animator.SetLayerWeight(2, layerBlend);

                foreach (int i in inventoryWeapons.Keys)
                {
                    if (inventoryWeapons.ContainsKey(i))
                    {
                        if (inventoryWeapons[i] == inventoryWeapons[currentWeapon])
                        {
                            animator.SetBool(inventoryWeapons[i].weaponName, true);
                            continue;
                        }

                        else
                        {
                            animator.SetBool(inventoryWeapons[i].weaponName, false);

                        }
                        if (Input.GetKeyDown(windowKeyCodes[i]))
                        {
                            swappedWeapon = false;
                            leftHandTarget = inventoryWeapons[currentWeapon].leftHandPosition;

                            Debug.Log(windowKeyCodes[i] + ": - If the keyPad buttons do not work for you, change it to whatever you want to use. You can do this by clicking on the gun and changing the keycode");
                            StartCoroutine(WaitForChangeWeapon(inventoryWeapons[currentWeapon].weaponSwitchTime, currentWeapon, i));
                            animator.ResetTrigger("Fire");
                        }
                    }
                }
            }
            void UpdateAmmoUI()
            {
                currentAmmo.text = inventoryWeapons[currentWeapon].currentMagazine.ToString();
                if (inventoryAmmos.ContainsKey(inventoryWeapons[currentWeapon].ammoType))
                {
                    maxAmmo.text = inventoryAmmos[inventoryWeapons[currentWeapon].ammoType].ToString();
                }
            }
            void UpdateHeathUI()
            {
                healthText.text = Mathf.RoundToInt(currentHealth).ToString();

                if (currentHealth >= 50)
                {
                    healthImage.sprite = healthUis[0];
                }
                else if (currentHealth >= 25)
                {
                    healthImage.sprite = healthUis[1];
                }
                else if (currentHealth < 25)
                {
                    healthImage.sprite = healthUis[2];
                }
                if (currentHealth <= 0)
                {
                    healthText.text = "0";
                }
            }
            IEnumerator WaitForChangeWeapon(float time, int oldValue, int newId)
            {
                yield return new WaitForSeconds(time);
                currentWeapon = newId;
                inventoryWeapons[oldValue].gameObject.SetActive(false);
                inventoryWeapons[currentWeapon].gameObject.SetActive(true);
                weaponsUI[oldValue].sprite = inventoryWeapons[oldValue].weaponUISpriteDisable;
                weaponsUI[currentWeapon].sprite = inventoryWeapons[currentWeapon].weaponUISprite;
                crosshair.sprite = inventoryWeapons[currentWeapon].crosshair;
                UpdateAmmoUI();
                swappedWeapon = true;
            }
            public void TakeDamage(float ammount)
            {
                currentHealth -= ammount;
                UpdateHeathUI();
                if (currentHealth <= 0 && !animator.GetBool("Death"))
                {
                    Death();
                }
            }
            void Death()
            {
                animator.SetBool("Death", true);
                StartCoroutine(WaitForDeathScreen());
                deathScreen.transform.GetChild(1).gameObject.SetActive(false);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            public void Respawn()
            {
                transform.position = spawnPoint.position;
                deathScreen.gameObject.SetActive(false);
                currentHealth = health;
                UpdateHeathUI();
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                animator.SetBool("Death", false);
                lastPos = transform.position;
                gameObject.SetActive(true);
            }
            public void PickAmmo(AmmoBox ammo)
            {
                if (inventoryAmmos.ContainsKey(ammo.ammoType))
                    inventoryAmmos[ammo.ammoType] += ammo.count;
                else
                    inventoryAmmos.Add(ammo.ammoType, ammo.count);
                UpdateAmmoUI();
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
                    UpdateHeathUI();
                }
                else
                {
                    StartCoroutine(Healing(medkit.healDuration, medkit.healAmount));
                }
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
                    UpdateHeathUI();
                    yield return null;
                }
            }
            IEnumerator WaitForReload()
            {
                animator.SetTrigger("Reload");
                yield return new WaitForSeconds(inventoryWeapons[currentWeapon].reloadTime);
                if (inventoryAmmos.ContainsKey(inventoryWeapons[currentWeapon].ammoType))
                {
                    inventoryAmmos[inventoryWeapons[currentWeapon].ammoType] -= inventoryWeapons[currentWeapon].Reload(inventoryAmmos[inventoryWeapons[currentWeapon].ammoType]);
                    UpdateAmmoUI();
                }
                reloading = false;
                animator.SetBool("Reloading", false);
            }
                    KeyCode[] windowKeyCodes = {
                KeyCode.Alpha1,
                KeyCode.Alpha2,
                KeyCode.Alpha3,
                KeyCode.Alpha4,
                KeyCode.Alpha5,
                KeyCode.Alpha6,
                KeyCode.Alpha7,
                KeyCode.Alpha8,
                KeyCode.Alpha9,
                KeyCode.Alpha0
            };

                    KeyCode[] macKeyCodes = {
                KeyCode.Keypad1,
                KeyCode.Keypad2,
                KeyCode.Keypad3,
                KeyCode.Keypad4,
                KeyCode.Keypad5,
                KeyCode.Keypad6,
                KeyCode.Keypad7,
                KeyCode.Keypad8,
                KeyCode.Keypad9,
                KeyCode.Keypad0
            };
            bool GroudCheck()
            {
                Ray ray = new Ray(transform.position, -transform.up);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100))
                {
                    if (hit.distance < 0.01)
                        return true;
                }
                return false;
            }
            public void StopInteracting()
            {
                canvas.gameObject.SetActive(true);
                transform.parent = null;
                interacting = false;
                animator.SetFloat("RunningSpeed", runningSpeed);
                animator.SetFloat("Speed", speed);
                lastPos = transform.position;
                //StartCoroutine(WaitForGetOut());
            }
            void OnAnimatorIK(int layerIndex)
            {
                if (IK != null)
                {
                    if (reloading || melee || animator.GetBool("isRunning"))
                    {
                        ikBlend = Mathf.Clamp(ikBlend - (Time.deltaTime * .5f), 0, 1);
                    }
                    else
                    {
                        ikBlend = Mathf.Clamp(ikBlend + (Time.deltaTime * .5f), 0, 1);
                    }

                    animator.SetLookAtPosition(IK.position);
                    animator.SetLookAtWeight(ikBlend, 1, 1, .2f);
                    if(animator.GetBool("Aiming") && !reloading)
                        inventoryWeapons[currentWeapon].transform.LookAt(IK);
                }

            }
            IEnumerator WaitForAnim(IInteractable interactable)
            {
                yield return new WaitForSeconds(2);
                interacting = true;
                interactable.OnInteract(gameObject);
                //animator.enabled = false;

            }
            IEnumerator WaitForGetOut()
            {
                yield return new WaitForSeconds(2);

            }
            IEnumerator WaitForDeathScreen()
            {
                yield return new WaitForSeconds(2);
                float deltaTime = 0f;
                deathScreen.color = new Color(0, 0, 0, 0);
                deathScreen.gameObject.SetActive(true);
                while (deltaTime < 2)
                {
                    deltaTime += Time.deltaTime;
                    deathScreen.color = new Color(0,0,0,deltaTime *0.5f);
                    yield return null;
                }
                deathScreen.transform.GetChild(1).gameObject.SetActive(true);
                gameObject.SetActive(false);
            }
            void HideCursor()
            {
                if (!hideCursor)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;

                    if (Input.GetMouseButtonUp(0))
                    {
                        hideCursor = true;
                    }
                }

                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;

                    if (Input.GetKeyUp(KeyCode.Escape))
                    {
                        hideCursor = false;
                    }
                }
            }
            [ContextMenu("Swap Keycodes Pc vs Mac")]
            void ChangeKeyBindingsIfMac()
            {
                bool isWinEditor = Application.platform == RuntimePlatform.WindowsEditor;
                bool isOSXEditor = Application.platform == RuntimePlatform.OSXEditor;

                if (isWinEditor)
                {
                    for (int i = 0; i < weapons.Count; i++)
                    {
                        //  weapons[i].GetComponent<WeaponController>().weaponSwitchKey = windowKeyCodes[i];
                    }
                }

                if (isOSXEditor)
                {
                    for (int i = 0; i < weapons.Count; i++)
                    {
                        // weapons[i].GetComponent<WeaponController>().weaponSwitchKey = macKeyCodes[i];
                    }
                }
            }

            public GameObject before;
            public void RemoveMag()
            {
                var weapon = inventoryWeapons[currentWeapon];

                if (weapon.modelMagazine != null)
                {
                    before = new GameObject("Temp");
                    before.transform.parent = weapon.modelMagazine.transform.parent;
                    before.transform.position = weapon.modelMagazine.transform.position;
                    before.transform.rotation = weapon.modelMagazine.transform.rotation;
                    before.transform.localScale = weapon.modelMagazine.transform.localScale;

                    weapon.modelMagazine.transform.parent = animator.GetBoneTransform(HumanBodyBones.LeftHand);
                }
            }

            public void AttachMag()
            {
                var weapon = inventoryWeapons[currentWeapon];
                if (weapon.modelMagazine != null && before != null)
                {
                    weapon.modelMagazine.transform.parent = before.transform.parent;
                    weapon.modelMagazine.transform.position = before.transform.position;
                    weapon.modelMagazine.transform.rotation = before.transform.rotation;
                    weapon.modelMagazine.transform.localScale = before.transform.localScale;
                    Destroy(before);
                }
            }
        }
        public class AmmoTypeAttribute : PropertyAttribute
        {

        }

#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(AmmoTypeAttribute))]
        public class AmmoTypePropertyDrawer : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                int currentIDx = -1;
                int currentID = property.intValue;
                var db = GameDatabase.Instance;
                // string[] names = db.GetAmmoAllNames();
                string[] names = new string[db.ammoDatabase.Count];
                for (int i = 0; i < db.ammoDatabase.Count; ++i)
                {
                    names[i] = db.ammoDatabase[i].ammoName;
                    if (db.ammoDatabase[i].ammoType == currentID)
                        currentIDx = i;
                }
                EditorGUI.BeginChangeCheck();
                int idx = EditorGUI.Popup(position, "Ammo Type", currentIDx, names);
                if (EditorGUI.EndChangeCheck())
                {
                    property.intValue = db.ammoDatabase[idx].ammoType;
                }
            }
        }

#endif
    }
}