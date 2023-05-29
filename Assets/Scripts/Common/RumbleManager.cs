using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RumbleManager : MonoBehaviour
{
    public void Rumble(float rumbleForce, float rumbleTime)
	{
		if (Gamepad.current != null)
		{
			
		}

		StartCoroutine(StartRumble(rumbleForce, rumbleTime));
	}

	public void StopRumble()
	{
		StartCoroutine(StartRumble(0, 0));

	}

	IEnumerator StartRumble(float rumbleForce, float rumbleTime)
	{
        //Gamepad.current.SetMotorSpeeds(rumbleForce, rumbleForce);

        for (int i = 0; i < Gamepad.all.Count; i++)
        {
			Gamepad.all[i].SetMotorSpeeds(rumbleForce, rumbleTime);
        }

		yield return new WaitForSeconds(rumbleTime);
		Gamepad.current.SetMotorSpeeds(0, 0);
	}
}
