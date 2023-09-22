using Microsoft.CodeAnalysis;

namespace Variety;

internal static class Diagnostics
{
    public static DiagnosticDescriptor VaryRecordMustBeReferenceType { get; } = new(
        id: "VARY001",
        title: "Vary record must be reference type",
        messageFormat: "Record {0} must be reference type in order to be [Vary]. Code generation skipped.",
        category: "Conditions",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true
    );
}