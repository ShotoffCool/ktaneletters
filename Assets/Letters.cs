using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Letters : MonoBehaviour {

    public KMBombModule module;
    public KMAudio audio;

    public bool[] line1Sol;
    public bool[] line2Sol;
    public bool[] line3Sol;

    public KMSelectable[] line1but;
    public KMSelectable[] line2but;
    public KMSelectable[] line3but;

    bool[] line1state = { false, false, false };
    bool[] line2state = { false, false, false };
    bool[] line3state = { false, false, false };

    public TextMesh[] buttonLetters;

    string[] alphabet = {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"};

    int lowLetter;

    void Start()
    {
        lowLetter = UnityEngine.Random.Range(0, 11);

        AssignLetters();
    }

    void Awake()
    {
        foreach (KMSelectable button in line1but)
        {
            button.OnInteract += delegate () { InputHandler1(button); return false; };
        }
        foreach (KMSelectable button in line2but)
        {
            button.OnInteract += delegate () { InputHandler2(button); return false; };
        }
        foreach (KMSelectable button in line3but)
        {
            button.OnInteract += delegate () { InputHandler3(button); return false; };
        }
    }

    void Update()
    {
        for (int i = 0; i < 3; i++)
        {
            if (line1state[i])
            {
                Transform pos = line1but[i].GetComponent<Transform>();
                
                if (pos.localPosition.y > -0.005f)
                {
                    pos.localPosition -= new Vector3(0, 0.02f * Time.deltaTime, 0);
                }
                else if (pos.localPosition.y < -0.005f)
                {
                    pos.localPosition = new Vector3(pos.localPosition.x, -0.005f, pos.localPosition.z);
                }
            }
        }

        for (int i = 0; i < 3; i++)
        {
            if (line2state[i])
            {
                Transform pos = line2but[i].GetComponent<Transform>();

                if (pos.localPosition.y > -0.005f)
                {
                    pos.localPosition -= new Vector3(0, 0.02f * Time.deltaTime, 0);
                }
                else if (pos.localPosition.y < -0.005f)
                {
                    pos.localPosition = new Vector3(pos.localPosition.x, -0.005f, pos.localPosition.z);
                }
            }
        }

        for (int i = 0; i < 3; i++)
        {
            if (line3state[i])
            {
                Transform pos = line3but[i].GetComponent<Transform>();

                if (pos.localPosition.y > -0.005f)
                {
                    pos.localPosition -= new Vector3(0, 0.02f * Time.deltaTime, 0);
                }
                else if (pos.localPosition.y < -0.005f)
                {
                    pos.localPosition = new Vector3(pos.localPosition.x, -0.005f, pos.localPosition.z);
                }
            }
        }
    }

    void AssignLetters()
    {
        List<string> curLetters = new List<string>();

        for (int i = lowLetter; i < lowLetter + 16; i++)
        {
            curLetters.Add(alphabet[i]);
        }

        for (int i = 0; i < 7; i++)
        {
            curLetters.RemoveAt(UnityEngine.Random.Range(1, 15-i));
        }

        for (int i = 0; i < 9; i++)
        {
            int randIndex = UnityEngine.Random.Range(0, 9 - i);
            buttonLetters[i].text = curLetters[randIndex];
            curLetters.RemoveAt(randIndex);
        }
    }

    void CheckForSolveState()
    {
        bool match = true;

        for(int i = 0; i < line1state.Length; i++)
            if (!(line1state[i] == line1Sol[i+lowLetter]))
                match = false;
        for (int i = 0; i < line2state.Length; i++)
            if (!(line2state[i] == line2Sol[i + lowLetter]))
                match = false;
        for (int i = 0; i < line3state.Length; i++)
            if (!(line3state[i] == line3Sol[i + lowLetter]))
                match = false;

        if (match)
            module.HandlePass();
    }

    void InputHandler1(KMSelectable button)
    {
        int index = Array.IndexOf(line1but, button);

        if (line1Sol[index + lowLetter] == true)
        {
            if (line1state[index] == false)
            {
                line1but[index].GetComponent<MeshRenderer>().material.color = new Color32(150, 150, 150, 255);

                audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                line1but[index].AddInteractionPunch();
            }
            line1state[index] = true;
            CheckForSolveState();
        }
        else
        {
            audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
            line1but[index].AddInteractionPunch();
            module.HandleStrike();
        }

    }
    void InputHandler2(KMSelectable button)
    {
        int index = Array.IndexOf(line2but, button);

        if (line2Sol[index + lowLetter] == true)
        {
            if (line2state[index] == false)
            {
                line2but[index].GetComponent<MeshRenderer>().material.color = new Color32(150, 150, 150, 255);

                audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                line2but[index].AddInteractionPunch();
            }
            line2state[index] = true;
            CheckForSolveState();
        }
        else
        {
            audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
            line2but[index].AddInteractionPunch();
            module.HandleStrike();
        }

    }
    void InputHandler3(KMSelectable button)
    {
        int index = Array.IndexOf(line3but, button);

        if (line3Sol[index + lowLetter] == true)
        {
            if (line3state[index] == false)
            {
                line3but[index].GetComponent<MeshRenderer>().material.color = new Color32(150, 150, 150, 255);

                audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                line3but[index].AddInteractionPunch();
            }
            line3state[index] = true;
            CheckForSolveState();
        }
        else
        {
            audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
            line3but[index].AddInteractionPunch();
            module.HandleStrike();
        }

    }
}