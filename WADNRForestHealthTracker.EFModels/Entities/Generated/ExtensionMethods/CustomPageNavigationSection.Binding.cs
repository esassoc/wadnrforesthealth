//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[CustomPageNavigationSection]
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
    public abstract partial class CustomPageNavigationSection : IHavePrimaryKey
    {
        public static readonly CustomPageNavigationSectionAbout About = CustomPageNavigationSectionAbout.Instance;
        public static readonly CustomPageNavigationSectionProjects Projects = CustomPageNavigationSectionProjects.Instance;
        public static readonly CustomPageNavigationSectionFinancials Financials = CustomPageNavigationSectionFinancials.Instance;
        public static readonly CustomPageNavigationSectionProgramInfo ProgramInfo = CustomPageNavigationSectionProgramInfo.Instance;

        public static readonly List<CustomPageNavigationSection> All;
        public static readonly ReadOnlyDictionary<int, CustomPageNavigationSection> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static CustomPageNavigationSection()
        {
            All = new List<CustomPageNavigationSection> { About, Projects, Financials, ProgramInfo };
            AllLookupDictionary = new ReadOnlyDictionary<int, CustomPageNavigationSection>(All.ToDictionary(x => x.CustomPageNavigationSectionID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected CustomPageNavigationSection(int customPageNavigationSectionID, string customPageNavigationSectionName)
        {
            CustomPageNavigationSectionID = customPageNavigationSectionID;
            CustomPageNavigationSectionName = customPageNavigationSectionName;
        }

        [Key]
        public int CustomPageNavigationSectionID { get; private set; }
        public string CustomPageNavigationSectionName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return CustomPageNavigationSectionID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(CustomPageNavigationSection other)
        {
            if (other == null)
            {
                return false;
            }
            return other.CustomPageNavigationSectionID == CustomPageNavigationSectionID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as CustomPageNavigationSection);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return CustomPageNavigationSectionID;
        }

        public static bool operator ==(CustomPageNavigationSection left, CustomPageNavigationSection right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CustomPageNavigationSection left, CustomPageNavigationSection right)
        {
            return !Equals(left, right);
        }

        public CustomPageNavigationSectionEnum ToEnum { get { return (CustomPageNavigationSectionEnum)GetHashCode(); } }

        public static CustomPageNavigationSection ToType(int enumValue)
        {
            return ToType((CustomPageNavigationSectionEnum)enumValue);
        }

        public static CustomPageNavigationSection ToType(CustomPageNavigationSectionEnum enumValue)
        {
            switch (enumValue)
            {
                case CustomPageNavigationSectionEnum.About:
                    return About;
                case CustomPageNavigationSectionEnum.Financials:
                    return Financials;
                case CustomPageNavigationSectionEnum.ProgramInfo:
                    return ProgramInfo;
                case CustomPageNavigationSectionEnum.Projects:
                    return Projects;
                default:
                    throw new ArgumentException(string.Format("Unable to map Enum: {0}", enumValue));
            }
        }
    }

    public enum CustomPageNavigationSectionEnum
    {
        About = 1,
        Projects = 2,
        Financials = 3,
        ProgramInfo = 4
    }

    public partial class CustomPageNavigationSectionAbout : CustomPageNavigationSection
    {
        private CustomPageNavigationSectionAbout(int customPageNavigationSectionID, string customPageNavigationSectionName) : base(customPageNavigationSectionID, customPageNavigationSectionName) {}
        public static readonly CustomPageNavigationSectionAbout Instance = new CustomPageNavigationSectionAbout(1, @"About");
    }

    public partial class CustomPageNavigationSectionProjects : CustomPageNavigationSection
    {
        private CustomPageNavigationSectionProjects(int customPageNavigationSectionID, string customPageNavigationSectionName) : base(customPageNavigationSectionID, customPageNavigationSectionName) {}
        public static readonly CustomPageNavigationSectionProjects Instance = new CustomPageNavigationSectionProjects(2, @"Projects");
    }

    public partial class CustomPageNavigationSectionFinancials : CustomPageNavigationSection
    {
        private CustomPageNavigationSectionFinancials(int customPageNavigationSectionID, string customPageNavigationSectionName) : base(customPageNavigationSectionID, customPageNavigationSectionName) {}
        public static readonly CustomPageNavigationSectionFinancials Instance = new CustomPageNavigationSectionFinancials(3, @"Financials");
    }

    public partial class CustomPageNavigationSectionProgramInfo : CustomPageNavigationSection
    {
        private CustomPageNavigationSectionProgramInfo(int customPageNavigationSectionID, string customPageNavigationSectionName) : base(customPageNavigationSectionID, customPageNavigationSectionName) {}
        public static readonly CustomPageNavigationSectionProgramInfo Instance = new CustomPageNavigationSectionProgramInfo(4, @"ProgramInfo");
    }
}