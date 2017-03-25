using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Chummer.Annotations;
using Chummer.Backend.Attributes.OptionAttributes;
using Chummer.Backend.Attributes.SaveAttributes;
using Chummer.Backend.Options;
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global

namespace Chummer
{
	public class CharacterOptions
	{
	    #region Default Values
		public readonly string FileName;



	    // Settings.
        //TODO: Im sure they had some effect. Or remove them
        /*
		private bool _blnAllow2ndMaxAttribute;
		private bool _blnAllowAttributePointsOnExceptional;
	    private bool _blnAlternateArmorEncumbrance;
		private bool _blnAlternateComplexFormCost;
		private bool _blnAlternateMatrixAttribute;
	    private bool _blnAutomaticCopyProtection = true;
		private bool _blnAutomaticRegistration = true;
	    private bool _blnCalculateCommlinkResponse = true;
	    private bool _blnEnforceSkillMaximumModifiedRating;
		private bool _blnErgonomicProgramsLimit = true;
	    private bool _blnIgnoreArmorEncumbrance = true;
	    private bool _blnMayBuyQualities;
	    private bool _blnNoSingleArmorEncumbrance;
		private bool _blnPrintArcanaAlternates;
	    private bool _blnPrintLeadershipAlternates;
	    private bool _blnSpecialAttributeKarmaLimit;
	    private bool _blnUseContactPoints;
	    private int _intFreeContactsFlatNumber = 0;
        */

	    private readonly XmlDocument _objBookDoc = new XmlDocument();
		private string _strBookXPath = "";

	    #endregion
		// Sourcebook list.

	    #region Initialization, Save, and Load Methods
		public CharacterOptions(string filename)
		{
		    FileName = filename;
		}

		#endregion

		#region Methods
		/// <summary>
		/// Convert a book code into the full name.
		/// </summary>
		/// <param name="strCode">Book code to convert.</param>
		public string BookFromCode(string strCode)
		{
			string strReturn = "";
			XmlNode objXmlBook = _objBookDoc.SelectSingleNode("/chummer/books/book[code = \"" + strCode + "\"]");
			try
			{
				strReturn = objXmlBook["name"].InnerText;
			}
			catch
			{
			}
			return strReturn;
		}

		/// <summary>
		/// Book code (using the translated version if applicable).
		/// </summary>
		/// <param name="strCode">Book code to search for.</param>
		public string LanguageBookShort(string strCode)
		{
			if (strCode == "")
				return "";

			string strReturn = "";
			XmlNode objXmlBook = _objBookDoc.SelectSingleNode("/chummer/books/book[code = \"" + strCode + "\"]");
			try
			{
				if (objXmlBook["altcode"] != null)
					strReturn = objXmlBook["altcode"].InnerText;
				else
					strReturn = strCode;
			}
			catch
			{
			}
			return strReturn;
		}

		/// <summary>
		/// Determine the book's original code by using the alternate code.
		/// </summary>
		/// <param name="strCode">Alternate code to look for.</param>
		public string BookFromAltCode(string strCode)
		{
			if (strCode == "")
				return "";

			XmlNode objXmlBook = _objBookDoc.SelectSingleNode("/chummer/books/book[altcode = \"" + strCode + "\"]");
			if (objXmlBook == null)
				return strCode;
			else
				return objXmlBook["code"].InnerText;
		}

		/// <summary>
		/// Book name (using the translated version if applicable).
		/// </summary>
		/// <param name="strCode">Book code to search for.</param>
		public string LanguageBookLong(string strCode)
		{
			if (strCode == "")
				return "";

			string strReturn = "";
			XmlNode objXmlBook = _objBookDoc.SelectSingleNode("/chummer/books/book[code = \"" + strCode + "\"]");
			try
			{
				if (objXmlBook["translate"] != null)
					strReturn = objXmlBook["translate"].InnerText;
				else
					strReturn = objXmlBook["name"].InnerText;
			}
			catch
			{
			}
			return strReturn;
		}

		/// <summary>
		/// Determine whether or not a given book is in use.
		/// </summary>
		/// <param name="strCode">Book code to search for.</param>
		public bool BookEnabled(string strCode)
		{
		    return Books[strCode];
		}

		/// <summary>
		/// XPath query used to filter items based on the user's selected source books.
		/// </summary>
		public string BookXPath()
		{
			if (_strBookXPath != "")
				return _strBookXPath;

			string strPath = "(";

			foreach (string strBook in EnabledBooks())
			{
				if (strBook != "")
					strPath += "source = \"" + strBook + "\" or ";
			}
			strPath = strPath.Substring(0, strPath.Length - 4) + ")";

			if (GlobalOptions.Instance.MissionsOnly)
			{
				strPath += " and not(nomission)";
			}

			if (!GlobalOptions.Instance.Dronemods)
			{
				strPath += " and not(optionaldrone)";
			}
			_strBookXPath = strPath;
			
			return strPath;
		}

		public List<string> BookLinq()
		{
			return EnabledBooks().ToList();
		}
		#endregion

		#region Program behavour

	    #region Program options
		
        #endregion

	    #endregion

	    #region Character behavour
	    [OptionAttributes("OptionHeader_CharacterOptions")]

	    /// <summary>
	    /// Number of Limbs a standard character has.
	    /// </summary>
	    [SavePropertyAs("limbcount")]
	    
	    public LimbCount LimbCount { get; set; } = LimbCount.All;

	    /// <summary>
	    /// Exclude a particular Limb Slot from count towards the Limb Count.
	    /// </summary>
	    [DisplayIgnore] //TODO: Do something
	    [SavePropertyAs("excludelimbslot")]
	    //TODO: Handler for comboboxes
	    public string ExcludeLimbSlot { get; set; } = "";

	    #region Optional Rules
		/// <summary>
		/// Whether or not the More Lethal Gameplay optional rule is enabled.
		/// </summary>
		//
		[OptionAttributes("OptionHeader_CharacterOptions/Display_HouseRules")]
		[SavePropertyAs("morelethalgameplay")]
		public bool MoreLethalGameplay { get; set; }

	    /// <summary>
		/// Whether or not to require licensing restricted items.
		/// </summary>
		[SavePropertyAs("licenserestricted")]
		public bool LicenseRestricted { get; set; }

	    /// <summary>
		/// Whether or not a Spirit's Maximum Force is based on the character's total MAG.
		/// </summary>
		[SavePropertyAs("spiritforcebasedontotalmag")]
		public bool SpiritForceBasedOnTotalMAG { get; set; }

	    /// <summary>
		/// Whether or not Knucks benefit from improvements to Unarmed DV, such as Adept Powers.
		/// </summary>
		[SavePropertyAs("knucksuseunarmed")]
		public bool KnucksUseUnarmed { get; set; }

	    /// <summary>
		/// Whether or not characters may use Initiation/Submersion in Create mode.
		/// </summary>
		[SavePropertyAs("allowinitiationincreatemode")]
		public bool AllowInitiationInCreateMode { get; set; }

        /// <summary>
        /// Whether or not Essence loss only reduces MAG/RES maximum value, not the current value.
        /// </summary>
        [SavePropertyAs("esslossreducesmaximumonly")]
		public bool ESSLossReducesMaximumOnly { get; set; }

        /// <summary>
        /// Allow Cyberware Essence cost discounts.
        /// </summary>
        [SavePropertyAs("allowcyberwareessdiscounts")]
		public bool AllowCyberwareESSDiscounts { get; set; }

        /// <summary>
        /// Whether or not Maximum Armor Modifications is in use.
        /// </summary>
        [SavePropertyAs("maximumarmormodifications")]
		public bool MaximumArmorModifications { get; set; }

	    /// <summary>
		/// Whether or not Armor Degredation is allowed.
		/// </summary>
		[SavePropertyAs("armordegredation")]
		public bool ArmorDegradation { get; set; }

	    /// <summary>
		/// Whether or not the Karma cost for increasing Special Attributes is based on the shown value instead of actual value.
		/// </summary>
		[SavePropertyAs("specialkarmacostbasedonshownvalue")]
		public bool SpecialKarmaCostBasedOnShownValue { get; set; }

	    /// <summary>
		/// Whether or not characters can have more than 25 BP in Positive Qualities.
		/// </summary>
		[SavePropertyAs("exceedpositivequalities")]
		public bool ExceedPositiveQualities { get; set; }

	    /// <summary>
		/// Whether or not characters can have more than 25 BP in Negative Qualities.
		/// </summary>
		[SavePropertyAs("exceednegativequalities")]
		public bool ExceedNegativeQualities { get; set; }

	    /// <summary>
		/// If true, the character will not receive additional BP from Negative Qualities past the initial 25
		/// </summary>
		[SavePropertyAs("exceednegativequalitieslimit")]
		public bool ExceedNegativeQualitiesLimit { get; set; }

        /// <summary>
        /// Whether or not Restricted items have their cost multiplied.
        /// </summary>
        [SavePropertyAs("multiplyrestrictedcost")]
        public bool MultiplyRestrictedCost { get; set; }

        /// <summary>
        /// Constrains the RestrictedCostMultiplier option to only be enabled if the MultiplyRestrictedCost rule is enabled.
        /// </summary>
        [UsedImplicitly]
        private OptionConstraint<CharacterOptions> MultiplyRestrictedCostConstraint { get; } =
            new OptionConstraint<CharacterOptions>(option => option.MultiplyRestrictedCost);

        /// <summary>
        /// Cost multiplier for Restricted items.
        /// </summary>
        [SavePropertyAs("restrictedcostmultiplier")]
		public int RestrictedCostMultiplier { get; set; } = 1;

        /// <summary>
        /// Whether or not Forbidden items have their cost multiplied.
        /// </summary>
        [SavePropertyAs("multiplyforbiddencost")]
        public bool MultiplyForbiddenCost { get; set; }

        /// <summary>
        /// Constrains the ForbiddenCostMultiplier option to only be enabled if the MultiplyForbiddenCost rule is enabled.
        /// </summary>
        [UsedImplicitly]
        private OptionConstraint<CharacterOptions> MultiplyForbiddenCostConstraint { get; } =
            new OptionConstraint<CharacterOptions>(option => option.MultiplyForbiddenCost);
        
        /// <summary>
        /// Cost multiplier for Forbidden items.
        /// </summary>
        [SavePropertyAs("forbiddencostmultiplier")]
		public int ForbiddenCostMultiplier { get; set; } = 1;

        /// <summary>
        /// Whether to use the rules from SR4 to calculate Public Awareness.
        /// </summary>
        [SavePropertyAs("usecalculatedpublicawareness")]
		public bool UseCalculatedPublicAwareness { get; set; }

        /// <summary>
        /// Whether or not to ignore the art requirements from street grimoire.
        /// </summary>
        [SavePropertyAs("ignoreart")]
		public bool IgnoreArtRequirements { get; set; }

	    /// <summary>
		/// Whether or not to use stats from Cyberlegs when calculating movement rates
		/// </summary>
		[SavePropertyAs("cyberlegmovement")]
		public bool CyberlegMovement { get; set; }

        /// <summary>
        /// Whether or not the DroneArmorMultiplier house rule is enabled.
        /// </summary>
        [SavePropertyAs("dronearmormultiplierenabled")]
        public bool DroneArmorMultiplierEnabled { get; set; }

        /// <summary>
        /// Constrains the DroneArmorMultiplier option to only be enabled if the DroneArmorMultiplierEnabled rule is enabled.
        /// </summary>
        [UsedImplicitly]
	    private OptionConstraint<CharacterOptions> DroneArmorConstraint { get; } =
	        new OptionConstraint<CharacterOptions>(option => option.DroneArmorMultiplierEnabled);

	    /// <summary>
		/// The Drone Body multiplier for maximal Armor
		/// </summary>
		[SavePropertyAs("dronearmorflatnumber")]
	    public int DroneArmorMultiplier { get; set; } = 2;


	    /// <summary>
		/// Whether or not Capacity limits should be enforced.
		/// </summary>
		[SavePropertyAs("enforcecapacity")]
		public bool EnforceCapacity { get; set; } = true;

	    /// <summary>
		/// Whether or not Recoil modifiers are restricted (AR 148).
		/// </summary>
		//TODO: Check this is the same as what is somewhere in R&G
	    //TODO: Should probably be an inverted option, and moved to house rule
		[SavePropertyAs("restrictrecoil")]
		public bool RestrictRecoil { get; set; } = true;

	    /// <summary>
		/// Whether or not characters are unresicted in the number of points they can invest in Nuyen.
		/// </summary>
		[SavePropertyAs("unrestrictednuyen")]
		public bool UnrestrictedNuyen { get; set; }

	    /// <summary>
		/// Whether or not the user can change the Part of Base Weapon flag for a Weapon Accessory or Mod.
		/// </summary>
		[SavePropertyAs("alloweditpartofbaseweapon")]
		public bool AllowEditPartOfBaseWeapon { get; set; }

	    /// <summary>
		/// Whether or not the user can mark any piece of Bioware as being Transgenic.
		/// </summary>
		[SavePropertyAs("allowcustomtransgenics")]
		public bool AllowCustomTransgenics { get; set; }

	    /// <summary>
		/// Whether or not the user is allowed to break Skill Groups while in Create Mode.
		/// </summary>
		[SavePropertyAs("breakskillgroupsincreatemode")]
		public bool StrictSkillGroupsInCreateMode { get; set; }

	    /// <summary>
		/// Whether or not any Detection Spell can be taken as Extended range version.
		/// </summary>
		[SavePropertyAs("extendanydetectionspell")]
		public bool ExtendAnyDetectionSpell { get; set; }

	    /// <summary>
		/// Whether or not dice rolling is allowed for Skills.
		/// </summary>
		[SavePropertyAs("allowskilldicerolling")]
		public bool AllowSkillDiceRolling { get; set; }

	    /// <summary>
		/// Whether or not cyberlimbs stats are used in attribute calculation
		/// </summary>
		[SavePropertyAs("dontusecyberlimbcalculation")]
		public bool DontUseCyberlimbCalculation { get; set; }

	    /// <summary>
		/// Whether or not characters in Career Mode should pay double for qualities.
		/// </summary>
		//TODO: HEADER[OptionAttributes("House Rules/Qualities")]
		[SavePropertyAs("dontdoublequalities")]
		public bool DontDoubleQualityPurchases { get; set; }

	    /// <summary>
		/// Whether or not characters in Career Mode should pay double for removing Negative Qualities.
		/// </summary>
		[SavePropertyAs("dontdoublequalityrefunds")]
		public bool DontDoubleQualityRefunds { get; set; }

	    /// <summary>
		/// Whether or not Obsolescent can be removed/upgraded in the same way as Obsolete.
		/// </summary>
		//TODO: Does this still exist?
		[SavePropertyAs("allowobsolescentupgrade")]
		public bool AllowObsolescentUpgrade { get; set; }

	    /// <summary>
		/// Whether or not Bioware Suites can be added and created.
		/// </summary>
		[SavePropertyAs("allowbiowaresuites")]
		public bool AllowBiowareSuites { get; set; }

	    /// <summary>
		/// House rule: Free Spirits calculate their Power Points based on their MAG instead of EDG.
		/// </summary>
		//TODO: Find out what this is and probably remove it
		[SavePropertyAs("freespiritpowerpointsmag")]
		public bool FreeSpiritPowerPointsMAG { get; set; }

	    /// <summary>
		/// Whether or not Technomancers can select Autosofts as Complex Forms.
		/// </summary>
		[SavePropertyAs("technomancerallowautosoft")]
		public bool TechnomancerAllowAutosoft { get; set; }

        #region Character Creation
        //TODO: HEADER[OptionAttributes("House Rules/Character Creation")]

        /// <summary>
        /// Whether or not the FreeContactsMultiplier house rule is enabled.
        /// </summary>
        [SavePropertyAs("freecontactsmultiplierenabled")]
        public bool FreeContactsMultiplierEnabled { get; set; }

        /// <summary>
        /// Constrains the FreeContactsMultiplier option to only be enabled if the freecontactsmultiplierenabled rule is enabled.
        /// </summary>
        [UsedImplicitly]
        private OptionConstraint<CharacterOptions> ContactsMultiplierConstraint { get; } =
            new OptionConstraint<CharacterOptions>(option => option.FreeContactsMultiplierEnabled);

        /// <summary>
        /// The CHA multiplier to be used with the Free Contacts Option.
        /// </summary>
        [SavePropertyAs("freekarmacontactsmultiplier")]
        public int FreeContactsMultiplier { get; set; } = 3;


        /// <summary>
        /// Whether or not the multiplier for Free Knowledge points are used.
        /// </summary>
        [SavePropertyAs("freekarmaknowledgemultiplierenabled")]
        public bool FreeKnowledgeMultiplierEnabled { get; set; }

        /// <summary>
        /// Constrains the FreeContactsMultiplier option to only be enabled if the freecontactsmultiplierenabled rule is enabled.
        /// </summary>
        [UsedImplicitly]
        private OptionConstraint<CharacterOptions> KnowledgeMultiplierConstraint { get; } =
            new OptionConstraint<CharacterOptions>(option => option.FreeKnowledgeMultiplierEnabled);

        /// <summary>
        /// The INT+LOG multiplier to be used with the Free Knowledge Option.
        /// </summary>
        [SavePropertyAs("freekarmaknowledgemultiplier")]
		public int FreeKnowledgeMultiplier { get; set; } = 2;

	    /// <summary>
		/// Whether or not Metatypes cost Karma.
		/// </summary>
		[SavePropertyAs("metatypecostskarma")]
		public bool MetatypeCostsKarma { get; set; } = true;

	    /// <summary>
		/// Mutiplier for Metatype Karma Costs.
		/// </summary>
		[SavePropertyAs("metatypecostskarmamultiplier")]
		public int MetatypeCostsKarmaMultiplier { get; set; } = 1;

	    /// <summary>
		/// House rule: Treat the Metatype Attribute Minimum as 1 for the purpose of calculating Karma costs.
		/// </summary>
		[SavePropertyAs("alternatemetatypeattributekarma")]
		public bool AlternateMetatypeAttributeKarma { get; set; }

	    /// <summary>
		/// Maximum amount of remaining Karma that is carried over to the character once they are created.
		/// </summary>
		[SavePropertyAs("karmacarryover")]
		public int KarmaCarryover { get; set; } = 7;

	    /// <summary>
		/// Amount of Nuyen gained per Karma spent.
		/// </summary>
		[SavePropertyAs("karmanuyenper")]
		public int NuyenPerBP { get; set; } = 2000;


	    /// <summary>
		/// Whether you benefit from augmented values for contact points.
		/// </summary>
		[SavePropertyAs("usetotalvalueforcontacts")]
		public bool UseTotalValueForFreeContacts { get; set; }

	    /// <summary>
		/// Whether you benefit from augmented values for free knowledge points.
		/// </summary>
		[SavePropertyAs("usetotalvalueforknowledge")]
		public bool UseTotalValueForFreeKnowledge { get; set; }
        #endregion

        #endregion

        #region Optional Rules

        [OptionAttributes("OptionHeader_CharacterOptions/Display_OptionalRules")]
        public bool DroneMods { get; set; } = false;
        #endregion

        #region Unused Rules
        /* These rules have no code references. Some may still be used and were disconnected accidentally. 

        /// <summary>
        /// Whether or not characters can spend skill points to break groups.
        /// </summary>
        [SavePropertyAs("usepointsonbrokengroups")]
        public bool BreakSkillGroupsWithPoints { get; set; }

        /// <summary>
        /// Whether or not total Skill ratings are capped at 20 or 2 x natural Attribute + Rating, whichever is higher.
        /// </summary>
        //[OptionAttributes("Display_CharacterOptions")]
        [SavePropertyAs("capskillrating")]
        public bool CapSkillRating { get; set; }

        /// <summary>
        /// Whether or not Defaulting on a Skill should include any Modifiers.
        /// </summary>
        [SavePropertyAs("skilldefaultingincludesmodifiers")]
        //TODO: Hook this up?
        public bool SkillDefaultingIncludesModifiers { get; set; }

        /// <summary>
        /// Whether or not characters are allowed to put points into a Skill Group again once it is broken and all Ratings are the same.
        /// </summary>
        [SavePropertyAs("allowskillregrouping")]
        public bool AllowSkillRegrouping { get; set; } = true;

        /// <summary>
        /// Whether or not a character's Strength affects Weapon Recoil.
        /// </summary>
        [SavePropertyAs("strengthaffectsrecoil")]
        public bool StrengthAffectsRecoil { get; set; }

        /// <summary>
        /// Whether or not Armor Suit Capacity is in use.
        /// </summary>
        [SavePropertyAs("armorsuitcapacity")]
        public bool ArmorSuitCapacity { get; set; }

	    /// <summary>
		/// Karma cost for Complex Form Skillsofts = Rating x this value.
		/// </summary>
		[SavePropertyAs("karmacomplexformskillsoft")]
		public int KarmaComplexFormSkillsoft { get; set; } = 1;

        */
        #endregion

        #region Karma Costs
        /// <summary>
        /// Karma cost to improve an Attribute = New Rating X this value.
        /// </summary>
        [Header("Character Creation")]
		[OptionAttributes("OptionHeader_CharacterOptions/Display_KarmaCosts")]
		[SavePropertyAs("karmaattribute")]
		public int KarmaAttribute { get; set; } = 5;

	    /// <summary>
		/// Karma cost to purchase a Quality = BP Cost x this value.
		/// </summary>
		[SavePropertyAs("karmaquality")]
		public int KarmaQuality { get; set; } = 1;

	    /// <summary>
		/// Karma cost for a Contact = (Connection + Loyalty) x this value.
		/// </summary>
		[SavePropertyAs("karmacontact")]
		public int KarmaContact { get; set; } = 1;

	    /// <summary>
		/// Karma cost for an Enemy = (Connection + Loyalty) x this value.
		/// </summary>
		[SavePropertyAs("karmaenemy")]
		public int KarmaEnemy { get; set; } = 1;

	    /// <summary>
		/// Karma cost for a Combat Maneuver = this value.
		/// </summary>
		[SavePropertyAs("karmamaneuver")]
		public int KarmaManeuver { get; set; } = 5;

	    #region Skills
		/// <summary>
		/// Karma cost to purchase a Specialization = this value.
		/// </summary>
		[Header("Skills")]
		[SavePropertyAs("karmaspecialization")]
		public int KarmaSpecialization { get; set; } = 7;

	    /// <summary>
		/// Karma cost to purchase a new Knowledge Skill = this value.
		/// </summary>
		[SavePropertyAs("karmanewknowledgeskill")]
		public int KarmaNewKnowledgeSkill { get; set; } = 1;

	    /// <summary>
		/// Karma cost to purchase a new Active Skill = this value.
		/// </summary>
		[SavePropertyAs("karmanewactiveskill")]
		public int KarmaNewActiveSkill { get; set; } = 2;

	    /// <summary>
		/// Karma cost to purchase a new Skill Group = this value.
		/// </summary>
		[SavePropertyAs("karmanewskillgroup")]
		public int KarmaNewSkillGroup { get; set; } = 5;

	    /// <summary>
		/// Karma cost to improve a Knowledge Skill = New Rating x this value.
		/// </summary>
		[SavePropertyAs("karmaimproveknowledgeskill")]
		public int KarmaImproveKnowledgeSkill { get; set; } = 1;

	    /// <summary>
		/// Karma cost to improve an Active Skill = New Rating x this value.
		/// </summary>
		[SavePropertyAs("karmaimproveactiveskill")]
		public int KarmaImproveActiveSkill { get; set; } = 2;

	    /// <summary>
		/// Karma cost to improve a Skill Group = New Rating x this value.
		/// </summary>
		[SavePropertyAs("karmaimproveskillgroup")]
		public int KarmaImproveSkillGroup { get; set; } = 5;

	    #endregion
		#region Magic
		/// <summary>
		/// Karma cost for each Spell = this value.
		/// </summary>
		[Header("Magic")]
		[SavePropertyAs("karmaspell")]
		public int KarmaSpell { get; set; } = 5;

	    /// <summary>
		/// Karma cost for each Enhancement = this value.
		/// </summary>
		[SavePropertyAs("karmaenhancement")]
		public int KarmaEnhancement { get; set; } = 2;

	    /// <summary>
		/// Karma cost for a Spirit = this value.regis
		/// </summary>
		[SavePropertyAs("karmaspirit")]
		public int KarmaSpirit { get; set; } = 1;

	    /// <summary>
		/// Karma cost for a Initiation = 10 + (New Rating x this value).
		/// </summary>
		[SavePropertyAs("karmainitiation")]
		public int KarmaInitiation { get; set; } = 3;

	    /// <summary>
		/// Karma cost for a Metamagic = this value.
		/// </summary>
		[SavePropertyAs("karmametamagic")]
		public int KarmaMetamagic { get; set; } = 15;

	    /// <summary>
		/// Karma cost to join a Group = this value.
		/// </summary>
		[SavePropertyAs("karmajoingroup")]
		public int KarmaJoinGroup { get; set; } = 5;

	    /// <summary>
		/// Karma cost to leave a Group = this value.
		/// </summary>
		[SavePropertyAs("karmaleavegroup")]
		public int KarmaLeaveGroup { get; set; } = 1;

	    /// <summary>
		/// Karma cost for Alchemical Foci.
		/// </summary>
	    [Header("Foci")]
		[SavePropertyAs("karmaalchemicalfocus")]
		public int KarmaAlchemicalFocus { get; set; } = 3;

	    /// <summary>
		/// Karma cost for Banishing Foci.
		/// </summary>
		[SavePropertyAs("karmabanishingfocus")]
		public int KarmaBanishingFocus { get; set; } = 2;

	    /// <summary>
		/// Karma cost for Binding Foci.
		/// </summary>
		[SavePropertyAs("karmabindingfocus")]
		public int KarmaBindingFocus { get; set; } = 2;

	    /// <summary>
		/// Karma cost for Centering Foci.
		/// </summary>
		[SavePropertyAs("karmacenteringfocus")]
		public int KarmaCenteringFocus { get; set; } = 3;

	    /// <summary>
		/// Karma cost for Counterspelling Foci.
		/// </summary>
		[SavePropertyAs("karmacounterspellingfocus")]
		public int KarmaCounterspellingFocus { get; set; } = 2;

	    /// <summary>
		/// Karma cost for Disenchanting Foci.
		/// </summary>
		[SavePropertyAs("karmadisenchantingfocus")]
		public int KarmaDisenchantingFocus { get; set; } = 3;

	    /// <summary>
		/// Karma cost for Flexible Signature Foci.
		/// </summary>
		[SavePropertyAs("karmaflexiblesignaturefocus")]
		public int KarmaFlexibleSignatureFocus { get; set; } = 3;

	    /// <summary>
		/// Karma cost for Masking Foci.
		/// </summary>
		[SavePropertyAs("karmamaskingfocus")]
		public int KarmaMaskingFocus { get; set; } = 3;

	    /// <summary>
		/// Karma cost for Power Foci.
		/// </summary>
		[SavePropertyAs("karmapowerfocus")]
		public int KarmaPowerFocus { get; set; } = 6;

	    /// <summary>
		/// Karma cost for Qi Foci.
		/// </summary>
		[SavePropertyAs("karmaqifocus")]
		public int KarmaQiFocus { get; set; } = 2;

	    /// <summary>
		/// Karma cost for Ritual Spellcasting Foci.
		/// </summary>
		[SavePropertyAs("karmaritualspellcastingfocus")]
		public int KarmaRitualSpellcastingFocus { get; set; } = 2;

	    /// <summary>
		/// Karma cost for Spellcasting Foci.
		/// </summary>
		[SavePropertyAs("karmaspellcastingfocus")]
		public int KarmaSpellcastingFocus { get; set; } = 2;

	    /// <summary>
		/// Karma cost for Spell Shaping Foci.
		/// </summary>
		[SavePropertyAs("karmaspellshapingfocus")]
		public int KarmaSpellShapingFocus { get; set; } = 3;

	    /// <summary>
		/// Karma cost for Summoning Foci.
		/// </summary>
		[SavePropertyAs("karmasummoningfocus")]
		public int KarmaSummoningFocus { get; set; } = 2;

	    /// <summary>
		/// Karma cost for Sustaining Foci.
		/// </summary>
		[SavePropertyAs("karmasustainingfocus")]
		public int KarmaSustainingFocus { get; set; } = 2;

	    /// <summary>
		/// Karma cost for Weapon Foci.
		/// </summary>
		[SavePropertyAs("karmaweaponfocus")]
		public int KarmaWeaponFocus { get; set; } = 3;

	    #endregion
		#region Complex Forms
		/// <summary>
		/// Karma cost for a new Complex Form = this value.
		/// </summary>
		//TODO: HEADER[OptionAttributes("Karma Costs/Complex Forms")]
		[Header("Complex Forms")]
		[SavePropertyAs("karmanewcomplexform")]
		public int KarmaNewComplexForm { get; set; } = 4;

	    /// <summary>
		/// Karma cost to improve a Complex Form = New Rating x this value.
		/// </summary>
		[SavePropertyAs("karmaimprovecomplexform")]
		public int KarmaImproveComplexForm { get; set; } = 1;

	    /// <summary>
		/// Karma cost for Complex Form Options = Rating x this value.
		/// </summary>
		[SavePropertyAs("karmacomplexformoption")]
		public int KarmaComplexFormOption { get; set; } = 2;

	    #endregion
		#endregion

	    #endregion

	    /// <summary>
	    /// Sourcebooks.
	    /// </summary>
	    public Dictionary<string, bool> Books { get; } = GlobalOptions.Instance.SourcebookInfo.ToDictionary(x => x.Code, x => x.Code == "SR5");

	    public IEnumerable<string> EnabledBooks()
	    {
	        foreach (KeyValuePair<string,bool> book in Books)
	        {
	            if (book.Value)
	                yield return book.Key;
	        }
	    }

	    /// <summary>
		/// Setting name.
		/// </summary>
		[DisplayIgnore] //TODO: Do something
		public string Name { get; set; } = "Default Settings";

	    /// <summary>
		/// 
		/// </summary>
		[DisplayIgnore] //TODO: Do something
	    public string RecentImageFolder { get; set; } = "";
	}

    public enum LimbCount
    {
        //Hack with 2 purposes. First it fixes sorting order, second it allows us to have 2 enum values with the value 5
        All = 6,
        NoHead = 0x100+5,
        NoTorso = 0x200+5,
        NoHeadNoTorso = 0x300+4
    }

    public static class LimbCountExtensions
    {
        public static int GetNumberOfLimbs(this LimbCount limbCount) => (int)limbCount & 0xff;
    }

    public class CreationOptions
    {
        public CharacterBuildMethod DefaultBuildMethod { get; set; } = CharacterBuildMethod.Priority;
        public int PointCount { get; set; } = 25;
    }
}