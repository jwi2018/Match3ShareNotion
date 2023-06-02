using UnityEngine;

public class AnimationManager : Singleton<AnimationManager>
{
    //public bool EndCombineAnim { get; set; }
    public static int AnimCount;

    public static int RainbowCount;
    public static int AutoTargetBombCount;
    public static int FactionCount;

    public float dropStartSpeed = 5f;
    public float dropAcc = 11f;
    public float dropMaxSpeed = 8f;
    public float createBlockTime = 0.1f;
    public float movingRange = 0.1f;
    public float swapSpeed = 0.2f;
    [SerializeField] private GameObject notShakingObj;
    [SerializeField] private GameObject hammerObj;
    [SerializeField] private GameObject crossObj;
    [SerializeField] private GameObject circleObj;
    [SerializeField] private GameObject rainbowObj;

    public void Clear()
    {
        AutoTargetBombCount = 0;
        AnimCount = 0;
        RainbowCount = 0;
        FactionCount = 0;
    }

    public void ShowBomb(EID id, EColor color, GameTile tile)
    {
        var isTuto = StageManager.GetInstance.IsTutorialActive();

        switch (id)
        {
            case EID.HORIZONTAL:
                var rocketObj = DynamicObjectPool.GetInstance.GetObjectForType("RocketH" + (int)color, false);
                if (rocketObj != null)
                {
                    rocketObj.transform.SetParent(notShakingObj.transform);
                    rocketObj.transform.localPosition = tile.transform.position;
                    rocketObj.GetComponent<Animator>().SetTrigger("NotX");
                }

                break;

            case EID.VERTICAL:
                var rocketObj2 = DynamicObjectPool.GetInstance.GetObjectForType("RocketH" + (int)color, false);
                if (rocketObj2 != null)
                {
                    rocketObj2.transform.SetParent(notShakingObj.transform);
                    rocketObj2.transform.localPosition = tile.transform.position;
                    rocketObj2.transform.rotation = Quaternion.Euler(0, 0, 90);
                    rocketObj2.GetComponent<Animator>().SetTrigger("NotX");

                    if (isTuto && color == EColor.RED)
                    {
                        Highlight highlight = rocketObj2.GetComponent<Highlight>();
                        if (null != highlight)
                        {
                            highlight.Init();
                            highlight.SetHighlight(true);
                        }
                    }
                }

                break;

            case EID.X:
                var rocketObj3 = DynamicObjectPool.GetInstance.GetObjectForType("RocketH" + (int)color, false);
                var rocketObj4 = DynamicObjectPool.GetInstance.GetObjectForType("RocketH" + (int)color, false);
                if (rocketObj3 != null)
                {
                    rocketObj3.transform.SetParent(notShakingObj.transform);
                    rocketObj3.transform.localPosition = tile.transform.position;
                    rocketObj3.transform.rotation = Quaternion.Euler(0, 0, 45);
                    rocketObj3.GetComponent<Animator>().SetTrigger("X");
                }

                if (rocketObj4 != null)
                {
                    rocketObj4.transform.SetParent(notShakingObj.transform);
                    rocketObj4.transform.localPosition = tile.transform.position;
                    rocketObj4.transform.rotation = Quaternion.Euler(0, 0, 135);
                    rocketObj4.GetComponent<Animator>().SetTrigger("X");
                }

                break;

            case EID.RHOMBUS:
                ParticleManager.GetInstance.ShowParticle(EID.RHOMBUS, color, 0, tile.transform.position);
                break;
        }
    }

    public void ShowBomb(ECombine combine, EColor color, GameTile tile)
    {
        var isTuto = StageManager.GetInstance.IsTutorialActive();
        EColor colorValue;

        if (DoubleClickSystem.GetInstance != null)
        {
            colorValue = EColor.NONE;
        }
        else
        {
            if (color == EColor.NONE) colorValue = EColor.ORANGE;
            else colorValue = color;
        }

        switch (combine)
        {
            case ECombine.CROSS:
                var rocketObj1 = DynamicObjectPool.GetInstance.GetObjectForType("RocketH" + (int)colorValue, false);
                var rocketObj2 = DynamicObjectPool.GetInstance.GetObjectForType("RocketH" + (int)colorValue, false);
                if (rocketObj1 != null)
                {
                    rocketObj1.transform.SetParent(notShakingObj.transform);
                    rocketObj1.transform.localPosition = tile.transform.position;
                    rocketObj1.transform.rotation = Quaternion.Euler(0, 0, 90);
                    rocketObj1.GetComponent<Animator>().SetTrigger("NotX");
                }

                if (rocketObj2 != null)
                {
                    rocketObj2.transform.SetParent(notShakingObj.transform);
                    rocketObj2.transform.localPosition = tile.transform.position;
                    rocketObj2.transform.rotation = Quaternion.Euler(0, 0, 0);
                    rocketObj2.GetComponent<Animator>().SetTrigger("NotX");
                }

                break;

            case ECombine.ASTERISK:
                var rocketObj3 = DynamicObjectPool.GetInstance.GetObjectForType("RocketH" + (int)colorValue, false);
                var rocketObj4 = DynamicObjectPool.GetInstance.GetObjectForType("RocketH" + (int)colorValue, false);
                var rocketObj5 = DynamicObjectPool.GetInstance.GetObjectForType("RocketH" + (int)colorValue, false);
                var rocketObj6 = DynamicObjectPool.GetInstance.GetObjectForType("RocketH" + (int)colorValue, false);
                if (rocketObj3 != null)
                {
                    rocketObj3.transform.SetParent(notShakingObj.transform);
                    rocketObj3.transform.localPosition = tile.transform.position;
                    rocketObj3.transform.rotation = Quaternion.Euler(0, 0, 90);
                    rocketObj3.GetComponent<Animator>().SetTrigger("NotX");
                }

                if (rocketObj4 != null)
                {
                    rocketObj4.transform.SetParent(notShakingObj.transform);
                    rocketObj4.transform.localPosition = tile.transform.position;
                    rocketObj4.transform.rotation = Quaternion.Euler(0, 0, 0);
                    rocketObj4.GetComponent<Animator>().SetTrigger("NotX");
                }

                if (rocketObj5 != null)
                {
                    rocketObj5.transform.SetParent(notShakingObj.transform);
                    rocketObj5.transform.localPosition = tile.transform.position;
                    rocketObj5.transform.rotation = Quaternion.Euler(0, 0, 45);
                    rocketObj5.GetComponent<Animator>().SetTrigger("X");
                }

                if (rocketObj6 != null)
                {
                    rocketObj6.transform.SetParent(notShakingObj.transform);
                    rocketObj6.transform.localPosition = tile.transform.position;
                    rocketObj6.transform.rotation = Quaternion.Euler(0, 0, 135);
                    rocketObj6.GetComponent<Animator>().SetTrigger("X");
                }

                break;

            case ECombine.BIG_CROSS:
                var rocketObj7 = DynamicObjectPool.GetInstance.GetObjectForType("RocketH" + (int)colorValue, false);
                var rocketObj8 = DynamicObjectPool.GetInstance.GetObjectForType("RocketH" + (int)colorValue, false);
                var rocketObj9 = DynamicObjectPool.GetInstance.GetObjectForType("RocketH" + (int)colorValue, false);
                var rocketObj10 = DynamicObjectPool.GetInstance.GetObjectForType("RocketH" + (int)colorValue, false);
                var rocketObj11 = DynamicObjectPool.GetInstance.GetObjectForType("RocketH" + (int)colorValue, false);
                var rocketObj12 = DynamicObjectPool.GetInstance.GetObjectForType("RocketH" + (int)colorValue, false);
                if (rocketObj7 != null)
                {
                    rocketObj7.transform.SetParent(notShakingObj.transform);
                    rocketObj7.transform.localPosition = tile.transform.position;
                    rocketObj7.transform.rotation = Quaternion.Euler(0, 0, 90);
                    rocketObj7.GetComponent<Animator>().SetTrigger("NotX");
                }

                if (rocketObj8 != null)
                {
                    rocketObj8.transform.SetParent(notShakingObj.transform);
                    rocketObj8.transform.localPosition = tile.transform.position;
                    rocketObj8.transform.rotation = Quaternion.Euler(0, 0, 0);
                    rocketObj8.GetComponent<Animator>().SetTrigger("NotX");
                }

                if (rocketObj9 != null)
                {
                    rocketObj9.transform.SetParent(notShakingObj.transform);
                    rocketObj9.transform.localPosition = new Vector3(tile.transform.position.x,
                        tile.transform.position.y + TileManager.GetInstance.TileSize.y, 0);
                    rocketObj9.transform.rotation = Quaternion.Euler(0, 0, 0);
                    rocketObj9.GetComponent<Animator>().SetTrigger("NotX");
                }

                if (rocketObj10 != null)
                {
                    rocketObj10.transform.SetParent(notShakingObj.transform);
                    rocketObj10.transform.localPosition = new Vector3(tile.transform.position.x,
                        tile.transform.position.y - TileManager.GetInstance.TileSize.y, 0);
                    rocketObj10.transform.rotation = Quaternion.Euler(0, 0, 0);
                    rocketObj10.GetComponent<Animator>().SetTrigger("NotX");
                }

                if (rocketObj11 != null)
                {
                    rocketObj11.transform.SetParent(notShakingObj.transform);
                    rocketObj11.transform.localPosition = new Vector3(
                        TileManager.GetInstance.TileSize.x + tile.transform.position.x, tile.transform.position.y, 0);
                    rocketObj11.transform.rotation = Quaternion.Euler(0, 0, 90);
                    rocketObj11.GetComponent<Animator>().SetTrigger("NotX");
                }

                if (rocketObj12 != null)
                {
                    rocketObj12.transform.SetParent(notShakingObj.transform);
                    rocketObj12.transform.localPosition = new Vector3(
                        -TileManager.GetInstance.TileSize.x + tile.transform.position.x, tile.transform.position.y, 0);
                    rocketObj12.transform.rotation = Quaternion.Euler(0, 0, 90);
                    rocketObj12.GetComponent<Animator>().SetTrigger("NotX");
                }

                break;

            case ECombine.BIG_X:
                var rocketObj13 = DynamicObjectPool.GetInstance.GetObjectForType("RocketH" + (int)colorValue, false);
                var rocketObj14 = DynamicObjectPool.GetInstance.GetObjectForType("RocketH" + (int)colorValue, false);
                var rocketObj15 = DynamicObjectPool.GetInstance.GetObjectForType("RocketH" + (int)colorValue, false);
                var rocketObj16 = DynamicObjectPool.GetInstance.GetObjectForType("RocketH" + (int)colorValue, false);
                var rocketObj17 = DynamicObjectPool.GetInstance.GetObjectForType("RocketH" + (int)colorValue, false);
                var rocketObj18 = DynamicObjectPool.GetInstance.GetObjectForType("RocketH" + (int)colorValue, false);
                var rocketObj19 = DynamicObjectPool.GetInstance.GetObjectForType("RocketH" + (int)colorValue, false);
                var rocketObj20 = DynamicObjectPool.GetInstance.GetObjectForType("RocketH" + (int)colorValue, false);
                var rocketObj21 = DynamicObjectPool.GetInstance.GetObjectForType("RocketH" + (int)colorValue, false);
                var rocketObj22 = DynamicObjectPool.GetInstance.GetObjectForType("RocketH" + (int)colorValue, false);
                if (rocketObj13 != null)
                {
                    rocketObj13.transform.SetParent(notShakingObj.transform);
                    rocketObj13.transform.localPosition = tile.transform.position;
                    rocketObj13.transform.rotation = Quaternion.Euler(0, 0, 45);
                    rocketObj13.GetComponent<Animator>().SetTrigger("X");
                }

                if (rocketObj14 != null)
                {
                    rocketObj14.transform.SetParent(notShakingObj.transform);
                    rocketObj14.transform.localPosition = tile.transform.position;
                    rocketObj14.transform.rotation = Quaternion.Euler(0, 0, 135);
                    rocketObj14.GetComponent<Animator>().SetTrigger("X");
                }

                if (rocketObj15 != null)
                {
                    rocketObj15.transform.SetParent(notShakingObj.transform);
                    rocketObj15.transform.localPosition = new Vector3(tile.transform.position.x,
                        tile.transform.position.y + TileManager.GetInstance.TileSize.y, 0);
                    rocketObj15.transform.rotation = Quaternion.Euler(0, 0, 45);
                    rocketObj15.GetComponent<Animator>().SetTrigger("X");
                }

                if (rocketObj16 != null)
                {
                    rocketObj16.transform.SetParent(notShakingObj.transform);
                    rocketObj16.transform.localPosition = new Vector3(tile.transform.position.x,
                        tile.transform.position.y + TileManager.GetInstance.TileSize.y, 0);
                    rocketObj16.transform.rotation = Quaternion.Euler(0, 0, 135);
                    rocketObj16.GetComponent<Animator>().SetTrigger("X");
                }

                if (rocketObj17 != null)
                {
                    rocketObj17.transform.SetParent(notShakingObj.transform);
                    rocketObj17.transform.localPosition = new Vector3(tile.transform.position.x,
                        tile.transform.position.y + -TileManager.GetInstance.TileSize.y, 0);
                    rocketObj17.transform.rotation = Quaternion.Euler(0, 0, 45);
                    rocketObj17.GetComponent<Animator>().SetTrigger("X");
                }

                if (rocketObj18 != null)
                {
                    rocketObj18.transform.SetParent(notShakingObj.transform);
                    rocketObj18.transform.localPosition = new Vector3(tile.transform.position.x,
                        tile.transform.position.y - TileManager.GetInstance.TileSize.y, 0);
                    rocketObj18.transform.rotation = Quaternion.Euler(0, 0, 135);
                    rocketObj18.GetComponent<Animator>().SetTrigger("X");
                }

                if (rocketObj19 != null)
                {
                    rocketObj19.transform.SetParent(notShakingObj.transform);
                    rocketObj19.transform.localPosition = new Vector3(
                        TileManager.GetInstance.TileSize.x + tile.transform.position.x, tile.transform.position.y, 0);
                    rocketObj19.transform.rotation = Quaternion.Euler(0, 0, 45);
                    rocketObj19.GetComponent<Animator>().SetTrigger("X");
                }

                if (rocketObj20 != null)
                {
                    rocketObj20.transform.SetParent(notShakingObj.transform);
                    rocketObj20.transform.localPosition = new Vector3(
                        TileManager.GetInstance.TileSize.x + tile.transform.position.x, tile.transform.position.y, 0);
                    rocketObj20.transform.rotation = Quaternion.Euler(0, 0, 135);
                    rocketObj20.GetComponent<Animator>().SetTrigger("X");
                }

                if (rocketObj21 != null)
                {
                    rocketObj21.transform.SetParent(notShakingObj.transform);
                    rocketObj21.transform.localPosition = new Vector3(
                        -TileManager.GetInstance.TileSize.x + tile.transform.position.x, tile.transform.position.y, 0);
                    rocketObj21.transform.rotation = Quaternion.Euler(0, 0, 45);
                    rocketObj21.GetComponent<Animator>().SetTrigger("X");
                }

                if (rocketObj22 != null)
                {
                    rocketObj22.transform.SetParent(notShakingObj.transform);
                    rocketObj22.transform.localPosition = new Vector3(
                        -TileManager.GetInstance.TileSize.x + tile.transform.position.x, tile.transform.position.y, 0);
                    rocketObj22.transform.rotation = Quaternion.Euler(0, 0, 135);
                    rocketObj22.GetComponent<Animator>().SetTrigger("X");
                }

                break;

            case ECombine.X_X:
                var rocketObj23 = DynamicObjectPool.GetInstance.GetObjectForType("RocketH" + (int)colorValue, false);
                var rocketObj24 = DynamicObjectPool.GetInstance.GetObjectForType("RocketH" + (int)colorValue, false);
                var rocketObj25 = DynamicObjectPool.GetInstance.GetObjectForType("RocketH" + (int)colorValue, false);
                var rocketObj26 = DynamicObjectPool.GetInstance.GetObjectForType("RocketH" + (int)colorValue, false);
                if (rocketObj23 != null)
                {
                    rocketObj23.transform.SetParent(notShakingObj.transform);
                    rocketObj23.transform.localPosition = tile.transform.position;
                    rocketObj23.transform.rotation = Quaternion.Euler(0, 0, 90);
                    rocketObj23.GetComponent<Animator>().SetTrigger("NotX");
                }

                if (rocketObj24 != null)
                {
                    rocketObj24.transform.SetParent(notShakingObj.transform);
                    rocketObj24.transform.localPosition = tile.transform.position;
                    rocketObj24.transform.rotation = Quaternion.Euler(0, 0, 0);
                    rocketObj24.GetComponent<Animator>().SetTrigger("NotX");
                }

                if (rocketObj25 != null)
                {
                    rocketObj25.transform.SetParent(notShakingObj.transform);
                    rocketObj25.transform.localPosition = tile.transform.position;
                    rocketObj25.transform.rotation = Quaternion.Euler(0, 0, 45);
                    rocketObj25.GetComponent<Animator>().SetTrigger("X");
                }

                if (rocketObj26 != null)
                {
                    rocketObj26.transform.SetParent(notShakingObj.transform);
                    rocketObj26.transform.localPosition = tile.transform.position;
                    rocketObj26.transform.rotation = Quaternion.Euler(0, 0, 135);
                    rocketObj26.GetComponent<Animator>().SetTrigger("X");
                }

                break;

            case ECombine.BIG_RHOMBUS:
                AnimCount++;
                var bigRhombus = DynamicObjectPool.GetInstance.GetObjectForType("Bomb_Bomb", false);
                if (bigRhombus != null)
                {
                    bigRhombus.transform.SetParent(notShakingObj.transform);
                    bigRhombus.transform.localPosition = tile.transform.position;
                }

                if (SoundManager.GetInstance != null)
                    SoundManager.GetInstance.Play("BombDoubleCircle");
                break;

            case ECombine.RAINBOW_RAINBOW:
                AnimCount++;
                var doubleRainbow = DynamicObjectPool.GetInstance.GetObjectForType("DoubleRainbowActive", false);
                if (doubleRainbow != null)
                {
                    doubleRainbow.transform.SetParent(notShakingObj.transform);
                    var centerPosition = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y,
                        0);
                    doubleRainbow.transform.localPosition =
                        centerPosition; // 화면의 가운데서 터트릴때
                    //tile.transform.position; // 매치시킨 곳에서 터트릴때
                    //BlockManager.GetInstance.SwapPosition; // 기존방식(문제발생)
                    var doubleRainbowAnimator = doubleRainbow.GetComponent<Animator>();
                    if (doubleRainbowAnimator != null)
                    {
                        if (BlockManager.GetInstance.IsSwapHorizontal)
                            doubleRainbowAnimator.SetTrigger("Horizontal");
                        else
                            doubleRainbowAnimator.SetTrigger("Vertical");
                    }
                }

                break;
        }
    }

    public void ShowDoubleRhombus(Vector3 position)
    {
        var particle = DynamicObjectPool.GetInstance.GetObjectForType("BigBombParticle", false);
        if (particle != null) particle.transform.position = position;
    }

    public void ShowDoubleRainbowPop(Vector3 position)
    {
        var particle = DynamicObjectPool.GetInstance.GetObjectForType("Particle_DoubleRainbow", false);
        if (particle != null) particle.transform.position = position;
    }

    public void ShowHammer(GameBlock block)
    {
        //hammerObj.transform.SetParent(block.Tile.gameObject.transform);
        hammerObj.transform.position = block.Tile.GetPosition();
        hammerObj.SetActive(true);
    }

    public void ShowCross(GameBlock block)
    {
        crossObj.transform.position = block.Tile.GetPosition();
        crossObj.SetActive(true);
    }

    public void ShowCircle(GameBlock block)
    {
        circleObj.transform.position = block.Tile.GetPosition();
        circleObj.SetActive(true);
    }

    public void ShowRainbow(GameBlock block)
    {
        rainbowObj.transform.position = block.Tile.GetPosition();
        rainbowObj.SetActive(true);
    }
}