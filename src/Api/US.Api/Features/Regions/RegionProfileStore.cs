using US.SharedKernel.Contracts.Regions;
using US.SharedKernel.Enums;

namespace US.Api.Features.Regions;

public sealed class RegionProfileStore : IRegionProfileStore
{
    private static readonly IReadOnlyList<RegionProfileDto> Regions =
    [
        new()
        {
            Code = "drc",
            Name = "Eastern DRC",
            Description = "Community accountability and reparative justice workflows for mining, displacement, and service-delivery commitments.",
            Languages =
            [
                new LanguageOptionDto { Code = "en", Name = "English" },
                new LanguageOptionDto { Code = "fr", Name = "French", IsDefault = true },
                new LanguageOptionDto { Code = "sw", Name = "Swahili" }
            ],
            PriorityIssueTypes = [IssueType.BrokenPromise, IssueType.ResourceConflict, IssueType.ServiceGap, IssueType.EnvironmentalHarm],
            RegionalBodies = ["African Union", "EAC"],
            EscalationPathways = ["Local peace committee", "Provincial authority", "OSF partner review", "Regional body briefing"]
        },
        new()
        {
            Code = "sahel",
            Name = "Sahel",
            Description = "Low-bandwidth civic evidence workflows for communities affected by insecurity, displacement, and governance exclusion.",
            Languages =
            [
                new LanguageOptionDto { Code = "fr", Name = "French", IsDefault = true },
                new LanguageOptionDto { Code = "ar", Name = "Arabic" },
                new LanguageOptionDto { Code = "en", Name = "English" }
            ],
            PriorityIssueTypes = [IssueType.SecurityConcern, IssueType.GovernanceFailure, IssueType.ResourceConflict],
            RegionalBodies = ["African Union", "ECOWAS"],
            EscalationPathways = ["Community validator", "Civil society partner", "ECOWAS policy channel"]
        },
        new()
        {
            Code = "sudan",
            Name = "Sudan",
            Description = "Anonymity-aware reporting for communities documenting displacement, local harms, and recovery needs.",
            Languages =
            [
                new LanguageOptionDto { Code = "ar", Name = "Arabic", IsDefault = true },
                new LanguageOptionDto { Code = "en", Name = "English" }
            ],
            PriorityIssueTypes = [IssueType.SecurityConcern, IssueType.ServiceGap, IssueType.CommunityResilience],
            RegionalBodies = ["African Union"],
            EscalationPathways = ["Trusted validator", "Humanitarian partner", "OSF legal or advocacy review"]
        },
        new()
        {
            Code = "mozambique",
            Name = "Mozambique",
            Description = "Localized reporting and accountability workflows for recovery, resource governance, and community resilience.",
            Languages =
            [
                new LanguageOptionDto { Code = "pt", Name = "Portuguese", IsDefault = true },
                new LanguageOptionDto { Code = "en", Name = "English" }
            ],
            PriorityIssueTypes = [IssueType.ResourceConflict, IssueType.EnvironmentalHarm, IssueType.RecoveryNeed, IssueType.CommunityResilience],
            RegionalBodies = ["African Union", "SADC"],
            EscalationPathways = ["Local partner", "District authority", "SADC-aligned advocacy brief"]
        }
    ];

    public Task<IReadOnlyList<RegionProfileDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(Regions);
    }
}
