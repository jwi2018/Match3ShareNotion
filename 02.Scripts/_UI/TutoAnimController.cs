using System.Collections.Generic;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

public class TutoAnimController : MonoBehaviour
{
    [SerializeField] private List<GameObject> Lightningobj;

    [SerializeField] private GameObject RainbowParticle;

    [SerializeField] private Text TimeBombText;

    [SerializeField] private int TimeBombTurn;

    [SerializeField] private Toggle MatchToggle;

    [SerializeField] private Text TutorialText;

    public void OnTutorialPop()
    {
        GetComponent<Animator>().SetTrigger("Tutorial0");
        TutorialText.GetComponent<Localize>().SetTerm("3Match");
        if (MatchToggle != null)
        {
            MatchToggle.isOn = true;
            MatchToggle.transform.parent.localPosition = new Vector2(MatchToggle.transform.parent.localPosition.x, 0);
        }
    }

    public void OnWhatTutorial(int num)
    {
        GetComponent<Animator>().SetTrigger("Tutorial" + num);
        switch (num)
        {
            #region ----- case 0 ~ 9 -----

            case 0:
                TutorialText.GetComponent<Localize>().SetTerm("3Match");
                break;
            case 1:
                TutorialText.GetComponent<Localize>().SetTerm("4Match");
                break;
            case 2:
                TutorialText.GetComponent<Localize>().SetTerm("5Match_L");
                break;
            case 3:
                TutorialText.GetComponent<Localize>().SetTerm("5Match_T");
                break;
            case 4:
                TutorialText.GetComponent<Localize>().SetTerm("5Match_I");
                break;
            case 5:
                TutorialText.GetComponent<Localize>().SetTerm("Combine");
                break;
            case 6:
                TutorialText.GetComponent<Localize>().SetTerm("StoneTile");
                break;
            case 7:
                TutorialText.GetComponent<Localize>().SetTerm("Oak");
                break;
            case 8:
                TutorialText.GetComponent<Localize>().SetTerm("Relic");
                break;
            case 9:
                TutorialText.GetComponent<Localize>().SetTerm("Box");
                break;

            #endregion ----- case 0 ~ 9 -----

            #region ----- case 10 ~ 19 -----

            case 10:
                TutorialText.GetComponent<Localize>().SetTerm("Sand");
                break;
            case 11:
                TutorialText.GetComponent<Localize>().SetTerm("Grass");
                break;
            case 12:
                TutorialText.GetComponent<Localize>().SetTerm("Shield");
                break;
            case 13:
                TutorialText.GetComponent<Localize>().SetTerm("Nautilus");
                break;
            case 14:
                TutorialText.GetComponent<Localize>().SetTerm("Barricades");
                break;
            case 15:
                TutorialText.GetComponent<Localize>().SetTerm("Bubble");
                break;
            case 16:
                TutorialText.GetComponent<Localize>().SetTerm("Hat");
                break;
            case 17:
                TutorialText.GetComponent<Localize>().SetTerm("Gravity");
                break;
            case 18:
                TutorialText.GetComponent<Localize>().SetTerm("Prison");
                break;
            case 19:
                TutorialText.GetComponent<Localize>().SetTerm("Lamp");
                break;

            #endregion ----- case 10 ~ 19 -----

            #region ----- case 20 ~ 29 -----

            case 20:
                TutorialText.GetComponent<Localize>().SetTerm("Tunnel");
                break;
            case 21:
                TutorialText.GetComponent<Localize>().SetTerm("Anemone");
                break;
            case 22:
                TutorialText.GetComponent<Localize>().SetTerm("Rail");
                break;
            case 23:
                TutorialText.GetComponent<Localize>().SetTerm("Pearl");
                break;
            case 24:
                TutorialText.GetComponent<Localize>().SetTerm("Ice");
                break;
            case 25:
                TutorialText.GetComponent<Localize>().SetTerm("GiftBox");
                break;
            case 26:
                TutorialText.GetComponent<Localize>().SetTerm("Seaweeds");
                break;
            case 27:
                TutorialText.GetComponent<Localize>().SetTerm("Lemon");
                break;
            case 28:
                TutorialText.GetComponent<Localize>().SetTerm("Chameleon");
                break;
            case 29:
                TutorialText.GetComponent<Localize>().SetTerm("ColorBox");
                break;

            #endregion ----- case 20 ~ 29 -----

            #region ---- case 30 ~ 39 -----

            case 30:
                TutorialText.GetComponent<Localize>().SetTerm("Lava");
                break;
            case 31:
                TutorialText.GetComponent<Localize>().SetTerm("TimeBomb_Ice");
                break;
            case 32:
                TutorialText.GetComponent<Localize>().SetTerm("Potion");
                break;
            case 33:
                TutorialText.GetComponent<Localize>().SetTerm("Gear");
                break;
            case 34:
                TutorialText.GetComponent<Localize>().SetTerm("Gear");
                break;
            case 35:
                break;
            case 36:
                break;
            case 37:
                break;
            case 38:
                break;
            case 39:
                break;

            #endregion ----- case 30 ~ 39 -----

            default:
                break;
        }
    }

    public void StartCorutine()
    {
        foreach (var nCnt in Lightningobj)
        {
            /*Debug.Log(11);
            nCnt.SetActive(true);
            nCnt.GetComponent<LightningAnimScript>().SetStartPosition = new Vector2(0, 70);

            if(nCnt == Lightningobj[0])
            {
                
                nCnt.GetComponent<LightningAnimScript>().SetEndPosition_Tutorial = new Vector2(0, 140);
            }
            else if(nCnt == Lightningobj[1])
            {
                nCnt.GetComponent<LightningAnimScript>().SetEndPosition_Tutorial = new Vector2(0, 0);
            }
            else if(nCnt == Lightningobj[2])
            {
                nCnt.GetComponent<LightningAnimScript>().SetEndPosition_Tutorial = new Vector2(70, -140);
            }*/
            nCnt.SetActive(true);
            nCnt.GetComponent<AnimationLightning>().StartLightningAnimation();
        }
    }

    public void RainbowAnimControll()
    {
        RainbowParticle.GetComponent<Animator>().SetTrigger("End");
    }

    public void TimeBombUpdate()
    {
        TimeBombText.text = "" + (TimeBombTurn - 1);
    }

    public void TimeBombStart()
    {
        TimeBombText.text = "" + TimeBombTurn;
    }
}