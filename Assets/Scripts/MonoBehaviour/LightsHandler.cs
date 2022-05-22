using UnityEngine;
using UnityEngine.UI;

public class LightsHandler : MonoBehaviour
{
    [SerializeField] Color randomizerColor, color1, color2;

    [SerializeField] float maxRandomTime, maxTime, minRandomTime, minTime;

    [SerializeField] Material lightMat;

    [SerializeField] Text number, prompt;

    [SerializeField] string[] glitchList;

    private bool initialCondition;

    private Color current, from, to;

    private float initialTime, randomInitialTime, randomTimer, timer;

    private int dir;

    private void Start()
    {
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

        current = from;
        randomTimer = randomInitialTime = Random.Range(minRandomTime, maxRandomTime);
        timer = initialTime = Random.Range(minTime, maxTime);
        UpdateColors();
        Invoke("Timer", Time.deltaTime);
    }

    private void Randomize()
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
            prompt.text = glitchList[Random.Range(0, glitchList.Length)];
            randomTimer = randomInitialTime = Random.Range(minRandomTime, maxRandomTime);
            Invoke("Randomize", 0.2f);
        }
    }

    private void Timer()
    {
        randomTimer = Mathf.Clamp(randomTimer - Time.deltaTime, 0, randomInitialTime);

        if (randomTimer == 0)
        {
            Invoke("Randomize", Time.deltaTime);
        }

        else
        {
            timer = Mathf.Clamp(timer - Time.deltaTime, 0, initialTime);
            current = Color.Lerp(from, to, 1 - timer / initialTime);
            UpdateColors();

            if (timer == 0)
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
                timer = initialTime = Random.Range(minTime, maxTime);
            }

            Invoke("Timer", Time.deltaTime);
        }
    }

    private void UpdateColors()
    {
        number.color = current;
        lightMat.SetColor("_EmissionColor", current);
    }
}