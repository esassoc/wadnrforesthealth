//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FirmaPageType]
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Web;
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public abstract partial class FirmaPageType : IHavePrimaryKey
    {
        public static readonly FirmaPageTypeHomePage HomePage = FirmaPageTypeHomePage.Instance;
        public static readonly FirmaPageTypeDemoScript DemoScript = FirmaPageTypeDemoScript.Instance;
        public static readonly FirmaPageTypeInternalSetupNotes InternalSetupNotes = FirmaPageTypeInternalSetupNotes.Instance;
        public static readonly FirmaPageTypeFullProjectList FullProjectList = FirmaPageTypeFullProjectList.Instance;
        public static readonly FirmaPageTypeProjectTypeList ProjectTypeList = FirmaPageTypeProjectTypeList.Instance;
        public static readonly FirmaPageTypeTaxonomyBranchList TaxonomyBranchList = FirmaPageTypeTaxonomyBranchList.Instance;
        public static readonly FirmaPageTypeTaxonomyTrunkList TaxonomyTrunkList = FirmaPageTypeTaxonomyTrunkList.Instance;
        public static readonly FirmaPageTypeOrganizationsList OrganizationsList = FirmaPageTypeOrganizationsList.Instance;
        public static readonly FirmaPageTypeMyProjects MyProjects = FirmaPageTypeMyProjects.Instance;
        public static readonly FirmaPageTypeProjectMap ProjectMap = FirmaPageTypeProjectMap.Instance;
        public static readonly FirmaPageTypeHomeMapInfo HomeMapInfo = FirmaPageTypeHomeMapInfo.Instance;
        public static readonly FirmaPageTypeHomeAdditionalInfo HomeAdditionalInfo = FirmaPageTypeHomeAdditionalInfo.Instance;
        public static readonly FirmaPageTypeFeaturedProjectList FeaturedProjectList = FirmaPageTypeFeaturedProjectList.Instance;
        public static readonly FirmaPageTypeFullProjectListSimple FullProjectListSimple = FirmaPageTypeFullProjectListSimple.Instance;
        public static readonly FirmaPageTypeTaxonomy Taxonomy = FirmaPageTypeTaxonomy.Instance;
        public static readonly FirmaPageTypeTagList TagList = FirmaPageTypeTagList.Instance;
        public static readonly FirmaPageTypeManageUpdateNotifications ManageUpdateNotifications = FirmaPageTypeManageUpdateNotifications.Instance;
        public static readonly FirmaPageTypeProjectStewardOrganizationList ProjectStewardOrganizationList = FirmaPageTypeProjectStewardOrganizationList.Instance;
        public static readonly FirmaPageTypeEnterHistoricProjectInstructions EnterHistoricProjectInstructions = FirmaPageTypeEnterHistoricProjectInstructions.Instance;
        public static readonly FirmaPageTypePendingProjects PendingProjects = FirmaPageTypePendingProjects.Instance;
        public static readonly FirmaPageTypeTraining Training = FirmaPageTypeTraining.Instance;
        public static readonly FirmaPageTypeCustomFooter CustomFooter = FirmaPageTypeCustomFooter.Instance;
        public static readonly FirmaPageTypeFactSheetCustomText FactSheetCustomText = FirmaPageTypeFactSheetCustomText.Instance;
        public static readonly FirmaPageTypeFocusAreasList FocusAreasList = FirmaPageTypeFocusAreasList.Instance;
        public static readonly FirmaPageTypeFullFundSourceList FullFundSourceList = FirmaPageTypeFullFundSourceList.Instance;
        public static readonly FirmaPageTypeFullFundSourceAllocationList FullFundSourceAllocationList = FirmaPageTypeFullFundSourceAllocationList.Instance;
        public static readonly FirmaPageTypeRegionsList RegionsList = FirmaPageTypeRegionsList.Instance;
        public static readonly FirmaPageTypePriorityLandscapesList PriorityLandscapesList = FirmaPageTypePriorityLandscapesList.Instance;
        public static readonly FirmaPageTypeFullAgreementList FullAgreementList = FirmaPageTypeFullAgreementList.Instance;
        public static readonly FirmaPageTypeInteractionEventList InteractionEventList = FirmaPageTypeInteractionEventList.Instance;
        public static readonly FirmaPageTypeGisUploadAttemptInstructions GisUploadAttemptInstructions = FirmaPageTypeGisUploadAttemptInstructions.Instance;
        public static readonly FirmaPageTypeProgramsList ProgramsList = FirmaPageTypeProgramsList.Instance;
        public static readonly FirmaPageTypeUploadLoaTabularDataExcel UploadLoaTabularDataExcel = FirmaPageTypeUploadLoaTabularDataExcel.Instance;
        public static readonly FirmaPageTypeManageFindYourForester ManageFindYourForester = FirmaPageTypeManageFindYourForester.Instance;
        public static readonly FirmaPageTypeFindYourForester FindYourForester = FirmaPageTypeFindYourForester.Instance;
        public static readonly FirmaPageTypeExternalMapLayers ExternalMapLayers = FirmaPageTypeExternalMapLayers.Instance;
        public static readonly FirmaPageTypeCounty County = FirmaPageTypeCounty.Instance;
        public static readonly FirmaPageTypeReports Reports = FirmaPageTypeReports.Instance;
        public static readonly FirmaPageTypeReportProjects ReportProjects = FirmaPageTypeReportProjects.Instance;
        public static readonly FirmaPageTypeReportAddReport ReportAddReport = FirmaPageTypeReportAddReport.Instance;
        public static readonly FirmaPageTypeVendor Vendor = FirmaPageTypeVendor.Instance;
        public static readonly FirmaPageTypeProjectCreateInstructions ProjectCreateInstructions = FirmaPageTypeProjectCreateInstructions.Instance;
        public static readonly FirmaPageTypeProjectUpdateInstructions ProjectUpdateInstructions = FirmaPageTypeProjectUpdateInstructions.Instance;

        public static readonly List<FirmaPageType> All;
        public static readonly ReadOnlyDictionary<int, FirmaPageType> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static FirmaPageType()
        {
            All = new List<FirmaPageType> { HomePage, DemoScript, InternalSetupNotes, FullProjectList, ProjectTypeList, TaxonomyBranchList, TaxonomyTrunkList, OrganizationsList, MyProjects, ProjectMap, HomeMapInfo, HomeAdditionalInfo, FeaturedProjectList, FullProjectListSimple, Taxonomy, TagList, ManageUpdateNotifications, ProjectStewardOrganizationList, EnterHistoricProjectInstructions, PendingProjects, Training, CustomFooter, FactSheetCustomText, FocusAreasList, FullFundSourceList, FullFundSourceAllocationList, RegionsList, PriorityLandscapesList, FullAgreementList, InteractionEventList, GisUploadAttemptInstructions, ProgramsList, UploadLoaTabularDataExcel, ManageFindYourForester, FindYourForester, ExternalMapLayers, County, Reports, ReportProjects, ReportAddReport, Vendor, ProjectCreateInstructions, ProjectUpdateInstructions };
            AllLookupDictionary = new ReadOnlyDictionary<int, FirmaPageType>(All.ToDictionary(x => x.FirmaPageTypeID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected FirmaPageType(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID)
        {
            FirmaPageTypeID = firmaPageTypeID;
            FirmaPageTypeName = firmaPageTypeName;
            FirmaPageTypeDisplayName = firmaPageTypeDisplayName;
            FirmaPageRenderTypeID = firmaPageRenderTypeID;
        }
        public FirmaPageRenderType FirmaPageRenderType { get { return FirmaPageRenderType.AllLookupDictionary[FirmaPageRenderTypeID]; } }
        [Key]
        public int FirmaPageTypeID { get; private set; }
        public string FirmaPageTypeName { get; private set; }
        public string FirmaPageTypeDisplayName { get; private set; }
        public int FirmaPageRenderTypeID { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return FirmaPageTypeID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(FirmaPageType other)
        {
            if (other == null)
            {
                return false;
            }
            return other.FirmaPageTypeID == FirmaPageTypeID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as FirmaPageType);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return FirmaPageTypeID;
        }

        public static bool operator ==(FirmaPageType left, FirmaPageType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(FirmaPageType left, FirmaPageType right)
        {
            return !Equals(left, right);
        }

        public FirmaPageTypeEnum ToEnum { get { return (FirmaPageTypeEnum)GetHashCode(); } }

        public static FirmaPageType ToType(int enumValue)
        {
            return ToType((FirmaPageTypeEnum)enumValue);
        }

        public static FirmaPageType ToType(FirmaPageTypeEnum enumValue)
        {
            switch (enumValue)
            {
                case FirmaPageTypeEnum.County:
                    return County;
                case FirmaPageTypeEnum.CustomFooter:
                    return CustomFooter;
                case FirmaPageTypeEnum.DemoScript:
                    return DemoScript;
                case FirmaPageTypeEnum.EnterHistoricProjectInstructions:
                    return EnterHistoricProjectInstructions;
                case FirmaPageTypeEnum.ExternalMapLayers:
                    return ExternalMapLayers;
                case FirmaPageTypeEnum.FactSheetCustomText:
                    return FactSheetCustomText;
                case FirmaPageTypeEnum.FeaturedProjectList:
                    return FeaturedProjectList;
                case FirmaPageTypeEnum.FindYourForester:
                    return FindYourForester;
                case FirmaPageTypeEnum.FocusAreasList:
                    return FocusAreasList;
                case FirmaPageTypeEnum.FullAgreementList:
                    return FullAgreementList;
                case FirmaPageTypeEnum.FullFundSourceAllocationList:
                    return FullFundSourceAllocationList;
                case FirmaPageTypeEnum.FullFundSourceList:
                    return FullFundSourceList;
                case FirmaPageTypeEnum.FullProjectList:
                    return FullProjectList;
                case FirmaPageTypeEnum.FullProjectListSimple:
                    return FullProjectListSimple;
                case FirmaPageTypeEnum.GisUploadAttemptInstructions:
                    return GisUploadAttemptInstructions;
                case FirmaPageTypeEnum.HomeAdditionalInfo:
                    return HomeAdditionalInfo;
                case FirmaPageTypeEnum.HomeMapInfo:
                    return HomeMapInfo;
                case FirmaPageTypeEnum.HomePage:
                    return HomePage;
                case FirmaPageTypeEnum.InteractionEventList:
                    return InteractionEventList;
                case FirmaPageTypeEnum.InternalSetupNotes:
                    return InternalSetupNotes;
                case FirmaPageTypeEnum.ManageFindYourForester:
                    return ManageFindYourForester;
                case FirmaPageTypeEnum.ManageUpdateNotifications:
                    return ManageUpdateNotifications;
                case FirmaPageTypeEnum.MyProjects:
                    return MyProjects;
                case FirmaPageTypeEnum.OrganizationsList:
                    return OrganizationsList;
                case FirmaPageTypeEnum.PendingProjects:
                    return PendingProjects;
                case FirmaPageTypeEnum.PriorityLandscapesList:
                    return PriorityLandscapesList;
                case FirmaPageTypeEnum.ProgramsList:
                    return ProgramsList;
                case FirmaPageTypeEnum.ProjectCreateInstructions:
                    return ProjectCreateInstructions;
                case FirmaPageTypeEnum.ProjectMap:
                    return ProjectMap;
                case FirmaPageTypeEnum.ProjectStewardOrganizationList:
                    return ProjectStewardOrganizationList;
                case FirmaPageTypeEnum.ProjectTypeList:
                    return ProjectTypeList;
                case FirmaPageTypeEnum.ProjectUpdateInstructions:
                    return ProjectUpdateInstructions;
                case FirmaPageTypeEnum.RegionsList:
                    return RegionsList;
                case FirmaPageTypeEnum.ReportAddReport:
                    return ReportAddReport;
                case FirmaPageTypeEnum.ReportProjects:
                    return ReportProjects;
                case FirmaPageTypeEnum.Reports:
                    return Reports;
                case FirmaPageTypeEnum.TagList:
                    return TagList;
                case FirmaPageTypeEnum.Taxonomy:
                    return Taxonomy;
                case FirmaPageTypeEnum.TaxonomyBranchList:
                    return TaxonomyBranchList;
                case FirmaPageTypeEnum.TaxonomyTrunkList:
                    return TaxonomyTrunkList;
                case FirmaPageTypeEnum.Training:
                    return Training;
                case FirmaPageTypeEnum.UploadLoaTabularDataExcel:
                    return UploadLoaTabularDataExcel;
                case FirmaPageTypeEnum.Vendor:
                    return Vendor;
                default:
                    throw new ArgumentException(string.Format("Unable to map Enum: {0}", enumValue));
            }
        }
    }

    public enum FirmaPageTypeEnum
    {
        HomePage = 1,
        DemoScript = 4,
        InternalSetupNotes = 5,
        FullProjectList = 6,
        ProjectTypeList = 11,
        TaxonomyBranchList = 13,
        TaxonomyTrunkList = 14,
        OrganizationsList = 16,
        MyProjects = 18,
        ProjectMap = 22,
        HomeMapInfo = 24,
        HomeAdditionalInfo = 25,
        FeaturedProjectList = 26,
        FullProjectListSimple = 33,
        Taxonomy = 34,
        TagList = 36,
        ManageUpdateNotifications = 41,
        ProjectStewardOrganizationList = 46,
        EnterHistoricProjectInstructions = 47,
        PendingProjects = 48,
        Training = 49,
        CustomFooter = 51,
        FactSheetCustomText = 54,
        FocusAreasList = 55,
        FullFundSourceList = 56,
        FullFundSourceAllocationList = 57,
        RegionsList = 58,
        PriorityLandscapesList = 59,
        FullAgreementList = 60,
        InteractionEventList = 62,
        GisUploadAttemptInstructions = 63,
        ProgramsList = 65,
        UploadLoaTabularDataExcel = 66,
        ManageFindYourForester = 67,
        FindYourForester = 68,
        ExternalMapLayers = 69,
        County = 70,
        Reports = 71,
        ReportProjects = 72,
        ReportAddReport = 73,
        Vendor = 74,
        ProjectCreateInstructions = 75,
        ProjectUpdateInstructions = 76
    }

    public partial class FirmaPageTypeHomePage : FirmaPageType
    {
        private FirmaPageTypeHomePage(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypeHomePage Instance = new FirmaPageTypeHomePage(1, @"HomePage", @"Home Page", 2);
    }

    public partial class FirmaPageTypeDemoScript : FirmaPageType
    {
        private FirmaPageTypeDemoScript(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypeDemoScript Instance = new FirmaPageTypeDemoScript(4, @"DemoScript", @"Demo Script", 2);
    }

    public partial class FirmaPageTypeInternalSetupNotes : FirmaPageType
    {
        private FirmaPageTypeInternalSetupNotes(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypeInternalSetupNotes Instance = new FirmaPageTypeInternalSetupNotes(5, @"InternalSetupNotes", @"Internal Setup Notes", 2);
    }

    public partial class FirmaPageTypeFullProjectList : FirmaPageType
    {
        private FirmaPageTypeFullProjectList(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypeFullProjectList Instance = new FirmaPageTypeFullProjectList(6, @"FullProjectList", @"Full Project List", 1);
    }

    public partial class FirmaPageTypeProjectTypeList : FirmaPageType
    {
        private FirmaPageTypeProjectTypeList(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypeProjectTypeList Instance = new FirmaPageTypeProjectTypeList(11, @"ProjectTypeList", @"Project Type List", 1);
    }

    public partial class FirmaPageTypeTaxonomyBranchList : FirmaPageType
    {
        private FirmaPageTypeTaxonomyBranchList(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypeTaxonomyBranchList Instance = new FirmaPageTypeTaxonomyBranchList(13, @"TaxonomyBranchList", @"Taxonomy Branch List", 1);
    }

    public partial class FirmaPageTypeTaxonomyTrunkList : FirmaPageType
    {
        private FirmaPageTypeTaxonomyTrunkList(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypeTaxonomyTrunkList Instance = new FirmaPageTypeTaxonomyTrunkList(14, @"TaxonomyTrunkList", @"Taxonomy Trunk List", 1);
    }

    public partial class FirmaPageTypeOrganizationsList : FirmaPageType
    {
        private FirmaPageTypeOrganizationsList(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypeOrganizationsList Instance = new FirmaPageTypeOrganizationsList(16, @"OrganizationsList", @"Organizations List", 1);
    }

    public partial class FirmaPageTypeMyProjects : FirmaPageType
    {
        private FirmaPageTypeMyProjects(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypeMyProjects Instance = new FirmaPageTypeMyProjects(18, @"MyProjects", @"My Projects", 1);
    }

    public partial class FirmaPageTypeProjectMap : FirmaPageType
    {
        private FirmaPageTypeProjectMap(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypeProjectMap Instance = new FirmaPageTypeProjectMap(22, @"ProjectMap", @"Project Map", 1);
    }

    public partial class FirmaPageTypeHomeMapInfo : FirmaPageType
    {
        private FirmaPageTypeHomeMapInfo(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypeHomeMapInfo Instance = new FirmaPageTypeHomeMapInfo(24, @"HomeMapInfo", @"Home Page Map Info", 2);
    }

    public partial class FirmaPageTypeHomeAdditionalInfo : FirmaPageType
    {
        private FirmaPageTypeHomeAdditionalInfo(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypeHomeAdditionalInfo Instance = new FirmaPageTypeHomeAdditionalInfo(25, @"HomeAdditionalInfo", @"Home Page Additional Info", 2);
    }

    public partial class FirmaPageTypeFeaturedProjectList : FirmaPageType
    {
        private FirmaPageTypeFeaturedProjectList(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypeFeaturedProjectList Instance = new FirmaPageTypeFeaturedProjectList(26, @"FeaturedProjectList", @"Featured Project List", 1);
    }

    public partial class FirmaPageTypeFullProjectListSimple : FirmaPageType
    {
        private FirmaPageTypeFullProjectListSimple(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypeFullProjectListSimple Instance = new FirmaPageTypeFullProjectListSimple(33, @"FullProjectListSimple", @"Full Project List (Simple)", 1);
    }

    public partial class FirmaPageTypeTaxonomy : FirmaPageType
    {
        private FirmaPageTypeTaxonomy(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypeTaxonomy Instance = new FirmaPageTypeTaxonomy(34, @"Taxonomy", @"Taxonomy", 1);
    }

    public partial class FirmaPageTypeTagList : FirmaPageType
    {
        private FirmaPageTypeTagList(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypeTagList Instance = new FirmaPageTypeTagList(36, @"TagList", @"Tag List", 1);
    }

    public partial class FirmaPageTypeManageUpdateNotifications : FirmaPageType
    {
        private FirmaPageTypeManageUpdateNotifications(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypeManageUpdateNotifications Instance = new FirmaPageTypeManageUpdateNotifications(41, @"ManageUpdateNotifications", @"Manage Project Update Notifications", 1);
    }

    public partial class FirmaPageTypeProjectStewardOrganizationList : FirmaPageType
    {
        private FirmaPageTypeProjectStewardOrganizationList(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypeProjectStewardOrganizationList Instance = new FirmaPageTypeProjectStewardOrganizationList(46, @"ProjectStewardOrganizationList", @"ProjectStewardOrganizationList", 1);
    }

    public partial class FirmaPageTypeEnterHistoricProjectInstructions : FirmaPageType
    {
        private FirmaPageTypeEnterHistoricProjectInstructions(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypeEnterHistoricProjectInstructions Instance = new FirmaPageTypeEnterHistoricProjectInstructions(47, @"EnterHistoricProjectInstructions", @"Enter Historic Project Instructions", 2);
    }

    public partial class FirmaPageTypePendingProjects : FirmaPageType
    {
        private FirmaPageTypePendingProjects(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypePendingProjects Instance = new FirmaPageTypePendingProjects(48, @"PendingProjects", @"Pending Projects", 1);
    }

    public partial class FirmaPageTypeTraining : FirmaPageType
    {
        private FirmaPageTypeTraining(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypeTraining Instance = new FirmaPageTypeTraining(49, @"Training", @"Training", 2);
    }

    public partial class FirmaPageTypeCustomFooter : FirmaPageType
    {
        private FirmaPageTypeCustomFooter(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypeCustomFooter Instance = new FirmaPageTypeCustomFooter(51, @"CustomFooter", @"Custom Footer", 1);
    }

    public partial class FirmaPageTypeFactSheetCustomText : FirmaPageType
    {
        private FirmaPageTypeFactSheetCustomText(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypeFactSheetCustomText Instance = new FirmaPageTypeFactSheetCustomText(54, @"FactSheetCustomText", @"Fact Sheet Custom Text", 2);
    }

    public partial class FirmaPageTypeFocusAreasList : FirmaPageType
    {
        private FirmaPageTypeFocusAreasList(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypeFocusAreasList Instance = new FirmaPageTypeFocusAreasList(55, @"FocusAreasList", @"Focus Areas List", 1);
    }

    public partial class FirmaPageTypeFullFundSourceList : FirmaPageType
    {
        private FirmaPageTypeFullFundSourceList(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypeFullFundSourceList Instance = new FirmaPageTypeFullFundSourceList(56, @"FullFundSourceList", @"Full Fund Source List", 1);
    }

    public partial class FirmaPageTypeFullFundSourceAllocationList : FirmaPageType
    {
        private FirmaPageTypeFullFundSourceAllocationList(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypeFullFundSourceAllocationList Instance = new FirmaPageTypeFullFundSourceAllocationList(57, @"FullFundSourceAllocationList", @"Full Fund Source Allocation List", 1);
    }

    public partial class FirmaPageTypeRegionsList : FirmaPageType
    {
        private FirmaPageTypeRegionsList(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypeRegionsList Instance = new FirmaPageTypeRegionsList(58, @"RegionsList", @"Regions List", 1);
    }

    public partial class FirmaPageTypePriorityLandscapesList : FirmaPageType
    {
        private FirmaPageTypePriorityLandscapesList(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypePriorityLandscapesList Instance = new FirmaPageTypePriorityLandscapesList(59, @"PriorityLandscapesList", @"Priority Landscapes List", 1);
    }

    public partial class FirmaPageTypeFullAgreementList : FirmaPageType
    {
        private FirmaPageTypeFullAgreementList(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypeFullAgreementList Instance = new FirmaPageTypeFullAgreementList(60, @"FullAgreementList", @"Full Agreement List", 1);
    }

    public partial class FirmaPageTypeInteractionEventList : FirmaPageType
    {
        private FirmaPageTypeInteractionEventList(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypeInteractionEventList Instance = new FirmaPageTypeInteractionEventList(62, @"InteractionEventList", @"Interaction/Event List", 1);
    }

    public partial class FirmaPageTypeGisUploadAttemptInstructions : FirmaPageType
    {
        private FirmaPageTypeGisUploadAttemptInstructions(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypeGisUploadAttemptInstructions Instance = new FirmaPageTypeGisUploadAttemptInstructions(63, @"GisUploadAttemptInstructions", @"GIS Upload Attempt Instructions", 1);
    }

    public partial class FirmaPageTypeProgramsList : FirmaPageType
    {
        private FirmaPageTypeProgramsList(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypeProgramsList Instance = new FirmaPageTypeProgramsList(65, @"ProgramsList", @"Programs List", 1);
    }

    public partial class FirmaPageTypeUploadLoaTabularDataExcel : FirmaPageType
    {
        private FirmaPageTypeUploadLoaTabularDataExcel(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypeUploadLoaTabularDataExcel Instance = new FirmaPageTypeUploadLoaTabularDataExcel(66, @"UploadLoaTabularDataExcel", @"Upload Loa Tabular Data", 1);
    }

    public partial class FirmaPageTypeManageFindYourForester : FirmaPageType
    {
        private FirmaPageTypeManageFindYourForester(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypeManageFindYourForester Instance = new FirmaPageTypeManageFindYourForester(67, @"ManageFindYourForester", @"Manage Find Your Forester", 1);
    }

    public partial class FirmaPageTypeFindYourForester : FirmaPageType
    {
        private FirmaPageTypeFindYourForester(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypeFindYourForester Instance = new FirmaPageTypeFindYourForester(68, @"FindYourForester", @"Find Your Forester", 1);
    }

    public partial class FirmaPageTypeExternalMapLayers : FirmaPageType
    {
        private FirmaPageTypeExternalMapLayers(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypeExternalMapLayers Instance = new FirmaPageTypeExternalMapLayers(69, @"ExternalMapLayers", @"External Map Layers", 1);
    }

    public partial class FirmaPageTypeCounty : FirmaPageType
    {
        private FirmaPageTypeCounty(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypeCounty Instance = new FirmaPageTypeCounty(70, @"County", @"Counties", 1);
    }

    public partial class FirmaPageTypeReports : FirmaPageType
    {
        private FirmaPageTypeReports(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypeReports Instance = new FirmaPageTypeReports(71, @"Reports", @"Reports", 1);
    }

    public partial class FirmaPageTypeReportProjects : FirmaPageType
    {
        private FirmaPageTypeReportProjects(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypeReportProjects Instance = new FirmaPageTypeReportProjects(72, @"ReportProjects", @"Report Projects", 1);
    }

    public partial class FirmaPageTypeReportAddReport : FirmaPageType
    {
        private FirmaPageTypeReportAddReport(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypeReportAddReport Instance = new FirmaPageTypeReportAddReport(73, @"ReportAddReport", @"Add a Report", 2);
    }

    public partial class FirmaPageTypeVendor : FirmaPageType
    {
        private FirmaPageTypeVendor(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypeVendor Instance = new FirmaPageTypeVendor(74, @"Vendor", @"Vendors", 1);
    }

    public partial class FirmaPageTypeProjectCreateInstructions : FirmaPageType
    {
        private FirmaPageTypeProjectCreateInstructions(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypeProjectCreateInstructions Instance = new FirmaPageTypeProjectCreateInstructions(75, @"ProjectCreateInstructions", @"Project Create Instructions", 2);
    }

    public partial class FirmaPageTypeProjectUpdateInstructions : FirmaPageType
    {
        private FirmaPageTypeProjectUpdateInstructions(int firmaPageTypeID, string firmaPageTypeName, string firmaPageTypeDisplayName, int firmaPageRenderTypeID) : base(firmaPageTypeID, firmaPageTypeName, firmaPageTypeDisplayName, firmaPageRenderTypeID) {}
        public static readonly FirmaPageTypeProjectUpdateInstructions Instance = new FirmaPageTypeProjectUpdateInstructions(76, @"ProjectUpdateInstructions", @"Project Update Instructions", 2);
    }
}