using System;
using System.Xml.Linq;
using Grynwald.XmlDocs;

namespace Cake.GitLab.Generator;

internal static class XmlDocumentationSerializer
{
    private class Visitor(SourceCodeBuilder output) : IDocumentationVisitor
    {
        public void Visit(DocumentationFile documentationFile) => throw new NotImplementedException();

        public void Visit(NamespaceMemberElement member) => throw new NotImplementedException();

        public void Visit(TypeMemberElement member) => throw new NotImplementedException();

        public void Visit(FieldMemberElement member) => throw new NotImplementedException();

        public void Visit(PropertyMemberElement member) => throw new NotImplementedException();

        public void Visit(EventMemberElement member) => throw new NotImplementedException();

        public void Visit(MethodMemberElement member)
        {
            member.Summary?.Accept(this);
            member.Remarks?.Accept(this);
            foreach (var parameter in member.Parameters)
            {
                parameter.Accept(this);
            }
            foreach (var typeParameter in member.TypeParameters)
            {
                typeParameter.Accept(this);
            }
            member.Returns?.Accept(this);
            foreach (var exception in member.Exceptions)
            {
                exception.Accept(this);
            }
            member.Example?.Accept(this);
            foreach (var seeAlso in member.SeeAlso)
            {
                seeAlso?.Accept(this);
            }
        }

        public void Visit(ParameterElement param)
        {
            output.BeginLine();
            output.AppendLine(@$"/// <param name=""{XmlEscape(param.Name)}"">");

            param.Text?.Accept(this);

            output.BeginLine();
            output.AppendLine("/// </param>");
        }

        public void Visit(TypeParameterElement typeParam)
        {
            output.BeginLine();
            output.AppendLine(@$"/// <typeparam name=""{XmlEscape(typeParam.Name)}"">");

            typeParam.Text?.Accept(this);

            output.BeginLine();
            output.AppendLine("/// </typeparam>");
        }

        public void Visit(ExceptionElement exception)
        {
            output.BeginLine();
            output.AppendLine(@$"/// <exception name=""{XmlEscape(exception.Reference.ToString())}"">");

            exception.Text?.Accept(this);

            output.BeginLine();
            output.AppendLine("/// </exception>");
        }

        public void Visit(SeeAlsoUrlReferenceElement seeAlso)
        {
            output.BeginLine();
            output.AppendLine(@$"/// <seealso href=""{XmlEscape(seeAlso.Link)}"">");

            seeAlso.Text?.Accept(this);

            output.BeginLine();
            output.AppendLine("/// </seealso>");
        }

        public void Visit(SeeAlsoCodeReferenceElement seeAlso)
        {
            output.BeginLine();
            output.AppendLine(@$"/// <seealso cref=""{XmlEscape(seeAlso.Reference.ToString())}"">");

            seeAlso.Text?.Accept(this);

            output.BeginLine();
            output.AppendLine("/// </seealso>");
        }

        public void Visit(SummaryElement summary)
        {
            output.BeginLine();
            output.AppendLine("/// <summary>");

            summary.Text?.Accept(this);

            output.BeginLine();
            output.AppendLine("/// </summary>");
        }

        public void Visit(RemarksElement remarks)
        {
            output.BeginLine();
            output.AppendLine("/// <remarks>");

            remarks.Text?.Accept(this);

            output.BeginLine();
            output.AppendLine("/// </remarks>");
        }

        public void Visit(ExampleElement example)
        {
            output.BeginLine();
            output.AppendLine("/// <example>");

            example.Text?.Accept(this);

            output.BeginLine();
            output.AppendLine("/// </example>");
        }

        public void Visit(ValueElement value) => throw new NotImplementedException();

        public void Visit(ReturnsElement returns)
        {
            output.BeginLine();
            output.AppendLine("/// <returns>");

            returns.Text?.Accept(this);

            output.BeginLine();
            output.AppendLine("/// </returns>");
        }

        public void Visit(UnrecognizedSectionElement unrecognizedElement)
        { }

        public void Visit(TextBlock textBlock)
        {
            output.BeginLine();
            output.Append("/// ");
            foreach (var element in textBlock.Elements)
            {
                element.Accept(this);
            }
            output.EndLine();
        }

        public void Visit(PlainTextElement plainText)
        {
            output.Append(XmlEscape(plainText.Content));
        }

        public void Visit(BulletedListElement bulletedList)
        {
            output.BeginLine();
            output.AppendLine(@"/// <list  type=""bullet"">");

            foreach (var listItem in bulletedList.Items)
            {
                listItem.Accept(this);
            }

            output.BeginLine();
            output.AppendLine("/// </list>");
        }

        public void Visit(NumberedListElement numberedList)
        {
            output.BeginLine();
            output.AppendLine(@"/// <list  type=""number"">");

            foreach (var listItem in numberedList.Items)
            {
                listItem.Accept(this);
            }

            output.BeginLine();
            output.AppendLine("/// </list>");
        }

        public void Visit(TableElement table) => throw new NotImplementedException();

        public void Visit(DefinitionListItem item) => throw new NotImplementedException();

        public void Visit(SimpleListItem simpleListItem) => throw new NotImplementedException();

        public void Visit(TableRow tableRow) => throw new NotImplementedException();

        public void Visit(CElement c)
        {
            output.Append(new XElement("c", c.Content).ToString());
        }

        public void Visit(CodeElement code)
        {
            output.BeginLine();
            if (code.Language is not null)
            {
                output.AppendLine(@$"/// <code  language=""{code.Language}"">");
            }
            else
            {
                output.AppendLine(@$"/// <code>");
            }

            foreach (var line in code.Content.Split(["\r\n", "\n"], StringSplitOptions.None))
            {
                output.BeginLine();
                output.AppendLine(line);
            }

            output.BeginLine();
            output.AppendLine("/// </code>");
        }

        public void Visit(ParagraphElement para)
        {
            output.BeginLine();
            output.AppendLine("/// <para>");

            para.Content?.Accept(this);

            output.BeginLine();
            output.AppendLine("/// </para>");
        }

        public void Visit(ParameterReferenceElement paramRef)
        {
            output.Append(new XElement("paramref", new XAttribute("name", paramRef.Name)).ToString());
        }

        public void Visit(TypeParameterReferenceElement typeParamRef)
        {
            output.Append(new XElement("typeparamref", new XAttribute("name", typeParamRef.Name)).ToString());
        }

        public void Visit(SeeCodeReferenceElement see)
        {
            output.Append(new XElement("typeparamref", new XAttribute("cref", see.Reference.ToString())).ToString());
        }

        public void Visit(SeeUrlReferenceElement see)
        {
            output.Append(new XElement("typeparamref", new XAttribute("href", see.Link)).ToString());
        }

        public void Visit(UnrecognizedTextElement unrecognizedElement)
        { }

        public void Visit(EmphasisElement emphasis)
        {
            output.Append(new XElement("em", emphasis.Content).ToString());
        }

        public void Visit(IdiomaticElement idiomatic)
        {
            output.Append(new XElement("i", idiomatic.Content).ToString());
        }

        public void Visit(BoldElement bold)
        {
            output.Append(new XElement("b", bold.Content).ToString());
        }

        public void Visit(StrongElement strong)
        {
            output.Append(new XElement("strong", strong.Content).ToString());
        }

        public void Visit(LineBreakElement lineBreakElement)
        {
            output.Append("<br />");
        }

        private string XmlEscape(string value) => new XText(value).ToString();
    }

    public static void WriteDocumentation(MemberElement memberDocumentation, SourceCodeBuilder output)
    {
        var visitor = new Visitor(output);
        memberDocumentation.Accept(visitor);
    }
}
