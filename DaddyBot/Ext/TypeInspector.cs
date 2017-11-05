using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Daddy.Ext
{
    public struct FieldDescription
    {
        public string Name;
        public string Value;
        public string Type;
        public bool isValid;
    }

    public class TypeInspector
    {
        public static FieldDescription VisualizeMethod(MethodInfo methodInfo, Dictionary<string, FieldDescription> acc = null, bool noStatics = true)
        {
            var sb = new StringBuilder(methodInfo.Name);
            sb.Append("(");
            sb.Append(String.Join(", ", methodInfo.GetParameters().Select(paramInfo =>
            {
                var opt = (paramInfo.HasDefaultValue) ? "?" : "";
                var strtype = VisualizeType(paramInfo.ParameterType.GetTypeInfo());
                return $"{strtype} {paramInfo.Name}{opt}";
            })));
            sb.Append(")");
            var desc = new FieldDescription
            {
                Type = VisualizeType(methodInfo.ReturnType.GetTypeInfo()),
                isValid = !(methodInfo.IsConstructor || methodInfo.IsSpecialName || (methodInfo.IsStatic && noStatics) || (!noStatics && !methodInfo.IsStatic)),
                Name = sb.ToString()
            };
            if (desc.isValid && acc != null && (!acc.ContainsKey(methodInfo.Name) || desc.Name.Length > acc[methodInfo.Name].Name.Length))
            {
                acc[methodInfo.Name] = desc;
            }
            return desc;
        }

        public static string VisualizeMethods(object data, bool noStatics = true, string findMethod = null)
        {
            if (data == null) return "[?]";
            var type = data as TypeInfo;

            if (type == null)
            {
                type = data.GetType().GetTypeInfo();
            }
            if (!(type.IsInterface || type.IsClass || type.IsAnsiClass || type.IsUnicodeClass))
            {
                return "[?]";
            }

            var methodTypes = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            IEnumerable<FieldDescription> fieldDescription;
            if (findMethod != null)
            {
                fieldDescription = methodTypes.Where(x => x.Name.ToLowerInvariant().Contains(findMethod.ToLowerInvariant())).Select(methodInfo => VisualizeMethod(methodInfo, noStatics: noStatics)).Where(x => x.isValid);
            }
            else
            {
                fieldDescription = methodTypes.Aggregate(new Dictionary<string, FieldDescription>(), (acc, methodInfo) => { VisualizeMethod(methodInfo, acc, noStatics); return acc; }).Select(x => x.Value);
            }

            return VisualizeFieldDescription(fieldDescription, false, methods: true);
        }

        public static string VisualizeEvents(object data)
        {
            if (data == null) return "[?]";
            var type = data as TypeInfo;
            if (type == null)
            {
                type = data.GetType().GetTypeInfo();
            }
            if (!(type.IsInterface || type.IsClass || type.IsAnsiClass || type.IsUnicodeClass))
            {
                return "[?]";
            }
            var events = type.GetEvents(BindingFlags.Public | BindingFlags.Instance).Select(x =>
            {
                return new FieldDescription()
                {
                    Name = $"Event:{x.Name}",
                    Value = null,
                    Type = VisualizeType(x.EventHandlerType.GetTypeInfo()),
                    isValid = true
                };
            });
            return VisualizeFieldDescription(events, noHeader: true);
        }

        public static string VisualizeMember(object data, bool withType = true, string findMember = null)
        {
            var type = data as TypeInfo;
            if (type == null)
            {
                type = data.GetType().GetTypeInfo();
            }
            //var type = data.GetType().GetTypeInfo();
            if (!(type.IsInterface || type.IsClass || type.IsAnsiClass || type.IsUnicodeClass))
            {
                return "[?]";
            }

            var memberTypes = type.GetMembers(BindingFlags.Public | BindingFlags.Instance | ((type.IsEnum) ? BindingFlags.Static : BindingFlags.Default));
            var tmember = memberTypes.Where(x =>
                {
                    return (x.MemberType & (MemberTypes.Field | MemberTypes.Property)) != 0 &&
                        (findMember == null || x.Name.ToLowerInvariant().Contains(findMember.ToLowerInvariant()));
                });
            var member = tmember.Select(memberInfo =>
            {
                var field = new FieldDescription();
                field.Name = memberInfo.Name;
                object fieldValue = null;
                TypeInfo fieldType = null;
                if (memberInfo is FieldInfo fieldInfo)
                {
                    try
                    {
                        fieldValue = fieldInfo.GetValue(data);
                    }
                    catch (ArgumentException e)
                    {
                        Main.Daddy.log("Invalid argument", exception: e).GetAwaiter().GetResult();
                    }
                    if (fieldValue == null)
                    {
                        fieldType = fieldInfo.FieldType.GetTypeInfo();
                    }
                }
                if (memberInfo is PropertyInfo propertyInfo)
                {
                    if (propertyInfo.GetIndexParameters().Count() <= 0)
                    {
                        //InvalidOperationException
                        try
                        {
                            if (propertyInfo.PropertyType.IsGenericParameter || propertyInfo.GetMethod.ContainsGenericParameters)
                            {
                                fieldValue = null;
                                fieldType = propertyInfo.PropertyType.GetTypeInfo();
                            }
                            else
                            {
                                fieldValue = propertyInfo.GetValue(data);
                            }
                        }
                        catch (TargetInvocationException e)
                        {
                            Main.Daddy.log("Invalid operation", exception: e).GetAwaiter().GetResult();
                        }
                        catch (TargetException e)
                        {
                            Main.Daddy.log("Invalid object", exception: e).GetAwaiter().GetResult();
                        }
                        if (fieldValue == null)
                        {
                            fieldType = propertyInfo.PropertyType.GetTypeInfo();
                        }
                    }
                    else
                    {
                        fieldValue = "[…skipped…]";
                        fieldType = propertyInfo.PropertyType.GetTypeInfo();
                    }
                }
                if (fieldValue != null && fieldType == null)
                {
                    fieldType = fieldValue.GetType().GetTypeInfo(); //-- real type
                }
                field.Type = VisualizeType(fieldType);
                field.Value = VisualizeValue(fieldType, fieldValue, withType: withType, findMember: findMember);
                field.isValid = true;
                return field;
            }).Where(x => x.isValid);
            if (member.Count() > 0)
            {
                return VisualizeFieldDescription(member, withType);
            }
            return VisualizeFallback(data, withType);
        }

        public static string VisualizeType(object data)
        {
            if (data == null)
            {
                return "?";
            }
            var type = data as TypeInfo;
            if (type == null)
            {
                type = data.GetType().GetTypeInfo();
            }
            return VisualizeType(type);
        }

        public static string VisualizeValue(object data, bool withType = true, string findMember = null)
        {
            if (data == null)
            {
                return "";
            }
            var type = data as TypeInfo;
            if (type == null)
            {
                type = data.GetType().GetTypeInfo();
            }
            return VisualizeValue(type, data, true, withType, findMember: findMember);
        }

        // ----
        private static string VisualizeFallback(object data, bool withType = true)
        {
            if (data == null)
            {
                return "";
            }
            var value = data.ToString().Replace("\n", "\\n").Replace("\r", "\\r");
            var length = value.Length;
            var limit = withType ? 50 : 20;
            if (length > limit)
            {
                value = $"{value.Substring(0, limit - 1)}…";
            }
            return value;
        }

        private static string VisualizeValue(TypeInfo type, object data, bool recurse = false, bool withType = true, string findMember = null)
        {
            if (type.IsArray)
            {
                var array = data as IEnumerable<object>;
                return VisualizeEnumerate(array);
            }

            if (type.IsGenericType)
            {
                if (data is IEnumerable<object> enumerable)
                {
                    return VisualizeEnumerate(enumerable);
                }
            }
            //if (type.IsPrimitive) return VisualizeFallback(data);
            if (recurse)
            {
                return VisualizeMember(data, withType, findMember);
            }
            return VisualizeFallback(data, withType);

            // Helper
            string VisualizeEnumerate(IEnumerable<object> array)
            {
                if (array == null) return VisualizeValue(array, findMember: findMember);
                var length = array.Count();
                if (recurse)
                {
                    var sb = new StringBuilder("[\n");
                    var elems = array.Take(20);
                    elems.Select(x => VisualizeValue(x.GetType().GetTypeInfo(), x, withType: withType, findMember: findMember)).Aggregate(sb, (acc, x) => acc.AppendFormat("    {0}\n", x));
                    var inlineCount = elems.Count();
                    if (length - inlineCount > 0)
                    {
                        sb.AppendFormat("    …[+{0} more]\n", length - inlineCount);
                    }
                    sb.AppendLine("]");
                    return sb.ToString();
                }
                else
                {
                    return $"[Count = {length}]";
                }
            }
        }

        private static string VisualizeType(TypeInfo type)
        {
            if (type.IsArray)
            {
                var dim = type.GetArrayRank();
                var itype = type.GetElementType();
                return VisualizeType(itype.GetTypeInfo()) + $"[{dim}]";
            }

            if (type.IsGenericType)
            {
                var idx = type.Name.LastIndexOf('`');
                var name = type.Name.Substring(0, idx > 0 ? idx : type.Name.Length);
                var generics = String.Join(", ", type.GenericTypeArguments.Select(x => VisualizeType(x.GetTypeInfo())));
                return $"{name}<{generics}>";
            }

            if (type.IsInterface || type.IsClass || type.IsAnsiClass || type.IsUnicodeClass)
            {
                return type.Name;
            }

            return type.Name;
        }

        private static string VisualizeFieldDescription(IEnumerable<FieldDescription> table, bool withType = true, bool methods = false, bool noHeader = false)
        {
            var sb = new StringBuilder();
            if (table.Count() <= 0)
            {
                return "";
            }
            var nameLength = table.Max(x => x.Name.Length) + 2;
            var typeLength = Math.Max(11, table.Max(x => x.Type.Length)) + 2;
            sb.AppendLine();
            var format = "";
            if (methods)
            {
                format = $"{{0,-{typeLength}}}{{2}}\n";
                if (!noHeader) sb.AppendFormat(format, "Return type", "", "Signature");
            }
            else
            {
                format = (withType) ? $"{{0,-{nameLength}}}{{1,-{typeLength}}}{{2}}\n" : $"{{0,-{nameLength}}}{{2,-20}}\n";
                if (!noHeader) sb.AppendFormat(format, "Name", "Type", "Value");
            }

            table.OrderBy(x => x.Name).Aggregate(sb, (acc, x) =>
            {
                if (methods)
                {
                    acc.AppendFormat(format, x.Type, x.Value, x.Name);
                }
                else
                {
                    acc.AppendFormat(format, x.Name, x.Type, x.Value);
                }
                return acc;
            });
            return sb.ToString();
        }
    }
}
