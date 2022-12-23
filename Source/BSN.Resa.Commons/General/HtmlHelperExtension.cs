using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Resources;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace BSN.Resa.Commons
{
    public static class HtmlHelperExtension
	{
		public static MvcHtmlString LabelForField<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TValue>> expression, bool showAsteriskOnRequired = true, bool showColonAtEnd = true, object htmlAttributes = null)
		{
			var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
			string resolvedLabelText = metadata.DisplayName ?? metadata.PropertyName;

			if (metadata.IsRequired && showAsteriskOnRequired && (htmlHelper.ViewBag.IsReadOnly == null || htmlHelper.ViewBag.IsReadOnly.Equals(false)))
				resolvedLabelText += "*";

			if(showColonAtEnd)
				resolvedLabelText += ":";

			return htmlHelper.LabelFor(expression, resolvedLabelText, htmlAttributes);
		}

		public static MvcHtmlString LabelForField(this HtmlHelper htmlHelper,
			string expression, string labelText, bool showColonAtEnd = true, object htmlAttributes = null)
		{
			string resolvedLabelText = labelText;

			if (showColonAtEnd)
				resolvedLabelText += ":";

			return htmlHelper.Label(expression, resolvedLabelText, htmlAttributes);
		}

        public static MvcHtmlString TextBoxForField<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
        {
            IDictionary<string, object> newHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            if (htmlHelper.ViewBag.IsReadOnly != null && htmlHelper.ViewBag.IsReadOnly.Equals(true))
                newHtmlAttributes["readonly"] = "readonly";

            return htmlHelper.TextBoxFor(expression, newHtmlAttributes);
        }

        public static MvcHtmlString DateBoxForField<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
        {
            IDictionary<string, object> newHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            if (htmlHelper.ViewBag.IsReadOnly != null && htmlHelper.ViewBag.IsReadOnly.Equals(true))
                newHtmlAttributes["readonly"] = "readonly";
            newHtmlAttributes.Add("type", "date");

            return htmlHelper.TextBoxFor(expression, "{0:yyyy-MM-dd}", newHtmlAttributes);
        }

        public static MvcHtmlString DateTimeBoxForField<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
        {
            IDictionary<string, object> newHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            if (htmlHelper.ViewBag.IsReadOnly != null && htmlHelper.ViewBag.IsReadOnly.Equals(true))
                newHtmlAttributes["readonly"] = "readonly";
            newHtmlAttributes.Add("type", "datetime-local");

            return htmlHelper.TextBoxFor(expression, "{0:yyyy-MM-dd\\Thh:mm}", newHtmlAttributes);
        }

        // create partial view with complex type and filled it correctly when submit form
        // info : https://stackoverflow.com/questions/29808573/getting-the-values-from-a-nested-complex-object-that-is-passed-to-a-partial-view
        public static MvcHtmlString PartialFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, string partialViewName)
        {
            string name = ExpressionHelper.GetExpressionText(expression);
            object model = ModelMetadata.FromLambdaExpression(expression, helper.ViewData).Model;
            var viewData = new ViewDataDictionary(helper.ViewData)
            {
                TemplateInfo = new System.Web.Mvc.TemplateInfo
                {
                    HtmlFieldPrefix = string.IsNullOrEmpty(helper.ViewData.TemplateInfo.HtmlFieldPrefix) ?
                        name : $"{helper.ViewData.TemplateInfo.HtmlFieldPrefix}.{name}"
                }
            };
            return helper.Partial(partialViewName, model, viewData);
        }

        public static string SelectIf(this HtmlHelper htmlHelper, bool? condition)
        {
	        return ShowIf(htmlHelper, "selected", condition);
		}

		public static string ShowIf(this HtmlHelper htmlHelper, string value, bool? condition)
		{
			if(condition != null && condition.Value)
				return value;

			return string.Empty;
		}

		public static MvcHtmlString EnumDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TProperty>> expression, ResourceManager resourceManager, object htmlAttributes) where TModel : class
		{
			IDictionary<string, object> newHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            if (htmlHelper.ViewBag.IsReadOnly != null && htmlHelper.ViewBag.IsReadOnly.Equals(true))
                newHtmlAttributes["readonly"] = "readonly";
            ProcessDisabledAttribute(newHtmlAttributes);

            string inputName = GetInputName(expression);
            var value = htmlHelper.ViewData.Model == null ? default(TProperty) : expression.Compile()(htmlHelper.ViewData.Model);

            return htmlHelper.DropDownList(inputName, ToSelectList(typeof(TProperty), value.ToString(), resourceManager), newHtmlAttributes);
		}

        public static MvcHtmlString BooleanDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, ResourceManager resourceManager, object htmlAttributes) where TModel : class
        {
            IDictionary<string, object> newHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            if (htmlHelper.ViewBag.IsReadOnly != null && htmlHelper.ViewBag.IsReadOnly.Equals(true))
                newHtmlAttributes["readonly"] = "readonly";
            ProcessDisabledAttribute(newHtmlAttributes);

            string inputName = GetInputName(expression);
            var value = htmlHelper.ViewData.Model == null ? default(TProperty) : expression.Compile()(htmlHelper.ViewData.Model);

            var selectListItems = new List<SelectListItem>()
            {
                new SelectListItem() { Value = "true", Text = resourceManager.GetString("True"), Selected = (value?.ToString() == "True") },
                new SelectListItem() { Value = "false", Text = resourceManager.GetString("False"), Selected = (value?.ToString() == "False") }
            };
            var selectList = new SelectList(selectListItems, "Value", "Text", value?.ToString());
            
            return htmlHelper.DropDownList(inputName, selectList, newHtmlAttributes);
        }

        public static MvcHtmlString EnumFlagsDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, ResourceManager resourceManager, object htmlAttributes) where TModel : class
        {
            IDictionary<string, object> newHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            if (htmlHelper.ViewBag.IsReadOnly != null && htmlHelper.ViewBag.IsReadOnly.Equals(true))
                newHtmlAttributes["readonly"] = "readonly";
            ProcessDisabledAttribute(newHtmlAttributes);
            newHtmlAttributes.Add("multiple", true);

            string inputName = GetInputName(expression);
            var value = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData).Model;

            var select = htmlHelper.DropDownList(inputName, ToSelectList(typeof(TProperty), null, resourceManager), newHtmlAttributes).ToHtmlString();
            select = select.Replace(" selected=\"selected\"", "");

            foreach (var item in Enum.GetValues(typeof(TProperty)).Cast<TProperty>())
            {
                if ((Convert.ToInt64(item) & Convert.ToInt64(value)) == Convert.ToInt64(item))
                {
                    string option = $"option value=\"{Convert.ToInt64(item)}\"";
                    int optionIndex = select.IndexOf(option, StringComparison.Ordinal);
                    select = select.Insert(optionIndex + option.Length, " selected=\"selected\"");
                }
            }
            
            return new MvcHtmlString(select);
        }

        public static string GetInputName<TModel, TProperty>(Expression<Func<TModel, TProperty>> expression)
		{
			if (expression.Body.NodeType == ExpressionType.Call)
			{
				var methodCallExpression = (MethodCallExpression)expression.Body;
				string name = GetInputName(methodCallExpression);
				return name.Substring(expression.Parameters[0].Name.Length + 1);
			}

			return expression.Body.ToString().Substring(expression.Parameters[0].Name.Length + 1);
		}

		private static string GetInputName(MethodCallExpression expression)
		{
			var methodCallExpression = expression.Object as MethodCallExpression;
			if (methodCallExpression != null)
				return GetInputName(methodCallExpression);

			return expression.Object.ToString();
		}

	    private static void ProcessDisabledAttribute(IDictionary<string, object> attributes)
	    {
            if (attributes.ContainsKey("disabled"))
            {
                if ((bool)attributes["disabled"])
                    attributes["disabled"] = "disabled";
                else
                    attributes.Remove("disabled");
            }
	    }

		public static SelectList ToSelectList(Type enumType, string selectedItem, ResourceManager resourceManager)
		{
            var items = new List<SelectListItem>();

            Type underlyingenumType = Nullable.GetUnderlyingType(enumType);
            if (underlyingenumType != null)
            {
                enumType = underlyingenumType;
                items.Add(new SelectListItem
                {
                    Value = "",
                    Text = "-",
                    Selected = false
                });
            }

            foreach (var item in Enum.GetValues(enumType))
			{
                var attribute = enumType.GetField(item.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();
				var listItem = new SelectListItem
				{
					Value = ((int)item).ToString(),
					Text = resourceManager.GetString(attribute == null ? item.ToString() : ((DescriptionAttribute)attribute).Description),
					Selected = selectedItem == ((int)item).ToString()
				};
                items.Add(listItem);
			}

            return new SelectList(items, "Value", "Text", selectedItem);
		}
    }
}
