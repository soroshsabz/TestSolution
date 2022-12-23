using System;
using System.Net.Http;
using System.Text;
using System.Web.Mvc;

namespace BSN.Resa.Commons
{
	public static class LogTool
    {
        public static string FormatActionLog(
            string controller, 
            string action,
            string url = null,
            string method = null,
            string data = null,
            string fromIP = null,
            string description = null)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine().Append("Controller: ").Append(controller.NullableToString());
            builder.AppendLine().Append("Action: ").Append(action.NullableToString());
            builder.AppendLine().Append("Url: ").Append(url.NullableToString());
            builder.AppendLine().Append("Method: ").Append(method.NullableToString());
            builder.AppendLine().Append("Data: ").Append(data.NullableToString());
            builder.AppendLine().Append("IP: ").Append(fromIP.NullableToString());
            builder.AppendLine().Append("Description: ").Append(description.NullableToString());
            return builder.ToString();
        }

        public static string ActionLog(Controller controller, string description)
        {
            return FormatActionLog(controller.ControllerContext.RouteData.Values["controller"].ToString(),
                                    controller.ControllerContext.RouteData.Values["action"].ToString(),
                                    controller.Request.Url?.ToString(),
                                    controller.Request.HttpMethod,
                                    controller.Request.ReceivedData(),
                                    controller.Request.UserHostAddress,
                                    description);

        }
        public static string RequestLog(Controller controller, HttpResponseMessage response, string description)
        {
            var builder = new StringBuilder();
            builder.Append(ActionLog(controller, description));
			HttpContent sentContent = response.RequestMessage.Content ?? new StringContent("");
			HttpContent receivedContent = response.Content ?? new StringContent("");
			
			builder.AppendLine().Append("Request Log:");
            builder.AppendLine().Append("Status Code: ").Append(ObjectExtension.NullableToString(response.StatusCode));
			builder.AppendLine().Append("Received Content: ").Append(ObjectExtension.NullableToString(receivedContent.ReadAsStringAsync().Result));
            builder.AppendLine().Append("Request Method: ").Append(ObjectExtension.NullableToString(response.RequestMessage.Method));
            builder.AppendLine().Append("Request Uri: ").Append(ObjectExtension.NullableToString(response.RequestMessage.RequestUri));
	        try
	        {
				builder.AppendLine().Append("Request Content: ").Append(ObjectExtension.NullableToString(sentContent.ReadAsStringAsync().Result));
			}
			catch (ObjectDisposedException objectDisposedException)
	        {
				builder.AppendLine().Append("Request Content: ").Append(objectDisposedException.Message);
			}
			return builder.ToString();
        }
    }
}