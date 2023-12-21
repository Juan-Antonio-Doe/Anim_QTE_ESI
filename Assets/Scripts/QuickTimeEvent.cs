using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class QuickTimeEvent : MonoBehaviour {

    [field: SerializeField] private Animator anim;
    [field: SerializeField] private float timeToReact = 0.5f;
    [field: SerializeField] private KeyCode keyToPress_1 = KeyCode.A;
    [field: SerializeField] private KeyCode keyToPress_2 = KeyCode.D;
    [field: SerializeField] private float defaultSpeed = 1f;
    [field: SerializeField] private float slowMotionSpeed = 0.2f;

    [field: SerializeField] private Text keyIcon1;
    [field: SerializeField] private Text keyIcon2;

    [field: SerializeField] private Rigidbody rb { get; set; }
    // Virtual camera to follow the player and look at the player.
    [field: SerializeField] private CinemachineVirtualCamera vcam { get; set; }

    private bool leftStep;
    private bool rightStep;
    private bool fail;
    private float timer;

    [field: SerializeField] private KeyCode restartKey = KeyCode.Space;
    [field: SerializeField] private KeyCode quitKey = KeyCode.Escape;
    private bool demostrationEnded;

    void Start() {
        anim.speed = defaultSpeed;

        keyIcon1.text = keyToPress_1.ToString();
        keyIcon2.text = keyToPress_2.ToString();

        keyIcon1.transform.parent.gameObject.SetActive(false);
        keyIcon2.transform.parent.gameObject.SetActive(false);
    }

    void Update() {
        if (leftStep) {

            // Show & Hide Key Icon fast.
            if (keyIcon1.transform.parent.gameObject.activeSelf) {
                keyIcon1.transform.parent.gameObject.SetActive(false);
            }
            else {
                keyIcon1.transform.parent.gameObject.SetActive(true);
            }

            timer += Time.deltaTime;

            if (timer >= timeToReact) {
                ResetQTE();
                anim.SetTrigger("FailLeft");
                return;
            }

            if (Input.GetKeyDown(keyToPress_1)) {
                ResetQTE();
            }
        }
        else if (rightStep) {

            // Show & Hide Key Icon fast.
            if (keyIcon2.transform.parent.gameObject.activeSelf) {
                keyIcon2.transform.parent.gameObject.SetActive(false);
            }
            else {
                keyIcon2.transform.parent.gameObject.SetActive(true);
            }

            timer += Time.deltaTime;

            if (timer >= timeToReact) {
                ResetQTE();
                anim.SetTrigger("FailRight");
                return;
            }

            if (Input.GetKeyDown(keyToPress_2)) {
                ResetQTE();
            }
        }
        else if (fail && !demostrationEnded) {
            rb.AddForce(Vector3.down * 100f);
        }

        if (Input.GetKeyDown(restartKey)) {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
        if (Input.GetKeyDown(quitKey)) {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

    void LeftStep() {
        leftStep = true;
        anim.speed = slowMotionSpeed;
    }

    void RightStep() {
        rightStep = true;
        anim.speed = slowMotionSpeed;
    }

    void ResetQTE() {
        anim.speed = defaultSpeed;
        leftStep = false;
        rightStep = false;
        keyIcon1.transform.parent.gameObject.SetActive(false);
        keyIcon2.transform.parent.gameObject.SetActive(false);
        timer = 0f;
    }

    void Fail() {
        if (demostrationEnded)
            return;

        fail = true;
        rb.isKinematic = false;
        vcam.Follow = null;
        anim.applyRootMotion = false;
        StartCoroutine(StopObject());
    }

    void OnTriggerEnter(Collider other) {
        if (!demostrationEnded)
            StartCoroutine(StopObject());
    }

    IEnumerator StopObject() {
        yield return new WaitForSeconds(2f);
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        demostrationEnded = true;
    }

    private void OnGUI() {
        if (demostrationEnded) {
            GUI.color = Color.red;
            GUI.skin.label.fontSize = 60;
            // Show text on the center of the screen.
            GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 25, 1000, 1000), 
                $"Demostración finalizada. \n" +
                $" - Pulse {restartKey} para reiniciar.\n" +
                $" - Pulse {quitKey} para salir.");
        }
    }
}