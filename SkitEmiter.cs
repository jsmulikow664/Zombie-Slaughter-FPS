using UnityEngine;
using System.Collections;

public class SkitEmiter : MonoBehaviour
{

    public SoundController skid;
    public ParticleEmitter emiterFL;
    public ParticleEmitter emiterFR;
    public ParticleEmitter emiterRL;
    public ParticleEmitter emiterRR;

    void Update()
    {
        if (skid.emit == true)
        {
            emiterFL.emit = true;
            emiterFR.emit = true;
            emiterRL.emit = true;
            emiterRR.emit = true;
        }
        else
        {
            emiterFL.emit = false;
            emiterFR.emit = false;
            emiterRL.emit = false;
            emiterRR.emit = false;
        }
    }
}