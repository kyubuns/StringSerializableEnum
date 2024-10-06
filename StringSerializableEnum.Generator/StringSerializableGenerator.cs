﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Linq;
using System.Text;

namespace StringSerializableEnum.Generator;

[Generator]
public class StringSerializableGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var enumDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) =>
                {
                    return
                        node is EnumDeclarationSyntax enumDeclaration
                        && enumDeclaration.AttributeLists
                            .SelectMany(attributeList => attributeList.Attributes)
                            .Any(attribute => attribute.Name.ToString() == "StringSerializable");
                },
                transform: static (context, _) =>
                {
                    var enumDeclaration = (EnumDeclarationSyntax) context.Node;
                    var semanticModel = context.SemanticModel;

                    var enumSymbol = semanticModel.GetDeclaredSymbol(enumDeclaration);
                    if (enumSymbol is not INamedTypeSymbol namedTypeSymbol) return null;

                    var enumName = namedTypeSymbol.Name;

                    var members = namedTypeSymbol.GetMembers().OfType<IFieldSymbol>()
                        .Where(f => f.ConstantValue != null)
                        .Select(f => f.Name)
                        .ToArray();

                    return new EnumInfo(enumName, enumSymbol.ContainingNamespace?.ToDisplayString(), members);
                }
            )
            .Where(static m => m is not null);

        context.RegisterSourceOutput(enumDeclarations, (spc, enumInfo) =>
        {
            if (enumInfo != null)
            {
                spc.AddSource($"{enumInfo.EnumName}.g.cs", SourceText.From(GenerateCode(enumInfo), Encoding.UTF8));
            }
        });
    }

    private static string GenerateCode(EnumInfo enumInfo)
    {
        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Linq;");
        sb.AppendLine("using UnityEngine;");
        sb.AppendLine();

        if (enumInfo.NamespaceName != null)
        {
            sb.AppendLine($"namespace {enumInfo.NamespaceName}");
            sb.AppendLine("{");
        }

        sb.AppendLine("    [Serializable]");
        sb.AppendLine($"    public struct StringSerializable{enumInfo.EnumName}");
        sb.AppendLine("    {");
        sb.AppendLine("        [SerializeField] private string stringValue;");
        sb.AppendLine();
        sb.AppendLine($"        private {enumInfo.EnumName}? _cached;");
        sb.AppendLine();
        sb.AppendLine($"        public {enumInfo.EnumName} Value");
        sb.AppendLine("        {");
        sb.AppendLine("            get");
        sb.AppendLine("            {");
        sb.AppendLine("                if (_cached == null)");
        sb.AppendLine("                {");
        sb.AppendLine("                    _cached = stringValue switch");
        sb.AppendLine("                    {");
        foreach (var member in enumInfo.Members)
        {
            sb.AppendLine($"                        \"{member}\" => {enumInfo.EnumName}.{member},");
        }
        sb.AppendLine($"                    _ => throw new ArgumentOutOfRangeException($\"\\\"{{stringValue}}\\\" is not a valid value for {enumInfo.EnumName}\")");
        sb.AppendLine("                    };");
        sb.AppendLine("                }");
        sb.AppendLine("                return _cached.Value;");
        sb.AppendLine("            }");
        sb.AppendLine("            set");
        sb.AppendLine("            {");
        sb.AppendLine("                _cached = value;");
        sb.AppendLine("                stringValue = value switch");
        sb.AppendLine("                {");
        foreach (var member in enumInfo.Members)
        {
            sb.AppendLine($"                    {enumInfo.EnumName}.{member} => \"{member}\",");
        }
        sb.AppendLine($"                    _ => throw new ArgumentOutOfRangeException($\"\\\"{{stringValue}}\\\" is not a valid value for {enumInfo.EnumName}\")");
        sb.AppendLine("                };");
        sb.AppendLine("            }");
        sb.AppendLine("        }");
        sb.AppendLine("    }");

        sb.AppendLine();

        sb.AppendLine("#if UNITY_EDITOR");
        sb.AppendLine($"    [UnityEditor.CustomPropertyDrawer(typeof(StringSerializable{enumInfo.EnumName}))]");
        sb.AppendLine($"    public class StringSerializable{enumInfo.EnumName}Drawer : UnityEditor.PropertyDrawer");
        sb.AppendLine("    {");
        sb.AppendLine("        private static readonly string[] Values =");
        sb.AppendLine("        {");
        foreach (var member in enumInfo.Members)
        {
            sb.AppendLine($"            \"{member}\",");
        }
        sb.AppendLine("        };");
        sb.AppendLine();
        sb.AppendLine("        public override void OnGUI(UnityEngine.Rect position, UnityEditor.SerializedProperty property, UnityEngine.GUIContent label)");
        sb.AppendLine("        {");
        sb.AppendLine("            using var propertyScope = new UnityEditor.EditorGUI.PropertyScope(position, label, property);");
        sb.AppendLine("            position = UnityEditor.EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);");
        sb.AppendLine("            var p = property.FindPropertyRelative(\"stringValue\");");
        sb.AppendLine("            var currentValue = p.stringValue;");
        sb.AppendLine("            var selectedIndex = Array.FindIndex(Values, x => x == currentValue);");
        sb.AppendLine("            if (selectedIndex == -1)");
        sb.AppendLine("            {");
        sb.AppendLine("                var warningWidth = UnityEditor.EditorGUIUtility.singleLineHeight;");
        sb.AppendLine("                var popupPosition = new UnityEngine.Rect(position.x, position.y, position.width - warningWidth - UnityEditor.EditorGUIUtility.standardVerticalSpacing, position.height);");
        sb.AppendLine("                var warningPosition = new UnityEngine.Rect(position.x + position.width - warningWidth, position.y, position.width, position.height);");
        sb.AppendLine("                var newValue = UnityEditor.EditorGUI.Popup(popupPosition, 0, new[] { $\"Invalid Value: {currentValue}\" }.Concat(Values).ToArray());");
        sb.AppendLine("                if (newValue > 0)");
        sb.AppendLine("                {");
        sb.AppendLine("                    p.stringValue = Values[newValue - 1];");
        sb.AppendLine("                }");
        sb.AppendLine("                UnityEditor.EditorGUI.LabelField(warningPosition, new UnityEngine.GUIContent(\"\", UnityEditor.EditorGUIUtility.IconContent(\"console.erroricon.sml\").image, $\"Invalid Value: {currentValue}\"));");
        sb.AppendLine("            }");
        sb.AppendLine("            else");
        sb.AppendLine("            {");
        sb.AppendLine("                p.stringValue = Values[UnityEditor.EditorGUI.Popup(position, selectedIndex, Values)];");
        sb.AppendLine("            }");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine("#endif");

        if (enumInfo.NamespaceName != null)
        {
            sb.AppendLine("}");
        }

        return sb.ToString();
    }

    private class EnumInfo
    {
        public EnumInfo(string enumName, string? namespaceName, string[] members)
        {
            EnumName = enumName;
            NamespaceName = namespaceName;
            Members = members;
        }

        public string EnumName { get; }
        public string? NamespaceName { get; }
        public string[] Members { get; }
    }
}
