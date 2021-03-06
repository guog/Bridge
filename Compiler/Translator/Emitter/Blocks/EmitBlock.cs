using Bridge.Contract;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Object.Net.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Bridge.Translator
{
    public class EmitBlock : AbstractEmitterBlock
    {
        public EmitBlock(IEmitter emitter)
            : base(emitter, null)
        {
            this.Emitter = emitter;
        }

        protected virtual StringBuilder GetOutputForType(ITypeInfo typeInfo, string name)
        {
            string module = null;

            if (typeInfo != null && typeInfo.Module != null)
            {
                module = typeInfo.Module;
            }
            else if (this.Emitter.AssemblyInfo.Module != null)
            {
                module = this.Emitter.AssemblyInfo.Module;
            }

            var fileName = typeInfo != null ? typeInfo.FileName : name;

            if (fileName.IsEmpty() && this.Emitter.AssemblyInfo.OutputBy != OutputBy.Project)
            {
                switch (this.Emitter.AssemblyInfo.OutputBy)
                {
                    case OutputBy.ClassPath:
                        fileName = typeInfo.Type.FullName;
                        break;

                    case OutputBy.Class:
                        fileName = this.GetIteractiveClassPath(typeInfo);
                        break;

                    case OutputBy.Module:
                        fileName = module;
                        break;

                    case OutputBy.NamespacePath:
                    case OutputBy.Namespace:
                        fileName = this.GetNamespaceFilename(typeInfo);
                        break;

                    default:
                        break;
                }

                var isPathRelated = this.Emitter.AssemblyInfo.OutputBy == OutputBy.ClassPath ||
                                    this.Emitter.AssemblyInfo.OutputBy == OutputBy.NamespacePath;

                if (fileName.IsNotEmpty() && isPathRelated)
                {
                    fileName = fileName.Replace('.', System.IO.Path.DirectorySeparatorChar);

                    if (this.Emitter.AssemblyInfo.StartIndexInName > 0)
                    {
                        fileName = fileName.Substring(this.Emitter.AssemblyInfo.StartIndexInName);
                    }
                }
            }

            if (fileName.IsEmpty() && this.Emitter.AssemblyInfo.FileName != null)
            {
                fileName = this.Emitter.AssemblyInfo.FileName;
            }

            if (fileName.IsEmpty())
            {
                fileName = AssemblyInfo.DEFAULT_FILENAME;
            }

            // Apply lowerCamelCase to filename if set up in bridge.json (or left default)
            if (this.Emitter.AssemblyInfo.FileNameCasing == FileNameCaseConvert.CamelCase)
            {
                var sepList = new string[] { ".", System.IO.Path.DirectorySeparatorChar.ToString(), "\\", "/" };

                // Populate list only with needed separators, as usually we will never have all four of them
                var neededSepList = new List<string>();

                foreach (var separator in sepList)
                {
                    if (fileName.Contains(separator.ToString()) && !neededSepList.Contains(separator))
                    {
                        neededSepList.Add(separator);
                    }
                }

                // now, separating the filename string only by the used separators, apply lowerCamelCase
                if (neededSepList.Count > 0)
                {
                    foreach (var separator in neededSepList)
                    {
                        var stringList = new List<string>();

                        foreach (var str in fileName.Split(separator[0]))
                        {
                            stringList.Add(str.ToLowerCamelCase());
                        }

                        fileName = stringList.Join(separator);
                    }
                }
                else
                {
                    fileName = fileName.ToLowerCamelCase();
                }
            }

            // Append '.js' extension to file name at translator.Outputs level: this aids in code grouping on files
            // when filesystem is not case sensitive.
            if (!fileName.ToLower().EndsWith("." + Bridge.Translator.AssemblyInfo.JAVASCRIPT_EXTENSION))
            {
                fileName += "." + Bridge.Translator.AssemblyInfo.JAVASCRIPT_EXTENSION;
            }

            switch (this.Emitter.AssemblyInfo.FileNameCasing)
            {
                case FileNameCaseConvert.Lowercase:
                    fileName = fileName.ToLower();
                    break;

                default:
                    var lcFileName = fileName.ToLower();

                    // Find a file name that matches (case-insensitive) and use it as file name (if found)
                    // The used file name will use the same casing of the existing one.
                    foreach (var existingFile in this.Emitter.Outputs.Keys)
                    {
                        if (lcFileName == existingFile.ToLower())
                        {
                            fileName = existingFile;
                        }
                    }

                    break;
            }

            IEmitterOutput output = null;

            if (this.Emitter.Outputs.ContainsKey(fileName))
            {
                output = this.Emitter.Outputs[fileName];
            }
            else
            {
                output = new EmitterOutput(fileName);
                this.Emitter.Outputs.Add(fileName, output);
            }

            this.Emitter.EmitterOutput = output;

            if (module == null)
            {
                return output.NonModuletOutput;
            }

            if (module == "")
            {
                module = Bridge.Translator.AssemblyInfo.DEFAULT_FILENAME;
            }

            if (output.ModuleOutput.ContainsKey(module))
            {
                this.Emitter.CurrentDependencies = output.ModuleDependencies[module];
                return output.ModuleOutput[module];
            }

            StringBuilder moduleOutput = new StringBuilder();
            output.ModuleOutput.Add(module, moduleOutput);
            var dependencies = new List<IPluginDependency>();
            output.ModuleDependencies.Add(module, dependencies);

            if (typeInfo != null && typeInfo.Dependencies.Count > 0)
            {
                dependencies.AddRange(typeInfo.Dependencies);
            }

            this.Emitter.CurrentDependencies = dependencies;

            return moduleOutput;
        }

        private string GetNamespaceFilename(ITypeInfo typeInfo)
        {
            var cas = this.Emitter.BridgeTypes.Get(typeInfo.Key).TypeDefinition.CustomAttributes;

            // Search for an 'NamespaceAttribute' entry
            foreach (var ca in cas)
            {
                if (ca.AttributeType.Name == "NamespaceAttribute" &&
                    ca.ConstructorArguments.Count > 0 &&
                    ca.ConstructorArguments[0].Value is string &&
                    !string.IsNullOrWhiteSpace(ca.ConstructorArguments[0].Value.ToString()))
                {
                    return ca.ConstructorArguments[0].Value.ToString();
                }
            }

            return typeInfo.Namespace;
        }

        /// <summary>
        /// Gets class path iterating until its root class, writing something like this on a 3-level nested class:
        /// RootClass.Lv1ParentClass.Lv2ParentClass.Lv3NestedClass
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <returns></returns>
        private string GetIteractiveClassPath(ITypeInfo typeInfo)
        {
            var fullClassName = typeInfo.Name;
            var maxIterations = 100;
            var curIterations = 0;
            var parent = typeInfo.ParentType;

            while (parent != null && curIterations++ < maxIterations)
            {
                fullClassName = parent.Name + "." + fullClassName;
                parent = parent.ParentType;
            }

            // This should never happen but, just to be sure...
            if (curIterations >= maxIterations)
            {
                throw new EmitterException(typeInfo.TypeDeclaration, "Iteration count for class '" + typeInfo.Type.FullName + "' exceeded " +
                    maxIterations + " depth iterations until root class!");
            }

            return fullClassName;
        }

        protected override void DoEmit()
        {
            this.Emitter.Writers = new Stack<IWriter>();
            this.Emitter.Outputs = new EmitterOutputs();
            var metas = new Dictionary<IType, JObject>();

            this.Emitter.Translator.Plugins.BeforeTypesEmit(this.Emitter, this.Emitter.Types);
            this.Emitter.ReflectableTypes = this.GetReflectableTypes();
            var reflectedTypes = this.Emitter.ReflectableTypes;

            foreach (var type in this.Emitter.Types)
            {
                this.Emitter.Translator.Plugins.BeforeTypeEmit(this.Emitter, type);

                this.Emitter.Translator.EmitNode = type.TypeDeclaration;
                if (type.IsObjectLiteral)
                {
                    this.Emitter.Translator.Plugins.AfterTypeEmit(this.Emitter, type);
                    continue;
                }

                this.Emitter.InitEmitter();

                ITypeInfo typeInfo;

                if (this.Emitter.TypeInfoDefinitions.ContainsKey(type.Key))
                {
                    typeInfo = this.Emitter.TypeInfoDefinitions[type.Key];

                    type.Module = typeInfo.Module;
                    type.FileName = typeInfo.FileName;
                    type.Dependencies = typeInfo.Dependencies;
                    typeInfo = type;
                }
                else
                {
                    typeInfo = type;
                }

                this.Emitter.Output = this.GetOutputForType(typeInfo, null);
                this.Emitter.TypeInfo = type;

                if (this.Emitter.TypeInfo.Module != null)
                {
                    this.Indent();
                }

                new ClassBlock(this.Emitter, this.Emitter.TypeInfo).Emit();
                this.Emitter.Translator.Plugins.AfterTypeEmit(this.Emitter, type);
            }

            foreach (var type in this.Emitter.Types)
            {
                var typeDef = type.Type.GetDefinition();
                bool isGlobal = false;
                if (typeDef != null)
                {
                    isGlobal = typeDef.Attributes.Any(a => a.AttributeType.FullName == "Bridge.GlobalMethodsAttribute" || a.AttributeType.FullName == "Bridge.MixinAttribute");
                }

                if (reflectedTypes.Any(t => t == type.Type) || isGlobal)
                {
                    continue;
                }

                var meta = MetadataUtils.ConstructTypeMetadata(typeDef, this.Emitter, true, type.TypeDeclaration.GetParent<SyntaxTree>());

                if (meta != null)
                {
                    metas.Add(type.Type, meta);
                }
            }

            foreach (var reflectedType in reflectedTypes)
            {
                var typeDef = reflectedType.GetDefinition();
                JObject meta = null;
                if (typeDef != null)
                {
                    var tInfo = this.Emitter.Types.FirstOrDefault(t => t.Type == reflectedType);
                    SyntaxTree tree = null;

                    if (tInfo != null && tInfo.TypeDeclaration != null)
                    {
                        tree = tInfo.TypeDeclaration.GetParent<SyntaxTree>();
                    }
                    meta = MetadataUtils.ConstructTypeMetadata(reflectedType.GetDefinition(), this.Emitter, false, tree);
                }
                else
                {
                    meta = MetadataUtils.ConstructITypeMetadata(reflectedType, this.Emitter);
                }

                if (meta != null)
                {
                    metas.Add(reflectedType, meta);
                }
            }

            var lastOutput = this.Emitter.Output;
            var output = this.Emitter.AssemblyInfo.Reflection.Output;

            if (!string.IsNullOrEmpty(output))
            {
                this.Emitter.Output = this.GetOutputForType(null, output);
                this.Emitter.MetaDataOutputName = this.Emitter.EmitterOutput.FileName;
            }
            var scriptableAttributes = MetadataUtils.GetScriptableAttributes(this.Emitter.Resolver.Compilation.MainAssembly.AssemblyAttributes, this.Emitter, null).ToList();

            if (metas.Count > 0 || scriptableAttributes.Count > 0)
            {
                this.WriteNewLine();
            }

            foreach (var meta in metas)
            {
                var metaData = meta.Value;
                string typeArgs = "";

                if (meta.Key.TypeArguments.Count > 0)
                {
                    StringBuilder arr_sb = new StringBuilder();
                    var comma = false;
                    foreach (var typeArgument in meta.Key.TypeArguments)
                    {
                        if (comma)
                        {
                            arr_sb.Append(", ");
                        }

                        arr_sb.Append(typeArgument.Name);
                        comma = true;
                    }

                    typeArgs = arr_sb.ToString();
                }

                this.Write(string.Format("Bridge.setMetadata({0}, function ({2}) {{ return {1}; }});", BridgeTypes.ToJsName(meta.Key, this.Emitter, true), metaData.ToString(Formatting.None), typeArgs));
                this.WriteNewLine();
            }
            
            if (scriptableAttributes.Count > 0)
            {
                JArray attrArr = new JArray();
                foreach (var a in scriptableAttributes)
                {
                    attrArr.Add(MetadataUtils.ConstructAttribute(a, null, this.Emitter));
                }

                this.Write(string.Format("$asm.attr= {0};", attrArr.ToString(Formatting.None)));
                this.WriteNewLine();
            }

            this.Emitter.Output = lastOutput;

            //this.RemovePenultimateEmptyLines(true);

            this.Emitter.Translator.Plugins.AfterTypesEmit(this.Emitter, this.Emitter.Types);
        }

        private IType[] GetReflectableTypes()
        {
            var config = this.Emitter.AssemblyInfo.Reflection;
            var configInternal = ((AssemblyInfo)this.Emitter.AssemblyInfo).ReflectionInternal;

            bool? enable = config.Enabled.HasValue ? config.Enabled : (configInternal.Enabled.HasValue ? configInternal.Enabled : null);
            TypeAccessibility? typeAccessibility = config.TypeAccessibility.HasValue ? config.TypeAccessibility : (configInternal.TypeAccessibility.HasValue ? configInternal.TypeAccessibility : null);
            string filter = !string.IsNullOrEmpty(config.Filter) ? config.Filter : (!string.IsNullOrEmpty(configInternal.Filter) ? configInternal.Filter : null);

            var hasSettings = !string.IsNullOrEmpty(config.Filter) ||
                              config.MemberAccessibility.HasValue ||
                              config.TypeAccessibility.HasValue ||
                              !string.IsNullOrEmpty(configInternal.Filter) ||
                              configInternal.MemberAccessibility.HasValue ||
                              configInternal.TypeAccessibility.HasValue;

            if (enable.HasValue && !enable.Value)
            {
                return new IType[0];
            }

            if (enable.HasValue && enable.Value && !hasSettings)
            {
                this.Emitter.IsAnonymousReflectable = true;
            }

            if (typeAccessibility.HasValue)
            {
                this.Emitter.IsAnonymousReflectable = typeAccessibility.Value.HasFlag(TypeAccessibility.Anonymous);
            }

            List<IType> reflectTypes = new List<IType>();

            foreach (var bridgeType in this.Emitter.BridgeTypes)
            {
                var result = false;
                var type = bridgeType.Value.Type;
                var thisAssembly = bridgeType.Value.TypeInfo != null;

                if (enable.HasValue && enable.Value && !hasSettings && thisAssembly)
                {
                    result = true;
                }

                var typeDef = type.GetDefinition();

                if (typeDef != null)
                {
                    var isGlobal = typeDef.Attributes.Any(a => a.AttributeType.FullName == "Bridge.GlobalMethodsAttribute" || a.AttributeType.FullName == "Bridge.MixinAttribute");
                    if (isGlobal)
                    {
                        continue;
                    }

                    var attr = typeDef.Attributes.FirstOrDefault(a => a.AttributeType.FullName == "Bridge.ReflectableAttribute");

                    if (attr != null)
                    {
                        if (attr.PositionalArguments.Count == 0)
                        {
                            reflectTypes.Add(type);
                            continue;
                        }

                        var value = attr.PositionalArguments.First().ConstantValue;

                        if (!(value is bool) || (bool)value)
                        {
                            reflectTypes.Add(type);
                        }

                        continue;
                    }
                }

                if (typeAccessibility.HasValue)
                {
                    result = false;

                    if (typeAccessibility.Value.HasFlag(TypeAccessibility.All))
                    {
                        result = true;
                    }

                    if (typeAccessibility.Value.HasFlag(TypeAccessibility.Anonymous) && type.Kind == TypeKind.Anonymous)
                    {
                        result = true;
                    }

                    if (typeAccessibility.Value.HasFlag(TypeAccessibility.NonAnonymous) && type.Kind != TypeKind.Anonymous)
                    {
                        result = true;
                    }

                    if (typeAccessibility.Value.HasFlag(TypeAccessibility.NonPrivate) && (typeDef == null || !typeDef.IsPrivate))
                    {
                        result = true;
                    }

                    if (typeAccessibility.Value.HasFlag(TypeAccessibility.Public) && (typeDef == null || typeDef.IsPublic || typeDef.IsInternal))
                    {
                        result = true;
                    }

                    if (typeAccessibility.Value.HasFlag(TypeAccessibility.None))
                    {
                        continue;
                    }
                }

                if (!string.IsNullOrEmpty(filter))
                {
                    var fullName = type.FullName;
                    var parts = filter.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var part in parts)
                    {
                        string pattern;
                        bool exclude = part.StartsWith("!");

                        if (part == "this")
                        {
                            result = !exclude && thisAssembly;
                        }
                        else
                        {
                            if (part.StartsWith("regex:"))
                            {
                                pattern = part.Substring(6);
                            }
                            else
                            {
                                pattern = "^" + Regex.Escape(part).Replace("\\*", ".*").Replace("\\?", ".") + "$";
                            }

                            if (Regex.IsMatch(fullName, pattern))
                            {
                                result = !exclude;
                            }
                        }
                    }

                    if (!result)
                    {
                        continue;
                    }
                }

                if (result)
                {
                    reflectTypes.Add(type);
                }
            }

            return reflectTypes.ToArray();
        }
    }
}