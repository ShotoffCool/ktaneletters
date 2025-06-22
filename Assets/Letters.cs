using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Random = UnityEngine.Random;

public class Letters : MonoBehaviour
{
    public KMBombModule Module;
    public KMAudio Audio;
    public KMSelectable[] Buttons;
    public TextMesh[] ButtonLetters;
    public KMRuleSeedable RuleSeedable;

    private readonly bool[] state = new bool[9];
    private bool[] solution;    // will be populated by rule seed
    private bool moduleSolved;
    private int lowLetter;
    private int moduleId;
    private static int moduleIdCounter = 1;

    void Start()
    {
        moduleId = moduleIdCounter++;
        var rnd = RuleSeedable.GetRNG();
        Debug.Log($"[Letters #{moduleId}] Using rule seed: {rnd.Seed}");
        solution = new bool[3 * 13];

        // Fill the 13×3 grid with random booleans such that no 3×3 square is completely false.
        // A cell has a 40% chance of being true.
        // If we are in the bottom row, and the 3×3 above and to our left is all empty, we force a true value.
        for (var i = 0; i < 3 * 13; i++)
            solution[i] = i >= 2 * 13 + 2 && Enumerable.Range(0, 8).All(j => !solution[i - 2 * 13 - 2 + (j % 3) + 13 * (j / 3)]) || rnd.Next(0, 5) < 2;

        lowLetter = Random.Range(0, 11);
        Debug.Log($"[Letters #{moduleId}] Letter range is: {(char) ('A' + lowLetter)}–{(char) ('A' + lowLetter + 15)}");

        AssignLetters();
    }

    void Awake()
    {
        for (var buttonIx = 0; buttonIx < Buttons.Length; buttonIx++)
            Buttons[buttonIx].OnInteract += InputHandler(buttonIx);
    }

    void Update()
    {
        for (int i = 0; i < 9; i++)
        {
            if (state[i])
            {
                Transform pos = Buttons[i].GetComponent<Transform>();

                if (pos.localPosition.y > -0.005f)
                    pos.localPosition -= new Vector3(0, 0.02f * Time.deltaTime, 0);
                else if (pos.localPosition.y < -0.005f)
                    pos.localPosition = new Vector3(pos.localPosition.x, -0.005f, pos.localPosition.z);
            }
        }
    }

    void AssignLetters()
    {
        var curLetters = new List<char>();
        for (int i = lowLetter; i < lowLetter + 16; i++)
            curLetters.Add((char) ('A' + i));

        for (int i = 0; i < 7; i++)
            curLetters.RemoveAt(Random.Range(1, 15 - i));

        for (int i = 0; i < 9; i++)
        {
            int randIndex = Random.Range(0, 9 - i);
            ButtonLetters[i].text = curLetters[randIndex].ToString();
            curLetters.RemoveAt(randIndex);
        }
    }

    void CheckForSolveState()
    {
        bool match = true;

        for (int i = 0; i < state.Length; i++)
            if (state[i] != solution[i % 3 + 13 * (i / 3) + lowLetter])
                match = false;

        if (match)
        {
            Module.HandlePass();
            moduleSolved = true;
        }
    }

    private KMSelectable.OnInteractHandler InputHandler(int index)
    {
        return delegate
        {
            if (state[index])
                return false;
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
            Buttons[index].AddInteractionPunch();
            if (moduleSolved)
                return false;
            if (solution[index % 3 + 13 * (index / 3) + lowLetter])
            {
                Buttons[index].GetComponent<MeshRenderer>().material.color = new Color32(150, 150, 150, 255);
                state[index] = true;
                CheckForSolveState();
            }
            else
            {
                Module.HandleStrike();
            }
            return false;
        };
    }

    // Twitch Plays
#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} A1, B2 [columns A–C, rows 1–3] | !{0} 1, 5 [buttons in reading order]";
#pragma warning restore 414

    List<KMSelectable> ProcessTwitchCommand(string command)
    {
        var buttons = new List<KMSelectable>();
        while (!Regex.IsMatch(command, @"^\s*$"))
        {
            Match m;
            int result;
            if ((m = Regex.Match(command, @"^\s*(?<col>[A-C])(?<row>[1-3])\s*[;,]?", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)).Success)
                buttons.Add(Buttons[m.Groups[1].Value.ToLowerInvariant()[0] - 'a' + 3 * (m.Groups[2].Value[0] - '1')]);
            else if ((m = Regex.Match(command, @"^\s*(\d+)\s*[;,]?")).Success && int.TryParse(m.Groups[1].Value, out result) && result >= 1 && result <= 9)
                buttons.Add(Buttons[result - 1]);
            else
                return null;
            command = command.Substring(m.Length);
        }
        return buttons.Count > 0 ? buttons : null;
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        if (moduleSolved)
            yield break;

        for (var i = 0; i < 9; i++)
            if (state[i] != solution[i % 3 + 13 * (i / 3) + lowLetter])
            {
                Buttons[i].OnInteract();
                yield return new WaitForSeconds(.2f);
            }
    }
}