using UnityEngine;

public class AnimationListener : MonoBehaviour
{
    public void Play(int num)
    {
        if (SoundManager.GetInstance != null)
            switch (num)
            {
                #region ----- case 0 ~ 9 -----

                case 0:
                    SoundManager.GetInstance.Play("DailyBonus");
                    break;
                case 1:
                    SoundManager.GetInstance.Play("GetCoin");
                    break;
                case 2:
                    SoundManager.GetInstance.Play("StarBoxComing");
                    break;
                case 3:
                    SoundManager.GetInstance.Play("StarBoxClose");
                    break;
                case 4:
                    SoundManager.GetInstance.Play("StarBoxGetItem");
                    break;
                case 5:
                    SoundManager.GetInstance.Play("Popup");
                    break;
                case 6:
                    SoundManager.GetInstance.Play("PopupShop");
                    break;
                case 7:
                    SoundManager.GetInstance.Play("ButtonPush");
                    break;
                case 8:
                    SoundManager.GetInstance.Play("MissionPopupOpen");
                    break;
                case 9:
                    SoundManager.GetInstance.Play("MissionPopupClose");
                    break;

                #endregion ------ case 0 ~ 9 -----

                #region ----- case 10 ~ 19 -----

                case 10:
                    SoundManager.GetInstance.Play("BlockTouch");
                    break;
                case 11:
                    SoundManager.GetInstance.Play("BlockSwap");
                    break;
                case 12:
                    SoundManager.GetInstance.Play("BlockDrop");
                    break;
                case 13:
                    SoundManager.GetInstance.Play("BlockDestroy");
                    break;
                case 14:
                    SoundManager.GetInstance.Play("BlockCreateItem");
                    break;
                case 15:
                    SoundManager.GetInstance.Play("BombDirect");
                    break;
                case 16:
                    SoundManager.GetInstance.Play("BombX");
                    break;
                case 17:
                    SoundManager.GetInstance.Play("BombCircle");
                    break;
                case 18:
                    SoundManager.GetInstance.Play("BombCross");
                    break;
                case 19:
                    SoundManager.GetInstance.Play("BombBigCross");
                    break;

                #endregion ----- case 10 ~ 19 -----

                #region ----- case 20 ~ 29 -----

                case 20:
                    SoundManager.GetInstance.Play("BombBigX");
                    break;
                case 21:
                    SoundManager.GetInstance.Play("BombDoubleCircle");
                    break;
                case 22:
                    SoundManager.GetInstance.Play("BombRainbow");
                    break;
                case 23:
                    SoundManager.GetInstance.Play("BombDoubleRainbow");
                    break;
                case 24:
                    SoundManager.GetInstance.Play(SoundManager.GetInstance.ComboMatch);
                    break;
                case 25:
                    SoundManager.GetInstance.Play("ComboMatchMessage");
                    break;
                case 26:
                    SoundManager.GetInstance.Play(SoundManager.GetInstance.Hint);
                    break;
                case 27:
                    SoundManager.GetInstance.Play("GetStarInPlay");
                    break;
                case 28:
                    SoundManager.GetInstance.Play("UseHammer");
                    break;
                case 29:
                    SoundManager.GetInstance.Play("UseShuffle");
                    break;

                #endregion ----- case 20 ~ 29 -----

                #region ----- case 30 ~ 39 -----

                case 30:
                    SoundManager.GetInstance.Play("ClearFirework");
                    break;
                case 31:
                    SoundManager.GetInstance.Play("ClearBravo");
                    break;
                case 32:
                    SoundManager.GetInstance.Play("ClearStar");
                    break;
                case 33:
                    SoundManager.GetInstance.Play("ClearPopup");
                    break;
                case 34:
                    SoundManager.GetInstance.Play("FailPopup");
                    break;
                case 35:
                    SoundManager.GetInstance.Play(SoundManager.GetInstance.ShowFiveCount);
                    break;
                case 36:
                    SoundManager.GetInstance.Play(SoundManager.GetInstance.GetFiveCount);
                    break;
                case 37:
                    SoundManager.GetInstance.Play("CollectRelic");
                    break;
                case 38:
                    SoundManager.GetInstance.Play(SoundManager.GetInstance.CheckMissionClear);
                    break;
                case 39:
                    SoundManager.GetInstance.Play("Oak");
                    break;

                #endregion ----- case 30 ~ 39 -----

                #region ----- case 40 ~ 49 -----

                case 40:
                    SoundManager.GetInstance.Play("Box");
                    break;
                case 41:
                    SoundManager.GetInstance.Play("Sand");
                    break;
                case 42:
                    SoundManager.GetInstance.Play(SoundManager.GetInstance.Floor);
                    break;
                case 43:
                    SoundManager.GetInstance.Play("StoneFloor");
                    break;
                case 44:
                    SoundManager.GetInstance.Play(SoundManager.GetInstance.ColorCup);
                    break;
                case 45:
                    SoundManager.GetInstance.Play("Bubble");
                    break;
                case 46:
                    SoundManager.GetInstance.Play("Prison");
                    break;
                case 47:
                    SoundManager.GetInstance.Play("Ice");
                    break;
                case 48:
                    SoundManager.GetInstance.Play(SoundManager.GetInstance.Lamp);
                    break;
                case 49:
                    SoundManager.GetInstance.Play(SoundManager.GetInstance.Nelumbo);
                    break;

                #endregion ----- case 40 ~ 49 -----

                #region ----- case 50 ~ 59 -----

                case 50:
                    SoundManager.GetInstance.Play("GiftBox");
                    break;
                case 51:
                    SoundManager.GetInstance.Play(SoundManager.GetInstance.Band);
                    break;
                case 52:
                    SoundManager.GetInstance.Play(SoundManager.GetInstance.BigGoldBox);
                    break;
                case 53:
                    SoundManager.GetInstance.Play("ChangeChameleon");
                    break;
                case 54:
                    SoundManager.GetInstance.Play("ColorBox");
                    break;
                case 55:
                    SoundManager.GetInstance.Play("Lava");
                    break;
                case 56:
                    SoundManager.GetInstance.Play("TimeBombNoTime");
                    break;
                case 57:
                    SoundManager.GetInstance.Play("TimeBomb");
                    break;
                case 58:
                    SoundManager.GetInstance.Play(SoundManager.GetInstance.SuperBomb);
                    break;
                case 59:
                    SoundManager.GetInstance.Play(SoundManager.GetInstance.GoldMine);
                    break;

                #endregion ----- case 50 ~ 59 -----

                #region ----- case 60 ~ 69 -----

                case 60:
                    SoundManager.GetInstance.Play(SoundManager.GetInstance.Pick);
                    break;
                case 61:
                    SoundManager.GetInstance.Play(SoundManager.GetInstance.BigEntrails);
                    break;
                case 62:
                    SoundManager.GetInstance.Play(SoundManager.GetInstance.Gear);
                    break;
                case 63:
                    SoundManager.GetInstance.Play("RelicDrop");
                    break;
                case 64:
                    SoundManager.GetInstance.Play("RouletteSpin");
                    break;
                case 65:
                    SoundManager.GetInstance.Play("RouletteItem");
                    break;
                case 66:
                    SoundManager.GetInstance.Play("IntroSFX");
                    break;
                case 67:
                    SoundManager.GetInstance.Play("Lemon");
                    break;
                case 68:
                    SoundManager.GetInstance.Play("GiftBoxOpen");
                    break;
                case 69:
                    break;

                #endregion ----- case 60 ~ 69 -----

                case 70:
                    SoundManager.GetInstance.Play(SoundManager.GetInstance.Bravo);
                    break;

                case 71:
                    SoundManager.GetInstance.Play("SeasonPass_GetItem");
                    break;

                case 72:
                    SoundManager.GetInstance.Play("SeasonPass_NewItemOpen");
                    break;

                case 73:
                    SoundManager.GetInstance.Play("GetStarSound_1");
                    break;

                case 74:
                    SoundManager.GetInstance.Play("GetStarSound_2");
                    break;

                case 75:
                    SoundManager.GetInstance.Play("GetStarSound_3");
                    break;

                default:
                    break;
            }
    }

}