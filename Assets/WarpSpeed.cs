using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class WarpSpeed : MonoBehaviour
{
    public VisualEffect WarpDrive;
    public float rate = 0.02f;

    public bool warpActive;
    // Start is called before the first frame update
    void Start()
    {
        WarpDrive.Stop();
        WarpDrive.SetFloat("WarpAmount", 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            warpActive = true;
            StartCoroutine(ActivateParticles());
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            warpActive = false;
            StartCoroutine(ActivateParticles());
        }
        
    }

    IEnumerator ActivateParticles()
    {
        if (warpActive)
        {
            WarpDrive.Play();

            float amount = WarpDrive.GetFloat("WarpAmount");
            while(amount < 1 & warpActive)
            {
                amount =+ rate;
                WarpDrive.SetFloat("WarpAmount", amount);
                yield return new WaitForSeconds(0.1f);
            }
        }
        else
        {
            float amount = WarpDrive.GetFloat("WarpAmount");
            while (amount > 0 & !warpActive)
            {
                amount =- rate;
                WarpDrive.SetFloat("WarpAmount", amount);
                yield return new WaitForSeconds(0.1f);

                if(amount <= 0 + rate)
                {
                    amount = 0;
                    WarpDrive.SetFloat("WarpAmount", amount);
                    WarpDrive.Stop();
                }
            }

            
        }
    }
}
