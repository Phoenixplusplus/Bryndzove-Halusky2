using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoCamera : MonoBehaviour {

    public Camera demoCamera;
    public Transform gunsTransform, dudeTransform, pickupTransform;
    public GameObject Body, Head, LArm, RArm;

    int scene, maxScene;

    bool isFocusGuns, isFocusDude, isFocusDudeSnap1, isFocusDudeSnap2, isFocusDudeSnap3, isFocusPaint;
    public Dictionary<int, Material> characterTexDict;
    public Material Head1, Head2, Head3, Head4, Head5, Head6, Head7, Head8, Head9;
    public Material Body1, Body2, Body3, Body4, Body5, Body6, Body7, Body8, Body9;

    // Use this for initialization
    void Start ()
    {
        characterTexDict = new Dictionary<int, Material>();
        characterTexDict.Add(1, Head1);
        characterTexDict.Add(2, Head2);
        characterTexDict.Add(3, Head3);
        characterTexDict.Add(4, Head4);
        characterTexDict.Add(5, Head5);
        characterTexDict.Add(6, Head6);
        characterTexDict.Add(7, Head7);
        characterTexDict.Add(8, Head8);
        characterTexDict.Add(9, Head9);
        characterTexDict.Add(11, Body1);
        characterTexDict.Add(12, Body2);
        characterTexDict.Add(13, Body3);
        characterTexDict.Add(14, Body4);
        characterTexDict.Add(15, Body5);
        characterTexDict.Add(16, Body6);
        characterTexDict.Add(17, Body7);
        characterTexDict.Add(18, Body8);
        characterTexDict.Add(19, Body9);

        scene = 0;
        maxScene = 4;
        isFocusGuns = false;
        isFocusDude = false;
        isFocusDudeSnap1 = false;
        isFocusDudeSnap2 = false;
        isFocusDudeSnap3 = false;
        isFocusPaint = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        switch (scene)
        {
            case 0: break;
            case 1:
                {
                    if (!isFocusGuns) FocusGuns();
                    demoCamera.transform.RotateAround(gunsTransform.position, Vector3.up, 40 * Time.deltaTime);
                    demoCamera.transform.LookAt(gunsTransform, Vector3.up);
                    break;
                }
            case 2:
                {
                    if (!isFocusDude) FocusDudeSnap1();
                    if (isFocusDudeSnap1)
                    {
                        Vector3 snapLerp = Vector3.Lerp(transform.position, new Vector3(17f, 0.4f, -21f), 2.5f * Time.deltaTime);
                        demoCamera.transform.position = snapLerp;
                        if (snapLerp.x < 17.5f) FocusDudeSnap2();
                    }
                    if (isFocusDudeSnap2)
                    {
                        Vector3 snapLerp = Vector3.Lerp(transform.position, new Vector3(21.4f, 2f, -21f), 2.5f * Time.deltaTime);
                        demoCamera.transform.position = snapLerp;
                        if (snapLerp.x > 20.9f) FocusDudeSnap3();
                    }
                    if (isFocusDudeSnap3)
                    {
                        Vector3 snapLerp = Vector3.Lerp(transform.position, new Vector3(19.4595f, 2.091285f, -19.83152f), Time.deltaTime);
                        demoCamera.transform.position = snapLerp;
                        demoCamera.transform.LookAt(Body.transform.position + new Vector3(0, 0.5f, 0), Vector3.up);
                    }
                    break;
                }
            case 3:
                {
                    demoCamera.transform.position = new Vector3(-20.70979f, 1.478f, -19.06917f);
                    demoCamera.transform.rotation = new Quaternion(0, 180, 0f, 1);
                    demoCamera.transform.LookAt(pickupTransform);
                    break;
                }
            case 4:
                {
                    if (!isFocusPaint) FocusPaint();
                    Vector3 snapLerp = Vector3.Lerp(transform.position, new Vector3(-4.4f, 3.58f, -13.78f), Time.deltaTime / 2);
                    demoCamera.transform.position = snapLerp;
                    if (snapLerp.z < 13.2f) Debug.Log("FIN");
                    break;
                }
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (scene < maxScene) scene++;
            else scene = 0;
            isFocusGuns = false;
            isFocusDude = false;
            isFocusDudeSnap1 = false;
            isFocusDudeSnap2 = false;
            isFocusDudeSnap3 = false;
            isFocusPaint = false;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (scene > 0) scene--;
            else scene = maxScene;
            isFocusGuns = false;
            isFocusDude = false;
            isFocusDudeSnap1 = false;
            isFocusDudeSnap2 = false;
            isFocusDudeSnap3 = false;
            isFocusPaint = false;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            StartCoroutine(RandomColourDude(50f));
        }
    }
    
    void FocusGuns()
    {
        demoCamera.transform.position = new Vector3(-5.970694f, 1.557681f, 17.69923f);
        demoCamera.transform.rotation = new Quaternion(0, 180, 0f, 1);
        isFocusGuns = true;
    }

    void FocusDudeSnap1()
    {
        isFocusDude = true;
        demoCamera.transform.position = new Vector3(21.4f, 0.4f, -21f);
        demoCamera.transform.rotation = new Quaternion(0, 180, 0f, 1);
        isFocusDudeSnap1 = true;
    }

    void FocusDudeSnap2()
    {
        isFocusDudeSnap1 = false;
        demoCamera.transform.position = new Vector3(17f, 2f, -21f);
        demoCamera.transform.rotation = new Quaternion(0, 180, 0f, 1);
        isFocusDudeSnap2 = true;
    }

    void FocusDudeSnap3()
    {
        isFocusDudeSnap2 = false;
        demoCamera.transform.position = new Vector3(19.38725f, 1.671069f, -21.10727f);
        demoCamera.transform.rotation = new Quaternion(0, 180, 0f, 1);
        isFocusDudeSnap3 = true;
    }

    void FocusPaint()
    {
        demoCamera.transform.position = new Vector3(0.87f, 3.58f, -18.3f);
        demoCamera.transform.eulerAngles = new Vector3(13.278f, 220.643f, 0f);
        isFocusPaint = true;
    }

    IEnumerator RandomColourDude(float time)
    {
        float colourTime = 0f;
        while (colourTime < time)
        {
            colourTime += Time.deltaTime;
            int randSkin = Random.Range(1, 10);
            int randBody = Random.Range(11, 20);

            Body.GetComponent<Renderer>().material = characterTexDict[randBody];
            Head.GetComponent<Renderer>().material = characterTexDict[randSkin];
            LArm.GetComponent<Renderer>().material = characterTexDict[randSkin];
            RArm.GetComponent<Renderer>().material = characterTexDict[randSkin];

            if (!isFocusDudeSnap3) yield break;

            yield return new WaitForSeconds(0.2f);
        }
        yield break;
    }
}
