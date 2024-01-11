using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public List<CharacterController> characterControllers;
    public CharacterController currentCharacterController;


    private bool characterSwitchinputEnabled = true;
    [SerializeField] private float characterSwitchinputDelay = 0.7f;

    private int currentIndex = 0;

    [SerializeField] private string targetCharacterTag = "Characters";
    // Start is called before the first frame update

    private void Awake()
    {
        // Find all objects with the "YourTag" tag
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(targetCharacterTag);

        // Iterate through the found objects
        foreach (GameObject obj in taggedObjects)
        {
            CharacterController temp = obj.GetComponent<CharacterController>();
            if(temp != null) characterControllers.Add(temp);
        }
    }

    void Start()
    {
        getNextCharcter();

        characterSwitchinputEnabled = true;
    }

    private void getNextCharcter()
    {
        // Move to the next element based on direction
        currentIndex = (currentIndex + 1) % characterControllers.Count;

        // Access the current element
        CharacterController currentElement = characterControllers[currentIndex];

        //Set 
        setCurrentCharacter(currentElement);
    }

    private void getPreviousCharcter()
    {
        
        // Move to the Previous element based on direction
        currentIndex = (currentIndex - 1 + characterControllers.Count) % characterControllers.Count;

        // Access the current element
        CharacterController currentElement = characterControllers[currentIndex];


        setCurrentCharacter(currentElement);
    }

    private void setCurrentCharacter(CharacterController characterController)
    {
        //Setting the previous Charcter movement  inactive
        if(currentCharacterController != null)
        {
            currentCharacterController.toggleCharacterActive(false);
        }
        
        //Setting the new characterController
        currentCharacterController = characterController;
        //Setting the new Charcter movement active
        currentCharacterController.toggleCharacterActive(true);

        CameraTargetManager.Instance.setCameraTarget(currentCharacterController.transform);
    }


    // Update is called once per frame
    void Update()
    {
        if(characterControllers.Count < 2)
        {
            GameStateManger.Instance.GameOver.Invoke();
        }


        if (Input.GetKey(KeyCode.Q) && characterSwitchinputEnabled)
        {
            getNextCharcter();
            StartCoroutine(DisableInputForDelay());
        }

        if (Input.GetKey(KeyCode.E) && characterSwitchinputEnabled)
        {
            getPreviousCharcter();
            StartCoroutine(DisableInputForDelay());
        }
    }


    IEnumerator DisableInputForDelay()
    {
        // Disable input
        characterSwitchinputEnabled = false;

        // Wait for the specified delay
        yield return new WaitForSeconds(characterSwitchinputDelay);

        // Enable input
        characterSwitchinputEnabled = true;
    }

}
