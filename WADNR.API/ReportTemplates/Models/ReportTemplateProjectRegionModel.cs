using System;
using WADNR.EFModels.Entities;

namespace WADNR.API.ReportTemplates.Models
{
    public class ReportTemplateProjectRegionModel : ReportTemplateBaseModel
    {
        private Project Project { get; set; }
        private ProjectRegion ProjectRegion { get; set; }
        private DNRUplandRegion DNRUplandRegion { get; set; }

        public string Abbreviation { get; set; }
        public string Name { get; set; }

        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        public string CityStateZip
        {
            get
            {
                if (!string.IsNullOrEmpty(City)
                    && !string.IsNullOrEmpty(State)
                    && !string.IsNullOrEmpty(Zip))
                {
                    return $"{City}, {State}  {Zip}";
                }

                if (!string.IsNullOrEmpty(City) && !string.IsNullOrEmpty(State))
                    return $"{City}, {State}";

                if (!string.IsNullOrEmpty(City))
                    return City;

                if (!string.IsNullOrEmpty(State))
                    return State;

                if (!string.IsNullOrEmpty(Zip))
                    return Zip;

                return string.Empty;
            }
        }

        public string AddressDisplay => $"{(!string.IsNullOrEmpty(Address) ? Address + Environment.NewLine : string.Empty)}" +
                                        $"{CityStateZip}";

        public string Phone { get; set; }
        public string Email { get; set; }

        public ReportTemplateProjectRegionModel(ProjectRegion projectRegion)
        {
            Project = projectRegion.Project;
            ProjectRegion = projectRegion;
            DNRUplandRegion = projectRegion.DNRUplandRegion;

            Abbreviation = projectRegion.DNRUplandRegion.DNRUplandRegionAbbrev;
            Name = projectRegion.DNRUplandRegion.DNRUplandRegionName;

            Address = DNRUplandRegion.RegionAddress;
            City = DNRUplandRegion.RegionCity;
            State = DNRUplandRegion.RegionState;
            Zip = DNRUplandRegion.RegionZip;
            Phone = DNRUplandRegion.RegionPhone;
            Email = DNRUplandRegion.RegionEmail;
        }
    }
}
