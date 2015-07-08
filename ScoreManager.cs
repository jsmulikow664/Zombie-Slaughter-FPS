using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ScoreManager : MonoBehaviour
{
    /**
    *  Script written by OMA [www.armedunity.com]
    **/

    public int currentScore = 0;

    public float time = 0.0f;
    public Texture hitCrosshairTexture;
    private float alphaHit;
    public AudioClip hitSound;

    public GUISkin mySkin;

    public int pointsToNextRank = 50;
    public int rank = 0;
    public AudioClip rankSound;

    void Update()
    {

        if (time > 0)
        {
            time -= Time.deltaTime;
        }
        alphaHit = time;
    }

    public void DrawCrosshair()
    {
        time = 1.0f;
        GetComponent<AudioSource>().PlayOneShot(hitSound, .5f);
    }

    public void addScore(int value)
    {
        currentScore += value;

        if (currentScore >= pointsToNextRank)
        {
            rank++;
            PlayAudioClip(rankSound, transform.position, 1.0f);
            pointsToNextRank += currentScore;
        }
    }

    public void PlayAudioClip(AudioClip clip, Vector3 position, float volume)
    {
        GameObject go = new GameObject("One shot audio");
        go.transform.position = position;
        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.pitch = Random.Range(0.95f, 1.05f);
        source.Play();
        Destroy(go, clip.length);
    }

    void OnGUI()
    {
        if (!Screen.lockCursor) return;
        GUI.skin = mySkin;
        GUIStyle style1 = mySkin.customStyles[0];

        GUI.Label(new Rect(40, Screen.height - 80, 100, 60), " SCORE :");
        GUI.Label(new Rect(100, Screen.height - 80, 160, 60), "" + currentScore, style1);

        GUI.Label(new Rect(40, Screen.height - 110, 100, 60), " LVL :");
        GUI.Label(new Rect(100, Screen.height - 110, 160, 60), "" + rank, style1);

        GUI.color = new Color(1.0f, 1.0f, 1.0f, alphaHit);
        GUI.DrawTexture(new Rect((Screen.width - hitCrosshairTexture.width) / 2, (Screen.height - hitCrosshairTexture.height) / 2, hitCrosshairTexture.width, hitCrosshairTexture.height), hitCrosshairTexture);
    }
}