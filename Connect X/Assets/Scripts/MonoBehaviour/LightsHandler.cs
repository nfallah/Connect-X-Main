/* The LightsHandler script deals with the lighting found in the main menu, which involves
 * -the switching from red to yellow and vice-versa, as well as the frequent "glitching" of text
 * -that randomly occurs from time to time.
 */ 

using UnityEngine;
using UnityEngine.UI;

public class LightsHandler : MonoBehaviour
{
    [SerializeField] Color randomizerColor, color1, color2; // randomizerColor is the color of the glitched text, and color1 and color2 are the colors that the scene transfers between.

    [SerializeField] float maxRandomTime, maxTime, minRandomTime, minTime; // Time values for the glitching, as well as switching between color1 and color2.

    [SerializeField] Material lightMat; // The material attached to GameObjects in the scene; changes based on color1 and color2

    [SerializeField] Text number, prompt; // The "4" in Connect 4 (though it glitches to other numbers) and the "press X to play" prompt respectively; UI elements

    [SerializeField] string[] glitchList; // List of things to replace the prompt with when the UI elements "glitch"

    private bool initialCondition; // the conditions which determines the "glitched" and "normal" states and their respective transitions

    private Color current, from, to; // from and to are either color1 or color2, and current is a blend of said colors determined through linear interpolation

    private float initialTime, randomInitialTime, randomTimer, timer; // Various time values used to keep the glitchiness and color blending going

    private int dir; // Alternates color1 and color2 transitions to ensures a constant cycle of color switching

    private void Start()
    {
        // Conditional that determines what direction the color blending should start with
        if (Random.Range(0, 2) == 1)
        {
            dir = 1;
            from = color1;
            to = color2;
        }

        else
        {
            dir = -1;
            from = color2;
            to = color1;
        }

        current = from; // The current color IS the from color, at least initially
        randomTimer = randomInitialTime = Random.Range(minRandomTime, maxRandomTime); // Sets the random timer (for glitching)
        timer = initialTime = Random.Range(minTime, maxTime); // Sets the normal timer (for color blending)
        UpdateColors();
        Invoke("Timer", Time.deltaTime);
    }

    private void Randomize() // As soon as the text is glitched, it invokes itself in 0.2f seconds to unglitch itself and resume the normal color blending cycle
    {
        if (initialCondition)
        {
            initialCondition = false;
            number.color = current;
            number.text = "4";
            prompt.text = "press any key to play";
            Invoke("Timer", Time.deltaTime);
        }

        else
        {
            initialCondition = true;
            number.color = randomizerColor;
            number.text = Random.Range(5, 21).ToString();
            prompt.text = glitchList[Random.Range(0, glitchList.Length)]; // Prompt replaced by random text determined in the Unity inspector
            randomTimer = randomInitialTime = Random.Range(minRandomTime, maxRandomTime); // The random timer is refreshed
            Invoke("Randomize", 0.2f);
        }
    }

    // The mainstay of the lighting; deals with the glitchiness as well as the color blending
    private void Timer()
    {
        randomTimer = Mathf.Clamp(randomTimer - Time.deltaTime, 0, randomInitialTime);

        // Should randomize, or just normally continue with the color blending cycle
        if (randomTimer == 0)
        {
            Invoke("Randomize", Time.deltaTime);
        }

        else
        {
            timer = Mathf.Clamp(timer - Time.deltaTime, 0, initialTime); // Decrements timer
            current = Color.Lerp(from, to, 1 - timer / initialTime); // Determines the value between 0 and 1 to blend the current color with
            UpdateColors();

            if (timer == 0) // If the time has reached 0, then refresh the cycle and reverse the color blending process (switch from and to with each other)
            {
                dir *= -1;

                if (dir == 1)
                {
                    from = color1;
                    to = color2;
                }

                else
                {
                    from = color2;
                    to = color1;
                }

                current = from;
                timer = initialTime = Random.Range(minTime, maxTime); // Refreshes time value
            }

            Invoke("Timer", Time.deltaTime);
        }
    }

    // Changes the colors of UI and scene elements based on the current blend color
    private void UpdateColors()
    {
        number.color = current;
        lightMat.SetColor("_EmissionColor", current);
    }
}