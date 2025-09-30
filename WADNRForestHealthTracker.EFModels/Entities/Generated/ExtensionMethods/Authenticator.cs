//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Authenticator]
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
    public abstract partial class Authenticator : IHavePrimaryKey
    {
        public static readonly AuthenticatorADFS ADFS = AuthenticatorADFS.Instance;
        public static readonly AuthenticatorSAWPROD SAWPROD = AuthenticatorSAWPROD.Instance;
        public static readonly AuthenticatorSAWTEST SAWTEST = AuthenticatorSAWTEST.Instance;

        public static readonly List<Authenticator> All;
        public static readonly ReadOnlyDictionary<int, Authenticator> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static Authenticator()
        {
            All = new List<Authenticator> { ADFS, SAWPROD, SAWTEST };
            AllLookupDictionary = new ReadOnlyDictionary<int, Authenticator>(All.ToDictionary(x => x.AuthenticatorID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected Authenticator(int authenticatorID, string authenticatorName, string authenticatorFullName)
        {
            AuthenticatorID = authenticatorID;
            AuthenticatorName = authenticatorName;
            AuthenticatorFullName = authenticatorFullName;
        }

        [Key]
        public int AuthenticatorID { get; private set; }
        public string AuthenticatorName { get; private set; }
        public string AuthenticatorFullName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return AuthenticatorID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(Authenticator other)
        {
            if (other == null)
            {
                return false;
            }
            return other.AuthenticatorID == AuthenticatorID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as Authenticator);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return AuthenticatorID;
        }

        public static bool operator ==(Authenticator left, Authenticator right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Authenticator left, Authenticator right)
        {
            return !Equals(left, right);
        }

        public AuthenticatorEnum ToEnum { get { return (AuthenticatorEnum)GetHashCode(); } }

        public static Authenticator ToType(int enumValue)
        {
            return ToType((AuthenticatorEnum)enumValue);
        }

        public static Authenticator ToType(AuthenticatorEnum enumValue)
        {
            switch (enumValue)
            {
                case AuthenticatorEnum.ADFS:
                    return ADFS;
                case AuthenticatorEnum.SAWPROD:
                    return SAWPROD;
                case AuthenticatorEnum.SAWTEST:
                    return SAWTEST;
                default:
                    throw new ArgumentException(string.Format("Unable to map Enum: {0}", enumValue));
            }
        }
    }

    public enum AuthenticatorEnum
    {
        ADFS = 1,
        SAWPROD = 2,
        SAWTEST = 3
    }

    public partial class AuthenticatorADFS : Authenticator
    {
        private AuthenticatorADFS(int authenticatorID, string authenticatorName, string authenticatorFullName) : base(authenticatorID, authenticatorName, authenticatorFullName) {}
        public static readonly AuthenticatorADFS Instance = new AuthenticatorADFS(1, @"ADFS", @"Washington DNR ADFS Account");
    }

    public partial class AuthenticatorSAWPROD : Authenticator
    {
        private AuthenticatorSAWPROD(int authenticatorID, string authenticatorName, string authenticatorFullName) : base(authenticatorID, authenticatorName, authenticatorFullName) {}
        public static readonly AuthenticatorSAWPROD Instance = new AuthenticatorSAWPROD(2, @"SAW-PROD", @"Secure Access Washington Account (PROD)");
    }

    public partial class AuthenticatorSAWTEST : Authenticator
    {
        private AuthenticatorSAWTEST(int authenticatorID, string authenticatorName, string authenticatorFullName) : base(authenticatorID, authenticatorName, authenticatorFullName) {}
        public static readonly AuthenticatorSAWTEST Instance = new AuthenticatorSAWTEST(3, @"SAW-TEST", @"Secure Access Washington Account (TEST)");
    }
}