using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PolyPerfect
{
    namespace War
    {
        public enum UIHealth
        {
            Text,
            Slider,
            TextAndSlider
        }
        //Base class of all interacteble object (cars, turrents, tanks, planes)
        public class InteractableObject : MonoBehaviour, IInteractable, IDamageable<float>
        {

            public GameObject player;
            public bool hideCursor = true;
            public bool interactingWith;
            public bool canGetOut = false;
            public Transform playerSlot;
            public Transform getOutPoint;
            public GameObject blowUpFX;
            public VirtualCamera virtualCamera;
            public Canvas canvas;
            public KeyCode stopInteractingKey;

            public UIHealth uIHealthType;
            public TextMeshProUGUI uIText;
            public Slider uISlider;

            public float health;

            float currentHealth;
            public void Awake()
            {
                currentHealth = health;
            }
            public void HideCursor()
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
            // Takes care of taking damege
            public void TakeDamage(float ammount)
            {
                currentHealth -= ammount;
                if (currentHealth <= 0)
                {
                    currentHealth = 0;
                    SetUIHealth();
                    BlowUp();
                }
                else
                {
                    SetUIHealth();
                }
            }
            // Handles destroction of gameobject and spawns blowup efect
            public virtual void BlowUp()
            {
                PoolSystem.Instance.Spawn(blowUpFX, transform.position, Quaternion.identity);
                Destroy(transform.parent.gameObject);
            }
            public void OnCollisionEnter(Collision collision)
            {
                // Detect collision of projectiles
                for (int i = 0; i < collision.contactCount; i++)
                {
                    Projectile projectile = collision.GetContact(i).otherCollider.GetComponent<Projectile>();
                    if (projectile != null)
                    {
                        TakeDamage(projectile.weaponDamage);
                        break;
                    }
                }
            }

            protected void OnTriggerEnter(Collider other)
            {
                // Detect player entering trigger collider and allows him to interact whith this object
                if (other.tag == "Player" && !other.isTrigger)
                {
                    PlayerController playerController = other.GetComponent<PlayerController>();
                    if (playerController != null)
                    {
                        playerController.canInteract = true;
                        playerController.interactingObject = gameObject;
                    }
                }
            }

            protected void OnTriggerExit(Collider other)
            {
                // Detect player leaving trigger collider and player can no longer interact whith this object
                if (other.tag == "Player" && !other.isTrigger)
                {
                    PlayerController playerController = other.GetComponent<PlayerController>();
                    if (playerController != null)
                    {
                        playerController.canInteract = false;
                        playerController.interactingObject = null;
                    }
                }
            }

            public virtual void OnInteract(GameObject _player)
            {
                // Set player as parents of this object, increase priority of virtual camera (camera will atach to virtual camera)
                // Enable cancas for this object
                _player.transform.parent = null;
                _player.transform.SetParent(playerSlot);
                player = _player;
                canvas.gameObject.SetActive(true);
                canGetOut = false;
                _player.SetActive(false);
                interactingWith = true;
                if (virtualCamera != null)
                {
                    virtualCamera.priority += 12;
                }
                SetUIHealth();
                StartCoroutine(CoolDownGetInButton());
            }
            // Update health ui for this object
            public void SetUIHealth()
            {
                switch (uIHealthType)
                {
                    case UIHealth.Slider:
                        uISlider.maxValue = health;
                        uISlider.value = currentHealth;
                        break;
                    case UIHealth.Text:
                        uIText.text = ((int)currentHealth).ToString();
                        break;
                    case UIHealth.TextAndSlider:
                        uISlider.maxValue = health;
                        uISlider.value = currentHealth;
                        uIText.text = ((int)currentHealth).ToString();
                        break;
                }
            }
            // Handles player leaving this object
            public virtual void StopInteracting()
            {
                interactingWith = false;
                player.SetActive(true);
                player.GetComponent<PlayerController>().StopInteracting();
                if (virtualCamera != null)
                {
                    virtualCamera.priority -= 12;
                }
                canvas.gameObject.SetActive(false);
            }
            public IEnumerator CoolDownGetInButton()
            {
                yield return new WaitForSeconds(.2f);
                canGetOut = true;
            }
        }
    }
}
