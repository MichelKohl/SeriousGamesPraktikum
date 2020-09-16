using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{
    [SerializeField] private StoryManager manager;
    [SerializeField] private Light spotlight;
    [SerializeField] private Transform playerCharacterTransform;
    [SerializeField] private Transform thirdPersonCam;
    [SerializeField] private Transform deathCam;
    [SerializeField] private float moveStep = 2f;
    [SerializeField] private float moveDelta = 0.001f;
    [SerializeField] private float rotateStep = 2f;
    [SerializeField] private float rotateDelta = 0.001f;
    [SerializeField] private Vector3 startMenuPos;
    [SerializeField] private Vector3 startMenuRot;
 
    private DCPlayer player;

    private Vector3 firstPersonPos;
    private Quaternion firstPersonQuat;

    private Vector3 moveToPosition;
    private Quaternion rotateToRotation;
    private bool positionSet = true;
    private bool firstPerson = true;
    private bool death = false;
    public bool PositionSet { get => positionSet; }
    private float lastSpotlightRange;
    private float lastSpotlightIntensity;


    private void Start()
    {
        firstPersonPos = transform.position;
        firstPersonQuat = transform.rotation;

        ChangeToStartMenuCam();
    }

    // Update is called once per frame
    void Update()
    {
        // TODO delete next two lines, which are for testing purposes
        if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeToFirstPerson();
        if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeToThirdPerson();

        if (player == null) player = manager.GetPlayerCharacter();

        if (!positionSet)
        {
            transform.position = Vector3.MoveTowards(transform.position, moveToPosition, moveStep);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotateToRotation, rotateStep);

            if (Approx(transform.rotation, rotateToRotation, rotateDelta) &&
                Approx(transform.position, moveToPosition, moveDelta))
            {
                positionSet = true;
                firstPerson = !firstPerson;
                if(!death) player.gameObject.SetActive(!firstPerson);
            }
        }
    }

    public void SetSpotlight(float range, float intensity = 1)
    {
        lastSpotlightRange = spotlight.range;
        lastSpotlightIntensity = spotlight.intensity;

        spotlight.range = range;
        spotlight.intensity = intensity;
    }

    public void ResetSpotlight()
    {
        spotlight.range = lastSpotlightRange;
        spotlight.intensity = lastSpotlightIntensity;
    }

    public void UpdateCamPositionAndRotation(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
        firstPersonPos = position;
        firstPersonQuat = rotation;
    }

    public void ChangeToFirstPerson()
    {
        if (positionSet && !firstPerson)
        {
            moveToPosition = firstPersonPos;
            rotateToRotation = firstPersonQuat;
            positionSet = false;
        }
    }

    public void ChangeToThirdPerson()
    {
        if (positionSet && firstPerson)
        {
            moveToPosition = thirdPersonCam.position;
            rotateToRotation = thirdPersonCam.rotation;
            positionSet = false;
            player.transform.position = playerCharacterTransform.position;
            player.transform.rotation = playerCharacterTransform.rotation;
            player.gameObject.SetActive(true);
        }
    }

    public void ChangeToDeathCam()
    {
        if (positionSet && !firstPerson)
        {
            positionSet = false;
            death = true;
            moveToPosition = deathCam.position;
            rotateToRotation = deathCam.rotation;
        }
    }

    public void ChangeToStartMenuCam()
    {
        transform.position = startMenuPos;
        transform.rotation = Quaternion.Euler(startMenuRot);
    }

    private bool Approx(Quaternion current, Quaternion target, float delta)
    {
        return Quaternion.Dot(current, target) > 1f - delta;
    }

    private bool Approx(Vector3 current, Vector3 target, float delta)
    {
        return Vector3.Distance(current, target) < delta;
    }
}
