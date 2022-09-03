using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Swagger.Configure
{

    #region conf
    public class EnumSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.IsEnum)
            {
                var array = new OpenApiArray();
                array.AddRange(Enum.GetNames(context.Type).Select(n => new OpenApiString(n)));
                // NSwag
                schema.Extensions.Add("x-enumNames", array);
                // Openapi-generator
                schema.Extensions.Add("x-enum-varnames", array);
            }
        }
    }

    public class DescribeEnumMembers : ISchemaFilter
    {
        private readonly XDocument mXmlComments;

        /// <summary>
        /// Initialize schema filter.
        /// </summary>
        /// <param name="argXmlComments">Document containing XML docs for enum members.</param>
        public DescribeEnumMembers(XDocument argXmlComments)
          => mXmlComments = argXmlComments;

        /// <summary>
        /// Apply this schema filter.
        /// </summary>
        /// <param name="argSchema">Target schema object.</param>
        /// <param name="argContext">Schema filter context.</param>
        public void Apply(OpenApiSchema argSchema, SchemaFilterContext argContext)
        {
            var EnumType = argContext.Type;

            if (!EnumType.IsEnum) return;

            var sb = new StringBuilder(argSchema.Description);
            var sbli = new StringBuilder();


            foreach (var EnumMemberName in Enum.GetNames(EnumType))
            {
                var FullEnumMemberName = $"F:{EnumType.FullName}.{EnumMemberName}";

                var EnumMemberDescription = mXmlComments.XPathEvaluate(
                  $"normalize-space(//member[@name = '{FullEnumMemberName}']/summary/text())"
                ) as string;

                var valueenum = (int)(Enum.Parse(EnumType, EnumMemberName));

                if (string.IsNullOrEmpty(EnumMemberDescription)) continue;

                sbli.AppendLine($"<li><b>{valueenum}-{EnumMemberName}</b>: {EnumMemberDescription}</li>");
            }

            if (sbli.Length > 0)
            {
                sb.AppendLine("<p>Possible values:</p>");
                sb.AppendLine("<ul>");
                sb.Append(sbli);
                sb.AppendLine("</ul>");
            }


            argSchema.Description = sb.ToString();
        }
    }



    public class EnumTypesDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var path in swaggerDoc.Paths.Values)
            {
                foreach (var operation in path.Operations.Values)
                {
                    foreach (var parameter in operation.Parameters)
                    {
                        var schemaReferenceId = parameter.Schema.Reference?.Id;

                        if (string.IsNullOrEmpty(schemaReferenceId)) continue;

                        var schema = context.SchemaRepository.Schemas[schemaReferenceId];

                        if (schema.Enum == null || schema.Enum.Count == 0) continue;

                        parameter.Description += "<p>Variants:</p>";

                        int cutStart = schema.Description.IndexOf("<ul>");

                        int cutEnd = schema.Description.IndexOf("</ul>") + 5;

                        parameter.Description += schema.Description
                            .Substring(cutStart, cutEnd - cutStart);
                    }
                }
            }
        }
    }

    #endregion
}
