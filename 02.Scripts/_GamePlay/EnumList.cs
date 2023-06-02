using System;

[Serializable]
public enum EDirection
{
    UP = 0,
    LEFT = 1,
    RIGHT = 2,
    DOWN = 3,
    UP_LEFT = 4,
    UP_RIGHT = 5,
    DOWN_LEFT = 6,
    DOWN_RIGHT = 7,
    NONE
}

[Serializable]
public enum EOtherGames
{
    NONE = 0,
    FANTASY,
    TOY,
    FRUITS,
    CANDY,
    GARDENBLAST,
    JEWELSKING,
    JEWELCITY,
    TOYSPACE,
    SWEETCOOKIE,
    TEMPLE,
    LAST,
    FARM,
    CATS,
    WITCHCASTLE,
    FRUITSGARDEN,
    ADVENTURE,
    CIRCUS,
    MYSTERY,
    JEWELCITY2
}

public enum ESetupKinds
{
    SETTING,
    PROMOTION,
    GPGS
}

public enum EColor
{
    RED = 0,
    YELLOW,
    GREEN,
    BLUE,
    PURPLE,
    ORANGE,
    NONE
}

public enum EDepth
{
    TABLET = 1,
    FLOOR = 2,
    OVER_RAIL = 3,
    NORMAL = 4,
    TOP = 8,
    RELIC = 9,
    BARRICADE = 10,
    SPECIAL = 11
}

public enum EOneWay
{
    NONE = 0,
    RIGHT_TO_LEFT = 1,
    LEFT_TO_RIGHT = 2,
    DOWN_TO_UP = 3,
    UP_TO_DOWN = 4,
    RIGHT_TO_DOWN = 5,
    DOWN_TO_LEFT = 6,
    DOWN_TO_RIGHT = 7,
    LEFT_TO_DOWN = 8,
    UP_TO_RIGHT = 9,
    LEFT_TO_UP = 10,
    RIGHT_TO_UP = 11,
    UP_TO_LEFT = 12
}

public enum EID
{
    NONE = -1,
    NORMAL = 0,
    HORIZONTAL = 1,
    VERTICAL = 2,
    RHOMBUS = 3,
    X = 4,
    COLOR_BOMB = 5,
    FISH = 6,
    BLANK = 7,
    BOX = 8,
    OAK = 9,
    GOLD = 10,
    JAM = 11,
    ICE = 12,
    BOX_COLOR = 13,
    GLASS_COLOR = 14,
    SPECIAL_TILE = 15,
    BIG_SIDE = 16,
    BIG_SIDE_COLOR = 17,
    BIG_SIDE_DIRECTION = 18,
    LAVA = 19,
    JAIL = 20,
    TIMEBOMB_ICE = 21,
    TIMEBOMB_LAVA = 22,
    BANDAGE = 23,
    GIFTBOX = 24,
    DROP_RELIC1 = 25,
    DROP_RELIC2 = 26,
    TARGET_RELIC = 27,
    TABLET_FLOOR = 32,
    BARRICADE = 33,
    TUNNEL_ENTRANCE = 34,
    TUNNEL_EXIT = 35,
    CHAMELEON = 36,
    RAIL = 37,
    SHIELD = 38,
    RELIC_IN_INVISIBLE_BOX = 39,
    INVISIBLE_BOX = 40,
    GEAR = 41,
    GEAR_CORE = 42,
    TABLET = 43,
    DROP_RELIC3 = 44,
    ACTINIARIA = 45,
    CLAM = 46,
    RAIL_COVER = 47,
    METAL_BOX = 48,
    TURN_BOX = 49,
    FIZZ = 50,
    DOUBLE = 51,
    CONNECTION = 52,
    CREATOR_LAVA = 53,
    CREATOR_BOMB = 54,
    METAL_OAK = 55,
    FACTION = 56,
    COIN_BOX = 57,
    BOOSTER_ASTERISK = 58,
    BOOSTER_GREATBOMB = 59,
    CLIMBER_NEST = 60,
    CLIMBER = 61,
}

public enum ETileKind
{
    NORMAL = 0,
    VOID,
    START,
    END,
    LADDER,
    NORMAL_EVEN,
    NORMAL1
}

public enum EItem
{
    HORIZONTAL_BOMB = 1,
    VERTICAL_BOMB = 4,
    RHOMBUS_BOMB = 16,
    X_BOMB = 64,
    COLOR_BOMB = 256,
    FISH = 1024,
    BOOSTER_ASTERISK = 4096,
    BOOSTER_GREATBOMB = 16384,
    NONE = 65536
}

public enum ECombine
{
    NONE,
    CROSS,
    ASTERISK,
    BIG_CROSS,
    BIG_X,
    BIG_RHOMBUS,
    X_X,
    RAINBOW_NORMAL,
    RAINBOW_DIRECTION,
    RAINBOW_RHOMBUS,
    RAINBOW_X,
    RAINBOW_RAINBOW,
    FISH_HORIZONTAL,
    FISH_VERTICAL,
    FISH_RHOMBUS,
    FISH_FISH,
    FISH_X,
    RAINBOW_FISH,
    ACTIVE_ASTERISK,
    ACTIVE_GREATBOMB
}

public enum EBlockKind
{
    NONE,
    NORMAL,
    BOMB,
    OBSTACLE
}

public enum EUseItem
{
    NONE = 0,
    HAMMER,
    CROSS,
    BOMB,
    COLOR
}

public enum EShopKind
{
    PACKAGE,
    COIN,
    GOLD,
}

public enum EBannerKind
{
    BANNER
}

public enum EInterstitialKind
{
    // 2022 - 01 - 18 전완익.
    INTERSTITIAL
}

public enum ERewardedKind
{
    // 2022 - 01 - 18 전완익.
    //WORLDMAP
    REWARD
}

public enum EUnifiedNativeKind
{
    Native,
    MissionClear,
    MissionFail,
    GameExit
}

public enum EDailyQuestType
{
    GAMEPLAY,
    GETSTAR,
    USEHAMMER,
    USECOLOR,
    USEBOMB,
    USECOIN,
    OPENTREASURE,
    USEROULETTE,
    USERMOVEADREWARD,
    WATCHAD,
    GAMECLEAR,
    DAILYQUESTCLEAR,
    DAILYQUESTCLEARAD,
    GETITEMFROMLOBBY,
    GAMEPLAY_NOTS,
    GETSTAR_NOTS,
    USEHAMMER_NOTS,
    USECOLOR_NOTS,
    USEBOMB_NOTS,
    USECOIN_NOTS,
    OPENTREASURE_NOTS,
    USEROULETTE_NOTS,
    USERMOVEADREWARD_NOTS,
    WATCHAD_NOTS,
    GAMECLEAR_NOTS
}

public enum EDailyQuestRewardType
{
    COIN,
    HAMMER,
    BOMB,
    COLOR,
    HAMMERBOMB,
    HAMMERCOLOR,
    BOMBCOLOR,
    ALLITEM,
    ACORN
}

public enum EChallengeRewardType
{
    Acorn,
    Coin,
    Hammer,
    Bomb,
    Rainbow
}

public enum ESystemType
{
    AcornSystem,
    ChallengeSystem
}

public enum EContentsTextType
{
    Acorn = 0,
    TreasureBox = 1,
}

public enum EPackageType
{
    Champion,
    Massive,
    Giant,
    Mystery,
    Mega,
    Super,
    Beginner,
    Subs,
}

public enum EGoldType
{
    E255000,
    E142000,
    E61000,
    E27500,
    E11100,
    E2000
}

public enum ESubsType
{
    None,
    AOSWeekVip,
    AOSMonthVip,
    IOSWeekVip,
    IOSMonthVip
}

public enum EditorMode
{
    GhostMode,
    NormalMode
}