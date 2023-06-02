using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CompleteProject;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Serialization;

[SerializeField]
public class SaveDataClass
{
    public int _clearCount;
    public int _dailyDay;
    public int _dailyMonth;
    public int _dailyYear;
    public int _faceBookDay;
    public int _faceBookMonth;
    public int _faceBookYear;
    public int _gold;
    public int _isAdsFree;
    public int _isAdsRoulette;
    public int _isBuyLimitedPackage;
    public int _isCondtion;
    public int _isDailyRoulette;
    public int _isGpgsLogin;
    public int _isPlayBGM;
    public int _isPlaySFX;
    public int _isPlayVib;
    public int _isPush;
    public int _isRateUs;
    public int _isSeeLimitedPackage;
    public int _isTermsOfUse_EU;
    public int _isTermsOfUse_KR;
    public int _itemBomb;
    public int _itemColor;
    public int _itemCross;
    public int _itemHammer;
    public int _language;
    public int[] _levelStarCount;
    public int _limitedShopHour;
    public int _limitedShopDay;
    public int _limitedShopMonth;
    public int _limitedShopYear;
    public int _moreGameDay;
    public int _moreGameMonth;
    public int _moreGameYear;
    public int _notificationDay;
    public int _notificationHour;
    public int _notificationMonth;
    public int _notificationYear;
    public int _presentLevel;
    public int _rateUsDay;
    public int _rateUsMonth;
    public int _rateUsYear;
    public int _RouletteDay;
    public int _RouletteHour;
    public int _RouletteMinute;
    public int _RouletteMonth;
    public int _RouletteSecond;
    public int _RouletteYear;
    public int _savingCoin;
    public int _starBoxOpenCount;
    public int _weeklyXDay;
    public int _worldMapDay;
    public int _worldMapHour;
    public int _worldMapMinute;
    public int _worldMapMonth;
    public int _worldMapSecond;
    public int _worldMapYear;
    public int _specialRewardYear;
    public int _specialRewardMonth;
    public int _specialRewardDay;
    public int _specialRewardHour;
    public int _specialRewardMinute;
    public int _specialRewardSecond;
    public int _youtubeYear;
    public int _youtubeMonth;
    public int _youtubeDay;
    public string _freeGoldDate;
    public int _freeGoldCount;
    public int _freeGoldCooltime;

    public string _dailyMonthRewardDate;
    public int _dailyMonthRewardContinueDay;
    public int _dailyMonthRewardGetDay;

    public int _acorn;
    public List<int> _challengeStage = new List<int>();
    public List<int> _pickRandomNumList = new List<int>();
    public bool _weeklyOnePickRandomNum;
    public bool _isEventMapAllClear;
    public int _eventLevelNum;
    public int _beforeEventLevelNum;
    public int _beforeAcornGettingLevel;
    public List<int> _gotAcornStage = new List<int>();
    
    public Dictionary<int, bool> _isItemUnlock = new Dictionary<int, bool>();
    public Dictionary<int, bool> _isItemClicked = new Dictionary<int, bool>();

    public Dictionary<DateTime, bool> _pickDayCheck = new Dictionary<DateTime, bool>();
    public string _lastVipGetDay;

    public int _primiumTicketStar;
    public int _primiumTicketLevel;
    public int _primiumTicketReceiveLevel_primium;
    public int _primiumTicketReceiveLevel_free;
    public bool _buyprimiumTicket;

    public DateTime buyLimitedPackageTime;
    public DateTime _primiumTicketRewardDate;
    public int _mainPackageCheck;
    public DateTime mainPackageTime;
    public DateTime mainPackageChangeCheckTime;
}

public class PlayerData : Singleton<PlayerData>
{
    private int _clearCount;
    private int _dailyDay;
    private int _dailyMonth;
    private int _dailyYear;
    private int _faceBookDay;
    private int _faceBookMonth;

    private int _faceBookYear;
    private int _gold;
    private int _isAdsFree;
    private int _isAdsRoulette;
    private int _isBuyLimitedPackage;
    private int _isCondtion;
    private int _isDailyRoulette;
    private int _isGpgsLogin;

    private int _isPlayBGM = 1;
    private int _isPlaySFX = 1;
    private int _isPlayVib = 1;
    private int _isPush = 1;
    private int _isRateUs;
    private int _isSeeLimitedPackage;
    private int _isTermsOfUse_EU;
    private int _isTermsOfUse_KR;
    private int _itemBomb;
    private int _itemColor;
    private int _itemCross;
    private int _itemHammer;
    private int _language;
    private readonly Dictionary<int, int> _levelStarCount = new Dictionary<int, int>();
    private int _limitedShopHour;
    private int _limitedShopDay;
    private int _limitedShopMonth;
    private int _limitedShopYear;
    private bool _islimitedPopupCheck;
    private bool _isWeeklyPopupEnd;

    private int _moreGameDay;
    private int _moreGameMonth;

    private int _moreGameYear;
    private int _notificationDay;
    private int _notificationHour;
    private int _notificationMonth;

    private int _notificationYear;

    private int _presentLevel;
    private int _rateUsDay;
    private int _rateUsMonth;
    private int _rateUsYear;
    private int _RouletteDay;
    private int _RouletteHour;
    private int _RouletteMinute;
    private int _RouletteMonth;
    private int _RouletteSecond;
    private int _RouletteYear;

    private int _savingCoin;
    private int _starBoxOpenCount;
    private int _weeklyXDay;
    private int _worldMapDay;
    private int _worldMapHour;
    private int _worldMapMinute;
    private int _worldMapMonth;
    private int _worldMapSecond;
    private int _worldMapYear;
    
    private int _specialRewardYear = 0;
    private int _specialRewardMonth = 0;
    private int _specialRewardDay = 0;
    private int _specialRewardHour = 0;
    private int _specialRewardMinute = 0;
    private int _specialRewardSecond = 0;

    private int _youtubeYear;
    private int _youtubeMonth;
    private int _youtubeDay;

    private string _dailyQuestDate;
    private string _freeGoldDate;
    private int _freeGoldCount;
    private int _freeGoldCooltime;

    private Dictionary<int, bool> _isItemUnlock = new Dictionary<int, bool>();
    private Dictionary<int, bool> _isItemClicked = new Dictionary<int, bool>();

    private string _dailyMonthRewardDate;
    private int _dailyMonthRewardContinueDay;
    private int _dailyMonthRewardGetDay;
    private int _acorn;

    private List<int> _challengeStage = new List<int>();
    private List<int> _pickRandomNumList = new List<int>();
    private bool _weeklyOnePickRandomNum;
    private bool _isEventMapAllClear;
    private int _eventLevelNum;
    private int _beforeEventLevelNum;
    private int _beforeAcornGettingLevel;
    private List<int> _gotAcornStage = new List<int>();

    private Dictionary<DateTime, bool> _pickDayCheck = new Dictionary<DateTime, bool>();

    private string _lastVipGetDay;

    private DateTime buyLimitedPackageTime;
    #region DoNotNeedSave
    public bool _vipContinue;
    public ESubsType _vipType;
    #endregion

    private int _primiumTicketStar;
    private int _primiumTicketLevel;
    private int _primiumTicketReceiveLevel_primium;
    private int _primiumTicketReceiveLevel_free;
    private bool _buyprimiumTicket;
    private DateTime _primiumTicketRewardDate;

    private int _mainPackageCheck;
    private DateTime mainPackageTime;
    private DateTime mainPackageChangeCheckTime;
    public bool isLimitedPackageCoolTime()
    {
        return !(buyLimitedPackageTime == DateTime.MinValue);
    }

    public DateTime BuyLimitedPackageTime
    {
        get => buyLimitedPackageTime;
        set
        {
            buyLimitedPackageTime = value; 
            SaveGameString("BuyLimitedPackageTime",buyLimitedPackageTime.ToString());
        }
    }

    public DateTime PrimiumTicketRewardDate
    {
        get => _primiumTicketRewardDate;
        set
        {
            _primiumTicketRewardDate = value;
            SaveGameString("PrimiumTicketRewardDate", _primiumTicketRewardDate.ToString());
        }
    }

    public DateTime MainPackageTime
    {
        get => mainPackageTime;
        set
        {
            mainPackageTime = value;
            SaveGameString("MainPackageTime", mainPackageTime.ToString());
        }
    }

    public DateTime MainPackageChangeCheckTime
    {
        get => mainPackageChangeCheckTime;
        set
        {
            mainPackageChangeCheckTime = value;
            SaveGameString("MainPackageChangeCheckTime", mainPackageChangeCheckTime.ToString());
        }
    }

    public string LastVipGetDay
    {
        get => _lastVipGetDay;
        set
        {
            _lastVipGetDay = value;
            SaveGameString("LastVipGetDay", _lastVipGetDay);
        }
    }
    
    public string DailyQuestDate
    {
        get { return _dailyQuestDate; }
        set
        {
            _dailyQuestDate = value;
            SaveGameString("DailyQuestDate", _dailyQuestDate);
        }
    }

    public int PresentLevel
    {
        get => _presentLevel;
        set
        {
            _presentLevel = value;
            SaveData("PresentLevel", _presentLevel);
        }
    }

    public int Gold
    {
        get => _gold;
        set
        {
            DailyQuestManager.CollectMission(EDailyQuestType.USECOIN, _gold - value);
            DailyQuestManager.Save();
            _gold = value;
            SaveData("Gold", _gold);
        }
    }

    public int ItemHammer
    {
        get => _itemHammer;
        set
        {
            DailyQuestManager.CollectMission(EDailyQuestType.USEHAMMER, _itemHammer - value);
            DailyQuestManager.Save();
            _itemHammer = value;
            SaveData("ItemHammer", _itemHammer);
        }
    }

    public int ItemCross
    {
        get => _itemCross;
        set
        {
            _itemCross = value;
            SaveData("ItemCross", _itemCross);
        }
    }

    public int ItemBomb
    {
        get => _itemBomb;
        set
        {
            DailyQuestManager.CollectMission(EDailyQuestType.USEBOMB, _itemBomb - value);
            DailyQuestManager.Save();
            _itemBomb = value;
            SaveData("ItemBomb", _itemBomb);
        }
    }

    public int ItemColor
    {
        get => _itemColor;
        set
        {
            DailyQuestManager.CollectMission(EDailyQuestType.USECOLOR, _itemColor - value);
            DailyQuestManager.Save();
            _itemColor = value;
            SaveData("ItemColor", _itemColor);
        }
    }

    public int DailyYear
    {
        get => _dailyYear;
        set
        {
            _dailyYear = value;
            SaveData("DailyYear", _dailyYear);
        }
    }

    public int DailyMonth
    {
        get => _dailyMonth;
        set
        {
            _dailyMonth = value;
            SaveData("DailyMonth", _dailyMonth);
        }
    }

    public int DailyDay
    {
        get => _dailyDay;
        set
        {
            _dailyDay = value;
            SaveData("DailyDay", _dailyDay);
        }
    }

    public int YoutubeYear
    {
        get { return _youtubeYear; }
        set
        {
            _youtubeYear = value;
            SaveData("YoutubeYear", _youtubeYear);
        }
    }

    public int YoutubeMonth
    {
        get { return _youtubeMonth; }
        set
        {
            _youtubeMonth = value;
            SaveData("YoutubeMonth", _youtubeMonth);
        }
    }

    public int YoutubeDay
    {
        get { return _youtubeDay; }
        set
        {
            _youtubeDay = value;
            SaveData("YoutubeDay", _youtubeDay);
        }
    }

    public int RouletteYear
    {
        get => _RouletteYear;
        set
        {
            _RouletteYear = value;
            SaveData("RouletteYear", _RouletteYear);
        }
    }

    public int RouletteMonth
    {
        get => _RouletteMonth;
        set
        {
            _RouletteMonth = value;
            SaveData("RouletteMonth", _RouletteMonth);
        }
    }

    public int RouletteDay
    {
        get => _RouletteDay;
        set
        {
            _RouletteDay = value;
            SaveData("RouletteDay", _RouletteDay);
        }
    }

    public int RouletteHour
    {
        get => _RouletteHour;
        set
        {
            _RouletteHour = value;
            SaveData("RouletteHour", _RouletteHour);
        }
    }

    public int RouletteMinute
    {
        get => _RouletteMinute;
        set
        {
            _RouletteMinute = value;
            SaveData("RouletteMinute", _RouletteMinute);
        }
    }

    public int RouletteSecond
    {
        get => _RouletteSecond;
        set
        {
            _RouletteSecond = value;
            SaveData("RouletteSecond", _RouletteSecond);
        }
    }

    public int WorldMapYear
    {
        get => _worldMapYear;
        set
        {
            _worldMapYear = value;
            SaveData("WorldMapYear", _worldMapYear);
        }
    }

    public int WorldMapMonth
    {
        get => _worldMapMonth;
        set
        {
            _worldMapMonth = value;
            SaveData("WorldMapMonth", _worldMapMonth);
        }
    }

    public int WorldMapDay
    {
        get => _worldMapDay;
        set
        {
            _worldMapDay = value;
            SaveData("WorldMapDay", _worldMapDay);
        }
    }

    public int WorldMapHour
    {
        get => _worldMapHour;
        set
        {
            _worldMapHour = value;
            SaveData("WorldMapHour", _worldMapHour);
        }
    }

    public int WorldMapMinute
    {
        get => _worldMapMinute;
        set
        {
            _worldMapMinute = value;
            SaveData("WorldMapMinute", _worldMapMinute);
        }
    }

    public int WorldMapSecond
    {
        get => _worldMapSecond;
        set
        {
            _worldMapSecond = value;
            SaveData("WorldMapSecond", _worldMapSecond);
        }
    }
    
    public int SpecialRewardYear
    {
        get { return _specialRewardYear; }
        set
        {
            _specialRewardYear = value;
            SaveData("SpecialRewardYear", _specialRewardYear);
        }
    }
    
    public int SpecialRewardMonth
    {
        get { return _specialRewardMonth; }
        set
        {
            _specialRewardMonth = value;
            SaveData("SpecialRewardMonth", _specialRewardMonth);
        }
    }
    
    public int SpecialRewardDay
    {
        get { return _specialRewardDay; }
        set
        {
            _specialRewardDay = value;
            SaveData("SpecialRewardDay", _specialRewardDay);
        }
    }
    
    public int SpecialRewardHour
    {
        get { return _specialRewardHour; }
        set
        {
            _specialRewardHour = value;
            SaveData("SpecialRewardHour", _specialRewardHour);
        }
    }
    
    public int SpecialRewardMinute
    {
        get { return _specialRewardMinute; }
        set
        {
            _specialRewardMinute = value;
            SaveData("SpecialRewardMinute", _specialRewardMinute);
        }
    }

    public int SpecialRewardSecond
    {
        get { return _specialRewardSecond; }
        set
        {
            _specialRewardSecond = value;
            SaveData("SpecialRewardSecond", _specialRewardSecond);
        }
    }

    public int WeeklyXDay
    {
        get => _weeklyXDay;
        set
        {
            _weeklyXDay = value;
            SaveData("WeeklyXDay", _weeklyXDay);
        }
    }

    public int RateUsYear
    {
        get => _rateUsYear;
        set
        {
            _rateUsYear = value;
            SaveData("RateUsYear", _rateUsYear);
        }
    }

    public int RateUsMonth
    {
        get => _rateUsMonth;
        set
        {
            _rateUsMonth = value;
            SaveData("RateUsMonth", _rateUsMonth);
        }
    }

    public int RateUsDay
    {
        get => _rateUsDay;
        set
        {
            _rateUsDay = value;
            SaveData("RateUsDay", _rateUsDay);
        }
    }

    public int FaceBookYear
    {
        get => _faceBookYear;
        set
        {
            _faceBookYear = value;
            SaveData("FaceBookYear", _faceBookYear);
        }
    }

    public int FaceBookMonth
    {
        get => _faceBookMonth;
        set
        {
            _faceBookMonth = value;
            SaveData("FaceBookMonth", _faceBookMonth);
        }
    }

    public int FaceBookDay
    {
        get => _faceBookDay;
        set
        {
            _faceBookDay = value;
            SaveData("FaceBookDay", _faceBookDay);
        }
    }

    public int MoreGameYear
    {
        get => _moreGameYear;
        set
        {
            _moreGameYear = value;
            SaveData("MoreGameYear", _moreGameYear);
        }
    }

    public int MoreGameMonth
    {
        get => _moreGameMonth;
        set
        {
            _moreGameMonth = value;
            SaveData("MoreGameMonth", _moreGameMonth);
        }
    }

    public int MoreGameDay
    {
        get => _moreGameDay;
        set
        {
            _moreGameDay = value;
            SaveData("MoreGameDay", _moreGameDay);
        }
    }

    public int NotificationYear
    {
        get => _notificationYear;
        set
        {
            _notificationYear = value;
            SaveData("NotificationYear", _notificationYear);
        }
    }

    public int NotificationMonth
    {
        get => _notificationMonth;
        set
        {
            _notificationMonth = value;
            SaveData("NotificationMonth", _notificationMonth);
        }
    }

    public int NotificationDay
    {
        get => _notificationDay;
        set
        {
            _notificationDay = value;
            SaveData("NotificationDay", _notificationDay);
        }
    }

    public int NotificationHour
    {
        get => _notificationHour;
        set
        {
            _notificationHour = value;
            SaveData("NotificationHour", _notificationHour);
        }
    }

    public bool IsWeeklyPopupEnd
    {
        get => _isWeeklyPopupEnd;
        set
        {
            _isWeeklyPopupEnd = value;
            SaveData("IsWeeklyPopupEnd",_isWeeklyPopupEnd);
        }
    }
    
    public int LimitedShopYear
    {
        get => _limitedShopYear;
        set
        {
            _limitedShopYear = value;
            SaveData("LimitedShopYear", _limitedShopYear);
        }
    }

    public int LimitedShopMonth
    {
        get => _limitedShopMonth;
        set
        {
            _limitedShopMonth = value;
            SaveData("LimitedShopMonth", _limitedShopMonth);
        }
    }

    public int LimitedShopDay
    {
        get => _limitedShopDay;
        set
        {
            _limitedShopDay = value;
            SaveData("LimitedShopDay", _limitedShopDay);
        }
    }
    
    public int LimitedShopHour
    {
        get => _limitedShopHour;
        set
        {
            _limitedShopHour = value;
            SaveData("LimitedShopHour", _limitedShopHour);
        }
    }

    public bool IsLimitedPopupCheck
    {
        get => _islimitedPopupCheck;
        set
        {
            _islimitedPopupCheck = value;
            SaveData("IsLimitedPopupCheck", _islimitedPopupCheck);
        }
    }

    public int NumLanguage
    {
        get => _language;
        set
        {
            _language = value;
            SaveData("Language", _language);
        }
    }

    public bool IsPlayBGM
    {
        get
        {
            if (_isPlayBGM == 1)
                return true;
            return false;
        }
        set
        {
            if (value)
                _isPlayBGM = 1;
            else
                _isPlayBGM = 0;

            SaveData("IsPlayBGM", _isPlayBGM);
        }
    }

    public bool IsPlaySFX
    {
        get
        {
            if (_isPlaySFX == 1)
                return true;
            return false;
        }
        set
        {
            if (value)
                _isPlaySFX = 1;
            else
                _isPlaySFX = 0;

            SaveData("IsPlaySFX", _isPlaySFX);
        }
    }

    public bool IsPlayVib
    {
        get
        {
            if (_isPlayVib == 1)
                return true;
            return false;
        }
        set
        {
            if (value)
                _isPlayVib = 1;
            else
                _isPlayVib = 0;

            SaveData("IsPlayVib", _isPlayVib);
        }
    }

    public bool IsPush
    {
        get
        {
            if (_isPush == 1) return true;
            return false;
        }
        set
        {
            if (value) _isPush = 1;
            else _isPush = 0;

            SaveData("IsPush", _isPush);
        }
    }

    public bool IsAdsFree
    {
        get
        {
            if (_isAdsFree == 1)
                return true;
            return false;
        }
        set
        {
            if (value)
                _isAdsFree = 1;
            else
                _isAdsFree = 0;

            SaveData("IsAdsFree", _isAdsFree);
        }
    }

    public bool IsTermsOfUse_KR
    {
        get
        {
            if (_isTermsOfUse_KR == 1)
                return true;
            return false;
        }
        set
        {
            if (value)
                _isTermsOfUse_KR = 1;
            else
                _isTermsOfUse_KR = 0;

            SaveData("IsTermsOfUse_KR", _isTermsOfUse_KR);
        }
    }

    public bool IsTermsOfUse_EU
    {
        get
        {
            if (_isTermsOfUse_EU == 1)
                return true;
            return false;
        }
        set
        {
            if (value)
                _isTermsOfUse_EU = 1;
            else
                _isTermsOfUse_EU = 0;

            SaveData("IsTermsOfUse_EU", _isTermsOfUse_EU);
        }
    }

    public bool IsRateUs
    {
        get
        {
            if (_isRateUs == 1)
                return true;
            return false;
        }
        set
        {
            if (value)
                _isRateUs = 1;
            else
                _isRateUs = 0;

            SaveData("IsRateUs", _isRateUs);
        }
    }

    public bool IsDailyRoulette
    {
        get
        {
            if (_isDailyRoulette == 1)
                return true;
            return false;
        }
        set
        {
            if (value)
                _isDailyRoulette = 1;
            else
                _isDailyRoulette = 0;

            SaveData("IsDailyRoulette", _isDailyRoulette);
        }
    }

    public bool IsAdsRoulette
    {
        get
        {
            if (_isAdsRoulette == 1)
                return true;
            return false;
        }
        set
        {
            if (value)
                _isAdsRoulette = 1;
            else
                _isAdsRoulette = 0;

            SaveData("IsAdsRoulette", _isAdsRoulette);
        }
    }

    public bool IsGpgsLogin
    {
        get
        {
            if (_isGpgsLogin == 1) return true;
            return false;
        }
        set
        {
            if (value) _isGpgsLogin = 1;
            else _isGpgsLogin = 0;

            SaveData("IsGpgsLogin", _isGpgsLogin);
        }
    }

    public bool IsBuyLimitedPackage
    {
        get
        {
            if (_isBuyLimitedPackage == 1)
                return true;
            return false;
        }
        set
        {
            if (value)
                _isBuyLimitedPackage = 1;
            else
                _isBuyLimitedPackage = 0;

            SaveData("IsBuyLimitedPackage", _isBuyLimitedPackage);
        }
    }

    public bool IsSeeLimitedPackage
    {
        get
        {
            if (_isSeeLimitedPackage == 1)
                return true;
            return false;
        }
        set
        {
            if (value)
                _isSeeLimitedPackage = 1;
            else
                _isSeeLimitedPackage = 0;

            SaveData("IsSeeLimitedPackage", _isSeeLimitedPackage);
        }
    }

    public Dictionary<int, bool> IsItemUnlock
    {
        get => _isItemUnlock;
    }

    public void SaveIsItemUnlock()
    {
        foreach (var item in _isItemUnlock)
        {
            SaveGameBool("IsItemUnlock" + item.Key, item.Value);
        }
    }

    public Dictionary<int, bool> IsItemClicked
    {
        get => _isItemClicked;
    }

    public void SaveIsItemClicked()
    {
        foreach (var item in _isItemClicked)
        {
            SaveGameBool("IsItemClicked" + item.Key, item.Value);
        }
    }

    public int SavingCoin
    {
        get => _savingCoin;
        set
        {
            _savingCoin = Mathf.Min(value, SavingInfomation.isTotalSavingCoin);
            SaveData("SavingCoin", _savingCoin);
        }
    }

    public int StarBoxOpenCount
    {
        get => _starBoxOpenCount;
        set
        {
            _starBoxOpenCount = value;
            SaveData("StarBoxOpenCount", _starBoxOpenCount);
        }
    }

    public int FreeGoldCount
    {
        get => _freeGoldCount;
        set
        {
            _freeGoldCount = value;
            SaveData("FreeGoldCount", _freeGoldCount);
        }
    }

    public int FreeGoldCooltime
    {
        get => _freeGoldCooltime;
        set
        {
            _freeGoldCooltime = value;
            SaveData("FreeGoldCooltime", _freeGoldCooltime);
        }
    }

    public string FreeGoldDate
    {
        get => _freeGoldDate;
        set
        {
            _freeGoldDate = value;
            SaveGameString("FreeGoldDate", _freeGoldDate);
        }
    }

    public int DailyMonthRewardContinueDay
    {
        get => _dailyMonthRewardContinueDay;
        set
        {
            _dailyMonthRewardContinueDay = value;
            SaveData("DailyMonthRewardContinueDay", _dailyMonthRewardContinueDay);
        }
    }

    public int DailyMonthRewardGetDay
    {
        get => _dailyMonthRewardGetDay;
        set
        {
            _dailyMonthRewardGetDay = value;
            SaveData("DailyMonthRewardGetDay", _dailyMonthRewardGetDay);
        }
    }

    public string DailyMonthRewardDate
    {
        get => _dailyMonthRewardDate;
        set
        {
            _dailyMonthRewardDate = value;
            SaveGameString("DailyMonthRewardDate", _dailyMonthRewardDate);
        }
    }

    public int ClearCount
    {
        get => _clearCount;
        set
        {
            _clearCount = value;
            SaveData("ClearCount", _clearCount);
        }
    }

    public int Acorn
    {
        get => _acorn;
        set
        {
            _acorn = value;
            SaveData("Acorn", _acorn);
        }
    }

    public int BeforeEventLevelNum
    {
        get => _beforeEventLevelNum;
        set
        {
            _beforeEventLevelNum = value;
            SaveData("BeforeEventLevelNum", _beforeEventLevelNum);
        }
    }

    public int BeforeAcornGettingLevel
    {
        get => _beforeAcornGettingLevel;
        set
        {
            _beforeAcornGettingLevel = value;
            SaveData("BeforeAcornGettingLevel", _beforeAcornGettingLevel);
        }
    }

    public int PrimiumTicketStar
    {
        get => _primiumTicketStar;
        set
        {
            _primiumTicketStar = value;
            SaveData("PrimiumTicketStar", _primiumTicketStar);
        }
    }
    public int PrimiumTicketLevel
    {
        get => _primiumTicketLevel;
        set
        {
            _primiumTicketLevel = value;
            SaveData("PrimiumTicketLevel", _primiumTicketLevel);
        }
    }

    public int PrimiumTicketReceiveLevel_primium
    {
        get => _primiumTicketReceiveLevel_primium;
        set
        {
            _primiumTicketReceiveLevel_primium = value;
            SaveData("PrimiumTicketReceiveLevel_primium", _primiumTicketReceiveLevel_primium);
        }
    } 
    
    public int PrimiumTicketReceiveLevel_free
    {
        get => _primiumTicketReceiveLevel_free;
        set
        {
            _primiumTicketReceiveLevel_free = value;
            SaveData("PrimiumTicketReceiveLevel_free", _primiumTicketReceiveLevel_free);
        }
    }

    

    public bool IsBuyPrimiumTicket
    {
        get
        {
            if (_buyprimiumTicket) return true;

            return false;
        }

        set
        {
            if (value)
                _buyprimiumTicket = true;
            else
                _buyprimiumTicket = false;

            SaveData("IsBuyPrimiumTicket", _buyprimiumTicket);
        }
    }

    public int MainPackageCheck
    {
        get => _mainPackageCheck;
        set
        {
            _mainPackageCheck = value;
            SaveData("MainPackageCheck", _mainPackageCheck);
        }
    }




    private void Awake()
    {
        EncryptedPlayerPrefs.keys = new string[5];
        EncryptedPlayerPrefs.keys[0] = "82Emssnq";
        EncryptedPlayerPrefs.keys[1] = "SOPI23sn";
        EncryptedPlayerPrefs.keys[2] = "olkpoww";
        EncryptedPlayerPrefs.keys[3] = "pplzmql";
        EncryptedPlayerPrefs.keys[4] = "wrrwlqs";
    }

    public void LoadData()
    {
        _presentLevel = LoadGameInt("PresentLevel");
        _gold = LoadGameInt("Gold");
        _itemHammer = LoadGameInt("ItemHammer", 1);
        _itemCross = LoadGameInt("ItemCross", 1);
        _itemBomb = LoadGameInt("ItemBomb", 1);
        _itemColor = LoadGameInt("ItemColor", 1);
        _language = LoadGameInt("Language");
        for (var i = 1; i < StaticGameSettings.TotalStage + 1; i++)
            if (LoadGameInt("Star_Level" + i) != 0)
                _levelStarCount.Add(i, LoadGameInt("Star_Level" + i));
        _dailyYear = LoadGameInt("DailyYear");
        _dailyMonth = LoadGameInt("DailyMonth");
        _dailyDay = LoadGameInt("DailyDay");
        _RouletteYear = LoadGameInt("RouletteYear");
        _RouletteMonth = LoadGameInt("RouletteMonth");
        _RouletteDay = LoadGameInt("RouletteDay");
        _RouletteHour = LoadGameInt("RouletteHour");
        _RouletteMinute = LoadGameInt("RouletteMinute");
        _RouletteSecond = LoadGameInt("RouletteSecond");
        _worldMapYear = LoadGameInt("WorldMapYear");
        _worldMapMonth = LoadGameInt("WorldMapMonth");
        _worldMapDay = LoadGameInt("WorldMapDay");
        _worldMapHour = LoadGameInt("WorldMapHour");
        _worldMapMinute = LoadGameInt("WorldMapMinute");
        _worldMapSecond = LoadGameInt("WorldMapSecond");
        _specialRewardYear = LoadGameInt("SpecialRewardYear", 0);
        _specialRewardMonth = LoadGameInt("SpecialRewardMonth", 0);
        _specialRewardDay = LoadGameInt("SpecialRewardDay", 0);
        _specialRewardHour = LoadGameInt("SpecialRewardHour", 0);
        _specialRewardMinute = LoadGameInt("SpecialRewardMinute", 0);
        _specialRewardSecond = LoadGameInt("SpecialRewardSecond", 0);
        _isDailyRoulette = LoadGameInt("IsDailyRoulette");
        _isAdsRoulette = LoadGameInt("IsAdsRoulette");
        _weeklyXDay = LoadGameInt("WeeklyXDay");
        _rateUsYear = LoadGameInt("RateUsYear");
        _rateUsMonth = LoadGameInt("RateUsMonth");
        _rateUsDay = LoadGameInt("RateUsDay");
        _faceBookYear = LoadGameInt("FaceBookYear");
        _faceBookMonth = LoadGameInt("FaceBookMonth");
        _faceBookDay = LoadGameInt("FaceBookDay");
        _moreGameYear = LoadGameInt("MoreGameYear");
        _moreGameMonth = LoadGameInt("MoreGameMonth");
        _moreGameDay = LoadGameInt("MoreGameDay");
        _notificationYear = LoadGameInt("NotificationYear");
        _notificationMonth = LoadGameInt("NotificationMonth");
        _notificationDay = LoadGameInt("NotificationDay");
        _notificationHour = LoadGameInt("NotificationHour");
        _isPlayBGM = LoadGameInt("IsPlayBGM", 1);
        _isPlaySFX = LoadGameInt("IsPlaySFX", 1);
        _isPlayVib = LoadGameInt("IsPlayVib", 1);
        _isPush = LoadGameInt("IsPush", 1);
        _isAdsFree = LoadGameInt("IsAdsFree");
        _isTermsOfUse_KR = LoadGameInt("IsTermsOfUse_KR");
        _isTermsOfUse_EU = LoadGameInt("IsTermsOfUse_EU");
        _isRateUs = LoadGameInt("IsRateUs");
        _isGpgsLogin = LoadGameInt("IsGpgsLogin");
        _limitedShopHour = LoadGameInt("LimitedShopHour");
        _limitedShopYear = LoadGameInt("LimitedShopYear");
        _limitedShopMonth = LoadGameInt("LimitedShopMonth");
        _limitedShopDay = LoadGameInt("LimitedShopDay");
        _isBuyLimitedPackage = LoadGameInt("IsBuyLimitedPackage");
        _isCondtion = LoadGameInt("IsCondtion");
        _clearCount = LoadGameInt("ClearCount");

        _youtubeYear = LoadGameInt("YoutubeYear", 0);
        _youtubeMonth = LoadGameInt("YoutubeMonth", 0);
        _youtubeDay = LoadGameInt("YoutubeDay", 0);

        _dailyQuestDate = LoadGameString("DailyQuestDate");

        _acorn = LoadGameInt("Acorn", 0);
        
        if (PlayerPrefs.HasKey("LastVipGetDay"))
        {
            _lastVipGetDay = LoadGameString("LastVipGetDay");
        }
        else
        {
            _lastVipGetDay = DateTime.MinValue.ToString();
        }
        
        
        string stageKeyStr = null;
        int stageKeyInt = 0;
        while (true)
        {
            stageKeyStr = "PassedChallenge" + stageKeyInt.ToString();
            if (PlayerPrefs.HasKey(stageKeyStr))
            {
                _challengeStage.Add(LoadGameInt(stageKeyStr));
                stageKeyInt++;
            }
            else
            {
                break;
            }
        }

        if (PlayerPrefs.HasKey("IsWeeklyPopupEnd"))
        {
            _isWeeklyPopupEnd = LoadGameBool("IsWeeklyPopupEnd");
        }
        

        if (PlayerPrefs.HasKey("IsLimitedPopupCheck"))
        {
            _islimitedPopupCheck = LoadGameBool("IsLimitedPopupCheck");
        }

        if (PlayerPrefs.HasKey("WeeklyRandNum"))
        {
            _weeklyOnePickRandomNum = LoadGameBool("WeeklyRandNum");
        }

        if (PlayerPrefs.HasKey("WeeklyAllClear"))
        {
            _isEventMapAllClear = LoadGameBool("WeeklyAllClear");
        }

        _eventLevelNum = LoadGameInt("EventStageNum", 1);

        stageKeyInt = 0;
        while (true)
        {
            stageKeyStr = "WeeklyEventLevelList" + stageKeyInt.ToString();
            if (PlayerPrefs.HasKey(stageKeyStr))
            {
                _pickRandomNumList.Add(LoadGameInt(stageKeyStr));
                stageKeyInt++;
            }
            else
            {
                break;
            }
        }

        /*//한정판매 확인할때 사용함
        _dailyYear = 1;
       _dailyMonth = 1;
       _dailyDay = 1;
       _weeklyXDay = 0;
       _isBuyLimitedPackage = 0;
       SaveGameInt("WeeklyXDay", _weeklyXDay);
       Debug.Log("위클리 보너스 값 확인 : " + _weeklyXDay);
       Debug.Log("한정판매 확인 " + _isBuyLimitedPackage);
*/
        _isSeeLimitedPackage = LoadGameInt("IsSeeLimitedPackage");
        _savingCoin = LoadGameInt("SavingCoin");
        _starBoxOpenCount = LoadGameInt("StarBoxOpenCount");

        _freeGoldDate = LoadGameString("FreeGoldDate");
        _freeGoldCount = LoadGameInt("FreeGoldCount");
        _freeGoldCooltime = LoadGameInt("FreeGoldCooltime");

        _dailyMonthRewardContinueDay = LoadGameInt("DailyMonthRewardContinueDay");
        _dailyMonthRewardDate = LoadGameString("DailyMonthRewardDate");
        _dailyMonthRewardGetDay = LoadGameInt("DailyMonthRewardGetDay");

        _primiumTicketStar = LoadGameInt("PrimiumTicketStar");
        _primiumTicketLevel = LoadGameInt("PrimiumTicketLevel");
        _primiumTicketReceiveLevel_primium = LoadGameInt("PrimiumTicketReceiveLevel_primium");
        _primiumTicketReceiveLevel_free = LoadGameInt("PrimiumTicketReceiveLevel_free");

        _mainPackageCheck = LoadGameInt("MainPackageCheck");

        if (PlayerPrefs.HasKey("IsBuyPrimiumTicket"))
        {
            _buyprimiumTicket = LoadGameBool("IsBuyPrimiumTicket");
        }


        for (int i = 0; i < Enum.GetValues(typeof(EUseItem)).Length; i++)
        {
            if (PlayerPrefs.HasKey("IsItemUnlock" + i))
            {
                _isItemUnlock.Add(i, LoadGameBool("IsItemUnlock" + i));
            }
            else
            {
                _isItemUnlock.Add(i, false);
            }
        }

        for (int i = 0; i < Enum.GetValues(typeof(EUseItem)).Length; i++)
        {
            if (PlayerPrefs.HasKey("IsItemClicked" + i))
            {
                _isItemClicked.Add(i, LoadGameBool("IsItemClicked" + i));
            }
            else
            {
                _isItemClicked.Add(i, false);
            }
        }

        stageKeyInt = 0;
        while (true)
        {
            stageKeyStr = "GotAcornStage" + stageKeyInt.ToString();
            if (PlayerPrefs.HasKey(stageKeyStr))
            {
                _gotAcornStage.Add(LoadGameInt(stageKeyStr));
                stageKeyInt++;
            }
            else
            {
                break;
            }
        }

        _beforeEventLevelNum = LoadGameInt("BeforeEventLevelNum", 0);
        _beforeAcornGettingLevel = LoadGameInt("BeforeAcornGettingLevel", 0);

        if (PlayerPrefs.HasKey("BuyLimitedPackageTime"))
        {
            buyLimitedPackageTime = Convert.ToDateTime(LoadGameString("BuyLimitedPackageTime"));
        }
        else
        {
            buyLimitedPackageTime = DateTime.MinValue;
        }

        if (PlayerPrefs.HasKey("MainPackageTime")) mainPackageTime = Convert.ToDateTime(LoadGameString("MainPackageTime"));
        else mainPackageTime = DateTime.MinValue;

        if (PlayerPrefs.HasKey("MainPackageChangeCheckTime")) mainPackageChangeCheckTime = Convert.ToDateTime(LoadGameString("MainPackageChangeCheckTime"));
        else mainPackageChangeCheckTime = DateTime.MinValue;


        if (PlayerPrefs.HasKey("PrimiumTicketRewardDate"))
        {
            _primiumTicketRewardDate = Convert.ToDateTime(LoadGameString("PrimiumTicketRewardDate"));
        }
        else
        {
            _primiumTicketRewardDate = DateTime.MinValue;
        }

        LoadEventLevelPickDayCheck();
    }

    public void SaveData(string key, int value)
    {
        SaveGameInt(key, value);
    }

    public void SaveData(string key, bool value)
    {
        SaveGameBool(key, value);
    }

    public void SaveLevel()
    {
        foreach (var item in _levelStarCount) SaveGameInt("Star_Level" + item.Key, item.Value);
    }

    public void SaveAllData()
    {
        SaveGameInt("PresentLevel", _presentLevel);
        SaveGameInt("Gold", _gold);
        SaveGameInt("ItemHammer", _itemHammer);
        SaveGameInt("ItemCross", _itemCross);
        SaveGameInt("ItemBomb", _itemBomb);
        SaveGameInt("ItemColor", _itemColor);
        SaveGameInt("Language", _language);

        foreach (var item in _levelStarCount) SaveGameInt("Star_Level" + item.Key, item.Value);

        SaveGameInt("DailyYear", _dailyYear);
        SaveGameInt("DailyMonth", _dailyMonth);
        SaveGameInt("DailyDay", _dailyDay);
        SaveGameInt("RouletteYear", _RouletteYear);
        SaveGameInt("RouletteMonth", _RouletteMonth);
        SaveGameInt("RouletteDay", _RouletteDay);
        SaveGameInt("RouletteHour", _RouletteHour);
        SaveGameInt("RouletteMinute", _RouletteMinute);
        SaveGameInt("RouletteSecond", _RouletteSecond);
        SaveGameInt("WorldMapYear", _worldMapYear);
        SaveGameInt("WorldMapMonth", _worldMapMonth);
        SaveGameInt("WorldMapDay", _worldMapDay);
        SaveGameInt("WorldMapHour", _worldMapHour);
        SaveGameInt("WorldMapMinute", _worldMapMinute);
        SaveGameInt("WorldMapSecond", _worldMapSecond);
        SaveGameInt("SpecialRewardYear", _specialRewardYear);
        SaveGameInt("SpecialRewardMonth", _specialRewardMonth);
        SaveGameInt("SpecialRewardDay", _specialRewardDay);
        SaveGameInt("SpecialRewardHour", _specialRewardHour);
        SaveGameInt("SpecialRewardMinute", _specialRewardMinute);
        SaveGameInt("SpecialRewardSecond", _specialRewardSecond);
        SaveGameInt("IsDailyRoulette", _isDailyRoulette);
        SaveGameInt("IsAdsRoulette", _isAdsRoulette);
        SaveGameInt("WeeklyXDay", _weeklyXDay);
        SaveGameInt("RateUsYear", _rateUsYear);
        SaveGameInt("RateUsMonth", _rateUsMonth);
        SaveGameInt("RateUsDay", _rateUsDay);
        SaveGameInt("FaceBookYear", _faceBookYear);
        SaveGameInt("FaceBookMonth", _faceBookMonth);
        SaveGameInt("FaceBookDay", _faceBookDay);
        SaveGameInt("MoreGameYear", _moreGameYear);
        SaveGameInt("MoreGameMonth", _moreGameMonth);
        SaveGameInt("MoreGameDay", _moreGameDay);
        SaveGameInt("NotificationYear", _notificationYear);
        SaveGameInt("NotificationMonth", _notificationMonth);
        SaveGameInt("NotificationDay", _notificationDay);
        SaveGameInt("NotificationHour", _notificationHour);
        SaveGameInt("IsPlayBGM", _isPlayBGM);
        SaveGameInt("IsPlaySFX", _isPlaySFX);
        SaveGameInt("IsPlayVib", _isPlayVib);
        SaveGameInt("IsPush", _isPush);
        SaveGameInt("IsAdsFree", _isAdsFree);
        SaveGameInt("IsTermsOfUse_KR", _isTermsOfUse_KR);
        SaveGameInt("IsTermsOfUse_EU", _isTermsOfUse_EU);
        SaveGameInt("IsRateUs", _isRateUs);
        SaveGameInt("IsGpgsLogin", _isGpgsLogin);
        SaveGameBool("IsWeeklyPopupEnd", _isWeeklyPopupEnd);
        SaveGameBool("IsLimitedPopupCheck", _islimitedPopupCheck);
        SaveGameInt("LimitedShopHour", _limitedShopHour);
        SaveGameInt("LimitedShopYear", _limitedShopYear);
        SaveGameInt("LimitedShopMonth", _limitedShopMonth);
        SaveGameInt("LimitedShopDay", _limitedShopDay);
        SaveGameInt("IsBuyLimitedPackage", _isBuyLimitedPackage);
        SaveGameInt("IsSeeLimitedPackage", _isSeeLimitedPackage);
        SaveGameInt("SavingCoin", _savingCoin);
        SaveGameInt("StarBoxOpenCount", _starBoxOpenCount);
        SaveGameInt("IsCondtion", _isCondtion);
        SaveGameInt("ClearCount", _clearCount);
        SaveGameInt("YoutubeYear", _youtubeYear);
        SaveGameInt("YoutubeMonth", _youtubeMonth);
        SaveGameInt("YoutubeDay", _youtubeDay);
        SaveGameString("FreeGoldDate", _freeGoldDate);
        SaveGameInt("FreeGoldCount", _freeGoldCount);
        SaveGameInt("FreeGoldCooltime", _freeGoldCooltime);
        SaveGameString("DailyMonthRewardDate", _dailyMonthRewardDate);
        SaveGameInt("DailyMonthRewardContinueDay", _dailyMonthRewardContinueDay);
        SaveGameInt("DailyMonthRewardGetDay", _dailyMonthRewardGetDay);
        SaveGameInt("Acorn", _acorn);
        SaveGameString("LastVipGetDay", _lastVipGetDay);

        SaveGameInt("PrimiumTicketStar", _primiumTicketStar);
        SaveGameInt("PrimiumTicketLevel", _primiumTicketLevel);
        SaveGameInt("PrimiumTicketReceiveLevel_primium", _primiumTicketReceiveLevel_primium);
        SaveGameInt("PrimiumTicketReceiveLevel_free", _primiumTicketReceiveLevel_free);
        SaveGameBool("IsBuyPrimiumTicket", _buyprimiumTicket);

        SaveGameInt("MainPackageCheck", _mainPackageCheck);
        SaveGameString("MainPackageTime", mainPackageTime.ToString());
        SaveGameString("MainPackageChangeCheckTime", mainPackageChangeCheckTime.ToString());

        foreach (var item in _isItemUnlock)
        {
            SaveGameBool("IsItemUnlock" + item.Key, item.Value);
        }

        foreach (var item in _isItemClicked)
        {
            SaveGameBool("IsItmeClicked" + item.Key, item.Value);
        }
        
        SaveGameString("BuyLimitedPackageTime", buyLimitedPackageTime.ToString());
        SaveGameString("PrimiumTicketRewardDate", _primiumTicketRewardDate.ToString());

        SaveCallengeStage();
        SaveEventLevelList();
    }

    public void SaveChallengeData(int stageNum)
    {
        _challengeStage.Add(stageNum);
        SaveCallengeStage();
    }

    public void SaveCallengeStage()
    {
        string stageKeyStr = null;
        int stageKeyInt = 0;
        foreach (var item in _challengeStage)
        {
            stageKeyStr = "PassedChallenge" + stageKeyInt.ToString();
            SaveGameInt(stageKeyStr, item);
            stageKeyInt++;
        }
    }

    public void SaveGotAcornStageData(int stageNum)
    {
        _gotAcornStage.Add(stageNum);
        SaveGotAcornStage();
    }

    public void SaveGotAcornStage()
    {
        string stageKeyStr = null;
        int stageKeyInt = 0;
        foreach (var item in _gotAcornStage)
        {
            stageKeyStr = "GotAcornStage" + stageKeyInt.ToString();
            SaveGameInt(stageKeyStr, item);
            stageKeyInt++;
        }
    }

    public bool IsGotAcornStage(int stageNumber)
    {
        if (_gotAcornStage.Contains(stageNumber))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //일주일에 한번만.
    public void SaveIsWeeklyOnePick(bool _ispick)
    {
        _weeklyOnePickRandomNum = _ispick;
        SaveGameBool("WeeklyRandNum", _weeklyOnePickRandomNum);
    }

    public void SaveIsEventMapAllClear(bool _isclear)
    {
        _isEventMapAllClear = _isclear;
        SaveGameBool("WeeklyAllClear", _isEventMapAllClear);
    }

    public void SaveEventLevelNumber(int _num)
    {
        _eventLevelNum = _num;
        SaveGameInt("EventStageNum", _num);
    }

    //이벤트 리스트.
    public void SaveEventLevelData(int _num, int _randnum)
    {
        _pickRandomNumList.Add(_randnum);

        if (_num == 4)
        {
            SaveEventLevelList();
        }
    }

    public void SaveEventLevelList()
    {
        string stageKeyStr = null;
        int stageKeyInt = 0;
        foreach (var list in _pickRandomNumList)
        {
            stageKeyStr = "WeeklyEventLevelList" + stageKeyInt.ToString();
            SaveGameInt(stageKeyStr, list);
            stageKeyInt++;
        }
    }

    public void SaveGameInt(string key, int value)
    {
        EncryptedPlayerPrefs.SetInt(key, value);
    }

    public void SaveGameBool(string key, bool value)
    {
        EncryptedPlayerPrefs.SetString(key, value.ToString());
    }

    public void SaveGameFloat(string key, float value)
    {
        EncryptedPlayerPrefs.SetFloat(key, value);
    }

    public void SaveGameString(string key, string value)
    {
        EncryptedPlayerPrefs.SetString(key, value);
    }

    public int LoadGameInt(string key, int defaultValue = 0)
    {
        return EncryptedPlayerPrefs.GetInt(key, defaultValue);
    }

    public bool LoadGameBool(string key)
    {
        return Convert.ToBoolean(EncryptedPlayerPrefs.GetString(key));
    }

    public float LoadGameFloat(string key)
    {
        return EncryptedPlayerPrefs.GetFloat(key, 0);
    }

    public string LoadGameString(string key)
    {
        return EncryptedPlayerPrefs.GetString(key);
    }

    public void SetLevelStartCount(int level, int count)
    {
        if (_levelStarCount.ContainsKey(level))
        {
            if (_levelStarCount[level] < count) _levelStarCount[level] = count;
        }
        else
        {
            _levelStarCount.Add(level, count);
        }

        DailyQuestManager.CollectMission(EDailyQuestType.GETSTAR, count);
        DailyQuestManager.Save();

        SaveLevel();
    }

    public int GetLevelStartCount(int level)
    {
        if (_levelStarCount.ContainsKey(level))
            return _levelStarCount[level];
        return 0;
    }

    public int GetTotalStarCount()
    {
        var returnValue = 0;

        foreach (var item in _levelStarCount) returnValue += item.Value;

        return returnValue;
    }

    public List<int> GetChallengeStage()
    {
        return _challengeStage;
    }

    public List<int> GetEventLevelList()
    {
        return _pickRandomNumList;
    }

    public bool GetIsWeeklyOnePick()
    {
        return _weeklyOnePickRandomNum;
    }

    public bool GetIsEventMapAllClear()
    {
        return _isEventMapAllClear;
    }

    public int GetEventStageNum()
    {
        return _eventLevelNum;
    }

    public void AddEventLevelPickDayCheck(DateTime day)
    {
        _pickDayCheck.Add(day, true);
        SaveEventLevelPickDayCheck();
    }

    public void SaveEventLevelPickDayCheck()
    {
        string keyString = "PickEventLevelDay";

        foreach (var key in _pickDayCheck.Keys)
        {
            SaveGameBool(key.ToString(), _pickDayCheck[key]);
            SaveGameString(keyString, key.ToString());
        }
    }

    public void LoadEventLevelPickDayCheck()
    {
        string keyString = "PickEventLevelDay";

        if (PlayerPrefs.HasKey(keyString))
        {
            DateTime time = Convert.ToDateTime(LoadGameString(keyString));
            _pickDayCheck.Add(time, LoadGameBool(time.ToString()));
        }
    }

    public Dictionary<DateTime, bool> GetEventLevelPickDay()
    {
        return _pickDayCheck;
    }
    

    public void ClearEventLevelPickDay()
    {
        _pickDayCheck.Clear();
    }

    /// <summary>
    /// vip 상품을 구독 상태를 확인하는 함수.
    /// 구독중이면 true, 아니면 false.
    /// </summary>
    /// <returns></returns>
    public bool isVipContinue()
    {
        return _vipContinue;
    }

    /// <summary>
    /// 현재 구독중인 상품이 무엇인지 확인하는 함수.
    /// </summary>
    /// <returns>구독중인 상품의 종류</returns>
    public ESubsType GetVipType()
    {
        return _vipType;
    }

    public string GetLastVipGetDay()
    {
        return _lastVipGetDay;
    }

    public string ToJsonData()
    {
        var data = new SaveDataClass();
        data._dailyDay = _dailyDay;
        data._dailyMonth = _dailyMonth;
        data._dailyYear = _dailyYear;
        data._gold = _gold;
        data._isAdsFree = _isAdsFree;
        data._isPlayBGM = _isPlayBGM;
        data._isPlaySFX = _isPlaySFX;
        data._isPlayVib = _isPlayVib;
        data._isRateUs = _isRateUs;
        data._isTermsOfUse_EU = _isTermsOfUse_EU;
        data._isTermsOfUse_KR = _isTermsOfUse_KR;
        data._itemBomb = _itemBomb;
        data._itemColor = _itemColor;
        data._itemCross = _itemCross;
        data._itemHammer = _itemHammer;
        data._language = _language;
        _weeklyXDay = 0;
        data._weeklyXDay = _weeklyXDay;

        data._levelStarCount = new int[_levelStarCount.Count];
        for ( var i = 0; i < _levelStarCount.Count; i++)
        {
            data._levelStarCount[i] = new int();
            data._levelStarCount[i] = _levelStarCount[i + 1];
        }

        data._presentLevel = _presentLevel;
        data._rateUsDay = _rateUsDay;
        data._rateUsMonth = _rateUsMonth;
        data._rateUsYear = _rateUsYear;

        data._RouletteYear = _RouletteYear;
        data._RouletteMonth = _RouletteMonth;
        data._RouletteDay = _RouletteDay;
        data._RouletteHour = _RouletteHour;
        data._RouletteMinute = _RouletteMinute;
        data._RouletteSecond = _RouletteSecond;

        data._worldMapYear = _worldMapYear;
        data._worldMapMonth = _worldMapMonth;
        data._worldMapDay = _worldMapDay;
        data._worldMapHour = _worldMapHour;
        data._worldMapMinute = _worldMapMinute;
        data._worldMapSecond = _worldMapSecond;
        
        data._specialRewardYear = _specialRewardYear;
        data._specialRewardMonth = _specialRewardMonth;
        data._specialRewardDay = _specialRewardDay;
        data._specialRewardHour = _specialRewardHour;
        data._specialRewardMinute = _specialRewardMinute;
        data._specialRewardSecond = _specialRewardSecond;

        data._faceBookYear = _faceBookYear;
        data._faceBookMonth = _faceBookMonth;
        data._faceBookDay = _faceBookDay;

        data._moreGameYear = _moreGameYear;
        data._moreGameMonth = _moreGameMonth;
        data._moreGameDay = _moreGameDay;

        data._notificationYear = _notificationYear;
        data._notificationMonth = _notificationMonth;
        data._notificationDay = _notificationDay;
        data._notificationHour = _notificationHour;
        
        data._limitedShopHour = _limitedShopHour;
        data._limitedShopYear = _limitedShopYear;
        data._limitedShopMonth = _limitedShopMonth;
        data._limitedShopDay = _limitedShopDay;

        data._isDailyRoulette = _isDailyRoulette;
        data._isAdsRoulette = _isAdsRoulette;

        data._isPush = _isPush;
        data._isBuyLimitedPackage = _isBuyLimitedPackage;
        data._isSeeLimitedPackage = _isSeeLimitedPackage;

        data._savingCoin = _savingCoin;
        data._starBoxOpenCount = _starBoxOpenCount;
        data._clearCount = _clearCount;

        data._youtubeYear = _youtubeYear;
        data._youtubeMonth = _youtubeMonth;
        data._youtubeDay = _youtubeDay;

        data._freeGoldDate = _freeGoldDate;
        data._freeGoldCount = _freeGoldCount;
        data._freeGoldCooltime = _freeGoldCooltime;

        data._dailyMonthRewardDate = _dailyMonthRewardDate;
        data._dailyMonthRewardContinueDay = _dailyMonthRewardContinueDay;
        data._dailyMonthRewardGetDay = _dailyMonthRewardGetDay;
        
        data._acorn=_acorn;
        data._challengeStage = _challengeStage;
        data._pickRandomNumList = _pickRandomNumList;
        data._weeklyOnePickRandomNum=_weeklyOnePickRandomNum;
        data._isEventMapAllClear=_isEventMapAllClear;
        data._eventLevelNum = _eventLevelNum;
        data._beforeEventLevelNum = _beforeEventLevelNum;
        data._beforeAcornGettingLevel = _beforeAcornGettingLevel;
        data._gotAcornStage = _gotAcornStage;

        foreach (var item in _isItemUnlock)
        {
            data._isItemUnlock.Add(item.Key, item.Value);
        }
        foreach (var item in _isItemClicked)
        {
            data._isItemClicked.Add(item.Key, item.Value);
        }
        foreach (var item in _pickDayCheck)
        {
            data._pickDayCheck.Add(item.Key, item.Value);
        }
        data._lastVipGetDay = _lastVipGetDay;

        data._primiumTicketStar = _primiumTicketStar;
        data._primiumTicketLevel = _primiumTicketLevel;
        data._primiumTicketReceiveLevel_primium = _primiumTicketReceiveLevel_primium;
        data._primiumTicketReceiveLevel_free = _primiumTicketReceiveLevel_free;
        data._buyprimiumTicket = _buyprimiumTicket;

        data._mainPackageCheck = _mainPackageCheck;
        data.mainPackageTime = mainPackageTime;
        data.mainPackageChangeCheckTime = mainPackageChangeCheckTime;

        data.buyLimitedPackageTime = buyLimitedPackageTime;
        data._primiumTicketRewardDate = _primiumTicketRewardDate;

        return JsonUtility.ToJson(data);
    }

    public void SetCloudData(SaveDataClass data)
    {
        _dailyDay = data._dailyDay;
        _dailyMonth = data._dailyMonth;
        _dailyYear = data._dailyYear;
        _gold = data._gold;
        _isAdsFree = data._isAdsFree;
        _isPlayBGM = data._isPlayBGM;
        _isPlaySFX = data._isPlaySFX;
        //_isPlayVib = data._isPlayVib;
        _isRateUs = data._isRateUs;
        _isTermsOfUse_EU = data._isTermsOfUse_EU;
        _isTermsOfUse_KR = data._isTermsOfUse_KR;
        _itemBomb = data._itemBomb;
        _itemColor = data._itemColor;
        _itemCross = data._itemCross;
        _itemHammer = data._itemHammer;
        _language = data._language;
        _weeklyXDay = data._weeklyXDay;

        for (var i = 0; i < data._levelStarCount.Length; i++)
            if (_levelStarCount.ContainsKey(i + 1))
                _levelStarCount[i + 1] = data._levelStarCount[i];
            else
                _levelStarCount.Add(i + 1, data._levelStarCount[i]);
        _presentLevel = data._presentLevel;
        _rateUsDay = data._rateUsDay;
        _rateUsMonth = data._rateUsMonth;
        _rateUsYear = data._rateUsYear;

        _RouletteYear = data._RouletteYear;
        _RouletteMonth = data._RouletteMonth;
        _RouletteDay = data._RouletteDay;
        _RouletteHour = data._RouletteHour;
        _RouletteMonth = data._RouletteMonth;
        _RouletteSecond = data._RouletteSecond;

        _worldMapYear = data._worldMapYear;
        _worldMapMonth = data._worldMapMonth;
        _worldMapDay = data._worldMapDay;
        _worldMapHour = data._worldMapHour;
        _worldMapMinute = data._worldMapMinute;
        _worldMapSecond = data._worldMapSecond;
        
        _specialRewardYear = data._specialRewardYear;
        _specialRewardMonth = data._specialRewardMonth;
        _specialRewardDay = data._specialRewardDay;
        _specialRewardHour = data._specialRewardHour;
        _specialRewardMinute = data._specialRewardMinute;
        _specialRewardSecond = data._specialRewardSecond;

        _faceBookYear = data._faceBookYear;
        _faceBookMonth = data._faceBookMonth;
        _faceBookDay = data._faceBookDay;

        _moreGameYear = data._moreGameYear;
        _moreGameMonth = data._moreGameMonth;
        _moreGameDay = data._moreGameDay;

        _notificationYear = data._notificationYear;
        _notificationMonth = data._notificationMonth;
        _notificationDay = data._notificationDay;
        _notificationHour = data._notificationHour;

        _limitedShopHour = data._limitedShopHour;
        _limitedShopYear = data._limitedShopYear;
        _limitedShopMonth = data._limitedShopMonth;
        _limitedShopDay = data._limitedShopDay;

        _isDailyRoulette = data._isDailyRoulette;
        _isAdsRoulette = data._isAdsRoulette;

        _isBuyLimitedPackage = data._isBuyLimitedPackage;
        _isSeeLimitedPackage = data._isSeeLimitedPackage;
        _isCondtion = data._isCondtion;
        _savingCoin = data._savingCoin;
        _starBoxOpenCount = data._starBoxOpenCount;

        _clearCount = data._clearCount;

        _youtubeYear = data._youtubeYear;
        _youtubeMonth = data._youtubeMonth;
        _youtubeDay = data._youtubeDay;
        _freeGoldDate = data._freeGoldDate;
        _freeGoldCount = data._freeGoldCount;
        _freeGoldCooltime = data._freeGoldCooltime;

        _dailyMonthRewardDate = data._dailyMonthRewardDate;
        _dailyMonthRewardContinueDay = data._dailyMonthRewardContinueDay;
        _dailyMonthRewardGetDay = data._dailyMonthRewardGetDay;

        _acorn = data._acorn;
        _challengeStage = data._challengeStage;
        _pickRandomNumList = data._pickRandomNumList;
        _weeklyOnePickRandomNum = data._weeklyOnePickRandomNum;
        _isEventMapAllClear = data._isEventMapAllClear;
        _eventLevelNum = data._eventLevelNum;
        _beforeEventLevelNum = data._beforeEventLevelNum;
        _beforeAcornGettingLevel = data._beforeAcornGettingLevel;
        _gotAcornStage = data._gotAcornStage;

        foreach (var item in data._isItemUnlock)
        {
            _isItemUnlock.Add(item.Key, item.Value);
        }
        foreach (var item in data._isItemClicked)
        {
            _isItemClicked.Add(item.Key, item.Value);
        }

//_pickDayCheck = data._pickDayCheck;

        foreach (var item in data._pickDayCheck)
        {
            _pickDayCheck.Add(item.Key, item.Value);
        }
        
        _lastVipGetDay = data._lastVipGetDay;

        _primiumTicketStar = data._primiumTicketStar;
        _primiumTicketLevel = data._primiumTicketLevel;
        _primiumTicketReceiveLevel_primium = data._primiumTicketReceiveLevel_primium;
        _primiumTicketReceiveLevel_free = data._primiumTicketReceiveLevel_free;
        _buyprimiumTicket = data._buyprimiumTicket;

        _mainPackageCheck = data._mainPackageCheck;
        mainPackageTime = data.mainPackageTime;
        mainPackageChangeCheckTime = data.mainPackageChangeCheckTime;

        buyLimitedPackageTime = data.buyLimitedPackageTime;
        _primiumTicketRewardDate = data._primiumTicketRewardDate;

        SaveAllData();

        DailyQuestManager.LoadData();
    }
}