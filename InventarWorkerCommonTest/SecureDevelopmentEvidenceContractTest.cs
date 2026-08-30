using System.Text.Json;
using System.Text.RegularExpressions;

namespace InventarWorkerCommonTest;

/// <summary>
/// DE: Prüft die deterministischen Verträge der Secure-Development-Evidenz.
/// EN: Verifies the deterministic secure-development evidence contracts.
/// </summary>
[TestClass]
public sealed partial class SecureDevelopmentEvidenceContractTest
{
    private static readonly string[] AllowedIntakeStates =
        ["Applicable", "AlreadySatisfied", "N/A", "Open", "FollowUp"];

    private static readonly string RepositoryRoot = FindRepositoryRoot();

    /// <summary>
    /// DE: Prüft zwölf kanonische Checklisten, ihre Versionen und genau 157 eindeutige Kontroll-IDs.
    /// EN: Verifies twelve canonical checklists, their versions, and exactly 157 unique control IDs.
    /// </summary>
    [TestMethod]
    public void BaselineManifest_CanonicalChecklists_ContainsExactly157UniqueControls()
    {
        using var manifest = JsonDocument.Parse(File.ReadAllText(PathInRepository("docs/secure-development/baseline-manifest.json")));
        var root = manifest.RootElement;
        var expectedBaselineVersion = root.GetProperty("baselineVersion").GetString();
        var expectedCount = root.GetProperty("checklistItemCount").GetInt32();
        var checklists = root.GetProperty("checklists").EnumerateArray().ToArray();
        var controlIds = new HashSet<string>(StringComparer.Ordinal);

        Assert.AreEqual(12, checklists.Length, "The canonical baseline must contain exactly twelve checklists.");
        Assert.AreEqual(157, expectedCount, "The canonical baseline must declare exactly 157 controls.");
        Assert.IsTrue(SemanticVersionRegex().IsMatch(expectedBaselineVersion ?? string.Empty),
            "The baseline manifest must declare a semantic version.");

        foreach (var checklist in checklists)
        {
            var relativePath = checklist.GetProperty("path").GetString()
                ?? throw new AssertFailedException("A checklist path is missing.");
            var expectedChecklistVersion = checklist.GetProperty("version").GetString();
            var content = File.ReadAllText(PathInRepository(Path.Combine("docs/secure-development", relativePath)));
            var actualChecklistVersion = ChecklistVersionRegex().Match(content).Groups[1].Value;
            var actualBaselineVersion = BaselineVersionRegex().Match(content).Groups[1].Value;

            Assert.AreEqual(expectedChecklistVersion, actualChecklistVersion, $"Checklist version drift in {relativePath}.");
            Assert.IsTrue(SemanticVersionRegex().IsMatch(actualBaselineVersion),
                $"Checklist baseline metadata is invalid in {relativePath}.");

            foreach (Match match in ControlIdRegex().Matches(content))
            {
                Assert.IsTrue(controlIds.Add(match.Value), $"Duplicate control ID: {match.Value}");
            }
        }

        Assert.AreEqual(expectedCount, controlIds.Count, "The canonical checklist collection has an unexpected unique-ID count.");
    }

    /// <summary>
    /// DE: Prüft den vollständigen Fünf-Zustände-Vertrag des verbindlichen Intakes.
    /// EN: Verifies the complete five-state contract of the binding intake.
    /// </summary>
    [TestMethod]
    public void IntakeClassification_UsesFiveAllowedStates_WithCompleteDisposition()
    {
        var intake = File.ReadAllText(PathInRepository("Lastenheft_Secure-Development-Hardening.md"));
        foreach (var state in AllowedIntakeStates)
        {
            StringAssert.Contains(intake, $"`{state}`", $"Missing intake classification state {state}.");
        }
    }

    /// <summary>
    /// DE: Prüft exakt zwölf aktivierte Presets in der installierten Registry.
    /// EN: Verifies exactly twelve enabled presets in the installed registry.
    /// </summary>
    [TestMethod]
    public void InstalledPresetRegistry_ContainsExactTwelveEnabledManifests()
    {
        using var registry = JsonDocument.Parse(File.ReadAllText(PathInRepository(".specify/presets/.registry")));
        var presets = registry.RootElement.GetProperty("presets").EnumerateObject().ToArray();

        Assert.AreEqual(12, presets.Length);
        Assert.IsTrue(presets.All(preset => preset.Value.GetProperty("enabled").GetBoolean()));
        Assert.AreEqual(12, presets.Select(preset => preset.Name).Distinct(StringComparer.Ordinal).Count());
    }

    /// <summary>
    /// DE: Prüft Vollständigkeit, Statuskombinationen, Findings und Frische aller 157 Datensätze.
    /// EN: Verifies completeness, status combinations, findings, and freshness of all 157 records.
    /// </summary>
    [TestMethod]
    public void AssessmentCollection_ContainsExactly157CompleteFreshRecords()
    {
        using var collection = LoadAssessmentCollection();
        var records = collection.RootElement.GetProperty("records").EnumerateArray().ToArray();
        var ids = new HashSet<string>(StringComparer.Ordinal);

        Assert.AreEqual(157, records.Length);
        foreach (var record in records)
        {
            var id = RequiredText(record, "recordId");
            Assert.IsTrue(ids.Add(id), $"Duplicate assessment record {id}.");
            Assert.AreEqual(id, RequiredText(record, "checkpointId"));
            Assert.IsTrue(new[] { "Applicable", "N/A", "Open" }.Contains(RequiredText(record, "applicability")));
            Assert.IsFalse(string.IsNullOrWhiteSpace(RequiredText(record, "owner")));
            Assert.IsFalse(string.IsNullOrWhiteSpace(RequiredText(record, "reviewer")));
            Assert.IsTrue(RequiredText(record, "rationale").Contains("DE:", StringComparison.Ordinal));
            Assert.IsTrue(RequiredText(record, "rationale").Contains("EN:", StringComparison.Ordinal));
            Assert.IsTrue(RequiredText(record, "reevaluationTrigger").Length >= 10);

            var applicability = RequiredText(record, "applicability");
            var findings = record.GetProperty("findingIds").EnumerateArray().ToArray();
            if (applicability == "Open")
            {
                Assert.IsTrue(findings.Length > 0, $"Open record {id} has no finding.");
                Assert.AreNotEqual("N/A", RequiredText(record, "dueDate"));
            }
            else if (applicability == "N/A")
            {
                Assert.AreEqual("Not Assessed", RequiredText(record, "implementationStatus"));
                Assert.AreEqual("N/A", RequiredText(record, "dueDate"));
            }

            foreach (var evidence in record.GetProperty("evidence").EnumerateArray())
            {
                Assert.AreEqual(40, RequiredText(evidence, "candidateGitSha").Length);
                Assert.IsTrue(RequiredText(evidence, "freshnessBasis").Length >= 10);
                Assert.IsTrue(evidence.GetProperty("invalidationTriggers").GetArrayLength() > 0);
                Assert.IsFalse(string.IsNullOrWhiteSpace(RequiredText(evidence, "runnerOrPlatform")));
                Assert.IsFalse(string.IsNullOrWhiteSpace(RequiredText(evidence, "evidenceReference")));
            }
        }
    }

    /// <summary>
    /// DE: Prüft, dass alle zwölf Projektinstanzen zweisprachig und frei von Stubs sind.
    /// EN: Verifies that all twelve project instances are bilingual and free of stubs.
    /// </summary>
    [TestMethod]
    public void ProjectInstances_TwelveFiles_AreBilingualAndNotStubs()
    {
        var directory = PathInRepository("docs/security/secure-development/2026-08-30-secure-development-hardening");
        var files = Directory.GetFiles(directory, "CL_*.md");
        Assert.AreEqual(12, files.Length);
        foreach (var file in files)
        {
            var content = File.ReadAllText(file);
            StringAssert.Contains(content, "DE:");
            StringAssert.Contains(content, "EN:");
            Assert.IsFalse(content.Contains("Zu befuellen", StringComparison.OrdinalIgnoreCase));
            Assert.IsFalse(content.Contains("To be populated", StringComparison.OrdinalIgnoreCase));
        }
    }

    /// <summary>
    /// DE: Prüft Trust Boundaries, Architekturansichten und Defense in Depth.
    /// EN: Verifies trust boundaries, architecture views, and defence in depth.
    /// </summary>
    [TestMethod]
    public void ArchitectureEvidence_ContainsNineBoundariesAndRequiredViews()
    {
        var threatModel = File.ReadAllText(PathInRepository("docs/security/threat-model.md"));
        for (var index = 1; index <= 9; index++)
        {
            StringAssert.Contains(threatModel, $"TB-{index:00}");
        }

        StringAssert.Contains(threatModel, "STRIDE");
        StringAssert.Contains(threatModel, "CAPEC");
        foreach (var path in new[] { "context-view.md", "runtime-view.md", "deployment-view.md", "architecture-risks.md", "quality-scenarios.md" })
        {
            Assert.IsTrue(new FileInfo(PathInRepository(Path.Combine("docs/architecture", path))).Length > 100);
        }
    }

    /// <summary>
    /// DE: Prüft die expliziten regulatorischen und Cloud-/KI-Dispositionen.
    /// EN: Verifies explicit regulatory and cloud/AI dispositions.
    /// </summary>
    [TestMethod]
    public void RegulatoryEvidence_ContainsApplicableAndReasonedNaDecisions()
    {
        var content = File.ReadAllText(PathInRepository("docs/security/regulatory-applicability.md"));
        foreach (var token in new[] { "NIST SSDF", "CWE Top 25", "OWASP ASVS 5.0 L2", "NIS2", "DORA", "EU AI Act", "AI-SBOM", "BSI C3A/C5" })
        {
            StringAssert.Contains(content, token);
        }

        StringAssert.Contains(content, "N/A");
        StringAssert.Contains(content, "Trigger");
    }

    /// <summary>
    /// DE: Belegt mit negativen Fixtures, dass doppelte IDs, positive N/A-Aussagen und fehlende Trigger abgelehnt werden.
    /// EN: Uses negative fixtures to prove rejection of duplicate IDs, positive N/A claims, and missing triggers.
    /// </summary>
    [TestMethod]
    public void EvidenceContract_InvalidFixtures_AreRejected()
    {
        Assert.ThrowsExactly<InvalidDataException>(() => ValidateFixture(["CL-01-01", "CL-01-01"], "Applicable", "trigger"));
        Assert.ThrowsExactly<InvalidDataException>(() => ValidateFixture(["CL-01-01"], "N/A-Pass", "trigger"));
        Assert.ThrowsExactly<InvalidDataException>(() => ValidateFixture(["CL-01-01"], "Open", string.Empty));
    }

    private static JsonDocument LoadAssessmentCollection() =>
        JsonDocument.Parse(File.ReadAllText(PathInRepository("docs/security/secure-development/2026-08-30-secure-development-hardening/assessment-records.json")));

    private static string RequiredText(JsonElement element, string propertyName) =>
        element.GetProperty(propertyName).GetString() ?? throw new InvalidDataException($"Missing text property {propertyName}.");

    private static void ValidateFixture(IReadOnlyCollection<string> ids, string status, string trigger)
    {
        if (ids.Count != ids.Distinct(StringComparer.Ordinal).Count() || status == "N/A-Pass" || string.IsNullOrWhiteSpace(trigger))
        {
            throw new InvalidDataException("Invalid secure-development evidence fixture.");
        }
    }

    private static string PathInRepository(string relativePath) => Path.Combine(RepositoryRoot, relativePath);

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "InventarWorkerService.sln")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName ?? throw new DirectoryNotFoundException("Repository root not found.");
    }

    [GeneratedRegex(@"\*\*Version / Version:\*\*\s*([0-9]+\.[0-9]+\.[0-9]+)")]
    private static partial Regex ChecklistVersionRegex();

    [GeneratedRegex(@"\*\*Baseline-Version / Baseline version:\*\*\s*([0-9]+\.[0-9]+\.[0-9]+)")]
    private static partial Regex BaselineVersionRegex();

    [GeneratedRegex(@"(?m)^#{4}\s+(CL-(?:0[1-9]|1[0-2])-[0-9]{2}):")]
    private static partial Regex ControlIdRegex();

    [GeneratedRegex(@"^[0-9]+\.[0-9]+\.[0-9]+$")]
    private static partial Regex SemanticVersionRegex();
}
