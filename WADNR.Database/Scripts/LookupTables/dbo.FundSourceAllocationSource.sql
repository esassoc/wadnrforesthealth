merge into dbo.FundSourceAllocationSource as Target
using (values
(1, 'State', 'State', 10),
(2, 'StateGFS', 'State - GFS', 20),
(3, 'StateCapital', 'State - Capital', 30),
(4, 'StateOther', 'State - Other', 40),
(5, 'FederalWSFM', 'Federal - WSFM', 50),
(6, 'FederaNFPWUINonFedWUI', 'Federal - NFP WUI (Non-fed WUI)', 60),
(7, 'FederalCWDG', 'Federal - CWDG', 70),
(8, 'FederalLSR', 'Federal - LSR', 80),
(9, 'FederalBipartisanInfrastructureLaw', 'Federal - Bipartisan Infrastructure Law', 90),
(10, 'FederalInflationReductionAct', 'Federal - Inflation Reduction Act', 100),
(11, 'FederalConsolidatedPaymentFundSource', 'Federal - Consolidated Payment FundSource', 110),
(12, 'FederalCooperativeAgreements', 'Federal - Cooperative Agreements', 120),
(13, 'FederalDisasterRelief', 'Federal - Disaster Relief', 130),
(14, 'FederalForestHealthProtection', 'Federal - Forest Health Protection', 140),
(15, 'FederalForestLegacy', 'Federal - Forest Legacy', 150),
(16, 'FederalWesternBarkBeetle', 'Federal - Western Bark Beetle', 160),
(17, 'FederalFEMA', 'Federal - FEMA', 170),
(18, 'FederalBLM', 'Federal - BLM', 180),
(19, 'FederalOther', 'Federal - Other', 190),
(20, 'Private', 'Private', 200),
(21, 'Other', 'Other', 210)
) as Source (FundSourceAllocationSourceID, FundSourceAllocationSourceName, FundSourceAllocationSourceDisplayName, SortOrder)
on Target.FundSourceAllocationSourceID = Source.FundSourceAllocationSourceID
when matched then
    update set
        FundSourceAllocationSourceName = Source.FundSourceAllocationSourceName,
        FundSourceAllocationSourceDisplayName = Source.FundSourceAllocationSourceDisplayName,
        SortOrder = Source.SortOrder
when not matched by target then
    insert (FundSourceAllocationSourceID, FundSourceAllocationSourceName, FundSourceAllocationSourceDisplayName, SortOrder)
    values (FundSourceAllocationSourceID, FundSourceAllocationSourceName, FundSourceAllocationSourceDisplayName, SortOrder)
when not matched by source then
    delete;