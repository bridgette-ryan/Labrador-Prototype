using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Labrador.Models
{
    #region Dictionary DB
    [Table("Word")]
    public class DbWord
    {
        [PrimaryKey, AutoIncrement, Column("WordID")]
        public int WordID { get; set; }
        [Column("Word"),NotNull]
        public string Word { get; set; }
        [Column("ImageFile")]
        public string ImageFile { get; set; }
        [Column("PersonalInformation"), NotNull]
        public bool PersonalInformation { get; set; }
        [Column("Phonetic")]
        public string Phonetic { get; set; }
    }

    [Table("Tree")]
    public class DbTree
    {
        [PrimaryKey, Column("WordID")]
        public int WordID { get; set; }
        [PrimaryKey, Column("ParentID")]
        public int ParentID { get; set; }
    }

    [Table("Predictions")]
    public class DbPredictions
    {
        [PrimaryKey, Column("PreceedingWord")]
        public int PreceedingWord { get; set; }
        [PrimaryKey, Column("WordID")]
        public int WordID { get; set; }
        [PrimaryKey, Column("Count")]
        public int Count { get; set; }
    }
    #endregion

    #region Configuration DB
    [Table("configuration")]
    public class DbConfiguration
    {
        [PrimaryKey, Column("configID")]
        public int ConfigID { set; get; }
        [Column("languageID"),NotNull]
        public int LanguageID { set; get; }
        [Column("skinTone"), NotNull]
        public int SkinTone { set; get; }
        [Column("PTSColumns"), NotNull]
        public int PTSColumns { set; get; }
        [Column("PTSRows")]
        public int PTSRows { set; get; }
        [Column("showSGD")]
        public bool ShowSGD { set; get; }
        [Column("showDS")]
        public bool ShowDS { set; get; }
        [Column("showSI")]
        public bool ShowSI { set; get; }
        [Column("showPI")]
        public bool ShowPI { set; get; }
        [Column("showER")]
        public bool ShowER { set; get; }
    }

    [Table("languages")]
    public class DbLanguages
    {
        [PrimaryKey,NotNull,Column("languageID")]
        public int LanguageID { set; get; }
        [Column("dictionaryName"),NotNull]
        public string LanguageDB { set; get; }
        [Column("display")]
        public string LanguageName { set; get; }
        [Column("rtl")]
        public bool RightToLeft { set; get; }
    }

    [Table("interface")]
    public class DBInterface
    {
        [PrimaryKey, NotNull, Column("languageID")]
        public int LanguageID { get; set; }
        [Column("PTSEditPageButton")]
        public string PTSEditPageButton { get; set;}
        [Column("PTSHomeButton")]
        public string PTSHomeButton { get; set; }
        [Column("PTSEditInterfaceWordLabel")]
        public string PTSEditInterfaceWordLabel { get; set; }
        [Column("PTSEditInterfacePronounciationLabel")]
        public string PTSEditInterfacePronounciationLabel { get; set; }
        [Column("PTSEditInterfacePersonalLabel")]
        public string PTSEditInterfacePersonalLabel { get; set; }
        [Column("InterfaceBrowseButton")]
        public string InterfaceBrowseButton { get; set; }
        [Column("InterfaceCameraButton")]
        public string InterfaceCameraButton { get; set; }
        [Column("InterfaceListenButton")]
        public string InterfaceListenButton { get; set; }
        [Column("InterfaceApplyButton")]
        public string InterfaceApplyButton { get; set; }
        [Column("ConfigLanguageLabel")]
        public string ConfigLanguageLabel { get; set; }
        [Column("ConfigSkinToneLabel")]
        public string ConfigSkinToneLabel { get; set; }
        [Column("ConfigSGGSizeLabel")]
        public string ConfigSGGSizeLabel { get; set; }
        [Column("ConfigColumnsLabel")]
        public string ConfigColumnsLabel { get; set; }
        [Column("ConfigRowsLabel")]
        public string ConfigRowsLabel { get; set; }
        [Column("ConfigResetPinButton")]
        public string ConfigResetPinButton { get; set; }
        [Column("ConfigSaveButton")]
        public string ConfigSaveButton { get; set; }
        [Column("ConfigShowSectionLabel")]
        public string ConfigShowSectionLabel { get; set; }
        [Column("NavigationSettingsLabel")]
        public string NavigationSettingsLabel { get; set; }
        [Column("NavigationSGDLabel")]
        public string NavigationSGDLabel { get; set; }
        [Column("NavigationDailyStoriesLabel")]
        public string NavigationDailyStoriesLabel { get; set; }
        [Column("NavigationSocialIntLabel")]
        public string NavigationSocialIntLabel { get; set; }
        [Column("NavigationPainIndicatorLabel")]
        public string NavigationPainIndicatorLabel { get; set; }
        [Column("NavigationEmotionalRegLabel")]
        public string NavigationEmotionalRegLabel { get; set; }
        [Column("DSEditCalendarButton")]
        public string DSEditCalendarButton { get; set; }
        [Column("DSGoToDateButton")]
        public string DSGoToDateButton { get; set; }
        [Column("DSAddEventButton")]
        public string DSAddEventButton { get; set; }
        [Column("DSSaveCalendarButton")]
        public string DSSaveCalendarButton { get; set; }
        [Column("DSEditScreenDateLabel")]
        public string DSEditScreenDateLabel { get; set; }
        [Column("DSEditScreenStartLabel")]
        public string DSEditScreenStartLabel { get; set; }
        [Column("DSEditScreenEndLabel")]
        public string DSEditScreenEndLabel { get; set; }
        [Column("DSEditScreenEventLabel")]
        public string DSEditScreenEventLabel { get; set; }
        [Column("DSEditScreenStoryLabel")]
        public string DSEditScreenStoryLabel { get; set; }
        [Column("DSEditScreenRecurringLabel")]
        public string DSEditScreenRecurringLabel { get; set; }
        [Column("DSEditScreenDailyLabel")]
        public string DSEditScreenDailyLabel { get; set; }
        [Column("DSEditScreenWeekdayLabel")]
        public string DSEditScreenWeekdayLabel { get; set; }
        [Column("DSEditScreenWeekendsLabel")]
        public string DSEditScreenWeekendsLabel { get; set; }
        [Column("DSEditScreenWeeklyLabel")]
        public string DSEditScreenWeeklyLabel { get; set; }
        [Column("DSEditScreenFortnightLabel")]
        public string DSEditScreenFortnightLabel { get; set; }
        [Column("DSEditScreenMonthlyLabel")]
        public string DSEditScreenMonthlyLabel { get; set; }
        [Column("PTSBackButton")]
        public string PTSBackButton { get; set; }

        public DBInterface ShallowCopy()
        {
            return (DBInterface)this.MemberwiseClone();
        }
    }
    [Table("security")]
    public class DBSecurity
    {
        [Column("PIN")]
        public int PIN { get; set; }
    }

    #endregion
}
