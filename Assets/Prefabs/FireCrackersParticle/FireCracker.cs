using DG.Tweening;
using UnityEngine;
using System.Collections;

public class FireCracker : MonoBehaviour
{
    [SerializeField] private Material redEmission, blueEmission, fireCracker2Mat;
    [SerializeField] private ParticleSystem fireCrackerType1, fireCrackerType2;
    [SerializeField] private float xLimit;
    internal float yStart;
    int crackCount = 0;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.7f);
        while (crackCount <= 3)
        {
            crackCount++;
            if (Random.value < 0.8)
            {
                BlastFireCracker1();
            }
            else
            {
                BlastFireCracker2();
            }
            yield return new WaitForSeconds(0.45f);
        }

    }

    private void BlastFireCracker1()
    {
        bool isRed = Random.value < 0.5f;
        ParticleSystem fireCracker = Instantiate(fireCrackerType1, transform.parent);
        fireCracker.transform.position = new Vector3(Random.Range(-xLimit, xLimit), yStart + 0.4f, 90);
        fireCracker.GetComponent<Renderer>().material = isRed ? redEmission : blueEmission;
        Destroy(fireCracker.gameObject, 3);
        SoundManager.Inst.Play(Random.value < 0.5 ? "smallBlast1" : "smallBlast2");
        ParticleSystem.MainModule mainCrack = fireCracker.main;
        mainCrack.startColor = isRed ? Color.red : new Color(0, 0.784f, 1, 1);
    }

    private void BlastFireCracker2()
    {
        ParticleSystem fireCracker = Instantiate(fireCrackerType2, transform.parent);
        fireCracker.transform.position = new Vector3(Random.Range(-xLimit, xLimit), yStart + 0.4f, 90);
        fireCracker.transform.GetChild(0).GetComponent<ParticleSystem>().randomSeed = fireCracker.randomSeed = (uint)Random.Range(0, 9999999);
        fireCracker.Play();
        Destroy(fireCracker.gameObject, 3);
        SoundManager.Inst.Play("bigBlast");
    }
}